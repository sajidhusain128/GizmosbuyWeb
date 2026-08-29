using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;

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

        public async Task<object> GetExpenseList(IPager pager, int searchLocationId)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                bool isExport = false;

                var paramReturnTotalCount = new OutputParameter<int?>();

                var expenseList = await _applicationDbContext.Procedures.spGetExpenseListAsync(sessionUserId, searchLocationId, pager.SearchValue, pager.PageLength, pager.Offset, pager.ColumnName, pager.SortDirection, isExport, paramReturnTotalCount);

                var totalCount = paramReturnTotalCount.Value.GetValueOrDefault();
                var filterCount = totalCount;

                var data = new
                {
                    data = expenseList,
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
                int sessionLocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));
                string sessionUserRole = Utilities.GetSessionValue("Role", _httpContextAccessor.HttpContext) ?? "";

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
                    LocationId = (sessionUserRole == "SuperAdmin") ? expenseModel.LocationId : sessionLocationId,
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
                    LocationId = expense.LocationId.Value
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
                int sessionLocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));
                string sessionUserRole = Utilities.GetSessionValue("Role", _httpContextAccessor.HttpContext) ?? "";

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
                    expense.LocationId = (sessionUserRole == "SuperAdmin") ? expenseModel.LocationId : sessionLocationId;
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

        public async Task<IList<spGetExpenseListResult>> GetExpenseExport(IPager pager, int searchLocationId)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                bool isExport = true;

                var paramReturnTotalCount = new OutputParameter<int?>();

                var expenseList = await _applicationDbContext.Procedures.spGetExpenseListAsync(sessionUserId, searchLocationId, pager.SearchValue, null, null, pager.ColumnName, pager.SortDirection, isExport, paramReturnTotalCount);

                return expenseList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
