using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Client.Networking.Functions.System
{
    internal class CommandPrompt
    {
        public static void HandlePacket(MsgPack msgpck)
        {
            string status = msgpck.ForcePathObject("Status").AsString;
            if(status == "begin")
            {

            }else if (status == "end")
            {

            }
        }
        public static void BeginCMD()
        {

        }
    }
}
