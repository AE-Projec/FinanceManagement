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
            dB.DataShouldBeReloaded += ReloadData;
            RefreshDataGrid();

            
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (dB != null)
            {
                dB.DataShouldBeReloaded -= ReloadData;
            }
        }
        private void ReloadData()
        {
            dB.ReadData<Revenue>(RevenuesDataGrid);
        }

        public DataGrid PublicDataGrid
        {
            get { return RevenuesDataGrid; }

        }

        /*
          void OpenCustomDialog()
        {
            var customDialog = new CustomDialog
            {
                Owner = this,
                Bw = this
            };
            CurrentCustomDialog = customDialog; // speichern der referenz
            customDialog.ShowDialog();
        }
         */
        // public CustomDialog CurrentCustomDialog { get; set; }
        public void RefreshDataGrid()
        {

            RevenuesDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            RevenuesDataGrid.CommitEdit();

            var data = dB.ReadData<Revenue>("Revenue");
            RevenuesDataGrid.ItemsSource = data;
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
            RefreshDataGrid();
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
            var row = sender as DataGridRow;
            var editRevenue = new UpdateRevenueWindow();

            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            this.Effect = blurEffect;
            RevenuesDataGrid.SelectedIndex = 0;
            editRevenue.PrevRevenue += EditRevenue_PrevRevenue;
            editRevenue.NextRevenue += EditRevenue_NextRevenue;
            var firstRevenue = dB.ReadFirstEntry<Revenue>("Revenue");
            editRevenue.Owner = this;

            if (firstRevenue != null)
            {
                editRevenue.ShowRevenues(firstRevenue);
            }

            editRevenue.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
                SetFocusOnUpdatedItem(editRevenue.LastUpdatedId);
            };
            this.Effect = null;
   
        /*
        UpdateRevenueWindow updateRevenueWindow = new UpdateRevenueWindow();
        var blurEffect = new System.Windows.Media.Effects.BlurEffect();
        blurEffect.Radius = 5;
        Effect = blurEffect;
        updateRevenueWindow.Owner = this;
        updateRevenueWindow.ShowDialog();
        Effect = null;*/
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

        //setzt den fokus auf die ID die aktualisiert wurde
        private void SetFocusOnUpdatedItem(int updatedId)
        {
            var itemToSelect = RevenuesDataGrid.Items.OfType<FinanceManagement.Revenue>().FirstOrDefault(
                item => item.RevenueID == updatedId);
            // var itemToSelect = budgetsDataGrid.Items.Cast<BudgetLimits>().FirstOrDefault(item => item.BudgetID == updatedId);
            if (itemToSelect != null)
            {
                RevenuesDataGrid.SelectedItem = itemToSelect;
                RevenuesDataGrid.ScrollIntoView(itemToSelect);
            }
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

