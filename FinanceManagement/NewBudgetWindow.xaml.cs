using FinanceManagement.View.UserControls;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Xml.Linq;

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für NewBudgetWindow.xaml
    /// </summary>
    public partial class NewBudgetWindow : Window
    {

        DB dB = new DB();

        public NewBudgetWindow()
        {
            InitializeComponent();

            dB.RecordAdded += DB_RecordAdded;

            this.Loaded += NewBudgetWindow_Loaded;

        }

        //liest die Daten erneut aus um sie direkt anzeigen zu können.
        private void DB_RecordAdded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dB.ReadData<BudgetLimits>("BudgetLimits");
            });

        }

        private void NewBudgetWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtonVisibility();
        }

        private void Button_InserIntoDB_Click(object sender, RoutedEventArgs e)
        {
            
            var budget = new BudgetLimits
            {
                Budget_Amount = int.Parse(Budget_Amount.Text),
                Currency = Currency.Text,
                Budget_Limit_Year = string.IsNullOrEmpty(Year_Limit.Text) ? null : int.Parse(Year_Limit.Text),
                Budget_Category = Budget_Category.Text,
                Creation_Date = string.IsNullOrEmpty(Creation_Date.Text) ? null : DateOnly.Parse(Creation_Date.Text),
                Budget_Status = Budget_Status.Text,
                Approved_By = Approved_By.Text,
                Comment = Comment.Text
            
            };

            dB.InsertIntoData("BudgetLimits", budget);

            MessageBox.Show("Ein neuer Datensatz wurde hinzugefügt!");

            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            dB.RecordAdded -= DB_RecordAdded;
        }

        //Pflichtangaben
        private void UpdateButtonVisibility()
        {

            if (string.IsNullOrEmpty(Budget_Amount.Text) ||
                string.IsNullOrEmpty(Currency.Text) ||
                !IsBudgetAmountValid() ||
                !IsCurrencyValid())
            {
                insertIntoDB_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                insertIntoDB_btn.Visibility = Visibility.Visible;
            }
        }


        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private bool IsBudgetAmountValid()
        {
            return int.TryParse(Budget_Amount.Text, out _);
        }

        private void Budget_Amount_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (!IsBudgetAmountValid() && !string.IsNullOrWhiteSpace(Budget_Amount.Text))
            {
                Budget_Amount_Label.Content = "Ihr eingegebener Wert ist keine Zahl";
            }
            else
            {
                Budget_Amount_Label.Content = "";
            }
            UpdateButtonVisibility();
        }

        private bool IsCurrencyValid()
        {
            return Currency.Text.Length <= 3;
        }


        private void Currency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsCurrencyValid() && !string.IsNullOrWhiteSpace(Currency.Text))
            {
                Currency_Length.Content = "Ihre Währungseingabe ist zu lang";
            }
            else
            {
                Currency_Length.Content = "";
            }
            UpdateButtonVisibility();
        }
    }
}
