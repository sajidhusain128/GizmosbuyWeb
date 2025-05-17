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

        public async Task<int> CreateSales(SalesModel salesModel)
        {
            try
            {
                string sessionUserName = Utility.GetSessionValue("UserName", _httpContextAccessor.HttpContext);

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
                    LocationId = salesModel.LocationId,
                    BillNo = salesModel.BillNo,
                    CreatedBy = sessionUserName,
                    CreatedDate = DateTime.Now
                };

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                var i = await _applicationDbContext.Procedures.spSaveSalesAsync(0,
                    saleMaster.PurchaseId,
                    saleMaster.SellingDate,
                    saleMaster.SellingPrice,
                    saleMaster.SellingQuantity,
                    saleMaster.PaymentModeId,
                    saleMaster.SellingLead,
                    saleMaster.CustomerName,
                    saleMaster.ContactNo,
                    saleMaster.LocationId,
                    saleMaster.BillNo,
                    saleMaster.CreatedBy,
                    saleMaster.CreatedDate,
                    null, // ModifiedBy
                    null, // ModifiedDate
                    "INSERT",
                    outputParameter);

                //_applicationDbContext.Entry(saleMaster).State = EntityState.Added;

                //var i = await _applicationDbContext.SaveChangesAsync();

                return await Task.FromResult(outputParameter.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Object> GetSalesList(IPager pager)
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
                    mainData = salesList.Where(Utility.GetSearchValue<spGetSalesListResult>(searchValue)).ToList();
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
                        LocationId = spGetSalesByIDResult.LocationID.GetValueOrDefault(),
                        LocationName = spGetSalesByIDResult.LocationName,
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
                string sessionUserName = Utility.GetSessionValue("UserName", _httpContextAccessor.HttpContext);

                Sale saleMaster = new Sale
                {
                    SalesId = salesModel.SalesId,
                    SellingDate = salesModel.SellingDate,
                    SellingPrice = salesModel.SellingPrice,
                    PaymentModeId = salesModel.PaymentModeId,
                    SellingLead = salesModel.SellingLead,
                    CustomerName = salesModel.CustomerName,
                    ContactNo = salesModel.ContactNo,
                    LocationId = salesModel.LocationId,
                    BillNo = salesModel.BillNo,
                    ModifiedBy = sessionUserName,
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
                    saleMaster.LocationId,
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
    }
}
