using Lopushok.Utilities;
using Lopushok.Entities;
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
using Lopushok.UI.Windows;

namespace Lopushok.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductView.xaml
    /// </summary>
    public partial class ProductView : Page
    {
        #region Коструктор страницы ProductView

        public ProductView()
        {
            InitializeComponent();

            var allTypes = Transition.Context.ProductType.ToList();
            allTypes.Insert(0, new ProductType { Title = "Все типы" });
            TypesCBox.ItemsSource = allTypes;
            TypesCBox.SelectedIndex = 0;
            SortCBox.SelectedIndex = 0;

            ViewProduct.ItemsSource = Transition.Context.Product.ToList();
        }

        #endregion

        #region Сортировка данных в ViewProduct

        private void SortingProduct()
        {
            var itemUpdate = Transition.Context.Product.ToList();

            if (TypesCBox.SelectedIndex > 0)
                itemUpdate = itemUpdate
                    .Where(p => p.ProductTypeID == (TypesCBox.SelectedItem as ProductType).ID)
                    .ToList();

            if (SearchTBox.Text != "Введите для поиска")
                itemUpdate = itemUpdate
                    .Where(p => p.Title.ToLower().Contains(SearchTBox.Text.ToLower())
                    || p.Description.ToLower().Contains(SearchTBox.Text.ToLower()))
                    .ToList();

            switch (SortCBox.SelectedIndex)
            {
                case 1:
                    {
                        if (!(bool)DecreasingCheck.IsChecked)
                            itemUpdate = itemUpdate.OrderBy(p => p.Title).ToList();
                        else
                            itemUpdate = itemUpdate.OrderByDescending(p => p.Title).ToList();
                        break;
                    }
                case 2:
                    {
                        if (!(bool)DecreasingCheck.IsChecked)
                            itemUpdate = itemUpdate.OrderBy(p => p.ProductionWorkshopNumber).ToList();
                        else
                            itemUpdate = itemUpdate.OrderByDescending(p => p.ProductionWorkshopNumber).ToList();
                        break;
                    }
                case 3:
                    {
                        if (!(bool)DecreasingCheck.IsChecked)
                            itemUpdate = itemUpdate.OrderBy(p => p.MinCostForAgent).ToList();
                        else
                            itemUpdate = itemUpdate.OrderByDescending(p => p.MinCostForAgent).ToList();
                        break;
                    }
            }

            ViewProduct.ItemsSource = itemUpdate;

        }

        private void SearchTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTBox.Text != "Введите для поиска" && SearchTBox.Text != null)
                SortingProduct();
        }

        private void SearchTBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTBox.Text != null)
                SearchTBox.Text = "Введите для поиска";
        }

        private void SearchTBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchTBox.Text = null;
        }

        private void TypesCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
        }

        private void SortCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
        }

        private void DecreasingCheck_Checked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
        }

        private void DecreasingCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
        }

        #endregion

        #region Отображение модального окна 

        private void UpdateMinCostBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeMinCost changeMinCost = new ChangeMinCost();

            if (changeMinCost.ShowDialog() == true)
            {

            }
        }

        private void ViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewProduct.SelectedItems.Count > 1)
                UpdateMinCostBtn.Visibility = Visibility.Visible;
            else
                UpdateMinCostBtn.Visibility=Visibility.Hidden;
        }

        #endregion

        #region Переход на страницу AddEditProduct для добавления/редактирования продуктов

        private void ViewProduct_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewProduct.SelectedItem is Product tempProd)
                Transition.MainFrame.Navigate(new AddEditProduct(tempProd));
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Transition.MainFrame.Navigate(new AddEditProduct(null));
        }

        #endregion

        #region Обновление данных в ViewProduct после добавления или редактирования продукта

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                SortingProduct();
            }
        }

        #endregion
    }
}
