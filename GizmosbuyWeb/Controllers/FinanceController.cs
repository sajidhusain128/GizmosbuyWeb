using ClosedXML.Excel;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Configurations;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace Gizmosbuy.Web.Controllers
{
    public class FinanceController : Controller
    {
        private readonly IFinanceBL _financeBL;
        private readonly ICommonBL _commonBL;
        public FinanceController(IFinanceBL financeBL, ICommonBL commonBL)
        {
            _financeBL = financeBL;
            _commonBL = commonBL;
        }

        public IActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> Expense()
        {
            try
            {
                List<ILocationModel> locationModel = await _commonBL.GetAllLocations("_locationList");

                if (locationModel != null && locationModel.Count > 0)
                {
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
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetExpenseList(int searchLocationId)
        {
            IPager pager = new Pager();

            try
            {
                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.Offset = (pager.PageStart / pager.PageLength) * pager.PageLength;
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");
                pager.SortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                pager.SortDirection = Request.Form["order[0][dir]"].FirstOrDefault();
                pager.ColumnName = Utility.CapitalizeFirstChar(Request.Form[$"columns[{pager.SortColumnIndex}][data]"].FirstOrDefault());

                var expenseList = await _financeBL.GetExpenseList(pager, searchLocationId);

                return Json(expenseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> CreateExpense()
        {
            try
            {
                List<IExpenseTypeModel> cachedExpenseTypeList = await _commonBL.GetAllExpenseTypes("_expenseTypeList");
                List<IExpenseTypeModel> expenseTypes = [.. cachedExpenseTypeList.Any() ? cachedExpenseTypeList : null];

                if (expenseTypes != null && expenseTypes.Count > 0)
                {
                    if (!expenseTypes.Any(p => p.ExpenseTypeId == 0))
                        expenseTypes.Insert(0, new ExpenseTypeModel { ExpenseTypeId = 0, ExpenseTypeName = "Select Expense Type" });
                    ViewBag.expenseTypes = expenseTypes;
                }
                else
                {
                    expenseTypes = new List<IExpenseTypeModel> { new ExpenseTypeModel { ExpenseTypeId = 0, ExpenseTypeName = "Select Expense Type" } };
                    ViewBag.ExpenseTypes = expenseTypes;
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

                List<ILocationModel> cachedLocationList = await _commonBL.GetAllLocations("_locationList");
                List<ILocationModel> locationModel = [.. cachedLocationList.Any() ? cachedLocationList : null];

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.RemoveAll(r => r.LocationId == 1);
                    if (!locationModel.Any(l => l.LocationId == 0))
                        locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Expense Location" });

                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                List<string> stringValues = new List<string> { "January", "February", "March", "April", "May", "Jun", "July", "August", "September", "October", "November", "December" };
                List<SelectListItem> selectListItems = new List<SelectListItem>();

                foreach (var (item, index) in stringValues.WithIndex())
                {
                    int increment = index;
                    selectListItems.Add(new SelectListItem
                    {
                        Text = item,
                        Value = (++increment).ToString()
                    });
                }

                ViewBag.Months = selectListItems;

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
        public async Task<IActionResult> SaveExpense(ExpenseModel expenseModel)
        {
            try
            {
                int i = await _financeBL.CreateExpense(expenseModel);

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

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> EditExpense([FromQuery] int Id)
        {
            try
            {
                List<IExpenseTypeModel> cachedExpenseTypeList = await _commonBL.GetAllExpenseTypes("_expenseTypeList");
                List<IExpenseTypeModel> expenseTypes = [.. cachedExpenseTypeList.Any() ? cachedExpenseTypeList : null];

                if (expenseTypes != null && expenseTypes.Count > 0)
                {
                    if (!expenseTypes.Any(p => p.ExpenseTypeId == 0))
                        expenseTypes.Insert(0, new ExpenseTypeModel { ExpenseTypeId = 0, ExpenseTypeName = "Select Expense Type" });
                    ViewBag.expenseTypes = expenseTypes;
                }
                else
                {
                    expenseTypes = new List<IExpenseTypeModel> { new ExpenseTypeModel { ExpenseTypeId = 0, ExpenseTypeName = "Select Expense Type" } };
                    ViewBag.ExpenseTypes = expenseTypes;
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

                List<ILocationModel> cachedLocationList = await _commonBL.GetAllLocations("_locationList");
                List<ILocationModel> locationModel = [.. cachedLocationList.Any() ? cachedLocationList : null];

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.RemoveAll(r => r.LocationId == 1);
                    if (!locationModel.Any(l => l.LocationId == 0))
                        locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Expense Location" });

                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Transfer Location" } };
                    ViewBag.LocationModel = locationModel;
                }

                List<string> stringValues = new List<string> { "January", "February", "March", "April", "May", "Jun", "July", "August", "September", "October", "November", "December" };
                List<SelectListItem> selectListItems = new List<SelectListItem>();

                foreach (var (item, index) in stringValues.WithIndex())
                {
                    int increment = index;
                    selectListItems.Add(new SelectListItem
                    {
                        Text = item,
                        Value = (++increment).ToString()
                    });
                }

                ViewBag.Months = selectListItems;

                IExpenseModel expenseModel = await _financeBL.GetExpenseByID(Id);

                if (expenseModel != null)
                {
                    return View(expenseModel);
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
        public async Task<IActionResult> UpdateExpense(ExpenseModel expenseModel)
        {
            try
            {
                int i = await _financeBL.UpdateExpense(expenseModel);

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

        [HttpPost()]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> DeleteExpense(int Id)
        {
            try
            {
                var response = await _financeBL.DeleteExpense(Id);

                if (response == 1)
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

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> GetExpenseSummayData(SummaryModel summaryModel)
        {
            try
            {
                var result = await _financeBL.GetExpenseSummaryData(summaryModel);

                if (result == null)
                {
                    result = new List<IExpenseSummaryModel>();
                }

                return PartialView("_ExpenseSummaryPartial", result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> ExpenseExportExcel(int searchLocationId, string Search = null, string SortBy = null, string SortOrder = null)
        {
            try
            {
                IPager pager = new Pager();

                pager.SearchValue = Search ?? "";
                pager.ColumnName = !string.IsNullOrWhiteSpace(SortBy) ? Utility.CapitalizeFirstChar(SortBy) : "";
                pager.SortDirection = !string.IsNullOrWhiteSpace(SortOrder) ? SortOrder : "";

                var result = await _financeBL.GetExpenseExport(pager, searchLocationId);

                if (result != null && result.Count > 0)
                {
                    DataTable dt = Utilities.CreateDataTable(result); // Fetch your data

                    if (dt.Columns.Count > 0)
                    {
                        if (dt.Columns.Contains("ExpenseTypeId"))
                        {
                            dt.Columns.Remove("ExpenseTypeId");
                        }
                        if (dt.Columns.Contains("PaymentModeId"))
                        {
                            dt.Columns.Remove("PaymentModeId");
                        }
                        if (dt.Columns.Contains("ExpenseMonth"))
                        {
                            dt.Columns.Remove("ExpenseMonth");
                        }
                        if (dt.Columns.Contains("ExpenseMonthName"))
                        {
                            dt.Columns["ExpenseMonthName"].ColumnName = "ExpenseMonth";
                        }
                    }

                    string fileName = $"Expense_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";

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
