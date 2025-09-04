using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class TempSalesModel : PurchaseModel, ITempSalesModel
    {
        public int TempSalesId { get; set; }

        public new int PurchaseId { get; set; }

        public DateTime? SellingDate { get; set; }

        public decimal? SellingPrice { get; set; }

        public int? SellingQuantity { get; set; }

        public new int? PaymentModeId { get; set; }

        public string SellingLead { get; set; }

        public string CustomerName { get; set; }

        public long? ContactNo { get; set; }

        public string Location { get; set; }

        public string BillNo { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? UserId { get; set; }
    }
}
