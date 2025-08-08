
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IInventoryBL
    {
        Task<Object> GetRawData(IDateRange dateRange,IPager pager);
        Task<List<ISalesSummaryModel>> GetSalesSummaryData(int locationId, int month, int year);
        Task<List<IPurchaseSummaryModel>> GetPurchaseSummaryData(int locationId, int month, int year);
        Task<List<spGetRawDataResult>> GetRawDataExport(IDateRange dateRange, IPager pager);
    }
}
