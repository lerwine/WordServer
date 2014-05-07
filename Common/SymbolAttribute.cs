using System;
using System.Linq;

namespace Erwine.Leonard.T.WordServer.Common
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class SymbolAttribute : Attribute
    {
        readonly string _symbol;

        public SymbolAttribute(string symbol)
        {
            this._symbol = symbol;
        }

        public string Symbol
        {
            get { return _symbol; }
        }

        public string Description { get; set; }

        public static TEnum GetEnum<TEnum>(string symbol)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = typeof(TEnum);
            var matching = Enum.GetValues(t).OfType<TEnum>().Select(s => new
            {
                Value = s,
                Attributes = t.GetField(Enum.GetName(t, s)).GetCustomAttributes(typeof(SymbolAttribute), false).OfType<SymbolAttribute>()
            }).FirstOrDefault(a => a.Attributes.Any(aa => String.Compare(aa.Symbol, symbol, true) == 0));

            if (matching == null)
                throw new ArgumentException("Invalid symbol", "symbol");

            return matching.Value;
        }

        public static string GetSymbol<TEnum>(TEnum enumValue)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = enumValue.GetType();
            SymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(SymbolAttribute), false).OfType<SymbolAttribute>().FirstOrDefault();
            return (attr == null) ? null : attr.Symbol;
        }

        public static string GetDescription<TEnum>(TEnum enumValue)
        {
            Type t = enumValue.GetType();
            SymbolAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(SymbolAttribute), false).OfType<SymbolAttribute>().FirstOrDefault();
            return (attr == null) ? null : attr.Description;
        }
    }
}
