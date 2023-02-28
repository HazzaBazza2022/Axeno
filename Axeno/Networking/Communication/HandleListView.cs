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
            MainWindowSlides.lvClients.Items.Add(new ClientsLV { groupName = group, appLevel = permissions, installDate = installdate, clientName = username, isOnline = "True", ping = "N/A", Socket = cli.Socket });
        }

    }
}
