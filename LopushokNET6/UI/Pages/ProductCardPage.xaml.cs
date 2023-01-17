using Lopushok.LopushokDataBase;
using Lopushok.LopushokDataBase.Entities;
using Lopushok.UI.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lopushok.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для ProductCardPage.xaml
    /// </summary>
    public partial class ProductCardPage : Page
    {
        private readonly Product _product;

        public ProductCardPage(Product? product)
        {
            InitializeComponent();

            using var context = new LopushokContext();
            comboBoxProductType.ItemsSource = context.ProductTypes.ToList();

            textBlockTitlePage.Text = product != null
                ? "Редактирование продукта"
                : "Добавление продукта";

            if (product == null)
                btnDelete.Visibility = Visibility.Collapsed;

            _product = product ?? new Product();
            DataContext = _product;
        }

        private async void btnSaveChanges_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using var context = new LopushokContext();

            var error = new StringBuilder();
            if (string.IsNullOrEmpty(_product.ArticleNumber))
                error.AppendLine("Укажите артикул");

            if (!await context.Products.AllAsync(p => p.ArticleNumber != _product.ArticleNumber))
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

            _product.ProductTypeId = _product.ProductType?.Id;

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", 
                    "Сохранение продукта", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error);
                return;
            }

            if (_product.Id == 0)
                await context.AddAsync(_product);

            await context.SaveChangesAsync();

            MessageBox.Show($"Продукт успешно сохранен",
                    "Сохранение продукта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            AppController.AppFrame.GoBack();
        }

        private async void btnDelete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using var context = new LopushokContext();
            var product = await context.Products
                .FirstOrDefaultAsync(p => p.Id == _product.Id);

            if (product == null)
            {
                MessageBox.Show($"Продукта не существует",
                    "Удаление продукта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            context.Products.Remove(product);
            await context.SaveChangesAsync();

            MessageBox.Show($"Продукт успешно удален",
                    "Удаление продукта",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            AppController.AppFrame.GoBack();
        }
    }
}
