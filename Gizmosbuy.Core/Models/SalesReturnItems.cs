namespace Gizmosbuy.Core.Models
{
    public class SalesReturnItems
    {
        public int PurchaseId { get; set; }
        public int SalesId { get; set; }
        public string SerialNo { get; set; }
        public string BillNo { get; set; }
        public int ReturnQuantity { get; set; }
    }
}
