namespace Gizmosbuy.Core.Interfaces
{
    public interface IPager
    {
        public int PageStart { get; set; }
        public int PageLength { get; set; }
        public string? SearchValue { get; set; }
        public int Draw { get; set; }
    }
}
