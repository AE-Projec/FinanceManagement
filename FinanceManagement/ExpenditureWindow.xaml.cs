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
    /// Interaktionslogik für ExpenditureWindow.xaml
    /// </summary>
    public partial class ExpenditureWindow : Window
    {
        DB dB = new DB();
        public ExpenditureWindow()
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
            dB.ReadData<Expenditure>(ExpenditureDataGrid);
        }

        public DataGrid PublicDataGrd
        {
            get { return ExpenditureDataGrid; }
        }

        public void RefreshDataGrid()
        {
            ExpenditureDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            ExpenditureDataGrid.CommitEdit();

            var data = dB.ReadData<Expenditure>("Expenditure");
            ExpenditureDataGrid.ItemsSource = data;
        }

        private void addNew_Expenditure_Click(object sender, RoutedEventArgs e)
        {
            NewExpenditureWindow newExpenditureWindow = new NewExpenditureWindow();
            var blurEffect = new BlurEffect();
            blurEffect.Radius = 5;
            Effect = blurEffect;
            newExpenditureWindow.Owner = this;
            newExpenditureWindow.ShowDialog();
            Effect = null;
            RefreshDataGrid();

        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var expenditure = row.DataContext as Expenditure;
            var editexpenditure = new UpdateExpenditureWindow();
            editexpenditure.PrevExpenditure += EditExpenditure_PrevExpenditure;
            editexpenditure.NextExpenditure += EditExpenditure_NextExpenditure;
            editexpenditure.Owner = this;
            editexpenditure.ShowExpenditures(expenditure);


            editexpenditure.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
                SetFocusOnUpdatedItem(editexpenditure.LastUpdatedId);
            };

            //hebt den fokus auf nach dem schließen des fensters
            editexpenditure.Closed += (s, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    // RevenueDataGrid.UnselectAll();
                    Keyboard.ClearFocus();
                }), System.Windows.Threading.DispatcherPriority.Background);
            };

        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void deleteExpenditureBtn_Click(object sender, RoutedEventArgs e)
        {
            var deleteExpenditure = new DeleteExpenditureWindow();
            var row = sender as DataGridRow;
            deleteExpenditure.DataDeleted += (sender, e) =>
            {
                RefreshDataGrid();
            };
            var blurEffect = new BlurEffect();
            blurEffect.Radius = 5;
            Effect = blurEffect;
            deleteExpenditure.Owner = this;
            deleteExpenditure.PrevExpenditure += DeleteExpenditure_PrevExpenditure;
            deleteExpenditure.NextExpenditure += DeleteExpenditure_NextExpenditure;
            ExpenditureDataGrid.SelectedIndex = 0;
          
            deleteExpenditure.ShowDialog();
            Effect = null;
        }

        private void DeleteExpenditure_NextExpenditure(DeleteExpenditureWindow obj)
        {
            if (ExpenditureDataGrid.SelectedIndex + 1 < ExpenditureDataGrid.Items.Count - 1)
            {
                ExpenditureDataGrid.SelectedIndex += 1;
            }
            var expenditure = ExpenditureDataGrid.SelectedItem as Expenditure;
            obj.ShowExpenditures(expenditure);
        }

        private void DeleteExpenditure_PrevExpenditure(DeleteExpenditureWindow obj)
        {
            if (ExpenditureDataGrid.SelectedIndex > 0)
            {
                ExpenditureDataGrid.SelectedIndex -= 1;

            }
            var expenditure = ExpenditureDataGrid.SelectedItem as Expenditure;
            obj.ShowExpenditures(expenditure);
        }

        private void updateExpenditureBtn_Click(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            var editExpenditure = new UpdateExpenditureWindow();

            var blurEffect = new BlurEffect();
            blurEffect.Radius = 5;
            Effect = blurEffect;
            ExpenditureDataGrid.SelectedIndex = 0;
            editExpenditure.PrevExpenditure += EditExpenditure_PrevExpenditure;
            editExpenditure.NextExpenditure += EditExpenditure_NextExpenditure;
            var firstExpenditure = dB.ReadFirstEntry<Expenditure>("Expenditure");
            editExpenditure.Owner = this;

            if (firstExpenditure != null)
            {
                editExpenditure.ShowExpenditures(firstExpenditure);
            }
            editExpenditure.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
                SetFocusOnUpdatedItem(editExpenditure.LastUpdatedId);
            };

            Effect = null;
        }

        private void EditExpenditure_NextExpenditure(UpdateExpenditureWindow obj)
        {

            if (ExpenditureDataGrid.SelectedIndex + 1 < ExpenditureDataGrid.Items.Count -1)
            {
                ExpenditureDataGrid.SelectedIndex += 1;
            }
            var expenditure = ExpenditureDataGrid.SelectedItem as Expenditure;
            obj.ShowExpenditures(expenditure);
            
        }

        private void EditExpenditure_PrevExpenditure(UpdateExpenditureWindow obj)
        {
            if (ExpenditureDataGrid.SelectedIndex > 0)
            {
                ExpenditureDataGrid.SelectedIndex -= 1;
            }

            var expenditure = ExpenditureDataGrid.SelectedItem as Expenditure;
            obj.ShowExpenditures(expenditure);
        }

        //setzt den fokus auf die ID die aktualisiert wurde
        private void SetFocusOnUpdatedItem(int updatedId)
        {
            var itemToSelect = ExpenditureDataGrid.Items.OfType<Expenditure>().FirstOrDefault(
             item => item.ExpenditureID == updatedId);
            if (itemToSelect != null)
            {
                ExpenditureDataGrid.SelectedItem = itemToSelect;
                ExpenditureDataGrid.ScrollIntoView(itemToSelect);
            }
        }
    }
}
