using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class SalesModel : PurchaseModel, ISalesModel
    {
        public int SalesId { get; set; }

        public DateTime? SellingDate { get; set; }

        public decimal? SellingPrice { get; set; }
        public int? SellingQuantity { get; set; }

        public new int? PaymentModeId { get; set; }

        public string SellingLead { get; set; }

        public string CustomerName { get; set; }

        public int? ContactNo { get; set; }

        public int? LocationId { get; set; }

        public string LocationName { get; set; }

        public string BillNo { get; set; }
    }
}
