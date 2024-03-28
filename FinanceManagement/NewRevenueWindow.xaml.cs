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

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für NewRevenueWindow.xaml
    /// </summary>
    public partial class NewRevenueWindow : Window
    {
        DB dB = new DB();
        public NewRevenueWindow()
        {
            InitializeComponent();
            dB.RecordAdded += DB_RecordAdded;
            this.Loaded += NewRevenueWindow_Loaded;
        }

        private void NewRevenueWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtonVisibility();
            
        }
        
        private void DB_RecordAdded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dB.ReadData<Revenue>("Revenue");
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);   
            dB.RecordAdded -= DB_RecordAdded;
        }


        private void insertIntoDB_btn_Click(object sender, RoutedEventArgs e)
        {
            var revenue = new Revenue
            {
                Amount = decimal.Parse(Amount.Text),
                Currency = Currency.Text,
                TransactionDate = string.IsNullOrEmpty(TransactionDate.Text) ? null : DateTime.ParseExact(TransactionDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Category =  Category.Text,
                Description = Description.Text,
                PaymentMethod = PaymentMethod.Text,
                TransactionType = TransactionType.Text,
            };

            dB.InsertIntoData("Revenue", revenue);

            MessageBox.Show("Ein neuer Datensatz wurde hinzugefügt!");

            this.Close();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TransactionType_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        private bool IsRevenueAmountValid()
        {
            return decimal.TryParse(Amount.Text, out _);
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsRevenueAmountValid() && !string.IsNullOrWhiteSpace(Amount.Text))
            {
                amountLabel.Content = "Ihr eingegebener Wert ist keine Zahl";
            }
            else
            {
                amountLabel.Content = "";
            }
            UpdateButtonVisibility();
        }
        private bool IsCurrencyValid()
        {
            return Currency.Text.Length <= 3;
        }
        private void Currency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsCurrencyValid()&&!string.IsNullOrWhiteSpace(Currency.Text))
            {
                currencyLabel.Content = "Ihre Währungseingabe ist zu lang";
            }
            else
            {
                currencyLabel.Content = "";
            }
            UpdateButtonVisibility();
        }


        private void UpdateButtonVisibility()
        {

            if (string.IsNullOrEmpty(Amount.Text) ||
                string.IsNullOrEmpty(Currency.Text) ||
                !IsRevenueAmountValid() ||
                !IsCurrencyValid())
            {
                insertIntoDB_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                insertIntoDB_btn.Visibility = Visibility.Visible;
            }
        }
    }
}
