using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IFinanceBL
    {
        Task<object> GetExpenseList(IPager pager);
        Task<int> CreateExpense(ExpenseModel expenseModel);
        Task<IExpenseModel> GetExpenseByID(int id);
        Task<int> UpdateExpense(ExpenseModel expenseModel);
        Task<int> DeleteExpense(int id);
        Task<List<IExpenseSummaryModel>> GetExpenseSummaryData(ISummaryModel summaryModel);
        Task<IList<ExpenseModel>> GetExpenseExport(IPager pager);
    }
}
