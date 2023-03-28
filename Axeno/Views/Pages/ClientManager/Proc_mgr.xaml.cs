using Axeno.Helper;
using Axeno.Networking.Connection;
using Axeno.Networking.Functions.System;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Axeno.Views.Pages.ClientManager
{
    /// <summary>
    /// Interaction logic for Proc_mgr.xaml
    /// </summary>
    public partial class Proc_mgr : Page
    {
        public static Client Socket { get; set; }
        private DispatcherTimer debounceTimer;
        private const int DebounceDelay = 500; // milliseconds
        public Proc_mgr(Client cli)
        {
            InitializeComponent();
            Socket = cli;
            lvinfo.Items.Filter = ProcNameFilter;
            cli.Proc_mgr = this;
            this.lvinfo.IsEnabled = false;
            cli.Send(ProcessManager.SendCommand());
            debounceTimer = new DispatcherTimer();
            debounceTimer.Interval = TimeSpan.FromMilliseconds(DebounceDelay);
            debounceTimer.Tick += DebounceTimer_Tick;

        }
        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            debounceTimer.Stop();
            lvinfo.Items.Filter = GetFilter();
        }
        private void tbsortlv_TextChanged(object sender, TextChangedEventArgs e)
        {
            debounceTimer.Stop();
            debounceTimer.Start();
        }
        private bool ProcNameFilter(object obj)
        {
            
            var filterobj = obj as ProcessManagerLV;
            return filterobj.processName.ToLower().Contains(tbsortlv.Text);
        }
        public Predicate<object> GetFilter()
        {
            return ProcNameFilter;
        }

        private void kill_proc_Click(object sender, RoutedEventArgs e)
        {
            if(lvinfo.SelectedItem!= null)
            {
                ProcessManagerLV proc = lvinfo.SelectedItem as ProcessManagerLV;
                ProcessManager.KillProcess(proc.processID, Socket);
            }

        }

        private void kill_proc_tree_Click(object sender, RoutedEventArgs e)
        {
            if (lvinfo.SelectedItem != null)
            {
                ProcessManagerLV proc = lvinfo.SelectedItem as ProcessManagerLV;
                ProcessManager.KillProcessTree(proc.processName, Socket);
            }
        }

        private void refresh_list_Click(object sender, RoutedEventArgs e)
        {
            Socket.Send(ProcessManager.SendCommand());
            lvinfo.Items.Clear();
            lvinfo.IsEnabled = false;
            progring.Visibility = Visibility.Visible;
            tbsortlv.Text = null;
        }
    }
}
