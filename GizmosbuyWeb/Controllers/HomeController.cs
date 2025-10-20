using System.Diagnostics;
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
    [CustomAuthorize(Role.User)]
    public class HomeController : Controller
    {
        [CustomAuthorize(Role.User)]
        public IActionResult Index()
        {
            ViewBag.Location = "";

            if (HttpContext.Session.GetString("Location") != null)
            {
                ViewBag.Location = HttpContext.Session.GetString("Location");
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
    }
}
