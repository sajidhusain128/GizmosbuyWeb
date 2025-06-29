using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.BAL.Repository;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Gizmosbuy.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryBL _inventoryBL;
        private readonly ICommonBL _commonBL;
        public InventoryController(IInventoryBL inventoryBL, ICommonBL commonBL)
        {
            _inventoryBL = inventoryBL;
            _commonBL = commonBL;
        }

        public IActionResult Index()
        {
            return View();
        }

        [CustomAuthorize]
        public IActionResult RawData()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public async Task<IActionResult> GetRawData(DateRange dateRange)
        {
            try
            {
                IPager pager = new Pager();

                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");

                var result = await _inventoryBL.GetRawData(dateRange, pager);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [CustomAuthorize]
        public async Task<IActionResult> Summary()
        {
            try
            {
                string RoleName = HttpContext.Session.GetString("Role");

                List<ILocationModel> locationModel = await _commonBL.GetAllLocations();

                if (locationModel != null && locationModel.Count > 0)
                {
                    //locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Location" });
                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    //locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                if (RoleName == "Admin")
                {
                    string location = HttpContext.Session.GetString("Location");
                    ViewBag.defaultLocationValue = location;

                    int locationId = Convert.ToInt32(HttpContext.Session.GetString("LocationId"));
                    ViewBag.defaultLocationId= locationId.ToString();
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        //[HttpPost]
        //[CustomAuthorize]
        //public async Task<IActionResult> Summery()
        //{
        //    try
        //    {
        //        //var result = await _inventoryBL.GetRawData(new DateRange(), new Pager());
        //        return Json("result");
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public IActionResult PurchaseEntry()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize]
        public async Task<IActionResult> GetSummayData(string transactionType, int locationId, int month, int year)
        {
            try
            {
                

                if (transactionType == "Sales")
                {
                    var result = await _inventoryBL.GetSalesSummaryData(locationId, month, year);

                    if (result == null)
                    {
                        result = new List<ISalesSummaryModel>();
                    }

                    return PartialView("_SalesSummaryPartial", result);
                }
                else if (transactionType == "Purchase")
                {
                    var result = await _inventoryBL.GetPurchaseSummaryData(locationId, month, year);

                    if (result == null)
                    {
                        result = new List<IPurchaseSummaryModel>();
                    }

                    return PartialView("_PurchaseSummaryPartial", result);
                }

                return PartialView(null, null);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
