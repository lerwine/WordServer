using System;
using Erwine.Leonard.T.WordServer.WinWordNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WindWordNetUnitTestProject
{
    [TestClass]
    public class AttributesTest
    {
        [TestMethod]
        public void GetSynsetTypeTestMethod()
        {
            SynsetType expected = SynsetType.Noun;
            SynsetType actual = SSTypeSymbolAttribute.GetSynsetType("n");
            Assert.AreEqual(expected, actual);

            expected = SynsetType.Verb;
            actual = SSTypeSymbolAttribute.GetSynsetType("v");
            Assert.AreEqual(expected, actual);

            expected = SynsetType.Adjective;
            actual = SSTypeSymbolAttribute.GetSynsetType("a");
            Assert.AreEqual(expected, actual);

            expected = SynsetType.AdjectiveSatellite;
            actual = SSTypeSymbolAttribute.GetSynsetType("s");
            Assert.AreEqual(expected, actual);

            expected = SynsetType.Adverb;
            actual = SSTypeSymbolAttribute.GetSynsetType("r");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetPartOfSpeechTestMethod()
        {
            PartOfSpeech expected = PartOfSpeech.Noun;
            PartOfSpeech actual = SSTypeSymbolAttribute.GetPartOfSpeech(SynsetType.Noun);
            Assert.AreEqual(expected, actual);

            expected = PartOfSpeech.Verb;
            actual = SSTypeSymbolAttribute.GetPartOfSpeech(SynsetType.Verb);
            Assert.AreEqual(expected, actual);

            expected = PartOfSpeech.Adjective;
            actual = SSTypeSymbolAttribute.GetPartOfSpeech(SynsetType.Adjective);
            Assert.AreEqual(expected, actual);

            expected = PartOfSpeech.Adjective;
            actual = SSTypeSymbolAttribute.GetPartOfSpeech(SynsetType.AdjectiveSatellite);
            Assert.AreEqual(expected, actual);

            expected = PartOfSpeech.Adverb;
            actual = SSTypeSymbolAttribute.GetPartOfSpeech(SynsetType.Adverb);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetPointerSymbolTestMethod()
        {
            SynsetType synsetType = SynsetType.Noun;
            PointerType actual;
            string symbol = "!";
            PointerType expected = PointerType.Antonym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "@";
            expected = PointerType.Hypernym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "@i";
            expected = PointerType.Instance_Hypernym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "~";
            expected = PointerType.Hyponym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "~i";
            expected = PointerType.Instance_Hyponym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "#m";
            expected = PointerType.Member_holonym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "#s";
            expected = PointerType.Substance_holonym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "#p";
            expected = PointerType.Part_holonym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "%m";
            expected = PointerType.Member_meronym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "%s";
            expected = PointerType.Substance_meronym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "%p";
            expected = PointerType.Part_meronym;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "=";
            expected = PointerType.Attribute;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "+";
            expected = PointerType.Derivationally_related_form;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = ";c";
            expected = PointerType.Domain_of_synset_TOPIC;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "-c";
            expected = PointerType.Member_of_this_domain_TOPIC;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = ";r";
            expected = PointerType.Domain_of_synset_REGION;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "-r";
            expected = PointerType.Member_of_this_domain_REGION;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = ";u";
            expected = PointerType.Domain_of_synset_USAGE;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);
            symbol = "-u";
            expected = PointerType.Member_of_this_domain_USAGE;
            actual = PointerSymbolAttribute.GetPointerType(symbol, synsetType);
            Assert.AreEqual(expected, actual);

            synsetType = SynsetType.Verb;
        }
    }
}
