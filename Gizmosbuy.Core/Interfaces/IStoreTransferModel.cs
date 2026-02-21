namespace Gizmosbuy.Core.Interfaces
{
    public interface IStoreTransferModel : IPurchaseModel
    {
        public int SalesId { get; set; }
        public DateTime? TransferDate { get; set; }
        public decimal? SellingPrice { get; set; }
        public int? SellingQuantity { get; set; }
        public string PaymentMode { get; set; }
        public int FromLocationId { get; set; }
        public string FromLocationName { get; set; }
        public int ToLocationId { get; set; }
        public string ToLocationName { get; set; }
        public string BillNo { get; set; }
        public bool IsReturned { get; set; }
        public int TransferPurchaseID { get; set; }
    }
}
