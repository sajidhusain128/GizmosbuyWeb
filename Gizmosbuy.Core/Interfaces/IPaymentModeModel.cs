namespace Gizmosbuy.Core.Interfaces
{
    public interface IPaymentModeModel
    {
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public bool? IsActive { get; set; }
    }
}
