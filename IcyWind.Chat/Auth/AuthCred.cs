using System;
using System.Security;

namespace IcyWind.Chat.Auth
{
    public class AuthCred
    {
        /// <summary>
        /// The user's username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// (Obsolete) The users password - prefer SecurePassword
        /// </summary>
        [Obsolete("Use the SecurePassword for added security")]
        public string Password { get; set; }

        /// <summary>
        /// The user's password
        /// </summary>
        public SecureString SecurePassword { get; set; }

        /// <summary>
        /// This is used for token based auth methods (ex. RSO)
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// This is used for Sasl when a challenge string is given
        /// </summary>
        public string ChallengeString { get; set; }
    }
}
