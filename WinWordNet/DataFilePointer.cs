using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Erwine.Leonard.T.WordServer.WinWordNet
{
    public class DataFilePointer
    {
        public PointerType PointerType { get; set; }

        public long SynsetOffset { get; set; }

        public short Source { get; set; }

        public PartOfSpeech PartOfSpeech { get; set; }

        public short Target { get; set; }
    }
}
