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
using Axeno.Views.Pages.MainWindow;
using System.Windows.Shapes;
using System.IO;
using Axeno.Helper;
namespace Axeno
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowSlides.mainFrame = frmMain;
            frmMain.BeginInit();
            Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("Certificate/AxenoCert.p12"))
            {
                frmMain.Navigate(MainWindowSlides.NoClients);
            }
            else
            {
                ClientPanel cli = new ClientPanel();
                MainWindowSlides.ClientPanel = cli;
                MainWindowSlides.lvClients = cli.lvclients;
                frmMain.Navigate(cli);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch { }
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }


        private void Ellipse_MouseDown_2(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximise_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized) { this.WindowState = WindowState.Normal; return; }
            this.WindowState = WindowState.Maximized;
        }
    }
}
