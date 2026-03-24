using System.Net;
using System.Security.Claims;
using GizmosbuyWeb.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GizmosbuyWeb.Filters
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = [claim];
        }
    }

    public class AuthorizeFilter : IAuthorizationFilter
    {
        readonly string[] _claim;

        public AuthorizeFilter(params string[] claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authPrinciple = context.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme).Result.Principal;
            var IsAuthenticated = authPrinciple == null ? false : authPrinciple.Identity.IsAuthenticated;


            if (IsAuthenticated)
            {
                bool flagClaim = false;
                if (_claim.Length > 0)
                {
                    foreach (var item in _claim)
                    {
                        if (authPrinciple.HasClaim(ClaimTypes.Role, item))
                        {
                            flagClaim = true;
                        }
                        //else if (userSession == Role.SuperAdmin || userSession == Role.Admin)
                        //{
                        //    flagClaim = true;
                        //}
                    }

                }
                //else if (context.HttpContext.User.HasClaim("Role", Role.Admin) || context.HttpContext.User.HasClaim("Role", Role.SuperAdmin))
                //{
                //    flagClaim = true;
                //}

                if (!flagClaim)
                {
                    if (context.HttpContext.Request.IsAjaxRequest())
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden; //Set Response 401   
                        context.Result = new RedirectResult("~/Auth/AccessDenied");
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Result = new RedirectResult("~/Auth/AccessDenied");
                    }
                }
            }
            else
            {
                if (context.HttpContext.Request.IsAjaxRequest())
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized; //Set Response 401  
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                }
                else
                {
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    context.Result = new RedirectToActionResult("Login", "Auth", null);
                }
            }
            return;
        }
    }
}
