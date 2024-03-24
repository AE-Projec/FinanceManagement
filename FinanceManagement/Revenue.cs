using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManagement
{
    public class Revenue
    {
        public int? RevenueID { get; set; }
        public string? TransactionType { get; set; }
        public decimal? Amount { get; set; }
        public string? Currency {  get; set; }
        public DateOnly? TransactionDate { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
