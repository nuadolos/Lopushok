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
        #region Состояние страницы ProductView

        Pagination pagination;

        #endregion

        #region Коструктор страницы ProductView

        public ProductView()
        {
            InitializeComponent();

            pagination = new Pagination(Transition.Context.Product.ToList().Count);

            var allTypes = Transition.Context.ProductType.ToList();
            allTypes.Insert(0, new ProductType { Title = "Все типы" });
            TypesCBox.ItemsSource = allTypes;
            TypesCBox.SelectedIndex = 0;
            SortCBox.SelectedIndex = 0;
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

            pagination.IsSorted(itemUpdate.Count);
            pagination.Definition(FirstNumList, SecondNumList, ThirdNumList);

            if (!pagination.HasPreviousPage && !pagination.HasNextPage)
            {
                BackList.Visibility = Visibility.Hidden;
                ForwardList.Visibility = Visibility.Hidden;
            }
            else if (!pagination.HasPreviousPage)
            {
                BackList.Visibility = Visibility.Hidden;
                ForwardList.Visibility = Visibility.Visible;
            }
            else if (!pagination.HasNextPage)
            {
                ForwardList.Visibility = Visibility.Hidden;
                BackList.Visibility = Visibility.Visible;
            }
            else
            {
                BackList.Visibility = Visibility.Visible;
                ForwardList.Visibility = Visibility.Visible;
            }

            ViewProduct.ItemsSource = pagination.LimitedList(itemUpdate);
        }

        private void SearchTBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchTBox.Text != "Введите для поиска" && SearchTBox.Text != null)
            {
                SortingProduct();
                pagination?.Zeroing();
            }
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
            pagination?.Zeroing();
        }

        private void SortCBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SortingProduct();
            pagination?.Zeroing();
        }

        private void DecreasingCheck_Checked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
            pagination?.Zeroing();
        }

        private void DecreasingCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SortingProduct();
            pagination?.Zeroing();
        }

        #endregion

        #region Отображение модального окна 

        private void UpdateMinCostBtn_Click(object sender, RoutedEventArgs e)
        {
            ChangeMinCost changeMinCost = new ChangeMinCost(ViewProduct.SelectedItems.Cast<Product>().ToList());

            if (changeMinCost.ShowDialog() == true)
            {
                SortingProduct();
            }
        }

        private void ViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewProduct.SelectedItems.Count > 1)
                UpdateMinCostBtn.Visibility = Visibility.Visible;
            else
                UpdateMinCostBtn.Visibility = Visibility.Hidden;
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

        #region Пагинация

        private void BackList_Click(object sender, RoutedEventArgs e)
        {
            pagination.GoBack();
            SortingProduct();
        }

        private void ForwardList_Click(object sender, RoutedEventArgs e)
        {
            pagination.GoForward();
            SortingProduct();
        }

        private void FirstNumList_Click(object sender, RoutedEventArgs e)
        {
            pagination.NumPage = int.Parse((sender as Button).Content.ToString()) - 1;
            pagination.GetIndex();
            SortingProduct();
        }

        private void SecondNumList_Click(object sender, RoutedEventArgs e)
        {
            pagination.NumPage = int.Parse((sender as Button).Content.ToString()) - 1;
            pagination.GetIndex();
            SortingProduct();
        }

        private void ThirdNumList_Click(object sender, RoutedEventArgs e)
        {
            pagination.NumPage = int.Parse((sender as Button).Content.ToString()) - 1;
            pagination.GetIndex();
            SortingProduct();
        }

        #endregion
    }
}
