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

    $('#btnSalesSave').click(function (e) {
        e.stopPropagation();

        if ($("#tblSalesTemp").DataTable().data().count() > 0) {
            saveSales();
        }
        else {
            WarningToast("Sale entry not added in temporary list.");
        }
    });

    $('#btnSalesUpdate').click(function (e) {
        e.stopPropagation();

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() > 0) {
            updateSales();
        }
        else {
            WarningToast("Purchase quatity not available.");
        }
    });

    $('#btnAddTempSales').click(function (e) {
        e.stopPropagation();

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() > 0) {
            saveTempSales();
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

    $('#ddlCategory').change(function () {
        try {
            var category = $(this).select2('data')[0];
            if (category.text == "Adjustment") {
                $('#txtSrNo').val("NA");    
                $('#ddlBrand').select2('val', "0");
                $('#ddlBrand').prop("disabled", true);
                $('#txtModel').attr("readonly", "readonly");
                $('#txtModel').val("");
                $('#txtUpgrade').attr("readonly", "readonly");
                $('#txtUpgrade').val("");
                $('#ddlPayMode').select2('val', "0");
                $('#ddlPayMode').prop("disabled", true);
                $('#txtBuyLead').attr("readonly", "readonly");
                $('#txtBuyLead').val("");
            }
            else {
                $('#txtSrNo').val("");   
                $('#ddlBrand').prop("disabled", false);
                $('#txtModel').removeAttr("readonly");
                $('#txtUpgrade').removeAttr("readonly");
                $('#ddlPayMode').prop("disabled", false);
                $('#txtBuyLead').removeAttr("readonly", "readonly");
            }

            if (category.text == "Accessories") {
                $('#txtSrNo').val("NA");
            }

        } catch (e) {
            console.log(e);
        }
    });
    //btnSearchSummary

    $('#btnSearchSummary').click(function (e) {
        e.stopPropagation();

        var year = $('#txtSellYear').val();
        var month = $('#txtSellMonth').val();

        var ddlLocationValue = $('#ddlLoaction').select2('val');
        var ddlTypeValue = $('#ddlStatus').select2('val');

        getSalesSummary(ddlTypeValue, ddlLocationValue, month, year);
       
    });
});

function validatePuchaseForm() {
    try {

        var errorCount = 0

        var categoryText = "";

        var category = $('#ddlCategory').select2('data')[0];

        if (category != undefined && category != null && category.text != undefined) {
            categoryText = category.text;
        }

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

        if (categoryText != "Adjustment" && $('#ddlBrand').select2('val') == "0") {
            errorCount++;
            $('#ddlBrand').parents('.row').find('.field-validation-error').text("Brand is required.");
        }
        else {
            $('#ddlBrand').parents('.row').find('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtModel').val() == "") {
            errorCount++;
            $('#txtModel').parents('.row').find('.field-validation-error').text("Model is required.");
        }
        else {
            $('#txtModel').parents('.row').find('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtSpecs').val() == "") {
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

        if (categoryText != "Adjustment" && $('#txtPurcRepair').val() == "" || $('#txtPurcRepair').val() == "0") {
            errorCount++;
            $('#txtPurcRepair').parents('.row').find('.field-validation-error').text("Purhase & Repair Price is required, and should be greater than 0.");
        }
        else {
            $('#txtPurcRepair').parents('.row').find('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#ddlPayMode').select2('val') == "0") {
            errorCount++;
            $('#ddlPayMode').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#ddlPayMode').parents('.row').find('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtBuyLead').val() == "") {
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
                            location.href = "/Purchase/Index";
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
        console.log(e);
    }
}

function clearSalesForm() {
    try {
        $('#txtSrNo').val("");
        $('#txtCategory').val("");
        $('#txtBrand').val("");
        $('#txtModel').val("");
        $('#txtSpecs').val("");
        $('#txtQuantity').val("");
        $('#txtSellDate').val("");
        $('#txtSellingPrice').val("");
        $('#txtSellQuantity').val("");
        $('#ddlPayMode').select2('val', "0");
        $('#txtSellLead').val("");
        $('#txtCustomerName').val("");
        $('#txtContactNo').val("");
        $('#txtLoaction').val("");
    } catch (e) {
        console.log(e);
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

        if ($('#ddlLoaction').val() == "") {
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

        var hdnSalesId = $('#hdnSalesId').val();


        var form = $("#frmSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        if (hdnSalesId == undefined) {

            $.ajax({
                type: "POST",
                url: '/Sales/SaveSales',
                data: { __RequestVerificationToken: token },
                dataType: "json",
                //async: true,
                success: function (response) {

                    if (response == "Success") {
                        SuccessToast("Sales created successfully.");
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
    } catch (e) {
        console.log(e);
    }
}

function updateSales() {
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
            var locationName = $('#txtLoaction').val();
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
                locationName: locationName,
                billNo: billNo
            }

            var form = $("#frmSales");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            if (hdnSalesId != undefined && hdnSalesId > 0) {

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

function saveTempSales() {
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
            var locationName = $('#txtLoaction').val();
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
                location: locationName,
                billNo: billNo
            }

            var form = $("#frmSales");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            if (hdnSalesId == undefined) {

                $.ajax({
                    type: "POST",
                    url: '/Sales/SaveTempSales',
                    data: { __RequestVerificationToken: token, tempSalesModel: model },
                    dataType: "json",
                    //async: true,
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Sales temporory entry added.");
                            clearSalesForm();
                            location.reload();
                        }
                        else if (response == "Failed") {
                            ErrorToast("Error in adding temporory sales.");
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
                    url: '/Sales/UpdateTempSales',
                    data: { __RequestVerificationToken: token, tempSalesModel: model },
                    dataType: "json",
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Sales temporory entry updated.");
                            clearSalesForm();
                            location.reload();

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

function EditTempSales(id) {
    try {

        var form = $("#frmSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "GET",
            url: '/Sales/GetTempSalesEdit',
            data: { __RequestVerificationToken: token, Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {

                    var modal = new bootstrap.Modal('#tempSalesEditModel', {
                        backdrop: 'static',
                        keyboard: false
                    })
                    modal.show();

                    $('#txtSrNoTemp').val(response.serialNo);
                    $('#txtPurchaseDateTemp').val(response.purchaseDate);
                    $('#txtCategoryTemp').val(response.categoryName);
                    $('#txtBrandTemp').val(response.brandName);
                    $('#txtModelTemp').val(response.model);
                    $('#txtSpecsTemp').val(response.specifications);
                    $('#txtQuantityTemp').val(response.quantity);
                    $('#txtSellDateTemp').val(localDateFormat(response.sellingDate,"dd/mm/yyyy"));
                    $('#txtSellingPriceTemp').val(response.sellingPrice);
                    $('#txtSellQuantityTemp').val(response.sellingQuantity);
                    $('#ddlPayModeTemp').select2('val', response.paymentModeId.toString());
                    $('#txtSellLeadTemp').val(response.sellingLead);
                    $('#txtCustomerNameTemp').val(response.customerName);
                    $('#txtContactNoTemp').val(response.contactNo);
                    $('#txtLoactionTemp').val(response.location);
                    $('#txtBillNoTemp').val(response.billNo);
                    $('#hdnPurchaseIdTemp').val(response.purchaseId);
                    getPurchaseRecordInTempSales(response.purchaseId);

                    $('#txtSellDateTemp').datepicker({
                        format: 'dd/mm/yyyy',
                        endDate: '+1d',
                        maxDate: 'today',
                        autoclose: true,
                        todayHighlight: true,
                        todayBtn: true,
                        clearBtn: true
                    });
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

function getPurchaseRecordInTempSales(purchaseId) {
    try {

        var form = $("#frmTempSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "GET",
            url: '/Purchase/GetPurchaseById',
            data: { __RequestVerificationToken: token, purchaseId: purchaseId },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    $('#txtSrNoTemp').val(response.serialNo);
                    $('#txtCategoryTemp').val(response.categoryName);
                    $('#txtBrandTemp').val(response.brandName);
                    $('#txtModelTemp').val(response.model);
                    $('#txtSpecsTemp').val(response.specifications);
                    $('#txtQuantityTemp').val(response.quantity);
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

function DeleteTempSales(id) {
    try {

        var form = $("#frmTempSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Sales/TempSalesDelete',
            data: { __RequestVerificationToken: token, Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {

                    $('#tempSalesEditModel').modal("hide");

                    location.href = "/Sales/Create";
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

        $('#tblRawData thead tr').css("height","40px")
        table = $('#tblRawData').DataTable({
            scrollX: true,
            scrollY: 360,
            scrollCollapse: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            paging: true,
            ajax: {
                "url": "/Inventory/GetRawData",
                "data": function (d) {
                    d.dateRange = dateRange,
                        $('.column-search').each(function (index) {
                        var input = $(this);
                        if (input.length) {
                            d.columns[index].search.value = input.val();
                        }
                    })
                },
                "type": "POST",
                "datatype": "json"
            },
            columns: [
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
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
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

        // Apply the search
      

    } catch (e) {
        console.log(e);
    }
}

function getSalesSummary(transactionType, locationId, month, year) {
    try {

        var form = $("#frmSummery");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Inventory/GetSummayData',
            data: { __RequestVerificationToken: token, TransactionType: transactionType, locationId: locationId, month: getMonthNumber(month), year: year },
            //dataType: "html",
            success: function (response) {
                if (response != null) {
                    $('#divSummaryData').html(response);
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