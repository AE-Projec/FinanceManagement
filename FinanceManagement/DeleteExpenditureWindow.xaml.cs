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
    /// Interaktionslogik für DeleteExpenditureWindow.xaml
    /// </summary>
    public partial class DeleteExpenditureWindow : Window
    {
        DB dB = new DB();

        public event EventHandler DataDeleted;

        public event Action<DeleteExpenditureWindow> NextExpenditure;
        public event Action<DeleteExpenditureWindow> PrevExpenditure;

        public Expenditure expenditure { get; set; }
        public DeleteExpenditureWindow()
        {
            InitializeComponent();
            loadFirstExpenditureEntry();
            dB.RecordRemoved += DB_RecordRemoved;
        }

        private void DB_RecordRemoved(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dB.ReadData<Expenditure>("Expenditure");
            });
        }

        public void ShowExpenditures(Expenditure expenditures)
        {
            expenditure = expenditures;
            ExpenditureID.Text = $"{expenditure.ExpenditureID}";
            TransactionType.Text = $"{expenditure.TransactionType}";
            TransactionDate.Text = $"{expenditure.TransactionDate}";
            Amount.Text = $"{expenditure.Amount}";
            Currency.Text = $"{expenditure.Currency.Trim()}";
            Category.Text = $"{expenditure.Category}";
            Description.Text = $"{expenditure.TransactionDescription}";
            PaymentMethod.Text = $"{expenditure.PaymentMethod}";
            Vendor.Text = $"{expenditure.Vendor}";
            Show();
        }
        public int LastDeletedId { get; private set; }

        private void deleteEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            int expenditureID = Convert.ToInt32(ExpenditureID.Text);
            dB.DeleteData<Expenditure>("Expenditure", "ExpenditureID", expenditureID);
            LastDeletedId = expenditureID;

            DataDeleted?.Invoke(this, EventArgs.Empty);
            MessageBox.Show($"Datensatz {expenditureID} erfolgreich aus der Datenbank entfernt.");
        }

        private void cancel_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void previousEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            PrevExpenditure.Invoke(this);
        }

        private void nextEntry_btn_Click(object sender, RoutedEventArgs e)
        {
            NextExpenditure.Invoke(this);
        }

        private void loadFirstExpenditureEntry()
        {
            var firstExpenditure = dB.ReadFirstEntry<Expenditure>("Expenditure");
            if(firstExpenditure != null)
            {
                ExpenditureID.Text = firstExpenditure.ExpenditureID.ToString();
                TransactionType.Text = firstExpenditure.TransactionType ?? "";
                TransactionDate.Text = firstExpenditure.TransactionDate.HasValue ? firstExpenditure.TransactionDate.Value.ToString("dd.MM.yyyy") : "";
                Amount.Text = firstExpenditure.Amount.ToString() ?? "";
                Currency.Text = firstExpenditure.Currency ?? "";
                Category.Text = firstExpenditure.Category ?? "";
                Description.Text = firstExpenditure.TransactionDescription ?? "";
                PaymentMethod.Text = firstExpenditure.PaymentMethod ?? "";
                Vendor.Text = firstExpenditure.Vendor ?? "";
            }
        }
    }
}
