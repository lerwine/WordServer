using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace WordServer.WordNet
{
    public class WordNetRefresher
    {
        private readonly object _syncRoot = new object();
        private CancellationTokenSource _cts = null;
        private Worker _worker = null;

        public WordNetRefresher() { }

        public class Worker : IRegisteredObject
        {
            private readonly object _syncRoot = new object();
            private CancellationToken _token;
            private string _dataFolder = null;
            private WordNetModelContainer _container = null;
            private SourceDataReader _reader = null;
            private bool _shutDownInProgress = false;
            public bool ShutDownInProgress
            {
                get { return this._shutDownInProgress || (this._token != null && this._token.IsCancellationRequested); }
                set { this._shutDownInProgress = value; }
            }

            public void Stop(bool immediate)
            {
                lock (this._syncRoot)
                {
                    this.ShutDownInProgress = true;
                }

                if (immediate)
                    HostingEnvironment.UnregisterObject(this);
            }

            public void DoWork(object state)
            {
                try
                {
                    object[] values = state as object[];

                    this._dataFolder = values[0] as string;
                    this._token = (CancellationToken)(values[1]);
                    this._container = new WordNetModelContainer();
                    if (!this.ShutDownInProgress)
                    {
                        try
                        {
                            this.UpdateDatabase();
                        }
                        catch
                        {
                            throw;
                        }
                        finally
                        {
                            this._container.Dispose();
                        }
                    }
                }
                catch (Exception exc)
                {
                    // TODO: Report error
                }

                bool shutDownInProgress;
                lock (this._syncRoot)
                {
                    shutDownInProgress = this._shutDownInProgress;
                }

                if (!shutDownInProgress)
                    HostingEnvironment.UnregisterObject(this);
            }

            private void UpdateDatabase()
            {
                if (this.ShutDownInProgress)
                    return;

                this._container.SynsetPointers.RemoveRange(this._container.SynsetPointers);
                this._container.SynsetWords.RemoveRange(this._container.SynsetWords);

                if (this.ShutDownInProgress)
                    return;

                this._container.SaveChanges();

                if (this.ShutDownInProgress)
                    return;

                this._container.IndexWords.RemoveRange(this._container.IndexWords);
                this._container.Synsets.RemoveRange(this._container.Synsets);

                if (this.ShutDownInProgress)
                    return;

                this._container.SaveChanges();

                if (this.ShutDownInProgress)
                    return;

                // TODO: This is actually a folder path and not a file. Need to change logic to pull in each of the files
                this._reader = new SourceDataReader(this._dataFolder);

                try
                {
                    DataItem dataItem;
                    while ((dataItem = this._reader.ReadNext()) != null)
                    {
                        using (System.Data.Entity.DbContextTransaction tran = this._container.Database.BeginTransaction())
                        {
                            this.ProcessDataItem(dataItem);
                            if (this.ShutDownInProgress)
                                tran.Rollback();
                            else
                                tran.Commit();
                            // TODO: Make a way to save where we'd last left off
                        }
                    }
                    // TODO: Next, we need to position the pointer back at the beginning and add the SynsetPointers relationships
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (this._reader != null)
                        this._reader.Dispose();
                }
            }

            private void ProcessDataItem(DataItem dataItem)
            {
                Synset synset = this._container.Synsets.Create();
                synset.Id = Guid.NewGuid();
                synset.Offset = dataItem.SynsetOffset;
                synset.LexFileNum = dataItem.LexFileNum;
                switch (dataItem.SynsetType.ToLower())
                {
                    case "n":
                        synset.SynsetType = 0;
                        break;
                    case "v":
                        synset.SynsetType = 1;
                        break;
                    case "a":
                        synset.SynsetType = 2;
                        break;
                    case "s":
                        synset.SynsetType = 3;
                        break;
                    default:
                        synset.SynsetType = 4;
                        break;
                }

                synset.Definition = dataItem.Gloss;
                this._container.Synsets.Add(synset);
                this._container.SaveChanges();

                for (int position = 0; position < dataItem.Words.Length; position++)
                {
                    IndexWord word = this._container.IndexWords.FirstOrDefault(w => w.Word == dataItem.Words[position].Word);

                    if (word == null)
                    {
                        word = this._container.IndexWords.Create();
                        word.Id = Guid.NewGuid();
                        word.Word = dataItem.Words[position].Word;
                        this._container.IndexWords.Add(word);
                        this._container.SaveChanges();
                    }

                    SynsetWord synsetWord = this._container.SynsetWords.FirstOrDefault(s => s.SynsetId.Equals(synset.Id) && s.WordId.Equals(word.Id));
                    if (synsetWord == null)
                    {
                        synsetWord = this._container.SynsetWords.Create();
                        synsetWord.SynsetId = synset.Id;
                        synsetWord.WordId = word.Id;
                        synsetWord.LexId = dataItem.Words[position].LexId;
                        synsetWord.Position = (short)position;
                        this._container.SynsetWords.Add(synsetWord);
                        this._container.SaveChanges();
                    }
                }
            }
        }

        public void CancelAsync()
        {
            lock (this._syncRoot)
            {
                if (this._cts != null && !this._worker.ShutDownInProgress)
                    this._cts.Cancel();

                this._cts = null;
            }
        }

        public void Invoke(string dataFolder)
        {
            lock (this._syncRoot)
            {
                if (this._cts != null)
                    throw new InvalidOperationException("Background task has already been invoked");

                this._cts = new CancellationTokenSource();
            }

            this._worker = new Worker();
            HostingEnvironment.RegisterObject(this._worker);

            ThreadPool.QueueUserWorkItem(new WaitCallback(this._worker.DoWork), new object[] { dataFolder, this._cts.Token });
        }
    }
}