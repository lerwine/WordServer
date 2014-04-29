using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    [Serializable]
    public class WordNetParseException : Exception
    {
        public string Pattern { get; private set; }
        public RegexOptions Options { get; private set; }
        public string SourceLine { get; private set; }
        public int Position { get; private set; }

        public WordNetParseException(string message, Regex regex, string sourceLine, int position)
            :base(message)
        {
            this.Pattern = regex.ToString();
            this.Options = regex.Options;
            this.SourceLine = sourceLine;
            this.Position = position;
        }

        public WordNetParseException()
        {
            this.Position = -1;
        }
        public WordNetParseException(string message)
            : base(message)
        {
            this.Position = -1;
        }
        public WordNetParseException(string message, Exception inner)
            : base(message, inner)
        {
            this.Position = -1;
        }
        protected WordNetParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
