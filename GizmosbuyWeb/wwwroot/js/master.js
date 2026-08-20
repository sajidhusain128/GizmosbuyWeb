
$(document).ready(function () {

    $('#btnPurchaseUserPassword').click(function (e) {
        try {
            updateUserPassword();
        } catch (e) {
            console.error(e);
        }
    });

    $("#btnExportUserPassword").click(function () {
        location.href = "/Master/UserPasswordExportExcel?Search=" + tableUserPassword.search();
    })

});
function maskPassword(password) {
    if (password && typeof password === 'string') {
        // Create a string of asterisks with the same length as the password
        return '*'.repeat(password.length);
    }
    return '';
}

function togglePasswordView(_this) {
    try {
        const cell = $(_this).parents("td");
        const span = _this.closest("tr").querySelector(".password-span");
        const isHidden = span.textContent.includes("*");


        if (isHidden) {
            $(_this).children(".fa-eye").removeClass("fa-eye").addClass("fa-eye-slash");
            $(_this).tooltip('dispose');
            $(_this).attr("title", "Hide Password");
            $(_this).tooltip('update');
            span.textContent = cell[0].dataset.password;
        }
        else {
            $(_this).children(".fa-eye-slash").removeClass("fa-eye-slash").addClass("fa-eye");
            $(_this).tooltip('dispose');
            $(_this).attr("title", "Show Password");
            $(_this).tooltip('update');
            span.textContent = maskPassword(cell[0].dataset.password);
        }
    } catch (e) {
        console.error(e);
    }
}

function validateChangePasswordForm() {
    try {

        var errorCount = 0

        

        if ($('#txtPassword').val() !== "" && $('#txtPassword').val() !== $('#hdnCurrentPassword').val()) {
            errorCount++;
            $('#txtPassword').parents('.row').find('.field-validation-error').text("Your current password is incorrect.");
        }
        else if ($('#txtPassword').val() === "") {
            errorCount++;
            $('#txtPassword').parents('.row').find('.field-validation-error').text("Current password is required.");
        }
        else {
            $('#txtPassword').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtNewPassword').val() === "") {
            errorCount++;
            $('#txtNewPassword').parents('.row').find('.field-validation-error').text("New password is required.");
        }
        else {
            $('#txtNewPassword').parents('.row').find('.field-validation-error').text("");
        }

        if ($('#txtConfirmPassword').val() === "") {
            errorCount++;
            $('#txtConfirmPassword').parents('.row').find('.field-validation-error').text("Confirm password is required.");
        }
        else {
            $('#txtConfirmPassword').parents('.row').find('.field-validation-error').text("");
        }

        if (($('#txtNewPassword').val() !== "" && $('#txtConfirmPassword').val() !== "") && $('#txtPassword').val() === $('#txtNewPassword').val()) {
            errorCount++;
            $('#spnUpdtPwdCommonError').text("Current and New password should not be same.");
        }
        else {
            $('#spnUpdtPwdCommonError').text("");
        }

        if ($('#txtNewPassword').val() === $('#txtUserName').val()) {
            errorCount++;
            var msg = "New password should not be same as Username."
            var errMsg = $('#spnUpdtPwdCommonError').text();
            errMsg = errMsg != "" ? errMsg + "<br>" + msg : msg;
            $('#spnUpdtPwdCommonError').html(errMsg);
        }
        //else {
        //    $('#txtPassword').parents('.row').find('.field-validation-error').text("");
        //}

        if ($('#txtNewPassword').val() !== "" && $('#txtNewPassword').val().length < 6) {
            errorCount++;
            var msg = "New password length should not be minimum 6 character."
            var errMsg = $('#spnUpdtPwdCommonError').text();
            errMsg = errMsg != "" ? errMsg + "<br>" + msg : msg;
            $('#spnUpdtPwdCommonError').html(errMsg);
        }
        else if (errorCount == 0) {
            $('#spnUpdtPwdCommonError').text("");
        }

        if (($('#txtNewPassword').val() !== "" && $('#txtConfirmPassword').val() !== "") && $('#txtNewPassword').val() !== $('#txtConfirmPassword').val()) {
            errorCount++;
            var msg = "New and Confirm password should be matched."
            var errMsg = $('#spnUpdtPwdCommonError').text();
            errMsg = errMsg != "" ? errMsg + "<br>" + msg : msg;
            $('#spnUpdtPwdCommonError').html(errMsg);
        }
        else if (errorCount == 0) {
            $('#spnUpdtPwdCommonError').text("");
        }

        if (errorCount > 0) {
            // Get the HTML content (with <br>)
            var content = $("#spnUpdtPwdCommonError").html();

            // Split by <br>
            var lines = content.split("<br>");

            $("#spnUpdtPwdCommonError").html("")
            // Append each line as <li>
            $("#spnUpdtPwdCommonError").append("<ul></ul>");
            $("#spnUpdtPwdCommonError").find("ul").css({
                "margin": "0",
                "padding-left": "20px"
            });
            $.each(lines, function (index, line) {
                if ($.trim(line).length > 0) { // avoid empty lines
                    $("#spnUpdtPwdCommonError").find("ul").append("<li>" + line.trim() + "</li>");
                }
            });

            return false;
        }
        else {
            $("#spnUpdtPwdCommonError").html("");
            return true;
        }

    } catch (e) {
        console.error(e);
    }
}

function clearUpdatePasswordForm() {
    try {
        $('#txtPassword').val("");
        $('#txtNewPassword').val("");
        $('#txtConfirmPassword').val("");
    } catch (e) {
        console.error(e);
    }
}

function updateUserPassword() {
    try {
        if (!validateChangePasswordForm()) {
            return false;
        }
        else {
            showLoader();

            var hdnUserId = parseInt($('#hdnUserId').val());
            var txtUserName = $('#txtUserName').val();
            var txtPassword = $('#txtPassword').val();
            var txtNewPassword = $('#txtNewPassword').val();
            var txtConfirmPassword = $('#txtConfirmPassword').val();


            var model = {
                userId: hdnUserId,
                userName: txtUserName,
                password: txtPassword,
                newPassword: txtNewPassword,
                confirmPassword: txtConfirmPassword
            }

            var form = $("#frmChangePassword");
            var token = $('input[name="__RequestVerificationToken"]', form).val();

            $.ajax({
                type: "POST",
                url: '/Master/UpdateUserPassword',
                data: { __RequestVerificationToken: token, userModel: model },
                dataType: "json",
                //async: true,
                success: function (response) {
                    if (response == "Success") {
                        SuccessToast("Password updates successfully.");
                        clearUpdatePasswordForm();
                        hideLoader();

                        setTimeout(function () {
                            location.href = "/Master/UsersPassword";
                        }, 2000);
                    }
                    else if (response == "Failed") {
                        hideLoader();
                        ErrorToast("Error occurred while updating user password.");
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