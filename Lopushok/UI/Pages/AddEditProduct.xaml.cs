using Lopushok.Entities;
using Lopushok.Utilities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Lopushok.UI.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddEditProduct.xaml
    /// </summary>
    public partial class AddEditProduct : Page
    {
        #region Состояние и свойства страницы AddEditProduct

        private Product addProduct;
        private string articleProd;

        public string Path { get { return System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "../../UI")); } }

        #endregion

        #region Конструктор страницы AddEditProduct

        public AddEditProduct(Product transferProd)
        {
            InitializeComponent();

            addProduct = transferProd ?? new Product();
            HeaderBlock.Text = transferProd != null ? "Редактирование продукта" : "Добавление продукта";
            this.Title = HeaderBlock.Text;

            if (transferProd != null)
            {
                DeleteProdBtn.Visibility = Visibility.Visible;
                articleProd = transferProd.ArticleNumber;
            }

            if (!string.IsNullOrWhiteSpace(transferProd?.Image))
            {
                ImageProduct.Source = (ImageSource)new ImageSourceConverter().ConvertFromString($@"{Path}\{transferProd.Image}");
            }

            var allTypes = Transition.Context.ProductType.ToList();
            ProdTypeCBox.ItemsSource = allTypes;

            DataContext = addProduct;
        }

        #endregion

        #region Сохранение новых данных, сформированные пользователем

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder error = new StringBuilder();

            if (string.IsNullOrWhiteSpace(addProduct.ArticleNumber))
                error.AppendLine("Укажите артикул");
            else
            {
                if (addProduct.ArticleNumber != articleProd)
                    foreach (var item in Transition.Context.Product.ToList())
                    {
                        if (item.ArticleNumber == addProduct.ArticleNumber)
                        {
                            error.AppendLine("Данный артикул имеется у другого товара");
                            break;
                        }
                    }
            }

            if (string.IsNullOrWhiteSpace(addProduct.Title))
                error.AppendLine("Укажите наименование");
            if (addProduct.ProductType == null)
                error.AppendLine("Выберите тип продукта");
            if (string.IsNullOrWhiteSpace(addProduct.ProductionPersonCount.ToString()))
                error.AppendLine("Укажите кол-во человек для производства");
            if (string.IsNullOrWhiteSpace(addProduct.ProductionWorkshopNumber.ToString()))
                error.AppendLine("Укажите номер производственного цеха");

            if (string.IsNullOrWhiteSpace(addProduct.MinCostForAgent.ToString()))
                error.AppendLine("Укажите мин. стоимость для агента");
            else if (addProduct.MinCostForAgent.ToString().StartsWith("-"))
                error.AppendLine("Стоимость не может быть отрицательной");

            if (string.IsNullOrWhiteSpace(addProduct.Description))
                addProduct.Description = "";

            if (error.Length > 0)
            {
                MessageBox.Show($"Данные не соотвествуют следующим критериям:\n{error}", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (addProduct.ID == 0)
                Transition.Context.Product.Add(addProduct);

            try
            {
                Transition.Context.SaveChanges();
                MessageBox.Show($"Данные успешно сохранены", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Information);
                Transition.MainFrame.GoBack();
            }
            catch (Exception er)
            {
                MessageBox.Show($"При сохранении продукта произошла ошибка:\n{er.Message}", "Сохранение продукта", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Добавление фото

        private void DownloadImageBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog downloadImage = new OpenFileDialog();
            downloadImage.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            downloadImage.InitialDirectory = $@"{Path}\products";

            if ((bool)downloadImage.ShowDialog())
            {
                if (!File.Exists(Path + "\\products\\" + downloadImage.SafeFileName))
                    File.Copy(downloadImage.FileName, Path + "\\products\\" + downloadImage.SafeFileName);

                addProduct.Image = $@"\products\{downloadImage.SafeFileName}";
                ImageProduct.Source = (ImageSource)new ImageSourceConverter().ConvertFromString(downloadImage.FileName);
            }
        }

        #endregion

        #region Удаление продукта из базы данных

        private void DeleteProdBtn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in Transition.Context.ProductSale.ToList())
            {
                if (item.ProductID == addProduct.ID)
                {
                    MessageBox.Show("У продукта существует информация о его продажах",
                        "Удаление продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (MessageBox.Show($"Вы хотите удалить продукт {addProduct.Title}?",
                "Удаление продукта", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Transition.Context.Product.Remove(addProduct);
                    Transition.Context.SaveChanges();
                    MessageBox.Show("Продукт успешно удален",
                        "Удаление продукта", MessageBoxButton.OK, MessageBoxImage.Information);
                    Transition.MainFrame.GoBack();
                }
                catch (Exception er)
                {
                    MessageBox.Show($"При удалении продукта возникла ошибка:\n{er.Message}",
                        "Удаление продукта", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        #endregion
    }
}
