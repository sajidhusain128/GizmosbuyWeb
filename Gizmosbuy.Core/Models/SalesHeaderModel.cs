namespace Gizmosbuy.Core.Models
{
    public class SalesHeaderModel
    {
        public string CustomerName { get; set; }
        public long? ContactNo { get; set; }
        public string Location { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? SellingDate { get; set; }
        public string PaymentModeName { get; set; }
        public string SellingLead { get; set; }
        public string TotalPriceInWord  { get; set; }
    }
}
