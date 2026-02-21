using System.ComponentModel.DataAnnotations;

namespace Gizmosbuy.Core.Interfaces
{
    public interface IUserModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "UserName is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string Password { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string UserRole { get; set; }
        public string Location { get; set; }
        public int locationId { get; set; }
        public string SessionId { get; set; }
    }
}
