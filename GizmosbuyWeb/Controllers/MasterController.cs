using ClosedXML.Excel;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.BAL.Repository;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Gizmosbuy.Web.Controllers
{
    public class MasterController : Controller
    {
        private readonly IMasterBL _masterBL;
        public MasterController(IMasterBL masterBL)
        {
            _masterBL = masterBL;
        }

        public IActionResult Index()
        {
            return View();
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public IActionResult UsersPassword()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(Role.SuperAdmin)]
        public async Task<IActionResult> GetUserList()
        {
            IPager pager = new Pager();

            try
            {
                pager.PageStart = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                pager.PageLength = int.Parse(Request.Form["length"].FirstOrDefault() ?? "10");
                pager.SearchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                pager.Draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");

                var response = await _masterBL.GetUserList(pager);

                return Json(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [CustomAuthorize(Role.SuperAdmin)]
        public async Task<IActionResult> ChangePassword(int Id)
        {
            try
            {
                var response = await _masterBL.GetUserById(Id);

                return View(response);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthorize(Role.SuperAdmin)]
        public async Task<IActionResult> UpdateUserPassword(UserModel userModel)
        {
            try
            {
                var response = await _masterBL.UpdateUserPassword(userModel);

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

        [HttpGet]
        [CustomAuthorize(Role.SuperAdmin, Role.Admin)]
        public async Task<IActionResult> UserPasswordExportExcel(string Search)
        {
            try
            {
                IPager pager = new Pager();

                pager.SearchValue = Search ?? "";

                var result = await _masterBL.GetUserPasswordExport(pager);

                if (result != null && result.Count > 0)
                {

                    DataTable dt = Utilities.CreateDataTable(result); // Fetch your data

                    if (dt.Columns.Count > 0)
                    {
                        if (dt.Columns.Contains("NewPassword"))
                        {
                            dt.Columns.Remove("NewPassword");
                        }
                        if (dt.Columns.Contains("ConfirmPassword"))
                        {
                            dt.Columns.Remove("ConfirmPassword");
                        }
                        if (dt.Columns.Contains("Email"))
                        {
                            dt.Columns.Remove("Email");
                        }
                        if (dt.Columns.Contains("FirstName"))
                        {
                            dt.Columns.Remove("FirstName");
                        }
                        if (dt.Columns.Contains("locationId"))
                        {
                            dt.Columns.Remove("locationId");
                        }
                        if (dt.Columns.Contains("SessionId"))
                        {
                            dt.Columns.Remove("SessionId");
                        }
                    }

                    string fileName = $"Users_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.xlsx";

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
