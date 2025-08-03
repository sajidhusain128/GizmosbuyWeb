using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Repository
{
    public class InventoryBL : IInventoryBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public InventoryBL(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Object> GetRawData(IDateRange dateRange, IPager pager)
        {
            try
            {
                var rawDataResults = await _applicationDbContext.Procedures.spGetRawDataAsync(dateRange.StartDate.ToString(), dateRange.EndDate.ToString());

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                List<spGetRawDataResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = rawDataResults.Where(Utilities.GetSearchValue<spGetRawDataResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = rawDataResults;
                }

                if (mainData != null && mainData.Count > 0)
                {
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

                    return data;
                }
                else
                {
                    var data = new
                    {
                        data = new List<spGetRawDataResult>(),
                        draw = pager.Draw,
                        recordsTotal = 0,
                        recordsFiltered = 0
                    };

                    return data;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ISalesSummaryModel>> GetSalesSummaryData(int locationId, int month, int year)
        {
            try
            {
                List<ISalesSummaryModel> salesSummaryModelList = null;

                var salesDataList = await _applicationDbContext.Procedures.spGetSalesSummaryDataAsync(locationId, month, year);

                if (salesDataList != null && salesDataList.Count > 0)
                {
                    salesSummaryModelList = new List<ISalesSummaryModel>();

                    foreach (var item in salesDataList)
                    {
                        salesSummaryModelList.Add(new SalesSummaryModel
                        {
                            CategoryName = item.CategoryName,
                            Quantity = item.Quantity.GetValueOrDefault(),
                            SellingPrices = item.SellingPriceRevenue.GetValueOrDefault(),
                            Sumofprofit = item.SumOfProfit.GetValueOrDefault(),
                            OrderBy = item.OrderBy.GetValueOrDefault()
                        });
                    }
                }

                return salesSummaryModelList; ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IPurchaseSummaryModel>> GetPurchaseSummaryData(int locationId, int month, int year)
        {
            try
            {
                List<IPurchaseSummaryModel>purchaseSummaryModelList = null;
                var purchaseDataList = await _applicationDbContext.Procedures.spGetPurchaseSummaryDataAsync(locationId, month, year);

                if (purchaseDataList != null && purchaseDataList.Count > 0)
                {
                    purchaseSummaryModelList = new List<IPurchaseSummaryModel>();

                    foreach (var item in purchaseDataList)
                    {
                        purchaseSummaryModelList.Add(new PurchaseSummaryModel
                        {
                            CategoryName = item.CategoryName,
                            Quantity = item.Quantity,
                            PurchaseAmount = item.PurchaseAmount,
                            OrderBy = item.OrderBy.GetValueOrDefault()
                        });
                    }
                }
                return purchaseSummaryModelList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
