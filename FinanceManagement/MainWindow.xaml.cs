using FinanceManagement.View.UserControls;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Budgets_Click(object sender, RoutedEventArgs e)
        {

            //Erstellen des neuen Fensters
            var budgetsWindow = new BudgetsWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            //anzeigen des dialoges
            budgetsWindow.ShowDialog();

        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {

            Application.Current.Shutdown();


        }

        private void financial_planning_Click(object sender, RoutedEventArgs e)
        {
            financial_planning_popup.IsOpen = !financial_planning_popup.IsOpen;
        }

        private void financial_planing_expenditure_Click(object sender, RoutedEventArgs e)
        {
            ExpenditureWindow expenditureWindow = new ExpenditureWindow();
            expenditureWindow.Owner = this;
            expenditureWindow.ShowDialog();
            
        }

        private void financial_planing_revenue_Click(object sender, RoutedEventArgs e)
        {
           RevenueWindow revenueWindow = new RevenueWindow();
            revenueWindow.Owner = this;
            revenueWindow.ShowDialog();
        }
        private string currentDisplayedPage;
        private void financial_overview_pieChart_Click(object sender, RoutedEventArgs e)
        {
            if (currentDisplayedPage != nameof(PieChartPage))
            {
                ContentFrame.Navigate(new PieChartPage());
                currentDisplayedPage = nameof(PieChartPage);
                financial_overview_popup.IsOpen = false;
            }

        }

        private void financial_overview_cartasianChart_Click(object sender, RoutedEventArgs e)
        {
            if(currentDisplayedPage != nameof(CartasianPage))
            {
                ContentFrame.Navigate(new CartasianPage());
                currentDisplayedPage = nameof(CartasianPage);
                financial_overview_popup.IsOpen = false;
            }
           
        }

        private void financial_overview_Click(object sender, RoutedEventArgs e)
        {
            financial_overview_popup.IsOpen = !financial_overview_popup.IsOpen;
        }
    }


}