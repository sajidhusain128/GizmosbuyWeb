//const { Toast } = require("bootstrap");
$(document).ready(function() {

    $(document).on("click", "#btnPurchaseSave", function(e) {
        e.stopPropagation();
        validatePuchaseForm();
    });

    //$("#frmPurchase").submit(function (e) {
    //    return true;
    //});
});
