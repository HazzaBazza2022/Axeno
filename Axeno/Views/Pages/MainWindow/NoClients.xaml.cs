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
    /// Interaction logic for NoClients.xaml
    /// </summary>
    public partial class NoClients : Page
    {
        public NoClients()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Helper.MainWindowSlides.mainFrame.Navigate(new addDevice());
        }
    }
}
