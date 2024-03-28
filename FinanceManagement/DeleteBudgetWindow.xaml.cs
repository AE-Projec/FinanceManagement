using System;
using System.Collections.Generic;
using System.DirectoryServices;
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

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für DeleteBudgetWindow.xaml
    /// </summary>
    public partial class DeleteBudgetWindow : Window
    {
        DB db = new DB();
        public event EventHandler DataDeleted;
        //NewBudgetWindow budgetWindow = new NewBudgetWindow();

        public event Action<DeleteBudgetWindow> NextBudget;
        public event Action<DeleteBudgetWindow> PrevBudget;

        public BudgetLimits budgetLimit { get; set; }
        // public int CurrentIndex {get; set;}
        public DeleteBudgetWindow()
        {
            InitializeComponent();
            LoadFirstBudgetEntry();
            db.RecordRemoved += Db_RecordRemoved;

        }

        private void Db_RecordRemoved(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                db.ReadData<BudgetLimits>("BudgetLimits");
            });
        }
        
        public void ShowBudgets(BudgetLimits budgets)
        {
            budgetLimit = budgets;
            BudgetID.Text = $"{budgetLimit.BudgetID}";
            Budget_Amount.Text = $"{budgetLimit.Budget_Amount}";
            Currency.Text = $"{budgetLimit.Currency.Trim()}";
            Year_Limit.Text = $"{budgetLimit.Budget_Limit_Year}";
            Budget_Category.Text = $"{budgetLimit.Budget_Category}";
            Creation_Date.Text = $"{budgetLimit.Creation_Date}";
            Budget_Status.Text = $"{budgetLimit.Budget_Status}";
            Approved_By.Text = $"{budgetLimit.Approved_By}";
            Comment.Text = $"{budgetLimit.Comment}";
            Show();
        }

        private void LoadFirstBudgetEntry()
        {

            var firstBudget = db.ReadFirstEntry<BudgetLimits>("BudgetLimits");
            //var firstBudget = db.GetFirstBudgetEntry();
            
            if (firstBudget != null)
            {
                
                BudgetID.Text = firstBudget.BudgetID.ToString();
                Budget_Amount.Text = firstBudget.Budget_Amount?.ToString() ?? "";
                Currency.Text = firstBudget.Currency ?? "";
                Year_Limit.Text = firstBudget.Budget_Limit_Year?.ToString() ?? "";
                Budget_Category.Text = firstBudget.Budget_Category ?? "";
                Creation_Date.Text = firstBudget.Creation_Date.HasValue ? firstBudget.Creation_Date.Value.ToString("dd.MM.yyyy") : "";
                Budget_Status.Text = firstBudget.Budget_Status ?? "";
                Approved_By.Text = firstBudget.Approved_By ?? "";
                Comment.Text = firstBudget.Comment ?? "";
            }
        }

        
        public int LastDeletedId { get; private set; }
        private void deleteEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            
            int budgetId = Convert.ToInt32(BudgetID.Text);
            db.DeleteData<BudgetLimits>("BudgetLimits", "BudgetID", budgetId);
            LastDeletedId = budgetId;

            DataDeleted?.Invoke(this, EventArgs.Empty);
            MessageBox.Show($"Datensatz {budgetId} erfolgreich aus der Datenbank entfernt.");

        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void previousEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            PrevBudget.Invoke(this);
        }

        private void nextEntry_btn_Click(object sender, RoutedEventArgs e)
        { 
            NextBudget.Invoke(this);
        }
        private void UpdateBudgetDisplay()
        {
  
        }
    }
}
