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
    /// Логика взаимодействия для ChangeMinCost.xaml
    /// </summary>
    public partial class ChangeMinCost : Window
    {
        #region Состояние окна ChangeMinCost

        private decimal averageCost;
        private List<Product> lEditMinCost;

        #endregion

        #region Конструктор окна ChangeMinCost

        public ChangeMinCost(List<Product> transferEditMinCost)
        {
            InitializeComponent();

            lEditMinCost = transferEditMinCost;

            foreach (var item in lEditMinCost)
            {
                averageCost += item.MinCostForAgent;
            }

            EditMinCost.Text = (averageCost / lEditMinCost.Count).ToString();
        }

        #endregion

        #region Изменение минимальной стоимость

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            decimal tempCost;

            if (decimal.TryParse(EditMinCost.Text, out tempCost))
            {
                foreach (var item in lEditMinCost)
                {
                    item.MinCostForAgent += tempCost;
                }

                try
                {
                    Transition.Context.SaveChanges();
                    Transition.Context.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                    MessageBox.Show($"Минимальная стоимость выбранных продуктов увеличина на {tempCost}",
                        "Изменение стоимость", MessageBoxButton.OK, MessageBoxImage.Information);
                    DialogResult = true;
                    this.Close();
                }
                catch (Exception er)
                {
                    MessageBox.Show($"При изменении мин. стоимость возникла ошибка:\n{er.Message}",
                        "Изменение стоимость", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show("В поле для ввода должно быть число", 
                    "Изменение стоимость", MessageBoxButton.OK, MessageBoxImage.Error);
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
