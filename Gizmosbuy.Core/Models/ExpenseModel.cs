using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class ExpenseModel : IExpenseModel
    {
        public int ExpenseId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal Amount { get; set; }
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public string Remark { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public string ExpenseMonthName { get; set; }
        public short ExpenseMonth { get; set; }
        public short ExpenseYear { get; set; }
    }
}