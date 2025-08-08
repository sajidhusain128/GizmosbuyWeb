
var tblRawData;

$(document).ready(function () {

    $('*[title]').tooltip();

    $('#btnPurchaseSave,#btnPurchaseUpdate').click(function (e) {
        e.stopPropagation();
        savePurchase();
    });

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

        if ($('#txtQuantity').val() != "" || $('#txtQuantity').val() > 0) {
            updateSales();
        }
        else {
            WarningToast("Purchase quatity not available.");
        }
    });

    $('#btnAddTempSales').click(function (e) {
        e.stopPropagation();

        if ($('#txtSellQuantity').val() != "" || $('#txtSellQuantity').val() > 0) {
            saveTempSales();
        }
        else {
            WarningToast("Purchase quatity not available.");
        }
    });

    $('#btnUpdateTempSales').click(function (e) {
        e.stopPropagation();

        if ($('#txtSellQuantityTemp').val() != "" || $('#txtSellQuantityTemp').val() > 0) {
            updateTempSales();
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
                //$('#txtSrNo').val("");   
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

    $('#btnSearchSummary').click(function (e) {
        try {
            e.stopPropagation();

            var year = $('#txtSellYear').val();
            var month = $('#txtSellMonth').val();

            var ddlLocationValue = $('#ddlLoaction').select2('val');
            var ddlTypeValue = $('#ddlStatus').select2('val');

            getSalesSummary(ddlTypeValue, ddlLocationValue, month, year);
        } catch (e) {
            console.log(e);
        }
    });

    $("#btnExportRawData").click(function () {
        location.href = "/Inventory/RawDateExportExcel?FromDate=" + $("#txtStartDate").val() + "&ToDate=" + $("#txtEndDate").val() + "&Search=" + tblRawData.search();
    })

    $("#btnDeleteTempSales").click(function () {
        try {
            var id = $('#hdnTempDeleteId').val();
            DeleteTempSales(id)
        } catch (e) {
            console.log(e);
        }
    })
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
                            SuccessToast("Purchase saved successfully.");
                            clearPurchaseForm();
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
                            clearPurchaseForm();
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

function clearTempSalesForm() {
    try {
        $('#txtSrNoTemp').val("");
        $('#txtCategoryTemp').val("");
        $('#txtBrandTemp').val("");
        $('#txtModelTemp').val("");
        $('#txtSpecsTemp').val("");
        $('#txtQuantityTemp').val("");
        $('#txtSellDateTemp').val("");
        $('#txtSellingPriceTemp').val("");
        $('#txtSellQuantityTemp').val("");
        $('#ddlPayModeTemp').select2('val', "0");
        $('#txtSellLeadTemp').val("");
        $('#txtCustomerNameTemp').val("");
        $('#txtContactNoTemp').val("");
        $('#txtLoactionTemp').val("");
        $('#txtBillNoTemp').val("");
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

                    if (response.item1 > 0) {
                        SuccessToast("Sales created successfully.");
                        downloadReport(response.item2);
                        setTimeout(function () {
                            location.href = "/Sales/Index";
                        }, 2000);
                    }
                    else if (response.item1 == 0) {
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

        if ($('#txtSellDateTemp').val() == "") {
            errorCount++;
            $('#txtSellDateTemp').parents('.row').find('.field-validation-error').text("Sell Date is required.");
        }
        else {
            $('#txtSellDateTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellingPriceTemp').val() == "" || $('#txtSellingPriceTemp').val() == "0") {
            errorCount++;
            $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("Selling Price is required, and should be greater than 0.");
        }
        else {
            $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellQuantityTemp').val() == "" || $('#txtSellQuantityTemp').val() == "0") {
            errorCount++;
            $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("Selling Quantity is required, and should be greater than 0.");
        }
        else {
            if ($('#txtSellQuantityTemp').val() != "" && parseInt($('#txtSellQuantityTemp').val()) > parseInt($('#txtQuantity').val())) {
                errorCount++;
                $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("Selling Quantity should not be greater than Purchase Quantity.");
            }
            else {
                $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("");
            }
        }

        if ($('#ddlPayModeTemp').select2('val') == "0") {
            errorCount++;
            $('#ddlPayModeTemp').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#ddlPayModeTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtSellLeadTemp').val() == "") {
            errorCount++;
            $('#txtSellLeadTemp').parents('.row').find('.field-validation-error').text("Selling Lead is required.");
        }
        else {
            $('#txtSellLeadTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtCustomerNameTemp').val() == "") {
            errorCount++;
            $('#txtCustomerNameTemp').parents('.row').find('.field-validation-error').text("Customer Name is required.");
        }
        else {
            $('#txtCustomerNameTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtContactNoTemp').val() == "") {
            errorCount++;
            $('#txtContactNoTemp').parents('.row').find('.field-validation-error').text("Contact No is required.");
        }
        else {
            $('#txtContactNoTemp').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtLoactionTemp').val() == "") {
            errorCount++;
            $('#txtLoactionTemp').parents('.row').find('.field-validation-error').text("Loaction is required.");
        }
        else {
            $('#txtLoactionTemp').parents('.row').find('.field-validation-error').text("");
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
                            setTimeout(function () {
                                location.reload();
                            }, 2000);
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
        }

    } catch (e) {
        console.log(e);
    }
}

function updateTempSales() {
    try {
        if (!validateTempSalesForm()) {
            return false;
        }
        else {

            var hdnTempSalesId = $('#hdnTempSalesId').val();

            if (hdnTempSalesId > 0) {

                var hdnPurchaseId = $('#hdnTempPurchaseId').val();
                var serialNo = $('#txtSrNoTemp').val();
                var sellingDate = $('#txtSellDateTemp').val();
                var sellingPrice = parseFloat($('#txtSellingPriceTemp').val());
                var sellingQuantity = parseInt($('#txtSellQuantityTemp').val());
                var paymentModeId = parseInt($('#ddlPayModeTemp').select2('val'));
                var sellingLead = $('#txtSellLeadTemp').val();
                var customerName = $('#txtCustomerNameTemp').val();
                var contactNo = $('#txtContactNoTemp').val();
                var locationName = $('#txtLoactionTemp').val();
                var billNo = $('#txtBillNoTemp').val();

                var model = {
                    tempSalesId: hdnTempSalesId,
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

                var form = $("#frmTempSales");
                var token = $('input[name="__RequestVerificationToken"]', form).val();

                $.ajax({
                    type: "PUT",
                    url: '/Sales/UpdateTempSales',
                    data: { __RequestVerificationToken: token, tempSalesModel: model },
                    dataType: "json",
                    success: function (response) {

                        if (response == "Success") {
                            SuccessToast("Sales temporory entry updated.");
                            clearTempSalesForm();

                            $('#tempSalesEditModel').modal("hide");

                            setTimeout(function () {
                                location.reload();
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

                    $('#hdnTempSalesId').val(response.tempSalesId);
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
                    $('#hdnTempPurchaseId').val(response.purchaseId);
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

function showDeleteTempSalesModel(id) {
    try {

        $('#hdnTempDeleteId').val(id);

        var modal = new bootstrap.Modal('#tempSalesDeleteModel', {
            backdrop: 'static',
            keyboard: false
        })
        modal.show();
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
                    SuccessToast("Sales temporory entry deleted.");
                    $('#tempSalesDeleteModel').modal("hide");

                    setTimeout(function () {
                        location.reload();
                    }, 2000);
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

        //$('#tblRawData thead tr').css("height","40px")
        tblRawData = $('#tblRawData').DataTable({
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
            stateSave: true,
            //destroy: true,
            //bSortCellsTop: true,          
            //initComplete: function () {
            //    const api = this.api();

            //    const headerRow = $('<tr>');

            //    api.columns().every(function () {
            //        const th = $('<th>');
            //        const input = $('<input type="text" placeholder="Search" />')
            //        .off()
            //            .on('keyup change', function () {
            //                if (this.value !== this.lastValue) {
            //                    this.lastValue = this.value;
            //                    api.column(this.columnIndex).search(this.value).draw();
            //                }
            //            });
            //        input[0].columnIndex = this.index(); // Store column index
            //        th.append(input);
            //        headerRow.append(th);

            //    });

            //    $('#tblRawData thead').append(headerRow);
            //},
            columns: [
                { "data": "srNo", "title": "#" },
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
                { targets: 0, className: 'text-nowrap bg-purchase text-center' },
                { targets: 1, className: 'text-nowrap bg-purchase' },
                { targets: 2, className: 'text-nowrap bg-purchase' },
                { targets: 3, className: 'text-nowrap bg-purchase' },
                { targets: 4, className: 'text-nowrap bg-purchase' },
                { targets: 5, className: 'text-nowrap bg-purchase' },
                { targets: 6, className: ' bg-purchase'},
                { targets: 7, className: 'text-nowrap bg-purchase text-right' },
                { targets: 8, className: 'text-nowrap bg-purchase text-right' },
                { targets: 9, className: 'text-nowrap bg-purchase text-right' },
                { targets: 10, className: 'text-nowrap bg-purchase text-right' },
                { targets: 11, className: 'text-nowrap bg-purchase' },
                { targets: 12, className: 'text-nowrap bg-purchase' },
                { targets: 13, className: 'text-nowrap' },

                { targets: 14, className: 'text-nowrap' },
                { targets: 15, className: 'text-nowrap text-right' },
                { targets: 16, className: 'text-nowrap' },
                { targets: 17, className: 'text-nowrap text-right' },
                { targets: 18, className: 'text-nowrap text-right' },
                { targets: 19, className: 'text-nowrap' },
                { targets: 20, className: 'text-nowrap' },
                { targets: 21, className: 'text-nowrap' },
                { targets: 22, className: 'text-nowrap' },
                { targets: 23, className: 'text-nowrap' },
                { targets: 24, className: 'text-nowrap' },
                { targets: 25, className: 'text-nowrap' },
                { targets: 26, className: 'text-nowrap' },
                { targets: 27, className: 'text-nowrap' }
            ],
            fnRowCallback: function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData["sellingQuantity"] > 0) {
                    for (let i = 14; i <= 27; i++) {
                        $(nRow.children[i]).addClass('bg-sales');
                    }
                }

                if (aData["profit"] > 0) {
                    $(nRow.children[17]).addClass('bg-profit');
                } else if (aData["loss"] > 0) {
                    $(nRow.children[18]).addClass('bg-loss');
                }
            },
            drawCallback: function (settings) {
                if (settings.aoData.length == 0) {
                    $("#btnExportRawData").addClass("disabled");
                } else {
                    $("#btnExportRawData").removeClass("disabled");
                }
            }
        });
        
        //$('#tblRawData thead tr:first th').each(function (i) {
        //    $('#tblRawData thead tr:last th').eq(i).css('width', $(this).width());
        //});
      
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

function downloadReport(invoiceNo) {
    try {

        $.ajax({
            type: "GET",
            url: '/Sales/DownloadSalesReport?invoiceNo=' + invoiceNo,
            xhrFields: {
                responseType: 'blob' // This tells jQuery to treat the response as a Blob
            },
            //async: true,
            success: function (response) {

                const url = window.URL.createObjectURL(response);
                const a = document.createElement('a');
                a.href = url;
                a.download = 'SalesReport_' + invoiceNo + "_" + formatCustomDate(new Date(), "_");
                document.body.appendChild(a);
                a.click();
                a.remove();
            },
            error: function (e) {
                ErrorToast("Something wen wrong!");
            }
        });

    } catch (e) {
        console.log(e);
    }
}