//const { Toast } = require("bootstrap");
var table;

$(document).ready(function () {

    

    $('#btnPurchaseSave,#btnPurchaseUpdate').click(function (e) {
        e.stopPropagation();
        savePurchase();
    });

    //$("#frmPurchase").submit(function (e) {
    //    return true;
    //});

    $('#txtPurcRepair').focus(function () {
        var purcPrice = parseFloat($('#txtPurchasePrice').val());
        var qty = parseInt($('#txtQuantity').val());
        var upgradePrice = parseFloat($('#txtUpgrade').val());

        var total = "";

        if (!isNaN(purcPrice) && !isNaN(qty) && !isNaN(upgradePrice)) {
            total = (purcPrice * qty) + upgradePrice;
        }
        else if (!isNaN(purcPrice) && !isNaN(qty) && isNaN(upgradePrice)) {
            total = (purcPrice * qty);
        }

        $(this).val(total);
    })

    $('#btnSalesSave,#btnSalesUpdate').click(function (e) {
        e.stopPropagation();

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() > 0) {
            saveSales();
        }
        else {
            WarningToast("Purchase quatity not available.");
        }
    });

    $('#btnSearchRawData').click(function (e) {
        e.stopPropagation();

        if ($('#txtStartDate').val() != "" || $('#txtEndDate').val() != "") {

            if (new Date($('#txtEndDate').val()) < new Date($('#txtStartDate').val())) {
                WarningToast("Start date should not be less than end date.");
            }
            else {
                callRawData();
            }
        }
        else {
            WarningToast("Please select date range to search data..");
        }
    });
});

function validatePuchaseForm() {
    try {

        var errorCount = 0

        if ($('#txtSrNo').val() == "") {
            errorCount++;
            $('#txtSrNo').parents('.row').find('.field-validation-error').text("Serial No. is required.");
        }
        else {
            $('#txtSrNo').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtPurchaseDate').val() == "") {
            errorCount++;
            $('#txtPurchaseDate').parents('.row').find('.field-validation-error').text("Purchase Date is required.");
        }
        else {
            $('#txtPurchaseDate').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlCategory').select2('val') == "0") {
            errorCount++;
            $('#ddlCategory').parents('.row').find('.field-validation-error').text("Category is required.");
        }
        else {
            $('#ddlCategory').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlBrand').select2('val') == "0") {
            errorCount++;
            $('#ddlBrand').parents('.row').find('.field-validation-error').text("Brand is required.");
        }
        else {
            $('#ddlBrand').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtModel').val() == "") {
            errorCount++;
            $('#txtModel').parents('.row').find('.field-validation-error').text("Model is required.");
        }
        else {
            $('#txtModel').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSpecs').val() == "") {
            errorCount++;
            $('#txtSpecs').parents('.row').find('.field-validation-error').text("Specifications is required.");
        }
        else {
            $('#txtSpecs').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtPurchasePrice').val() == "" || $('#txtPurchasePrice').val() == "0") {
            errorCount++;
            $('#txtPurchasePrice').parents('.row').find('.field-validation-error').text("Purchase Price is required, and should be greater than 0.");
        }
        else {
            $('#txtPurchasePrice').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() == "0") {
            errorCount++;
            $('#txtQuantity').parents('.row').find('.field-validation-error').text("Quantity is required, and should be greater than 0.");
        }
        else {
            $('#txtQuantity').parents('.row').find('.field-validation-error').text("");
        }

        //if ($('#txtUpgrade').val() == "" || $('#txtUpgrade').val() == "0") {
        //    errorCount++;
        //    $('#txtUpgrade').parents('.row').find('.field-validation-error').text("Repair or Upgrade Price is required, and should be greater than 0.");
        //}
        //else {
        //    $('#txtUpgrade').parents('.row').find('.field-validation-error').text("");
        //}

        if ($('#txtPurcRepair').val() == "" || $('#txtPurcRepair').val() == "0") {
            errorCount++;
            $('#txtPurcRepair').parents('.row').find('.field-validation-error').text("Purhase & Repair Price is required, and should be greater than 0.");
        }
        else {
            $('#txtPurcRepair').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlPayMode').select2('val') == "0") {
            errorCount++;
            $('#ddlPayMode').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#ddlPayMode').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtBuyLead').val() == "") {
            errorCount++;
            $('#txtBuyLead').parents('.row').find('.field-validation-error').text("Buying Lead is required.");
        }
        else {
            $('#txtBuyLead').parents('.row').find('.field-validation-error').text("");
        }

        if (errorCount > 0) {
            return false;
        }
        else {
            return true;
        }

    } catch (e) {
        console.log(e);
    }
}

function savePurchase() {
    try {

        if (!validatePuchaseForm()) {
            return false;
        }
        else {

            var serialNo = $('#txtSrNo').val();
            var purchaseDate = $('#txtPurchaseDate').val();
            var categoryId = parseInt($('#ddlCategory').select2('val'));
            var brandId = parseInt($('#ddlBrand').select2('val'));
            var modelNo = $('#txtModel').val();
            var specs = $('#txtSpecs').val();
            var purchasePrice = parseFloat($('#txtPurchasePrice').val());
            var quantity = parseInt($('#txtQuantity').val());
            var upgradePrice = parseFloat($('#txtUpgrade').val());
            var purcRepairPrice = parseFloat($('#txtPurcRepair').val());
            var paymentModeId = parseInt($('#ddlPayMode').select2('val'));
            var buyingLead = $('#txtBuyLead').val();

            var hdnPurchaseId = $('#hdnPurchaseId').val();

            var model = {
                serialNo: serialNo,
                purchaseDate: purchaseDate,
                categoryId: categoryId,
                brandId: brandId,
                model: modelNo,
                specifications: specs,
                purchasePrice: purchasePrice,
                quantity: quantity,
                upgradePrice: upgradePrice,
                totalPrice: purcRepairPrice,
                paymentModeId: paymentModeId,
                buyingLead: buyingLead
            }

            var form = $("#frmPurchase");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            if (hdnPurchaseId == undefined) {

                $.ajax({
                    type: "POST",
                    url: '/Purchase/SavePurchase',
                    data: { __RequestVerificationToken: token, purchaseModel: model },
                    dataType: "json",
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Purchase created successfully.");
                            clearPurchaseForm();
                        }
                        else if (response == "Failed") {
                            ErrorToast("Error in saving purchase.");
                        }
                    },
                    error: function (e) {
                        ErrorToast("Something wen wrong!");
                    }
                });
            }
            else if (hdnPurchaseId != undefined && hdnPurchaseId > 0) {

                model.purchaseId = hdnPurchaseId;
                model.quantity = null;

                $.ajax({
                    type: "POST",
                    url: '/Purchase/UpdatePurchase',
                    data: { __RequestVerificationToken: token, purchaseModel: model },
                    dataType: "json",
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Purchase updated successfully.");
                            setTimeout(function () {
                                location.href = "/Purchase/Index";
                            }, 2000);

                        }
                        else if (response == "Failed") {
                            ErrorToast("Error in saving purchase.");
                        }
                    },
                    error: function (e) {
                        ErrorToast("Something wen wrong!");
                    }
                });
            }

            //$("#frmPurchase").submit();

            //return true;
        }

    } catch (e) {
        console.log(e);
    }
}

function clearPurchaseForm() {
    try {
        $('#txtSrNo').val("");
        $('#txtPurchaseDate').val("");
        $('#ddlCategory').select2('val',"0");
        $('#ddlBrand').select2('val',"0");
        $('#txtModel').val("");
        $('#txtSpecs').val("");
        $('#txtPurchasePrice').val("");
        $('#txtQuantity').val("");
        $('#txtUpgrade').val("");
        $('#txtPurcRepair').val("");
        $('#ddlPayMode').select2('val',"0");
        $('#txtBuyLead').val("");
    } catch (e) {

    }
}

function getPurchaseRecordInSales(purchaseId) {
    try {

        var form = $("#frmSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "GET",
            url: '/Purchase/GetPurchaseById',
            data: { __RequestVerificationToken: token, purchaseId: purchaseId },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    $('#txtSrNo').val(response.serialNo);
                    //$('#txtPurchaseDate').val(response.purchaseDate);
                    $('#txtCategory').val(response.categoryName);
                    $('#txtBrand').val(response.brandName);
                    $('#txtModel').val(response.model);
                    $('#txtSpecs').val(response.specifications);
                    //$('#txtPurchasePrice').val(response.purchasePrice);
                    $('#txtQuantity').val(response.quantity);
                    //$('#txtUpgrade').val(response.upgradePrice);
                    //$('#txtPurcRepair').val(response.totalPrice);
                    //$('#ddlPayMode').select2('val', response.paymentModeId);
                    //$('#txtBuyLead').val(response.buyingLead);
                    //$('#hdnPurchaseId').val(response.purchaseId);
                }
            },
            error: function (e) {
                ErrorToast("Something wen wrong!");
            }
        });
    } catch (e) {
        console.log(e);
    }
}

function validateSalesForm() {
    try {

        var errorCount = 0

        if ($('#txtSrNo').val() == "") {
            errorCount++;
            $('#txtSrNo').parents('.row').find('.field-validation-error').text("Serial No. is required.");
        }
        else {
            $('#txtSrNo').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellDate').val() == "") {
            errorCount++;
            $('#txtSellDate').parents('.row').find('.field-validation-error').text("Sell Date is required.");
        }
        else {
            $('#txtSellDate').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellingPrice').val() == "" || $('#txtSellingPrice').val() == "0") {
            errorCount++;
            $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("Selling Price is required, and should be greater than 0.");
        }
        else {
            $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellQuantity').val() == "" || $('#txtSellQuantity').val() == "0") {
            errorCount++;
            $('#txtSellQuantity').parents('.row').find('.field-validation-error').text("Selling Quantity is required, and should be greater than 0.");
        }
        else {
            if ($('#txtQuantity').val() != "" && parseInt($('#txtSellQuantity').val()) > parseInt($('#txtQuantity').val())) {
                errorCount++;
                $('#txtSellQuantity').parents('.row').find('.field-validation-error').text("Selling Quantity should not be greater than Purchase Quantity.");
            }
            else {
                $('#txtSellQuantity').parents('.row').find('.field-validation-error').text("");
            }
        }


        if ($('#ddlPayMode').select2('val') == "0") {
            errorCount++;
            $('#ddlPayMode').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#ddlPayMode').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellLead').val() == "") {
            errorCount++;
            $('#txtSellLead').parents('.row').find('.field-validation-error').text("Selling Lead is required.");
        }
        else {
            $('#txtSellLead').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtCustomerName').val() == "") {
            errorCount++;
            $('#txtCustomerName').parents('.row').find('.field-validation-error').text("Customer Name is required.");
        }
        else {
            $('#txtCustomerName').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtContactNo').val() == "") {
            errorCount++;
            $('#txtContactNo').parents('.row').find('.field-validation-error').text("Contact No is required.");
        }
        else {
            $('#txtContactNo').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlLoaction').select2('val') == "0") {
            errorCount++;
            $('#ddlLoaction').parents('.row').find('.field-validation-error').text("Loaction is required.");
        }
        else {
            $('#ddlLoaction').parents('.row').find('.field-validation-error').text("");
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
        console.log(e);
    }
}

function saveSales() {
    try {

        if (!validateSalesForm()) {
            return false;
        }
        else {
            var hdnPurchaseId = $('#hdnPurchaseId').val();
            var serialNo = $('#txtSrNo').val();
            var sellingDate = $('#txtSellDate').val();
            var sellingPrice = parseFloat($('#txtSellingPrice').val());
            var sellingQuantity = parseInt($('#txtSellQuantity').val());
            var paymentModeId = parseInt($('#ddlPayMode').select2('val'));
            var sellingLead = $('#txtSellLead').val();
            var customerName = $('#txtCustomerName').val();
            var contactNo = $('#txtContactNo').val();
            var locationId = parseInt($('#ddlLoaction').select2('val'));
            var billNo = $('#txtBillNo').val();

            var hdnSalesId = $('#hdnSalesId').val();

            var model = {
                purchaseId: hdnPurchaseId,
                serialNo: serialNo,
                sellingDate: sellingDate,
                sellingPrice: sellingPrice,
                sellingQuantity: sellingQuantity,
                paymentModeId: paymentModeId,
                sellingLead: sellingLead,
                customerName: customerName,
                contactNo: contactNo,
                locationId: locationId,
                billNo: billNo
            }

            var form = $("#frmSales");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            if (hdnSalesId == undefined) {

                $.ajax({
                    type: "POST",
                    url: '/Sales/SaveSales',
                    data: { __RequestVerificationToken: token, salesModel: model },
                    dataType: "json",
                    //async: true,
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Sales created successfully.");
                            clearPurchaseForm();
                        }
                        else if (response == "Failed") {
                            ErrorToast("Error in saving sales.");
                        }
                    },
                    error: function (e) {
                        ErrorToast("Something wen wrong!");
                    }
                });
            }
            else if (hdnSalesId != undefined && hdnSalesId > 0) {

                model.salesId = hdnSalesId;
                model.purchaseId = null;
                model.sellingQuantity = null;

                $.ajax({
                    type: "POST",
                    url: '/Sales/UpdateSales',
                    data: { __RequestVerificationToken: token, salesModel: model },
                    dataType: "json",
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Sales updated successfully.");
                            setTimeout(function () {
                                location.href = "/Sales/Index";
                            }, 2000);

                        }
                        else if (response == "Failed") {
                            ErrorToast("Error in saving sales.");
                        }
                    },
                    error: function (e) {
                        ErrorToast("Something wen wrong!");
                    }
                });
            }
        }

    } catch (e) {
        console.log(e);
    }
}

function callRawData() {
    try {

        let startdate = $('#txtStartDate').val();
        let enddate = $('#txtEndDate').val();

        var dateRange = {
            startDate: startdate,
            endDate: enddate
        };

        if ($.fn.DataTable.isDataTable("#tblRawData")) {
            $('#tblRawData').DataTable().destroy();
        }

        table = $('#tblRawData').DataTable({
            scrollX: true,
            scrollY: 360,
            scrollCollapse: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            "paging": true,
            "ajax": {
                "url": "/Inventory/GetRawData",
                "data": {
                    dateRange: dateRange
                },
                "type": "POST",
                "datatype": "json"
            },
            "columns": [
                { "data": "serialNo", "title": "Serial No" },
                {
                    "data": "purchaseDate", "title": "Purchase Date", render: function (data, type, row) {
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
                    "data": "purchasePrice", "title": "Purchase Price", render: function (data, type, row) {
                        if (data === null) return "";

                        return "&#8377; " + data;
                    }
                },
                { "data": "purchaseQuantity", "title": "Purchase Quantity" },
                {
                    "data": "upgradePrice", "title": "Upgrade Price", render: function (data, type, row) {
                        if (data === null) return "";

                        return "&#8377; " + data;
                    }
                },
                {
                    "data": "totalPrice", "title": "Total Price", render: function (data, type, row) {
                        if (data === null) return "";

                        return "&#8377; " + data;
                    }
                },
                { "data": "purchasePaymentMode", "title": "Purchase Payment Mode" },
                { "data": "buyingLead", "title": "Buying Lead" },
                { "data": "stockStatus", "title": "Stock Status" },
                {
                    "data": "sellingDate", "title": "Selling Date", render: function (data, type, row) {
                        if (data === null) return "-";

                        return moment(data).format('DD/MM/YYYY');
                    }
                },
                {
                    "data": "sellingPrice", "title": "Selling Price", render: function (data, type, row) {
                        if (data === null) return "-";

                        return "&#8377; " + data;
                    }
                },
                { "data": "sellingQuantity", "title": "Selling Quantity" },
                {
                    "data": "profit", "title": "Profit", render: function (data, type, row, a) {
                        if (data === null || data == 0) return "-";

                        return "&#8377; " + data;
                    }
                },
                {
                    "data": "loss", "title": "Loss", render: function (data, type, row) {
                        if (data === null || data == 0) return "-";

                        return "&#8377; " + data;
                    }
                },
                {
                    "data": "remainingQuantity", "title": "Remaining Quantity", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "salesPaymentMode", "title": "Sales Payment Mode", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "sellingLead", "title": "Selling Lead", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "customerName", "title": "Customer Name", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "contactNo", "title": "Contact No", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "location", "title": "Location", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "billNo", "title": "Bill No", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "sellMonth", "title": "Sell Month", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
                {
                    "data": "sellYear", "title": "Sell Year", render: function (data, type, row) {
                        if (data === null) return "-";
                        else return data;
                    }
                },
            ],
            columnDefs: [
                { targets: 0, className: 'text-nowrap bg-purchase' },
                { targets: 1, className: 'text-nowrap bg-purchase' },
                { targets: 2, className: 'text-nowrap bg-purchase' },
                { targets: 3, className: 'text-nowrap bg-purchase' },
                { targets: 4, className: 'text-nowrap bg-purchase' },
                { targets: 5, className: ' bg-purchase'},
                { targets: 6, className: 'text-nowrap bg-purchase text-right' },
                { targets: 7, className: 'text-nowrap bg-purchase text-right' },
                { targets: 8, className: 'text-nowrap bg-purchase text-right' },
                { targets: 9, className: 'text-nowrap bg-purchase' },
                { targets: 10, className: 'text-nowrap bg-purchase' },
                { targets: 11, className: 'text-nowrap bg-purchase' },
                { targets: 12, className: 'text-nowrap' },

                { targets: 13, className: 'text-nowrap' },
                { targets: 14, className: 'text-nowrap text-center' },
                { targets: 15, className: 'text-nowrap' },
                { targets: 16, className: 'text-nowrap text-center' },
                { targets: 17, className: 'text-nowrap text-center' },
                { targets: 18, className: 'text-nowrap' },
                { targets: 19, className: 'text-nowrap' },
                { targets: 20, className: 'text-nowrap' },
                { targets: 21, className: 'text-nowrap' },
                { targets: 22, className: 'text-nowrap' },
                { targets: 23, className: 'text-nowrap' },
                { targets: 24, className: 'text-nowrap' },
                { targets: 25, className: 'text-nowrap' },
                { targets: 26, className: 'text-nowrap' }
            ],
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData["sellingQuantity"] > 0) {
                    for (let i = 13; i <= 27; i++) {
                        $(nRow.children[i]).addClass('bg-sales');
                    }
                }

                if (aData["profit"] > 0) {
                    $(nRow.children[16]).addClass('bg-profit');
                } else if (aData["loss"] > 0) {
                    $(nRow.children[17]).addClass('bg-loss');
                }
            }
        });

    } catch (e) {
        console.log(e);
    }
}