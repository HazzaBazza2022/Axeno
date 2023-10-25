using Axeno.Licensing;
using System.IO;
using System.Windows;
using Axeno.Views.Pages.MainWindow;
using Axeno.Helper;
using Axeno.Views.Windows;
using System;

namespace Axeno
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow MW = new MainWindow();

        public static api auth = new api(
    name: "Axeno",
    ownerid: "SUweiaBFS7",
    secret: "82a50c3044c7a08a65e9e269073de4ae1af35b8fa691f58f20edbb97617db586",
    version: "1.0"
    );
        private void Application_Startup(object sender, StartupEventArgs e)
        {

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            MainWindowSlides.BuildClientPanel = new addDevice();
            MainWindowSlides.PortManager = new portManager();
            MainWindowSlides.NoClients = new NoClients();
            MainWindowSlides.ClientPanel = new ClientPanel();

            MW.Show();

            //auth.init();
            //if (File.Exists("licence.axeno"))
            //{
            //    string key = File.ReadAllText("licence.axeno");
            //    auth.license(key);
            //    if (auth.response.success)
            //    {
            //        MW.Show();
            //        return;
            //    }
            //    else
            //    {
            //        //File.Delete("licence.axeno");
            //        MessageBox.Show("Authentication failed. If you would like to request a Hardware ID reset, please contact Axeno Support.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //LicensingWindow licensing = new LicensingWindow();
            //licensing.Show();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MW.Hide();

            new ExceptionHandler().Show();
        }
    }
}
