using System.Data;
using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Constants;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Gizmosbuy.BAL.Repository
{
    public class StoreTransferBL : IStoreTransferBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public StoreTransferBL(ApplicationDbContext applicationDbContext, IHttpContextAccessor httpContextAccessor)
        {
            _applicationDbContext = applicationDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> CreateTempStoreTransfer(TempStoreTransferModel tempStoreTransferModel)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                int fromLocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));

                if (fromLocationId == tempStoreTransferModel.ToLocationId)
                {
                    return -2;
                }

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                tempStoreTransferModel.CreatedBy = sessionUserId;
                tempStoreTransferModel.CreatedDate = DateTime.Now;
                tempStoreTransferModel.FromLocationId = fromLocationId;

                var i = await _applicationDbContext.Procedures.spSaveTempStoreTransferAsync(0,
                        tempStoreTransferModel.PurchaseId,
                        tempStoreTransferModel.TransferDate,
                        tempStoreTransferModel.SellingPrice,
                        tempStoreTransferModel.Quantity,
                        tempStoreTransferModel.SellingQuantity,
                        tempStoreTransferModel.FromLocationId,
                        tempStoreTransferModel.ToLocationId,
                        tempStoreTransferModel.BillNo,
                        tempStoreTransferModel.CreatedBy,
                        tempStoreTransferModel.CreatedDate,
                        null,
                        null,
                        "INSERT",
                        outputParameter);

                return outputParameter.Value;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetTempStoreTransferList(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var tempStoreList = await _applicationDbContext.Procedures.spGetTempStoreTransferListAsync(sessionUserId);

                string searchValue = pager.SearchValue ?? "";

                List<spGetTempStoreTransferListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = tempStoreList.Where(Utilities.GetSearchValue<spGetTempStoreTransferListResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = tempStoreList;
                }

                var totalCount = mainData.Count;
                var filterCount = mainData.Count;

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

        public async Task<int> CreateStoreTransfer()
        {
            try
            {
                int response = 0;
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                string fromlocation = Utilities.GetSessionValue("Location", _httpContextAccessor.HttpContext);

                var tempStoreModels = await _applicationDbContext.TempStoreTransfers.Where(x => x.UserId == sessionUserId).ToArrayAsync();

                var toLocationName = await _applicationDbContext.LocationMasters.FindAsync(tempStoreModels.FirstOrDefault().ToLocationId);

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                if (tempStoreModels != null && tempStoreModels.Count() > 0)
                {
                    foreach (var tempStoreModel in tempStoreModels)
                    {
                        var i = await _applicationDbContext.Procedures.spSaveSalesAsync(0,
                            tempStoreModel.PurchaseId,
                            tempStoreModel.TransferDate,
                            tempStoreModel.SellingPrice,
                            tempStoreModel.SellingQuantity,
                            null,
                            toLocationName.LocationName,
                            null,
                            null,
                            fromlocation,
                            tempStoreModel.BillNo,
                            tempStoreModel.CreatedBy,
                            tempStoreModel.CreatedDate,
                            null,
                            null,
                            "INSERT",
                            outputParameter);



                        var purchases = await _applicationDbContext.Purchases.Where(x => x.PurchaseId == tempStoreModel.PurchaseId).FirstOrDefaultAsync();

                        IPurchaseModel purchaseModel = new PurchaseModel()
                        {
                            SerialNo = purchases.SerialNo,
                            PurchaseDate = tempStoreModel.TransferDate.GetValueOrDefault(),
                            CategoryId = purchases.CategoryId.GetValueOrDefault(),
                            BrandId = purchases.BrandId.GetValueOrDefault(),
                            Model = purchases.Model,
                            Specifications = purchases.Specifications,
                            PurchasePrice = tempStoreModel.SellingPrice.GetValueOrDefault(),
                            Quantity = tempStoreModel.SellingQuantity.GetValueOrDefault(),
                            UpgradePrice = 0,
                            TotalPrice = tempStoreModel.SellingPrice.GetValueOrDefault() * tempStoreModel.SellingQuantity.GetValueOrDefault(),
                            PaymentModeName = null,
                            BuyingLead = "GIZMOSBUY"
                        };

                        int purchaseLocationID = tempStoreModel.ToLocationId.GetValueOrDefault();
                        var parameterreturnValue = new OutputParameter<int?>();
                        var parameterreturnValue2 = new OutputParameter<int?>();

                        DataTable SearlNoDataTable = new DataTable();
                        SearlNoDataTable.Columns.Add("SerialNo", typeof(string));

                        var j = await _applicationDbContext.Procedures.spSavePurchaseAsync(
                            0,
                            purchaseModel.SerialNo,
                            SearlNoDataTable,
                            purchaseModel.PurchaseDate,
                            purchaseModel.CategoryId,
                            purchaseModel.BrandId,
                            purchaseModel.Model,
                            purchaseModel.Specifications,
                            purchaseModel.PurchasePrice,
                            purchaseModel.Quantity,
                            purchaseModel.UpgradePrice,
                            purchaseModel.TotalPrice,
                            purchaseModel.PaymentModeName,
                            purchaseModel.BuyingLead,
                            purchaseLocationID,
                            sessionUserId,
                            DateTime.Now,
                            "Single",
                            parameterreturnValue,
                            parameterreturnValue2
                        );

                        int? transferPurchaseId = parameterreturnValue2.Value ?? null;

                        var i1 = await _applicationDbContext.Procedures.spSaveStoreTransferAsync(0,
                            tempStoreModel.PurchaseId,
                            tempStoreModel.TransferDate,
                            tempStoreModel.SellingPrice,
                            tempStoreModel.SellingQuantity,
                            tempStoreModel.FromLocationId,
                            tempStoreModel.ToLocationId,
                            tempStoreModel.BillNo,
                            transferPurchaseId,
                            sessionUserId,
                            DateTime.Now,
                            null,
                            null,
                            "INSERT",
                            outputParameter);
                    }

                    _applicationDbContext.TempStoreTransfers.RemoveRange(tempStoreModels);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ITempStoreTransferModel> GetTempStoreTransferByID(int id)
        {
            try
            {
                var tempStoreTransferList = await _applicationDbContext.Procedures.spGetTempStoreTransferByIDAsync(id);

                if (tempStoreTransferList == null || tempStoreTransferList.Count == 0)
                {
                    return new TempStoreTransferModel();
                }

                var spGetStoreTsfrByIDResult = tempStoreTransferList.FirstOrDefault();
                ITempStoreTransferModel tempStoreTransferModel = null;

                if (spGetStoreTsfrByIDResult != null)
                {
                    tempStoreTransferModel = new TempStoreTransferModel()
                    {
                        TempStoreTransferID = spGetStoreTsfrByIDResult.TempStoreTransferID,
                        SerialNo = spGetStoreTsfrByIDResult.SerialNo,
                        CategoryId = spGetStoreTsfrByIDResult.CategoryID.GetValueOrDefault(),
                        CategoryName = spGetStoreTsfrByIDResult.CategoryName,
                        BrandId = spGetStoreTsfrByIDResult.BrandID.GetValueOrDefault(),
                        BrandName = spGetStoreTsfrByIDResult.BrandName,
                        Model = spGetStoreTsfrByIDResult.Model,
                        Specifications = spGetStoreTsfrByIDResult.Specifications,
                        Quantity = spGetStoreTsfrByIDResult.Quantity.GetValueOrDefault(),
                        SellingQuantity = spGetStoreTsfrByIDResult.SellingQuantity,
                        TransferDate = spGetStoreTsfrByIDResult.TransferDate,
                        SellingPrice = spGetStoreTsfrByIDResult.SellingPrice,
                        ToLocationId = spGetStoreTsfrByIDResult.ToLocationID.GetValueOrDefault(),
                        ToLocationName = spGetStoreTsfrByIDResult.ToLocationName,
                        BillNo = spGetStoreTsfrByIDResult.BillNo,
                        PurchaseId = spGetStoreTsfrByIDResult.PurchaseID
                    };
                }

                return tempStoreTransferModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateTempStoreTransfer(TempStoreTransferModel tempStoreTransferModel)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                tempStoreTransferModel.ModifiedBy = sessionUserId;
                tempStoreTransferModel.ModifiedDate = DateTime.Now;

                OutputParameter<int> outputParameter = new OutputParameter<int>();

                var i = await _applicationDbContext.Procedures.spSaveTempStoreTransferAsync(tempStoreTransferModel.TempStoreTransferID,
                        tempStoreTransferModel.PurchaseId,
                        tempStoreTransferModel.TransferDate,
                        tempStoreTransferModel.SellingPrice,
                        tempStoreTransferModel.Quantity,
                        tempStoreTransferModel.SellingQuantity,
                        tempStoreTransferModel.FromLocationId,
                        tempStoreTransferModel.ToLocationId,
                        tempStoreTransferModel.BillNo,
                        null,
                        null,
                        tempStoreTransferModel.ModifiedBy,
                        tempStoreTransferModel.ModifiedDate,
                        "UPDATE",
                        outputParameter);

                return await Task.FromResult(outputParameter.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<object> GetStoreTransferList(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var storeTransferList = await _applicationDbContext.Procedures.spGetStoreTransferListAsync(sessionUserId);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                List<spGetStoreTransferListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = storeTransferList.Where(Utilities.GetSearchValue<spGetStoreTransferListResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = storeTransferList;
                }

                var totalCount = mainData.Count;
                var filterCount = mainData.Count;

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

        public async Task<int> TempStoreTransferDelete(int id)
        {
            try
            {
                int response = 0;

                var tempStoreTransfers = await _applicationDbContext.TempStoreTransfers.FindAsync(id);

                if (tempStoreTransfers != null)
                {
                    _applicationDbContext.TempStoreTransfers.Remove(tempStoreTransfers);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return await Task.FromResult(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> DeleteStoreTransferByInvoice(string invoiceNo, int purchaseId)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                OutputParameter<int> outputParameter = new OutputParameter<int>();
                var i = await _applicationDbContext.Procedures.spDeleteStoreTransferByPurchaseIdAsync(invoiceNo, purchaseId, sessionUserId, outputParameter);

                return await Task.FromResult(outputParameter.Value);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<IStoreTransferModel>> GetReturnItemInvoiceDetails(string invoiceNo)
        {
            try
            {
                List<IStoreTransferModel> storeTransferModelList = null;
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var response = await _applicationDbContext.Procedures.spGetStoreTransferInvoiceDetailsAsync(invoiceNo, sessionUserId);

                if (response.Any())
                {
                    storeTransferModelList = new List<IStoreTransferModel>();

                    foreach (var item in response)
                    {
                        storeTransferModelList.Add(new StoreTransferModel
                        {
                            SalesId = item.StoreTransferID,
                            BillNo = item.BillNo,
                            SerialNo = item.SerialNo,
                            BrandName = item.BrandName,
                            Model = item.Model,
                            Specifications = item.Specifications,
                            SellingQuantity = item.SellingQuantity,
                            FromLocationId = item.FromLocationID.GetValueOrDefault(),
                            ToLocationId = item.ToLocationID.GetValueOrDefault(),
                            TransferPurchaseID = item.TransferPurchaseID.GetValueOrDefault()
                        });
                    }
                }

                return storeTransferModelList;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> SendStoreReturnItemNotification(List<StoreReturnItemNotificationModel> storeReturnItemNotificationModel)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                int response = 0;

                using (var transaction = await _applicationDbContext.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in storeReturnItemNotificationModel)
                        {
                            StoreReturnItemNotification storeReturnItemNotification = new StoreReturnItemNotification()
                            {
                                BillNo = item.BillNo,
                                FromLocationId = item.FromLocationId,
                                ToLocationId = item.ToLocationId,
                                ReturnQuantity = item.ReturnQuantity,
                                TransferPurchaseId = item.TransferPurchaseId,
                                ApprovalStatusId = null,
                                CreatedBy = sessionUserId,
                                CreatedDate = DateTime.Now
                            };

                            _applicationDbContext.StoreReturnItemNotifications.Add(storeReturnItemNotification);
                            response = await _applicationDbContext.SaveChangesAsync();
                        }

                        // If all operations succeed, commit the transaction
                        transaction.Commit();
                        response = 1;
                    }
                    catch (Exception ex)
                    {
                        // If an exception occurs, roll back all changes within the transaction
                        transaction.Rollback();
                        response = 0;
                    }
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Object> GetStoreRetunNotificationsList(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                int locationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));
                var returnNotifyList = await _applicationDbContext.Procedures.spGetStoreTransferNotificationListAsync(locationId, sessionUserId);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                IEnumerable<spGetStoreTransferNotificationListResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = returnNotifyList.Where(Utilities.GetSearchValue<spGetStoreTransferNotificationListResult>(searchValue));
                }
                else
                {
                    mainData = returnNotifyList;
                }

                var totalCount = mainData.ToList().Count;
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

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> RejectStoreReturnItems(int id)
        {
            try
            {
                int response = 0;

                var storeReturnItemNotificationModel = await _applicationDbContext.StoreReturnItemNotifications.FindAsync(id);

                if (storeReturnItemNotificationModel != null)
                {
                    storeReturnItemNotificationModel.ApprovalStatusId = ApprovalStatusConstants.Reject;

                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CreateTransferPayment(TransferPaymentModel transferPaymentModel)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                int fromLocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));

                if (fromLocationId == transferPaymentModel.ToLocationId)
                {
                    return -1;
                }

                int? toLocationID = _applicationDbContext.UserMasters.Where(x => x.UserRole == Role.SuperAdmin)
                                                                    .Select(x => x.LocationId.GetValueOrDefault())
                                                                    .FirstOrDefault();

                TransferPayment transferPayment = new TransferPayment()
                {
                    PaymentDate = transferPaymentModel.PaymentDate,
                    Amount = transferPaymentModel.Amount,
                    TransferMode = transferPaymentModel.TransferMode,
                    Remark = transferPaymentModel.Remark,
                    FromLocationId = fromLocationId,
                    ToLocationId = transferPaymentModel.ToLocationId,
                    CreatedBy = sessionUserId,
                    CreatedDate = DateTime.Now
                };

                _applicationDbContext.TransferPayments.Add(transferPayment);
                int result = await _applicationDbContext.SaveChangesAsync();

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Object> GetTransferPaymentList(IPager pager)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                var returnNotifyList = await _applicationDbContext.Procedures.spGetTransferPaymentListAsync(sessionUserId);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                IEnumerable<object> mainData = null;

                if (searchValue != "")
                {
                    mainData = returnNotifyList.Where(Utilities.GetSearchValue<object>(searchValue));
                }
                else
                {
                    mainData = returnNotifyList;
                }

                var totalCount = mainData.ToList().Count;
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

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Object> GetTransferPaymentNotificationsList(IPager pager)
        {
            try
            {
                //var returnNotifyList = await _applicationDbContext.TransferPayments.Where(x => x.IsApproved == false).ToListAsync();

                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                var returnNotifyList = await _applicationDbContext.Procedures.spGetTransferPaymentNotificationsListAsync(sessionUserId);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                IEnumerable<object> mainData = null;

                if (searchValue != "")
                {
                    mainData = returnNotifyList.Where(Utilities.GetSearchValue<object>(searchValue));
                }
                else
                {
                    mainData = returnNotifyList;
                }

                var totalCount = mainData.ToList().Count;
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

                return data;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> TransferPaymentDelete(int id)
        {
            try
            {
                int response = 0;

                var transferPayments = await _applicationDbContext.TransferPayments.FindAsync(id);

                if (transferPayments != null)
                {
                    _applicationDbContext.TransferPayments.Remove(transferPayments);
                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return await Task.FromResult(response);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> TransferPaymentStausUpdate(int id, string type)
        {
            try
            {
                int response = 0;

                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));

                var transferPayments = await _applicationDbContext.TransferPayments.FindAsync(id);

                if (transferPayments != null)
                {
                    if (type == "Approve")
                    {
                        transferPayments.IsApproved = true;
                    }
                    else if (type == "Reject")
                    {
                        transferPayments.IsApproved = false;
                    }
                    transferPayments.ModifiedBy = sessionUserId;
                    transferPayments.ModifiedDate = DateTime.Now;

                    response = await _applicationDbContext.SaveChangesAsync();
                }

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ITransferPaymentModel> GetTransferPaymentByID(int id)
        {
            try
            {
                var transferPayments = await _applicationDbContext.TransferPayments.FindAsync(id);

                ITransferPaymentModel transferPaymentModel = null;

                if (transferPayments != null)
                {
                    transferPaymentModel = new TransferPaymentModel()
                    {
                        TransferPaymentId = transferPayments.TransferPaymentId,
                        PaymentDate = transferPayments.PaymentDate.GetValueOrDefault(),
                        Amount = transferPayments.Amount.GetValueOrDefault(),
                        TransferMode = transferPayments.TransferMode,
                        Remark = transferPayments.Remark,
                        FromLocationId = transferPayments.FromLocationId.GetValueOrDefault(),
                        ToLocationId = transferPayments.ToLocationId.GetValueOrDefault(),
                        IsApproved = transferPayments.IsApproved.GetValueOrDefault()
                    };
                }

                return transferPaymentModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> UpdateTransferPayment(TransferPaymentModel transferPaymentModel)
        {
            try
            {
                int sessionUserId = Convert.ToInt32(Utilities.GetSessionValue("UserId", _httpContextAccessor.HttpContext));
                int fromLocationId = Convert.ToInt32(Utilities.GetSessionValue("LocationId", _httpContextAccessor.HttpContext));

                if (fromLocationId == transferPaymentModel.ToLocationId)
                {
                    return -1;
                }

                var transferPayments = await _applicationDbContext.TransferPayments.FindAsync(transferPaymentModel.TransferPaymentId);

                if (transferPayments != null)
                {
                    transferPayments.PaymentDate = transferPaymentModel.PaymentDate;
                    transferPayments.Amount = transferPaymentModel.Amount;
                    transferPayments.TransferMode = transferPaymentModel.TransferMode;
                    transferPayments.Remark = transferPaymentModel.Remark;
                    transferPayments.ToLocationId = transferPaymentModel.ToLocationId;
                    transferPayments.ModifiedBy = sessionUserId;
                    transferPayments.ModifiedDate = DateTime.Now;

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
    }
}
