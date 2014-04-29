namespace WordServer.WordNet
{
    public class DataItem
    {
        public long SynsetOffset { get; set; }

        public short LexFileNum { get; set; }

        public string SynsetType { get; set; }

        public SynsetWordItem[] Words { get; set; }

        public LexPointer[] Pointers { get; set; }

        public string Gloss { get; set; }
    }
}
