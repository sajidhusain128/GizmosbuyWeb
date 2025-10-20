$(document).ready(function () {
    setActiveMenu();
});

function numberOnly(event) {
    try {
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
    } catch (e) {
        console.error(e);
    } 
}

function isNumberDecimal(_this, event) {
    try {
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
    } catch (e) {
        console.error(e);
    }
    
}

function SuccessToast(message) {
    try {
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
    } catch (e) {
        console.error(e);
    }  
}

function ErrorToast(message) {
    try {
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
    } catch (e) {
        console.error(e);
    }
}

function WarningToast(message) {
    try {
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
    } catch (e) {
        console.error(e);
    }
}


function localDateFormat(value, format) {
    try {

        if (value.length > 10) {
            value = value.substring(0, 10);
        }

        if (typeof value === 'string' && value.includes('/')) {

            if (value.split('/')[0].length == 4) {
                var year = value.split('/')[0];
                var month = value.split('/')[1];
                var date = value.split('/')[2];
            }
            else {
                var date = value.split('/')[0];
                var month = value.split('/')[1];
                var year = value.split('/')[2];
            }

            var dt = new Date(year, month - 1, date);
        }
        else if (typeof value === 'string' && value.includes('-')) {
            if (value.split('-')[0].length == 4) {
                var year = value.split('-')[0];
                var month = value.split('-')[1];
                var date = value.split('-')[2];
            }
            else {
                var date = value.split('-')[0];
                var month = value.split('-')[1];
                var year = value.split('-')[2];
            }

            var dt = new Date(year, month - 1, date);
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
        console.error(e);
        return '';
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
        console.error(e);
    }
}

function geMonthFromDate(value) {
    try {
        var date = new Date(value);

        var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

        return months[date.getMonth()];

    } catch (e) {
        console.error(e);
        return '';
    }
}

function getMonthNumber(monthName) {
    try {
        const months = {
            January: 1, Jan: 1,
            February: 2, Feb: 2,
            March: 3, Mar: 3,
            April: 4, Apr: 4,
            May: 5,
            June: 6, Jun: 6,
            July: 7, Jul: 7,
            August: 8, Aug: 8,
            September: 9, Sep: 9,
            October: 10, Oct: 10,
            November: 11, Nov: 11,
            December: 12, Dec: 12
        };

        const normalized = monthName.trim().charAt(0).toUpperCase() + monthName.trim().slice(1).toLowerCase();
        return months[normalized] || -1; // returns -1 if not found
    } catch (e) {
        console.error(e);
        return -1;
    }
    
}

function formatCustomDate(date, delimiter) {
    try {
        const pad = (n) => n.toString().padStart(2, '0');

        const dd = pad(date.getDate());
        const MM = pad(date.getMonth() + 1); // Months are zero-based
        const yy = pad(date.getFullYear() % 100); // Get last two digits of year
        const HH = pad(date.getHours());
        const mm = pad(date.getMinutes());
        const ss = pad(date.getSeconds());

        return `${dd}${delimiter}${MM}${delimiter}${yy}${delimiter}${HH}${delimiter}${mm}${delimiter}${ss}`;
    } catch (e) {
        console.WarningToast(e);
        return '';
    }
    
}

function setActiveMenu() {
    try {
        var pathName = location.pathname;

        const trimmedPath = getBasePath(pathName)

        $('#sidebar div ul.list-unstyled').children('li').removeAttr("class");

        var getActivePath = "";

        switch (trimmedPath) {
            case "/Purchase/Index":
            case "/Purchase/Create":
            case "/Purchase/Edit":
                getActivePath = "/Purchase/Index";
                break;

            case "/Sales/Index":
            case "/Sales/Create":
            case "/Sales/Edit":
                getActivePath = "/Sales/Index";
                break;

            case "/Inventory/RawData":
                getActivePath = "/Inventory/RawData";
                break;

            case "/Inventory/Summary":
                getActivePath = "/Inventory/Summary";
                break;

            default:
                getActivePath = "/Home/Index";
                break;
        }

        //$('#sidebar div ul.list-unstyled').children('li a.href="' + getActivePath + '"').attr("class", "active");

        $('#sidebar div ul.list-unstyled').children('li').each(function (row, index) {
            if ($(this).find('a')[0].href.includes(getActivePath)) {
                $(this).attr("class", "active");
            }
        })

    } catch (e) {
        console.log(e);
    }
}

function getBasePath(url) {
    // Remove query string and hash if present
    const cleanUrl = url.split('?')[0].split('#')[0];

    return cleanUrl;
}

function preventBackspace(e) {
    var evt = e || window.event;
    if (evt) {
        var keyCode = evt.charCode || evt.keyCode;
        if (keyCode === 8) {
            if (evt.preventDefault) {
                evt.preventDefault();
            } else {
                evt.returnValue = false;
            }
        }
    }
}