using MessagePackLib.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                    case "ClientInformation":
                        {

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
