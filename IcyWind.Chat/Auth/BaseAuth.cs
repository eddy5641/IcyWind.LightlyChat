namespace IcyWind.Chat.Auth
{
    public abstract class BaseAuth
    {
        /// <summary>
        /// The XMPP connection to the server
        /// </summary>
        internal ChatClient ChatClient { get; }

        /// <summary>
        /// User's Authentication Credentials 
        /// </summary>
        internal AuthCred AuthCred { get; }

        /// <summary>
        /// Function that handles auth. Should be overriden by inherited class
        /// </summary>
        public abstract void HandleAuth();

        public abstract string AuthMethod { get; }

        protected BaseAuth(ChatClient client, AuthCred cred)
        {
            ChatClient = client;
            AuthCred = cred;
        }
    }
}
