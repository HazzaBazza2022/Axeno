﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using Axeno.Views.Pages.MainWindow;

namespace Axeno.Networking.Connection
{
    public class Listener
    {
        private Socket ClientSocket { get; set; }
        private bool Listening = true;
        public async void Connect(object port)
        {
            try
            {
                ClientSocket = null;
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, Convert.ToInt32(port));
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                {
                    SendBufferSize = 50 * 1024,
                    ReceiveBufferSize = 50 * 1024,
                };
                ClientSocket.Bind(ipEndPoint);
                ClientSocket.Listen(500);
                while (Listening)
                {
                    await Task.Delay(500);
                    ClientSocket.BeginAccept(EndAccept, null);
                }
            }
            catch (Exception ex)
            {
                Disconnect();
                return;
            }
        }
        private void EndAccept(IAsyncResult ar)
        {
            if (!Listening) return;
            try
            {
                new Client(ClientSocket.EndAccept(ar));
            }
            catch
            {
                ClientSocket.BeginAccept(EndAccept, null);
            }
        }
        public void Disconnect()
        {
            try
            {
                Listening = false;
                ClientSocket.Disconnect(true);
            }
            catch
            {
                ClientSocket.Close();
                return;
            }
        }
    }
}
