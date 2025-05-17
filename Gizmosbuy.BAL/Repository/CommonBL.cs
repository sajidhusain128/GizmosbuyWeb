using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Repository
{
    public class CommonBL : ICommonBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public CommonBL(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<List<ICategoryModel>> GetAllCategories()
        {
            try
            {
                List<CategoryMaster> _categoryMasterList = _applicationDbContext.CategoryMasters.Where(x => x.IsActive == true).ToList();

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

                return await Task.FromResult(categoryModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IBrandModel>> GetAllBrands()
        {
            try
            {
                List<BrandMaster> brandMasters = _applicationDbContext.BrandMasters.Where(x => x.IsActive == true).ToList();

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

                return await Task.FromResult(brandModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IPaymentModeModel>> GetAllPaymentModes()
        {
            try
            {
                List<PaymentModeMaster> paymentModeMasters = _applicationDbContext.PaymentModeMasters.Where(x => x.IsActive == true).ToList();

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

                return await Task.FromResult(paymentModeModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ILocationModel>> GetAllLocations()
        {
            try
            {
                List<LocationMaster> paymentModeMasters = _applicationDbContext.LocationMasters.Where(x => x.IsActive == true).ToList();

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

                return await Task.FromResult(locationModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
