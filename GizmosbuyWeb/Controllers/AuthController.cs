using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GizmosbuyWeb.Controllers
{
    public class AuthController : BaseController
    {
        private readonly IAuthenticationBL _authenticationBL;
        public AuthController(IAuthenticationBL authenticationBL, IOptions<WebConfiguration> webConfiguration) : base(webConfiguration)
        {
            _authenticationBL = authenticationBL;
        }

        public IActionResult Index()
        {
            return Ok("App is working!;");
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            ViewBag.ErrorMessage = "";
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserModel UserModel, string? returnUrl = null)
        {
            try
            {
                ViewData["ReturnUrl"] = returnUrl;

                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (ModelState.IsValid)
                {
                    IUserModel userModel = new UserModel
                    {
                        UserName = UserModel.UserName,
                        Password = UserModel.Password
                    };

                    var user = await AuthenticateUser(userModel);

                    if (user != null)
                    {
                        await CreateAuthenticationTicket(user);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Invalid UserName and Password.";
                        ModelState.AddModelError("", "Invalid UserName and Password.");
                        return View(UserModel);
                    }
                }
                ViewBag.ErrorMessage = "";
                return View();
            }
            catch (Exception)
            {
                return View("Login", UserModel);
            }
        }

        // Authenticate the user credentials
        private async Task<IUserModel?> AuthenticateUser(IUserModel UserModel)
        {
            // In a real application, you would query the database for user credentials
            var user = await _authenticationBL.ValidateUser(UserModel);
            if (user == null)
            {
                Console.WriteLine("User not found."); // Debug output
            }
            return user;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
