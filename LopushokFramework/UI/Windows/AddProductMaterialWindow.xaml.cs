using LopushokFramework.LopushokDataBase;
using LopushokFramework.UI.Utilities;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace LopushokFramework.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddProductMaterialWindow.xaml
    /// </summary>
    public partial class AddProductMaterialWindow : Window
    {
        private readonly ProductMaterial _productMaterial;

        public AddProductMaterialWindow(ProductMaterial productMaterial)
        {
            InitializeComponent();

            comboBoxMaterial.ItemsSource = AppController.DbContext.Materials
                .Where(m => !m.ProductMaterials.Any(pm => pm.ProductID == productMaterial.ProductID))
                .ToList();

            _productMaterial = productMaterial;
            DataContext = _productMaterial;
        }

        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            var error = new StringBuilder();

            if (_productMaterial.Material == null)
                error.AppendLine("Выберите материал");

            if (!int.TryParse(_productMaterial.Count.ToString(), out var _))
                error.AppendLine("Количество должно быть выражено числом");

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}",
                    "Добавление материала к продукту",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            AppController.DbContext.ProductMaterials.Add(_productMaterial);

            try
            {
                await AppController.DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Добавление материала к продукту",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Материал прикреплен к продукту в количестве {_productMaterial.Count}",
                    "Добавление материала к продукту",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) =>
            Close();
    }
}
