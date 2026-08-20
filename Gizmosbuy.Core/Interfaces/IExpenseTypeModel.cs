namespace Gizmosbuy.Core.Interfaces
{
    public interface IExpenseTypeModel
    {
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public bool? IsActive { get; set; }
    }
}
