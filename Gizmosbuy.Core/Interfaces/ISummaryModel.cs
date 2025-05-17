namespace Gizmosbuy.Core.Interfaces
{
    public class ISummaryModel
    {
        public int? LocationId { get; set; }
        public string LocationName { get; set; }
        public string Status { get; set; }
        public int SellYear { get; set; }
        public int SellMonth { get; set; }
    }
}
