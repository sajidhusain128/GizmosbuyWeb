using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class CategoryModel : ICategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsActive { get; set; }
    }
}
