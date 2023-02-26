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
using Axeno.Helper;
namespace Axeno.Views.Pages.MainWindow
{
    /// <summary>
    /// Interaction logic for ClientPanel.xaml
    /// </summary>
    public partial class ClientPanel : Page
    {
        public ClientPanel()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.PortManager);
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindowSlides.mainFrame.Navigate(MainWindowSlides.BuildClientPanel);
        }
    }
}
