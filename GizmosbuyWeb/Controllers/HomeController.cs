using System.Diagnostics;
using GizmosbuyWeb.Configurations;
using GizmosbuyWeb.Filters;
using GizmosbuyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GizmosbuyWeb.Controllers
{
    [EnableCors("MyPolicy")]
    [CustomAuthorize(Role.User)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

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
