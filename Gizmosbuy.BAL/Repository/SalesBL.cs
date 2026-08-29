using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var salesModels = await _applicationDbContext.TempSales.Where(x => x.UserId == sessionUserId).ToListAsync();

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                if (salesModels != null && salesModels.Count > 0)
                {
                    if (invoiceNo == null)
                    {
                        invoiceNo = salesModels.FirstOrDefault().BillNo;
                    }

                    foreach (var salesModel in salesModels)
                    {
                        DateTime currentDateTime = salesModel.SellingDate.GetValueOrDefault().Date + DateTime.Now.TimeOfDay;

                        var i = await _applicationDbContext.Procedures.spSaveSalesAsync(0,
                            salesModel.PurchaseId,
                            currentDateTime,
                            salesModel.SellingPrice,
                            salesModel.SellingQuantity,
                            salesModel.PaymentMode,
                            salesModel.SellingLead,
                            salesModel.CustomerName,
                            salesModel.ContactNo,
                            salesModel.Location,
                            salesModel.BillNo,
                            salesModel.Warranty,
                            salesModel.Remark,
                            sessionUserId,
                            DateTime.Now,
                            null, // ModifiedBy
                            null, // ModifiedDate
                            "INSERT",
                            outputParameter);
                    }

                    var tempSaleslist = await _applicationDbContext.TempSales.Where(x => x.UserId == sessionUserId).ToListAsync();

                    _applicationDbContext.TempSales.RemoveRange(tempSaleslist);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return new Tuple<int, string>(response, invoiceNo);
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                bool isExport = false;

                var paramReturnTotalCount = new OutputParameter<int?>();

                var salesList = await _applicationDbContext.Procedures.spGetSalesListAsync(sessionUserId, pager.SearchValue, pager.PageLength, pager.Offset, pager.ColumnName, pager.SortDirection, isExport, paramReturnTotalCount);

                var totalCount = paramReturnTotalCount.Value.GetValueOrDefault();
                var filterCount = totalCount;

                var data = new
                {
                    data = salesList,
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
                        PaymentModeName = spGetSalesByIDResult.PaymentMode,
                        CustomerName = spGetSalesByIDResult.CustomerName,
                        ContactNo = spGetSalesByIDResult.ContactNo.GetValueOrDefault(),
                        LocationName = spGetSalesByIDResult.Location,
                        BillNo = spGetSalesByIDResult.BillNo,
                        Warranty = spGetSalesByIDResult.Warranty,
                        Remark = spGetSalesByIDResult.Remark,
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                DateTime currentDateTime = salesModel.SellingDate.GetValueOrDefault().Date + DateTime.Now.TimeOfDay;

                var i = await _applicationDbContext.Procedures.spSaveSalesAsync(salesModel.SalesId,
                    salesModel.PurchaseId,
                    currentDateTime,
                    salesModel.SellingPrice,
                    salesModel.SellingQuantity,
                    salesModel.PaymentModeName,
                    salesModel.SellingLead,
                    salesModel.CustomerName,
                    salesModel.ContactNo,
                    salesModel.LocationName,
                    salesModel.BillNo,
                    salesModel.Warranty,
                    salesModel.Remark,
                    null, // CreatedBy
                    null, // CreatedDate
                    sessionUserId,
                    DateTime.Now,
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var salesList = await _applicationDbContext.Procedures.spGetTempSalesListAsync(sessionUserId);

                string searchValue = pager.SearchValue.Trim() ?? "";

                List<spGetTempSalesListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = salesList.Where(Utilities.GetSearchValue<spGetTempSalesListResult>(searchValue, Constant.GlobalDateFormat)).ToList();
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

                var prefix = _applicationDbContext.LocationMasters.FirstOrDefault(x => x.LocationName == sessionLocation).LocationCode;

                prefix = string.IsNullOrWhiteSpace(prefix) ? "JGS" : prefix;

                string billNo = string.Empty;

                var lastBillNo = await _applicationDbContext.Procedures.spGetLastSalesBillNoAsync(prefix, "Sales");

                if (lastBillNo != null && lastBillNo.Count > 0)
                {
                    billNo = Utilities.GenerateBillNo(prefix, lastBillNo.FirstOrDefault().BillNo);
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                tempSalesModel.CreatedBy = sessionUserId;
                tempSalesModel.CreatedDate = DateTime.Now;

                var i = await _applicationDbContext.Procedures.spSaveTempSalesAsync(0,
                        tempSalesModel.PurchaseId,
                        tempSalesModel.SellingDate,
                        tempSalesModel.SellingPrice,
                        tempSalesModel.Quantity,
                        tempSalesModel.SellingQuantity,
                        tempSalesModel.PaymentModeName,
                        tempSalesModel.SellingLead,
                        tempSalesModel.CustomerName,
                        tempSalesModel.ContactNo,
                        tempSalesModel.Location,
                        tempSalesModel.BillNo,
                        tempSalesModel.Warranty,
                        tempSalesModel.Remark,
                        tempSalesModel.CreatedBy,
                        tempSalesModel.CreatedDate,
                        null,
                        null,
                        "INSERT",
                        outputParameter);

                return await Task.FromResult(outputParameter.Value);
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
                        PaymentModeName = spGetSalesByIDResult.PaymentMode,
                        CustomerName = spGetSalesByIDResult.CustomerName,
                        ContactNo = spGetSalesByIDResult.ContactNo,
                        Location = spGetSalesByIDResult.Location,
                        BillNo = spGetSalesByIDResult.BillNo,
                        Warranty = spGetSalesByIDResult.Warranty,
                        Remark = spGetSalesByIDResult.Remark,
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                tempSalesModel.ModifiedBy = sessionUserId;
                tempSalesModel.ModifiedDate = DateTime.Now;

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                var i = await _applicationDbContext.Procedures.spSaveTempSalesAsync(tempSalesModel.TempSalesId,
                        tempSalesModel.PurchaseId,
                        tempSalesModel.SellingDate,
                        tempSalesModel.SellingPrice,
                        tempSalesModel.Quantity,
                        tempSalesModel.SellingQuantity,
                        tempSalesModel.PaymentModeName,
                        tempSalesModel.SellingLead,
                        tempSalesModel.CustomerName,
                        tempSalesModel.ContactNo,
                        tempSalesModel.Location,
                        tempSalesModel.BillNo,
                        tempSalesModel.Warranty,
                        tempSalesModel.Remark,
                        null,
                        null,
                        tempSalesModel.ModifiedBy,
                        tempSalesModel.ModifiedDate,
                        "UPDATE",
                        outputParameter);

                return await Task.FromResult(outputParameter.Value);
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
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                string TotalPriceInWord = null;
                List<SalesDataModel> salesDatas = null;
                var response = await _applicationDbContext.Procedures.spGetSalesReportDataAsync(invoiceNo);

                if (response != null && response.Count > 0)
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
                var response2 = await _applicationDbContext.Procedures.spGetSalesReportHeaderAsync(invoiceNo, sessionUserId);

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
                            TotalPriceInWord = TotalPriceInWord,
                            StoreAddress = item.StoreAddress,
                            StoreContactNo = item.StoreContactNo,
                            Warranty = item.Warranty
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

        public async Task<List<ISalesModel>> GetInvoiceDetails(string invoiceNo)
        {
            try
            {
                List<ISalesModel> salesModelList = null;
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var response = await _applicationDbContext.Procedures.spGetInvoiceDetailsAsync(invoiceNo, sessionUserId);

                if (response.Any())
                {
                    salesModelList = new List<ISalesModel>();

                    foreach (var item in response)
                    {
                        salesModelList.Add(new SalesModel
                        {
                            SalesId = item.SalesID,
                            BillNo = item.BillNo,
                            SerialNo = item.SerialNo,
                            Model = item.Model,
                            SellingPrice = item.SellingPrice,
                            CustomerName = item.CustomerName,
                            ContactNo = item.ContactNo,
                            SellingQuantity = item.SellingQuantity,
                            PurchaseId = item.PurchaseID
                        });
                    }
                }

                return await Task.FromResult(salesModelList);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteSalesByInvoice(string invoiceNo, List<SalesReturnItems> salesReturnItems)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                IEnumerable<DeleteSalesType> salesDeleteList = salesReturnItems != null && salesReturnItems.Any()
                    ? salesReturnItems.Select(s => new DeleteSalesType
                    {
                        BillNo = s.BillNo,
                        SalesID = s.SalesId,
                        PurchaseID = s.PurchaseId,
                        SellingQuantity = s.ReturnQuantity
                    })
                    : new List<DeleteSalesType>();

                OutputParameter<int> outputParameter = new OutputParameter<int>();
                var i = await _applicationDbContext.Procedures.spDeleteSalesByInvoiceAsync(invoiceNo, salesDeleteList, sessionUserId, outputParameter);

                return await Task.FromResult(outputParameter.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<spGetSalesListResult>> GetSalesExport(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                string searchValue = pager.SearchValue.Trim();

                bool isExport = true;

                var salesList = await _applicationDbContext.Procedures.spGetSalesListAsync(sessionUserId, searchValue, null, null, pager.ColumnName, pager.SortDirection, isExport, null);

                return salesList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
