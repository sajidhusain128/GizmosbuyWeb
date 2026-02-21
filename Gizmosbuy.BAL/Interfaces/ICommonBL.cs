using Gizmosbuy.Core.Interfaces;
using Twilio.Rest.Api.V2010.Account;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ICommonBL
    {
        Task<List<ICategoryModel>> GetAllCategories(string cacheKey);
        Task<List<IBrandModel>> GetAllBrands(string cacheKey);
        Task<List<IPaymentModeModel>> GetAllPaymentModes(string cacheKey);
        Task<List<ILocationModel>> GetAllLocations(string cacheKey);
        Task<MessageResource> SendWhatsAppService(IWebConfiguration webConfiguration, IWhatsAppSettings whatsAppSettings, Tuple<string, MemoryStream> webFile, long? contactNo, string CustomerName);
    }
}
