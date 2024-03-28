using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement
{
    internal class Expenditure
    {
        public int? ExpenditureID { get; set; }
        public string? TransactionType { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? TransactionDate {  get; set; }
        public string? Category { get; set; }
        public string? TransactionDescription { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Vendor { get; set; }
    }
}
