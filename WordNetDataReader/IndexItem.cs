using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class IndexItem
    {
        public string lemma { get; set; }

        public Common.PartOfSpeech pos { get; set; }

        public int synset_cnt { get; set; }

        public System.Collections.ObjectModel.Collection<Common.PointerSymbol> ptr_symbol { get; set; }

        public System.Collections.ObjectModel.Collection<long> synset_offset { get; set; }
    }
}
