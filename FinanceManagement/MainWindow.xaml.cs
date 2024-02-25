﻿using FinanceManagement.View.UserControls;
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
    }


}