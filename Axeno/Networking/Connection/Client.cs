using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Axeno.Networking.Connection
{
    public class Client
    {
        public Socket Socket { get; set; }
        public SslStream SslClient { get; set; }
        public string IP { get; set; }
        private byte[] ClientBuffer { get; set; }
        private long HeaderSize { get; set; }
        private long Offset { get; set; }
        private bool ClientBufferRecevied { get; set; }
        public object SendSync { get; set; }
        public long BytesRecevied { get; set; }
        public Client(Socket socket)
        {

            SendSync = new object();
            Socket = socket;
            IP = Socket.RemoteEndPoint.ToString().Split(':')[0];
            SslClient = new SslStream(new NetworkStream(Socket, true), false);
            SslClient.BeginAuthenticateAsServer(Settings.ServerCertificate, false, SslProtocols.Tls, false, EndAuthenticate, null);
        }
        private void EndAuthenticate(IAsyncResult ar)
        {
            try
            {
                SslClient.EndAuthenticateAsServer(ar);
                Offset = 0;
                HeaderSize = 4;
                ClientBuffer = new byte[HeaderSize];
                SslClient.BeginRead(ClientBuffer, (int)Offset, (int)HeaderSize, ReadClientData, null);
            }
            catch
            {
                SslClient?.Dispose();
                Socket?.Dispose();
            }
        }
        public void Disconnected()
        {
            Socket?.Dispose();

        }
        public void ReadClientData(IAsyncResult ar)
        {

        }
    }
}
