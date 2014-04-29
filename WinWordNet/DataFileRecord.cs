using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    [Serializable]
    public class DataFileRecord
    {
        public static readonly Regex FirstFourFields = new Regex(@"^(?<synset_offset>\d{8})\s+(?<lex_filenum>\d{2})\s+(?<ss_type>[nvasr])\s+(?<w_cnt>[\da-f]{2})(?<r>.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex Word = new Regex(@"^\s+(?<word>\S+?)(\((?<syntactic_marker>.+)\))?\s+(?<lex_id>[\da-f])(?<r>.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex PointerCount = new Regex(@"^\s+(?<ptr_cnt>\d{3})(?<r>.*)$", RegexOptions.Compiled);
        public static readonly Regex Pointer = new Regex(@"^\s+(?<pointer_symbol>\S{1,2})\s+(?<synset_offset>\d+)\s+(?<pos>[nvar])\s+(?<source>[\da-f]{2})(?<target>[\da-f]{2})(?<r>.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex FrameCount = new Regex(@"^\s+(?<f_cnt>\d{2})(?<r>.*)$", RegexOptions.Compiled);
        public static readonly Regex Frame = new Regex(@"^\s+\+\s+(?<f_num>\d{2})\s+(?<w_num>[\da-f]{2})(?<r>.*)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex Glossary = new Regex(@"^\s+|\s*(?<gloss>\S.*?)\s*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public long SynsetOffset { get; set; }

        public short LexFileNum { get; set; }

        public SynsetType SynsetType { get; set; }

        public DataFileWord[] Words { get; set; }

        public DataFilePointer[] Pointers { get; set; }

        public DataFileFrame[] Frames { get; set; }

        public string Definition { get; set; }

        public DataFileRecord() { }

        public DataFileRecord(string sourceLine)
        {
            if (sourceLine == null)
                throw new ArgumentNullException("sourceLine");

            if (String.IsNullOrWhiteSpace(sourceLine))
                throw new ArgumentException("Soure line is empty", "sourceLine");

            int position = 0;
            Match m = DataFileRecord.FirstFourFields.Match(sourceLine);
            if (!m.Success)
                throw new WordNetParseException("Error parsing first four fields", DataFileRecord.FirstFourFields, sourceLine, position);

            this.SynsetOffset = Convert.ToInt64(m.Groups["synset_offset"].Value);
            this.LexFileNum = Convert.ToInt16(m.Groups["lex_filenum"].Value);
            this.SynsetType = SSTypeSymbolAttribute.GetSynsetType(m.Groups["ss_type"].Value);

            int count = Convert.ToInt32(m.Groups["w_cnt"].Value, 16);

            Collection<DataFileWord>  words = new Collection<DataFileWord>();

            for (int i = 0; i < count; i++)
            {
                position = m.Groups["r"].Index;
                m = DataFileRecord.Word.Match(sourceLine, position);
                if (!m.Success)
                    throw new WordNetParseException("Error parsing word", DataFileRecord.Word, sourceLine, position);
                words.Add(new DataFileWord
                {
                    Word = m.Groups["word"].Value.Replace("_", " "),
                    SyntacticMarker = (m.Groups["syntactic_marker"].Success) ? m.Groups["syntactic_marker"].Value.Replace("_", " ") : "",
                    LexId = Convert.ToInt16(m.Groups["lex_id"].Value, 16)
                });
            }
            this.Words = words.ToArray();

            position = m.Groups["r"].Index;
            m = DataFileRecord.PointerCount.Match(sourceLine, position);
            if (!m.Success)
                throw new WordNetParseException("Error parsing pointer count", DataFileRecord.PointerCount, sourceLine, position);

            Collection<DataFilePointer> pointers = new Collection<DataFilePointer>();
            count = Convert.ToInt32(m.Groups["ptr_cnt"].Value, 16);
            for (int i = 0; i < count; i++)
            {
                position = m.Groups["r"].Index;
                m = DataFileRecord.Pointer.Match(sourceLine, position);
                if (!m.Success)
                    throw new WordNetParseException("Error parsing pointer", DataFileRecord.Pointer, sourceLine, position);
                PointerType pointerType;
                try
                {
                    pointerType = PointerSymbolAttribute.GetPointerType(m.Groups["pointer_symbol"].Value, this.SynsetType);
                }
                catch
                {
                    throw new WordNetParseException("Error parsing pointer symbol", DataFileRecord.Pointer, sourceLine, position);
                }

                pointers.Add(new DataFilePointer
                {
                    PointerType = pointerType,
                    SynsetOffset = Convert.ToInt64(m.Groups["synset_offset"].Value),
                    PartOfSpeech = PosSymbolAttribute.GetPartOfSpeech(m.Groups["pos"].Value),
                    Source = Convert.ToInt16(m.Groups["source"].Value),
                    Target = Convert.ToInt16(m.Groups["target"].Value, 16)
                });
            }
            this.Pointers = pointers.ToArray();

            if (this.SynsetType == WinWordNet.SynsetType.Verb)
            {
                Collection<DataFileFrame> frames = new Collection<DataFileFrame>();

                position = m.Groups["r"].Index;
                m = DataFileRecord.FrameCount.Match(sourceLine, position);
                if (!m.Success)
                    throw new WordNetParseException("Error parsing frame count", DataFileRecord.FrameCount, sourceLine, position);

                count = Convert.ToInt32(m.Groups["f_cnt"].Value, 16);
                for (int i = 0; i < count; i++)
                {
                    position = m.Groups["r"].Index;
                    m = DataFileRecord.Frame.Match(sourceLine, position);
                    if (!m.Success)
                        throw new WordNetParseException("Error parsing frame", DataFileRecord.Frame, sourceLine, position);
                    // (?<f_num>\d{2})\s+(?<w_num>[\da-f]{2})
                    frames.Add(new DataFileFrame
                    {
                        FrameNumber = Convert.ToInt16(m.Groups["f_num"].Value),
                        WordNumber = Convert.ToInt16(m.Groups["w_num"].Value, 16)
                    });
                }
                this.Frames = frames.ToArray();
            }
            else
                this.Frames = new DataFileFrame[0];

            position = m.Groups["r"].Index;
            m = DataFileRecord.Glossary.Match(sourceLine, position);
            if (!m.Success)
                throw new WordNetParseException("Error parsing glossary", DataFileRecord.Glossary, sourceLine, position);

            this.Definition = m.Groups["gloss"].Value;
        }
    }
}
