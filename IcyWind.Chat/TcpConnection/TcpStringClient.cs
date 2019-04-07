using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IcyWind.Chat.TcpConnection
{
    internal class TcpStringClient
    {
        /// <summary>
        /// The main connection to the server
        /// </summary>
        internal TcpClient Client { get; set; }

        /// <summary>
        /// The stream (ssl)
        /// </summary>
        internal SslStream SslStream { get; set; }

        /// <summary>
        /// The stream (non ssl)
        /// </summary>
        internal NetworkStream NetStream { get; set; }

        /// <summary>
        /// The IP and Port of the tcp server
        /// </summary>
        internal IPEndPoint EndPoint { get; }
        internal bool UseSSL { get; }


        internal delegate bool RecString(string x);
        internal event RecString OnStringReceived;


        public TcpStringClient(IPEndPoint serverIp, bool useSSL)
        {
            //Save the IP Address
            EndPoint = serverIp;
            UseSSL = useSSL;
            //Create the TcpClient for sending data to the server
            Client = new TcpClient { NoDelay = true };
            //Set the socket option
            Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            Client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
        }



        public async Task<bool> Connect(string host)
        {
            //Connect to Server
            await Client.ConnectAsync(EndPoint.Address, EndPoint.Port);
            NetStream = Client.GetStream();

            StartReadLoop();

            return true;
        }

        public async Task<bool> ConnectSSL(SslProtocols protocol, string host)
        {
            //Connect to Server
            await Client.ConnectAsync(EndPoint.Address, EndPoint.Port);
            //Create the SslStream for talking to the rito gems server
            SslStream = new SslStream(Client.GetStream(), true, (sender, certificate, chain, errors) => true);

            //Start stream auth
            SslStream.AuthenticateAsClient(host, null, protocol, false);

            StartSSLReadLoop();

            return true;
        }


        public async Task<bool> ConnectSSL(string host)
        {
            return await ConnectSSL(SslProtocols.Tls, host);
        }

        public bool SendBytes(byte[] sendBytes)
        {
            if (UseSSL)
            {
                SslStream.Write(sendBytes);
            }
            else
            {
                Client.Client.Send(sendBytes);
            }
            return true;
        }

        public bool SendString(string sendString)
        {
            SendBytes(Encoding.ASCII.GetBytes(sendString));
            return true;
        }

        public void StartSSLReadLoop()
        {
            var t = new Thread(() =>
            {
                //Sometimes strings are sent fragmented. This stores the fragmented string
                var fragStr = string.Empty;

                //Create the data for the SslStream.Read
                var buffer = new byte[1024 * 8];
                var messageData = new StringBuilder();
                int bytes;

                do
                {
                    //Read
                    bytes = SslStream.Read(buffer, 0, buffer.Length);

                    //Decode that data
                    var decoder = Encoding.UTF8.GetDecoder();
                    var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);

                    //Temp, log the data
                    Debugger.Log(0, "", messageData + "\n");
                    //Make sure that the message actually has content, or just ignore it
                    if (!string.IsNullOrWhiteSpace(messageData.ToString()))
                    {
                        //If the buffer is full and does not end with '>' it must be a fragmented string
                        if (messageData.Length == buffer.Length && !messageData.ToString().EndsWith(">"))
                        {
                            fragStr += messageData.ToString();
                        }
                        else
                        {
                            if (OnStringReceived?.Invoke(fragStr + messageData) == true)
                            {
                                fragStr = string.Empty;
                            }
                            else
                            {
                                fragStr += messageData.ToString();
                            }
                        }
                    }

                    messageData.Clear();


                } while (bytes != 0);
            })
            { Priority = ThreadPriority.AboveNormal };
            t.Start();
        }


        public void StartReadLoop()
        {
            var t = new Thread(() =>
            {
                //Sometimes strings are sent fragmented. This stores the fragmented string
                var fragStr = string.Empty;

                //Create the data for the SslStream.Read
                var buffer = new byte[1024 * 8];
                var messageData = new StringBuilder();
                int bytes;

                do
                {
                    //Read
                    bytes = NetStream.Read(buffer, 0, buffer.Length);

                    //Decode that data
                    var decoder = Encoding.UTF8.GetDecoder();
                    var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);

                    //Temp, log the data
                    Debugger.Log(0, "", messageData + "\n");
                    //Make sure that the message actually has content, or just ignore it
                    if (!string.IsNullOrWhiteSpace(messageData.ToString()))
                    {
                        //If the buffer is full and does not end with '>' it must be a fragmented string
                        if (messageData.Length == buffer.Length && !messageData.ToString().EndsWith(">"))
                        {
                            fragStr += messageData.ToString();
                        }
                        else
                        {
                            if (OnStringReceived?.Invoke(fragStr + messageData) == true)
                            {
                                fragStr = string.Empty;
                            }
                            else
                            {
                                fragStr += messageData.ToString();
                            }
                        }
                    }

                    messageData.Clear();


                } while (bytes != 0);
            })
            { Priority = ThreadPriority.AboveNormal };
            t.Start();
        }
    }
}
