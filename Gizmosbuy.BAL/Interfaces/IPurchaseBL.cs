using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IPurchaseBL
    { 
        Task<int> CreatePurchase(IPurchaseModel purchaseModel);
        Task<Object> GetPurchaseList(IPager pager);
        Task<IPurchaseModel> GetPurchaseByID(int id);
        Task<int> UpdatePurchase(IPurchaseModel purchaseModel);
        Task<List<IPurchaseModel>> GetSerialNoList(string serailNo);
    }
}
