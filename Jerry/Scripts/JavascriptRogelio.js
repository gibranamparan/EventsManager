
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
    
   