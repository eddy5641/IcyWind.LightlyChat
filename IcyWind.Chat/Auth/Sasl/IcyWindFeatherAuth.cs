using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcyWind.Chat.Auth.Sasl
{
    /// <summary>
    /// Feather Auth is only available for those with
    /// a subscription. FeatherAuth uses a Hardware 2FA
    /// </summary>
    internal class IcyWindFeatherAuth : BaseAuth
    {
        public override string AuthMethod => "IcyWind-Feather-Auth";

        public IcyWindFeatherAuth(ChatClient client) : base(client, client.AuthCred) { }

        public override void HandleAuth()
        {
            //TODO: Send challenge string to FeatherAuth-Board and retrieve result
            ChatClient.TcpClient.SendString("<auth xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\" " +
                                         $"mechanism=\"{AuthMethod}\">{AuthCred.Username}-{AuthCred.ChallengeString}</auth>");

            throw new NotImplementedException("FeatherAuth is still in development. Please wait");
        }
    }
}
