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
using System.Windows.Shapes;

namespace Axeno.Views.Windows
{
    /// <summary>
    /// Interaction logic for ExceptionHandler.xaml
    /// </summary>
    public partial class ExceptionHandler : Window
    {
        public ExceptionHandler()
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


        private void btnsendreport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Thanks for sending a report! It helps us improve our products.", "Report Successful", MessageBoxButton.OK, MessageBoxImage.Information);
            Environment.Exit(1);
        }

        private void btnclose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);
        }
    }
}
