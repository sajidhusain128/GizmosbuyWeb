using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class PurchaseSummaryModel : IPurchaseSummaryModel
    {
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchaseAmount { get; set; }
        public long OrderBy { get; set; }
    }
}
