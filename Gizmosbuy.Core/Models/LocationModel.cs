using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class LocationModel : ILocationModel
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public bool? IsActive { get; set; }
    }
}
