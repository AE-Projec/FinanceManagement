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
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Threading;
using System.Data;
using FinanceManagement.View.UserControls;
using System.ComponentModel;

namespace FinanceManagement
{

    /// <summary>
    /// Interaktionslogik für BudgetsWindow.xaml
    /// </summary>
    public partial class BudgetsWindow : Window
    {
        DB dB = new DB();
        

        public BudgetsWindow()
        {
            InitializeComponent();
            dB.DataShouldBeReloaded += ReloadData;
            // dB.ReadData(budgetsDataGrid);
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
        public void ReloadData()
        {
            dB.ReadData(budgetsDataGrid);
        }

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


        public CustomDialog CurrentCustomDialog { get; set; }

        public DataGrid PublicDataGrid
        {
            get { return budgetsDataGrid; }
            
         }

        private void Button_Add_New_Budget_Click(object sender, RoutedEventArgs e)
        {

            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            this.Effect = blurEffect;

            OpenCustomDialog();

            this.Effect = null;
        }
        

        public void RefreshDataGrid()
        {
            var data = dB.ReadData();
            budgetsDataGrid.ItemsSource = data;
            
        }
        /*
        private void Button_Reload_Click(object sender, RoutedEventArgs e)
        {
            dB.ReadData(budgetsDataGrid);
        }*/
        //gleiche funktion wie beim aktualisieren nur alles disabled
        //schließen, löschen, vor und zurück
        private void Button_Delete_BudgetWindow_Click(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            var deleteBudget = new DeleteBudgetWindow();

            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            this.Effect = blurEffect;
            budgetsDataGrid.SelectedIndex = 0;
            
            deleteBudget.PrevBudget += DeleteBudget_PrevBudget;
            deleteBudget.NextBudget += DeleteBudget_NextBudget;

            deleteBudget.Owner = this;
            
            deleteBudget.ShowDialog();
            this.Effect = null;


            deleteBudget.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
            };
        }
       
        private void DeleteBudget_PrevBudget(DeleteBudgetWindow deleteBudget)
        {
            if (budgetsDataGrid.SelectedIndex > 0)
            {
                budgetsDataGrid.SelectedIndex -= 1;
            }
            var budget = budgetsDataGrid.SelectedItem as BudgetLimit;
            deleteBudget.ShowBudgets(budget);

        }
       
        private void DeleteBudget_NextBudget(DeleteBudgetWindow deleteBudget)
        {
            if (budgetsDataGrid.SelectedIndex + 1 < budgetsDataGrid.Items.Count - 1)
            {
                budgetsDataGrid.SelectedIndex += 1;
            }
            var budget = budgetsDataGrid.SelectedItem as BudgetLimit;
            deleteBudget.ShowBudgets(budget);

        }

        public int LastUpdatedId { get; private set; }
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = sender as DataGridRow;
            var budget = row.DataContext as BudgetLimit;
            var editBudget = new UpdateBudgetWindow();
            editBudget.PrevBudget += EditBudget_PrevBudget;
            editBudget.NextBudget += EditBudget_NextBudget;
            editBudget.Owner = this;
            editBudget.ShowBudgets(budget);

            
            editBudget.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
                SetFocusOnUpdatedItem(editBudget.LastUpdatedId);
                
            };


            editBudget.Closed += (s, args) =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    budgetsDataGrid.UnselectAll();
                    Keyboard.ClearFocus();
                }), System.Windows.Threading.DispatcherPriority.Background);
            };

        }
        //setzt den fokus auf die ID die aktualisiert wurde
        private void SetFocusOnUpdatedItem(int updatedId)
        {
            var itemToSelect = budgetsDataGrid.Items.Cast<BudgetLimit>().FirstOrDefault(item => item.BudgetID == updatedId);
            if (itemToSelect != null)
            {
                budgetsDataGrid.SelectedItem = itemToSelect;
                budgetsDataGrid.ScrollIntoView(itemToSelect);
            }
        }
        private void EditBudget_PrevBudget(UpdateBudgetWindow editBudget)
        {
            if (budgetsDataGrid.SelectedIndex > 0)
            {
                budgetsDataGrid.SelectedIndex -= 1;
                
            }
            var budget = budgetsDataGrid.SelectedItem as BudgetLimit;
            editBudget.ShowBudgets(budget);
        }

        private void EditBudget_NextBudget(UpdateBudgetWindow editBudget)
        {
            if (budgetsDataGrid.SelectedIndex + 1 < budgetsDataGrid.Items.Count - 1)
            {
                budgetsDataGrid.SelectedIndex += 1;
            }
            var budget = budgetsDataGrid.SelectedItem as BudgetLimit;
            editBudget.ShowBudgets(budget);

        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void updateBudgetBtn_Click(object sender, RoutedEventArgs e)
        {
            var row = sender as DataGridRow;
            var editBudget = new UpdateBudgetWindow();

            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            this.Effect = blurEffect;
            budgetsDataGrid.SelectedIndex = 0;
            editBudget.PrevBudget += EditBudget_PrevBudget; // Annahme, dass diese Event-Handler bereits implementiert sind
            editBudget.NextBudget += EditBudget_NextBudget;
            var firstBudget = dB.GetFirstBudgetEntry();
            editBudget.Owner = this;

            if (firstBudget != null)
            {
                editBudget.ShowBudgets(firstBudget);
            }

            editBudget.DataUpdated += (sender, e) =>
            {
                RefreshDataGrid();
                SetFocusOnUpdatedItem(editBudget.LastUpdatedId);
            };
            this.Effect = null;
        }
    }
}

