namespace IcyWind.Chat.Auth.Sasl
{
    internal class XRiotRSO : BaseAuth
    {
        public override string AuthMethod => "X-Riot-RSO";

        public XRiotRSO(ChatClient client) : base(client, client.AuthCred) { }

        public override void HandleAuth()
        {
            ChatClient.TcpClient.SendString($"<auth xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\" mechanism=\"{AuthMethod}\">{AuthCred.Token}</auth>");
        }
    }
}
