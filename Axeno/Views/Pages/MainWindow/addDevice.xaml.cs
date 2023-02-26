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
    /// Interaction logic for addDevice.xaml
    /// </summary>
    public partial class addDevice : Page
    {
        public addDevice()
        {
            InitializeComponent();
        }

        private void btnreturn_Click(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.ClientPanel);
        }

        private void btnbuild_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
