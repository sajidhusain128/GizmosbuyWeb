using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ISalesBL
    {
        Task<int> CreateSales(SalesModel salesModel);
        Task<Object> GetSalesList(IPager pager);
        Task<ISalesModel> GetSalesByID(int id);
        Task<int> UpdateSales(ISalesModel salesModel);
    }
}
