namespace Gizmosbuy.Core.Interfaces
{
    public interface IPurchaseSummaryModel
    {
        public string CategoryName { get; set; }
        public int Quantity { get; set; }
        public decimal PurchaseAmount { get; set; }
    }
}
