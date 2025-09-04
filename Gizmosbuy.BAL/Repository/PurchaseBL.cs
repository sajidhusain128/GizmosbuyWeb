using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Gizmosbuy.BAL.Repository
{
    public class PurchaseBL : IPurchaseBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PurchaseBL(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreatePurchase(IPurchaseModel purchaseModel)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);
                var parameterreturnValue = new OutputParameter<int?>();
                var i = await _applicationDbContext.Procedures.spSavePurchaseAsync(
                    purchaseModel.PurchaseId,
                    purchaseModel.SerialNo,
                    purchaseModel.PurchaseDate,
                    purchaseModel.CategoryId,
                    purchaseModel.BrandId,
                    purchaseModel.Model,
                    purchaseModel.Specifications,
                    purchaseModel.PurchasePrice,
                    purchaseModel.Quantity,
                    purchaseModel.UpgradePrice,
                    purchaseModel.TotalPrice,
                    purchaseModel.PaymentModeId,
                    purchaseModel.BuyingLead,
                    Convert.ToInt32(sessionUserId),
                    DateTime.Now,
                    parameterreturnValue
                );

                return await Task.FromResult(parameterreturnValue.Value.GetValueOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Object> GetPurchaseList(IPager pager)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                var purcahseList = await _applicationDbContext.Procedures.spGetPurchaseListAsync(Convert.ToInt32(sessionUserId));

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                List<spGetPurchaseListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = purcahseList.Where(Utilities.GetSearchValue<spGetPurchaseListResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = purcahseList;
                }

                var totalCount = mainData.Count;
                var filterCount = mainData.Count;

                mainData = mainData
                    .Skip(start)
                    .Take(length)
                    .ToList();

                var data = new
                {
                    data = mainData,
                    draw = pager.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filterCount
                };

                return await Task.FromResult(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IPurchaseModel> GetPurchaseByID(int id)
        {
            try
            {
                var purcahseList = await _applicationDbContext.Procedures.spGetPurchaseByIDAsync(id);

                if (purcahseList == null || purcahseList.Count == 0)
                {
                    return await Task.FromResult(new PurchaseModel());
                }

                spGetPurchaseByIDResult spGetPurchaseByIDResult = purcahseList.FirstOrDefault();
                IPurchaseModel purchaseModel = null;

                if (spGetPurchaseByIDResult != null)
                {
                    purchaseModel = new PurchaseModel()
                    {
                        PurchaseId = spGetPurchaseByIDResult.PurchaseID,
                        SerialNo = spGetPurchaseByIDResult.SerialNo,
                        PurchaseDate = spGetPurchaseByIDResult.PurchaseDate.GetValueOrDefault(),
                        CategoryId = spGetPurchaseByIDResult.CategoryID.GetValueOrDefault(),
                        CategoryName = spGetPurchaseByIDResult.CategoryName,
                        BrandId = spGetPurchaseByIDResult.BrandID.GetValueOrDefault(),
                        BrandName = spGetPurchaseByIDResult.BrandName,
                        Model = spGetPurchaseByIDResult.Model,
                        Specifications = spGetPurchaseByIDResult.Specifications,
                        PurchasePrice = spGetPurchaseByIDResult.PurchasePrice.GetValueOrDefault(),
                        Quantity = spGetPurchaseByIDResult.Quantity.GetValueOrDefault(),
                        UpgradePrice = spGetPurchaseByIDResult.UpgradePrice.GetValueOrDefault(),
                        TotalPrice = spGetPurchaseByIDResult.TotalPrice.GetValueOrDefault(),
                        PaymentModeId = spGetPurchaseByIDResult.PaymentModeID.GetValueOrDefault(),
                        PaymentModeName = spGetPurchaseByIDResult.PaymentModeName,
                        BuyingLead = spGetPurchaseByIDResult.BuyingLead
                    };
                }

                return await Task.FromResult(purchaseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdatePurchase(IPurchaseModel purchaseModel)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);
                var parameterreturnValue = new OutputParameter<int?>();
                var i = await _applicationDbContext.Procedures.spSavePurchaseAsync(
                    purchaseModel.PurchaseId,
                    purchaseModel.SerialNo,
                    purchaseModel.PurchaseDate,
                    purchaseModel.CategoryId,
                    purchaseModel.BrandId,
                    purchaseModel.Model,
                    purchaseModel.Specifications,
                    purchaseModel.PurchasePrice,
                    purchaseModel.Quantity,
                    purchaseModel.UpgradePrice,
                    purchaseModel.TotalPrice,
                    purchaseModel.PaymentModeId,
                    purchaseModel.BuyingLead,
                    Convert.ToInt32(sessionUserId),
                    DateTime.Now,
                    parameterreturnValue
                );

                return await Task.FromResult(parameterreturnValue.Value.GetValueOrDefault());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IAutoCompleteModel>> GetSerialNoList(string searchValue)
        {
            try
            {
                List<IAutoCompleteModel> autoCompleteModelList = null;

                //var purcahseList = await _applicationDbContext.Purchases
                //    .Join(_applicationDbContext.CategoryMasters, P => P.CategoryId, CM => CM.CategoryId, (P, CM) => new { P, CM })
                //    .Where(x => x.P.SerialNo.Contains(searchValue)
                //            || x.P.Specifications.Contains(searchValue)
                //            || x.CM.CategoryName.Contains(searchValue))
                //    .ToListAsync();

                var purcahseList = await _applicationDbContext.Procedures.spGetSerialNoListAsync(searchValue);

                if (purcahseList == null || purcahseList.Count == 0)
                {
                    return await Task.FromResult(new List<IAutoCompleteModel>());
                }

                if (purcahseList != null && purcahseList.Count > 0)
                {
                    autoCompleteModelList = new List<IAutoCompleteModel>();

                    foreach (var item in purcahseList)
                    {
                        autoCompleteModelList.Add(new AutoCompleteModel()
                        {
                            ValueId = item.PurchaseId,
                            Description = item.Description
                        });
                    }
                }

                return await Task.FromResult(autoCompleteModelList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
