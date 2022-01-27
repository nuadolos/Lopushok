using Lopushok.Entities;
using Lopushok.Utilities;
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

namespace Lopushok.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddMaterial.xaml
    /// </summary>
    public partial class AddMaterial : Window
    {
        #region Поля окна AddMaterial

        private ProductMaterial addMaterial;
        private int checkAddOrEdit;

        #endregion

        #region Коструктор окна AddMaterial

        public AddMaterial(ProductMaterial transferProdMat, int idProduct)
        {
            InitializeComponent();

            addMaterial = transferProdMat ?? new ProductMaterial();
            checkAddOrEdit = idProduct;
            
            if (checkAddOrEdit == 0)
                MaterialCmb.IsEnabled = false;

            HeaderBlock.Text = transferProdMat != null
                ? "Редактирование материала\nк продукту" 
                : "Добавление материала\nк продукту";

            MaterialCmb.ItemsSource = Transition.Context.Material.ToList();

            DataContext = addMaterial;
        }

        #endregion

        #region Сохранение данных, связанных с материалом продукта

        private void AddMaterialBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (addMaterial.Material == null)
                error.AppendLine("Выберите материал");
            if (!int.TryParse(addMaterial.Count.ToString(), out _))
                error.AppendLine("Укажите числовое значение для количества");

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", 
                    "Сохранение материала", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (checkAddOrEdit != 0)
            {
                addMaterial.ProductID = checkAddOrEdit;
                Transition.Context.ProductMaterial.Add(addMaterial);
            }

            try
            {
                Transition.Context.SaveChanges();
                Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                MessageBox.Show($"Материал успешно прикреплен к продукту",
                    "Сохранение материала", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                this.Close();
            }
            catch (Exception er)
            {
                MessageBox.Show($"При сохранении материала возникла ошибка:\n{er.Message}",
                    "Сохранение материала", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion

        #region Закрытие окна без изменений данных

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}
