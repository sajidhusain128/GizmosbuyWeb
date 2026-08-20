namespace Gizmosbuy.Core.Interfaces
{
    public interface ISummaryModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string Status { get; set; }
        public int SellYear { get; set; }
        public int SellMonth { get; set; }
        public int SellDate { get; set; }
        public string SalesType { get; set; }
    }
}
