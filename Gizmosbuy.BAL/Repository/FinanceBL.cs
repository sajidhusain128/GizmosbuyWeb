using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Gizmosbuy.BAL.Repository
{
    public class FinanceBL : IFinanceBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FinanceBL(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<object> GetExpenseList(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var totalCount = 0;
                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue.Trim() ?? "";
                string columnName = pager.ColumnName ?? "";
                string sortDirection = pager.SortDirection ?? "";

                var monthListDict = Utilities.GetMonthList();

                IEnumerable<ExpenseModel> mainData = null;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    mainData = await _applicationDbContext.Expenses
                                        .Join(_applicationDbContext.ExpenseTypeMasters, E => E.ExpenseTypeId, ET => ET.ExpenseTypeId, (E, ET) => new { E, ET })
                                        .Join(_applicationDbContext.PaymentModeMasters, E2 => E2.E.PaymentModeId, PM => PM.PaymentModeId, (E2, PM) => new { E2, PM })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.E2.E.ExpenseId,
                                            ExpenseDate = S.E2.E.ExpenseDate.GetValueOrDefault(),
                                            Amount = S.E2.E.Amount.GetValueOrDefault(),
                                            ExpenseTypeName = S.E2.ET.ExpenseTypeName,
                                            Remark = S.E2.E.Remark,
                                            PaymentModeName = S.PM.PaymentModeName,
                                            ExpenseMonth = S.E2.E.ExpenseMonth.GetValueOrDefault(),
                                            ExpenseMonthName = "",// monthListDict.FirstOrDefault(f => f.Value == S.E2.E.ExpenseMonth.GetValueOrDefault()).Key,
                                            ExpenseYear = S.E2.E.ExpenseYear.GetValueOrDefault(),
                                        })
                                        .ToListAsync();

                    mainData = mainData.Join(monthListDict, M => M.ExpenseMonth, ML => ML.Value, (M, ML) => new { M, ML })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.M.ExpenseId,
                                            ExpenseDate = S.M.ExpenseDate,
                                            Amount = S.M.Amount,
                                            ExpenseTypeName = S.M.ExpenseTypeName,
                                            Remark = S.M.Remark,
                                            PaymentModeName = S.M.PaymentModeName,
                                            ExpenseMonth = S.M.ExpenseMonth,
                                            ExpenseMonthName = S.ML.Key,
                                            ExpenseYear = S.M.ExpenseYear,
                                        }).Where(Utilities.GetSearchValue<ExpenseModel>(searchValue, Constant.GlobalDateFormat))
                                        .Skip(start)
                                        .Take(length);

                    totalCount = mainData.Count();
                }
                else
                {
                    mainData = await _applicationDbContext.Expenses
                                        .Join(_applicationDbContext.ExpenseTypeMasters, E => E.ExpenseTypeId, ET => ET.ExpenseTypeId, (E, ET) => new { E, ET })
                                        .Join(_applicationDbContext.PaymentModeMasters, E2 => E2.E.PaymentModeId, PM => PM.PaymentModeId, (E2, PM) => new { E2, PM })
                                        .Skip(start)
                                        .Take(length)
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.E2.E.ExpenseId,
                                            ExpenseDate = S.E2.E.ExpenseDate.GetValueOrDefault(),
                                            Amount = S.E2.E.Amount.GetValueOrDefault(),
                                            ExpenseTypeName = S.E2.ET.ExpenseTypeName,
                                            Remark = S.E2.E.Remark,
                                            PaymentModeName = S.PM.PaymentModeName,
                                            ExpenseMonth = S.E2.E.ExpenseMonth.GetValueOrDefault(),
                                            ExpenseMonthName = "",// monthListDict.FirstOrDefault(f => f.Value == S.E2.E.ExpenseMonth.GetValueOrDefault()).Key,
                                            ExpenseYear = S.E2.E.ExpenseYear.GetValueOrDefault(),
                                        })
                                        .ToListAsync();

                    mainData = mainData.Join(monthListDict, M => M.ExpenseMonth, ML => ML.Value, (M, ML) => new { M, ML })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.M.ExpenseId,
                                            ExpenseDate = S.M.ExpenseDate,
                                            Amount = S.M.Amount,
                                            ExpenseTypeName = S.M.ExpenseTypeName,
                                            Remark = S.M.Remark,
                                            PaymentModeName = S.M.PaymentModeName,
                                            ExpenseMonth = S.M.ExpenseMonth,
                                            ExpenseMonthName = S.ML.Key,
                                            ExpenseYear = S.M.ExpenseYear,
                                        });

                    totalCount = mainData.Count();
                }

                // Apply sorting
                if (!string.IsNullOrEmpty(columnName))
                {
                    mainData = mainData.OrderByDynamic(columnName, sortDirection).ToList();
                }

                var filterCount = totalCount;


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

        public async Task<int> CreateExpense(ExpenseModel expenseModel)
        {
            try
            {
                int response = 0;
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                DateTime expenseDateTime = expenseModel.ExpenseDate.Date + DateTime.Now.TimeOfDay;

                Expense expense = new Expense
                {
                    ExpenseDate = expenseDateTime,
                    Amount = expenseModel.Amount,
                    ExpenseTypeId = expenseModel.ExpenseTypeId,
                    Remark = expenseModel.Remark,
                    PaymentModeId = expenseModel.PaymentModeId,
                    ExpenseMonth = expenseModel.ExpenseMonth,
                    ExpenseYear = expenseModel.ExpenseYear,
                    CreatedBy = sessionUserId,
                    CreatedDate = DateTime.Now
                };
                _applicationDbContext.Expenses.Add(expense);
                response = await _applicationDbContext.SaveChangesAsync();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IExpenseModel> GetExpenseByID(int id)
        {
            try
            {
                var expense = await _applicationDbContext.Expenses.FindAsync(id);

                IExpenseModel expenseModel = new ExpenseModel
                {
                    ExpenseId = expense.ExpenseId,
                    ExpenseDate = expense.ExpenseDate.Value,
                    Amount = expense.Amount.Value,
                    ExpenseTypeId = expense.ExpenseTypeId.Value,
                    Remark = expense.Remark,
                    PaymentModeId = expense.PaymentModeId.Value,
                    ExpenseMonth = expense.ExpenseMonth.Value,
                    ExpenseYear = expense.ExpenseYear.Value,
                };

                return expenseModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateExpense(ExpenseModel expenseModel)
        {
            try
            {
                int response = 0;
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                DateTime expenseDateTime = expenseModel.ExpenseDate.Date + DateTime.Now.TimeOfDay;

                var expense = await _applicationDbContext.Expenses.FindAsync(expenseModel.ExpenseId);

                if (expense != null)
                {
                    expense.ExpenseDate = expenseDateTime;
                    expense.Amount = expenseModel.Amount;
                    expense.ExpenseTypeId = expenseModel.ExpenseTypeId;
                    expense.Remark = expenseModel.Remark;
                    expense.PaymentModeId = expenseModel.PaymentModeId;
                    expense.ExpenseMonth = expenseModel.ExpenseMonth;
                    expense.ExpenseYear = expenseModel.ExpenseYear;
                    expense.ModifiedBy = sessionUserId;
                    expense.ModifiedDate = DateTime.Now;
                }
                ;

                response = await _applicationDbContext.SaveChangesAsync();
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteExpense(int id)
        {
            try
            {
                int response = 0;
                var expense = await _applicationDbContext.Expenses.FindAsync(id);

                if (expense != null)
                {
                    _applicationDbContext.Expenses.Remove(expense);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IExpenseSummaryModel>> GetExpenseSummaryData(ISummaryModel summaryModel)
        {
            try
            {
                List<IExpenseSummaryModel> expenseSummaryModelList = null;

                var salesDataList = await _applicationDbContext.Procedures.spGetExpenseSummaryDataAsync(summaryModel.LocationId, summaryModel.SellMonth, summaryModel.SellYear);

                if (salesDataList != null && salesDataList.Count > 0)
                {
                    expenseSummaryModelList = new List<IExpenseSummaryModel>();

                    foreach (var item in salesDataList)
                    {
                        expenseSummaryModelList.Add(new ExpenseSummaryModel
                        {
                            ExpenseType = item.ExpenseType,
                            SumAmount = item.SumAmount,
                            OrderBy = item.OrderBy.GetValueOrDefault()
                        });
                    }
                }

                return expenseSummaryModelList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IList<ExpenseModel>> GetExpenseExport(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                string searchValue = pager.SearchValue.Trim() ?? "";

                var monthListDict = Utilities.GetMonthList();

                IEnumerable<ExpenseModel> mainData = null;

                if (!string.IsNullOrEmpty(searchValue))
                {
                    mainData = await _applicationDbContext.Expenses
                                        .Join(_applicationDbContext.ExpenseTypeMasters, E => E.ExpenseTypeId, ET => ET.ExpenseTypeId, (E, ET) => new { E, ET })
                                        .Join(_applicationDbContext.PaymentModeMasters, E2 => E2.E.PaymentModeId, PM => PM.PaymentModeId, (E2, PM) => new { E2, PM })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.E2.E.ExpenseId,
                                            ExpenseDate = S.E2.E.ExpenseDate.GetValueOrDefault(),
                                            Amount = S.E2.E.Amount.GetValueOrDefault(),
                                            ExpenseTypeName = S.E2.ET.ExpenseTypeName,
                                            Remark = S.E2.E.Remark,
                                            PaymentModeName = S.PM.PaymentModeName,
                                            ExpenseMonth = S.E2.E.ExpenseMonth.GetValueOrDefault(),
                                            ExpenseMonthName = "",// monthListDict.FirstOrDefault(f => f.Value == S.E2.E.ExpenseMonth.GetValueOrDefault()).Key,
                                            ExpenseYear = S.E2.E.ExpenseYear.GetValueOrDefault(),
                                        })
                                        .ToListAsync();

                    mainData = mainData.Join(monthListDict, M => M.ExpenseMonth, ML => ML.Value, (M, ML) => new { M, ML })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.M.ExpenseId,
                                            ExpenseDate = S.M.ExpenseDate,
                                            Amount = S.M.Amount,
                                            ExpenseTypeName = S.M.ExpenseTypeName,
                                            Remark = S.M.Remark,
                                            PaymentModeName = S.M.PaymentModeName,
                                            ExpenseMonth = S.M.ExpenseMonth,
                                            ExpenseMonthName = S.ML.Key,
                                            ExpenseYear = S.M.ExpenseYear,
                                        }).Where(Utilities.GetSearchValue<ExpenseModel>(searchValue, Constant.GlobalDateFormat));
                }
                else
                {
                    mainData = await _applicationDbContext.Expenses
                                        .Join(_applicationDbContext.ExpenseTypeMasters, E => E.ExpenseTypeId, ET => ET.ExpenseTypeId, (E, ET) => new { E, ET })
                                        .Join(_applicationDbContext.PaymentModeMasters, E2 => E2.E.PaymentModeId, PM => PM.PaymentModeId, (E2, PM) => new { E2, PM })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.E2.E.ExpenseId,
                                            ExpenseDate = S.E2.E.ExpenseDate.GetValueOrDefault(),
                                            Amount = S.E2.E.Amount.GetValueOrDefault(),
                                            ExpenseTypeName = S.E2.ET.ExpenseTypeName,
                                            Remark = S.E2.E.Remark,
                                            PaymentModeName = S.PM.PaymentModeName,
                                            ExpenseMonth = S.E2.E.ExpenseMonth.GetValueOrDefault(),
                                            ExpenseMonthName = "",// monthListDict.FirstOrDefault(f => f.Value == S.E2.E.ExpenseMonth.GetValueOrDefault()).Key,
                                            ExpenseYear = S.E2.E.ExpenseYear.GetValueOrDefault(),
                                        })
                                        .ToListAsync();

                    mainData = mainData.Join(monthListDict, M => M.ExpenseMonth, ML => ML.Value, (M, ML) => new { M, ML })
                                        .Select(S => new ExpenseModel
                                        {
                                            ExpenseId = S.M.ExpenseId,
                                            ExpenseDate = S.M.ExpenseDate,
                                            Amount = S.M.Amount,
                                            ExpenseTypeName = S.M.ExpenseTypeName,
                                            Remark = S.M.Remark,
                                            PaymentModeName = S.M.PaymentModeName,
                                            ExpenseMonth = S.M.ExpenseMonth,
                                            ExpenseMonthName = S.ML.Key,
                                            ExpenseYear = S.M.ExpenseYear,
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
