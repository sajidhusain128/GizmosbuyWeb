using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class DateRange : IDateRange
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
