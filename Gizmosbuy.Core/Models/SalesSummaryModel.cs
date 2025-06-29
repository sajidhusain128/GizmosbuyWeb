using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class SalesSummaryModel : ISalesSummaryModel
    {
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrices { get; set; }
        public decimal Sumofprofit { get; set; }
        public long OrderBy { get; set; }
    }
}
