using System.Xml;
using IcyWind.Chat.Auth.Sasl;

namespace IcyWind.Chat.Auth
{
    public class AuthHandler
    {
        private ChatClient ChatClient { get; }

        public AuthHandler(ChatClient client)
        {
            ChatClient = client;
        }

        public bool HandleAuth(XmlNode xmlNode)
        {
            var authHandled = false;

            foreach (var mechanism in xmlNode.ChildNodes)
            {
                var mechanismXml = (XmlNode)mechanism;

                //Allow custom auth methods to be added
                //If a custom auth method is added, override IcyWind.Chat's
                //built in auth methods
                if (ChatClient.AuthMethod != null)
                {
                    //Check that the auth method specified is the
                    //wanted auth method
                    if (mechanismXml.InnerText != ChatClient.AuthMethod.AuthMethod)
                    {
                        continue;
                    }
                    //Handle the auth
                    authHandled = true;
                    ChatClient.AuthMethod.HandleAuth();
                    break;
                }

                //Check if the auth method is supported
                //If not continue to the next auth method
                if (!SupportedSASLAuth(mechanismXml.InnerText))
                {
                    continue;
                }
                //Handle auth
                authHandled = true;
                var authHandler = GetSASLAuthHandler(mechanismXml.InnerText);
                authHandler.HandleAuth();
                break;
            }

            if (authHandled)
                return true;

            if (ChatClient.AuthMethod != null)
            {
                throw new AuthNotSupportedException("The auth method specified is not supported by the server");
            }

            throw new AuthNotSupportedException(
                "IcyWind.Chat does not support any of the SASL auth mechanisms of the server");
        }

        /// <summary>
        /// Gets the SASL method for a given method
        /// </summary>
        /// <param name="method">The SASL Method (Case sensitive)</param>
        /// <returns>The SASL method as <see cref="BaseAuth"/></returns>
        public BaseAuth GetSASLAuthHandler(string method)
        {
            switch (method)
            {
                case "X-Riot-RSO":
                    return new XRiotRSO(ChatClient);
                case "PLAIN":
                    return new Plain(ChatClient);
                default:
                    throw new AuthNotSupportedException($"The SASL authentication ({method}) is not supported by IcyWind.Chat");
            }
        }

        public bool SupportedSASLAuth(string method)
        {
            switch (method)
            {
                case "ANONYMOUS":
                case "PLAIN":
                case "X-Riot-RSO":
                    return true;
                default:
                    return false;
            }
        }
    }
}
