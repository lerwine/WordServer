using System;
using System.Linq;

namespace Erwine.Leonard.T.WordServer.Common
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class PosValueAttribute : Attribute
    {
        readonly PartOfSpeech _partOfSpeech;

        public PosValueAttribute(PartOfSpeech partOfSpeech)
        {
            this._partOfSpeech = partOfSpeech;
        }

        public PartOfSpeech PartOfSpeech
        {
            get { return _partOfSpeech; }
        }

        public string Description { get; set; }

        public static PartOfSpeech GetPartOfSpeech<TEnum>(TEnum enumValue)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            Type t = enumValue.GetType();
            PosValueAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosValueAttribute), false).OfType<PosValueAttribute>().FirstOrDefault();
            return (attr == null) ? default(PartOfSpeech) : attr.PartOfSpeech;
        }

        public static string GetDescription<TEnum>(TEnum enumValue)
        {
            Type t = enumValue.GetType();
            PosValueAttribute attr = t.GetField(Enum.GetName(t, enumValue)).GetCustomAttributes(typeof(PosValueAttribute), false).OfType<PosValueAttribute>().FirstOrDefault();
            return (attr == null) ? null : attr.Description;
        }
    }
}
