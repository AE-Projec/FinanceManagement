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

        public event EventHandler DataDeleted;

        public event Action<DeleteRevenueWindow> NextRevenue;
        public event Action<DeleteRevenueWindow> PrevRevenue;

        public Revenue revenue {  get; set; }
        public DeleteRevenueWindow()
        {
            InitializeComponent();
            loadFirstRevenueEntry();
            dB.RecordRemoved += DB_RecordRemoved;
        }

        private void DB_RecordRemoved(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dB.ReadData<Revenue>("Revenue");
            });
        }

        public void ShowRevenues(Revenue revenues)
        {
            revenue = revenues;
            RevenueID.Text = $"{revenue.RevenueID}";
            TransactionType.Text = $"{revenue.TransactionType}";
            Amount.Text = $"{revenue.Amount}";
            Currency.Text = $"{revenue.Currency.Trim()}";
            TransactionDate.Text = $"{revenue.TransactionDate}";
            Category.Text = $"{revenue.Category}";
            Description.Text = $"{revenue.Description}";
            PaymentMethod.Text = $"{revenue.PaymentMethod}";
            Show();
        }

        public int LastDeletedId { get; private set; }
        private void deleteEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            int revenueId = Convert.ToInt32(RevenueID.Text);
            dB.DeleteData<Revenue>("Revenue", "RevenueID", revenueId);
            LastDeletedId = revenueId;

            DataDeleted?.Invoke(this, EventArgs.Empty);
            MessageBox.Show($"Datensatz {revenueId} erfolgreich aus der Datenbank entfernt.");

        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void previousEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            PrevRevenue.Invoke(this);
        }

        private void nextEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            
            NextRevenue.Invoke(this);
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
