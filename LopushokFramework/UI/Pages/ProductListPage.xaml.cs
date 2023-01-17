using Lopushok.UI.Windows;
using LopushokFramework.LopushokDataBase;
using LopushokFramework.UI.Utilities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lopushok.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductListPage.xaml
    /// </summary>
    public partial class ProductListPage : Page
    {
        private readonly int _count = 20;
        private int _currentPage = 0;
        private int _lastPage = 0;

        public ProductListPage()
        {
            InitializeComponent();

            var productTypes = AppController.DbContext.ProductTypes.ToList();
            productTypes.Insert(0, new ProductType { Title = "Все типы" });
            comboBoxProductType.ItemsSource = productTypes;
            comboBoxProductType.SelectedIndex = 0;
            comboBoxSort.SelectedIndex = 0;
        }

        private async Task GetProducts()
        {
            var products = AppController.DbContext.Products
                .AsQueryable();

            if (comboBoxProductType.SelectedIndex > 0)
                products = products.Where(p => p.ProductTypeID == ((ProductType)comboBoxProductType.SelectedItem).ID);

            if (textBoxSearch.Text != "Введите для поиска" && !string.IsNullOrEmpty(textBoxSearch.Text))
                products = products.Where(p => p.Title.ToLower().Contains(textBoxSearch.Text.ToLower())
                    || (p.Description != null && p.Description.ToLower().Contains(textBoxSearch.Text.ToLower())));

            switch (comboBoxSort.SelectedIndex)
            {
                case 1:
                    products = !(bool)checkBoxDescending.IsChecked
                        ? products.OrderBy(p => p.Title)
                        : products.OrderByDescending(p => p.Title);
                    break;
                case 2:
                    products = !(bool)checkBoxDescending.IsChecked
                        ? products.OrderBy(p => p.ProductionWorkshopNumber)
                        : products.OrderByDescending(p => p.ProductionWorkshopNumber);
                    break;
                case 3:
                    products = !(bool)checkBoxDescending.IsChecked
                        ? products.OrderBy(p => p.MinCostForAgent)
                        : products.OrderByDescending(p => p.MinCostForAgent);
                    break;
            };

            var productList = await products.ToListAsync();
            _lastPage = (int)Math.Ceiling((decimal)productList.Count / _count) - 1;

            if (_currentPage > 0)
                btnPrevPage.Visibility = Visibility.Visible;
            if (_currentPage < _lastPage)
                btnNextPage.Visibility = Visibility.Visible;

            listViewProductList.ItemsSource = productList
                .OrderBy(p => p.ID)
                .Skip(_currentPage * _count)
                .Take(_count)
                .ToList();
        }

        private async void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxSearch.Text != "Введите для поиска" && textBoxSearch.Text != null)
            {
                _currentPage = 0;
                await GetProducts();
            }
        }

        private void textBoxSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxSearch.Text = null;
        }

        private void textBoxSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (textBoxSearch.Text != null)
                textBoxSearch.Text = "Введите для поиска";
        }

        private void listViewProductList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AppController.Frame.Navigate(new ProductCardPage(listViewProductList.SelectedItem as Product));
        }

        private void listViewProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewProductList.SelectedItems.Count > 0)
                btnUpdateMinCostForAgent.Visibility = Visibility.Visible;
            else
                btnUpdateMinCostForAgent.Visibility = Visibility.Hidden;
        }

        private async void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 0;
            await GetProducts();
        }

        private async void comboBoxProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 0;
            await GetProducts();
        }

        private async void checkBoxDescending_Unchecked(object sender, RoutedEventArgs e)
        {
            _currentPage = 0;
            await GetProducts();
        }

        private async void checkBoxDescending_Checked(object sender, RoutedEventArgs e)
        {
            _currentPage = 0;
            await GetProducts();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AppController.Frame.Navigate(new ProductCardPage(null));
        }

        private async void btnUpdateMinCostForAgent_Click(object sender, RoutedEventArgs e)
        {
            var productForUpdate = listViewProductList.SelectedItems.Cast<Product>().ToList();

            var updateMinCostForAgentWindow = new UpdateMinCostForAgentWindow(productForUpdate);
            
            if (updateMinCostForAgentWindow.ShowDialog() == true)
                await GetProducts();
        }

        private async void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                AppController.DbContext.ChangeTracker.Entries().ToList().ForEach(entity => entity.Reload());
                await GetProducts();
            }
        }

        private async void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage--;
            await GetProducts();

            if (_currentPage <= 0)
                btnPrevPage.Visibility = Visibility.Hidden;
        }

        private async void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            _currentPage++;
            await GetProducts();

            if (_currentPage >= _lastPage)
                btnNextPage.Visibility = Visibility.Hidden;
        }
    }
}
