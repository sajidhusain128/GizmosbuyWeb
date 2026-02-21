using Gizmosbuy.BAL.Commons;
using Gizmosbuy.Core.Constants;

namespace Gizmosbuy.Web.Configurations
{
    public class Constants
    {
        public static void SetSessionInstance(HttpContext httpContext)
        {
            ConstantsSessions.UserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", httpContext));
            ConstantsSessions.UserName = Utilities.GetSessionValue("UserName", httpContext);
            ConstantsSessions.Email = Utilities.GetSessionValue("Email", httpContext);
            ConstantsSessions.Role = Utilities.GetSessionValue("Role", httpContext);
            ConstantsSessions.Location = Utilities.GetSessionValue("Location", httpContext);
            ConstantsSessions.LocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", httpContext));
        }

    }
}
