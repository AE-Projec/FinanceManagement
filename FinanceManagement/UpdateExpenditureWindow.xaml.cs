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
    /// Interaktionslogik für UpdateExpenditureWindow.xaml
    /// </summary>
    public partial class UpdateExpenditureWindow : Window
    {
        DB dB = new DB();

        public event EventHandler DataUpdated;

        public event Action<UpdateExpenditureWindow> NextExpenditure;
        public event Action<UpdateExpenditureWindow> PrevExpenditure;

        public Expenditure expenditure {  get; set; }

        public int LastUpdatedId { get; private set; }
        public UpdateExpenditureWindow()
        {
            InitializeComponent();
        }

        public void ShowExpenditures(Expenditure expenditures)
        {
            expenditure = expenditures;
            ExpenditureID.Text = $"{expenditure.ExpenditureID}";
            TransactionType.Text = $"{expenditure.TransactionType}";
            Amount.Text = $"{expenditure.Amount}";
            Currency.Text = $"{expenditure.Currency.Trim()}";
            TransactionDate.Text = $"{expenditure.TransactionDate}";
            Category.Text = $"{expenditure.Category}";
            Description.Text = $"{expenditure.TransactionDescription}";
            PaymentMethod.Text = $"{expenditure.PaymentMethod}";
            Vendor.Text = $"{expenditure.Vendor}";
            Show();
        }

        private void updateInDB_btn_Click(object sender, RoutedEventArgs e)
        {
            if(expenditure == null)
            {
                expenditure = new Expenditure();
            }
            // Übertragen der UI-Änderungen in das Expenditure-Objekt
            if(int.TryParse(ExpenditureID.Text, out int expenditureId))
            {
                expenditure.ExpenditureID = expenditureId;
            }
            if(decimal.TryParse(Amount.Text, out decimal expenditureAmount))
            {
                expenditure.Amount = expenditureAmount;
            }
            expenditure.Currency = Currency.Text;
            expenditure.Category = Category.Text;
            if (DateTime.TryParse(TransactionDate.Text.ToString(), out DateTime transactionDate))
            {
                expenditure.TransactionDate = transactionDate.Date;
            }
            expenditure.TransactionDescription = Description.Text;
            expenditure.PaymentMethod = PaymentMethod.Text;
            expenditure.TransactionType = TransactionType.Text;
            expenditure.Vendor = Vendor.Text;

            dB.UpdateData<Expenditure>("Expenditure",expenditure);
            LastUpdatedId = (int)expenditure.ExpenditureID;
            DataUpdated?.Invoke(this,EventArgs.Empty);

            MessageBox.Show("Datensatz/ Datensätze Aktualisiert");

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
    }
}
