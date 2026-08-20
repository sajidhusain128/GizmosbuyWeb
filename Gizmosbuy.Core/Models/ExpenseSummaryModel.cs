using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class ExpenseSummaryModel : IExpenseSummaryModel
    {
        public string ExpenseType { get; set; }
        public decimal SumAmount { get; set; }
        public long OrderBy { get; set; }
    }
}
