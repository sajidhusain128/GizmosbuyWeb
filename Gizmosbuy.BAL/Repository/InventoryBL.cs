using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Gizmosbuy.BAL.Repository
{
    public class InventoryBL : IInventoryBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public InventoryBL(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<object> GetRawData(IDateRange dateRange, IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext)); ;

                bool isExport = false;

                var paramReturnTotalCount = new OutputParameter<int?>();

                var rawDataResults = await _applicationDbContext.Procedures.spGetRawDataAsync(dateRange.StartDate, dateRange.EndDate, sessionUserId, pager.SearchValue, pager.PageLength, pager.Offset, pager.ColumnName, pager.SortDirection, isExport, paramReturnTotalCount);

                var totalCount = paramReturnTotalCount.Value.GetValueOrDefault();
                var filterCount = totalCount;

                var data = new
                {
                    data = rawDataResults,
                    draw = pager.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filterCount
                };

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ISalesSummaryModel>> GetSalesSummaryData(ISummaryModel summaryModel)
        {
            try
            {
                List<ISalesSummaryModel> salesSummaryModelList = null;

                var salesDataList = await _applicationDbContext.Procedures.spGetSalesSummaryDataAsync(summaryModel.LocationId, summaryModel.SalesType, summaryModel.SellDate, summaryModel.SellMonth, summaryModel.SellYear);

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

                return salesSummaryModelList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IPurchaseSummaryModel>> GetPurchaseSummaryData(int locationId)
        {
            try
            {
                List<IPurchaseSummaryModel> purchaseSummaryModelList = null;
                var purchaseDataList = await _applicationDbContext.Procedures.spGetPurchaseSummaryDataAsync(locationId);

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

        public async Task<List<spGetRawDataResult>> GetRawDataExport(IDateRange dateRange, IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                bool isExport = true;

                var rawDataResults = await _applicationDbContext.Procedures.spGetRawDataAsync(dateRange.StartDate, dateRange.EndDate, sessionUserId, pager.SearchValue, null, null, pager.ColumnName, pager.SortDirection, isExport, null);

                return rawDataResults;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetStoreTransferRawData(int searchLocationId, IPager pager)
        {
            try
            {
                bool isExport = false;

                var paramReturnTotalCount = new OutputParameter<int?>();

                var rawDataResults = await _applicationDbContext.Procedures.spGetStoreTransferRawDataAsync(searchLocationId, pager.SearchValue, pager.PageLength, pager.Offset, pager.ColumnName, pager.SortDirection, isExport, paramReturnTotalCount);

                var totalCount = paramReturnTotalCount.Value.GetValueOrDefault();
                var filterCount = totalCount;

                var data = new
                {
                    data = rawDataResults,
                    draw = pager.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filterCount
                };

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetStoreTransferPaymentSummary(int searchLocationId, IPager pager)
        {
            try
            {
                bool isExport = false;

                var paramReturnTotalCount = new OutputParameter<int?>();

                var rawDataResults = await _applicationDbContext.Procedures.spGetStoreTransferPaymentSummaryAsync(searchLocationId, pager.SearchValue, pager.PageLength, pager.Offset, pager.ColumnName, pager.SortDirection, isExport, paramReturnTotalCount);

                var totalCount = paramReturnTotalCount.Value.GetValueOrDefault();
                var filterCount = totalCount;

                var data = new
                {
                    data = rawDataResults,
                    draw = pager.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filterCount
                };

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetStoreTransferCalculation(int searchLocationId)
        {
            try
            {
                // First query: Total Bill
                var totalBillQuery = await _applicationDbContext.Purchases
                    .Join(_applicationDbContext.StoreTransfers,
                        p => p.PurchaseId,
                        st => st.TransferPurchaseId,
                        (p, st) => new { p, st })
                    .GroupJoin(_applicationDbContext.StoreReturnItemNotifications,
                        pst => pst.p.PurchaseId,
                        srn => srn.TransferPurchaseId,
                        (pst, srnGroup) => new { pst.p, pst.st, srnGroup })
                    .SelectMany(
                        x => x.srnGroup.DefaultIfEmpty(),
                        (x, srn) => new { x.p, x.st, srn })
                    .Where(x => x.st.ToLocationId == searchLocationId &&
                                (x.srn.ApprovalStatusId ?? 0) != 1)
                    .GroupBy(x => new { x.p.Quantity, x.st.SellingPrice })
                    .Select(g => new
                    {
                        Label = "Total Bill",
                        TotalPrice = g.Sum(y => y.p.Quantity * y.st.SellingPrice) ?? 0
                    })
                    .GroupBy(x => x.Label)
                    .Select(g => new
                    {
                        Label = g.Key,
                        TotalPrice = g.Sum(y => y.TotalPrice)
                    }).FirstOrDefaultAsync();



                // Second query: Total Paid
                var totalPaidQuery = await _applicationDbContext.TransferPayments
                    .Where(tp => tp.IsApproved == true && tp.FromLocationId == searchLocationId)
                    .GroupBy(tp => 1) // dummy grouping to allow SUM
                    .Select(g => new
                    {
                        Label = "Total Paid",
                        TotalPrice = g.Sum(tp => tp.Amount) ?? 0
                    }).FirstOrDefaultAsync();

                // Combine both queries (Union)
                List<object> result = new List<object>();
                result.Add(totalBillQuery == null ? new { Label = "Total Bill", TotalPrice = 0 } : totalBillQuery);
                result.Add(totalPaidQuery == null ? new { Label = "Total Paid", TotalPrice = 0 } : totalPaidQuery);

                decimal totalBal = 0;
                if (totalBillQuery != null && totalPaidQuery != null)
                {
                    totalBal = totalBillQuery.TotalPrice - totalPaidQuery.TotalPrice;
                }
                else if (totalBillQuery != null && totalPaidQuery == null)
                {
                    totalBal = totalBillQuery.TotalPrice - 0;
                }
                else if (totalBillQuery == null && totalPaidQuery != null)
                {
                    totalBal = 0 - totalPaidQuery.TotalPrice;
                }

                result.Add(new { Label = "Total Balance", TotalPrice = totalBal });

                //var result = result.ToList();

                return result;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IList<spGetStoreTransferRawDataResult>> GetStoreTransferRawDataExport(int searchLocationId, IPager pager)
        {
            try
            {
                bool isExport = true;

                var rawDataResults = await _applicationDbContext.Procedures.spGetStoreTransferRawDataAsync(searchLocationId, pager.SearchValue, null, null, pager.ColumnName, pager.SortDirection, isExport, null);

                return rawDataResults;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<spGetStoreTransferPaymentSummaryResult>> GetStoreTransferPaymentSummaryExport(int searchLocationId, IPager pager)
        {
            try
            {
                string sessionUserRole = Utilities.GetSessionValue("Role", _httpContextAccessor.HttpContext) ?? "";
                int sessionLocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));

                searchLocationId = sessionUserRole == "Admin" ? sessionLocationId : searchLocationId;

                bool isExport = true;

                var rawDataResults = await _applicationDbContext.Procedures.spGetStoreTransferPaymentSummaryAsync(searchLocationId, pager.SearchValue, null, null, pager.ColumnName, pager.SortDirection, isExport, null);

                return rawDataResults;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
