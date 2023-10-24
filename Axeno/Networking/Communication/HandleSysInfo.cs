using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using ControlzEx.Standard;
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
            cli.SysInfo.progring.Visibility = System.Windows.Visibility.Hidden;
            string pcname = msgpck.ForcePathObject("Name").AsString;
            string manufacturer = msgpck.ForcePathObject("Manafacturer").AsString;
            string model = msgpck.ForcePathObject("Model").AsString;
            string domain = msgpck.ForcePathObject("Domain").AsString;
            string uptime = msgpck.ForcePathObject("Uptime").AsString;
            string cpu = msgpck.ForcePathObject("CPU").AsString;
            string gpu = msgpck.ForcePathObject("GPU").AsString;
            string ram = msgpck.ForcePathObject("RAM").AsString;
            string monitor = msgpck.ForcePathObject("Monitor").AsString;
            string os = msgpck.ForcePathObject("OS").AsString;
            string osv = msgpck.ForcePathObject("OSV").AsString;

            string arc = msgpck.ForcePathObject("OSarc").AsString;
            string storagedev = msgpck.ForcePathObject("Storage").AsString;
            string[] storage_devices = storagedev.Split(new[] { "(SPLIT)" }, StringSplitOptions.None);

            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Name", Value=pcname });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Manafacturer", Value = manufacturer });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Model", Value = model });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Domain", Value = domain });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Computer Uptime", Value = uptime });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Processor", Value = cpu });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Graphics Card", Value = gpu });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "RAM", Value = ram });

            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Monitor", Value = monitor });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Operating System", Value = os });
            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Operating System Version", Value = osv });

            cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Architecture", Value = arc });
            AddStorage(storage_devices, cli);
            cli.SysInfo.progring.Visibility = System.Windows.Visibility.Hidden;
            cli.SysInfo.lvinfo.IsEnabled = true;


        }
        public static void AddStorage(string[] storageDevices, Client cli)
        {
            foreach (string s in storageDevices)
            {
                cli.SysInfo.lvinfo.Items.Add(new SysInfoLV { ItemName = "Storage Device", Value = s });

            }
        }
    }
}
