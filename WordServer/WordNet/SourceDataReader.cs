using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;

namespace WordServer.WordNet
{
    public class SourceDataReader : IDisposable
    {
        private FileStream _fileStream;
        private StreamReader _reader;

        private Regex _part0 = new Regex(@"^\s+\d+.*$", RegexOptions.Compiled);
        private Regex _part1 = new Regex(@"^(?<synset_offset>\d+)\s+(?<lex_filenum>\d+)\s+(?<ss_type>[nvasr])\s+(?<w_cnt>[\da-f]+\s+\(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _part2 = new Regex(@"^(?<word>\S+\s+(?<lex_id>[\da-f])\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _part3 = new Regex(@"^(?<p_cnt>\d+)\s+(?<r>\S.*)$", RegexOptions.Compiled);
        private Regex _part4 = new Regex(@"^(?<pointer_symbol>\S+)\s+(?<synset_offset>\d+)\s+(?<pos>)[nvasr])\s+(?<src_targ>\d+)\s+(?<r>\S.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private Regex _part5 = new Regex(@"^|\s*(?<gloss>\S+.*)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public SourceDataReader(string path) : this(File.OpenRead(path), false) { }

        public SourceDataReader(FileStream fileStream, bool leaveOpen)
        {
            this._fileStream = fileStream;
            this._reader = new StreamReader(fileStream, Encoding.UTF8, false, 4096, leaveOpen);
        }

        public DataItem ReadNext()
        {
            string line;
            do
            {
                if (this._reader.EndOfStream)
                    return null;
            } while ((line= this._reader.ReadLine()).Trim().Length == 0 || this._part0.IsMatch(line));

            Match m = this._part1.Match(line);
            if (!m.Success)
                throw new SourceParseException("Pattern group 1 mismatch", 0, line);

            DataItem result = new DataItem
            {
                SynsetOffset = Convert.ToInt64(m.Groups["synset_offset"].Value),
                LexFileNum = Convert.ToInt16(m.Groups["lex_filenum"].Value),
                SynsetType = m.Groups["ss_type"].Value
            };

            int count = Convert.ToInt32(m.Groups["w_cnt"].Value, 16);
            Collection<SynsetWordItem> words = new Collection<SynsetWordItem>();
            int position = 0;
            for (int i = 0; i < count; i++)
            {
                position += m.Groups["r"].Index;
                m = this._part2.Match(m.Groups["r"].Value);
                if (!m.Success)
                    throw new SourceParseException("Pattern group 2 mismatch", position, line);
                // (?<word>\S+\s+(?<lex_id>[\da-f])
                words.Add(new SynsetWordItem
                {
                    Word = m.Groups["word"].Value.Replace("_", " "),
                    LexId = Convert.ToInt16(m.Groups["lex_id"].Value, 16)
                });
            }

            result.Words = words.ToArray();

            position += m.Groups["r"].Index;
            m = this._part3.Match(m.Groups["r"].Value);
            if (!m.Success)
                throw new SourceParseException("Pattern group 3 mismatch", position, line);

            count = Convert.ToInt32(m.Groups["p_cnt"].Value, 16);

            Collection<LexPointer> pointers = new Collection<LexPointer>();
            for (int i = 0; i < count; i++)
            {
                position += m.Groups["r"].Index;
                m = this._part4.Match(m.Groups["r"].Value);
                if (!m.Success)
                    throw new SourceParseException("Pattern group 4 mismatch", position, line);

                pointers.Add(new LexPointer
                {
                    PointerSymbol = m.Groups["pointer_symbol"].Value.Replace("_", " "),
                    SynsetOffset = Convert.ToInt64(m.Groups["synset_offset"].Value, 16),
                    Pos = m.Groups["pos"].Value,
                    SourceTarget = Convert.ToInt32(m.Groups["src_targ"].Value),
                });
            }

            result.Pointers = pointers.ToArray();

            position += m.Groups["r"].Index;
            m = this._part5.Match(m.Groups["r"].Value);
            if (!m.Success)
                throw new SourceParseException("Pattern group 5 mismatch", position, line);

            result.Gloss = m.Groups["gloss"].Value;

            return result;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}