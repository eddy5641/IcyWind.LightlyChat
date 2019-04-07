using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcyWind.Chat.Jid
{
    public class InvalidJidTypeException : Exception
    {
        public InvalidJidTypeException() { }

        public InvalidJidTypeException(string message) : base(message) { }

        public InvalidJidTypeException(string message, Exception inner) : base(message, inner) { }
    }
}
