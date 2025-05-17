$(document).ready(function () {

});

function numberOnly(event) {
    var evt = event || window.event;

    // Handle paste
    if (evt.type === 'paste') {
        key = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        var key = evt.keyCode || evt.which;
        key = String.fromCharCode(key);
    }
    var regex = /[0-9]/;
    if (!regex.test(key)) {
        evt.returnValue = false;
        if (evt.preventDefault)
            evt.preventDefault();
    }
}

function isNumberDecimal(_this, event) {
    var evt = event || window.event;

    var charCode = 0;
    // Handle paste
    if (evt.type === 'paste') {
        charCode = event.clipboardData.getData('text/plain');
    } else {
        // Handle key press
        charCode = (event.which) ? event.which : event.keyCode;
    }

    evt.returnValue = true;
    //var charCode = (event.which) ? event.which : event.keyCode;
    if (charCode == 46) {
        //Check if the text already contains the . character
        if (_this.value.indexOf('.') === -1) {
            evt.returnValue = true;;
        } else {
            evt.returnValue = false;;
        }
    } else {
        if (charCode > 31 &&
            (charCode < 48 || charCode > 57))
            evt.returnValue = false;;
    }

    if (evt.preventDefault == false)
        evt.preventDefault();
}

function SuccessToast(message) {
    var toastElList = [].slice.call(document.querySelectorAll('#toastSuccess'))
    var toastList = toastElList.map(function (toastEl) {
        $(toastEl).find('.toast-body').find('.body-text').text(message);
        // Creates an array of toasts (it only initializes them)
        return new bootstrap.Toast(toastEl, {
            animation: true,
            delay: 4000
        }) // No need for options; use the default options
    });
    toastList.forEach(toast => toast.show()); // This show them
    console.log(toastList); // Testing to see if it works
}

function ErrorToast(message) {
    var toastElList = [].slice.call(document.querySelectorAll('#toastError'))
    var toastList = toastElList.map(function (toastEl) {
        $(toastEl).find('.toast-body').find('.body-text').text(message);
        // Creates an array of toasts (it only initializes them)
        return new bootstrap.Toast(toastEl, {
            animation: true,
            autohide: true,
            delay: 3000,
        }) // No need for options; use the default options
    });
    toastList.forEach(toast => toast.show()); // This show them
    console.log(toastList); // Testing to see if it works
}

function WarningToast(message) {
    var toastElList = [].slice.call(document.querySelectorAll('#toastWarning'))
    var toastList = toastElList.map(function (toastEl) {
        $(toastEl).find('.toast-body').find('.body-text').text(message);
        // Creates an array of toasts (it only initializes them)
        return new bootstrap.Toast(toastEl, {
            animation: true,
            autohide: true,
            delay: 3000,
        }) // No need for options; use the default options
    });
    toastList.forEach(toast => toast.show()); // This show them
    console.log(toastList); // Testing to see if it works
}


function localDateFormat(value, format) {
    try {

        if (typeof value === 'string' && value.includes('/')) {
            var date = value.split('/')[0];
            var month = value.split('/')[1];
            var year = value.split('/')[2];

            var dt = new Date(year, month, date);
        }
        else if (typeof value === 'string' && value.includes('-')) {
            var date = value.split('-')[0];
            var month = value.split('-')[1];
            var year = value.split('-')[2];

            var dt = new Date(year, month, date);
        }
        else {
            var dt = new Date(value);
        }
        
        var day = dt.getDate().toString().length > 1 ? dt.getDate() : "0" + dt.getDate();
        var month = (dt.getMonth() + 1).toString().length > 1 ? (dt.getMonth() + 1) : "0" + (dt.getMonth() + 1);
        var year = dt.getFullYear();

        if (format == "dd/mm/yyyy") {
            return day + "/" + month + "/" + year;
        }
        else if (format == "mm/dd/yyyy") {
            return month + "/" + day + "/" + year;
        }
        else if (format == "yyyy/dd/mm") {
            return year + "/" + day + "/" + month;
        }
        else if (format == "yyyy/mm/dd") {
            return year + "/" + month + "/" + day;
        }

    } catch (e) {
        console.log(e);
    }
}

function dateAutoFormatter(event) {
    try {
        if (event.which !== 8) {
            let input = event.target.value;
            let out = input.replace(/\D/g, '');
            let len = out.length;

            if (len > 1 && len < 4) {
                out = out.substring(0, 2) + '/' + out.substring(2, 3);
            } else if (len >= 4) {
                out = out.substring(0, 2) + '/' + out.substring(2, 4) + '/' + out.substring(4, len);
                out = out.substring(0, 10)
            }
            event.target.value = out;
        }
    } catch (e) {
        console.log(e);
    }
}

function geMonthFromDate(value) {
    try {
        var date = new Date(value);

        var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

        return months[date.getMonth()];

    } catch (e) {
        console.log(e);
    }
}