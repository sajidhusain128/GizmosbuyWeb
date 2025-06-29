using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class AutoCompleteModel : IAutoCompleteModel
    {
        public int ValueId { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
