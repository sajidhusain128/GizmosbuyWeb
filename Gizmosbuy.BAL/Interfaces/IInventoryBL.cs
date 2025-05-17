
using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IInventoryBL
    {
        Task<Object> GetRawData(IDateRange dateRange,IPager pager);
    }
}
