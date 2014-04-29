using System;
using System.Linq;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class PointerSymbolAttribute : Attribute
    {
        readonly string _symbol;
        readonly PartOfSpeech _partOfSpeech;

        public PointerSymbolAttribute(string symbol, PartOfSpeech partOfSpeech)
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

        public static PointerType GetPointerType(string symbol, SynsetType synsetType)
        {
            PartOfSpeech pos = SSTypeSymbolAttribute.GetPartOfSpeech(synsetType);

            Type t = typeof(PointerType);
            var matching = Enum.GetValues(t).OfType<PointerType>().Select(s => new
            {
                Value = s,
                Attributes = t.GetField(Enum.GetName(t, s)).GetCustomAttributes(typeof(PointerSymbolAttribute), false).OfType<PointerSymbolAttribute>()
            }).FirstOrDefault(a => a.Attributes.Any(aa => String.Compare(aa.Symbol, symbol, true) == 0 && aa.PartOfSpeech == pos));

            if (matching == null)
                throw new ArgumentException("Invalid symbol", "symbol");

            return matching.Value;
        }
    }
}
