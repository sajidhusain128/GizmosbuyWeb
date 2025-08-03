using System.Data;
using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using FastReport.Web;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.BAL.Repository;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;


namespace Gizmosbuy.Web.Controllers
{
    public class SalesController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        private readonly ISalesBL _salesBL;
        private readonly ICommonBL _commonBL;
        public SalesController(IHostingEnvironment hostingEnvironment, ISalesBL salesBL, ICommonBL commonBL)
        {
            _hostingEnvironment = hostingEnvironment;
            _salesBL = salesBL;
            _commonBL = commonBL;
        }

        [CustomAuthorize(Role.User)]
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [CustomAuthorize(Role.User)]
        public async Task<IActionResult> GetSalesList()
        {
            IPager pager = new Pager();

            try
            {
                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");

                var purcahseList = await _salesBL.GetSalesList(pager);

                return Json(purcahseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [CustomAuthorize]
        public async Task<IActionResult> Create()
        {
            try
            {
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

                string newBillNo = await _salesBL.GenerateNewBillNo();
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
        [CustomAuthorize]
        public async Task<IActionResult> SaveSales()
        {
            try
            {
                var response = await _salesBL.CreateSales();

                //if (response.Item1 > 0)
                //{
                //    return Json("Success");
                //}

                return Json(response);
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

                List<ILocationModel> locationModel = await _commonBL.GetAllLocations();

                if (locationModel != null && locationModel.Count > 0)
                {
                    locationModel.Insert(0, new LocationModel { LocationId = 0, LocationName = "Select Payment Mode" });
                    ViewBag.LocationModel = locationModel;
                }
                else
                {
                    locationModel = new List<ILocationModel> { new LocationModel { LocationId = 0, LocationName = "Select Payment Mode" } };
                    ViewBag.LocationModel = locationModel;
                }

                ISalesModel salesModel = await _salesBL.GetSalesByID(Id);

                if (salesModel != null)
                {
                    return View(salesModel);
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
        public async Task<IActionResult> UpdateSales(SalesModel salesModel)
        {
            try
            {
                int i = await _salesBL.UpdateSales(salesModel);

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
        [CustomAuthorize(Role.User)]
        public async Task<IActionResult> GetTempSalesList()
        {
            IPager pager = new Pager();

            try
            {
                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");

                var purcahseList = await _salesBL.GetTempSalesList(pager);

                return Json(purcahseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public async Task<IActionResult> SaveTempSales(TempSalesModel tempSalesModel)
        {
            try
            {
                int i = await _salesBL.CreateTempSales(tempSalesModel);

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
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public async Task<IActionResult> UpdateTempSales(TempSalesModel tempSalesModel)
        {
            try
            {
                int i = await _salesBL.UpdateTempSales(tempSalesModel);

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

        [HttpGet]
        //[ValidateAntiForgeryToken]
        [CustomAuthorize]
        public async Task<IActionResult> GetTempSalesEdit(int Id)
        {
            try
            {
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

                ITempSalesModel tempSalesModel = await _salesBL.GetTempSalesByID(Id);

                if (tempSalesModel != null)
                {
                    return Json(tempSalesModel);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize]
        public async Task<IActionResult> TempSalesDelete(int Id)
        {
            try
            {
                var response = await _salesBL.TempSalesDelete(Id);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> LoadSalesReport(string invoiceNo)
        {
            try
            {
                var response = await _salesBL.GetSalesReportData(invoiceNo);

                var webReport = new WebReport();
                webReport = GetReport(response);

                return View(webReport);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet]
        [CustomAuthorize]
        public async Task<IActionResult> DownloadSalesReport(string invoiceNo)
        {
            try
            {
                var response = await _salesBL.GetSalesReportData(invoiceNo);

                var webReport = new WebReport();
                webReport = GetReport(response);

                using var ms = new MemoryStream();
                var pdfExport = new PDFSimpleExport();
                webReport.Report.Export(pdfExport, ms);
                ms.Position = 0;

                string fileName = $"SalesReport_{invoiceNo}_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}.pdf";

                return File(ms.ToArray(), "application/pdf", fileName);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public WebReport GetReport(Tuple<List<SalesDataModel>, List<SalesHeaderModel>> DataEntities)
        {
            try
            {
                DataSet set = new DataSet();
                DataTable dataTable = Utilities.CreateDataTable(DataEntities.Item1, "SalesData");
                DataTable dataTable2 = Utilities.CreateDataTable(DataEntities.Item2, "SalesHeader");

                RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

                string webRootPath = _hostingEnvironment.ContentRootPath;
                var webReport = new WebReport();

                webReport.Report.RegisterData(dataTable, "SalesData");
                webReport.Report.RegisterData(dataTable2, "SalesHeader");
                webReport.Report.Load(Directory.GetCurrentDirectory() + "/Reports/SalesReport.frx");

                webReport.Report.Prepare();
                bool isPrepared = webReport.Report.IsPrepared;

                return webReport;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
