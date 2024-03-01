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
            
            //Budget_Amount.TextChanged += (s, e) => UpdateButtonVisibility();
          //  Currency.TextChanged += (s, e) => UpdateButtonVisibility();

        }


        private void DB_RecordAdded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dB.ReadData();
            });

        }

        private void NewBudgetWindow_Loaded(object sender, RoutedEventArgs e)
        {
           UpdateButtonVisibility();
        }

        private void Button_InserIntoDB_Click(object sender, RoutedEventArgs e)
        {

           string budgetAmount = Budget_Amount.Text;

            bool isYearLimitParseSuccessful = int.TryParse(Year_Limit.Text, out int yearLimit);
            if (!isYearLimitParseSuccessful)
           {

               yearLimit = 0;
           }

            string budgetCategory = Budget_Category.Text;
         

            string creationDateString = Creation_Date.Text;
            string budgetStatus = Budget_Status.Text;
            string approvedBy = Approved_By.Text;
            string comment = Comment.Text;
            string currency = Currency.Text;
           
            dB.InsertData(budgetAmount, yearLimit, budgetCategory, creationDateString, budgetStatus, approvedBy, comment, currency);

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

        /*
        private void DatePickerTextBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DatePickerTextBox datePickerTextBox = new DatePickerTextBox();
            if (datePickerTextBox != null)
            {
                if (!datePicker.IsDropDownOpen)
                {
                    datePicker.IsDropDownOpen = true;
                    e.Handled = true;
                }
        
    }

    }*/
    }
}
