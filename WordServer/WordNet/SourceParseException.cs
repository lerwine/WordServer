using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WordServer.WordNet
{
    [Serializable]
    public class SourceParseException : Exception
    {
        public int Position { get; private set; }

        public string Line { get; private set; }

        public SourceParseException() { }
        public SourceParseException(string message, int position, string line)
            : base(message)
        {
            this.Position = position;
            this.Line = line;
        }
        public SourceParseException(string message, int position, string line, Exception inner)
            : base(message, inner)
        {
            this.Position = position;
            this.Line = line;
        }
        protected SourceParseException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
