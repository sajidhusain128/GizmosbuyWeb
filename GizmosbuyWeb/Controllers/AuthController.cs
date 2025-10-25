using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GizmosbuyWeb.Controllers
{
    [NoCache]
    [EnableCors(Constant.MyPolicy)]
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

        public IActionResult Login(string returnUrl = null, string timeout = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ShowValidationSummary = true;
                }

                if (timeout == "true")
                {
                    ViewBag.ErrorMessage = "Your session has expired due to inactivity. Please sign in again.";
                }
                else
                {
                    ViewBag.ErrorMessage = "";
                }

                ViewData["ReturnUrl"] = returnUrl;

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserModel UserModel, string? returnUrl = null)
        {
            try
            {
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
                        await CreateSignIn(user);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                            return Redirect(returnUrl);
                        else
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
            try
            {
                // In a real application, you would query the database for user credentials
                var user = await _authenticationBL.ValidateUser(UserModel);
                if (user == null)
                {
                    Console.WriteLine("User not found."); // Debug output
                }
                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                await CreateSignOut();
                Response.Cookies.Delete(".AspNetCore.Cookies");
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
