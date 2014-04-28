using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WordNetImporter.Src
{
    public class SynsetData
    {
        public long synset_offset { get; set; }
        public short lex_filenum { get; set; }
        public SynsetTypeValues ss_type { get; set; }
        public string gloss { get; set; }
        public int w_cnt { get; set; }
        public Collection<SynsetWordData> words { get; set; }
        public int p_cnt { get; set; }
        public Collection<SynsetPointerData> pointers { get; set; }
        public int f_cnt { get; set; }
        public Collection<VerbFrameData> frames { get; set; }
    }
}
