using ClosedXML.Excel;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Configurations;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Gizmosbuy.Web.Controllers
{
    public class StoreController : Controller
    {
        private readonly IStoreTransferBL _storeTransferBL;
        private readonly ICommonBL _commonBL;
        public StoreController(IStoreTransferBL storeTransferBL, ICommonBL commonBL)
        {
            _storeTransferBL = storeTransferBL;
            _commonBL = commonBL;
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> Index()
        {
            List<ILocationModel> locationModel = await _commonBL.GetAllLocations("_locationList");

            if (locationModel != null && locationModel.Count > 0)
            {
                ViewBag.LocationModelST = locationModel;
            }

            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> CreateTransfer()
        {
            try
            {
                List<ILocationModel> cachedLocationList = await _commonBL.GetAllLocations("_locationList");
                List<ILocationModel> locationModel = [.. cachedLocationList.Any() ? cachedLocationList : null];

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.RemoveAll(r => r.LocationId == 1);
                    if (!locationModel.Any(l => l.LocationId == 0))
                        locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" });

                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                string newBillNo = await _storeTransferBL.GenerateStoreTransferNewBillNo();
                ViewBag.NewBillNo = newBillNo;

                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> SaveTempStoreTransfer(TempStoreTransferModel tempStoreTransferModel)
        {
            try
            {
                int i = await _storeTransferBL.CreateTempStoreTransfer(tempStoreTransferModel);

                if (i > 0)
                {
                    return Json("Success");
                }
                else if (i == -1)
                {
                    return Json("Exist");
                }
                else if (i == -2)
                {
                    return Json("SameLocation");
                }

                return Json("Failed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetTempStoreTransferList()
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

                var purcahseList = await _storeTransferBL.GetTempStoreTransferList(pager);

                return Json(purcahseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> SaveStoreTransfer()
        {
            try
            {
                var response = await _storeTransferBL.CreateStoreTransfer();

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetTempStoreTransferEdit(int Id)
        {
            try
            {
                ITempStoreTransferModel tempStoreTransferModel = await _storeTransferBL.GetTempStoreTransferByID(Id);

                if (tempStoreTransferModel != null)
                {
                    return Json(tempStoreTransferModel);
                }
                else
                {
                    return Json(null);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
        public async Task<IActionResult> UpdateTempStoreTransfer(TempStoreTransferModel tempStoreTransferModel)
        {
            try
            {
                int i = await _storeTransferBL.UpdateTempStoreTransfer(tempStoreTransferModel);

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
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetStoreTransferList(int searchToLocationId)
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

                var storeTransferList = await _storeTransferBL.GetStoreTransferList(pager, searchToLocationId);

                return Json(storeTransferList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
        public async Task<IActionResult> TempStoreTransferDelete(int Id)
        {
            try
            {
                var response = await _storeTransferBL.TempStoreTransferDelete(Id);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetReturnItemInvoiceDetails(string invoiceNo)
        {
            try
            {
                List<IStoreTransferModel> response = await _storeTransferBL.GetReturnItemInvoiceDetails(invoiceNo);

                if (response != null)
                {
                    return PartialView("_PartialReturnInvoiceDetailsList", response);
                }
                else
                {
                    return PartialView("_PartialReturnInvoiceDetailsList", null);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> DeleteStoreTransferByInvoice(string invoiceNo, int purchaseId)
        {
            try
            {
                var response = await _storeTransferBL.DeleteStoreTransferByInvoice(invoiceNo, purchaseId);

                if (response > 0)
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
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> SendStoreReturnItemNotification(List<StoreReturnItemNotificationModel> storeReturnItemNotificationList)
        {
            try
            {
                var response = await _storeTransferBL.SendStoreReturnItemNotification(storeReturnItemNotificationList);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetStoreRetunNotificationsList()
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

                var response = await _storeTransferBL.GetStoreRetunNotificationsList(pager);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> RejectStoreReturnItems(int Id)
        {
            try
            {
                var response = await _storeTransferBL.RejectStoreReturnItems(Id);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [CustomAuthorize(Role.Admin)]
        public IActionResult TransferPayment()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [CustomAuthorize(Role.Admin)]
        public async Task<IActionResult> CreateTransferPayment()
        {
            try
            {
                List<ILocationModel> cachedLocationList = await _commonBL.GetAllLocations("_locationList");
                List<ILocationModel> locationModel = [.. cachedLocationList.Any() ? cachedLocationList : null];

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.RemoveAll(r => r.LocationId == 1);
                    if (!locationModel.Any(l => l.LocationId == 0))
                        locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" });

                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.Admin)]
        public async Task<IActionResult> SaveTransferPayment(TransferPaymentModel transferPaymentModel)
        {
            try
            {
                int i = await _storeTransferBL.CreateTransferPayment(transferPaymentModel);

                if (i == 1)
                {
                    return Json("Success");
                }
                else if (i == -1)
                {
                    return Json("SameLocation");
                }

                return Json("Failed");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.Admin)]
        public async Task<IActionResult> GetTransferPaymentList()
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

                var response = await _storeTransferBL.GetTransferPaymentList(pager);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetTransferPaymentNotificationsList()
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

                var response = await _storeTransferBL.GetTransferPaymentNotificationsList(pager);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [CustomAuthorize(Role.Admin)]
        public async Task<IActionResult> TransferPaymentDelete(int Id)
        {
            try
            {
                var response = await _storeTransferBL.TransferPaymentDelete(Id);

                if (response > 0)
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
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> TransferPaymentStausUpdate(int Id, string Type)
        {
            try
            {
                var response = await _storeTransferBL.TransferPaymentStausUpdate(Id, Type);

                if (response > 0)
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

        [CustomAuthorize(Role.Admin)]
        public async Task<IActionResult> EditTransferPayment(int Id)
        {
            try
            {
                List<ILocationModel> cachedLocationList = await _commonBL.GetAllLocations("_locationList");
                List<ILocationModel> locationModel = [.. cachedLocationList.Any() ? cachedLocationList : null];

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.RemoveAll(r => r.LocationId == 1);
                    if (!locationModel.Any(l => l.LocationId == 0))
                        locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" });

                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                ITransferPaymentModel response = await _storeTransferBL.GetTransferPaymentByID(Id);

                return View(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.Admin)]
        public async Task<IActionResult> UpdateTransferPayment(TransferPaymentModel transferPaymentModel)
        {
            try
            {
                int i = await _storeTransferBL.UpdateTransferPayment(transferPaymentModel);

                if (i == 1)
                {
                    return Json("Success");
                }
                else if (i == -1)
                {
                    return Json("SameLocation");
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
        public async Task<IActionResult> StoreTransferExportExcel(string Search, int searchToLocationId)
        {
            try
            {
                IPager pager = new Pager();

                pager.SearchValue = Search ?? "";

                var result = await _storeTransferBL.GetStoreTransferExport(searchToLocationId, pager);

                if (result != null && result.Count > 0)
                {
                    DataTable dt = Utilities.CreateDataTable(result); // Fetch your data

                    string fileName = $"StoreTransfer_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";

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

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> TransferPaymentExportExcel(string Search)
        {
            try
            {
                IPager pager = new Pager();

                pager.SearchValue = Search ?? "";

                var result = await _storeTransferBL.GetTransferPaymentExport(pager);

                if (result != null && result.Count > 0)
                {
                    DataTable dt = Utilities.CreateDataTable(result); // Fetch your data

                    if (dt.Columns.Count > 0)
                    {
                        if (dt.Columns.Contains("TransferPaymentID"))
                        {
                            dt.Columns.Remove("TransferPaymentID");
                        }
                        if (dt.Columns.Contains("IsApproved"))
                        {
                            dt.Columns.Remove("IsApproved");
                        }
                    }

                    string fileName = $"TransferPayment_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";

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
