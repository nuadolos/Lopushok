using Lopushok.LopushokDataBase.Entities;
using Lopushok.UI.Utilities;
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
                MessageBox.Show("");
                return;
            }

            _products.ForEach(p => p.MinCostForAgent += minCostForAgent);
            AppController.AppDbContext.UpdateRange(_products);
            await AppController.AppDbContext.SaveChangesAsync();

            AppController.AppFrame.GoBack();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
