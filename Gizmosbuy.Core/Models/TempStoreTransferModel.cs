using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class TempStoreTransferModel : PurchaseModel, ITempStoreTransferModel
    {
        public int TempStoreTransferID { get; set; }
        public DateTime? TransferDate { get; set; }
        public decimal? SellingPrice { get; set; }
        public int? SellingQuantity { get; set; }
        public string PaymentMode { get; set; }
        public int FromLocationId { get; set; }
        public string FromLocationName { get; set; }
        public int ToLocationId { get; set; }
        public string ToLocationName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? UserId { get; set; }
        public string BillNo { get; set; }
    }
}
