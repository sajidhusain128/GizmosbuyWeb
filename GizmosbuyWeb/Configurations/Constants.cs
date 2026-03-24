using Gizmosbuy.BAL.Commons;

namespace Gizmosbuy.Web.Configurations
{
    public class Constants
    {
        public static void SetSessionInstance(HttpContext httpContext)
        {
            httpContext.Items.Add("UserId", Convert.ToInt32(Utilities.GetSessionValue("UserId", httpContext)));
            httpContext.Items.Add("UserName", Utilities.GetSessionValue("UserName", httpContext));
            httpContext.Items.Add("Email", Utilities.GetSessionValue("Email", httpContext));
            httpContext.Items.Add("Role", Utilities.GetSessionValue("Role", httpContext));
            httpContext.Items.Add("Location", Utilities.GetSessionValue("Location", httpContext));
            httpContext.Items.Add("LocationId", Convert.ToInt32(Utilities.GetSessionValue("LocationId", httpContext)));
        }

    }
}
