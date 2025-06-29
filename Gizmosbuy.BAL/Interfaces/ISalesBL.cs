using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ISalesBL
    {
        Task<int> CreateSales();
        Task<object> GetSalesList(IPager pager);
        Task<ISalesModel> GetSalesByID(int id);
        Task<int> UpdateSales(ISalesModel salesModel);
        Task<object> GetTempSalesList(IPager pager);
        Task<string> GenerateNewBillNo();
        Task<int> CreateTempSales(TempSalesModel tempSalesModel);
        Task<ITempSalesModel> GetTempSalesByID(int id);
        Task<int> TempSalesDelete(int id);
        Task<int> UpdateTempSales(TempSalesModel tempSalesModel);
    }
}
