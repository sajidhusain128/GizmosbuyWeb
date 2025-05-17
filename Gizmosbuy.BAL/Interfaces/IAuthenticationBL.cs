using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IAuthenticationBL
    {
        Task<IUserModel> ValidateUser(IUserModel userMaster);
    }
}
