using System;
using System.Runtime.InteropServices;
using System.Text;

namespace IcyWind.Chat.Auth.Sasl
{
    public class IcyWindAuth : BaseAuth
    {
        public override string AuthMethod => "IcyWind-Auth";

        public IcyWindAuth(ChatClient client) : base(client, client.AuthCred) { }

        public override void HandleAuth()
        {
            if (AuthCred.SecurePassword == null)
            {
                throw new NullReferenceException("SecurePassword must be used for IcyWind-Auth");
            }

            var sb = new StringBuilder();
            sb.Append(AuthCred.Username);
            sb.Append('-');

            var ptr = IntPtr.Zero;
            try
            {
                ptr = Marshal.SecureStringToBSTR(AuthCred.SecurePassword);
                sb.Append(Marshal.PtrToStringUni(ptr));
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(ptr);
            }

            ChatClient.TcpClient.SendString("<auth xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\" " +
                                         $"mechanism=\"{AuthMethod}\">{sb}</auth>");

            sb.Clear();

            //This is done to clear memory
#pragma warning disable IDE0059 // Value assigned to symbol is never used
            sb = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used

            //Nuke any data we don't need to be sure we have nuked the password in the
            //StringBuilder
            GC.Collect();
        }
    }
}
