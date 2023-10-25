using System;
using System.Collections.Generic;
using System.Linq;
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
using Axeno.Helper;
using Axeno.Networking.Connection;
using Axeno.Networking.Functions;
using Axeno.Views.Windows;

namespace Axeno.Views.Pages.MainWindow
{
    /// <summary>
    /// Interaction logic for ClientPanel.xaml
    /// </summary>
    public partial class ClientPanel : Page
    {
        public ClientPanel()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.PortManager);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.BuildClientPanel);
        }

        private void lvclients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        /// <summary>
        /// This is the reconnect function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if(lvclients.SelectedItems.Count > 0)
            {
                ClientsLV cli = lvclients.SelectedItems[0] as ClientsLV;
                cli.Client.QueueCommand(ClientControl.Reconnect());
                lvclients.Items.Remove(cli);
            }
        }

        private void disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (lvclients.SelectedItems.Count > 0)
            {
                ClientsLV cli = lvclients.SelectedItems[0] as ClientsLV;
                cli.Client.QueueCommand(ClientControl.Disconnect());
            }
        }

        private void uninstall_Click(object sender, RoutedEventArgs e)
        {
            if (lvclients.SelectedItems.Count > 0)
            {
                ClientsLV cli = lvclients.SelectedItems[0] as ClientsLV;
                cli.Client.QueueCommand(ClientControl.Uninstall());
            }
        }

        private void manageDevice_Click(object sender, RoutedEventArgs e)
        {
            if (lvclients.SelectedItems.Count > 0)
            {
                ClientsLV cli = lvclients.SelectedItems[0] as ClientsLV;
                if (cli.Client.Manager == null)
                {
                    Windows.ClientManager mngr = new Windows.ClientManager(cli.Client);
                    mngr.Show();
                }
            }
        }

        private void lvclients_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvclients.SelectedItems.Count > 0)
            {
                ClientsLV cli = lvclients.SelectedItems[0] as ClientsLV;
                if (cli.Client.Manager == null)
                {
                    Windows.ClientManager mngr = new Windows.ClientManager(cli.Client);
                    mngr.Show();
                }
            }
        }
    }
}
