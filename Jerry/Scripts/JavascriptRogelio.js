$('table.datatable').DataTable()
var datesInputs = $("input[type='datetime'], input[datetimepicker]")

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
    
   