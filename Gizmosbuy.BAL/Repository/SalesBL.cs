using Azure;
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
    public class SalesBL : ISalesBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SalesBL(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Tuple<int, string>> CreateSales()
        {
            try
            {
                int response = 0;
                string invoiceNo = null;
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                var salesModels = await _applicationDbContext.TempSales.Where(x => x.UserId == Convert.ToInt32(sessionUserId)).ToListAsync();

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                if (salesModels != null && salesModels.Count > 0)
                {
                    if (invoiceNo == null)
                    {
                        invoiceNo = salesModels.FirstOrDefault().BillNo;
                    }

                    foreach (var salesModel in salesModels)
                    {
                        Sale saleMaster = new Sale
                        {
                            PurchaseId = salesModel.PurchaseId,
                            SellingDate = salesModel.SellingDate,
                            SellingPrice = salesModel.SellingPrice,
                            SellingQuantity = salesModel.SellingQuantity,
                            PaymentModeId = salesModel.PaymentModeId,
                            SellingLead = salesModel.SellingLead,
                            CustomerName = salesModel.CustomerName,
                            ContactNo = salesModel.ContactNo,
                            Location = salesModel.Location,
                            BillNo = salesModel.BillNo,
                            CreatedBy = Convert.ToInt32(sessionUserId),
                            CreatedDate = DateTime.Now
                        };

                        var i = await _applicationDbContext.Procedures.spSaveSalesAsync(0,
                            saleMaster.PurchaseId,
                            saleMaster.SellingDate,
                            saleMaster.SellingPrice,
                            saleMaster.SellingQuantity,
                            saleMaster.PaymentModeId,
                            saleMaster.SellingLead,
                            saleMaster.CustomerName,
                            saleMaster.ContactNo,
                            saleMaster.Location,
                            saleMaster.BillNo,
                            saleMaster.CreatedBy,
                            saleMaster.CreatedDate,
                            null,
                            null,
                            "INSERT",
                            outputParameter);
                    }

                    var tempSaleslist = await _applicationDbContext.TempSales.Where(x => x.UserId == Convert.ToInt32(sessionUserId)).ToListAsync();

                    _applicationDbContext.TempSales.RemoveRange(tempSaleslist);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return new Tuple<int, string>(response,invoiceNo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetSalesList(IPager pager)
        {
            try
            {
                var salesList = await _applicationDbContext.Procedures.spGetSalesListAsync();

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                List<spGetSalesListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = salesList.Where(Utilities.GetSearchValue<spGetSalesListResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = salesList;
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

        public async Task<ISalesModel> GetSalesByID(int id)
        {
            try
            {
                var salesList = await _applicationDbContext.Procedures.spGetSalesByIDAsync(id);

                if (salesList == null || salesList.Count == 0)
                {
                    return await Task.FromResult(new SalesModel());
                }

                spGetSalesByIDResult spGetSalesByIDResult = salesList.FirstOrDefault();
                ISalesModel salesModel = null;

                if (spGetSalesByIDResult != null)
                {
                    salesModel = new SalesModel()
                    {
                        SalesId = spGetSalesByIDResult.SalesID,
                        SerialNo = spGetSalesByIDResult.SerialNo,
                        CategoryId = spGetSalesByIDResult.CategoryID.GetValueOrDefault(),
                        CategoryName = spGetSalesByIDResult.CategoryName,
                        BrandId = spGetSalesByIDResult.BrandID.GetValueOrDefault(),
                        BrandName = spGetSalesByIDResult.BrandName,
                        Model = spGetSalesByIDResult.Model,
                        Specifications = spGetSalesByIDResult.Specifications,
                        Quantity = spGetSalesByIDResult.Quantity.GetValueOrDefault(),
                        SellingQuantity = spGetSalesByIDResult.SellingQuantity.GetValueOrDefault(),
                        SellingDate = spGetSalesByIDResult.SellingDate.GetValueOrDefault(),
                        SellingPrice = spGetSalesByIDResult.SellingPrice.GetValueOrDefault(),
                        PaymentModeId = spGetSalesByIDResult.PaymentModeID.GetValueOrDefault(),
                        PaymentModeName = spGetSalesByIDResult.PaymentModeName,
                        CustomerName = spGetSalesByIDResult.CustomerName,
                        ContactNo = spGetSalesByIDResult.ContactNo.GetValueOrDefault(),
                        LocationName = spGetSalesByIDResult.Location,
                        BillNo = spGetSalesByIDResult.BillNo,
                        SellingLead = spGetSalesByIDResult.SellingLead

                    };
                }

                return await Task.FromResult(salesModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateSales(ISalesModel salesModel)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                Sale saleMaster = new Sale
                {
                    SalesId = salesModel.SalesId,
                    SellingDate = salesModel.SellingDate,
                    SellingPrice = salesModel.SellingPrice,
                    PaymentModeId = salesModel.PaymentModeId,
                    SellingLead = salesModel.SellingLead,
                    CustomerName = salesModel.CustomerName,
                    ContactNo = salesModel.ContactNo,
                    Location = salesModel.LocationName,
                    BillNo = salesModel.BillNo,
                    ModifiedBy = Convert.ToInt32(sessionUserId),
                    ModifiedDate = DateTime.Now
                };

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                var i = await _applicationDbContext.Procedures.spSaveSalesAsync(saleMaster.SalesId,
                    saleMaster.PurchaseId,
                    saleMaster.SellingDate,
                    saleMaster.SellingPrice,
                    saleMaster.SellingQuantity,
                    saleMaster.PaymentModeId,
                    saleMaster.SellingLead,
                    saleMaster.CustomerName,
                    saleMaster.ContactNo,
                    saleMaster.Location,
                    saleMaster.BillNo,
                    null, // CreatedBy
                    null, // CreatedDate
                    saleMaster.ModifiedBy,
                    saleMaster.ModifiedDate,
                    "UPDATE",
                    outputParameter);

                return await Task.FromResult(outputParameter.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetTempSalesList(IPager pager)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                var salesList = await _applicationDbContext.Procedures.spGetTempSalesListAsync(Convert.ToInt32(sessionUserId));

                string searchValue = pager.SearchValue ?? "";

                List<spGetTempSalesListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = salesList.Where(Utilities.GetSearchValue<spGetTempSalesListResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = salesList;
                }

                var totalCount = mainData.Count;
                var filterCount = mainData.Count;

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

        public async Task<string> GenerateNewBillNo()
        {
            try
            {
                string sessionLocation = Utilities.GetSessionValue("Location", _httpContextAccessor.HttpContext);
                string prefix = Utilities.GetPrefixByLocation(sessionLocation);

                string billNo = string.Empty;

                var sales = await _applicationDbContext.Sales.Where(x => x.BillNo.StartsWith(prefix)).OrderByDescending(o => o.SalesId).FirstOrDefaultAsync();

                if (sales != null)
                {
                    billNo = Utilities.GenerateBillNo(prefix, sales.BillNo);
                }
                else
                {
                    billNo = Utilities.GenerateBillNo(prefix, "");
                }

                return await Task.FromResult(billNo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateTempSales(TempSalesModel tempSalesModel)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                TempSale saleTemp = new TempSale
                {
                    PurchaseId = tempSalesModel.PurchaseId,
                    SellingDate = tempSalesModel.SellingDate,
                    SellingPrice = tempSalesModel.SellingPrice,
                    SellingQuantity = tempSalesModel.SellingQuantity,
                    PaymentModeId = tempSalesModel.PaymentModeId,
                    SellingLead = tempSalesModel.SellingLead,
                    CustomerName = tempSalesModel.CustomerName,
                    ContactNo = tempSalesModel.ContactNo,
                    Location = tempSalesModel.Location,
                    BillNo = tempSalesModel.BillNo,
                    CreatedBy = Convert.ToInt32(sessionUserId),
                    CreatedDate = DateTime.Now,
                    UserId = Convert.ToInt32(sessionUserId)
                };

                await _applicationDbContext.TempSales.AddAsync(saleTemp);
                var i = await _applicationDbContext.SaveChangesAsync();

                return await Task.FromResult(i);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ITempSalesModel> GetTempSalesByID(int id)
        {
            try
            {
                var tempSalesList = await _applicationDbContext.Procedures.spGetTempSalesByIDAsync(id);

                if (tempSalesList == null || tempSalesList.Count == 0)
                {
                    return await Task.FromResult(new TempSalesModel());
                }

                var spGetSalesByIDResult = tempSalesList.FirstOrDefault();
                ITempSalesModel tempSalesModel = null;

                if (spGetSalesByIDResult != null)
                {
                    tempSalesModel = new TempSalesModel()
                    {
                        TempSalesId = spGetSalesByIDResult.TempSalesID,
                        SerialNo = spGetSalesByIDResult.SerialNo,
                        CategoryId = spGetSalesByIDResult.CategoryID.GetValueOrDefault(),
                        CategoryName = spGetSalesByIDResult.CategoryName,
                        BrandId = spGetSalesByIDResult.BrandID.GetValueOrDefault(),
                        BrandName = spGetSalesByIDResult.BrandName,
                        Model = spGetSalesByIDResult.Model,
                        Specifications = spGetSalesByIDResult.Specifications,
                        SellingQuantity = spGetSalesByIDResult.SellingQuantity,
                        SellingDate = spGetSalesByIDResult.SellingDate,
                        SellingPrice = spGetSalesByIDResult.SellingPrice,
                        PaymentModeId = spGetSalesByIDResult.PaymentModeID,
                        PaymentModeName = spGetSalesByIDResult.PaymentModeName,
                        CustomerName = spGetSalesByIDResult.CustomerName,
                        ContactNo = spGetSalesByIDResult.ContactNo,
                        Location = spGetSalesByIDResult.Location,
                        BillNo = spGetSalesByIDResult.BillNo,
                        SellingLead = spGetSalesByIDResult.SellingLead,
                        PurchaseId = spGetSalesByIDResult.PurchaseID
                    };
                }

                return await Task.FromResult(tempSalesModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> TempSalesDelete(int id)
        {
            try
            {
                int response = 0;

                var tempSales = await _applicationDbContext.TempSales.FindAsync(id);

                if (tempSales != null)
                {
                    _applicationDbContext.TempSales.Remove(tempSales);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return await Task.FromResult(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateTempSales(TempSalesModel tempSalesModel)
        {
            try
            {
                string sessionUserId = Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext);

                int response = 0;

                var tempSales = await _applicationDbContext.TempSales.FindAsync(tempSalesModel.TempSalesId);

                if (tempSales != null)
                {
                    tempSales.PurchaseId = tempSalesModel.PurchaseId;
                    tempSales.SellingDate = tempSalesModel.SellingDate;
                    tempSales.SellingPrice = tempSalesModel.SellingPrice;
                    tempSales.SellingQuantity = tempSalesModel.SellingQuantity;
                    tempSales.PaymentModeId = tempSalesModel.PaymentModeId;
                    tempSales.SellingLead = tempSalesModel.SellingLead;
                    tempSales.CustomerName = tempSalesModel.CustomerName;
                    tempSales.ContactNo = tempSalesModel.ContactNo;
                    tempSales.Location = tempSalesModel.Location;
                    tempSales.BillNo = tempSalesModel.BillNo;
                    tempSales.ModifiedBy = Convert.ToInt32(sessionUserId);
                    tempSales.ModifiedDate = DateTime.Now;

                    _applicationDbContext.TempSales.Update(tempSales);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Tuple<List<SalesDataModel>, List<SalesHeaderModel>>> GetSalesReportData(string invoiceNo)
        {
            try
            {
                string TotalPriceInWord = null;
                List<SalesDataModel> salesDatas = null;
                var response = await _applicationDbContext.Procedures.spGetSalesReportDataAsync(invoiceNo);

                if(response != null && response.Count > 0)
                {
                    salesDatas = new List<SalesDataModel>();
                    foreach (var item in response)
                    {
                        salesDatas.Add(new SalesDataModel
                        {
                            RowNum = item.RowNum,
                            SalesID = item.SalesID,
                            CategoryName = item.CategoryName,
                            BrandName = item.BrandName,
                            Model = item.Model,
                            SerialNo = item.SerialNo,
                            Specifications = item.Specifications,
                            SellingQuantity = item.SellingQuantity,
                            SellingPrice = item.SellingPrice
                        });
                    }

                    Decimal total = salesDatas.Sum(s => s.SellingPrice.Value);
                    TotalPriceInWord = Utilities.ConvertToIndianCurrencyWords(total);
                }

                List<SalesHeaderModel> salesHeader = null;
                var response2 = await _applicationDbContext.Procedures.spGetSalesReportHeaderAsync(invoiceNo);

                if (response2 != null && response2.Count > 0)
                {
                    salesHeader = new List<SalesHeaderModel>();
                    foreach (var item in response2)
                    {
                        salesHeader.Add(new SalesHeaderModel
                        {
                            CustomerName = item.CustomerName,
                            ContactNo = item.ContactNo,
                            Location = item.Location,
                            InvoiceNo = item.InvoiceNo,
                            SellingDate = item.SellingDate,
                            SellingLead = item.SellingLead,
                            PaymentModeName = item.PaymentModeName,
                            TotalPriceInWord = TotalPriceInWord
                        });
                    }
                }

                return new Tuple<List<SalesDataModel>, List<SalesHeaderModel>>(salesDatas, salesHeader);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
