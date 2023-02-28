﻿using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Axeno.Helper;
using System.Web.Configuration;
using Axeno.Views.Pages.MainWindow;
using System.Windows.Threading;

namespace Axeno.Networking.Communication
{
    public class Packet
    {
        public Client client;
        public byte[] data;

        public void Read(object o)
        {
            try
            {
                MsgPack msgpck = new MsgPack();
                msgpck.DecodeFromBytes(data);
                switch (msgpck.ForcePathObject("Packet").AsString)
                {
                    case "ClientInformation":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                MainWindowSlides.ClientPanel.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {

                                new HandleListView().AddItem(client, msgpck);
                                }));
                            });
                            break;
                        }
                }
            }
            catch
            {
                return;
            }
        }
    }
}
