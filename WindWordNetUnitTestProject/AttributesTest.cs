using System;
using System.Linq;
using Erwine.Leonard.T.WordServer.WinWordNet;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WindWordNetUnitTestProject
{
    [TestClass]
    public class AttributesTest
    {
        [TestMethod]
        public void GetSynsetTypeTestMethod()
        {
            foreach (SynsetType expected in MappingHelper.SynsetTypeValues.Keys)
            {
                SynsetType actual = SSTypeSymbolAttribute.GetSynsetType(MappingHelper.SynsetTypeValues[expected]);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void GetPartOfSpeechTestMethod()
        {
            foreach (SynsetType sst in MappingHelper.SynsetTypeValues.Keys)
            {
                PartOfSpeech expected = (sst == SynsetType.AdjectiveSatellite) ? PartOfSpeech.Adjective : MappingHelper.LetterToPartOfSpeech(MappingHelper.SynsetTypeValues[sst]).Value;
                PartOfSpeech actual = SSTypeSymbolAttribute.GetPartOfSpeech(SynsetType.Noun);
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void GetPointerTypeTestMethod()
        {
            string[] allSymbols = MappingHelper.GetAllPointerSymbols();

            foreach (PartOfSpeech pos in Enum.GetValues(typeof(PartOfSpeech)).OfType<PartOfSpeech>())
            {
                

            }
        }
    }
}
