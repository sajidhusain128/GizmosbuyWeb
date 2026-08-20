
$(document).ready(function () {

    $('#btnAddTempTransferCreate').click(function (e) {
        e.stopPropagation();

        saveTempStoreTransfer();
    });

    $('#btnStoreTransferSave').click(function (e) {
        e.stopPropagation();

        if ($("#tblTransferCreateTemp").DataTable().data().count() > 0) {
            saveStoreTransfer();
        }
        else {
            WarningToast("Store transfer entry not added in temporary list.");
        }
    });

    $('#btnUpdateTempStoreTransfer').click(function (e) {
        e.stopPropagation();

        updateTempStoreTransfer();
    });

    $("#btnDeleteTempStoreTransfer").click(function () {
        try {
            var id = $('#hdnTempStoreTransferDeleteId').val();
            DeleteTempStoreTransfer(id)
        } catch (e) {
            console.error(e);
        }
    })

    $("#btnReturnStoreTransferModal").click(function () {
        try {
            $('#btnClearInvoiceDetails').attr("disabled", true);
            $('#btnReturnStoreTransferItemsSave').attr("disabled", true);

            showBoostrapModal('#tempReturnStoreTransferItemsModel');

        } catch (e) {
            console.error(e);
        }
    })

    $("#btnFetchInvoiceDetails").click(function () {
        try {

            var errorCount = 0

            if ($('#txtInvoiceNo').val() == "") {
                errorCount++;
                $('#spnInvoiceNoError').text("Invoice No. is required.");
            }
            else {
                $('#spnInvoiceNoError').text("");
            }

            if (errorCount > 0) {
                return false;
            }
            else {
                getInvoiceDetailsForStoreTransferRefund();
            }

        } catch (e) {
            console.error(e);
        }
    })

    $("#btnClearInvoiceDetails").click(function () {
        try {
            resetTransferReturnForm();

        } catch (e) {
            console.error(e);
        }
    })

    $("#btnReturnStoreTransferItemsClose").click(function () {
        try {
            resetTransferReturnForm();
            hideBoostrapModal('#tempReturnStoreTransferItemsModel')
        } catch (e) {
            console.error(e);
        }
    })

    $("#btnReturnStoreTransferItemsSave").click(function () {
        try {
            if ($('#txtInvoiceNo').val() !== "") {
                if ($('#hdnIvoiceNo').val() === $('#txtInvoiceNo').val()) {

                    var rows = $('#tblInvoiceDetails').DataTable().rows().nodes();
                    // Use jQuery to find all checkboxes within those rows and filter for checked ones
                    var checkedCheckboxes = $('input[type="checkbox"]:checked', rows);
                    // Get the length of the resulting jQuery object
                    var count = checkedCheckboxes.length

                    if (count > 0) {
                        $('#spnInvoiceNoError').text("");

                        showBoostrapModal('#storeTransferReturnDeleteModel');
                    }
                    else {
                        WarningToast("Please select the checkbox for items.");
                    }
                }
                else {
                    WarningToast("Fetched details and entered bill no. does not matched. So please enter correct.")
                }
            }
            else {
                $('#spnInvoiceNoError').text("Invoice No. is required.");
            }

        } catch (e) {
            console.error(e);
        }
    })

    $("#btnDeleteStoreTransferReturn").click(function () {
        try {
            //deleteStoreTransferReturnEntry();
            sendStoreReturnItemNotification();
        } catch (e) {
            console.error(e);
        }
    })

    $('#btnTransferPaymentSave').click(function (e) {
        e.stopPropagation();
        saveTransferPayment();
    });

    $('#btnTransferPaymentUpdate').click(function (e) {
        e.stopPropagation();
        saveTransferPayment();
    });

    $("#btnDeleteTransferPayment").click(function () {
        try {
            var id = $('#hdnDeleteTransferPaymentId').val();
            deleteTransferPayment(id)
        } catch (e) {
            console.error(e);
        }
    })

    $('#btnSearchStoreTransferSummary').click(function (e) {
        try {
            e.stopPropagation();

            var ddlLocationValue = parseInt($('#ddlLoactionfilter').select2('val'));

            if (ddlLocationValue > 0) {
                $('#errorLocation').text('');
                callStoreTransferRawData(ddlLocationValue);
                callStoreTransferPaymentData(ddlLocationValue);
                getStoreTransferCalculation(ddlLocationValue);
            }
            else {
                $('#errorLocation').text("Please select store.")
                callStoreTransferRawData(0);
                callStoreTransferPaymentData(0);
                getStoreTransferCalculation(0);
            }

        } catch (e) {
            console.error(e);
        }
    });

    $("#btnExportReturnStore").click(function () {
        location.href = "/Store/StoreTransferExportExcel?Search=" + tableStoreTransfer.search() + "&searchToLocationId=" + $("#ddlSellingStoreST").select2('val');
    })

    $("#btnExportTransferPayment").click(function () {
        location.href = "/Store/TransferPaymentExportExcel?Search=" + tableTransferPayment.search();
    })
});

function resetTransferReturnForm() {
    $('#hdnIvoiceNo').val("");
    $('#txtInvoiceNo').val("");
    $('#spnInvoiceNoError').text("");
    $('#divInvoiceDetails').html("");
    $('#btnFetchInvoiceDetails').attr("disabled", false);
    $('#btnClearInvoiceDetails').attr("disabled", true);
    $('#btnReturnStoreTransferItemsSave').attr("disabled", true);
}

function validateStoreTransferForm() {
    try {

        var errorCount = 0

        if ($('#txtSrNo').val() == "") {
            errorCount++;
            $('#txtSrNo').parents('.row').find('.field-validation-error').text("Serial No. is required.");
        }
        else {
            $('#txtSrNo').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtTransferDate').val() == "") {
            errorCount++;
            $('#txtTransferDate').parents('.row').find('.field-validation-error').text("Transfer Date is required.");
        }
        else {
            $('#txtTransferDate').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() == "0") {
            errorCount++;
            $('#txtQuantity').parents('.row').find('.field-validation-error').text("Purchase quatity not available.");
        }
        else {
            $('#txtQuantity').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellingPrice').val() == "") {
            errorCount++;
            $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("Enter at least 0");
        }
        else {
            $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellQuantity').val() == "" || $('#txtSellQuantity').val() == "0") {
            errorCount++;
            $('#txtSellQuantity').parents('.row').find('.field-validation-error').text("Selling Quantity is required, and should be more than 0.");
        }
        else {
            if ($('#txtQuantity').val() != "" && parseInt($('#txtSellQuantity').val()) > parseInt($('#txtQuantity').val())) {
                errorCount++;
                $('#txtSellQuantity').parents('.row').find('.field-validation-error').text("Selling Quantity should not be more than Purchase Quantity.");
            }
            else {
                $('#txtSellQuantity').parents('.row').find('.field-validation-error').text("");
            }
        }

        if ($('#ddlSellingStore').select2('val') == "0") {
            errorCount++;
            $('#ddlSellingStore').parents('.row').find('.field-validation-error').text("Selling Store is required.");
        }
        else {
            $('#ddlSellingStore').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtBillNo').val() == "") {
            errorCount++;
            $('#txtBillNo').parents('.row').find('.field-validation-error').text("Bill No is required.");
        }
        else {
            $('#txtBillNo').parents('.row').find('.field-validation-error').text("");
        }

        if (errorCount > 0) {
            return false;
        }
        else {
            return true;
        }

    } catch (e) {
        console.error(e);
    }
}

function clearStoreTransferForm() {
    try {
        $('#txtSrNo').val("");
        $('#txtCategory').val("");
        $('#txtBrand').val("");
        $('#txtModel').val("");
        $('#txtSpecs').val("");
        $('#txtQuantity').val("");
        $('#txtTransferDate').val("");
        $('#txtSellingPrice').val("");
        $('#txtSellQuantity').val("");
        $('#ddlSellingStore').select2("val", "0");
        $('#txtBillNo').val("");
    } catch (e) {
        console.error(e);
    }
}

function clearTempSalesForm() {
    try {
        $('#txtSrNoTemp').val("");
        $('#txtCategoryTemp').val("");
        $('#txtBrandTemp').val("");
        $('#txtModelTemp').val("");
        $('#txtSpecsTemp').val("");
        $('#txtQuantityTemp').val("");
        $('#txtTransferDateTemp').val("");
        $('#txtSellingPriceTemp').val("");
        $('#txtSellQuantityTemp').val("");
        $('#ddlSellingStoreTemp').select2("val", "0");
        $('#txtBillNoTemp').val("");
    } catch (e) {
        console.error(e);
    }
}

function saveTempStoreTransfer() {
    try {
        if (!validateStoreTransferForm()) {
            return false;
        }
        else {
            showLoader();

            var hdnPurchaseId = $('#hdnPurchaseId').val();
            var serialNo = $('#txtSrNo').val();
            var transferDate = $('#txtTransferDate').val();
            var sellingPrice = parseFloat($('#txtSellingPrice').val());
            var purchaseQuantity = parseInt($('#txtQuantity').val());
            var sellingQuantity = parseInt($('#txtSellQuantity').val());
            var ddlSellingStore = parseInt($('#ddlSellingStore').select2('val'));
            var billNo = $('#txtBillNo').val();

            var model = {
                purchaseId: hdnPurchaseId,
                serialNo: serialNo,
                transferDate: transferDate,
                sellingPrice: sellingPrice,
                quantity: purchaseQuantity,
                sellingQuantity: sellingQuantity,
                toLocationId: ddlSellingStore,
                billNo: billNo
            }

            var form = $("#frmStoreTransfer");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            $.ajax({
                type: "POST",
                url: '/Store/SaveTempStoreTransfer',
                data: { __RequestVerificationToken: token, tempStoreTransferModel: model },
                dataType: "json",
                //async: true,
                success: function (response) {
                    if (response == "Success") {
                        SuccessToast("Store transfer temporary entry added.");
                        clearStoreTransferForm();
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else if (response == "Exist") {
                        hideLoader();
                        WarningToast("This record already exist in temporary list.")
                    }
                    else if (response == "SameLocation") {
                        hideLoader();
                        WarningToast("Can not add temporary list for same store.");
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        ErrorToast("Error occurred while adding temporary list.");
                    }
                },
                error: function (e) {
                    hideLoader();
                    ErrorToast("Something went wrong!");
                }
            });

        }
    } catch (e) {
        console.error(e);
    }
}

function getPurchaseRecordInSales(purchaseId) {
    try {
        showLoader();

        var form = $("#frmStoreTransfer");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "GET",
            url: '/Purchase/GetPurchaseById',
            data: { __RequestVerificationToken: token, purchaseId: purchaseId },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    $('#txtSrNo').val(response.serialNo);
                    $('#txtCategory').val(response.categoryName);
                    $('#txtBrand').val(response.brandName);
                    $('#txtModel').val(response.model);
                    $('#txtSpecs').val(response.specifications);
                    $('#txtQuantity').val(response.quantity);

                    if (response.categoryName == "Laptop") {
                        $('#txtSellQuantity').val(1);
                        $('#txtSellQuantity').attr("disabled", true);
                    }
                    else {
                        $('#txtSellQuantity').val("");
                        $('#txtSellQuantity').attr("disabled", false);
                    }

                    if (response.categoryName != null && categoryList.indexOf(response.categoryName) >= 0) {
                        $('#txtSellingPrice').val(0);
                    }

                    if ($('#tblTransferCreateTemp').DataTable().rows().any()) {
                        var rowData = $('#tblTransferCreateTemp').DataTable().data();
                        if (rowData.length > 0) {
                            $('#txtTransferDate').datepicker("setDate", localDateFormat(rowData[0].transferDate, "dd/mm/yyyy"));
                            $('#ddlSellingStore').select2("val", rowData[0].locationID.toString());
                        }
                    }
                }
                hideLoader();
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });
    } catch (e) {
        console.error(e);
    }
}

function saveStoreTransfer() {
    try {
        showLoader();

        var form = $("#frmStoreTransfer");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Store/SaveStoreTransfer',
            data: { __RequestVerificationToken: token },
            dataType: "json",
            success: function (response) {
                if (response > 0) {
                    hideLoader();
                    SuccessToast("Store transfer created successfully.");

                    setTimeout(function () {
                        location.href = "/Store/Index"
                    }, 2000);
                }
                else if (response == 0) {
                    hideLoader();
                    ErrorToast("Error occurred while saving store transfer record(s).");
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function editTempStoreTransfer(id) {
    try {
        showLoader();

        $.ajax({
            type: "GET",
            url: '/Store/GetTempStoreTransferEdit',
            data: { Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {

                    showBoostrapModal('#tempStoreTransferEditModel');

                    $('#hdnTempStoreTransferId').val(response.tempStoreTransferID);
                    $('#txtSrNoTemp').val(response.serialNo);
                    $('#txtPurchaseDateTemp').val(response.purchaseDate);
                    $('#txtCategoryTemp').val(response.categoryName);
                    $('#txtBrandTemp').val(response.brandName);
                    $('#txtModelTemp').val(response.model);
                    $('#txtSpecsTemp').val(response.specifications);
                    $('#txtQuantityTemp').val(response.quantity);
                    $('#txtTransferDateTemp').val(localDateFormat(response.transferDate, "dd/mm/yyyy"));
                    $('#txtSellingPriceTemp').val(response.sellingPrice);
                    $('#txtSellQuantityTemp').val(response.sellingQuantity);
                    $('#ddlSellingStoreTemp').select2('val', response.toLocationId.toString());
                    $('#txtBillNoTemp').val(response.billNo);
                    $('#hdnTempPurchaseId').val(response.purchaseId);
                    //getPurchaseRecordInTempStoreTransfer(response.purchaseId);

                    $('#txtTransferDateTemp').datepicker({
                        format: 'dd/mm/yyyy',
                        endDate: '+0d',
                        maxDate: 'today',
                        autoclose: true,
                        todayHighlight: true,
                        todayBtn: 'linked',
                        clearBtn: true
                    });
                }
                hideLoader();
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

//function getPurchaseRecordInTempStoreTransfer(purchaseId) {
//    try {
//        showLoader();

//        var form = $("#frmTempSales");
//        var token = $('input[name="__RequestVerificationToken"]', form).val();

//        $.ajax({
//            type: "GET",
//            url: '/Purchase/GetPurchaseById',
//            data: { __RequestVerificationToken: token, purchaseId: purchaseId },
//            dataType: "json",
//            success: function (response) {
//                if (response != null) {
//                    $('#txtSrNoTemp').val(response.serialNo);
//                    $('#txtCategoryTemp').val(response.categoryName);
//                    $('#txtBrandTemp').val(response.brandName);
//                    $('#txtModelTemp').val(response.model);
//                    $('#txtSpecsTemp').val(response.specifications);
//                    $('#txtQuantityTemp').val(response.quantity);
//                }
//                hideLoader();
//            },
//            error: function (e) {
//                hideLoader();
//                ErrorToast("Something went wrong!");
//            }
//        });
//    } catch (e) {
//        console.error(e);
//    }
//}

function validateTempSalesForm() {
    try {

        var errorCount = 0

        if ($('#txtSrNoTemp').val() == "") {
            errorCount++;
            $('#txtSrNoTemp').parents('.row').find('.field-validation-error').text("Serial No. is required.");
        }
        else {
            $('#txtSrNoTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtTransferDateTemp').val() == "") {
            errorCount++;
            $('#txtTransferDateTemp').parents('.row').find('.field-validation-error').text("Transfer Date is required.");
        }
        else {
            $('#txtTransferDateTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellingPriceTemp').val() == "") {
            errorCount++;
            $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("Enter at least 0");
        }
        else {
            $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellQuantityTemp').val() == "" || $('#txtSellQuantityTemp').val() == "0") {
            errorCount++;
            $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("Selling Quantity is required, and should be more than 0.");
        }
        else {
            if ($('#txtSellQuantityTemp').val() != "" && parseInt($('#txtSellQuantityTemp').val()) > parseInt($('#txtQuantityTemp').val())) {
                errorCount++;
                $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("Selling Quantity should not be more than Purchase Quantity.");
            }
            else {
                $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("");
            }
        }

        if ($('#ddlSellingStoreTemp').select2('val') == "0") {
            errorCount++;
            $('#ddlSellingStoreTemp').parents('.row').find('.field-validation-error').text("Store loaction is required.");
        }
        else {
            $('#ddlSellingStoreTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtBillNoTemp').val() == "") {
            errorCount++;
            $('#txtBillNoTemp').parents('.row').find('.field-validation-error').text("Bill No is required.");
        }
        else {
            $('#txtBillNoTemp').parents('.row').find('.field-validation-error').text("");
        }

        if (errorCount > 0) {
            return false;
        }
        else {
            return true;
        }

    } catch (e) {
        console.error(e);
    }
}

function updateTempStoreTransfer() {
    try {
        if (!validateTempSalesForm()) {
            return false;
        }
        else {
            showLoader();
            var hdnTempStoreTransferId = $('#hdnTempStoreTransferId').val();

            if (hdnTempStoreTransferId > 0) {

                var hdnPurchaseId = $('#hdnTempPurchaseId').val();
                var serialNo = $('#txtSrNoTemp').val();
                var transferDate = $('#txtTransferDateTemp').val();
                var sellingPrice = parseFloat($('#txtSellingPriceTemp').val());
                var purchaseQuantity = parseInt($('#txtQuantityTemp').val());
                var sellingQuantity = parseInt($('#txtSellQuantityTemp').val());
                var toLocationId = $('#ddlSellingStoreTemp').select2('val');
                var billNo = $('#txtBillNoTemp').val();

                var model = {
                    tempStoreTransferID: hdnTempStoreTransferId,
                    purchaseId: hdnPurchaseId,
                    serialNo: serialNo,
                    transferDate: transferDate,
                    sellingPrice: sellingPrice,
                    quantity: purchaseQuantity,
                    sellingQuantity: sellingQuantity,
                    toLocationId: toLocationId,
                    billNo: billNo
                }

                var form = $("#frmTempStoreTransfer");
                var token = $('input[name="__RequestVerificationToken"]', form).val();

                $.ajax({
                    type: "PUT",
                    url: '/Store/UpdateTempStoreTransfer',
                    data: { __RequestVerificationToken: token, tempStoreTransferModel: model },
                    dataType: "json",
                    success: function (response) {

                        if (response == "Success") {
                            clearTempSalesForm();
                            hideBoostrapModal('#tempStoreTransferEditModel');
                            SuccessToast("Store transfer temporary entry updated.");
                            hideLoader();

                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while updating in temporary store transfer record(s).");
                        }
                    },
                    error: function (e) {
                        hideLoader();
                        ErrorToast("Something went wrong!");
                    }
                });
            }
        }
    } catch (e) {
        console.error(e);
    }
}

function showDeleteTempStoreTransferModel(id) {
    try {

        $('#hdnTempStoreTransferDeleteId').val(id);

        showBoostrapModal('#tempStoreTransferDeleteModel');

    } catch (e) {
        console.error(e);
    }
}

function DeleteTempStoreTransfer(id) {
    try {
        hideBoostrapModal('#tempStoreTransferDeleteModel');

        showLoader();
        var form = $("#frmTempStoreTransfer");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Store/TempStoreTransferDelete',
            data: { __RequestVerificationToken: token, Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    SuccessToast("Store transfer temporary entry deleted.");
                    hideLoader();

                    setTimeout(function () {
                        location.reload();
                    }, 2000);
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function getInvoiceDetailsForStoreTransferRefund() {
    try {
        showLoader();
        var invoiceNo = $("#txtInvoiceNo").val();

        if (invoiceNo.endsWith("ST")) {
            $.ajax({
                type: "GET",
                url: '/Store/GetReturnItemInvoiceDetails',
                data: { invoiceNo: invoiceNo },
                //dataType: "html",
                success: function (response) {
                    if (response !== null && response.trim() !== '') {
                        $('#divInvoiceDetails').html(response);
                        $('#btnFetchInvoiceDetails').attr("disabled", true);
                        $('#btnClearInvoiceDetails').attr("disabled", false);
                        //$('#btnReturnStoreTransferItemsSave').attr("disabled", false);

                        bindReturnItemTable();
                    }
                    else {
                        $('#divInvoiceDetails').html("");
                        $('#btnFetchInvoiceDetails').attr("disabled", false);
                        WarningToast("Entered invoice details not found.")
                    }
                    hideLoader();
                },
                error: function (e) {
                    hideLoader();
                    ErrorToast("Something went wrong!");
                }
            });
        }
        else {
            hideLoader();
            WarningToast("This invoice is for a retail sale, not a store transfer.");
        }

    } catch (e) {
        console.error(e);
    }
}

function bindReturnItemTable() {
    try {
        $('#tblInvoiceDetails').DataTable({
            maxScrollY: 360,
            columnDefs: [
                {
                    targets: 0,               // first column
                    orderable: false,         // disable sorting
                    searchable: false,        // disable search
                    className: 'dt-body-center'

                },
                {
                    targets: '_all',
                    className: 'text-nowrap'
                },
                {
                    targets: -2, // second last column
                    className: 'dt-body-center'
                },
                {
                    targets: -1, // last column
                    className: 'dt-body-center'
                }

            ],
            layout: {
                topStart: 'info',
                topEnd: 'search',
                bottomStart: ''
            },
            fixedColumns: {
                start: 2
            },
            select: {
                'style': 'multi'
            },
            order: [[1, 'asc']],
            paging: false
        }).on('draw', function () {
            var total = $('[id^="tblInvoiceDetails_"] tbody input.row-check').length;
            var checked = $('[id^="tblInvoiceDetails_"] tbody input.row-check:checked').length;
            //$('#check-all').prop('checked', total > 0 && total === checked);

            var selectAll = $('#check-all').get(0);

            if (checked === 0) {
                selectAll.checked = false;
                selectAll.indeterminate = false;
            }
            else if (checked === total) {
                selectAll.checked = true;
                selectAll.indeterminate = false;
            }
            else {
                selectAll.checked = false;
                selectAll.indeterminate = true;
            }

        });

        // Select all
        $('[id^="tblInvoiceDetails_"] #check-all').on('click', function () {
            $('[id^="tblInvoiceDetails_"] tbody input.row-check').prop('checked', this.checked);
        });

        // Sync header checkbox
        $('[id^="tblInvoiceDetails_"] tbody').on('change', 'input.row-check', function () {
            var total = $('[id^="tblInvoiceDetails_"] tbody input.row-check').length;
            var checked = $('[id^="tblInvoiceDetails_"] tbody input.row-check:checked').length;
            //$('#check-all').prop('checked', total === checked);

            var selectAll = $('#check-all').get(0);

            if (checked === 0) {
                // none selected
                selectAll.checked = false;
                selectAll.indeterminate = false;
            }
            else if (checked === total) {
                // all selected
                selectAll.checked = true;
                selectAll.indeterminate = false;
            }
            else {
                // some selected → show minus
                selectAll.checked = false;
                selectAll.indeterminate = true;
            }
        });

        $('[id^="tblInvoiceDetails_"] tbody').on('change', 'input.row-check', function () {
            enableDisableRowInputText($('#tblInvoiceDetails').DataTable(), $(this));
        });
        $('[id^="tblInvoiceDetails_"] #check-all').on('change', function () {
            $('[id^="tblInvoiceDetails_"] tbody input.row-check').change();
        });

    } catch (e) {
        console.error(e);
    }
}

function enableDisableRowInputText(table, input) {
    // Get the closest table row (<tr>) to the clicked checkbox
    var $row = $(input).closest('tr');

    // 3. Use the DataTables API to get the data for the specific row
    var $rowData = table.row($row).data();

    var $textbox = $row.find('.row-input');

    // Check if the checkbox is currently checked or unchecked
    if (input[0].checked) {
        $textbox.prop('disabled', false)
        table.row($row).selector.rows[0].style = "background-color: #f8b739";
    } else {
        $textbox.prop('disabled', true)
        $textbox.val($rowData[6]);
        $textbox.next('.field-validation-error').text('');
        table.row($row).selector.rows[0].style = "";
    }

    checkCheckedReturnItems(table);
}
function checkCheckedReturnItems(table) {
    try {
        var rows = table.rows().nodes();
        // Use jQuery to find all checkboxes within those rows and filter for checked ones
        var checkedCheckboxes = $('input[type="checkbox"]:checked', rows);
        // Get the length of the resulting jQuery object
        var count = checkedCheckboxes.length

        if (count > 0) {
            $('#btnReturnStoreTransferItemsSave').prop('disabled', false);
        }
        else {
            $('#btnReturnStoreTransferItemsSave').prop('disabled', true);
        }
    } catch (e) {
        console.error(e);
    }
}


function checkReturnQtyValidity(table) {
    try {
        var allValid = true;

        var allInputs = table.$('.row-input');
        //var $error = $row.find('.field-validation-error');

        allInputs.each(function () {
            // Add your specific validation logic here. 
            // For simple "not empty" validation:
            if ($(this).val().trim() === '' || $(this).next('.field-validation-error').text() != "") {
                allValid = false;
                return false; // Break the each loop
            }

            // You can add more complex validation (e.g., regex for email)
            // if (this.name === 'email' && !isValidEmail($(this).val())) { ... }
        });

        if (allValid) {
            $('#btnReturnStoreTransferItemsSave').prop('disabled', false);
        }
        else {
            $('#btnReturnStoreTransferItemsSave').prop('disabled', true);
        }

    } catch (e) {
        console.error(e);
    }
}

function deleteStoreTransferReturnEntry(invoiceNo, purchaseId) {
    try {
        showLoader();

        $.ajax({
            type: "POST",
            url: '/Store/DeleteStoreTransferByInvoice',
            data: { invoiceNo: invoiceNo, purchaseId: purchaseId },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == "Success") {
                        SuccessToast("Store transfered item return approved.");
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        ErrorToast("Error occurred while approving store transfered items(s).");
                    }
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function sendStoreReturnItemNotification() {
    try {
        hideBoostrapModal('#storeTransferReturnDeleteModel');
        showLoader();

        var invoiceNo = $("#txtInvoiceNo").val();
        var fromLocationId = $("#hdnFromLocationId").val();
        var toLocationId = $("#hdnCurrentLocationId").val();

        var itemList = [];

        var rows = $('#tblInvoiceDetails').DataTable().rows().nodes();
        // Use jQuery to find all checkboxes within those rows and filter for checked ones
        var checkedCheckboxes = $('input[type="checkbox"]:checked', rows);

        if (checkedCheckboxes.length > 0) {

            rows.each(row => {
                if ($(row).find('input[type="checkbox"]')[0].checked) {
                    var transferPurchaseID = $(row).find('.hidden_TrfrPurchaseID').val()
                    var returnQty = $(row).find('.ReturnQty').val()

                    var model = {
                        fromLocationId: fromLocationId,
                        toLocationId: toLocationId,
                        billNo: invoiceNo,
                        returnQuantity: returnQty,
                        transferPurchaseID: transferPurchaseID
                    };
                    itemList.push(model);
                }
            })
        }

        if (itemList.length > 0) {

            var form = $("#frmStoreTransferReturn");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            $.ajax({
                type: "POST",
                url: '/Store/SendStoreReturnItemNotification',
                data: { __RequestVerificationToken: token, storeReturnItemNotificationList: itemList },
                dataType: "json",
                success: function (response) {
                    if (response != null) {
                        if (response == 1) {
                            SuccessToast("Store transfered item return notification send successfully.");
                            hideBoostrapModal('#tempReturnStoreTransferItemsModel');
                            hideLoader();

                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        }
                        else {
                            hideLoader();
                            ErrorToast("Error occurred while return store transfer items(s).");
                        }
                    }
                },
                error: function (e) {
                    hideLoader();
                    ErrorToast("Something went wrong!");
                }
            });
        }
        else {
            WarningToast("Items not selected!");
        }

    } catch (e) {
        console.error(e);
    }
}

function rejectStoreReturnItems(_id) {
    try {
        showLoader();
        $.ajax({
            type: "POST",
            url: '/Store/RejectStoreReturnItems',
            data: { Id: _id },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == 1) {
                        WarningToast("Store transfered item return rejected.");
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else {
                        hideLoader();
                        ErrorToast("Error occurred while rejecting store transfer items(s).");
                    }
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function LoadTableStoreReturnItemNotification() {
    try {
        $('#tblStoreReturnItemNotification').DataTable({
            scrollX: true,
            scrollY: 406,
            scrollCollapse: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            paging: true,
            order: [], // disables initial sort
            "ajax": {
                "url": "/Store/GetStoreRetunNotificationsList",
                "type": "POST",
                "datatype": "json",
                "data": {}
            },
            "columns": [
                {
                    "title": "Sr No.", render: function (data, type, row) {
                        return "";
                    }
                },
                { "data": "billNo", "title": "Bill No" },
                { "data": "serialNo", "title": "Serial No" },
                { "data": "brandName", "title": "Brand" },
                { "data": "model", "title": "Model" },
                { "data": "specifications", "title": "Specifications" },
                { "data": "returnQuantity", "title": "Return Quantity" },
                { "data": "fromLocationName", "title": "Dispached Store" },
                { "data": "toLocationName", "title": "Returned Store" },
                {
                    "data": null, "title": "Action", render: function (data, type, row) {
                        if (data === null) return "";

                        return `<div class="column-flex">
                        <a href="javascript:void(0)" onclick="deleteStoreTransferReturnEntry('${row.billNo}', ${row.transferPurchaseID})" class="color-success mr-1" data-bs-toggle="tooltip" data-bs-placement="top" title="Approve Return Item(s)"><i class="fas fa-square-check fa-xl"></i></a>
                        <a href="javascript:void(0)" onclick="rejectStoreReturnItems(${row.storeReturnItemNotificationID})" class="color-danger" data-bs-toggle="tooltip" data-bs-placement="top" title="Reject Return Item(s)"><i class="fas fa-square-xmark fa-xl"></i></a>
                        </div>`;
                    }
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                $("td:first", nRow).html(iDisplayIndex + 1);
                return nRow;
            },
            columnDefs: [
                { targets: 0, className: 'text-nowrap text-center' },
                { targets: 1, className: 'text-nowrap' },
                { targets: 2, className: 'text-nowrap' },
                { targets: 3, className: 'text-nowrap' },
                { targets: 4, className: 'text-nowrap' },
                { targets: 5, className: 'text-nowrap' },
                { targets: 6, className: 'text-nowrap' },
                { targets: 7, className: 'text-nowrap' },
                { targets: 8, className: 'text-nowrap' },
                { targets: 9, className: 'text-nowrap text-center min-width-column', "orderable": false }
            ],
            fixedColumns: {
                start: 0,           // Unfress frist
                end: 1              // freeze the last column
            },
            initComplete: function () {
                $('*[title]').tooltip();
            },
            drawCallback: function (settings) {
                // if (settings.aoData.length == 0) {
                //     $('#divStoreReturnItemNotification').hide();
                //     $('#storeReturnNotificationNoDataMessage').show();
                // }
                // else {
                //     $('#divStoreReturnItemNotification').show();
                //     $('#storeReturnNotificationNoDataMessage').hide();
                // }
            }
        });
    } catch (e) {
        console.error(e);
    }
}

function validateTransferPaymentForm() {
    try {

        var errorCount = 0

        if ($('#txtPaymentDate').val() == "") {
            errorCount++;
            $('#txtPaymentDate').parents('.row').find('.field-validation-error').text("Payment Date is required.");
        }
        else {
            $('#txtPaymentDate').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtTransferAmount').val() == "" || $('#txtTransferAmount').val() == "0") {
            errorCount++;
            $('#txtTransferAmount').parents('.row').find('.field-validation-error').text("Transfer Amount is required, and should be more than 0.");
        }
        else {
            $('#txtTransferAmount').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtTransferMode').val() == "") {
            errorCount++;
            $('#txtTransferMode').parents('.row').find('.field-validation-error').text("Transfer Mode is required.");
        }
        else {
            $('#txtTransferMode').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlTransferStore').select2('val') == "0") {
            errorCount++;
            $('#ddlTransferStore').parents('.row').find('.field-validation-error').text("Transfer Store is required.");
        }
        else {
            $('#ddlTransferStore').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtRemark').val() == "") {
            errorCount++;
            $('#txtRemark').parents('.row').find('.field-validation-error').text("Remark is required.");
        }
        else {
            $('#txtRemark').parents('.row').find('.field-validation-error').text("");
        }

        if (errorCount > 0) {
            return false;
        }
        else {
            return true;
        }

    } catch (e) {
        console.error(e);
    }
}

function saveTransferPayment() {
    try {

        if (!validateTransferPaymentForm()) {
            return false;
        }
        else {
            showLoader();

            var paymentDate = $('#txtPaymentDate').val();
            var transferAmount = parseFloat($('#txtTransferAmount').val());
            var transferMode = $('#txtTransferMode').val();
            var transferStore = parseInt($('#ddlTransferStore').select2('val'));
            var remark = $('#txtRemark').val();

            var model = {
                paymentDate: paymentDate,
                amount: transferAmount,
                transferMode: transferMode,
                toLocationId: transferStore,
                remark: remark
            }

            var hdnTransferPaymentId = $('#hdnTransferPaymentId').val();

            if (hdnTransferPaymentId == undefined) {

                var form = $("#frmTransferPayment");
                var token = $('input[name="__RequestVerificationToken"]', form).val();

                $.ajax({
                    type: "POST",
                    url: '/Store/SaveTransferPayment',
                    data: { __RequestVerificationToken: token, transferPaymentModel: model },
                    dataType: "json",
                    success: function (response) {
                        if (response == "Success") {
                            SuccessToast("Transfer Payment saved successfully.");
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Store/TransferPayment";
                            }, 2000);
                        }
                        else if (response == "SameLocation") {
                            hideLoader();
                            WarningToast("Can not transfer for same store.");
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while saving transfer tayment record(s).");
                        }
                    },
                    error: function (e) {
                        hideLoader();
                        ErrorToast("Something went wrong!");
                    }
                });
            }
            else {
                model.transferPaymentId = hdnTransferPaymentId;

                var form = $("#frmEditTransferPayment");
                var token = $('input[name="__RequestVerificationToken"]', form).val();

                $.ajax({
                    type: "POST",
                    url: '/Store/UpdateTransferPayment',
                    data: { __RequestVerificationToken: token, transferPaymentModel: model },
                    dataType: "json",
                    success: function (response) {
                        if (response == "Success") {
                            SuccessToast("Transfer Payment updated successfully.");
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Store/TransferPayment";
                            }, 2000);
                        }
                        else if (response == "SameLocation") {
                            hideLoader();
                            WarningToast("Can not transfer for same store.");
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while updating transfer tayment record(s).");
                        }
                    },
                    error: function (e) {
                        hideLoader();
                        ErrorToast("Something went wrong!");
                    }
                });
            }
        }

    } catch (e) {
        console.error(e);
    }
}

function LoadTableTransferPaymentNotification() {
    try {
        $('#tblTransferPaymentNotification').DataTable({
            scrollX: true,
            scrollY: 406,
            scrollCollapse: true,
            responsive: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            paging: true,
            order: [], // disables initial sort
            "ajax": {
                "url": "/Store/GetTransferPaymentNotificationsList",
                "type": "POST",
                "datatype": "json",
                "data": {}
            },
            "columns": [
                {
                    "title": "Sr No.", render: function (data, type, row) {
                        return "";
                    }
                },
                {
                    "data": null, "title": "Particular", render: function (data, type, row) {
                        return `<b>${row.fromLocation}</b> paid &#8377; <b>${row.amount}</b> by <b>${row.transferMode}</b> on <b>${moment(row.paymentDate).format('DD/MM/YYYY')}</b>`
                    }
                },
                {
                    "data": null, "title": "Action", render: function (data, type, row) {
                        if (data === null) return "";
                        return `<div class="column-flex">
                        <a href="javascript:void(0)" onclick="updateApprovalTransferPayment(${row.transferPaymentID},'Approve')" class="color-success mr-1" data-bs-toggle="tooltip" data-bs-placement="top" title="Approve Transfer Payment"><i class="fas fa-square-check fa-xl"></i></a>
                        <a href="javascript:void(0)" onclick="updateApprovalTransferPayment(${row.transferPaymentID},'Reject')" class="color-danger" data-bs-toggle="tooltip" data-bs-placement="top" title="Reject Transfer Payment"><i class="fas fa-square-xmark fa-xl"></i></a>
                        </div>`;
                    }
                }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex) {
                $("td:first", nRow).html(iDisplayIndex + 1);
                return nRow;
            },
            columnDefs: [
                { targets: 0, className: 'text-nowrap text-center' },
                { targets: 1, className: 'text-nowrap' },
                { targets: 2, className: 'text-nowrap text-center', "orderable": false }
            ],
            fixedColumns: {
                start: 0,           // Unfress frist
                end: 1              // freeze the last column
            },
            initComplete: function () {
                $('*[title]').tooltip();
            },
            drawCallback: function (settings) {
                // if (settings.aoData.length == 0) {
                //     $('#divTransferPaymentNotification').hide();
                //     $('#transferPaymentNotificationNoDataMessage').show();
                // }
                // else {
                //     $('#divTransferPaymentNotification').show();
                //     $('#transferPaymentNotificationNoDataMessage').hide();
                // }
            }
        });
    } catch (e) {
        console.error(e);
    }
}

function showDeleteTransferPaymentModel(id) {
    try {

        $('#hdnDeleteTransferPaymentId').val(id);

        showBoostrapModal('#transferPaymentDeleteModel');

    } catch (e) {
        console.error(e);
    }
}

function deleteTransferPayment(id) {
    try {
        hideBoostrapModal('#transferPaymentDeleteModel');
        showLoader();

        $.ajax({
            type: "POST",
            url: '/Store/TransferPaymentDelete',
            data: { Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == "Success") {
                        SuccessToast("Transfer payment entry deleted.");
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        ErrorToast("Error occurred while deleting Transfer payment record(s).");
                    }
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function updateApprovalTransferPayment(_id, _type) {
    try {
        showLoader();
        $.ajax({
            type: "POST",
            url: '/Store/TransferPaymentStausUpdate',
            data: { Id: _id, Type: _type },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == "Success") {
                        if (_type == 'Approve') {
                            SuccessToast("Transfer payment Approved.");
                        }
                        else if (_type == 'Reject') {
                            WarningToast("Transfer payment rejected.");
                        }

                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        if (_type == 'Approve') {
                            ErrorToast("Error occurred while approving Transfer payment.");
                        }
                        else if (_type == 'Reject') {
                            ErrorToast("Error occurred while rejecting Transfer payment.");
                        }
                    }
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function callStoreTransferRawData(locationId) {
    try {


        if ($.fn.DataTable.isDataTable("#tblStoreTransferRawData")) {
            $('#tblStoreTransferRawData').DataTable().destroy();
        }

        //$('#tblRawData thead tr').css("height","40px")
        tblRawData = $('#tblStoreTransferRawData').DataTable({
            scrollX: true,
            scrollY: 406,
            scrollCollapse: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            paging: true,
            order: [], // disables initial sort
            ajax: {
                "url": "/Inventory/GetStoreTransferRawData",
                "data": function (d) {
                    d.searchLocationId = locationId;
                },
                "type": "POST",
                "datatype": "json"
            },
            stateSave: true,
            columns: [
                { "data": null, "title": "#" },
                { "data": "serialNo", "title": "Serial No" },
                {
                    "data": "transferDate", "title": "Date", render: function (data, type, row) {
                        if (data === null) return "";

                        return moment(data).format('DD/MM/YYYY');
                    }
                },
                { "data": "categoryName", "title": "Category" },
                { "data": "brandName", "title": "Brand" },
                { "data": "model", "title": "Model" },
                {
                    "data": "specifications", "title": "Specifications", render: function (data, type, row) {
                        if (data === null) return "";

                        return "<span class='text-ellises' title='" + data + "'>" + data + "</span>";
                    }
                },
                {
                    "data": "sellingPrice", "title": "Cost", render: function (data, type, row) {
                        if (data === null) return "";

                        return "&#8377; " + data;
                    }
                },
                { "data": "sellingQuantity", "title": "Quantity" },
                {
                    "data": "totalPrice", "title": "Total Amount", render: function (data, type, row) {
                        if (data === null) return "";

                        return "&#8377; " + data;
                    }
                }
            ],
            columnDefs: [
                { targets: 0, className: 'text-nowrap  text-center' },
                { targets: 1, className: 'text-nowrap' },
                { targets: 2, className: 'text-nowrap' },
                { targets: 3, className: 'text-nowrap' },
                { targets: 4, className: 'text-nowrap' },
                { targets: 5, className: 'text-nowrap' },
                { targets: 6, className: 'text-nowrap' },
                { targets: 7, className: 'text-nowrap  text-right' },
                { targets: 8, className: 'text-nowrap  text-right' },
                { targets: 9, className: 'text-nowrap  text-right' },

            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $("td:first", nRow).html(iDisplayIndex + 1);
                return nRow;
            }
        });

    } catch (e) {
        console.error(e);
    }
}

function callStoreTransferPaymentData(locationId) {
    try {

        if ($.fn.DataTable.isDataTable("#tblStoreTransferPaymentSummary")) {
            $('#tblStoreTransferPaymentSummary').DataTable().destroy();
        }

        //$('#tblRawData thead tr').css("height","40px")
        tblRawData = $('#tblStoreTransferPaymentSummary').DataTable({
            scrollX: true,
            scrollY: 406,
            scrollCollapse: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            paging: true,
            order: [], // disables initial sort
            ajax: {
                "url": "/Inventory/GetStoreTransferPaymentSummary",
                "data": function (d) {
                    d.searchLocationId = locationId;
                },
                "type": "POST",
                "datatype": "json"
            },
            stateSave: true,
            columns: [
                { "data": null, "title": "#" },
                {
                    "data": "paymentDate", "title": "Date", render: function (data, type, row) {
                        if (data === null) return "";

                        return moment(data).format('DD/MM/YYYY');
                    }
                },
                {
                    "data": "amount", "title": "Amount", render: function (data, type, row) {
                        if (data === null) return "";

                        return "&#8377; " + data;
                    }
                },
                { "data": "transferMode", "title": "Payment Mode" },
                { "data": "remark", "title": "Remark" }
            ],
            columnDefs: [
                { targets: 0, className: 'text-nowrap  text-center' },
                { targets: 1, className: 'text-nowrap' },
                { targets: 2, className: 'text-nowrap text-right' },
                { targets: 3, className: 'text-nowrap' },
                { targets: 4, className: 'text-nowrap' }
            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $("td:first", nRow).html(iDisplayIndex + 1);
                return nRow;
            }
        });


    } catch (e) {
        console.error(e);
    }
}

function getStoreTransferCalculation(locationId) {
    try {
        if (locationId > 0) {
            showLoader();

            $.ajax({
                type: "GET",
                url: '/Inventory/GetStoreTransferCalculation',
                data: { searchLocationId: locationId },
                dataType: "json",
                success: function (response) {
                    if (response != null) {
                        var totalBill = response.filter(element => element.label == "Total Bill")[0].totalPrice;
                        var totalPaid = response.filter(element => element.label == "Total Paid")[0].totalPrice;
                        var totalBalance = response.filter(element => element.label == "Total Balance")[0].totalPrice;
                        $('#lblTotalBill').text(`\u20B9 ${totalBill}`);
                        $('#lblTotalPaid').text(`\u20B9 ${totalPaid}`);
                        $('#lblTotalBalance').text(`\u20B9 ${totalBalance}`);
                    }

                    hideLoader();
                },
                error: function (e) {
                    hideLoader();
                    ErrorToast("Something went wrong!");
                }
            });
        }
        else {
            $('#lblTotalBill').text('\u20B9 0');
            $('#lblTotalPaid').text('\u20B9 0');
            $('#lblTotalBalance').text('\u20B9 0');
        }

    } catch (e) {
        console.error(e);
    }
}