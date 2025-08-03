namespace Gizmosbuy.Core.Models
{
    public class SalesDataModel
    {
        public long? RowNum { get; set; }
        public int SalesID { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string Specifications { get; set; }
        public int? SellingQuantity { get; set; }
        public decimal? SellingPrice { get; set; }
    }
}
