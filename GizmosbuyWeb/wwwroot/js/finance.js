
$(document).ready(function () {

    $('#btnExpenseSave,#btnExpenseUpdate').click(function (e) {
        try {
            e.stopPropagation();
            saveExpense();
        } catch (e) {
            console.error(e);
        }
    });

    $("#btnDeleteExpense").click(function () {
        try {
            var id = $('#hdnTempExpenseDeleteId').val();
            DeleteExpense(id)
        } catch (e) {
            console.log(e);
        }
    })

    $('#btnSearchExpenseSummary').click(function (e) {
        try {
            e.stopPropagation();

            var ddlLocationValue = $('#ddlExpenseLoaction').select2('val');
            var year = $('#txtExpenseYear').val();
            var month = $('#ddlExpenseMonth').select2('val');

            getExpenseSummaryDate(ddlLocationValue, month, year);
        } catch (e) {
            console.log(e);
        }
    });

    $("#btnExportExpense").click(function () {
        location.href = "/Finance/ExpenseExportExcel?Search=" + tableExpense.search();
    })

});

function validateExpenseSummary() {
    try {
        var errorCount = 0

        if ($('#txtExpenseYear').val() == "" && $('#ddlExpenseMonth').select2('val') != "0") {
            errorCount++;
            $('#txtExpenseYear').siblings('.field-validation-error').text("If month selected then year should not blank.");
        }
        else {
            $('#txtExpenseYear').siblings('.field-validation-error').text("");
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

function validateExpenseForm() {
    try {

        var errorCount = 0

        if ($('#txtExpenseDate').val() == "") {
            errorCount++;
            $('#txtExpenseDate').parents('.row').find('.field-validation-error').text("Expense Date is required.");
        }
        else {
            $('#txtExpenseDate').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtExpenseAmount').val() == "") {
            errorCount++;
            $('#txtExpenseAmount').parents('.row').find('.field-validation-error').text("Expense Amount is required.");
        }
        else {
            $('#txtExpenseAmount').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlExpenseType').select2('val') == "0") {
            errorCount++;
            $('#ddlExpenseType').parents('.row').find('.field-validation-error').text("Expense Type is required.");
        }
        else {
            $('#ddlExpenseType').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlPaymentMode').select2('val') == "0") {
            errorCount++;
            $('#ddlPaymentMode').parents('.row').find('.field-validation-error').text("Payment Mode is required.");
        }
        else {
            $('#ddlPaymentMode').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtRemark').val() == "") {
            errorCount++;
            $('#txtRemark').parents('.row').find('.field-validation-error').text("Remark is required.");
        }
        else {
            $('#txtRemark').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#ddlExpenseMonth').select2('val') == "") {
            errorCount++;
            $('#ddlExpenseMonth').parents('.row').find('.field-validation-error').text("Expense Month is required.");
        }
        else {
            $('#ddlExpenseMonth').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtExpenseYear').val() == "") {
            errorCount++;
            $('#txtExpenseYear').parents('.row').find('.field-validation-error').text("Expense Year is required.");
        }
        else {
            $('#txtExpenseYear').parents('.row').find('.field-validation-error').text("");
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

function saveExpense() {
    try {

        if (!validateExpenseForm()) {
            return false;
        }
        else {
            showLoader();

            var expenseDate = $('#txtExpenseDate').val();
            var expenseAmount = $('#txtExpenseAmount').val();
            var expenseTypeId = parseInt($('#ddlExpenseType').select2('val'));
            var paymentModeId = parseInt($('#ddlPaymentMode').select2('val'));
            var remark = $('#txtRemark').val();
            var expenseMonth = parseInt($('#ddlExpenseMonth').select2('val'));
            var expenseYear = parseInt($('#txtExpenseYear').val());

            var hdnExpenseId = $('#hdnExpenseId').val();

            var model = {
                expenseDate: expenseDate,
                amount: expenseAmount,
                expenseTypeId: expenseTypeId,
                remark: remark,
                paymentModeId: paymentModeId,
                expenseMonth: expenseMonth,
                expenseYear: expenseYear
            }

            var form = $("#frmCreateExpense");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            if (hdnExpenseId == undefined) {

                $.ajax({
                    type: "POST",
                    url: '/Finance/SaveExpense',
                    data: { __RequestVerificationToken: token, expenseModel: model },
                    dataType: "json",
                    success: function (response) {
                        if (response == "Success") {
                            SuccessToast("Expense saved successfully.");
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Finance/Expense";
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while saving expense record(s).");
                        }
                    },
                    error: function (e) {
                        hideLoader();
                        ErrorToast("Something went wrong!");
                    }
                });
            }
            else if (hdnExpenseId != undefined && hdnExpenseId > 0) {

                model.expenseId = hdnExpenseId;

                var form = $("#frmEditExpense");
                var token = $('input[name="__RequestVerificationToken"]', form).val();

                $.ajax({
                    type: "POST",
                    url: '/Finance/UpdateExpense',
                    data: { __RequestVerificationToken: token, expenseModel: model },
                    dataType: "json",
                    success: function (response) {
                        if (response == "Success") {
                            SuccessToast("Expense updated successfully.");
                            //clearPurchaseForm();
                            hideLoader();

                            setTimeout(function () {
                                location.href = "/Finance/Expense";
                            }, 2000);
                        }
                        else if (response == "Failed") {
                            hideLoader();
                            ErrorToast("Error occurred while updating expense record(s).");
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

function showDeleteExpenseModel(id) {
    try {

        $('#hdnTempExpenseDeleteId').val(id);

        var modal = new bootstrap.Modal('#expenseDeleteModel', {
            backdrop: 'static',
            keyboard: false
        })
        modal.show();
    } catch (e) {
        console.log(e);
    }
}

function DeleteExpense(id) {
    try {
        $('#expenseDeleteModel').modal("hide");
        showLoader();

        $.ajax({
            type: "POST",
            url: '/Finance/DeleteExpense',
            data: { Id: id },
            dataType: "json",
            success: function (response) {
                if (response != null) {
                    if (response == "Success") {
                        SuccessToast("Expense entry deleted.");
                        hideLoader();

                        setTimeout(function () {
                            location.reload();
                        }, 2000);
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

function getExpenseSummaryDate(locationId, month, year) {
    try {
        if (!validateExpenseSummary()) {
            return false;
        }
        else {
            showLoader();

            var form = $("#frmSummery");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            var model = {
                locationId: locationId,
                sellMonth: month,
                sellYear: year
            }

            $.ajax({
                type: "POST",
                url: '/Inventory/GetExpenseSummayData',
                data: {
                    __RequestVerificationToken: token,
                    summaryModel: model
                },
                //dataType: "html",
                success: function (response) {
                    if (response != null) {
                        $('#divExpenseSummaryData').html(response);
                    }
                    hideLoader();
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