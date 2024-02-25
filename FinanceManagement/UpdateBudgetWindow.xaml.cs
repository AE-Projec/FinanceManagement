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

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für UpdateBudgetWindow.xaml
    /// </summary>
    public partial class UpdateBudgetWindow : Window
    {
        DB db = new DB();
        
        public event EventHandler DataUpdated;

        public event Action<UpdateBudgetWindow> NextBudget;
        public event Action<UpdateBudgetWindow> PrevBudget;
        

        public BudgetLimit budgetLimit { get; set; }


        public void ShowBudgets(BudgetLimit budgets)
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




            //BudgetID.Text = $"{budgetsWindow.BudgetId}";
            //Budget_Amount.Text = $"{budgetsWindow.Budget_Amount}";
            //Currency.Text = $"{budgetsWindow.Currency.ToString().Trim()}";
            //Year_Limit.Text = $"{budgetsWindow.Budget_Limit_Year}";
            //Budget_Category.Text = $"{budgetsWindow.Budget_Category}";
            //Creation_Date.Text = $"{budgetsWindow.Creation_Date}";
            //Budget_Status.Text = $"{budgetsWindow.Budget_Status}";
            //Approved_By.Text = $"{budgetsWindow.Approved_By}";
            //Comment.Text = $"{budgetsWindow.Comment}";


            //Show();
        }

        public UpdateBudgetWindow()
        {
            InitializeComponent();

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
        public int LastUpdatedId { get; private set; }
        private void updateInDB_btn_Click(object sender, RoutedEventArgs e)
        {

            // Übertragen der UI-Änderungen in das BudgetLimit-Objekt
            if (int.TryParse(BudgetID.Text, out int budgetId))
            {
                budgetLimit.BudgetID = budgetId;
            }
            if (int.TryParse(Budget_Amount.Text, out int budgetAmount))
            {
                budgetLimit.Budget_Amount = budgetAmount;
            }
            budgetLimit.Currency = Currency.Text;
            if (int.TryParse(Year_Limit.Text, out int yearLimit))
            {
                budgetLimit.Budget_Limit_Year = yearLimit;
            }
            budgetLimit.Budget_Category = Budget_Category.Text;
            if (DateTime.TryParse(Creation_Date.Text, out DateTime creationDate))
            {
                budgetLimit.Creation_Date = DateOnly.FromDateTime(creationDate); // Annahme, dass Creation_Date vom Typ DateOnly ist
            }
            budgetLimit.Budget_Status = Budget_Status.Text;
            budgetLimit.Approved_By = Approved_By.Text;
            budgetLimit.Comment = Comment.Text;
           // UpdateBudgetWindow editBudget = new UpdateBudgetWindow();

            BudgetsWindow budgets = new BudgetsWindow();


            var dataG = budgets.budgetsDataGrid;
            //dataG.SelectedIndex = budgetId;/*
           // budgetId = dataG.SelectedIndex;




            // Aktualisieren der Daten in der Datenbank
            db.UpdateData(budgetLimit);
            LastUpdatedId = budgetId;
            DataUpdated?.Invoke(this, EventArgs.Empty);
            MessageBox.Show("Datensatz/ Datensätze Aktualisiert");
        
        }
 
    }
}

/*
 
            {
                if (budgetsDataGrid.SelectedIndex > 0)
                {
                    budgetsDataGrid.SelectedIndex -= 1;
                    var budget = budgetsDataGrid.SelectedItem as BudgetLimit;
                    editBudget.ShowBudgets(budget);

                }*/
/*

private void SearchValuebtn_Click(object sender, RoutedEventArgs e)
{
if(ColumnSelectionComboBox.SelectedItem != null && !string.IsNullOrEmpty(SearchValueTextBox.Text))
{
string? selectedColumnName = GetTechnicalColumnName(ColumnSelectionComboBox.SelectedItem.ToString());
string? searchValue = SearchValueTextBox.Text;
Console.WriteLine(searchValue);

//aufrufen der neuen suchmethode
var result = db.SearchData(selectedColumnName, searchValue);

//anzeigen der ergebnisse im datagrid 
ResultsDataGrid.ItemsSource = result;

}
else
{
MessageBox.Show("Bitte wählen Sie eine Spalte und geben Sie einen Suchwert ein.");
}
*/

//}

/*
     List <string>columnNames = DB.ColumnNames();
foreach (string name in columnNames)
{
string germanName = Properties.Resources.ResourceManager.GetString(name, new CultureInfo("de-DE")) ?? name;
ColumnSelectionComboBox.Items.Add(germanName);
}




private string? GetTechnicalColumnName(string? germanName)
{
    var reverseMapping = columnNamesMapping.ToDictionary(x => x.Value, x => x.Key);
    //return reverseMapping.ContainsKey(germanName) ? reverseMapping[germanName] : null;
    return reverseMapping.TryGetValue(germanName, out string? value) ? value : null;
}
private void ResultDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
{
    if(e.EditAction == DataGridEditAction.Commit)
    {
        var row = e.Row.Item as BudgetLimit;
        if(row != null)
        {
            db.UpdateData(row);

        }
    }
}
} */
//}

