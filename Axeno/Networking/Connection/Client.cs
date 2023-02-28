using Axeno.Helper;
using Axeno.Networking.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using static Axeno.Helper.MainWindowSlides;
namespace Axeno.Networking.Connection
{
    public class Client
    {
        public Socket Socket { get; set; }
        public string Ping { get; set; }
        public SslStream SslClient { get; set; }
        public string IP { get; set; }
        private byte[] ClientBuffer { get; set; }
        private long HeaderSize { get; set; }
        private long Offset { get; set; }
        private bool ClientBufferRecevied { get; set; }
        public Client(Socket socket)
        {

            Socket = socket;
            IP = Socket.RemoteEndPoint.ToString().Split(':')[0];
            SslClient = new SslStream(new NetworkStream(Socket, true), false);
            SslClient.BeginAuthenticateAsServer(Settings.ServerCertificate, false, SslProtocols.Tls, false, EndAuthenticate, null);
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(GetPing);
            aTimer.Interval = 5000;
            aTimer.Enabled = true;

        }
        public void GetPing(object source, ElapsedEventArgs e)
        {
            long pingTime = 0;
            Ping pingSender = new Ping();
            IPAddress address = IPAddress.Parse(Socket.RemoteEndPoint.ToString().Split(':')[0]);
            PingReply reply = pingSender.Send(address);

            if (reply.Status == IPStatus.Success)
            {
                pingTime = reply.RoundtripTime;
            }
            ThreadPool.QueueUserWorkItem(delegate
            {

                MainWindowSlides.ClientPanel.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                    foreach(ClientsLV cli in lvClients.Items)
                    {
                        if(cli.Socket == Socket)
                        {
                            cli.ping = pingTime.ToString() + "ms";
                        }
                    }
                }));
            });

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
            try
            {
                Socket?.Dispose();
            }
            catch { }

        }
        public void ReadClientData(IAsyncResult ar)
        {
            try
            {
                if (!Socket.Connected)
                {
                    Disconnected();
                    return;
                }
                else
                {
                    int recevied = SslClient.EndRead(ar);
                    if (recevied > 0)
                    {
                        HeaderSize -= recevied;
                        Offset += recevied;
                        switch (ClientBufferRecevied)
                        {
                            case false:
                                {
                                    if (HeaderSize == 0)
                                    {
                                        HeaderSize = BitConverter.ToInt32(ClientBuffer, 0);
                                        if (HeaderSize > 0)
                                        {
                                            ClientBuffer = new byte[HeaderSize];
                                            Offset = 0;
                                            ClientBufferRecevied = true;
                                        }
                                    }
                                    else if (HeaderSize < 0)
                                    {
                                        Disconnected();
                                        return;
                                    }
                                    break;
                                }

                            case true:
                                {
                                    if (HeaderSize == 0)
                                    {
                                        ThreadPool.QueueUserWorkItem(new Packet
                                        {
                                            client = this,
                                            data = ClientBuffer,
                                        }.Read, null);
                                        Offset = 0;
                                        HeaderSize = 4;
                                        ClientBuffer = new byte[HeaderSize];
                                        ClientBufferRecevied = false;
                                    }
                                    else if (HeaderSize < 0)
                                    {
                                        Disconnected();
                                        return;
                                    }
                                    break;
                                }
                        }
                        SslClient.BeginRead(ClientBuffer, (int)Offset, (int)HeaderSize, ReadClientData, null);
                    }
                    else
                    {
                        Disconnected();
                        return;
                    }
                }
            }
            catch
            {
                Disconnected();
                return;
            }
        }

    }
}
