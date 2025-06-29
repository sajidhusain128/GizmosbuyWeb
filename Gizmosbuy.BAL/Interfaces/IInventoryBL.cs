
using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IInventoryBL
    {
        Task<Object> GetRawData(IDateRange dateRange,IPager pager);
        Task<List<ISalesSummaryModel>> GetSalesSummaryData(int locationId, int month, int year);
        Task<List<IPurchaseSummaryModel>> GetPurchaseSummaryData(int locationId, int month, int year);
    }
}
