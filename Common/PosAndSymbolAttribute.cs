using System;
using System.Linq;

namespace Erwine.Leonard.T.WordServer.Common
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class PosAndSymbolAttribute : Attribute
    {
        readonly string _symbol;
        readonly PartOfSpeech _partOfSpeech;

        public PosAndSymbolAttribute(PartOfSpeech partOfSpeech, string symbol)
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

        public static TEnum GetEnum<TEnum>(string symbol)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = typeof(TEnum);
            var matching = Enum.GetValues(t).OfType<TEnum>().Select(s => new
            {
                Value = s,
                Attributes = t.GetField(Enum.GetName(t, s)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
            }).FirstOrDefault(a => a.Attributes.Any(aa => String.Compare(aa.Symbol, symbol, true) == 0));

            if (matching == null)
                throw new ArgumentException("Invalid symbol", "symbol");

            return matching.Value;
        }

        public static TEnum GetEnum<TEnum>(string symbol, PartOfSpeech partOfSpeech)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = typeof(TEnum);
            var matching = Enum.GetValues(t).OfType<TEnum>().Select(s => new
            {
                Value = s,
                Attributes = t.GetField(Enum.GetName(t, s)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
            }).FirstOrDefault(a => a.Attributes.Any(aa => aa.PartOfSpeech == partOfSpeech && String.Compare(aa.Symbol, symbol, true) == 0));

            if (matching == null)
                throw new ArgumentException("Invalid symbol", "symbol");

            return matching.Value;
        }

        public static string GetDescription<TEnum>(TEnum enumValue)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = enumValue.GetType();
            PosAndSymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>().FirstOrDefault();
            return (attr == null) ? null : attr.Description;
        }

        public static string GetDescription<TEnum>(TEnum enumValue, PartOfSpeech partOfSpeech)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = enumValue.GetType();
            PosAndSymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
                .FirstOrDefault(a => a.PartOfSpeech == partOfSpeech);
            return (attr == null) ? null : attr.Description;
        }

        public static string GetSymbol<TEnum>(TEnum enumValue)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = enumValue.GetType();
            PosAndSymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>().FirstOrDefault();
            return (attr == null) ? null : attr.Symbol;
        }

        public static string GetSymbol<TEnum>(TEnum enumValue, PartOfSpeech partOfSpeech)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = enumValue.GetType();
            PosAndSymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
                .FirstOrDefault(a => a.PartOfSpeech == partOfSpeech);
            return (attr == null) ? null : attr.Symbol;
        }

        public static PartOfSpeech GetPartOfSpeech<TEnum>(TEnum enumValue)
        {
            Type t = enumValue.GetType();
            PosAndSymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
                .FirstOrDefault();

            if (attr == null)
                return default(PartOfSpeech);

            return attr.PartOfSpeech;
        }

        public static bool HasPartOfSpeech<TEnum>(TEnum enumValue, PartOfSpeech partOfSpeech)
        {
            Type t = enumValue.GetType();
            return t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
                .Any(a => a.PartOfSpeech == partOfSpeech);
        }

        public static bool HasSymbol<TEnum>(TEnum enumValue, string symbol)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            if (symbol == null)
                return false;

            Type t = enumValue.GetType();
            return t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosAndSymbolAttribute), false).OfType<PosAndSymbolAttribute>()
                .Any(a => String.Compare(a.Symbol, symbol) == 0);
        }
    }
}
