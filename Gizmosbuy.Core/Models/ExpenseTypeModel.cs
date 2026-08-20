using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class ExpenseTypeModel : IExpenseTypeModel
    {
        public int ExpenseTypeId { get; set; }
        public string ExpenseTypeName { get; set; }
        public bool? IsActive { get; set; }
    }
}
