namespace Gizmosbuy.Core.Interfaces
{
    public interface IBrandModel
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool? IsActive { get; set; }
    }
}
