using ClosedXML.Excel;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.BAL.Repository;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Models;
using Gizmosbuy.Web.Filters;
using GizmosbuyWeb.Configurations;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Gizmosbuy.Web.Controllers
{
    [NoCache]
    [EnableCors(Constant.MyPolicy)]
    public class PurchaseController : Controller
    {
        private readonly IPurchaseBL _purchaseBL;
        private readonly ICommonBL _commonBL;
        public PurchaseController(IPurchaseBL purchaseBL, ICommonBL commonBL)
        {
            _purchaseBL = purchaseBL;
            _commonBL = commonBL;
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetPurchaseList()
        {
            IPager pager = new Pager();

            try
            {
                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");
                pager.SortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                pager.SortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                pager.ColumnName = Utility.CapitalizeFirstChar(Request.Form[$"columns[{pager.SortColumnIndex}][data]"].FirstOrDefault());

                var purcahseList = await _purchaseBL.GetPurchaseList(pager);

                return Json(purcahseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> Create()
        {
            try
            {
                List<ICategoryModel> cachedCategoryList = await _commonBL.GetAllCategories("_categoryList");
                List<ICategoryModel> categories = [.. cachedCategoryList.Any() ? cachedCategoryList : null];

                if (categories != null && categories.Count > 0)
                {
                    if (!categories.Any(c => c.CategoryId == 0))
                        categories.Insert(0, new CategoryModel { CategoryId = 0, CategoryName = "Select Category" });
                    ViewBag.Categories = categories;
                }
                else
                {
                    categories = new List<ICategoryModel> { new CategoryModel { CategoryId = 0, CategoryName = "Select Category" } };
                    ViewBag.Categories = categories;
                }

                List<IBrandModel> cachedBrandList = await _commonBL.GetAllBrands("_brandList");
                List<IBrandModel> brands = [.. cachedBrandList.Any() ? cachedBrandList : null];

                if (brands != null && brands.Count > 0)
                {
                    if (!brands.Any(b => b.BrandId == 0))
                        brands.Insert(0, new BrandModel { BrandId = 0, BrandName = "Select Brand" });
                    ViewBag.Brands = brands;
                }
                else
                {
                    brands = new List<IBrandModel> { new BrandModel { BrandId = 0, BrandName = "Select Brand" } };
                    ViewBag.Brands = brands;
                }

                List<IPaymentModeModel> cachedPaymentModeList = await _commonBL.GetAllPaymentModes("_paymentList");
                List<IPaymentModeModel> paymentModes = [.. cachedPaymentModeList.Any() ? cachedPaymentModeList : null];

                if (paymentModes != null && paymentModes.Count > 0)
                {
                    if (!paymentModes.Any(p => p.PaymentModeId == 0))
                        paymentModes.Insert(0, new PaymentModeModel { PaymentModeId = 0, PaymentModeName = "Select Payment Mode" });
                    ViewBag.PaymentModes = paymentModes;
                }
                else
                {
                    paymentModes = new List<IPaymentModeModel> { new PaymentModeModel { PaymentModeId = 0, PaymentModeName = "Select Payment Mode" } };
                    ViewBag.PaymentModes = paymentModes;
                }

            }
            catch (Exception)
            {

                throw;
            }

            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult Create(IPurchaseModel purchaseModel)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> SavePurchase(PurchaseModel purchaseModel)
        {
            try
            {
                int i = await _purchaseBL.CreatePurchase(purchaseModel);

                if (i == 1)
                {
                    return Json("Success");
                }
                else if (i == -1)
                {
                    return Json("Exist");
                }

                return Json("Failed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> Edit([FromQuery] int Id)
        {
            try
            {
                List<ICategoryModel> cachedCategoryList = await _commonBL.GetAllCategories("_categoryList");
                List<ICategoryModel> categories = [.. cachedCategoryList.Any() ? cachedCategoryList : null];

                if (categories != null && categories.Count > 0)
                {
                    if (!categories.Any(c => c.CategoryId == 0))
                        categories.Insert(0, new CategoryModel { CategoryId = 0, CategoryName = "Select Category" });
                    ViewBag.Categories = categories;
                }
                else
                {
                    categories = new List<ICategoryModel> { new CategoryModel { CategoryId = 0, CategoryName = "Select Category" } };
                    ViewBag.Categories = categories;
                }

                List<IBrandModel> cachedBrandList = await _commonBL.GetAllBrands("_brandList");
                List<IBrandModel> brands = [.. cachedBrandList.Any() ? cachedBrandList : null];

                if (brands != null && brands.Count > 0)
                {
                    if (!brands.Any(b => b.BrandId == 0))
                        brands.Insert(0, new BrandModel { BrandId = 0, BrandName = "Select Brand" });
                    ViewBag.Brands = brands;
                }
                else
                {
                    brands = new List<IBrandModel> { new BrandModel { BrandId = 0, BrandName = "Select Brand" } };
                    ViewBag.Brands = brands;
                }

                List<IPaymentModeModel> cachedPaymentModeList = await _commonBL.GetAllPaymentModes("_paymentList");
                List<IPaymentModeModel> paymentModes = [.. cachedPaymentModeList.Any() ? cachedPaymentModeList : null];

                if (paymentModes != null && paymentModes.Count > 0)
                {
                    if (!paymentModes.Any(p => p.PaymentModeId == 0))
                        paymentModes.Insert(0, new PaymentModeModel { PaymentModeId = 0, PaymentModeName = "Select Payment Mode" });
                    ViewBag.PaymentModes = paymentModes;
                }
                else
                {
                    paymentModes = new List<IPaymentModeModel> { new PaymentModeModel { PaymentModeId = 0, PaymentModeName = "Select Payment Mode" } };
                    ViewBag.PaymentModes = paymentModes;
                }

                IPurchaseModel purchaseModel = await _purchaseBL.GetPurchaseByID(Id);

                ViewBag.TransferPurchaseID = Convert.ToInt32(Request.Query["TransferPurchaseID"]);

                if (purchaseModel != null)
                {
                    return View(purchaseModel);
                }
            }
            catch (Exception)
            {
                throw;
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> UpdatePurchase(PurchaseModel purchaseModel)
        {
            try
            {
                int i = await _purchaseBL.UpdatePurchase(purchaseModel);

                if (i == 1)
                {
                    return Json("Success");
                }
                else if (i == -1)
                {
                    return Json("Exist");
                }

                return Json("Failed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
        public async Task<IActionResult> GetAutoCompleteSerialNo(string searchValue)
        {
            try
            {
                var result = await _purchaseBL.GetSerialNoList(searchValue);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
        public async Task<IActionResult> GetPurchaseById(int purchaseId)
        {
            try
            {
                IPurchaseModel purchaseModel = await _purchaseBL.GetPurchaseByID(purchaseId);

                return Json(purchaseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost()]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> PurchaseDelete(int Id)
        {
            try
            {
                var response = await _purchaseBL.PurchaseDelete(Id);

                if (response > 0)
                {
                    return Json("Success");
                }
                else if (response == -1)
                {
                    return Json("Exist");
                }

                return Json("Failed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> PurchaseExportExcel(string Search)
        {
            try
            {
                IPager pager = new Pager();

                pager.SearchValue = Search ?? "";

                var result = await _purchaseBL.GetPurchaseExport(pager);

                if (result != null && result.Count > 0)
                {

                    DataTable dt = Utilities.CreateDataTable(result); // Fetch your data

                    if (dt.Columns.Count > 0)
                    {
                        if(dt.Columns.Contains("TransferPurchaseID"))
                        {
                            dt.Columns.Remove("TransferPurchaseID");
                        }
                        if(dt.Columns.Contains("CanEdit"))
                        {
                            dt.Columns.Remove("CanEdit");
                        }
                        if(dt.Columns.Contains("CanDelete"))
                        {
                            dt.Columns.Remove("CanDelete");
                        }
                    }

                    string fileName = $"Purchase_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dt, "Sheet1");
                        using (MemoryStream stream = new MemoryStream())
                        {
                            wb.SaveAs(stream);
                            return File(stream.ToArray(),
                                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                        fileName);
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
