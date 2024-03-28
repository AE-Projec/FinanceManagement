using System;
using System.Collections.Generic;
using System.Configuration;
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
    /// Interaktionslogik für NewExpenditureWindow.xaml
    /// </summary>
    public partial class NewExpenditureWindow : Window
    {
        DB dB = new DB();
        public NewExpenditureWindow()
        {
            InitializeComponent();
            dB.RecordAdded += DB_RecordAdded;
            this.Loaded += NewExpenditureWindow_Loaded;
        }

        private void NewExpenditureWindow_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateButtonVisibility();
        }

        private void DB_RecordAdded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dB.ReadData<Expenditure>("Expenditure");
            });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            dB.RecordAdded -= DB_RecordAdded;
        }

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(!IsExpenditureAmountValid() && !string.IsNullOrEmpty(Amount.Text))
            {
                amountLabel.Content = "Ihr eingegebener Wert ist keine Zahl";
            }
            else
            {
                amountLabel.Content = "";
            }
            UpdateButtonVisibility();
        }

        private void insertIntoDB_btn_Click(object sender, RoutedEventArgs e)
        {
            var expenditure = new Expenditure
            {
                Amount = decimal.Parse(Amount.Text),
                Currency = Currency.Text,
                TransactionType = TransactionType.Text,
                TransactionDate = string.IsNullOrEmpty(TransactionDate.Text) ? null : DateTime.ParseExact(TransactionDate.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture),
                Category = Category.Text,
                PaymentMethod = PaymentMethod.Text,
                TransactionDescription = Description.Text,
                Vendor = Vendor.Text
            };

            dB.InsertIntoData<Expenditure>("Expenditure", expenditure);
            MessageBox.Show("Ein neuer Datensatz wurde hinzugefügt!");
            this.Close();
        }

        private void close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Currency_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsCurrencyValid() && !string.IsNullOrEmpty(Currency.Text))
            {
                currencyLabel.Content = "Ihre Währungseingabe ist zu lang";
            }
            else
            {
                currencyLabel.Content = "";
            }
            UpdateButtonVisibility();
        }

        private bool IsExpenditureAmountValid()
        {
            return decimal.TryParse(Amount.Text, out _);
        }
        private bool IsCurrencyValid()
        {
            return Currency.Text.Length <= 3;
        }

        private void UpdateButtonVisibility()
        {
            if (string.IsNullOrEmpty(Amount.Text) ||
                string.IsNullOrEmpty(Currency.Text) ||
                    !IsExpenditureAmountValid() ||
                    !IsCurrencyValid())
            {
                insertIntoDB_btn.Visibility = Visibility.Hidden;
            }
            else
            {
                insertIntoDB_btn.Visibility = Visibility.Visible;
            }
        }

        private void TransactionType_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
