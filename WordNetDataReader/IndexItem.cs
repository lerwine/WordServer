using System.Collections.ObjectModel;

namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class IndexItem
    {
        public string Word { get; set; }

        public Common.PartOfSpeech PartOfSpeech { get; set; }

        public int SynsetCount { get; set; }

        public Collection<Common.PointerSymbol> PointerSymbols { get; set; }

        public Collection<long> SynsetOffsets { get; set; }
    }
}
