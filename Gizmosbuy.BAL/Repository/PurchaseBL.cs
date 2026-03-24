using System.Data;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;

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
                DataTable SearlNoDataTable = new DataTable();
                SearlNoDataTable.Columns.Add("SerialNo", typeof(string));
                if (purchaseModel.SerialNos != null && purchaseModel.SerialNos.Count > 0)
                {
                    foreach (var serialNo in purchaseModel.SerialNos)
                    {
                        SearlNoDataTable.Rows.Add(serialNo);
                    }
                }

                int purchaseLocationID = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                var parameterreturnValue = new OutputParameter<int?>();
                var i = await _applicationDbContext.Procedures.spSavePurchaseAsync(
                    purchaseModel.PurchaseId,
                    purchaseModel.SerialNo,
                    SearlNoDataTable,
                    purchaseModel.PurchaseDate,
                    purchaseModel.CategoryId,
                    purchaseModel.BrandId,
                    purchaseModel.Model,
                    purchaseModel.Specifications,
                    purchaseModel.PurchasePrice,
                    purchaseModel.Quantity,
                    purchaseModel.UpgradePrice,
                    purchaseModel.TotalPrice,
                    purchaseModel.PaymentModeName,
                    purchaseModel.BuyingLead,
                    purchaseLocationID,
                    sessionUserId,
                    DateTime.Now,
                    purchaseModel.PurchaseType,
                    parameterreturnValue,
                    null
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var purcahseList = await _applicationDbContext.Procedures.spGetPurchaseListAsync(sessionUserId);

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
                        PaymentModeName = spGetPurchaseByIDResult.PaymentMode,
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
                DataTable SearlNoDataTable = new DataTable();
                SearlNoDataTable.Columns.Add("SerialNo", typeof(string));

                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                var parameterreturnValue = new OutputParameter<int?>();
                var i = await _applicationDbContext.Procedures.spSavePurchaseAsync(
                    purchaseModel.PurchaseId,
                    purchaseModel.SerialNo,
                    SearlNoDataTable,
                    purchaseModel.PurchaseDate,
                    purchaseModel.CategoryId,
                    purchaseModel.BrandId,
                    purchaseModel.Model,
                    purchaseModel.Specifications,
                    purchaseModel.PurchasePrice,
                    purchaseModel.Quantity,
                    purchaseModel.UpgradePrice,
                    purchaseModel.TotalPrice,
                    purchaseModel.PaymentModeName,
                    purchaseModel.BuyingLead,
                    0,
                    sessionUserId,
                    DateTime.Now,
                    "Single",
                    parameterreturnValue,
                    null
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

                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var purcahseList = await _applicationDbContext.Procedures.spGetSerialNoListAsync(searchValue, sessionUserId);

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

        public async Task<int> PurchaseDelete(int id)
        {
            try
            {
                int response = 0;

                var result = _applicationDbContext.Purchases
                            .Where(p => p.PurchaseId == id)
                            .Join(_applicationDbContext.Sales, p => p.PurchaseId, s => s.PurchaseId, (p, s) => new { p, s })
                            .Join(_applicationDbContext.SalesJournals, ps => ps.s.SalesId, sj => sj.SalesId, (ps, sj) => ps.p)
                            .Any();

                if (result)
                {
                    response = -1;
                    return await Task.FromResult(response);
                }
                else
                {

                    var tempPurchase = await _applicationDbContext.Purchases.FindAsync(id);

                    if (tempPurchase != null)
                    {
                        _applicationDbContext.Purchases.Remove(tempPurchase);
                        response = await _applicationDbContext.SaveChangesAsync();
                    }

                    return await Task.FromResult(response);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
