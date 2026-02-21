using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.Web.Filters;
using GizmosbuyWeb.Filters;
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

        public IActionResult Login()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.ShowValidationSummary = true;
                }

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
        public async Task<IActionResult> Login(UserModel UserModel)
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
        [CustomAuthorize(Role.SuperAdmin, Role.Admin, Role.User)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await CreateSignOut();
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
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
