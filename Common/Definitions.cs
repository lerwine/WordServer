using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Erwine.Leonard.T.WordServer.Common
{
    /// <summary>
    /// Part of speech, AKA Syntactic Category
    /// </summary>
	public enum PartOfSpeech : short
	{
		[Symbol("n")]
		Noun = 1,
		[Symbol("v")]
		Verb = 2,
		[Symbol("a")]
		Adjective = 3,
		[Symbol("r")]
		Adverb = 4,
	}

	public enum SynsetType : short
	{
		[PosAndSymbol(PartOfSpeech.Noun, "n", Description = "Noun")]
		Noun = 1,
		[PosAndSymbol(PartOfSpeech.Verb, "v", Description = "Verb")]
		Verb = 2,
		[PosAndSymbol(PartOfSpeech.Adjective, "a", Description = "Adjective")]
		Adjective = 3,
		[PosAndSymbol(PartOfSpeech.Adverb, "r", Description = "Adverb")]
		Adverb = 4,
		[PosAndSymbol(PartOfSpeech.Adjective, "s", Description = "Adjective Satellite")]
		Adjective_Satellite = 5
	}

	public enum SyntacticMarker : short
	{
		[Symbol("", Description = "No syntactic marker")]
		None = 0,
		[Symbol("p", Description = "Predicate position")]
		Predicate_Position = 1,
		[Symbol("a", Description = "Prenominal (attributive) position")]
		Prenominal_Attributive_Position = 2,
		[Symbol("ip", Description = "Immediately postnominal position")]
		Immediately_Postnominal_Position = 3
	}

	public enum PointerSymbol : short
	{
		[PosAndSymbol(PartOfSpeech.Noun, "!", Description = "Antonym")]
		[PosAndSymbol(PartOfSpeech.Verb, "!", Description = "Antonym")]
		[PosAndSymbol(PartOfSpeech.Adjective, "!", Description = "Antonym")]
		[PosAndSymbol(PartOfSpeech.Adverb, "!", Description = "Antonym")]
		Antonym = 1,
		[PosAndSymbol(PartOfSpeech.Noun, "@", Description = "Hypernym")]
		[PosAndSymbol(PartOfSpeech.Verb, "@", Description = "Hypernym")]
		Hypernym = 2,
		[PosAndSymbol(PartOfSpeech.Noun, "@i", Description = "Instance Hypernym")]
		Instance_Hypernym = 3,
		[PosAndSymbol(PartOfSpeech.Noun, "~", Description = "Hyponym")]
		[PosAndSymbol(PartOfSpeech.Verb, "~", Description = "Hyponym")]
		Hyponym = 4,
		[PosAndSymbol(PartOfSpeech.Noun, "~i", Description = "Instance Hyponym")]
		Instance_Hyponym = 5,
		[PosAndSymbol(PartOfSpeech.Noun, "#m", Description = "Member holonym")]
		Member_Holonym = 6,
		[PosAndSymbol(PartOfSpeech.Noun, "#s", Description = "Substance holonym")]
		Substance_Holonym = 7,
		[PosAndSymbol(PartOfSpeech.Noun, "#p", Description = "Part holonym")]
		Part_Holonym = 8,
		[PosAndSymbol(PartOfSpeech.Noun, "%m", Description = "Member meronym")]
		Member_Meronym = 9,
		[PosAndSymbol(PartOfSpeech.Noun, "%s", Description = "Substance meronym")]
		Substance_Meronym = 10,
		[PosAndSymbol(PartOfSpeech.Noun, "%p", Description = "Part meronym")]
		Part_Meronym = 11,
		[PosAndSymbol(PartOfSpeech.Noun, "=", Description = "Attribute")]
		[PosAndSymbol(PartOfSpeech.Adjective, "=", Description = "Attribute")]
		Attribute = 12,
		[PosAndSymbol(PartOfSpeech.Noun, "+", Description = "Derivationally related form")]
		[PosAndSymbol(PartOfSpeech.Verb, "+", Description = "Derivationally related form")]
		Derivationally_Related_Form = 13,
		[PosAndSymbol(PartOfSpeech.Noun, ";c", Description = "Domain of synset - Topic")]
		[PosAndSymbol(PartOfSpeech.Verb, ";c", Description = "Domain of synset - Topic")]
		[PosAndSymbol(PartOfSpeech.Adjective, ";c", Description = "Domain of synset - Topic")]
		[PosAndSymbol(PartOfSpeech.Adverb, ";c", Description = "Domain of synset - Topic")]
		Domain_Of_Synset_Topic = 14,
		[PosAndSymbol(PartOfSpeech.Noun, "-c", Description = "Member of this domain - Topic")]
		Member_Of_This_Domain_Topic = 15,
		[PosAndSymbol(PartOfSpeech.Noun, ";r", Description = "Domain of synset - Region")]
		[PosAndSymbol(PartOfSpeech.Verb, ";r", Description = "Domain of synset - Region")]
		[PosAndSymbol(PartOfSpeech.Adjective, ";r", Description = "Domain of synset - Region")]
		[PosAndSymbol(PartOfSpeech.Adverb, ";r", Description = "Domain of synset - Region")]
		Domain_Of_Synset_Region = 16,
		[PosAndSymbol(PartOfSpeech.Noun, "-r", Description = "Member of this domain - Region")]
		Member_Of_This_Domain_Region = 17,
		[PosAndSymbol(PartOfSpeech.Noun, ";u", Description = "Domain of synset - Usage")]
		[PosAndSymbol(PartOfSpeech.Verb, ";u", Description = "Domain of synset - Usage")]
		[PosAndSymbol(PartOfSpeech.Adjective, ";u", Description = "Domain of synset - Usage")]
		[PosAndSymbol(PartOfSpeech.Adverb, ";u", Description = "Domain of synset - Usage")]
		Domain_Of_Synset_Usage = 18,
		[PosAndSymbol(PartOfSpeech.Noun, "-u", Description = "Member of this domain - Usage")]
		Member_Of_This_Domain_Usage = 19,
		[PosAndSymbol(PartOfSpeech.Verb, "*", Description = "Entailment")]
		Entailment = 4,
		[PosAndSymbol(PartOfSpeech.Verb, ">", Description = "Cause")]
		Cause = 5,
		[PosAndSymbol(PartOfSpeech.Verb, "^", Description = "Also see")]
		[PosAndSymbol(PartOfSpeech.Adjective, "^", Description = "Also see")]
		Also_See = 6,
		[PosAndSymbol(PartOfSpeech.Verb, "$", Description = "Verb Group")]
		Verb_Group = 7,
		[PosAndSymbol(PartOfSpeech.Adjective, "&", Description = "Similar to")]
		Similar_To = 2,
		[PosAndSymbol(PartOfSpeech.Adjective, "<", Description = "Participle of verb")]
		Participle_Of_Verb = 3,
		[PosAndSymbol(PartOfSpeech.Adjective, @"\", Description = "Pertainym (pertains to noun)")]
		Pertainym_Pertains_To_Noun = 4,
		[PosAndSymbol(PartOfSpeech.Adverb, @"\", Description = "Derived from adjective")]
		Derived_From_Adjective = 2
	}

	public enum LexicographerFile : short
	{
		[PosValue(PartOfSpeech.Adjective, Description = "All adjective clusters")]
		Adj_All = 0,
		[PosValue(PartOfSpeech.Adjective, Description = "Relational adjectives (pertainyms)")]
		Adj_Pert = 1,
		[PosValue(PartOfSpeech.Adverb, Description = "All adverbs")]
		Adv_All = 2,
		[PosValue(PartOfSpeech.Noun, Description = "Unique beginner for nouns")]
		Noun_Tops = 3,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting acts or actions")]
		Noun_Act = 4,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting animals")]
		Noun_Animal = 5,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting man-made objects")]
		Noun_Artifact = 6,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting attributes of people and objects")]
		Noun_Attribute = 7,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting body parts")]
		Noun_Body = 8,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting cognitive processes and contents")]
		Noun_Cognition = 9,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting communicative processes and contents")]
		Noun_Communication = 10,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting natural events")]
		Noun_Event = 11,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting feelings and emotions")]
		Noun_Feeling = 12,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting foods and drinks")]
		Noun_Food = 13,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting groupings of people or objects")]
		Noun_Group = 14,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting spatial position")]
		Noun_Location = 15,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting goals")]
		Noun_Motive = 16,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting natural objects (not man-made)")]
		Noun_Object = 17,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting people")]
		Noun_Person = 18,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting natural phenomena")]
		Noun_Phenomenon = 19,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting plants")]
		Noun_Plant = 20,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting possession and transfer of possession")]
		Noun_Possession = 21,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting natural processes")]
		Noun_Process = 22,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting quantities and units of measure")]
		Noun_Quantity = 23,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting relations between people or things or ideas")]
		Noun_Relation = 24,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting two and three dimensional shapes")]
		Noun_Shape = 25,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting stable states of affairs")]
		Noun_State = 26,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting substances")]
		Noun_Substance = 27,
		[PosValue(PartOfSpeech.Noun, Description = "Nouns denoting time and temporal relations")]
		Noun_Time = 28,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of grooming, dressing and bodily care")]
		Verb_Body = 29,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of size, temperature change, intensifying, etc.")]
		Verb_Change = 30,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of thinking, judging, analyzing, doubting")]
		Verb_Cognition = 31,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of telling, asking, ordering, singing")]
		Verb_Communication = 32,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of fighting, athletic activities")]
		Verb_Competition = 33,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of eating and drinking")]
		Verb_Consumption = 34,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of touching, hitting, tying, digging")]
		Verb_Contact = 35,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of sewing, baking, painting, performing")]
		Verb_Creation = 36,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of feeling")]
		Verb_Emotion = 37,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of walking, flying, swimming")]
		Verb_Motion = 38,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of seeing, hearing, feeling")]
		Verb_Perception = 39,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of buying, selling, owning")]
		Verb_Possession = 40,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of political and social activities and events")]
		Verb_Social = 41,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of being, having, spatial relations")]
		Verb_Stative = 42,
		[PosValue(PartOfSpeech.Verb, Description = "Verbs of raining, snowing, thawing, thundering")]
		Verb_Weather = 43,
		[PosValue(PartOfSpeech.Adjective, Description = "Participial adjectives")]
		Adj_Ppl = 44
	}

    public class VerbFrames : ReadOnlyDictionary<short, string>
    {
        private static Dictionary<short, string> _innerDictionary = (new[]
        {
            new { Key = (short)1, Value = "Something {0}s" },
            new { Key = (short)2, Value = "Somebody {0}s" },
            new { Key = (short)3, Value = "It is {0}ing" },
            new { Key = (short)4, Value = "Something is {0}ing PP" },
            new { Key = (short)5, Value = "Something {0}s something Adjective/Noun" },
            new { Key = (short)6, Value = "Something {0}s Adjective/Noun" },
            new { Key = (short)7, Value = "Somebody {0}s Adjective" },
            new { Key = (short)8, Value = "Somebody {0}s something" },
            new { Key = (short)9, Value = "Somebody {0}s somebody" },
            new { Key = (short)10, Value = "Something {0}s somebody" },
            new { Key = (short)11, Value = "Something {0}s something" },
            new { Key = (short)12, Value = "Something {0}s to somebody" },
            new { Key = (short)13, Value = "Somebody {0}s on something" },
            new { Key = (short)14, Value = "Somebody {0}s somebody something" },
            new { Key = (short)15, Value = "Somebody {0}s something to somebody" },
            new { Key = (short)16, Value = "Somebody {0}s something from somebody" },
            new { Key = (short)17, Value = "Somebody {0}s somebody with something" },
            new { Key = (short)18, Value = "Somebody {0}s somebody of something" },
            new { Key = (short)19, Value = "Somebody {0}s something on somebody" },
            new { Key = (short)20, Value = "Somebody {0}s somebody PP" },
            new { Key = (short)21, Value = "Somebody {0}s something PP" },
            new { Key = (short)22, Value = "Somebody {0}s PP" },
            new { Key = (short)23, Value = "Somebody's (body part) {0}s" },
            new { Key = (short)24, Value = "Somebody {0}s somebody to INFINITIVE" },
            new { Key = (short)25, Value = "Somebody {0}s somebody INFINITIVE" },
            new { Key = (short)26, Value = "Somebody {0}s that CLAUSE" },
            new { Key = (short)27, Value = "Somebody {0}s to somebody" },
            new { Key = (short)28, Value = "Somebody {0}s to INFINITIVE" },
            new { Key = (short)29, Value = "Somebody {0}s whether INFINITIVE" },
            new { Key = (short)30, Value = "Somebody {0}s somebody into V-ing something" },
            new { Key = (short)31, Value = "Somebody {0}s something with something" },
            new { Key = (short)32, Value = "Somebody {0}s INFINITIVE" },
            new { Key = (short)33, Value = "Somebody {0}s VERB-ing" },
            new { Key = (short)34, Value = "It {0}s that CLAUSE" },
            new { Key = (short)35, Value = "Something {0}s INFINITIVE" }
        }).ToDictionary(a => a.Key, a => a.Value);

        private static VerbFrames _instance = null;

        public static VerbFrames Instance
        {
            get
            {
                lock (VerbFrames._innerDictionary)
                {
                    if (VerbFrames._instance == null)
                        VerbFrames._instance = new VerbFrames();
                }

                return VerbFrames._instance;
            }
        }

        public VerbFrames() : base(VerbFrames._innerDictionary) { }
    }
}
