using Gizmosbuy.BAL.Interfaces;

namespace Gizmosbuy.BAL.Repository
{
    public class Pager : IPager
    {
        public int PageStart { get; set; }
        public int PageLength { get; set; }
        public string? SearchValue { get; set; }
        public int Draw { get; set; }
    }
}
