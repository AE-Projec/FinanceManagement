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
    /// Interaktionslogik für NewRevenueWindow.xaml
    /// </summary>
    public partial class NewRevenueWindow : Window
    {
        public NewRevenueWindow()
        {
            InitializeComponent();
        }
        DB dB = new DB();
        private void insertIntoDB_btn_Click(object sender, RoutedEventArgs e)
        {
            var revenue = new Revenue
            {
                Amount = int.Parse(Amount.Text),
                Currency = Currency.Text,
                TransactionDate = string.IsNullOrEmpty(TransactionDate.Text) ? null : DateOnly.Parse(TransactionDate.Text),
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

        private void Amount_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
