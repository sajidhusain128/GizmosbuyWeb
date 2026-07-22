using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface IStoreTransferBL
    {
        Task<int> CreateStoreTransfer();
        Task<int> CreateTempStoreTransfer(TempStoreTransferModel tempStoreTransferModel);
        Task<object> GetStoreTransferList(IPager pager);
        Task<ITempStoreTransferModel> GetTempStoreTransferByID(int id);
        Task<object> GetTempStoreTransferList(IPager pager);
        Task<int> TempStoreTransferDelete(int id);
        Task<int> UpdateTempStoreTransfer(TempStoreTransferModel tempStoreTransferModel);
        Task<int> SendStoreReturnItemNotification(List<StoreReturnItemNotificationModel> storeReturnItemNotificationModel);
        Task<int> DeleteStoreTransferByInvoice(string invoiceNo, int purchaseId);
        Task<List<IStoreTransferModel>> GetReturnItemInvoiceDetails(string invoiceNo);
        Task<object> GetStoreRetunNotificationsList(IPager pager);
        Task<int> RejectStoreReturnItems(int id);
        Task<int> CreateTransferPayment(TransferPaymentModel transferPaymentModel);
        Task<object> GetTransferPaymentList(IPager pager);
        Task<object> GetTransferPaymentNotificationsList(IPager pager);
        Task<int> TransferPaymentDelete(int id);
        Task<int> TransferPaymentStausUpdate(int id, string type);
        Task<ITransferPaymentModel> GetTransferPaymentByID(int id);
        Task<int> UpdateTransferPayment(TransferPaymentModel transferPaymentModel);
        Task<string> GenerateStoreTransferNewBillNo();
    }
}
