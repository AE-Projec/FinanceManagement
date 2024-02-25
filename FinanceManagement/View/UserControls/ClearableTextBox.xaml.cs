using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceManagement.View.UserControls
{
    /// <summary>
    /// Interaktionslogik für ClearableTextBox.xaml
    /// </summary>
    public partial class ClearableTextBox : UserControl, INotifyPropertyChanged
    {
        public ClearableTextBox()
        {
            DataContext = this;
            InitializeComponent();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtInput.Clear();
            txtInput.Focus();

        }
        private string placeholder;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Placeholder
        {
            get 
            {
                return placeholder; 
            }
            set 
            { 
                placeholder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Placeholder"));
               // tbPlaceholder.Text = placeholder;
            }
        }
        

        public string Text
        {
            get 
            { 
                return txtInput.Text;
            }
            set 
            { 
                txtInput.Text = value;
            }
        }

        
        public event TextChangedEventHandler TextChanged
        {
            add
            {
                txtInput.TextChanged += value;
            }
            remove
            {
                txtInput.TextChanged -= value;
            }
        }
        private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtInput.Text))
            {
                tbPlaceholder.Visibility = Visibility.Visible;
                
            }
            else
            {
                tbPlaceholder.Visibility = Visibility.Hidden;
                
            }
        }
    }
}
