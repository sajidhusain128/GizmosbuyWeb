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

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string UserRole { get; set; }

        public string Location { get; set; }

        public int locationId { get; set; }
    }
}
