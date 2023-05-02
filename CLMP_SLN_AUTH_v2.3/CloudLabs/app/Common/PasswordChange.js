

function saveChangePass(id) {
    $('#changePassBody').hide();
    document.getElementById('btnCancel').setAttribute("disabled", "disabled");
    document.getElementById('btnSave').setAttribute("disabled", "disabled");
    $('#changePassLoading').show();

    var model = {
        Id: id,
        CurrentPassword: $("#CurrentPassword").val(),
        NewPassword: $("#NewPassword").val(),
        ConfirmPassword: $("#ConfirmPassword").val()

    };
    $.ajax(
        {
            type: "POST",
            url: "Account/ChangePassword",
            data: model,
            success: function (result) {
                if (result !== "")
                {
                    $('#changePassLoading').hide();

                    $('#changePassBody').show();
                    document.getElementById('btnCancel').removeAttribute("disabled");
                    document.getElementById('btnSave').removeAttribute("disabled");
                    $("#ErrorMessage").text(result);
                }
                else
                {
                    $('#changePassLoading').hide();

                    $('#changePassBody').show();
                    document.getElementById('btnCancel').removeAttribute("disabled", "disabled");
                    document.getElementById('btnSave').removeAttribute("disabled", "disabled");
                    $("#ConfirmationModal").modal();
                }
            },
            error: function (x, e) {

            }
        });
}

function closeModal() {
     window.location = 'Account/Logout';
}