namespace Gizmosbuy.Core.Constants
{
    public class Constant
    {
        public const string MyPolicy = "MyPolicy";
    }

    public class ConstantsSessions
    {
        public static int UserId { get; set; }
        public static string UserName { get; set; }
        public static string Email { get; set; }
        public static string Role { get; set; }
        public static string Location { get; set; }
        public static int LocationId { get; set; }
    }

    public class ApprovalStatusConstants
    {
        public const int Approve = 1;
        public const int Reject = 2;
    }
}