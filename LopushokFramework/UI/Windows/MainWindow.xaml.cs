using Lopushok.UI.Pages;
using LopushokFramework.UI.Utilities;
using System.Windows;

namespace LopushokFramework
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            mainFrame.Navigate(new ProductListPage());
            AppController.Frame = mainFrame;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            AppController.Frame.GoBack();
        }

        private void mainFrame_ContentRendered(object sender, System.EventArgs e)
        {
            if (AppController.Frame.CanGoBack)
                btnBack.Visibility = Visibility.Visible;
            else
                btnBack.Visibility = Visibility.Hidden;
        }
    }
}
