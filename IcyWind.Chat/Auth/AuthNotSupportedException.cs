using System;

namespace IcyWind.Chat.Auth
{
    /// <summary>
    /// Thrown when all presented SASL Methods are not supported by IcyWind.Chat
    /// </summary>
    public class AuthNotSupportedException : Exception
    {
        public AuthNotSupportedException() { }

        public AuthNotSupportedException(string message) : base(message) { }

        public AuthNotSupportedException(string message, Exception inner) : base(message, inner) { }
    }
}
