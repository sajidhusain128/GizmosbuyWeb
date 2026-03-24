using System.Diagnostics;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Web.Filters;
using GizmosbuyWeb.Filters;
using GizmosbuyWeb.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GizmosbuyWeb.Controllers
{
    [NoCache]
    [EnableCors(Constant.MyPolicy)]
    [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
    public class HomeController : Controller
    {
        private readonly ICacheService _cacheService;
        public HomeController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
        public IActionResult Index()
        {
            ViewBag.Location = "";

            string location = Utilities.GetSessionValue("Location", HttpContext);

            if (location != null)
            {
                ViewBag.Location = location;
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ClearCache()
        {
            _cacheService.ClearAll();

            return Content("Cleared all cache items.");
        }
    }
}
