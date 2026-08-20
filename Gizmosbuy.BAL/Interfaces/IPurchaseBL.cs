using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IPurchaseBL
    {
        Task<int> CreatePurchase(IPurchaseModel purchaseModel);
        Task<object> GetPurchaseList(IPager pager);
        Task<IPurchaseModel> GetPurchaseByID(int id);
        Task<int> UpdatePurchase(IPurchaseModel purchaseModel);
        Task<List<IAutoCompleteModel>> GetSerialNoList(string serailNo);
        Task<int> PurchaseDelete(int id);
        Task<IList<spGetPurchaseListResult>> GetPurchaseExport(IPager pager);
    }
}
