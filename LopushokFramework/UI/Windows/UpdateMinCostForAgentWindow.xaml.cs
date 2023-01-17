using LopushokFramework.LopushokDataBase;
using LopushokFramework.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Lopushok.UI.Windows
{
    /// <summary>
    /// Логика взаимодействия для UpdateMinCostForAgentWindow.xaml
    /// </summary>
    public partial class UpdateMinCostForAgentWindow : Window
    {
        private readonly List<Product> _products;

        public UpdateMinCostForAgentWindow(List<Product> products)
        {
            InitializeComponent();
            _products = products;

            var count = _products.Count;
            var avgCostForAgent = products
                .Select(p => p.MinCostForAgent)
                .Aggregate((prev, current) => prev + current) / count;
            textBoxMinCostForAgent.Text = avgCostForAgent.ToString();
        }

        private async void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(textBoxMinCostForAgent.Text, out decimal minCostForAgent))
            {
                MessageBox.Show("Не удалось преобразить текст в число",
                    "Изменение мин. стоимости для агента",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            _products.ForEach(p => p.MinCostForAgent += minCostForAgent);

            try
            {
                await AppController.DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Изменение мин. стоимости для агента",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MessageBox.Show($"Мин. стоимость для агента увеличена на {minCostForAgent}",
                    "Изменение мин. стоимости для агента",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => 
            Close();
    }
}
