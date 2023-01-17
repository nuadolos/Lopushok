using Lopushok.LopushokDataBase.Entities;
using Lopushok.UI.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        public ProductListPage()
        {
            InitializeComponent();

            var productTypes = AppController.AppDbContext.ProductTypes.ToList();
            productTypes.Insert(0, new ProductType { Title = "Все типы" });
            comboBoxProductType.ItemsSource = productTypes;
            comboBoxProductType.SelectedIndex = 0;
            comboBoxSort.SelectedIndex = 0;
        }

        private async void GetProducts()
        {
            var products = AppController.AppDbContext.Products
                .Include(p => p.ProductMaterials)
                    .ThenInclude(p => p.Material)
                .Include(p => p.ProductType)
                .Include(p => p.ProductSales)
                .AsQueryable();

            if (comboBoxProductType.SelectedIndex > 0)
                products = products.Where(p => p.ProductTypeId == ((ProductType)comboBoxProductType.SelectedItem).Id);

            if (textBoxSearch.Text != "Введите для поиска")
                products = products.Where(p => p.Title.ToLower().Contains(textBoxSearch.Text.ToLower())
                    || (p.Description != null && p.Description.ToLower().Contains(textBoxSearch.Text.ToLower())));

            products = comboBoxSort.SelectedIndex switch
            {
                1 => !(bool)checkBoxDescending.IsChecked!
                    ? products.OrderBy(p => p.Title)
                    : products.OrderByDescending(p => p.Title),
                2 => !(bool)checkBoxDescending.IsChecked!
                    ? products.OrderBy(p => p.ProductionWorkshopNumber)
                    : products.OrderByDescending(p => p.ProductionWorkshopNumber),
                _ => !(bool)checkBoxDescending.IsChecked!
                    ? products.OrderBy(p => p.MinCostForAgent)
                    : products.OrderByDescending(p => p.MinCostForAgent),
            };

            listViewProductList.ItemsSource = await products.ToListAsync();
        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxSearch.Text != "Введите для поиска" && textBoxSearch.Text != null)
            {
                GetProducts();
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
            AppController.AppFrame.Navigate(new ProductCardPage(listViewProductList.SelectedItem as Product));
        }

        private void listViewProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listViewProductList.SelectedItems.Count > 0)
                btnUpdateMinCostForAgent.Visibility = Visibility.Visible;
            else
                btnUpdateMinCostForAgent.Visibility = Visibility.Hidden;
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetProducts();
        }

        private void comboBoxProductType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetProducts();
        }

        private void checkBoxDescending_Unchecked(object sender, RoutedEventArgs e)
        {
            GetProducts();
        }

        private void checkBoxDescending_Checked(object sender, RoutedEventArgs e)
        {
            GetProducts();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AppController.AppFrame.Navigate(new ProductCardPage(null));
        }

        private void btnUpdateMinCostForAgent_Click(object sender, RoutedEventArgs e)
        {
            var productForUpdate = listViewProductList.SelectedItems.Cast<Product>().ToList();
        }
    }
}
