namespace Gizmosbuy.Core.Interfaces
{
    public interface ICategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsActive { get; set; }
    }
}
