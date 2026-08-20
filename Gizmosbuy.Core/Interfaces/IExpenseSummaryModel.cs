namespace Gizmosbuy.Core.Interfaces
{
    public interface IExpenseSummaryModel
    {
        public string ExpenseType { get; set; }
        public decimal SumAmount { get; set; }
        public long OrderBy { get; set; }
    }
}
