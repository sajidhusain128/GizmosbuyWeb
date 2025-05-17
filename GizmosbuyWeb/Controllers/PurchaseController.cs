using Gizmosbuy.BAL.Repository;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Gizmosbuy.Web.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly IPurchaseBL _purchaseBL;
        private readonly ICommonBL _commonBL;
        public PurchaseController(IPurchaseBL purchaseBL, ICommonBL commonBL)
        {
            _purchaseBL = purchaseBL;
            _commonBL = commonBL;
        }

        [CustomAuthorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public async Task<IActionResult> GetPurchaseList()
        {
            IPager pager = new Pager();

            try
            {
                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");

                var purcahseList = await _purchaseBL.GetPurchaseList(pager);

                return Json(purcahseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> Create()
        {
            try
            {
                List<ICategoryModel> categories = await _commonBL.GetAllCategories();

                if (categories != null && categories.Count > 0)
                {
                    categories.Insert(0, new CategoryModel { CategoryId = 0, CategoryName = "Select Category" });
                    ViewBag.Categories = categories;
                }
                else
                {
                    categories = new List<ICategoryModel> { new CategoryModel { CategoryId = 0, CategoryName = "Select Category" } };
                    ViewBag.Categories = categories;
                }

                List<IBrandModel> brands = await _commonBL.GetAllBrands();

                if (brands != null && brands.Count > 0)
                {
                    brands.Insert(0, new BrandModel { BrandId = 0, BrandName = "Select Brand" });
                    ViewBag.Brands = brands;
                }
                else
                {
                    brands = new List<IBrandModel> { new BrandModel { BrandId = 0, BrandName = "Select Brand" } };
                    ViewBag.Brands = brands;
                }

                List<IPaymentModeModel> paymentModes = await _commonBL.GetAllPaymentModes();

                if (paymentModes != null && paymentModes.Count > 0)
                {
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
        [CustomAuthorize]
        public ActionResult SavePurchase(PurchaseModel purchaseModel)
        {
            try
            {
                int i = _purchaseBL.CreatePurchase(purchaseModel).Result;

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

        [CustomAuthorize]
        public async Task<IActionResult> Edit(int Id)
        {
            try
            {
                List<ICategoryModel> categories = await _commonBL.GetAllCategories();

                if (categories != null && categories.Count > 0)
                {
                    categories.Insert(0, new CategoryModel { CategoryId = 0, CategoryName = "Select Category" });
                    ViewBag.Categories = categories;
                }
                else
                {
                    categories = new List<ICategoryModel> { new CategoryModel { CategoryId = 0, CategoryName = "Select Category" } };
                    ViewBag.Categories = categories;
                }

                List<IBrandModel> brands = await _commonBL.GetAllBrands();

                if (brands != null && brands.Count > 0)
                {
                    brands.Insert(0, new BrandModel { BrandId = 0, BrandName = "Select Brand" });
                    ViewBag.Brands = brands;
                }
                else
                {
                    brands = new List<IBrandModel> { new BrandModel { BrandId = 0, BrandName = "Select Brand" } };
                    ViewBag.Brands = brands;
                }

                List<IPaymentModeModel> paymentModes = await _commonBL.GetAllPaymentModes();

                if (paymentModes != null && paymentModes.Count > 0)
                {
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
        [CustomAuthorize]
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
        [CustomAuthorize]
        public async Task<IActionResult> GetAutoCompleteSerialNo(string serailNo)
        {
            try
            {
                var result = await _purchaseBL.GetSerialNoList(serailNo);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize]
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
    }
}
