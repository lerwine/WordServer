using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordNetImporter.Src
{
    class SynsetPointerData
    {
        public string pointer_symbol { get; set; }

        public long synset_offset { get; set; }

        public SynsetTypeValues pos { get; set; }

        public short src { get; set; }

        public short targ { get; set; }
    }
}
