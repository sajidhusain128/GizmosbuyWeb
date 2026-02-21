using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class PurchaseModel : IPurchaseModel
    {
        public int PurchaseId { get; set; }
        public string SerialNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string Specifications { get; set; }
        public decimal PurchasePrice { get; set; }
        public int Quantity { get; set; }
        public decimal UpgradePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public string BuyingLead { get; set; }
        public int PurchaseLocationID { get; set; }

        public List<string> SerialNos { get; set; }
        public string PurchaseType { get; set; }
    }
}
