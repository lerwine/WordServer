namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    public enum SynsetType
    {
        [SSTypeSymbol("n", PartOfSpeech.Noun)]
        Noun,
        [SSTypeSymbol("v", PartOfSpeech.Verb)]
        Verb,
        [SSTypeSymbol("a", PartOfSpeech.Adjective)]
        Adjective,
        [SSTypeSymbol("s", PartOfSpeech.Adjective)]
        AdjectiveSatellite,
        [SSTypeSymbol("r", PartOfSpeech.Adverb)]
        Adverb
    }
}
