using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class SummaryModel : ISummaryModel
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
