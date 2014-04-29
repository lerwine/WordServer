using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class SSTypeSymbolAttribute : Attribute
    {
        readonly string _symbol;
        readonly PartOfSpeech _partOfSpeech;

        public SSTypeSymbolAttribute(string symbol, PartOfSpeech partOfSpeech)
        {
            this._symbol = symbol;
            this._partOfSpeech = partOfSpeech;
        }

        public string Symbol
        {
            get { return _symbol; }
        }

        public PartOfSpeech PartOfSpeech
        {
            get { return _partOfSpeech; }
        }

        public string Description { get; set; }

        public static SynsetType GetSynsetType(string symbol)
        {
            Type t = typeof(SynsetType);
            var matching = Enum.GetValues(t).OfType<SynsetType>().Select(s => new
            {
                Value = s,
                Attributes = t.GetField(Enum.GetName(t, s)).GetCustomAttributes(typeof(SSTypeSymbolAttribute), false).OfType<SSTypeSymbolAttribute>()
            }).FirstOrDefault(a => a.Attributes.Any(aa => String.Compare(aa.Symbol, symbol, true) == 0));

            if (matching == null)
                throw new ArgumentException("Invalid symbol", "symbol");

            return matching.Value;
        }

        public static WinWordNet.PartOfSpeech GetPartOfSpeech(SynsetType synsetType)
        {
            Type t = synsetType.GetType();
            SSTypeSymbolAttribute attr = t.GetField(Enum.GetName(t, synsetType)).GetCustomAttributes(typeof(SSTypeSymbolAttribute), false).OfType<SSTypeSymbolAttribute>()
                .FirstOrDefault();

            if (attr == null)
                return default(WinWordNet.PartOfSpeech);

            return attr.PartOfSpeech;
        }
    }
}
