using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class BrandModel : IBrandModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool? IsActive { get; set; }
    }
}
