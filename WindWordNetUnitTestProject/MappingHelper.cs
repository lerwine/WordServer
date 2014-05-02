using Erwine.Leonard.T.WordServer.WinWordNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindWordNetUnitTestProject
{
   public static class MappingHelper
   {
       private static Regex _whitespaceRegex = new Regex(@"\s+", RegexOptions.Compiled);
       private static Regex _newLineRegex = new Regex(@"[\r\n]+", RegexOptions.Compiled);
       private static Regex _symbolDataRegex = new Regex(@"^\s*(?<v>\S{1,2})\s+(?<k>\S.*\S)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
       private static Regex _numberedDataRegex = new Regex(@"^\s*(?<v>\d+)\s+(?<k>\S.*\S)\s*$", RegexOptions.Compiled | RegexOptions.Multiline);
       private static Regex _nonLetterRegex = new Regex(@"[^a-z]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
       private static Regex _multipleDashesRegex = new Regex(@"-{3,}", RegexOptions.Compiled);
       public static readonly Regex _lexicographerFileRegex = new Regex(@"(?<v>\d{2})\s+(?<pos>\S+)\.(?<k>\S+)\s+(?<d>\S.*?)\s*$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

       public static string ToIdentifier(string source)
       {
           string st = source.Trim();
           return String.Join("_", MappingHelper._whitespaceRegex.Split((Char.IsLetter(st[0])) ? st : "N" + st)
                  .Select(s => new String(s.Take(1).Select(c => Char.ToUpper(c)).Concat(s.Skip(1).Select(c => Char.ToLower(c))).ToArray())).ToArray());
       }

       /// <summary>
       /// Key = Identifier name (Part of Speech), Value = symbol
       /// </summary>
       public readonly Dictionary<string, string> PartsOfSpeech = Regex.Matches(WordNetData.PartOfSpeech, @"[:,]\s+(?<v>\S)\s+for\s+(?<k>\S+)\s+files", RegexOptions.IgnoreCase)
           .OfType<Match>().ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["v"].Value);

       /// <summary>
       /// Key = Identifier name (Part of Speech), Value = category number
       /// </summary>
       public readonly Dictionary<string, int> SyntacticCategories = MappingHelper._numberedDataRegex.Matches(WordNetData.SyntacticCategories)
           .OfType<Match>().ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => Convert.ToInt32(m.Groups["v"].Value));

       /// <summary>
       /// Key = Frame number, Value = display format string (with {0} as word placeholder)
       /// </summary>
       public readonly Dictionary<int, string> VerbFrames = MappingHelper._numberedDataRegex.Matches(WordNetData.VerbFrames)
           .OfType<Match>().ToDictionary(m => Convert.ToInt32(m.Groups["v"].Value), m => MappingHelper._multipleDashesRegex.Replace(m.Groups["k"].Value, "{0}"));

       /// <summary>
       /// Key = Identifier name (Synset Type), Value = Numeric value
       /// </summary>
       public readonly Dictionary<string, int> SynsetTypeNumbers = MappingHelper._numberedDataRegex.Matches(WordNetData.SynsetTypeNumber)
           .OfType<Match>().ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => Convert.ToInt32(m.Groups["v"].Value));

       /// <summary>
       /// Key = Identifier name (Synset Type), Value = symbol
       /// </summary>
       public readonly Dictionary<string, string> SynsetTypeSymbols = MappingHelper._symbolDataRegex.Matches(WordNetData.SynsetTypeSymbol)
           .OfType<Match>().ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["v"].Value);

        /// <summary>
       /// Key = Identifier name (Synset Type), Value = Description
       /// </summary>
       public readonly Dictionary<string, string> SynsetTypeSDescriptions = MappingHelper._symbolDataRegex.Matches(WordNetData.SynsetTypeSymbol)
           .OfType<Match>().ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["k"].Value);

       /// <summary>
       /// Outer key = Identifier name (Part of Speech), Inner Key = Identifier name (Pointer Type), Value = symbol
       /// </summary>
       public readonly Dictionary<string, Dictionary<string, string>> PointerTypeSymbols = (new[]
       {
           new { K = "Noun", V = MappingHelper._symbolDataRegex.Matches(WordNetData.NounPointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["v"].Value) },
           new { K = "Verb", V = MappingHelper._symbolDataRegex.Matches(WordNetData.VerbPointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["v"].Value)},
           new { K = "Adjective", V = MappingHelper._symbolDataRegex.Matches(WordNetData.AdjectivePointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["v"].Value)},
           new { K = "Adverb", V = MappingHelper._symbolDataRegex.Matches(WordNetData.AdverbPointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["v"].Value)}
       }).ToDictionary(a => a.K, a => a.V);

       /// <summary>
       /// Outer key = Identifier name (Part of Speech), Inner Key = Identifier name (Pointer Type), Value = description
       /// </summary>
       public readonly Dictionary<string, Dictionary<string, string>> PointerTypeDescriptions = (new[]
       {
           new { K = "Noun", V = MappingHelper._symbolDataRegex.Matches(WordNetData.NounPointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["k"].Value) },
           new { K = "Verb", V = MappingHelper._symbolDataRegex.Matches(WordNetData.VerbPointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["k"].Value)},
           new { K = "Adjective", V = MappingHelper._symbolDataRegex.Matches(WordNetData.AdjectivePointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["k"].Value)},
           new { K = "Adverb", V = MappingHelper._symbolDataRegex.Matches(WordNetData.AdverbPointerSymbols).OfType<Match>()
               .ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["k"].Value)}
       }).ToDictionary(a => a.K, a => a.V);

       /// <summary>
       /// Outer key = Identifier name (Part of Speech), Inner Key = Identifier name (Lex Category), Value = file number
       /// </summary>
       public static readonly Dictionary<string, Dictionary<string, int>> LexicographerFileNumbers = MappingHelper._lexicographerFileRegex.Matches(WordNetData.LexicographerFiles).OfType<Match>()
           .GroupBy(m => m.Groups["pos"].Value)
           .ToDictionary(g => (g.Key == "adv") ? "adverb" : ((g.Key == "adj") ? "adjective" : g.Key), g => g.ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => Convert.ToInt32(m.Groups["v"].Value)));

       /// <summary>
       /// Outer key = Identifier name (Part of Speech), Inner Key = Identifier name (Lex Category), Value = description
       /// </summary>
       public static readonly Dictionary<string, Dictionary<string, string>> LexicographerFileDescriptions = MappingHelper._lexicographerFileRegex.Matches(WordNetData.LexicographerFiles).OfType<Match>()
           .GroupBy(m => m.Groups["pos"].Value)
           .ToDictionary(g => (g.Key == "adv") ? "adverb" : ((g.Key == "adj") ? "adjective" : g.Key), g => g.ToDictionary(m => MappingHelper.ToIdentifier(m.Groups["k"].Value), m => m.Groups["d"].Value));
    }
}
