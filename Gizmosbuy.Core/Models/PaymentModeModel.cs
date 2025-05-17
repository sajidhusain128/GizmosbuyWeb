using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class PaymentModeModel : IPaymentModeModel
    {
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public bool? IsActive { get; set; }
    }
}
