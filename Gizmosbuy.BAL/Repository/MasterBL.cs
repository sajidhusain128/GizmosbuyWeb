using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Gizmosbuy.BAL.Repository
{
    public class MasterBL : IMasterBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public MasterBL(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<object> GetUserList(IPager pager)
        {
            try
            {
                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue.Trim() ?? "";

                IEnumerable<UserModel> mainData = null;

                if (searchValue != "")
                {
                    mainData = _applicationDbContext.UserMasters
                        .Join(_applicationDbContext.LocationMasters, UM => UM.LocationId, LM => LM.LocationId, (UM, LM) => new { UM, LM })
                        .Select(S => new UserModel
                        {
                            UserId = S.UM.UserId,
                            UserName = S.UM.UserName,
                            Password = S.UM.Password,
                            UserRole = S.UM.UserRole,
                            LocationName = S.LM.LocationName
                        }).Where(Utilities.GetSearchValue<UserModel>(searchValue, Constant.GlobalDateFormat)).ToList();
                }
                else
                {
                    mainData = _applicationDbContext.UserMasters
                        .Join(_applicationDbContext.LocationMasters, UM => UM.LocationId, LM => LM.LocationId, (UM, LM) => new { UM, LM })
                        .Select(S => new UserModel
                        {
                            UserId = S.UM.UserId,
                            UserName = S.UM.UserName,
                            Password = S.UM.Password,
                            UserRole = S.UM.UserRole,
                            LocationName = S.LM.LocationName
                        });
                }

                var totalCount = mainData.Count();
                var filterCount = totalCount;

                mainData = mainData
                    .Skip(start)
                    .Take(length)
                    .ToList();

                var data = new
                {
                    data = mainData,
                    draw = pager.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filterCount
                };

                return await Task.FromResult(data);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IUserModel> GetUserById(int id)
        {
            try
            {
                var user = await _applicationDbContext.UserMasters.FirstOrDefaultAsync(u => u.UserId == id);
                IUserModel userModel = null;
                if (user != null)
                {
                    userModel = new UserModel
                    {
                        UserId = user.UserId,
                        UserName = user.UserName,
                        Password = user.Password
                    };
                }

                return userModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateUserPassword(IUserModel userModel)
        {
            try
            {
                var user = await _applicationDbContext.UserMasters.FirstOrDefaultAsync(u => u.UserId == userModel.UserId);

                if (user != null)
                {
                    user.Password = userModel.NewPassword;

                    int response = await _applicationDbContext.SaveChangesAsync();
                    return response;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<UserModel>> GetUserPasswordExport(IPager pager)
        {
            try
            {
                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue.Trim() ?? "";

                IEnumerable<UserModel> mainData = null;

                if (searchValue != "")
                {
                    mainData = _applicationDbContext.UserMasters
                        .Join(_applicationDbContext.LocationMasters, UM => UM.LocationId, LM => LM.LocationId, (UM, LM) => new { UM, LM })
                        .Select(S => new UserModel
                        {
                            UserId = S.UM.UserId,
                            UserName = S.UM.UserName,
                            Password = S.UM.Password,
                            UserRole = S.UM.UserRole,
                            LocationName = S.LM.LocationName
                        }).Where(Utilities.GetSearchValue<UserModel>(searchValue, Constant.GlobalDateFormat)).ToList();
                }
                else
                {
                    mainData = _applicationDbContext.UserMasters
                        .Join(_applicationDbContext.LocationMasters, UM => UM.LocationId, LM => LM.LocationId, (UM, LM) => new { UM, LM })
                        .Select(S => new UserModel
                        {
                            UserId = S.UM.UserId,
                            UserName = S.UM.UserName,
                            Password = S.UM.Password,
                            UserRole = S.UM.UserRole,
                            LocationName = S.LM.LocationName
                        });
                }

                return mainData.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
