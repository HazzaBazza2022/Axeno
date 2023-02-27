using Axeno.Helper;
using Axeno.Networking.Connection;
using System;
using System.Collections.Generic;
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

namespace Axeno.Views.Pages.MainWindow
{
    /// <summary>
    /// Interaction logic for portManager.xaml
    /// </summary>
    public partial class portManager : Page
    {
        public static List<Thread> listeningThreads = new List<Thread>();

        public static List<Listener> ListenerList = new List<Listener>();

        public portManager()
        {
            InitializeComponent();
        }

        private void btnlisten_Click(object sender, RoutedEventArgs e)
        {
            if(btnlisten.Content.ToString() == "Stop Listening")
            {
                btnlisten.IsEnabled = false;
                foreach (Listener L in ListenerList)
                {
                    L.Disconnect();
                }
                foreach(Thread t in listeningThreads)
                {
                    t.Abort();
                }
                btnlisten.Content = "Start Listening";
                btnlisten.IsEnabled = true;

                lvports.IsEnabled = true;
                tbport.IsEnabled = true;
                return;
            }
            if(lvports.Items.Count > 0)
            {
                btnlisten.IsEnabled = false;
                btnlisten.Content = "Working...";
                
                foreach(var item in lvports.Items)
                {
                    Port portno = item as Port;
                    Listener listener = new Listener();
                    Thread thread = new Thread(new ParameterizedThreadStart(listener.Connect));
                    thread.IsBackground = true;
                    thread.Start(Convert.ToInt32(portno.port.ToString().Trim()));
                    listeningThreads.Add(thread);
                    ListenerList.Add(listener);
                }
                lvports.IsEnabled = false;
                tbport.IsEnabled = false;
                btnlisten.Content = "Stop Listening";
                btnlisten.IsEnabled = true;
            }else
            {
                MessageBox.Show("Please add a port first.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnreturn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.ClientPanel);
        }

        private void tbport_KeyDown(object sender, KeyEventArgs e)
        {
            int portnum = 0;
            if(e.Key == Key.Return) {
                if(string.IsNullOrEmpty(tbport.Text))
                {
                    return;
                }
                try
                {
                    portnum = Convert.ToInt32(tbport.Text);
                }
                catch
                {
                    MessageBox.Show("Please enter a valid port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!Enumerable.Range(1, 65535).Contains(portnum))
                {
                    MessageBox.Show("Please enter a valid port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                lvports.Items.Add(new Port { port = tbport.Text });
                tbport.Text = null;
            }
        }

        private void lvremove_Click(object sender, RoutedEventArgs e)
        {
            lvports.Items.Remove(lvports.SelectedItem);
        }
    }
}
