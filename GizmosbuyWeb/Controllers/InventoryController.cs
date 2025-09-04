using System.Data;
using ClosedXML.Excel;
using Gizmosbuy.BAL.Commons;
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
            DateRange dateRange = new DateRange();

            return View(dateRange);
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
                    ViewBag.defaultLocationId = locationId.ToString();
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }

        }

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

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> RawDateExportExcel(string FromDate, string ToDate, string Search)
        {
            try
            {
                DateRange dateRange = new DateRange
                {
                    StartDate = FromDate,
                    EndDate = ToDate
                };

                IPager pager = new Pager();

                pager.SearchValue = Search ?? "";

                var result = await _inventoryBL.GetRawDataExport(dateRange, pager);

                if (result != null && result.Count > 0)
                {

                    DataTable dt = Utilities.CreateDataTable(result); // Fetch your data

                    if (dt.Columns.IndexOf("CreatedDate") > -1)
                    {
                        dt.Columns.Remove("CreatedDate");
                    }

                    string fileName = $"RawData_Report_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}.xlsx";

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
                    return RedirectToAction("RawData");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
