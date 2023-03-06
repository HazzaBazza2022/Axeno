using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Networking.Communication
{
    internal class HandleSysInfo
    {
        public void InsertInformation(Client cli, MsgPack msgpck)
        {
            string pcname = msgpck.ForcePathObject("Name").AsString;
            string manufacturer = msgpck.ForcePathObject("Manafacturer").AsString;
            string model = msgpck.ForcePathObject("Model").AsString;
            string domain = msgpck.ForcePathObject("Domain").AsString;
            string uptime = msgpck.ForcePathObject("Uptime").AsString;
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Name", Value=pcname });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Manafacturer", Value = manufacturer });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Model", Value = model });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Domain", Value = domain });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Uptime", Value = uptime });


        }
    }
}
