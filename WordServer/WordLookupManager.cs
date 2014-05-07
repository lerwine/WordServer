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
        public static WordLookupManager Instance { get { return (HttpContext.Current.ApplicationInstance as Global).CurrentWordLookupManager; } }

        protected WordNetDataReader.IndexFile NounIndex { get; private set; }
        protected WordNetDataReader.IndexFile VerbIndex { get; private set; }
        protected WordNetDataReader.IndexFile AdjectiveIndex { get; private set; }
        protected WordNetDataReader.IndexFile AdverbIndex { get; private set; }
        private object _syncRoot = new object();

        protected Task<bool> NounLoadTask { get; private set; }
        protected Task<bool> VerbLoadTask { get; private set; }
        protected Task<bool> AdjectiveLoadTask { get; private set; }
        protected Task<bool> AdverbLoadTask { get; private set; }

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
                if (this.NounLoadTask == null)
                {
                    this.NounIndex = new WordNetDataReader.IndexFile();
                    this.VerbIndex = new WordNetDataReader.IndexFile();
                    this.AdjectiveIndex = new WordNetDataReader.IndexFile();
                    this.AdverbIndex = new WordNetDataReader.IndexFile();

                    string dbFolder = this._applicationInstance.Server.MapPath("~/App_Data");
                    this.NounLoadTask = this.NounIndex.LoadAsync(dbFolder, Common.PartOfSpeech.Noun);
                    this.VerbLoadTask = this.VerbIndex.LoadAsync(dbFolder, Common.PartOfSpeech.Verb);
                    this.AdjectiveLoadTask = this.AdjectiveIndex.LoadAsync(dbFolder, Common.PartOfSpeech.Adjective);
                    this.AdverbLoadTask = this.AdverbIndex.LoadAsync(dbFolder, Common.PartOfSpeech.Adverb);
                }
            }
        }

        public static async Task<WordNetDataReader.IndexItem[]> LookupWordInstances(string word)
        {
            return await WordLookupManager.Instance._LookupWordInstances(word);
        }

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstances(string word)
        {
            bool isStatic;
            lock (this._syncRoot)
            {
                isStatic = this.NounLoadTask != null;
            }

            if (isStatic)
                return await this._LookupWordInstancesStatic(word);

            return await this._LookupWordInstancesDb(word);
        }

        public static readonly Regex _normalizeWordRegex = new Regex(@"\s{2,}", RegexOptions.Compiled);

        private async Task<WordNetDataReader.IndexItem[]> _LookupWordInstancesStatic(string word)
        {
            if (String.IsNullOrWhiteSpace(word))
                return new WordNetDataReader.IndexItem[0];

            bool shouldAwait;
            lock (this._syncRoot) { shouldAwait = this.NounIndex.LoadInProgress; }
            if (shouldAwait)
                await this.NounLoadTask;
            lock (this._syncRoot) { shouldAwait = this.VerbIndex.LoadInProgress; }
            if (shouldAwait)
                await this.VerbLoadTask;
            lock (this._syncRoot) { shouldAwait = this.AdjectiveIndex.LoadInProgress; }
            if (shouldAwait)
                await this.AdjectiveLoadTask;
            lock (this._syncRoot) { shouldAwait = this.AdverbIndex.LoadInProgress; }
            if (shouldAwait)
                await this.AdverbLoadTask;

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