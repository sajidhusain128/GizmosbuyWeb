namespace Gizmosbuy.Core.Interfaces
{
    public interface ILocationModel
    {
        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public bool? IsActive { get; set; }
    }
}
