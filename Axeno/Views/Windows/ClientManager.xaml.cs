using Axeno.Helper;
using Axeno.Networking.Connection;
using Axeno.Networking.Functions;
using Axeno.Views.Pages.ClientManager;
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
using System.Windows.Shapes;

namespace Axeno.Views.Windows
{
    /// <summary>
    /// Interaction logic for ClientManager.xaml
    /// </summary>
    public partial class ClientManager : Window
    {
        public static Client Client { get; set; }   
        public ClientManager(Client cli)
        {
            InitializeComponent();
            Client = cli;
            cli.Manager = this;
            Loaded += ClientManager_Loaded;
        }

        private void ClientManager_Loaded(object sender, RoutedEventArgs e)
        {
            ClientManagerSlides.frmdevmgr = frmdevmgr;
            frmdevmgr.Navigate(new SysInfo(Client));
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch
            {

            }
        }

        private void closebtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void minimizebtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void sysinfoclick_Click(object sender, RoutedEventArgs e)
        {
            frmdevmgr.Navigate(new SysInfo(Client));

        }

        private void reconclient_Click(object sender, RoutedEventArgs e)
        {
            Client.Send(ClientControl.Reconnect());
        }

        private void disconclient_Click(object sender, RoutedEventArgs e)
        {
            Client.Send(ClientControl.Disconnect());

        }

        private void uninclient_Click(object sender, RoutedEventArgs e)
        {
            Client.Send(ClientControl.Uninstall());

        }

        private void signout_Click(object sender, RoutedEventArgs e)
        {
            Client.Send(PowerControl.Signout());
        }

        private void shutdown_Click(object sender, RoutedEventArgs e)
        {
            Client.Send(PowerControl.Shutdown());

        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            Client.Send(PowerControl.Restart());

        }
    }
}
