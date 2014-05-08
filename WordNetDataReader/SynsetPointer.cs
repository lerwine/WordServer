namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class SynsetPointer
    {
        public Common.PointerSymbol PointerSymbol { get; set; }
        public long SynsetOffset { get; set; }
        public Common.PartOfSpeech PartOfSpeech { get; set; }
        public short Source { get; set; }
        public short Target { get; set; }

        public SynsetPointer(Common.PointerSymbol pointer_symbol, long synset_offset, Common.PartOfSpeech pos, short source, short target)
        {
            this.PointerSymbol = pointer_symbol;
            this.SynsetOffset = synset_offset;
            this.PartOfSpeech = pos;
            this.Source = source;
            this.Target = target;
        }
    }
}
