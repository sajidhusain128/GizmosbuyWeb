using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IInventoryBL
    {
        Task<object> GetRawData(IDateRange dateRange, IPager pager);
        Task<List<ISalesSummaryModel>> GetSalesSummaryData(ISummaryModel summaryModel);
        Task<List<IPurchaseSummaryModel>> GetPurchaseSummaryData(int locationId);
        Task<List<spGetRawDataResult>> GetRawDataExport(IDateRange dateRange, IPager pager);
        Task<object> GetStoreTransferRawData(int searchLocationId, IPager pager);
        Task<object> GetStoreTransferPaymentSummary(int searchLocationId, IPager pager);
        Task<object> GetStoreTransferCalculation(int searchLocationId);
        Task<IList<spGetStoreTransferRawDataResult>> GetStoreTransferRawDataExport(int searchLocationId, IPager pager);
        Task<IList<spGetStoreTransferPaymentSummaryResult>> GetStoreTransferPaymentSummaryExport(int searchLocationId, IPager pager);
    }
}
