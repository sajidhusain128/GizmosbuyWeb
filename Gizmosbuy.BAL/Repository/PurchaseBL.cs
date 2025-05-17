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
                string sessionUserName = Utility.GetSessionValue("UserName", _httpContextAccessor.HttpContext);

                Purchase purchaseMaster = new Purchase
                {
                    SerialNo = purchaseModel.SerialNo,
                    PurchaseDate = purchaseModel.PurchaseDate,
                    CategoryId = purchaseModel.CategoryId,
                    BrandId = purchaseModel.BrandId,
                    Model = purchaseModel.Model,
                    Specifications = purchaseModel.Specifications,
                    PurchasePrice = purchaseModel.PurchasePrice,
                    Quantity = purchaseModel.Quantity,
                    UpgradePrice = purchaseModel.UpgradePrice,
                    TotalPrice = purchaseModel.TotalPrice,
                    PaymentModeId = purchaseModel.PaymentModeId,
                    BuyingLead = purchaseModel.BuyingLead,
                    CreatedBy = sessionUserName,
                    CreatedDate = DateTime.Now
                };

                await _applicationDbContext.Purchases.AddAsync(purchaseMaster);
                var i = await _applicationDbContext.SaveChangesAsync();

                return await Task.FromResult(i);
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
                var purcahseList = await _applicationDbContext.Procedures.spGetPurchaseListAsync();

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                List<spGetPurchaseListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = purcahseList.Where(Utility.GetSearchValue<spGetPurchaseListResult>(searchValue)).ToList();
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
                string sessionUserName = Utility.GetSessionValue("UserName", _httpContextAccessor.HttpContext);

                Purchase purchase = await _applicationDbContext.Purchases.FirstAsync(x => x.PurchaseId == purchaseModel.PurchaseId);

                if (purchase != null)
                {
                    purchase.SerialNo = purchaseModel.SerialNo;
                    purchase.PurchaseDate = purchaseModel.PurchaseDate;
                    purchase.CategoryId = purchaseModel.CategoryId;
                    purchase.BrandId = purchaseModel.BrandId;
                    purchase.Model = purchaseModel.Model;
                    purchase.Specifications = purchaseModel.Specifications;
                    purchase.PurchasePrice = purchaseModel.PurchasePrice;
                    purchase.UpgradePrice = purchaseModel.UpgradePrice;
                    purchase.TotalPrice = purchaseModel.TotalPrice;
                    purchase.PaymentModeId = purchaseModel.PaymentModeId;
                    purchase.BuyingLead = purchaseModel.BuyingLead;
                    purchase.ModifiedBy = sessionUserName;
                    purchase.ModifiedDate = DateTime.Now;

                    _applicationDbContext.Purchases.Entry(purchase).State = EntityState.Modified;
                }

                var i = await _applicationDbContext.SaveChangesAsync();

                return await Task.FromResult(i);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IPurchaseModel>> GetSerialNoList(string serailNo)
        {
            try
            {
                List<IPurchaseModel> purchaseModelist = null;

                var purcahseList = await _applicationDbContext.Purchases.Where(x => x.SerialNo.Contains(serailNo)).ToListAsync();

                if (purcahseList == null || purcahseList.Count == 0)
                {
                    return await Task.FromResult(new List<IPurchaseModel>());
                }

                if (purcahseList != null && purcahseList.Count > 0)
                {
                    purchaseModelist = new List<IPurchaseModel>();

                    foreach (var item in purcahseList)
                    {
                        purchaseModelist.Add(new PurchaseModel()
                        {
                            PurchaseId = item.PurchaseId,
                            SerialNo = item.SerialNo,
                        });
                    }
                }

                return await Task.FromResult(purchaseModelist);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
