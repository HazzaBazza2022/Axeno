using Axeno.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Networking.Functions.System
{
    internal class CommandPrompt
    {
        public static byte[] Enable()
        {
            MsgPack msgpck = new MsgPack();
            msgpck.ForcePathObject("Packet").AsString = "cmd";
            msgpck.ForcePathObject("Status").AsString = "begin";

            return msgpck.Encode2Bytes();
        }
        public static byte[] Disable()
        {
            MsgPack msgpck = new MsgPack();
            msgpck.ForcePathObject("Packet").AsString = "cmd";
            msgpck.ForcePathObject("Status").AsString = "end";

            return msgpck.Encode2Bytes();
        }
    }
}
