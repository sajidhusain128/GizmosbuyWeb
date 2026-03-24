using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ICommonBL
    {
        Task<List<ICategoryModel>> GetAllCategories(string cacheKey);
        Task<List<IBrandModel>> GetAllBrands(string cacheKey);
        Task<List<IPaymentModeModel>> GetAllPaymentModes(string cacheKey);
        Task<List<ILocationModel>> GetAllLocations(string cacheKey);
        Task<string> SendWhatsAppService(IWebConfiguration webConfiguration, IWhatsAppSettings whatsAppSettings, Tuple<string, MemoryStream> webFile, string contactNo, string CustomerName);
    }
}
