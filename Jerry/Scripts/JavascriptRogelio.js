//Todos los inputs de tipo DateTime o con atribute datetimepicker, se conviernte en control JQuery para datetimepickers
$('table.datatable').DataTable()
var datesInputs = $("input[type='datetime'], input[datetimepicker]")

//La herramienta para formatos de fecha Moment en javascript se configura para español
moment.locale('es')

/* When the user clicks on the button, 
toggle between hiding and showing the dropdown content */
function openActionsDropDown(actionBtn) {
    $(actionBtn).parent().children("#myDropdown").slideToggle();
}

$.each(datesInputs, function (idx, item) {
    $(item).attr('type','datetime')
    $(item).datetimepicker({ lang: 'es', mask: true, value: item.value });
})

function changeIcon(bar) {
    $(bar).find("i").toggleClass("fa-window-minimize fa-window-maximize")
}

function doConfirm(msg, yesFn, noFn) {
    var confirmBox = $("#confirmBox");
    confirmBox.find(".message").text(msg);
    confirmBox.find(".yes,.no").unbind().click(function () {
        confirmBox.hide();
    });
    confirmBox.find(".yes").click(yesFn);
    confirmBox.find(".no").click(noFn);
    confirmBox.show();
}

function mostarFormulario(formulario) {
    //var frm = document.form1;
    var frm = formulario;
    if (frm.style.display == "block") { frm.style.display = "none" }
    else
        if (frm.style.display == "none") { frm.style.display = "block" }
}


$.fn.slideDownOrUp = function (show) {
    return show ? this.slideDown() : this.slideUp();
}

$.fn.fadeInOrOut = function (status) {
    return status ? this.fadeIn() : this.fadeOut();
}

function currencyToNumber(numStr) {
    numStr = numStr.trim().replace("$", "").replace(",", "");
    var num = isNaN(numStr) ? 0 : Number(numStr);
    return num;
}

function numberToCurrency(num) {
    num = !num ? 0 : numeral(num).format("$0,0.00")
    return num;
}

function strToDateTime(strDt) {
    var dt = new Date(strDt);
    return dt;
}

function dateTimeToDateTimePickerFormat(dt) {
    //yyyy/MM/dd HH:mm
    return (dt.getFullYear() + "/" + numeral(dt.getMonth() + 1).format("00") + "/"
        + numeral(dt.getDate()).format("00") + " " + numeral(dt.getHours()).format("00") + ":"
        + numeral(dt.getMinutes()).format("00"))
}

function jsonDateToJSDate(jsonDate) {
    return new Date(parseInt(jsonDate.substr(6)));
}

Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

Date.prototype.dateISOFormat = function (days) {
    var dat = new Date(this.valueOf());
    return dat.toISOString().slice(0, 10);
}

boolParse = function (myStr) {
    return myStr.toLowerCase() == 'true';
}

//Agrega funcion a JQuery para permitir solicitudes asincronas identificandose como usuario logeado
jQuery.postJSON = function (url, data, dataType, success, fail, always, antiForgeryToken) {
    if (dataType === void 0) { dataType = "json"; }
    if (typeof (data) === "object") { data = JSON.stringify(data); }
    var ajax = {
        url: url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: dataType,
        data: data,
        success: success,
        fail: fail,
        complete: always
    };
    if (antiForgeryToken) {
        ajax.headers = {
            "__RequestVerificationToken": antiForgeryToken
        };
    };

    return jQuery.ajax(ajax);
};
   