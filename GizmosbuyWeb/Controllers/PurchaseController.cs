using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.Web.Filters;
using GizmosbuyWeb.Configurations;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

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
                List<ICategoryModel> categories = await _commonBL.GetAllCategories("_categoryList");

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

                List<IBrandModel> brands = await _commonBL.GetAllBrands("_brandList");

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

                List<IPaymentModeModel> paymentModes = await _commonBL.GetAllPaymentModes("_paymentList");

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

                if (i > 0)
                {
                    return Json("Success");
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
                List<ICategoryModel> categories = await _commonBL.GetAllCategories("_categoryList");

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

                List<IBrandModel> brands = await _commonBL.GetAllBrands("_brandList");

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

                List<IPaymentModeModel> paymentModes = await _commonBL.GetAllPaymentModes("_paymentList");

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

                if (i > 0)
                {
                    return Json("Success");
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
    }
}
