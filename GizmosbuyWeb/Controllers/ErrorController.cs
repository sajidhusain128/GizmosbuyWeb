using GizmosbuyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Gizmosbuy.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index(int statusCode)
        {
            if (statusCode == 404)
            {
                ViewBag.StatusCode = 404;

                return View("PageNotFound");
            }
            else if (statusCode == 401)
            {
                ViewBag.StatusCode = 401;

                return View("UnauthorizedAccess");
            }
            else if (statusCode == 500)
            {
                ViewBag.StatusCode = 500;

                var model = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };
                return View("InternalServerError", model);
            }

            return View();
        }

        [Route("Error/UnauthorizedAccess")]
        public IActionResult UnauthorizedAccess()
        {
            return View();
        }

        [Route("Error/PageNotFound")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("Error/InternalServerError")]
        public IActionResult InternalServerError()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
