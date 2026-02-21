using System.Threading.Tasks;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using GizmosbuyWeb.Filters;
using Microsoft.AspNetCore.Mvc;

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
    }
}
