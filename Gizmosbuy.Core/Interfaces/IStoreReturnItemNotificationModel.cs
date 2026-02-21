namespace Gizmosbuy.Core.Interfaces
{
    public interface IStoreReturnItemNotificationModel
    {
        public int StoreReturnItemNotificationId { get; set; }
        public string BillNo { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public int ReturnQuantity { get; set; }
        public int TransferPurchaseId { get; set; }
        public int ApprovalStatusId { get; set; }
    }
}
