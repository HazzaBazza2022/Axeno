using Axeno.Helper;
using Axeno.Networking.Connection;
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
    }
}
