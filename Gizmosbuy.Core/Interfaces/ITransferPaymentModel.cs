namespace Gizmosbuy.Core.Interfaces
{
    public interface ITransferPaymentModel
    {
        public int TransferPaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string TransferMode { get; set; }
        public int FromLocationId { get; set; }
        public int ToLocationId { get; set; }
        public string Remark { get; set; }
        public bool IsApproved { get; set; }
    }
}
