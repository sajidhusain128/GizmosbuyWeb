using System.Net;
using System.Security.Claims;
using Gizmosbuy.Core.Constants;
using GizmosbuyWeb.Configurations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GizmosbuyWeb.Filters
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(params string[] claim) : base(typeof(AuthorizeFilter))
        {
            Arguments = new object[] { claim };
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
            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            var claimsIndentity = context.HttpContext.User.Identity as ClaimsIdentity;
            var userSession = context.HttpContext.Session.GetString("Role");


            if (IsAuthenticated)
            {
                bool flagClaim = false;
                if (_claim.Length > 0)
                {
                    foreach (var item in _claim)
                    {
                        if (context.HttpContext.User.HasClaim("Role", item))
                        {
                            flagClaim = true;
                        }
                        else if (userSession == Role.SuperAdmin || userSession == Role.Admin)
                        {
                            flagClaim = true;
                        }
                    }

                }
                else if (context.HttpContext.User.HasClaim("Role", Role.Admin) || context.HttpContext.User.HasClaim("Role", Role.SuperAdmin))
                {
                    flagClaim = true;
                }

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
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    if (context.HttpContext.Request.Cookies.Count > 0)
                    {
                        context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        var returnUrl = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            context.Result = new RedirectResult($"~/Auth/Login?returnUrl={returnUrl}&timeout=true");
                        }
                        else
                        {
                            context.Result = new RedirectResult("~/Auth/Login?timeout=true");
                        }
                    }
                    else
                    {
                        context.Result = new RedirectResult("~/Auth/Login");
                    }
                    
                }
            }
            return;
        }
    }
}
