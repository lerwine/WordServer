using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Erwine.Leonard.T.WordServer
{
    public class WordLookupManager
    {
        public class LoadStatus
        {
            private WordNetDataReader.IndexFile _indexFile;
            public bool LoadInProgress { get { return this._indexFile.IsLoadInProgress; } }
            public Exception LoadError { get { return this._indexFile.LoadError; } }
            public int LinesProcessed { get { return this._indexFile.LinesProcessed; } }
            public long BytesProcessed { get { return this._indexFile.BytesProcessed; } }
            public double PercentComplete { get { return this._indexFile.PercentComplete; } }
            public int Count { get { return this._indexFile.Count; } }

            public LoadStatus(WordNetDataReader.IndexFile indexFile)
            {
                this._indexFile = indexFile;
            }
        }

        public static readonly Regex _normalizeWordRegex = new Regex(@"\s{2,}", RegexOptions.Compiled);

        public static WordLookupManager Instance { get { return (HttpContext.Current.ApplicationInstance as Global).CurrentWordLookupManager; } }

        protected WordNetDataReader.IndexFile NounIndex { get; private set; }
        protected WordNetDataReader.IndexFile VerbIndex { get; private set; }
        protected WordNetDataReader.IndexFile AdjectiveIndex { get; private set; }
        protected WordNetDataReader.IndexFile AdverbIndex { get; private set; }

        protected WordNetDataReader.DataFile NounData { get; private set; }
        protected WordNetDataReader.DataFile VerbData { get; private set; }
        protected WordNetDataReader.DataFile AdjectiveData { get; private set; }
        protected WordNetDataReader.DataFile AdverbData { get; private set; }

        public LoadStatus NounStatus { get; private set; }
        public LoadStatus VerbStatus { get; private set; }
        public LoadStatus AdjectiveStatus { get; private set; }
        public LoadStatus AdverbStatus { get; private set; }

        private object _syncRoot = new object();

        private Global _applicationInstance = null;

        public WordLookupManager()
        {
            this._applicationInstance = HttpContext.Current.ApplicationInstance as Global;
        }

        public static void InitializeStaticDb()
        {
            WordLookupManager.Instance._InitializeStaticDb();
        }

        private void _InitializeStaticDb()
        {
            lock (this._syncRoot)
            {
                if (!this._applicationInstance.UseConnectedDatabase)
                {
                    string dbFolder = this._applicationInstance.Server.MapPath("~/App_Data");
                    this.NounIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Noun);
                    this.NounStatus = new LoadStatus(this.NounIndex);
                    this.NounData = new WordNetDataReader.DataFile(dbFolder, Common.PartOfSpeech.Noun);
                    this.VerbIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Verb);
                    this.VerbStatus = new LoadStatus(this.VerbIndex);
                    this.VerbData = new WordNetDataReader.DataFile(dbFolder, Common.PartOfSpeech.Verb);
                    this.AdjectiveIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Adjective);
                    this.AdjectiveStatus = new LoadStatus(this.AdjectiveIndex);
                    this.AdjectiveData = new WordNetDataReader.DataFile(dbFolder, Common.PartOfSpeech.Adjective);
                    this.AdverbIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Adverb);
                    this.AdverbStatus = new LoadStatus(this.AdverbIndex);
                    this.AdverbData = new WordNetDataReader.DataFile(dbFolder, Common.PartOfSpeech.Adverb);
                }
            }
        }

        public static Common.LongRunningJob<WordNetDataReader.IndexItem[]> CreateLookupWordInstancesJob(string word)
        {
            return new Common.LongRunningJob<WordNetDataReader.IndexItem[]>(WordLookupManager.Instance._LookupWordInstancesAsync(word));
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstancesAsync(string word)
        {
            if (this._applicationInstance.UseConnectedDatabase)
                return await this._LookupWordInstancesDbAsync(word);

            return await this._LookupWordInstancesStaticAsync(word);
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstancesStaticAsync(string word)
        {
            if (String.IsNullOrWhiteSpace(word))
                return new WordNetDataReader.IndexItem[0];

            await this.NounIndex.GetResultAsync();
            await this.VerbIndex.GetResultAsync();
            await this.AdjectiveIndex.GetResultAsync();
            await this.AdverbIndex.GetResultAsync();

            string w = WordLookupManager._normalizeWordRegex.Replace(word.Trim(), " ").ToLower();

            return this.NounIndex.Where(i => i.Word == w)
                .Concat(this.VerbIndex.Where(i => i.Word == w))
                .Concat(this.AdjectiveIndex.Where(i => i.Word == w))
                .Concat(this.AdverbIndex.Where(i => i.Word == w)).ToArray();
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstancesDbAsync(string word)
        {
            throw new NotImplementedException();
        }

        public static Common.LongRunningJob<WordNetDataReader.DataItem[]> CreateGetWordsJob(WordNetDataReader.IndexItem[] indexItems)
        {
            return new Common.LongRunningJob<WordNetDataReader.DataItem[]>(WordLookupManager.Instance._GetWordsAsync(indexItems));
        }

        private async Task<WordNetDataReader.DataItem[]> _GetWordsAsync(WordNetDataReader.IndexItem[] indexItems)
        {
            if (this._applicationInstance.UseConnectedDatabase)
                return await this._GetWordsDbAsync(indexItems);

            return await this._GetWordsStaticAsync(indexItems);
        }

        private Task<WordNetDataReader.DataItem[]> _GetWordsStaticAsync(WordNetDataReader.IndexItem[] indexItems)
        {
            Task<WordNetDataReader.DataItem[]> task = new Task<WordNetDataReader.DataItem[]>(() => indexItems.AsParallel().SelectMany(i =>
            {
                Task<WordNetDataReader.DataItem[]> t;
                switch (i.PartOfSpeech)
                {
                    case Common.PartOfSpeech.Noun:
                        t = this.NounData.GetWordsAsync(i.SynsetOffsets);
                        break;
                    case Common.PartOfSpeech.Verb:
                        t = this.VerbData.GetWordsAsync(i.SynsetOffsets);
                        break;
                    case Common.PartOfSpeech.Adjective:
                        t = this.AdjectiveData.GetWordsAsync(i.SynsetOffsets);
                        break;
                    case Common.PartOfSpeech.Adverb:
                        t = this.AdverbData.GetWordsAsync(i.SynsetOffsets);
                        break;
                    default:
                        return null;
                }
                t.Wait();
                return t.Result;
            }).ToArray());

            task.Start();

            return task;
        }

        private async Task<WordNetDataReader.DataItem[]> _GetWordsDbAsync(WordNetDataReader.IndexItem[] indexItems)
        {
            throw new NotImplementedException();
        }

    }
}