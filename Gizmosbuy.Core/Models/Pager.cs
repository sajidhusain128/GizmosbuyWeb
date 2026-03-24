using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class Pager : IPager
    {
        public int PageStart { get; set; }
        public int PageLength { get; set; }
        public string SearchValue { get; set; }
        public int Draw { get; set; }
        public string SortDirection { get; set; }
        public string SortColumnIndex { get; set; }
        public string ColumnName { get; set; }
    }
}
