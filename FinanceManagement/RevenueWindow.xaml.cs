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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für RevenueWindow.xaml
    /// </summary>
    public partial class RevenueWindow : Window
    {
        DB dB = new DB();
        public RevenueWindow()
        {

            InitializeComponent();
            RefreshDataGrid();
        }

        public void RefreshDataGrid()
        {

            RevenuesDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            RevenuesDataGrid.CommitEdit();
            try
            {
                var revenues = dB.ReadData<Revenue>("Revenue");
                RevenuesDataGrid.ItemsSource = revenues;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein fehler ist aufgetreten: {ex.Message}");
            }
        }

        private void Button_Add_New_Revenue_Click(object sender, RoutedEventArgs e)
        {
            NewRevenueWindow newRevenueWindow = new NewRevenueWindow();
            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            Effect = blurEffect;
            newRevenueWindow.Owner = this;
            newRevenueWindow.ShowDialog();
            Effect = null;
        }

        private void Button_Delete_Revenue_Click(object sender, RoutedEventArgs e)
        {


            DeleteRevenueWindow deleteRevenueWindow = new DeleteRevenueWindow();
             var deleteBudget = new DeleteBudgetWindow();
            var row = sender as DataGridRow;

            deleteBudget.DataDeleted += (sender, e) =>
             {
                 RefreshDataGrid();
             };
            
            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            Effect = blurEffect;
            deleteRevenueWindow.Owner = this;
            RevenuesDataGrid.SelectedIndex = 0;
            deleteRevenueWindow.ShowDialog();
            Effect = null;
        }

        private void updateRevenueBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateRevenueWindow updateRevenueWindow = new UpdateRevenueWindow();
            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            Effect = blurEffect;
            updateRevenueWindow.Owner = this;
            updateRevenueWindow.ShowDialog();
            Effect = null;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var revenue = row.DataContext as Revenue;
            var editRevenue = new UpdateRevenueWindow();
            editRevenue.PrevRevenue += EditRevenue_PrevRevenue;
            editRevenue.NextRevenue += EditRevenue_NextRevenue;
            editRevenue.Owner = this;
            editRevenue.ShowRevenues(revenue);


            editRevenue.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
            };

            //hebt den fokus auf nach dem schließen des fensters
            editRevenue.Closed += (s, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    // RevenueDataGrid.UnselectAll();
                    Keyboard.ClearFocus();
                }), System.Windows.Threading.DispatcherPriority.Background);
            };


        }

        private void EditRevenue_PrevRevenue(UpdateRevenueWindow revenues)
        {
            if (RevenuesDataGrid.SelectedIndex > 0)
            {
                RevenuesDataGrid.SelectedIndex -= 1;

            }
            var revenue = RevenuesDataGrid.SelectedItem as Revenue;
            revenues.ShowRevenues(revenue);

        }

        private void EditRevenue_NextRevenue(UpdateRevenueWindow revenues)
        {
            if (RevenuesDataGrid.SelectedIndex + 1 < RevenuesDataGrid.Items.Count - 1)
            {
                RevenuesDataGrid.SelectedIndex += 1;
            }
            var revenue = RevenuesDataGrid.SelectedItem as Revenue;
            revenues.ShowRevenues(revenue);
        }
    }
}

