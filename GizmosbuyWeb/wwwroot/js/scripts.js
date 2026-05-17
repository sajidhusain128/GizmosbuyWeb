
var tblRawData;

function preventBack() {
    window.history.forward();
}
setTimeout("preventBack()", 0);
window.onunload = function () {
    null;
};

$(document).ajaxError(function (xhr, props) {
    if (props.status === 401) {
        window.location.href = '/Auth/Login?timeout=true';
    }
});

$(document).ready(function () {

    $('#btnPurchaseSave,#btnPurchaseUpdate').click(function (e) {
        e.stopPropagation();
        savePurchase();
    });

    $('#txtPurchasePrice,#txtQuantity,#txtUpgrade').blur(function () {
        UpdatePurchasePrice();
    });

    $('#txtPurcRepair').focus(function () {
        UpdatePurchasePrice();
    })

    $('#txtSellingPrice').focus(function () {
        $(this).val(0);
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

        updateSales();
    });

    $('#btnAddTempSales').click(function (e) {
        e.stopPropagation();

        saveTempSales();
    });

    $('#btnUpdateTempSales').click(function (e) {
        e.stopPropagation();

        updateTempSales();
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
                //$('#txtSrNo').val("NA");
                $('#ddlBrand').select2('val', "0");
                $('#ddlBrand').prop("disabled", true);
                $('#txtModel').attr("readonly", "readonly");
                $('#txtModel').val("");
                $('#txtUpgrade').attr("readonly", "readonly");
                $('#txtUpgrade').val("");
                $('#txtPayMode').val("");
                $('#txtPayMode').attr("readonly", "readonly");
                $('#txtBuyLead').attr("readonly", "readonly");
                $('#txtBuyLead').val("");
            }
            else {
                //$('#txtSrNo').val("");   
                $('#ddlBrand').prop("disabled", false);
                $('#txtModel').removeAttr("readonly");
                $('#txtUpgrade').removeAttr("readonly");
                $('#txtPayMode').removeAttr("readonly");
                $('#txtBuyLead').removeAttr("readonly", "readonly");
            }

            //if (category.text == "Accessories") {
            //    $('#txtSrNo').val("NA");
            //}

        } catch (e) {
            console.log(e);
        }
    });

    $('#btnSearchSummary').click(function (e) {
        try {
            e.stopPropagation();

            var year = $('#txtSellYear').val();
            var month = $('#ddlMonths').select2('val');

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

    $("#btnRefundSalesModal").click(function () {
        try {
            $('#btnClearInvoiceDetails').attr("disabled", true);
            $('#btnRefundSalesItemsSave').attr("disabled", true);

            var modal = new bootstrap.Modal('#tempRefundSoldItemsModel', {
                backdrop: 'static',
                keyboard: false
            })
            modal.show();
        } catch (e) {
            console.log(e);
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
                getInvoiceDetailsForSalesRefund();
            }

        } catch (e) {
            console.log(e);
        }
    })

    $("#btnClearInvoiceDetails").click(function () {
        try {
            resetRefundForm();

        } catch (e) {
            console.log(e);
        }
    })

    $("#btnRefundSalesItemsSave").click(function () {
        try {
            if ($('#txtInvoiceNo').val() !== "") {
                if ($('#hdnIvoiceNo').val() === $('#txtInvoiceNo').val()) {

                    $('#spnInvoiceNoError').text("");

                    var modal = new bootstrap.Modal('#salesDeleteRefundModel', {
                        backdrop: 'static',
                        keyboard: false
                    })
                    modal.show();
                }
                else {
                    WarningToast("Fetched details and entered bill no. does not matched. So please enter correct.")
                }
            }
            else {
                $('#spnInvoiceNoError').text("Invoice No. is required.");
            }

        } catch (e) {
            console.log(e);
        }
    })

    $("#btnRefundSalesItemsClose").click(function () {
        try {
            resetRefundForm();
            $('#tempRefundSoldItemsModel').modal("hide");
        } catch (e) {
            console.log(e);
        }
    })

    $("#btnDeleteSalesRefund").click(function () {
        try {
            deleteSalesRefundEntry();
        } catch (e) {
            console.log(e);
        }
    })

    $('#tempRefundSoldItemsModel').on('hidden.bs.modal', function (e) {
        try {
            resetRefundForm();
        } catch (e) {
            console.log(e);
        }
    });

    $("#btnDeletePurchase").click(function () {
        try {
            var id = $('#hdnTempPurchaseDeleteId').val();
            DeletePurchase(id)
        } catch (e) {
            console.log(e);
        }
    })

    $('#ddlStatus').change(function () {
        try {
            var status = $(this).select2('data')[0];
            if (status.text == "Pending") {
                $('#divSellYear').hide();
                $('#divSellMonth').hide();
                $('#txtSellYear').val("");
                $('#txtSellMonth').val("");
                $('#divSummaryData').html("");
            }
            else {
                $('#divSellYear').show();
                $('#divSellMonth').show();
                $('#txtSellYear').val(new Date().getFullYear())
                $('#divSummaryData').html("");
            }

        } catch (e) {
            console.log(e);
        }
    });

    $('#switchPurchaseType input[type="radio"]').change(function () {
        try {

            if ($(this).attr("id") == "rdoSingle") {
                checkSerialNoChangeConfirm($(this));
                //purchaseType("Single")
            }
            else if ($(this).attr("id") == "rdoMultiple") {
                checkSerialNoChangeConfirm($(this));
                //purchaseType("Multiple")
            }
        } catch (e) {
            console.log(e);
        }
    });

    $("#divSerialContainer").on('click',"#btnAddSerialNoInput", function (e) {
        try {
            e.preventDefault();
            e.stopPropagation();
            var value = $('.box input[type="text"]').last().val();
            if (value !== "") {
                var length = parseInt($('.box .row-number').last().text());
                length = length + 1;
                $('<div class="box new-text-div"/>')
                    .html($(`<span class="row-number">${(length).toString()}</span>`))
                    .append($('<input type="text" class="form-control" name="txt-serial-no" placeholder="Serial No" autocomplete="off" />').addClass('someclass'))
                    .append($('<button type="button" title="Remove Serial No."/>').addClass('remove btn btn-sm btn-danger').append('<i class="fa-solid fa-trash"></i>'))
                    .insertBefore(this).appendTo('.box-body');

                $('.new-text-div .remove').tooltip();
                updateBulkQuantityValue(length);
            }
            else {
                WarningToast("Please enter serial no.");
            }
        } catch (e) {
            console.log(e);
        }
    });

    $("#divSerialContainer").on("click", "button.remove", function (e) {
        try {
            e.preventDefault();
            var length = $('.box').length;

            if (length > 1) {
                $(this).closest('div.new-text-div').find('.remove').tooltip('dispose');
                $(this).closest('div.new-text-div').remove();
            }
            else {
                $(this).closest('div.new-text-div').find('input[name="txt-serial-no"]').val('');
            }
            $('.box .row-number').each((index, row) => {
                index = index + 1;
                $(row).text(index);
            })
            updateBulkQuantityValue($('.box').length);
        } catch (e) {
            console.log(e);
        }
    });

    $("#divSerialContainer").on("keydown", '.new-text-div input[name="txt-serial-no"]', function (e) {
        // Check if the pressed key is the Enter key (key code 13, or event.key === "Enter" for modern JS)
        if (event.which === 13 || event.keyCode === 13 || event.key === "Enter") {
            // Prevent the default form submission behavior (if inside a form)
            event.preventDefault();

            if ($(this).val() !== "") {
                // Trigger the click event on the target button
                $('#btnAddSerialNoInput').click();
                $(this).closest('.box').next().find('input[name="txt-serial-no"]').focus();
            }
            else {
                WarningToast("Please enter serial no.");
            }
        }
    })

    $("#salesFileShareModel").on("click", 'button[name="ShareSalesFile"]', function (e) {
        try {
            var id = $(this).attr("id");
            var invoiceNo = $('#hdnTempInvoiceNo').val();

            if (invoiceNo !== "") {
                if (id === "btnSalesDownloadFile") {
                    downloadReport(invoiceNo);
                }
                else if (id === "btnSalesSendWhatsApp") {
                    SendWhatsAppReport(invoiceNo);
                }
            }

        } catch (e) {
            console.log(e);
        }
    });

    $("#salesFileShareModel").on("hidden.bs.modal", function () {
        try {
            hideBoostrapModal("#salesFileShareModel");
            location.href = "/Sales/Index";
        } catch (e) {
            console.log(e);
        }
    });
});

const radios = document.querySelectorAll('#switchPurchaseType input[type="radio"]');
let lastSelected = $('#switchPurchaseType input[type="radio"]:checked').val();
let textboxInitial = $("#txtSrNo").val();
let reverting = false;

function checkSerialNoChangeConfirm(_this) {
    try {
        if ($(_this).attr("value") === "Single") {
            let newSelected = $(_this).val();

            if (getBulkPurchaseChanges() > 0) {
                let confirmChange = confirm("Serial Nos. has changes. Do you want to switch purchase type?");
                if (confirmChange) {
                    lastSelected = newSelected; // accept new radio
                    removeAllBulkSerailNo();
                } else {
                    reverting = true;
                    // revert radio back
                }
            } else {
                lastSelected = newSelected; // no textbox changes, just update'
            }
            $("#switchPurchaseType input[value='" + lastSelected + "']").prop("checked", true);
            purchaseType(lastSelected);
        }
        else if ($(_this).attr("value") === "Bulk") {
            let currentTextbox = $("#txtSrNo").val();
            let newSelected = $(_this).val();

            if (currentTextbox !== "") {
                let confirmChange = confirm("Serial No. has changes. Do you want to switch purchase type?");
                if (confirmChange) {
                    lastSelected = newSelected; // accept new radio
                    textboxInitial = currentTextbox; // reset baseline
                    $("#txtSrNo").val("");
                } else {
                    reverting = true;
                    // revert radio back
                }
            } else {
                lastSelected = newSelected; // no textbox changes, just update    
            }
            $("#switchPurchaseType input[value='" + lastSelected + "']").prop("checked", true);
            purchaseType(lastSelected);
        }
    } catch (e) {
        console.log(e);
    }
}

function getBulkPurchaseChanges() {
    try {
        var counter = 0;
        $('#divSerialContainer .box-body').children('.new-text-div').each((index, row) => {
            var tempval = $(row).find('input[name="txt-serial-no"]').val();

            if (tempval != "") {
                counter += 1;
            }
        })
        return counter;
    } catch (e) {
        console.log(e);
    }
}

function removeAllBulkSerailNo() {
    try {
        $('#divSerialContainer .box-body').children('.new-text-div').each((index, row) => {
            if (index != 0) {
                $(row).remove();
            }
            else {
                $(row).find('input[name="txt-serial-no"]').val("");
            }
        })
    } catch (e) {
        console.log(e);
    }
}

function checkAnyBlankBulkPurchaseSerialNo() {
    try {
        var counter = 0;
        $('#divSerialContainer .box-body').children('.new-text-div').each((index, row) => {
            var tempval = $(row).find('input[name="txt-serial-no"]').val();

            if (tempval === "") {
                counter += 1;
            }
        })
        return counter;
    } catch (e) {
        console.log(e);
    }
}

function UpdatePurchasePrice() {
    try {
        var purcPrice = parseFloat($('#txtPurchasePrice').val());
        var qty = parseInt($('#txtQuantity').val());
        var upgradePrice = parseFloat($('#txtUpgrade').val());

        var total = "";

        var hdnPurchaseId = $('#hdnPurchaseId').val();

        if (hdnPurchaseId != "" && hdnPurchaseId > 0) {
            if (!isNaN(purcPrice) && !isNaN(qty) && !isNaN(upgradePrice)) {
                total = (purcPrice * qty) + upgradePrice;
            }
            else if (!isNaN(purcPrice) && !isNaN(qty) && isNaN(upgradePrice)) {
                total = (purcPrice * qty);
            }
        }
        else {
            let purchaseSelected = $('#switchPurchaseType input[type="radio"]:checked').val();

            if (purchaseSelected == "Single") {
                if (!isNaN(purcPrice) && !isNaN(qty) && !isNaN(upgradePrice)) {
                    total = (purcPrice * qty) + upgradePrice;
                }
                else if (!isNaN(purcPrice) && !isNaN(qty) && isNaN(upgradePrice)) {
                    total = (purcPrice * qty);
                }
            }
            else if (purchaseSelected == "Bulk") {
                if (!isNaN(purcPrice) && !isNaN(upgradePrice)) {
                    total = purcPrice + upgradePrice;
                }
                else if (!isNaN(purcPrice) && isNaN(upgradePrice)) {
                    total = purcPrice;
                }
            }
        }

        $('#txtPurcRepair').val(total);
    } catch (e) {
        console.log(e);
    }
}

function resetRefundForm() {
    $('#hdnIvoiceNo').val("");
    $('#txtInvoiceNo').val("");
    $('#spnInvoiceNoError').text("");
    $('#divInvoiceDetails').html("");
    $('#btnFetchInvoiceDetails').attr("disabled", false);
    $('#btnClearInvoiceDetails').attr("disabled", true);
    $('#btnRefundSalesItemsSave').attr("disabled", true);
}

function validatePuchaseForm() {
    try {

        var errorCount = 0

        var categoryText = "";

        var category = $('#ddlCategory').select2('data')[0];

        if (category != undefined && category != null && category.text != undefined) {
            categoryText = category.text;
        }

        var type = $('#switchPurchaseType input[type="radio"]:checked').attr("value");

        if (type === "Single") {
            if ($('#txtSrNo').val() == "") {
                errorCount++;
                $('#txtSrNo').next('.field-validation-error').text("Serial No. is required.");
            }
            else {
                $('#txtSrNo').next('.field-validation-error').text("");
            }
        }
        else if (type === "Bulk") {
            if (checkAnyBlankBulkPurchaseSerialNo() > 0) {
                errorCount++;
                $('#divSerialContainer').next('.field-validation-error').text("All Serial No. is required.");
            }
            else {
                $('#divSerialContainer').next('.field-validation-error').text("");
            }
        }

        if ($('#txtPurchaseDate').val() == "") {
            errorCount++;
            $('#txtPurchaseDate').closest("div").next('.field-validation-error').text("Purchase Date is required.");
        }
        else {
            $('#txtPurchaseDate').closest("div").next('.field-validation-error').text("");
        }

        if ($('#ddlCategory').select2('val') == "0") {
            errorCount++;
            $('#ddlCategory').siblings('.field-validation-error').text("Category is required.");
        }
        else {
            $('#ddlCategory').siblings('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#ddlBrand').select2('val') == "0") {
            errorCount++;
            $('#ddlBrand').siblings('.field-validation-error').text("Brand is required.");
        }
        else {
            $('#ddlBrand').siblings('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtModel').val() == "") {
            errorCount++;
            $('#txtModel').next('.field-validation-error').text("Model is required.");
        }
        else {
            $('#txtModel').next('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtSpecs').val() == "") {
            errorCount++;
            $('#txtSpecs').next('.field-validation-error').text("Specifications is required.");
        }
        else {
            $('#txtSpecs').next('.field-validation-error').text("");
        }

        if (categoryText != "Service" && categoryText != "Adjustment") {
            if ($('#txtPurchasePrice').val() == "" || $('#txtPurchasePrice').val() == "0") {
                errorCount++;
                $('#txtPurchasePrice').closest("div").next('.field-validation-error').text("Purchase Price is required, and should be more than 0.");
            }
            else {
                $('#txtPurchasePrice').closest("div").next('.field-validation-error').text("");
            }
        }
        else {
            $('#txtPurchasePrice').closest("div").next('.field-validation-error').text("");
        }

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() == "0") {
            errorCount++;
            $('#txtQuantity').next('.field-validation-error').text("Quantity is required, and should be more than 0.");
        }
        else {
            $('#txtQuantity').next('.field-validation-error').text("");
        }

        if (categoryText != "Service" && categoryText != "Adjustment" && ($('#txtPurcRepair').val() == "" || $('#txtPurcRepair').val() == "0")) {
            errorCount++;
            $('#txtPurcRepair').closest("div").next('.field-validation-error').text("Purhase & Repair Price is required, and should be more than 0.");
        }
        else {
            $('#txtPurcRepair').closest("div").next('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtPayMode').val() == "") {
            errorCount++;
            $('#txtPayMode').next('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#txtPayMode').next('.field-validation-error').text("");
        }

        if (categoryText != "Adjustment" && $('#txtBuyLead').val() == "") {
            errorCount++;
            $('#txtBuyLead').next('.field-validation-error').text("Buying Lead is required.");
        }
        else {
            $('#txtBuyLead').next('.field-validation-error').text("");
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
            showLoader();

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
            var paymentMode = $('#txtPayMode').val();
            var buyingLead = $('#txtBuyLead').val();

            var hdnPurchaseId = $('#hdnPurchaseId').val();
            var PurchaseType = $('#switchPurchaseType input[type="radio"]:checked').val();
            var serialNoList = [];

            if (PurchaseType == "Bulk") {
                $('#divSerialContainer .box-body').children('.new-text-div').each((index, row) => {
                    var tempval = $(row).find('input[name="txt-serial-no"]').val();
                    if (tempval != "") {
                        serialNoList.push(tempval);
                    }
                })
            }

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
                paymentModeName: paymentMode,
                buyingLead: buyingLead,
                purchaseType: PurchaseType
            }

            var form = $("#frmPurchase");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            if (hdnPurchaseId == undefined) {

                model.serialNos = serialNoList;

                $.ajax({
                    type: "POST",
                    url: '/Purchase/SavePurchase',
                    data: { __RequestVerificationToken: token, purchaseModel: model },
                    dataType: "json",
                    success: function (response) {
                        if (response == "Success") {
                            SuccessToast("Purchase saved successfully.");
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Purchase/Index";
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while saving purchase record(s).");
                        }
                    },
                    error: function (e) {
                        hideLoader();
                        ErrorToast("Something went wrong!");
                    }
                });
            }
            else if (hdnPurchaseId != undefined && hdnPurchaseId > 0) {

                model.purchaseId = hdnPurchaseId;
                //model.quantity = null;

                $.ajax({
                    type: "POST",
                    url: '/Purchase/UpdatePurchase',
                    data: { __RequestVerificationToken: token, purchaseModel: model },
                    dataType: "json",
                    success: function (response) {
                        if (response == "Success") {
                            SuccessToast("Purchase updated successfully.");
                            clearPurchaseForm();
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Purchase/Index";
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while updating purchase record(s).");
                        }
                    },
                    error: function (e) {
                        hideLoader();
                        ErrorToast("Something went wrong!");
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
        $('#ddlCategory').select2('val', "0");
        $('#ddlBrand').select2('val', "0");
        $('#txtModel').val("");
        $('#txtSpecs').val("");
        $('#txtPurchasePrice').val("");
        $('#txtQuantity').val("");
        $('#txtUpgrade').val("");
        $('#txtPurcRepair').val("");
        $('#txtPayMode').val("");
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
        $('#txtPayMode').val("");
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
        $('#txtPayModeTemp').val("");
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
        showLoader();

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

                    if (response.categoryName != null && categoryList.indexOf(response.categoryName) >= 0) {
                        $('#txtSellingPrice').val(0);
                    }

                    if ($('#tblSalesTemp').DataTable().rows().any()) {
                        var rowData = $('#tblSalesTemp').DataTable().data();
                        if (rowData.length > 0) {
                            $('#txtSellDate').datepicker("setDate", localDateFormat(rowData[0].sellingDate, "dd/mm/yyyy"));
                            $('#txtPayMode').val(rowData[0].paymentMode);
                            $('#txtSellLead').val(rowData[0].sellingLead);
                            $('#txtCustomerName').val(rowData[0].customerName);
                            $('#txtContactNo').val(rowData[0].contactNo);
                            $('#txtLoaction').val(rowData[0].location);
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

        if ($('#txtQuantity').val() == "" || $('#txtQuantity').val() == "0") {
            errorCount++;
            $('#txtQuantity').parents('.row').find('.field-validation-error').text("Purchase quatity not available.");
        }
        else {
            $('#txtQuantity').parents('.row').find('.field-validation-error').text("");
        }

        //if ($('#txtCategory').val() != "Accessories" && $('#txtCategory').val() != "Adjustment") {
        //    if ($('#txtSellingPrice').val() == "" || $('#txtSellingPrice').val() == "0") {
        //        errorCount++;
        //        $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("Selling Price is required, and should be more than 0.");
        //    }
        //    else {
        //        $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("");
        //    }
        //}
        //else if ($('#txtCategory').val() == "Accessories" || $('#txtCategory').val() == "Adjustment") {
        if ($('#txtSellingPrice').val() == "") {
            errorCount++;
            $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("Enter at least 0");
        }
        else {
            $('#txtSellingPrice').parents('.row').find('.field-validation-error').text("");
        }
        //}

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

        if ($('#txtPayMode').val() == "") {
            errorCount++;
            $('#txtPayMode').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#txtPayMode').parents('.row').find('.field-validation-error').text("");
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
        showLoader();

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
                        hideLoader();
                        //downloadReport(response.item2);
                        showSalesFileSharedModel(response.item2)
                    }
                    else if (response.item1 == 0) {
                        hideLoader();
                        ErrorToast("Error occurred while saving sales record(s).");
                    }
                },
                error: function (e) {
                    hideLoader();
                    ErrorToast("Something went wrong!");
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
            showLoader();

            var hdnPurchaseId = $('#hdnPurchaseId').val();
            var serialNo = $('#txtSrNo').val();
            var sellingDate = $('#txtSellDate').val();
            var sellingPrice = parseFloat($('#txtSellingPrice').val());
            var sellingQuantity = parseInt($('#txtSellQuantity').val());
            var paymentMode = $('#txtPayMode').val();
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
                paymentModeName: paymentMode,
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
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Sales/Index";
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while updating sales recor(s).");
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

        if ($('#txtCategoryTemp').val() != "Accessories") {
            if ($('#txtSellingPriceTemp').val() == "" || $('#txtSellingPriceTemp').val() == "0") {
                errorCount++;
                $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("Selling Price is required, and should be more than 0.");
            }
            else {
                $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("");
            }
        }
        else if ($('#txtCategoryTemp').val() == "Accessories") {
            if ($('#txtSellingPriceTemp').val() == "") {
                errorCount++;
                $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("If it's accessories then enter alteast 0.");
            }
            else {
                $('#txtSellingPriceTemp').parents('.row').find('.field-validation-error').text("");
            }
        }

        if ($('#txtSellQuantityTemp').val() == "" || $('#txtSellQuantityTemp').val() == "0") {
            errorCount++;
            $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("Selling Quantity is required, and should be more than 0.");
        }
        else {
            if ($('#txtSellQuantityTemp').val() != "" && parseInt($('#txtSellQuantityTemp').val()) > parseInt($('#txtQuantity').val())) {
                errorCount++;
                $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("Selling Quantity should not be more than Purchase Quantity.");
            }
            else {
                $('#txtSellQuantityTemp').parents('.row').find('.field-validation-error').text("");
            }
        }

        if ($('#txtPayModeTemp').val() == "") {
            errorCount++;
            $('#txtPayModeTemp').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#txtPayModeTemp').parents('.row').find('.field-validation-error').text("");
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
            showLoader();

            var hdnPurchaseId = $('#hdnPurchaseId').val();
            var serialNo = $('#txtSrNo').val();
            var sellingDate = $('#txtSellDate').val();
            var sellingPrice = parseFloat($('#txtSellingPrice').val());
            var purchaseQuantity = parseInt($('#txtQuantity').val());
            var sellingQuantity = parseInt($('#txtSellQuantity').val());
            var paymentMode = $('#txtPayMode').val();
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
                quantity: purchaseQuantity,
                sellingQuantity: sellingQuantity,
                paymentModeName: paymentMode,
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
                            SuccessToast("Sales temporary entry added.");
                            clearSalesForm();
                            hideLoader();

                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        }
                        else if (response == "Exist") {
                            hideLoader();
                            WarningToast("This record already exist in temporary list.")
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while adding temporary sales.");
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
        console.log(e);
    }
}

function updateTempSales() {
    try {
        if (!validateTempSalesForm()) {
            return false;
        }
        else {
            showLoader();
            var hdnTempSalesId = $('#hdnTempSalesId').val();

            if (hdnTempSalesId > 0) {

                var hdnPurchaseId = $('#hdnTempPurchaseId').val();
                var serialNo = $('#txtSrNoTemp').val();
                var sellingDate = $('#txtSellDateTemp').val();
                var sellingPrice = parseFloat($('#txtSellingPriceTemp').val());
                var purchaseQuantity = 0;
                var sellingQuantity = parseInt($('#txtSellQuantityTemp').val());
                var paymentMode = $('#txtPayModeTemp').val();
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
                    quantity: purchaseQuantity,
                    sellingQuantity: sellingQuantity,
                    paymentModeName: paymentMode,
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
                            SuccessToast("Sales temporary entry updated.");
                            clearTempSalesForm();
                            $('#tempSalesEditModel').modal("hide");
                            hideLoader();

                            setTimeout(function () {
                                location.reload();
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while updating in temporary sales record(s).");
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
        console.log(e);
    }
}

function EditTempSales(id) {
    try {
        showLoader();
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
                    $('#txtSellDateTemp').val(localDateFormat(response.sellingDate, "dd/mm/yyyy"));
                    $('#txtSellingPriceTemp').val(response.sellingPrice);
                    $('#txtSellQuantityTemp').val(response.sellingQuantity);
                    $('#txtPayModeTemp').val(response.paymentModeName);
                    $('#txtSellLeadTemp').val(response.sellingLead);
                    $('#txtCustomerNameTemp').val(response.customerName);
                    $('#txtContactNoTemp').val(response.contactNo);
                    $('#txtLoactionTemp').val(response.location);
                    $('#txtBillNoTemp').val(response.billNo);
                    $('#hdnTempPurchaseId').val(response.purchaseId);
                    getPurchaseRecordInTempSales(response.purchaseId);

                    $('#txtSellDateTemp').datepicker({
                        format: 'dd/mm/yyyy',
                        endDate: '+0d',
                        maxDate: 'today',
                        autoclose: true,
                        todayHighlight: true,
                        todayBtn: true,
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
        console.log(e);
    }
}

function getPurchaseRecordInTempSales(purchaseId) {
    try {
        showLoader();

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
                hideLoader();
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
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
        $('#tempSalesDeleteModel').modal("hide");
        showLoader();
        var form = $("#frmTempSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Sales/TempSalesDelete',
            data: { __RequestVerificationToken: token, Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    SuccessToast("Sales temporary entry deleted.");
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
            scrollY: 406,
            scrollCollapse: true,
            fixedColumns: true,
            processing: true,
            serverSide: true,
            pageLength: 10,
            paging: true,
            order: [], // disables initial sort
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
                { targets: 6, className: ' bg-purchase' },
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
        showLoader();

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
                hideLoader();
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });
    } catch (e) {
        console.log(e);
    }
}

function downloadReport(invoiceNo) {
    try {
        showLoader();

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
                hideLoader();
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.log(e);
    }
}

function SendWhatsAppReport(invoiceNo) {
    try {
        showLoader();

        $.ajax({
            type: "POST",
            url: '/Sales/SendWhatsAppSalesReport?invoiceNo=' + invoiceNo,
            //async: true,
            success: function (response) {
                if (response != "" && response.trim().startsWith("Dear Sir")) {
                    SuccessToast("Report send to whatsapp successfully.");
                }
                else {
                    WarningToast(response);
                }
                hideLoader();
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.log(e);
    }
}

function getInvoiceDetailsForSalesRefund() {
    try {
        showLoader();
        var invoiceNo = $("#txtInvoiceNo").val();

        $.ajax({
            type: "GET",
            url: '/Sales/GetInvoiceDetails',
            data: { invoiceNo: invoiceNo },
            //dataType: "html",
            success: function (response) {
                if (response !== null && response.trim() !== '') {
                    $('#divInvoiceDetails').html(response);
                    $('#btnFetchInvoiceDetails').attr("disabled", true);
                    $('#btnClearInvoiceDetails').attr("disabled", false);
                    $('#btnRefundSalesItemsSave').attr("disabled", false);
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

    } catch (e) {
        console.log(e);
    }
}

function deleteSalesRefundEntry() {
    try {
        $('#salesDeleteRefundModel').modal("hide");
        showLoader();
        var invoiceNo = $("#txtInvoiceNo").val();

        var form = $("#frmSalesRefund");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Sales/DeleteSalesByInvoice',
            data: { __RequestVerificationToken: token, invoiceNo: invoiceNo },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == "Success") {
                        SuccessToast("Sales entry deleted successfully.");
                        $('#tempRefundSoldItemsModel').modal("hide");
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else if (response == "Invalid") {
                        hideLoader();
                        WarningToast("Not found or Invalid bill no.");
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        ErrorToast("Error occurred while deleting sales record(s).");
                    }
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.log(e);
    }
}

function showDeletePurchaseModel(id) {
    try {

        $('#hdnTempPurchaseDeleteId').val(id);

        var modal = new bootstrap.Modal('#purchaseDeleteModel', {
            backdrop: 'static',
            keyboard: false
        })
        modal.show();
    } catch (e) {
        console.log(e);
    }
}

function DeletePurchase(id) {
    try {
        $('#purchaseDeleteModel').modal("hide");
        showLoader();
        var form = $("#frmTempSales");
        var token = $('input[name="__RequestVerificationToken"]', form).val();

        $.ajax({
            type: "POST",
            url: '/Purchase/PurchaseDelete',
            data: { __RequestVerificationToken: token, Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == "Success") {
                        SuccessToast("Purchase entry deleted.");
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
                    }
                    else if (response == "Exist") {
                        hideLoader();
                        WarningToast("Alreay sales this item can't delete it.")
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        ErrorToast("Error occurred while deleting purchase record(s).");
                    }
                }
            },
            error: function (e) {
                hideLoader();
                ErrorToast("Something went wrong!");
            }
        });

    } catch (e) {
        console.log(e);
    }
}

function purchaseType(type) {
    try {
        if (type == "Single") {
            $('#divSinglePurchase').show();
            $('#divMultipePurchase').hide();

            updateBulkQuantityValue("")
            $('#txtQuantity').removeAttr("readonly");
        }
        else { // multiple
            $('#divMultipePurchase').show();
            $('#divSinglePurchase').hide();

            updateBulkQuantityValue(1);
            $('#txtQuantity').attr("readonly", true);
        }
    } catch (e) {
        console.log(e);
    }
}

function updateBulkQuantityValue(qty) {
    try {
        $('#txtQuantity').val(qty);
    } catch (e) {
        console.log(e);
    }
}

function showSalesFileSharedModel(invoiceNo) {
    try {
        $('#hdnTempInvoiceNo').val(invoiceNo);
        showBoostrapModal("#salesFileShareModel");
    } catch (e) {
        console.log(e)
    }
}