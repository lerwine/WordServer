using System;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class PosSymbolAttribute : Attribute
    {
        readonly string _symbol;

        public PosSymbolAttribute(string symbol)
        {
            this._symbol = symbol;
        }

        public string Symbol
        {
            get { return _symbol; }
        }

        public static PartOfSpeech GetPartOfSpeech(string p)
        {
            throw new NotImplementedException();
        }
    }
}
