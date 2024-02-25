using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement
{
    public static class CsvToDbMapping
    {
        public static readonly Dictionary<string, string> ColumnMappings = new Dictionary<string, string>
        {

            {"Betrag","Budget_Amount" },
            {"Währung","Currency" },
            {"Limit des Jahres","Budget_Limit_Year"},
            {"Kategorie","Budget_Category" },
            {"Erstellt am","Creation_Date" },
            {"Status","Budget_Status" },
            {"Genehmigt von","Approved_By" },
            {"Kommentar","Comment" }            
        };
    }
}
