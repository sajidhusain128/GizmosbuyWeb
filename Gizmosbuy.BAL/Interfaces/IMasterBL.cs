using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IMasterBL
    {
        Task<object> GetUserList(IPager pager);
        Task<IUserModel> GetUserById(int id);
        Task<int> UpdateUserPassword(IUserModel userModel);
        Task<IList<UserModel>> GetUserPasswordExport(IPager pager);
    }
}
