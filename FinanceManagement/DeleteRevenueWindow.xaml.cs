using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
    /// Interaktionslogik für DeleteRevenueWindow.xaml
    /// </summary>
    public partial class DeleteRevenueWindow : Window
    {
        DB dB = new DB();
        public DeleteRevenueWindow()
        {
            InitializeComponent();
            loadFirstRevenueEntry();
        }

        private void deleteEntry_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void previousEntry_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void nextEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void loadFirstRevenueEntry()
        {
            var firstRevenue = dB.ReadFirstEntry<Revenue>("Revenue");

            if (firstRevenue != null)
            {
                RevenueID.Text = firstRevenue.RevenueID.ToString();
                TransactionType.Text = firstRevenue.TransactionType ?? "";
                Amount.Text = firstRevenue.Amount.ToString() ?? "";
                Currency.Text = firstRevenue.Currency ?? "";
                TransactionDate.Text = firstRevenue.TransactionDate.HasValue ? firstRevenue.TransactionDate.Value.ToString("dd.MM.yyyy") : "";
                Category.Text = firstRevenue.Category ?? "";
                Description.Text = firstRevenue.Description ?? "";
                PaymentMethod.Text = firstRevenue.PaymentMethod ?? "";
            }
        }
    }
}
