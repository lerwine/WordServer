namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    public enum PointerType : short
    {
        [PointerSymbol("!", WinWordNet.PartOfSpeech.Noun)]
        [PointerSymbol("!", WinWordNet.PartOfSpeech.Verb)]
        [PointerSymbol("!", WinWordNet.PartOfSpeech.Adjective)]
        [PointerSymbol("!", WinWordNet.PartOfSpeech.Adverb)]
        Antonym = 0,
        [PointerSymbol("@", WinWordNet.PartOfSpeech.Noun)]
        [PointerSymbol("@", WinWordNet.PartOfSpeech.Verb)]
        Hypernym,
        [PointerSymbol("@i", WinWordNet.PartOfSpeech.Noun, Description = "Instance Hypernym")]
        Instance_Hypernym,
        [PointerSymbol("~", WinWordNet.PartOfSpeech.Noun)]
        [PointerSymbol("~", WinWordNet.PartOfSpeech.Verb)]
        Hyponym,
        [PointerSymbol("~i", WinWordNet.PartOfSpeech.Noun, Description = "Instance Hyponym")]
        Instance_Hyponym,
        [PointerSymbol("#m", WinWordNet.PartOfSpeech.Noun, Description = "Member holonym")]
        Member_holonym,
        [PointerSymbol("#s", WinWordNet.PartOfSpeech.Noun, Description = "Substance holonym")]
        Substance_holonym,
        [PointerSymbol("#p", WinWordNet.PartOfSpeech.Noun, Description = "Part holonym")]
        Part_holonym,
        [PointerSymbol("%m", WinWordNet.PartOfSpeech.Noun, Description = "Member meronym")]
        Member_meronym,
        [PointerSymbol("%s", WinWordNet.PartOfSpeech.Noun, Description = "Substance meronym")]
        Substance_meronym,
        [PointerSymbol("%p", WinWordNet.PartOfSpeech.Noun, Description = "Part meronym")]
        Part_meronym,
        [PointerSymbol("=", WinWordNet.PartOfSpeech.Noun)]
        [PointerSymbol("=", WinWordNet.PartOfSpeech.Adjective)]
        Attribute,
        [PointerSymbol("+", WinWordNet.PartOfSpeech.Noun, Description = "Derivationally related form")]
        [PointerSymbol("+", WinWordNet.PartOfSpeech.Verb, Description = "Derivationally related form")]
        Derivationally_related_form,
        [PointerSymbol(";c", WinWordNet.PartOfSpeech.Noun, Description = "Domain of synset - TOPIC")]
        [PointerSymbol(";c", WinWordNet.PartOfSpeech.Verb, Description = "Domain of synset - TOPIC")]
        [PointerSymbol(";c", WinWordNet.PartOfSpeech.Adjective, Description = "Domain of synset - TOPIC")]
        [PointerSymbol(";c", WinWordNet.PartOfSpeech.Adverb, Description = "Domain of synset - TOPIC")]
        Domain_of_synset_TOPIC,
        [PointerSymbol("-c", WinWordNet.PartOfSpeech.Noun, Description = "Member of this domain - TOPIC")]
        Member_of_this_domain_TOPIC,
        [PointerSymbol(";r", WinWordNet.PartOfSpeech.Noun, Description = "Domain of synset - REGION")]
        [PointerSymbol(";r", WinWordNet.PartOfSpeech.Verb, Description = "Domain of synset - REGION")]
        [PointerSymbol(";r", WinWordNet.PartOfSpeech.Adjective, Description = "Domain of synset - REGION")]
        [PointerSymbol(";r", WinWordNet.PartOfSpeech.Adverb, Description = "Domain of synset - REGION")]
        Domain_of_synset_REGION,
        [PointerSymbol("-r", WinWordNet.PartOfSpeech.Noun, Description = "Member of this domain - REGION")]
        Member_of_this_domain_REGION,
        [PointerSymbol(";u", WinWordNet.PartOfSpeech.Noun, Description = "Domain of synset - USAGE")]
        [PointerSymbol(";u", WinWordNet.PartOfSpeech.Verb, Description = "Domain of synset - USAGE")]
        [PointerSymbol(";u", WinWordNet.PartOfSpeech.Adjective, Description = "Domain of synset - USAGE")]
        [PointerSymbol(";u", WinWordNet.PartOfSpeech.Adverb, Description = "Domain of synset - USAGE")]
        Domain_of_synset_USAGE,
        [PointerSymbol("-u", WinWordNet.PartOfSpeech.Noun, Description = "Member of this domain - USAGE")]
        Member_of_this_domain_USAGE,
        [PointerSymbol("*", WinWordNet.PartOfSpeech.Verb)]
        Entailment,
        [PointerSymbol(">", WinWordNet.PartOfSpeech.Verb)]
        Cause,
        [PointerSymbol("^", WinWordNet.PartOfSpeech.Verb)]
        [PointerSymbol("^", WinWordNet.PartOfSpeech.Adjective, Description = "Also see")]
        Also_see,
        [PointerSymbol("$", WinWordNet.PartOfSpeech.Verb, Description = "Verb Group")]
        Verb_Group,
        [PointerSymbol("&", WinWordNet.PartOfSpeech.Adjective, Description = "Similar to")]
        Similar_to,
        [PointerSymbol("<", WinWordNet.PartOfSpeech.Adjective, Description = "Participle of verb")]
        Participle_of_verb,
        [PointerSymbol("\\", WinWordNet.PartOfSpeech.Adjective, Description = "Pertainym (pertains to noun)")]
        Pertainym_pertains_to_noun,
        [PointerSymbol("\\", WinWordNet.PartOfSpeech.Adverb, Description = "Derived from adjective")]
        Derived_from_adjective
    }
}
