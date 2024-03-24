using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;

namespace FinanceManagement
{
    /// <summary>
    /// Interaktionslogik für csvWindow.xaml
    /// </summary>
    public partial class csvWindow : Window
    {
        private string filePath;
        DB dB = new DB();
        public BudgetsWindow Bw {  get; set; }
        
        public csvWindow()
        {
            InitializeComponent();
            dB.RecordAdded += DB_RecordAdded;
            filePath = string.Empty;
        }

        private void DB_RecordAdded(object? sender, EventArgs e)
        {
            Dispatcher.Invoke(() => {
                dB.ReadData<BudgetLimits>("BudgetLimits");
            });
            
        }

        private void LoadCsv_Click(object sender, RoutedEventArgs e)
        {
            
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "CSV-Dateien (*.csv)|*csv|Alle Dateien (*.*)|*.*",
                Title = "CSV-Datei auswählen"
            };
            if (ofd.ShowDialog() == true)
            {
                filePath = ofd.FileName;
                ProcessCsvFile(filePath);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            dB.RecordAdded -= DB_RecordAdded;
        }

        private void Border_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files != null && files.Length > 0)
            {
                filePath = files[0];
                ProcessCsvFile(filePath);
              
            }
        }
        //anzeige vom csvfile name 
        private void ProcessCsvFile(string filePath)
        {
           
            textblock_csvName.Text = $"{filePath}";
           
        }
    

        private void Border_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void closeCsv_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void safeCsv_inDB_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrEmpty(filePath))
            {
                MessageBox.Show("Bitte wählen Sie zuerst eine CSV-Datei aus.");
                return;
            }

            //var records = ReadCsvFile(filePath);
            //dB.InsertDataIntoDB_csv(records);
            Console.WriteLine("Beginne mit dem Einfügen der CSV-Daten in die Datenbank.");
            dB.ReadCsvAndInsertInToDB(filePath);

            MessageBox.Show("Daten wurden erfolgreich in die Datenbank eingefügt.");
            Bw?.RefreshDataGrid();
            //Bw.RefreshDataGrid();
            
            Close();
        }



        public List<Dictionary<string, string>> ReadCsvFile(string filePath)
        {
            var records = new List<Dictionary<string, string>>();
            var headers = new List<string>();

            //lesen der Kopfzeile für spaltennamen
            using (var reader = new StreamReader(filePath))
            {
                var headLine = reader.ReadLine();
                if(headLine != null)
                {
                    headers.AddRange(headLine.Split(",").Select(h => h.Trim()));
                }

            //Lesen der Datenzeilen
            while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(",");
                    var record = new Dictionary<string, string>();
                    for(int i = 0; i < headers.Count; i++)
                    {
                        record[headers[i].Trim()] = values[i].Trim();
                    }
                    records.Add(record);
                }
            }
            return records;
        }
    }
}
