using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ISalesBL
    {
        Task<Tuple<int, string>> CreateSales();
        Task<object> GetSalesList(IPager pager);
        Task<ISalesModel> GetSalesByID(int id);
        Task<int> UpdateSales(ISalesModel salesModel);
        Task<object> GetTempSalesList(IPager pager);
        Task<string> GenerateNewBillNo();
        Task<int> CreateTempSales(TempSalesModel tempSalesModel);
        Task<ITempSalesModel> GetTempSalesByID(int id);
        Task<int> TempSalesDelete(int id);
        Task<int> UpdateTempSales(TempSalesModel tempSalesModel);
        Task<Tuple<List<SalesDataModel>, List<SalesHeaderModel>>> GetSalesReportData(string invoiceNo);
        Task<List<ISalesModel>> GetInvoiceDetails(string invoiceNo);
        Task<int> DeleteSalesByInvoice(string invoiceNo, List<SalesReturnItems> salesReturnItems);
        Task<IList<spGetSalesListResult>> GetSalesExport(IPager pager);
    }
}
