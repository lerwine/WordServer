using System;
using System.Collections.Generic;
using System.Configuration;
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
                    this.VerbIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Verb);
                    this.VerbStatus = new LoadStatus(this.VerbIndex);
                    this.AdjectiveIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Adjective);
                    this.AdjectiveStatus = new LoadStatus(this.AdjectiveIndex);
                    this.AdverbIndex = new WordNetDataReader.IndexFile(dbFolder, Common.PartOfSpeech.Adverb);
                    this.AdverbStatus = new LoadStatus(this.AdverbIndex);
                }
            }
        }

        public static Common.LongRunningJob<WordNetDataReader.IndexItem[]> LookupWordInstances(string word)
        {
            return new Common.LongRunningJob<WordNetDataReader.IndexItem[]>(WordLookupManager.Instance._LookupWordInstances(word));
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstances(string word)
        {
            if (this._applicationInstance.UseConnectedDatabase)
                return await this._LookupWordInstancesDb(word);

            return await this._LookupWordInstancesStatic(word);
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstancesStatic(string word)
        {
            if (String.IsNullOrWhiteSpace(word))
                return new WordNetDataReader.IndexItem[0];

            await this.NounIndex.GetResult();
            await this.VerbIndex.GetResult();
            await this.AdjectiveIndex.GetResult();
            await this.AdverbIndex.GetResult();

            string w = WordLookupManager._normalizeWordRegex.Replace(word.Trim(), " ").ToLower();

            return this.NounIndex.Where(i => i.lemma == w)
                .Concat(this.VerbIndex.Where(i => i.lemma == w))
                .Concat(this.AdjectiveIndex.Where(i => i.lemma == w))
                .Concat(this.AdverbIndex.Where(i => i.lemma == w)).ToArray();
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstancesDb(string word)
        {
            throw new NotImplementedException();
        }
    }
}