using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ICommonBL
    {
        Task<List<ICategoryModel>> GetAllCategories();
        Task<List<IBrandModel>> GetAllBrands();
        Task<List<IPaymentModeModel>> GetAllPaymentModes();
        Task<List<ILocationModel>> GetAllLocations();
    }
}
