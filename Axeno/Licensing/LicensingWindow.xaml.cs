using System;
using System.Collections.Generic;
using System.IO;
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

namespace Axeno.Licensing
{
    /// <summary>
    /// Interaction logic for LicensingWindow.xaml
    /// </summary>
    public partial class LicensingWindow : Window
    {
        public static api auth = new api(
            name: "Axeno",
            ownerid: "SUweiaBFS7",
            secret: "82a50c3044c7a08a65e9e269073de4ae1af35b8fa691f58f20edbb97617db586",
            version: "1.0"
            );
        public LicensingWindow()
        {
            InitializeComponent();
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
            WindowState = WindowState.Minimized;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(btnvalidate.Content.ToString() == "Next")
            {
                this.Hide();
                MainWindow mw = new MainWindow();
                mw.Closed += (s, w) => this.Close();
                mw.Show();
                return;
            }
            if (string.IsNullOrEmpty(tblicence.Text))
            {
                MessageBox.Show("You must enter a licence key to proceed!", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            bool validatesuccess = false;
            btnvalidate.IsEnabled = false;
            tblicence.IsEnabled = false;
            btnvalidate.Content = "Validating...";
            progressring.Visibility = Visibility.Visible;

            string key = tblicence.Text;
            await Task.Run(()=>
            {
                auth.init();
                auth.license(key);
                if (auth.response.success)
                {
                    validatesuccess = true;
                }
            });
            progressring.Visibility = Visibility.Hidden;
            if (validatesuccess)
            {
                File.WriteAllText("licence.axeno", key);
                MessageBox.Show("Validation successful! Please press 'Next' to continue.", "Authentication Success", MessageBoxButton.OK, MessageBoxImage.Information);
                btnvalidate.IsEnabled = true;
                btnvalidate.Content = "Next";
            }
            else
            {
                MessageBox.Show("Validation failed! Please check key is correct.", "Authentication Error", MessageBoxButton.OK, MessageBoxImage.Error);
                btnvalidate.IsEnabled = true;
                tblicence.IsEnabled = true;
                btnvalidate.Content = "Validate Licence";
            }
        }
    }
}
