using Axeno.Helper;
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

namespace Axeno.Views.Pages.MainWindow
{
    /// <summary>
    /// Interaction logic for portManager.xaml
    /// </summary>
    public partial class portManager : Page
    {
        public portManager()
        {
            InitializeComponent();
        }

        private void btnlisten_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnreturn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.ClientPanel);
        }

        private void tbport_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Return) {
                if(string.IsNullOrEmpty(tbport.Text))
                {
                    return;
                }
                int portnum = Convert.ToInt32(tbport.Text);
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
