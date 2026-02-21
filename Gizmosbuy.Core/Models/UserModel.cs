using System.ComponentModel.DataAnnotations;
using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.Core.Models
{
    public class UserModel : IUserModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; } = null;
        public string FirstName { get; set; } = null;
        public string UserRole { get; set; } = null;
        public string Location { get; set; } = null;
        public int locationId { get; set; } = 0;
        public string SessionId { get; set; } = null;
    }
}
