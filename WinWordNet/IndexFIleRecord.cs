using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    public class IndexFileRecord
    {
        public static readonly Regex CommentLine = new Regex(@"^\s{2}(?<lineNUmber>\d+)(\s+(?<comment>.+))?$", RegexOptions.Compiled);
        // sense_cnt  tagsense_cnt   synset_offset  [synset_offset...]  
        public static readonly Regex FirstFourFields = new Regex(@"^(?<lemma>\S+)\s+(?<pos>[nvar])\s+(?<synset_cnt>\d+)\s+(?<p_cnt>\d+)(?<r>\s.*)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static readonly Regex PtrSymbol = new Regex(@"^\s+(?<ptr_symbol>\S{1,2})(?<r>\s.*)$", RegexOptions.Compiled);
        public static readonly Regex SenseCounts = new Regex(@"^\s+(?<sense_cnt>\d+)\s+(?<tagsense_cnt>\d+)(?<r>\s.*)$", RegexOptions.Compiled);
        public static readonly Regex SynsetOffset = new Regex(@"^\s+(?<synset_offset>\d{8})(?<r>.*?)$", RegexOptions.Compiled);
    }
}
