namespace IcyWind.Chat.Auth.Sasl
{
    public class Anonymous : BaseAuth
    {
        public override string AuthMethod => "ANONYMOUS";

        public Anonymous(ChatClient client) : base(client, client.AuthCred) { }

        public override void HandleAuth()
        {
            ChatClient.TcpClient.SendString("<auth xmlns='urn:ietf:params:xml:ns:xmpp-sasl' mechanism='ANONYMOUS'/>");
        }
    }
}
