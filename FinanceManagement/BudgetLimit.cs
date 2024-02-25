using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement
{
    public class BudgetLimit
    {
        public int? BudgetID { get; set; }
        public int? Budget_Amount { get; set; }
        public int? Budget_Limit_Year { get; set; }
        public string? Budget_Category { get; set; }
        public DateOnly? Creation_Date { get; set; }
        public string? Budget_Status { get; set; }
        public string? Approved_By { get; set; }
        public string? Comment { get; set; }
        public string? Currency { get; set; }
    }
}
