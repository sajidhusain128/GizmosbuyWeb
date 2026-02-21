using System.Collections.Immutable;
using System.Data;
using System.Data.Common;
using System.Linq;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Object> GetRawData(IDateRange dateRange, IPager pager)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                var rawDataResults = await _applicationDbContext.Procedures.spGetRawDataAsync(dateRange.StartDate.ToString(), dateRange.EndDate.ToString(), Convert.ToInt32(sessionUserId));

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

        public async Task<List<IPurchaseSummaryModel>>  GetPurchaseSummaryData(int locationId)
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
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                var rawDataResults = await _applicationDbContext.Procedures.spGetRawDataAsync(dateRange.StartDate.ToString(), dateRange.EndDate.ToString(), Convert.ToInt32(sessionUserId));
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

                return mainData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Object> GetStoreTransferRawData(int searchLocationId, IPager pager)
        {
            try
            {
                var rawDataResults = await _applicationDbContext.Procedures.spGetStoreTransferRawDataAsync(searchLocationId);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                IEnumerable<object> mainData = null;

                if (searchValue != "")
                {
                    mainData = rawDataResults.Where(Utilities.GetSearchValue<object>(searchValue));
                }
                else
                {
                    mainData = rawDataResults;
                }

                if (mainData != null && mainData.Any())
                {
                    var totalCount = mainData.ToList().Count;
                    var filterCount = totalCount;

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
                        data = new List<object>(),
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

        public async Task<Object> GetStoreTransferPaymentSummary(int searchLocationId, IPager pager)
        {
            try
            {
                var rawDataResults = await _applicationDbContext.Procedures.spGetStoreTransferPaymentSummaryAsync(searchLocationId);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                IEnumerable<object> mainData = null;

                if (searchValue != "")
                {
                    mainData = rawDataResults.Where(Utilities.GetSearchValue<object>(searchValue));
                }
                else
                {
                    mainData = rawDataResults;
                }

                if (mainData != null && mainData.Any())
                {
                    var totalCount = mainData.ToList().Count;
                    var filterCount = totalCount;

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
                        data = new List<object>(),
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

        public async Task<Object> GetStoreTransferCalculation(int searchLocationId)
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
    }
}
