using System.Data;
using ClosedXML.Excel;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.Web.Filters;
using GizmosbuyWeb.Configurations;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gizmosbuy.Web.Controllers
{
    [NoCache]
    [EnableCors(Constant.MyPolicy)]
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

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public IActionResult RawData()
        {
            DateRange dateRange = new DateRange();

            return View(dateRange);
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetRawData(DateRange dateRange)
        {
            try
            {
                IPager pager = new Pager();

                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");
                pager.SortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                pager.SortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                pager.ColumnName = Utility.CapitalizeFirstChar(Request.Form[$"columns[{pager.SortColumnIndex}][data]"].FirstOrDefault());

                var result = await _inventoryBL.GetRawData(dateRange, pager);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> Summary()
        {
            try
            {
                string RoleName = Utilities.GetSessionValue("Role", HttpContext);

                List<ILocationModel> locationModel = await _commonBL.GetAllLocations("_locationList");

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
                    string location = Utilities.GetSessionValue("Location", HttpContext);
                    ViewBag.defaultLocationValue = location;

                    int locationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", HttpContext));
                    ViewBag.defaultLocationId = locationId.ToString();
                }

                List<string> stringValues = new List<string> { "January", "February", "March", "April", "May", "Jun", "July", "August", "September", "October", "November", "December" };
                List<SelectListItem> selectListItems = new List<SelectListItem>();
                int index = 0;

                foreach (var item in stringValues)
                {
                    selectListItems.Add(new SelectListItem
                    {
                        Text = item,
                        Value = item,
                        Selected = (item == DateTime.Now.ToString("MMMM"))
                    });
                    index++; // Manually increment the index
                }

                ViewBag.Months = selectListItems;


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
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
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
                    var result = await _inventoryBL.GetPurchaseSummaryData(locationId);

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
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
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

                    string fileName = $"RawData_Report_{DateTime.Now.ToString("dd_MM_yy_HH_mm_ss")}.xlsx";

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

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> PaymentSummary()
        {
            try
            {
                string RoleName = Utilities.GetSessionValue("Role", HttpContext);

                List<ILocationModel> locationModel = await _commonBL.GetAllLocations("_locationList");

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.RemoveAll(r => r.LocationId == 1);
                    if (!locationModel.Any(l => l.LocationId == 0))
                        locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Store Location" });
                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Store Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                //var result = await _inventoryBL.GetStoreTransferCalculation(0);

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetStoreTransferRawData(int searchLocationId)
        {
            try
            {
                IPager pager = new Pager();

                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");
                pager.SortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                pager.SortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                pager.ColumnName = Utility.CapitalizeFirstChar(Request.Form[$"columns[{pager.SortColumnIndex}][data]"].FirstOrDefault());

                var result = await _inventoryBL.GetStoreTransferRawData(searchLocationId, pager);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetStoreTransferPaymentSummary(int searchLocationId)
        {
            try
            {
                IPager pager = new Pager();

                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");
                pager.SortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                pager.SortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                pager.ColumnName = Utility.CapitalizeFirstChar(Request.Form[$"columns[{pager.SortColumnIndex}][data]"].FirstOrDefault());

                var result = await _inventoryBL.GetStoreTransferPaymentSummary(searchLocationId, pager);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetStoreTransferCalculation(int searchLocationId)
        {
            try
            {
                var result = await _inventoryBL.GetStoreTransferCalculation(searchLocationId);

                return Json(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
