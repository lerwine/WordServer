using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class IndexFile : Collection<IndexItem>
    {
        private static readonly Regex _commentRegex = new Regex(@"^\s+(?<n>\d+)\s*(?<t>.*?)$", RegexOptions.Compiled);
        private static readonly Regex _firstFourRegex = new Regex(@"^(?<lemma>\S+)\s+(?<pos>[nvar])\s+(?<synset_cnt>\d+)\s+(?<p_cnt>\d+)\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private static readonly Regex _pointerSymbolRegex = new Regex(@"^(?<[ptr_symbol>\S+)\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private static readonly Regex _countsRegex = new Regex(@"^(?<sense_cnt>\d+)\s+(?<tagsense_cnt>\d+)\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private static readonly Regex _offsetRegex = new Regex(@"^(?<synset_offset>\d+)\s+(?<r>\S?.*)$", RegexOptions.Compiled);

        private object _syncRoot = new object();
        private IndexItem[] _backup = new IndexItem[0];
        private Common.PartOfSpeech _partOfSpeech;
        private string _staticDbFolder;
        private string _userFriendlyPathSpec;
        private Task<bool> _reloadTask;
        private Exception _loadError = null;
        private int _linesProcessed = 0;
        private long _bytesProcessed = 0;
        private double _percentComplete = 0.0;

        public event EventHandler LoadFinished;

        public Common.PartOfSpeech PartOfSpeech
        {
            get
            {
                Common.PartOfSpeech result;
                lock (this._syncRoot)
                {
                    result = this._partOfSpeech;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._partOfSpeech = value;
                }
            }
        }

        public string StaticDbFolder
        {
            get
            {
                string result;
                lock (this._syncRoot)
                {
                    result = this._staticDbFolder;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._staticDbFolder = value;
                }
            }
        }

        public string UserFriendlyPathSpec
        {
            get
            {
                string result;
                lock (this._syncRoot)
                {
                    result = (this._userFriendlyPathSpec == null) ? this.StaticDbFolder : this._userFriendlyPathSpec;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._userFriendlyPathSpec = value;
                }
            }
        }

        public Task<bool> ReloadTask
        {
            get
            {
                Task<bool> result;
                lock (this._syncRoot)
                {
                    result = this._reloadTask;
                }
                return result;
            }
        }

        public bool IsLoadInProgress { get { return this.ReloadTask != null; } }

        public Exception LoadError
        {
            get
            {
                Exception result;
                lock (this._syncRoot)
                {
                    result = this._loadError;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._loadError = value;
                }
            }
        }

        public int LinesProcessed
        {
            get
            {
                int result;
                lock (this._syncRoot)
                {
                    result = this._linesProcessed;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._linesProcessed = value;
                }
            }
        }

        public long BytesProcessed
        {
            get
            {
                long result;
                lock (this._syncRoot)
                {
                    result = this._bytesProcessed;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._bytesProcessed = value;
                }
            }
        }

        public double PercentComplete
        {
            get
            {
                double result;
                lock (this._syncRoot)
                {
                    result = this._percentComplete;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._percentComplete = value;
                }
            }
        }

        public IndexFile(string staticDbFolder, Common.PartOfSpeech partOfSpeech) : this(staticDbFolder, partOfSpeech, null) { }

        public IndexFile(string staticDbFolder, Common.PartOfSpeech partOfSpeech, string userFriendlyPathSpec)
        {
            if (staticDbFolder == null)
                throw new ArgumentNullException("staticDbFolder");

            if (String.IsNullOrWhiteSpace(staticDbFolder))
                throw new ArgumentException("Folder path not provided", "staticDbFolder");

            this.StaticDbFolder = staticDbFolder;
            this.UserFriendlyPathSpec = userFriendlyPathSpec;
            this.PartOfSpeech = partOfSpeech;
            this.StartReloadAsync();
        }

        public static string PosToFileName(Common.PartOfSpeech partOfSpeech)
        {
            switch (partOfSpeech)
            {
                case Common.PartOfSpeech.Noun:
                    return "index.noun";
                case Common.PartOfSpeech.Verb:
                    return "index.verb";
                case Common.PartOfSpeech.Adjective:
                    return "index.adj";
                case Common.PartOfSpeech.Adverb:
                    return "index.adv";
            }

            throw new Exception(String.Format("Value of '{0}' is not supported", Enum.GetName(partOfSpeech.GetType(), partOfSpeech)));
        }

        public void MakeInMemoryBackup()
        {
            if (this.IsLoadInProgress)
                throw new InvalidOperationException("Load is in progress");

            lock (this._syncRoot)
            {
                this._backup = this.ToArray();
            }
        }

        public void RestoreFromInMemoryBackup()
        {
            if (this.IsLoadInProgress)
                throw new InvalidOperationException("Load is in progress");

            lock (this._syncRoot)
            {
                this.Clear();
                foreach (IndexItem item in this._backup)
                    this.Add(item);
            }
        }

        public async Task<bool> GetResultAsync()
        {
            Task<bool> task;
            lock (this._syncRoot)
            {
                task = (this._reloadTask == null) ? Task<bool>.FromResult(this._loadError == null) : this._reloadTask;
            }

            return await task;
        }

        public Task<bool> StartReloadAsync()
        {
            return this.ReloadAsync();
        }

        public async Task<bool> ReloadAsync()
        {
            Task<bool> task;
            bool isNewTask;

            lock (this._syncRoot)
            {
                isNewTask = (this._reloadTask == null);
                if (isNewTask)
                {
                    this._reloadTask = new Task<bool>(this._Reload);
                    this._reloadTask.Start();
                }

                task = this._reloadTask;
            }

            if (isNewTask)
                this.LoadError = null;

            bool result = await task;

            if (!isNewTask)
                return result;

            this.PercentComplete = 100.0;
            this.OnLoadFinished();
            this.LoadError = this._reloadTask.Exception;

            lock (this._syncRoot)
            {
                this._reloadTask = null;
            }

            return result;
        }

        private bool _Reload()
        {
            this.BytesProcessed = 0;
            this.PercentComplete = 0.0;
            this.LinesProcessed = 0;

            lock (this._syncRoot)
            {
                this.Clear();
            }

            using (StreamReader reader = File.OpenText(Path.Combine(this.StaticDbFolder, IndexFile.PosToFileName(this.PartOfSpeech))))
            {
                string currentLine = "";
                while (!reader.EndOfStream && String.IsNullOrWhiteSpace(currentLine) && IndexFile._commentRegex.IsMatch(currentLine))
                {
                    currentLine = reader.ReadLine();
                    this.BytesProcessed = reader.BaseStream.Position;
                    this.PercentComplete = Math.Round((Convert.ToDouble(reader.BaseStream.Position) / Convert.ToDouble(reader.BaseStream.Length)) * 100.0, 2);
                    this.LinesProcessed++;
                }

                if (!reader.EndOfStream)
                {
                    do
                    {
                        if (String.IsNullOrWhiteSpace(currentLine))
                            continue;

                        int position = 0;
                        Match m = IndexFile._firstFourRegex.Match(currentLine);
                        if (!m.Success)
                            throw new WordNetParseException("Error parsing first four index file fields", IndexFile._firstFourRegex, currentLine, position);
                        IndexItem item = new IndexItem
                        {
                            Word = m.Groups["lemma"].Value.Replace('_', ' '),
                            PartOfSpeech = Common.SymbolAttribute.GetEnum<Common.PartOfSpeech>(m.Groups["pos"].Value),
                            SynsetCount = Convert.ToInt32(m.Groups["synset_cnt"].Value),
                            PointerSymbols = new Collection<Common.PointerSymbol>(),
                            SynsetOffsets = new Collection<long>()
                        };

                        int p_cnt = Convert.ToInt32(m.Groups["p_cnt"].Value);

                        for (int i = 0; i < p_cnt; i++)
                        {
                            position += m.Groups["r"].Index;
                            m = IndexFile._pointerSymbolRegex.Match(m.Groups["r"].Value);
                            if (!m.Success)
                                throw new WordNetParseException(String.Format("Error parsing pointer symbol {0}", i + 1), IndexFile._pointerSymbolRegex, currentLine, position);
                            item.PointerSymbols.Add(Common.PosAndSymbolAttribute.GetEnum<Common.PointerSymbol>(m.Groups["ptr_symbol"].Value, item.PartOfSpeech));
                        }

                        position += m.Groups["r"].Index;
                        m = IndexFile._countsRegex.Match(m.Groups["r"].Value);
                        if (!m.Success)
                            throw new WordNetParseException("Error parsing counts", IndexFile._pointerSymbolRegex, currentLine, position);
                        int sense_cnt = Convert.ToInt32(m.Groups["sense_cnt"].Value);

                        for (int i = 0; i < sense_cnt; i++)
                        {
                            position += m.Groups["r"].Index;
                            m = IndexFile._offsetRegex.Match(m.Groups["r"].Value);
                            if (!m.Success)
                                throw new WordNetParseException(String.Format("Error parsing offset {0}", i + 1), IndexFile._offsetRegex, currentLine, position);
                            item.SynsetOffsets.Add(Convert.ToInt64(m.Groups["synset_offset"].Value));
                        }

                        lock (this._syncRoot)
                        {
                            this.Add(item);
                        }

                        currentLine = reader.ReadLine();
                        this.LinesProcessed++;
                        this.BytesProcessed = reader.BaseStream.Position;
                        this.PercentComplete = Math.Round((Convert.ToDouble(reader.BaseStream.Position) / Convert.ToDouble(reader.BaseStream.Length)) * 100.0, 2);
                    } while (!reader.EndOfStream);

                    this.BytesProcessed = reader.BaseStream.Length;
                }
            }

            return this.LoadError == null;
        }

        protected virtual void OnLoadFinished()
        {
            if (this.LoadFinished != null)
                this.LoadFinished(this, EventArgs.Empty);
        }
    }
}
