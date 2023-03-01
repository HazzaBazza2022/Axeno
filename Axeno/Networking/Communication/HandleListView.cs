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
    internal class HandleListView
    {
        public void AddItem(Client cli, MsgPack msgpck)
        {
            string group = msgpck.ForcePathObject("Group").AsString;
            string permissions = msgpck.ForcePathObject("Applevel").AsString;
            string installdate = msgpck.ForcePathObject("Instdate").AsString;
            string username = msgpck.ForcePathObject("Username").AsString;
            string os = msgpck.ForcePathObject("Operatingsystem").AsString;
            string version = msgpck.ForcePathObject("Version").AsString;
            
            ClientsLV thisclient = new ClientsLV();
            thisclient.uid = GetUID();
            thisclient.version = version;
            thisclient.groupName = group;
            thisclient.appLevel = permissions;
            thisclient.installDate = installdate;
            thisclient.clientName = username;
            thisclient.Socket = cli.Socket;
            thisclient.operatingSystem = os;
            thisclient.ping = "N/A";
            MainWindowSlides.lvClients.Items.Add(thisclient);
            cli.CurrentClient = thisclient;
        }
        private static Random random = new Random();

        public string GetUID()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
