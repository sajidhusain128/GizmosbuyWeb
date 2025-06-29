namespace Gizmosbuy.Core.Interfaces
{
    public interface ISalesSummaryModel
    {
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal SellingPrices { get; set; }
        public decimal Sumofprofit { get; set; }
    }
}
