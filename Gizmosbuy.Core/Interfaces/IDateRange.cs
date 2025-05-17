namespace Gizmosbuy.Core.Interfaces
{
    public interface IDateRange
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
