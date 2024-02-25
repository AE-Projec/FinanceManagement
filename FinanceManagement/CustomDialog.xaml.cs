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
    /// Interaktionslogik für CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : Window
    {
        public CustomDialog()
        {
            InitializeComponent();
        }

        private void cancle_btn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        public BudgetsWindow Bw { get; set; }
        private void viaCSV_btn_Click(object sender, RoutedEventArgs e)
        {
            var csvWindow = new csvWindow();
            csvWindow.Owner = this;
            csvWindow.Bw = this.Bw;
            csvWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            csvWindow.ShowDialog();
            /*
            csvWindow csvWindow = new csvWindow
            {
                Owner = this,
                
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };*/



        }

        

        private void manual_btn_Click(object sender, RoutedEventArgs e)
        {
            //Visueller effekt um das alte fenster abzudunkeln
            var blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 5;
            this.Effect = blurEffect;

            //Erstellen des neuen Fensters
            var newBudgetWindow = new NewBudgetWindow()
            {
                Owner = this,
               
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };


            //anzeigen des dialog fensters
            newBudgetWindow.ShowDialog();
            Close();

            //wenn das fenster geschloßen wird, effekt entfernen
            this.Effect = null;
            Bw.RefreshDataGrid();
          //  dB.ReadData(bw.budgetsDataGrid);//budgetsDataGrid

        }
    }
}
