using LopushokFramework.LopushokDataBase;
using LopushokFramework.UI.Utilities;
using LopushokFramework.UI.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Lopushok.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductCardPage.xaml
    /// </summary>
    public partial class ProductCardPage : Page
    {
        private readonly Product _product;

        public ProductCardPage(Product product)
        {
            InitializeComponent();

            comboBoxProductType.ItemsSource = AppController.DbContext.ProductTypes.ToList();
            listViewProductMaterialsList.ItemsSource = product?.ProductMaterials.ToList();

            textBlockTitlePage.Text = product != null
                ? "Редактирование продукта"
                : "Добавление продукта";

            if (product == null)
            {
                btnDelete.Visibility = Visibility.Collapsed;
                panelProductMaterails.Visibility = Visibility.Collapsed;
            }

            _product = product ?? new Product();
            DataContext = _product;
        }

        private async void btnSaveChanges_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var error = new StringBuilder();
            if (string.IsNullOrEmpty(_product.ArticleNumber))
                error.AppendLine("Укажите артикул");

            if (await AppController.DbContext.Products.AnyAsync(p => p.ArticleNumber == _product.ArticleNumber && p.ID != _product.ID))
                error.AppendLine("Артикул уже присвоен другому продукту");

            if (string.IsNullOrWhiteSpace(_product.Title))
                error.AppendLine("Укажите наименование");

            if (_product.ProductType == null)
                error.AppendLine("Выберите тип продукта");

            if (string.IsNullOrWhiteSpace(_product.ProductionPersonCount.ToString()))
                error.AppendLine("Укажите кол-во человек для производства");

            if (string.IsNullOrWhiteSpace(_product.ProductionWorkshopNumber.ToString()))
                error.AppendLine("Укажите номер производственного цеха");

            if (string.IsNullOrWhiteSpace(_product.MinCostForAgent.ToString()))
                error.AppendLine("Укажите мин. стоимость для агента");

            if (_product.MinCostForAgent.ToString().StartsWith("-"))
                error.AppendLine("Стоимость не может быть отрицательной");

            if (string.IsNullOrWhiteSpace(_product.Description))
                _product.Description = "";

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", 
                    "Сохранение продукта", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }

            if (_product.ID == 0)
                AppController.DbContext.Products.Add(_product);

            await AppController.DbContext.SaveChangesAsync();

            MessageBox.Show($"Продукт успешно сохранен",
                    "Сохранение продукта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            AppController.Frame.GoBack();
        }

        private async void btnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (await AppController.DbContext.ProductSales.AnyAsync(ps => ps.ProductID == _product.ID))
            {
                MessageBox.Show("У продукта существует информация о его продажах",
                        "Удаление продукта", 
                        MessageBoxButton.OK, 
                        MessageBoxImage.Error);
                return;
            }

            AppController.DbContext.Products.Remove(_product);

            try
            {
                await AppController.DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Удаление продукта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Продукт успешно удален",
                    "Удаление продукта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            AppController.Frame.GoBack();
        }

        private async void btnAddMaterial_Click(object sender, RoutedEventArgs e)
        {
            var addProductMaterialWindow = new AddProductMaterialWindow(new ProductMaterial {
                ProductID = _product.ID,
            });

            if (addProductMaterialWindow.ShowDialog() == true)
                listViewProductMaterialsList.ItemsSource = await AppController.DbContext.ProductMaterials
                    .Where(p => p.ProductID == _product.ID)
                    .ToListAsync();
        }

        private async void btnDeleteMaterial_Click(object sender, RoutedEventArgs e)
        {
            var productMaterialToDelete = listViewProductMaterialsList.SelectedItems.Cast<ProductMaterial>().ToList();

            if (productMaterialToDelete.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один материал продукта",
                        "Удаление материала продукта",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return;
            }

            if (MessageBox.Show($"Вы хотите удалить {productMaterialToDelete.Count} элементов?",
                "Удаление материала продукта",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.No)
                return;

            AppController.DbContext.ProductMaterials.RemoveRange(productMaterialToDelete);
            await AppController.DbContext.SaveChangesAsync();

            AppController.DbContext.ChangeTracker.Entries().ToList().ForEach(entity => entity.Reload());
            listViewProductMaterialsList.ItemsSource = await GetProductMaterialsAsync();

            MessageBox.Show("Материал успешно удален",
                        "Удаление материала продукта",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
        }

        private void btnLoadProductImage_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {
                Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                InitialDirectory = $@"{AppController.PathToPictures}\products"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var newImagePath = $"products\\{openFileDialog.SafeFileName}";

                if (!File.Exists(AppController.PathToPictures + newImagePath))
                    File.Copy(openFileDialog.FileName, AppController.PathToPictures + newImagePath);

                _product.Image = newImagePath;
            }
        }

        private async Task<List<ProductMaterial>> GetProductMaterialsAsync() =>
            await AppController.DbContext.ProductMaterials
                .Where(p => p.ProductID == _product.ID)
                .ToListAsync();
    }
}
