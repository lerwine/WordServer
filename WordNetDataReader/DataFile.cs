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
    public class DataFile : Collection<DataItem>
    {
        public const int Default_CacheSize = 1024;
        public static readonly TimeSpan Default_FileOpenTimeout = new TimeSpan(0, 1, 0);
        private static readonly Regex _commentRegex = new Regex(@"^\s+(?<n>\d+)\s*(?<t>.*?)$", RegexOptions.Compiled);
        private static readonly Regex _firstFourRegex = new Regex(@"^(?<synset_offset>\d{8})\s+(?<lex_filenum>\d{2})\s+(?<ss_type>[nvasr])\s+(?<w_cnt>[\da-f]{2})\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _wordRegex = new Regex(@"^(?<word>\S+)(\((?<syntactic_marker>\S{1,2})\))?\s+(?<[lex_id>[\da-f])\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _pointerCountRegex = new Regex(@"^(?<p_cnt>\d{3})\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private static readonly Regex _pointerRegex = new Regex(@"^(?<pointer_symbol>\S{1,2})\s+(?<synset_offset>\d+)\s+(?<pos>[nvar])\s+(?<source>[\da-f]{2})(?<target>[\da-f]{2})\s+(?<r>\S?.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _frameCountRegex = new Regex(@"^(?<f_cnt>\d{2})\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private static readonly Regex _frameRegex = new Regex(@"^+\s+(?<f_num>\d{2})\s+(?<w_num>[\da-f]{2})\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private static readonly Regex _glossRegex = new Regex(@"^+\s+\|\s+(?<gloss>.+?)\s*$", RegexOptions.Compiled);

        #region Private fields

        private object _syncRoot = new object();
        private object _fileAcessSync = new object();
        private Common.PartOfSpeech _partOfSpeech;
        private string _staticDbFolder;
        private string _userFriendlyPathSpec;
        private int _cacheSize = DataFile.Default_CacheSize;
        private TimeSpan _fileOpenTimeout = DataFile.Default_FileOpenTimeout;
        private DateTime? _lastFileActivity = null;
        private DateTime? _closeFileAfter = null;
        private FileStream _inputStream = null;

        #endregion

        #region Public properties

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

        public int CacheSize
        {
            get
            {
                int result;
                lock (this._syncRoot)
                {
                    result = this._cacheSize;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._cacheSize = (value < 1) ? 0 : value;
                    if (this._cacheSize == 0)
                        this.Clear();
                    else
                    {
                        while (this.Count > this._cacheSize)
                            this.RemoveAt(0);
                    }
                }
            }
        }

        public TimeSpan FileOpenTimeout
        {
            get
            {
                TimeSpan result;
                lock (this._syncRoot)
                {
                    result = this._fileOpenTimeout;
                }
                return result;
            }
            private set
            {
                lock (this._syncRoot)
                {
                    this._fileOpenTimeout = (value < TimeSpan.Zero) ? TimeSpan.Zero : value;

                    if (this._lastFileActivity.HasValue)
                        this._closeFileAfter = this._lastFileActivity.Value.Add(this._fileOpenTimeout);
                }

                this.CheckFileOpenTimeout();
            }
        }

        private void CheckFileOpenTimeout()
        {
            lock (this._syncRoot)
            {
                if (this._closeFileAfter.HasValue && this._closeFileAfter < DateTime.Now)
                {
                    if (this._inputStream != null)
                    {
                        this._inputStream.Close();
                        this._inputStream.Dispose();
                        this._inputStream = null;
                    }

                    this._closeFileAfter = null;
                    this._lastFileActivity = null;
                }
            }
        }

        #endregion

        public DataFile(string staticDbFolder, Common.PartOfSpeech partOfSpeech) : this(staticDbFolder, partOfSpeech, null) { }

        public DataFile(string staticDbFolder, Common.PartOfSpeech partOfSpeech, string userFriendlyPathSpec)
        {
            if (staticDbFolder == null)
                throw new ArgumentNullException("staticDbFolder");

            if (String.IsNullOrWhiteSpace(staticDbFolder))
                throw new ArgumentException("Folder path not provided", "staticDbFolder");

            this.StaticDbFolder = staticDbFolder;
            this.UserFriendlyPathSpec = userFriendlyPathSpec;
            this.PartOfSpeech = partOfSpeech;
        }

        private TResult AccessFile<TResult>(Func<TResult> handler)
        {
            TResult result;

            lock (this._fileAcessSync)
            {
                lock (this._syncRoot)
                {
                    this._closeFileAfter = null;
                    this._lastFileActivity = null;
                    if (this._inputStream == null)
                        this._inputStream = File.OpenRead(Path.Combine(this.StaticDbFolder, IndexFile.PosToFileName(this.PartOfSpeech)));
                }

                result = handler();

                lock (this._syncRoot)
                {
                    if (this._inputStream != null)
                    {
                        if (this.FileOpenTimeout.Equals(TimeSpan.Zero))
                        {
                            this._inputStream.Close();
                            this._inputStream.Dispose();
                            this._inputStream = null;
                        }
                        else
                        {
                            this._lastFileActivity = DateTime.Now;
                            this._closeFileAfter = this._lastFileActivity.Value.Add(this.FileOpenTimeout);
                        }
                    }
                }
            }

            return result;
        }

        public Task<DataItem[]> GetWordsAsync(IEnumerable<long> synset_offset)
        {
            Task<DataItem[]> task = new Task<DataItem[]>((object state) => this.AccessFile<DataItem[]>(() =>
            {
                return ((IEnumerable<long>)state).Select(synsetOffset =>
                {
                    DataItem result = this.FirstOrDefault(i => i.SynsetOffset == synsetOffset);
                    if (result != null)
                    {
                        lock (this._syncRoot)
                        {
                            this.Remove(result);
                            this.Add(result);
                            this._PurgeCache();
                        }

                        return result;
                    }

                    this._inputStream.Seek(synsetOffset, SeekOrigin.Begin);
                    using (StreamReader reader = new StreamReader(this._inputStream, Encoding.UTF8, false, 4096, true))
                    {
                        string currentLine = reader.ReadLine();
                        int position = 0;
                        Match m = DataFile._firstFourRegex.Match(currentLine);
                        if (!m.Success)
                            throw new WordNetParseException("Error parsing first four data file fields", DataFile._firstFourRegex, currentLine, position);
                        result = new DataItem
                        {
                            SynsetOffset = Convert.ToInt64(m.Groups["synset_offset"]),
                            LexFilenum = Convert.ToInt16(m.Groups["lex_filenum"]),
                            SynsetType = Common.SymbolAttribute.GetEnum<Common.SynsetType>(m.Groups["ss_type"].Value),
                            Words = new Collection<SynsetWord>(),
                            Pointers = new Collection<SynsetPointer>(),
                            Frames = new Collection<VerbFrame>()
                        };

                        int w_cnt = Convert.ToInt32(m.Groups["w_cnt"].Value);
                        for (int i = 0; i < w_cnt; i++)
                        {
                            position += m.Groups["r"].Index;
                            m = DataFile._wordRegex.Match(m.Groups["r"].Value);
                            if (!m.Success)
                                throw new WordNetParseException(String.Format("Error parsing synset word {0}", i + 1), DataFile._wordRegex, currentLine, position);
                            result.Words.Add(new SynsetWord(m.Groups["word"].Value.Replace('_', ' '),
                                (m.Groups["syntactic_marker"].Success) ? Common.SymbolAttribute.GetEnum<Common.SyntacticMarker>(m.Groups["syntactic_marker"].Value) :
                                Common.SyntacticMarker.None, Convert.ToInt16(m.Groups["lex_id"].Value, 16)));

                        }

                        position += m.Groups["r"].Index;
                        m = DataFile._pointerCountRegex.Match(m.Groups["r"].Value);
                        if (!m.Success)
                            throw new WordNetParseException("Error parsing pointer count", DataFile._pointerCountRegex, currentLine, position);

                        int p_cnt = Convert.ToInt32(m.Groups["p_cnt"].Value);
                        for (int i = 0; i < p_cnt; i++)
                        {
                            position += m.Groups["r"].Index;
                            m = DataFile._pointerRegex.Match(m.Groups["r"].Value);
                            if (!m.Success)
                                throw new WordNetParseException(String.Format("Error parsing synset pointer {0}", i + 1), DataFile._pointerRegex, currentLine, position);
                            Common.PartOfSpeech pos = Common.SymbolAttribute.GetEnum<Common.PartOfSpeech>(m.Groups["pos"].Value);
                            result.Pointers.Add(new SynsetPointer(Common.PosAndSymbolAttribute.GetEnum<Common.PointerSymbol>(m.Groups["pointer_symbol"].Value, pos),
                                Convert.ToInt64(m.Groups["synset_offset"].Value), pos, Convert.ToInt16(m.Groups["source"].Value, 16), Convert.ToInt16(m.Groups["target"].Value, 16)));

                        }

                        if (result.SynsetType == Common.SynsetType.Verb)
                        {
                            position += m.Groups["r"].Index;
                            m = DataFile._frameCountRegex.Match(m.Groups["r"].Value);
                            if (!m.Success)
                                throw new WordNetParseException("Error parsing pointer count", DataFile._frameCountRegex, currentLine, position);
                            int f_cnt = Convert.ToInt32(m.Groups["f_cnt"].Value);
                            for (int i = 0; i < f_cnt; i++)
                            {
                                position += m.Groups["r"].Index;
                                m = DataFile._frameRegex.Match(m.Groups["r"].Value);
                                if (!m.Success)
                                    throw new WordNetParseException(String.Format("Error parsing verb frame {0}", i + 1), DataFile._frameRegex, currentLine, position);
                                result.Frames.Add(new VerbFrame(Convert.ToInt16(m.Groups["f_num"].Value), Convert.ToInt16(m.Groups["w_num"].Value, 16)));
                            }
                        }

                        position += m.Groups["r"].Index;
                        m = DataFile._glossRegex.Match(m.Groups["r"].Value);
                        if (!m.Success)
                            throw new WordNetParseException("Error parsing gloss", DataFile._glossRegex, currentLine, position);

                        result.Glossary = m.Groups["gloss"].Value;
                    }

                    return result;
                }).ToArray();
            }), synset_offset);

            task.Start();

            return task;
        }

        private void _PurgeCache()
        {
            lock (this._syncRoot)
            {
                while (this.Count > this._cacheSize)
                    this.RemoveAt(0);
            }
        }
    }
}
