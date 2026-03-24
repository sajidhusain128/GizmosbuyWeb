using Gizmosbuy.Core.Interfaces;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IMasterBL
    {
        Task<object> GetUserList(IPager pager);
        Task<IUserModel> GetUserById(int id);
        Task<int> UpdateUserPassword(IUserModel userModel);
    }
}
