using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Gizmosbuy.BAL.Repository
{
    public class CommonBL : ICommonBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ICacheService _cacheService;
        public CommonBL(ApplicationDbContext applicationDbContext, ICacheService cacheService)
        {
            _applicationDbContext = applicationDbContext;
            _cacheService = cacheService;
        }

        public async Task<List<ICategoryModel>> GetAllCategories(string cacheKey)
        {
            try
            {
                var value = await _cacheService.GetOrSetAsync(cacheKey, async entry =>
                {
                    List<CategoryMaster> _categoryMasterList = await _applicationDbContext.CategoryMasters.Where(x => x.IsActive == true).ToListAsync();

                    List<ICategoryModel> categoryModelList = new List<ICategoryModel>();
                    foreach (var category in _categoryMasterList)
                    {
                        categoryModelList.Add(new CategoryModel
                        {
                            CategoryId = category.CategoryId,
                            CategoryName = category.CategoryName,
                            IsActive = category.IsActive
                        });
                    }

                    return categoryModelList;
                });

                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IBrandModel>> GetAllBrands(string cacheKey)
        {
            try
            {
                var value = await _cacheService.GetOrSetAsync(cacheKey, async entry =>
                {
                    List<BrandMaster> brandMasters = await _applicationDbContext.BrandMasters.Where(x => x.IsActive == true).ToListAsync();

                    List<IBrandModel> brandModelList = new List<IBrandModel>();
                    foreach (var brand in brandMasters)
                    {
                        brandModelList.Add(new BrandModel
                        {
                            BrandId = brand.BrandId,
                            BrandName = brand.BrandName,
                            IsActive = brand.IsActive
                        });
                    }

                    return brandModelList;
                });

                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IPaymentModeModel>> GetAllPaymentModes(string cacheKey)
        {
            try
            {
                var value = await _cacheService.GetOrSetAsync(cacheKey, async entry =>
                {
                    List<PaymentModeMaster> paymentModeMasters = await _applicationDbContext.PaymentModeMasters.Where(x => x.IsActive == true).ToListAsync();

                    List<IPaymentModeModel> paymentModeModelList = new List<IPaymentModeModel>();
                    foreach (var paymentMode in paymentModeMasters)
                    {
                        paymentModeModelList.Add(new PaymentModeModel
                        {
                            PaymentModeId = paymentMode.PaymentModeId,
                            PaymentModeName = paymentMode.PaymentModeName,
                            IsActive = paymentMode.IsActive
                        });
                    }

                    return paymentModeModelList;
                });

                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ILocationModel>> GetAllLocations(string cacheKey)
        {
            try
            {
                var value = await _cacheService.GetOrSetAsync(cacheKey, async entry =>
                {
                    List<LocationMaster> paymentModeMasters = await _applicationDbContext.LocationMasters.Where(x => x.IsActive == true).ToListAsync();

                    List<ILocationModel> locationModelList = new List<ILocationModel>();
                    foreach (var paymentMode in paymentModeMasters)
                    {
                        locationModelList.Add(new LocationModel
                        {
                            LocationId = paymentMode.LocationId,
                            LocationName = paymentMode.LocationName,
                            IsActive = paymentMode.IsActive
                        });
                    }

                    return locationModelList;
                });

                return value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> SendWhatsAppService(IWebConfiguration webConfiguration, IWhatsAppSettings whatsAppSettings, Tuple<string, MemoryStream> webFile, string contactNo, string CustomerName)
        {
            var filePath = "";
            MessageResource message = null;

            try
            {
                string bodyMessage = string.Empty;
                string CustomerNumber = "";

                if (!string.IsNullOrWhiteSpace(contactNo))
                {
                    CustomerNumber = "+91" + contactNo.Trim();

                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TempFiles");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var fileName = webFile.Item1;
                    filePath = Path.Combine(uploadPath, fileName);

                    // Reset position before writing (important!)
                    var memoryStreamTemp = webFile.Item2;
                    // Use MemoryStream to hold data in memory
                    using (var memoryStream = new MemoryStream(memoryStreamTemp.ToArray()))
                    {
                        // Save the file to the physical path using a FileStream
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await memoryStream.CopyToAsync(fileStream);
                        }
                    }
                    var url = Path.Combine(webConfiguration.Issuer, "TempFiles/", fileName);

                    if (!string.IsNullOrEmpty(whatsAppSettings.ContentSid))
                    {
                        var contentVariables = new Dictionary<string, string>
                        {
                            { "1", Utilities.GetLast10Characters(whatsAppSettings.SenderNumber) },
                            { "2", url }
                        };

                        message = await MessageResource.CreateAsync(
                           from: new PhoneNumber($"whatsapp:{whatsAppSettings.SenderNumber}"), // Twilio Sandbox number
                           to: new PhoneNumber($"whatsapp:{CustomerNumber}"),  // Your verified WhatsApp number
                           contentSid: whatsAppSettings.ContentSid,
                           contentVariables: Newtonsoft.Json.JsonConvert.SerializeObject(contentVariables)
                       );
                    }
                    else
                    {
                        bodyMessage = whatsAppSettings.MessageContent;

                        message = await MessageResource.CreateAsync(
                            from: new PhoneNumber($"whatsapp:{whatsAppSettings.SenderNumber}"), // Twilio Sandbox number
                            to: new PhoneNumber($"whatsapp:{CustomerNumber}"),  // Your verified WhatsApp number
                            body: bodyMessage,
                            mediaUrl: new List<Uri> {
                                new Uri(url) // Publicly accessible PDF URL
                            }
                        );
                    }

                    await Task.Delay(2000);

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    return message.Body;
                }
                else
                {
                    return "Invalid contact number.";
                }
            }
            catch (Exception)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                throw;
            }
        }
    }
}
