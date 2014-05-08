namespace Erwine.Leonard.T.WordServer.WordNetDataReader
{
    public class SynsetWord
    {
        public string Word { get; set; }
        public Common.SyntacticMarker SyntacticMarker { get; set; }
        public short LexId { get; set; }

        public SynsetWord(string word, Common.SyntacticMarker syntactic_marker, short lex_id)
        {
            this.Word = word;
            this.SyntacticMarker = syntactic_marker;
            this.LexId = lex_id;
        }
    }
}
