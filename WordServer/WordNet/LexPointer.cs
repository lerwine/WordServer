namespace WordServer.WordNet
{
    public class LexPointer
    {
        public string PointerSymbol { get; set; }

        public long SynsetOffset { get; set; }

        public string Pos { get; set; }

        public int SourceTarget { get; set; }
    }
}
