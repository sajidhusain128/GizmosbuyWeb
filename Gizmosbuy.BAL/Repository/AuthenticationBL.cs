using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;

namespace Gizmosbuy.BAL.Repository
{
    public class AuthenticationBL : IAuthenticationBL
    {
        public readonly ApplicationDbContext _applicationDbContext;
        public AuthenticationBL(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task<IUserModel> ValidateUser(IUserModel userMaster)
        {
            IUserModel? response = null;
            try
            {
                var userMasterResponse = await _applicationDbContext.Procedures.spGetValidateLoginAsync(userMaster.UserName, userMaster.Password);

                if (userMasterResponse != null && userMasterResponse.Count > 0)
                {
                    string userName = userMasterResponse.FirstOrDefault().UserName;
                    string firstName = userMasterResponse.FirstOrDefault().FirstName;
                    string userRole = userMasterResponse.FirstOrDefault().UserRole;
                    string location = userMasterResponse.FirstOrDefault().Location;
                    string email = userMasterResponse.FirstOrDefault().Email;

                    response = new UserModel
                    {
                        UserName = userName,
                        Email = email,
                        FirstName = firstName,
                        UserRole = userRole,
                        Location = location
                    };
                }

                return await Task.FromResult(response);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
