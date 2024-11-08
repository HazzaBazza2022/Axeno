﻿using Axeno.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
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
using System.IO;
using Axeno.Helper;

namespace Axeno.Views.Pages.MainWindow
{
    /// <summary>
    /// Interaction logic for NoClients.xaml
    /// </summary>
    public partial class NoClients : Page
    {
        public NoClients()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(btncreatecert.Content == "Finish")
            {
                MainWindowSlides.mainFrame.Navigate(MainWindowSlides.ClientPanel);
                return;
            }
            btncreatecert.Content = "Creating...";
            btncreatecert.IsEnabled = false;
            certprog.Visibility = Visibility.Visible;
            await Task.Factory.StartNew(() =>
            {
                Settings.ServerCertificate = Helper.CertificateManager.CreateCertificateAuthority("Axeno", 4096);
                Directory.CreateDirectory("Certificate");
                File.WriteAllBytes("Certificate/AxenoCert.p12", Settings.ServerCertificate.Export(X509ContentType.Pkcs12));
            });
            certprog.Visibility = Visibility.Hidden;
            btncreatecert.Content = "Finish";
            btncreatecert.IsEnabled = true;
        }
    }
}
