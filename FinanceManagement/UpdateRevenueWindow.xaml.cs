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
using System.Xml.Linq;

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für UpdateRevenueWindow.xaml
    /// </summary>
    public partial class UpdateRevenueWindow : Window
    {

        DB db = new DB();

        public event EventHandler DataUpdated;

        public event Action<UpdateRevenueWindow> NextRevenue;
        public event Action<UpdateRevenueWindow> PrevRevenue;


        public Revenue revenue  { get; set; }


        public void ShowRevenues(Revenue revenues)
        {
            revenue = revenues;



            RevenueID.Text = $"{revenue.RevenueID}";
            TransactionType.Text = $"{revenue.TransactionType}";
            Amount.Text = $"{revenue.Amount}";
            Currency.Text = $"{revenue.Currency}";
            TransactionDate.Text = $"{revenue.TransactionDate}";
            Category.Text = $"{revenue.Category}";
            Description.Text = $"{revenue.Description}";
            PaymentMethod.Text = $"{revenue.PaymentMethod}";
            Show();
        }




        public UpdateRevenueWindow()
        {
            InitializeComponent();
        }

        private void updateInDB_btn_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
