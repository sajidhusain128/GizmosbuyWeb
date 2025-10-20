using Microsoft.AspNetCore.Mvc.Filters;

namespace Gizmosbuy.Web.Filters
{
    public class NoCacheAttribute : ActionFilterAttribute
    {
        // For prevent histroy back page in browser after logout.
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            filterContext.HttpContext.Response.Headers["Pragma"] = "no-cache";
            filterContext.HttpContext.Response.Headers["Expires"] = "0";
            base.OnResultExecuting(filterContext);
        }
    }
}
