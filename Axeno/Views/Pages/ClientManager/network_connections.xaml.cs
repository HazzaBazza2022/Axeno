using Axeno.Networking.Connection;
using Axeno.Networking.Functions.General;
using Axeno.Networking.Functions.Networking;
using Axeno.Networking.Functions.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Axeno.Views.Pages.ClientManager
{
    /// <summary>
    /// Interaction logic for network_connections.xaml
    /// </summary>
    public partial class network_connections : Page
    {

        public static Client Client { get; set; }
        public network_connections(Client cli)
        {
            InitializeComponent();
            Client = cli;
            cli.netCon = this;
            this.lvinfo.IsEnabled = false;
            cli.QueueCommand(NetworkConnections.Send());
        }
        private void refresh_list_Click(object sender, RoutedEventArgs e)
        {
            Client.QueueCommand(NetworkConnections.Send());
            lvinfo.Items.Clear();
            lvinfo.IsEnabled = false;
            progring.Visibility = Visibility.Visible;
        }
    }
}
