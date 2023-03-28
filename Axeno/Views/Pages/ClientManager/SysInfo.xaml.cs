using Axeno.Helper;
using Axeno.Networking.Connection;
using Axeno.Networking.Functions.General;
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
    /// Interaction logic for SysInfo.xaml
    /// </summary>
    public partial class SysInfo : Page
    {
        public static Client Client { get; set; }
        public SysInfo(Client cli)
        {
            InitializeComponent();
            Client = cli;
            cli.SysInfo = this;
            this.lvinfo.IsEnabled = false;
            cli.Send(SystemInformation.GetInfo());
        }


    }
}
