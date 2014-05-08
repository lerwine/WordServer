using System.Collections.ObjectModel;

namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class DataItem
    {
        public long SynsetOffset { get; set; }

        public short LexFilenum { get; set; }

        public Common.SynsetType SynsetType { get; set; }

        public Collection<SynsetWord> Words { get; set; }

        public Collection<SynsetPointer> Pointers { get; set; }

        public Collection<VerbFrame> Frames { get; set; }

        public string Glossary { get; set; }
    }
}
