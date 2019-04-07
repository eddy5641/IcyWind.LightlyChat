using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcyWind.Chat.Jid
{
    public class InvalidJidException : Exception
    {
        public InvalidJidException() { }

        public InvalidJidException(string message) : base(message) { }

        public InvalidJidException(string message, Exception inner) : base(message, inner) { }
    }
}
