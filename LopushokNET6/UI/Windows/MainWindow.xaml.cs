using Lopushok.UI.Pages;
using Lopushok.UI.Utilities;
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

namespace Lopushok
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mainFrame.Navigate(new ProductListPage());
            AppController.AppFrame = mainFrame;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppController.AppFrame.GoBack();
        }

        private void mainFrame_ContentRendered(object sender, System.EventArgs e)
        {
            if (AppController.AppFrame.CanGoBack)
                btnBack.Visibility = Visibility.Visible;
            else
                btnBack.Visibility = Visibility.Hidden;
        }
    }
}
