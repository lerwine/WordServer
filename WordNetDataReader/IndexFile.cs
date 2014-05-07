using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
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
        private Common.PartOfSpeech _partOfSpeech;
        private bool _loadInProgress = false;
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

        public bool LoadInProgress
        {
            get
            {
                bool result;
                lock (this._syncRoot)
                {
                    result = this._loadInProgress;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._loadInProgress = value;
                }
            }
        }

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

        public IndexFile() { }

        private IndexItem[] _backup = new IndexItem[0];

        public void MakeInMemoryBackup()
        {
            if (this.LoadInProgress)
                throw new InvalidOperationException("Load is in progress");

            lock (this._syncRoot)
            {
                this._backup = this.ToArray();
            }
        }

        public void RestoreFromInMemoryBackup()
        {
            if (this.LoadInProgress)
                throw new InvalidOperationException("Load is in progress");

            lock (this._syncRoot)
            {
                this.Clear();
                foreach (IndexItem item in this._backup)
                    this.Add(item);
            }
        }
        
        public async Task<bool> LoadAsync(string dbFolder, Common.PartOfSpeech partOfSpeech)
        {
            this.PartOfSpeech = partOfSpeech;
            if (dbFolder == null)
                throw new ArgumentNullException("dbFolder");

            this.LoadInProgress = true;
            this.LoadError = null;
            this.BytesProcessed = 0;
            this.PercentComplete = 0;
            this.LinesProcessed = 0;

            lock (this._syncRoot)
            {
                this.Clear();
            }

            try
            {
                using (StreamReader reader = File.OpenText(Path.Combine(dbFolder, IndexFile.PosToFileName(partOfSpeech))))
                {
                    string currentLine = "";
                    while (!reader.EndOfStream && String.IsNullOrWhiteSpace(currentLine) && IndexFile._commentRegex.IsMatch(currentLine))
                    {
                        currentLine = await reader.ReadLineAsync();
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
                                lemma = m.Groups["lemma"].Value.Replace('_', ' '),
                                pos = Common.SymbolAttribute.GetEnum<Common.PartOfSpeech>(m.Groups["pos"].Value),
                                synset_cnt = Convert.ToInt32(m.Groups["synset_cnt"].Value),
                                ptr_symbol = new Collection<Common.PointerSymbol>(),
                                synset_offset = new Collection<long>()
                            };

                            int p_cnt = Convert.ToInt32(m.Groups["p_cnt"].Value);

                            for (int i = 0; i < p_cnt; i++)
                            {
                                position += m.Groups["r"].Index;
                                m = IndexFile._pointerSymbolRegex.Match(m.Groups["r"].Value);
                                if (!m.Success)
                                    throw new WordNetParseException(String.Format("Error parsing pointer symbol {0}", i + 1), IndexFile._pointerSymbolRegex, currentLine, position);
                                item.ptr_symbol.Add(Common.PosAndSymbolAttribute.GetEnum<Common.PointerSymbol>(m.Groups["ptr_symbol"].Value, item.pos));
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
                                item.synset_offset.Add(Convert.ToInt64(m.Groups["synset_offset"].Value));
                            }

                            lock (this._syncRoot)
                            {
                                this.Add(item);
                            }

                            currentLine = await reader.ReadLineAsync();
                            this.LinesProcessed++;
                            this.BytesProcessed = reader.BaseStream.Position;
                            this.PercentComplete = Math.Round((Convert.ToDouble(reader.BaseStream.Position) / Convert.ToDouble(reader.BaseStream.Length)) * 100.0, 2);
                        } while (!reader.EndOfStream);

                        this.BytesProcessed = reader.BaseStream.Length;
                    }
                }
            }
            catch (Exception exc)
            {
                this.LoadError = exc;
            }
            finally
            {
                this.PercentComplete = 100.0;
                this.LoadInProgress = false;
                this.OnLoadFinished();
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
