
var userGroup = currentUserGroup;
var roleName = userRoleName;
var userEmail = currentEmail;
var item = "";
var divElement = "";
var index = null;
var isCreateButton = false;
var isEditButton = false;
var currentDate = new Date();
var editStartDate = null;
var editEndDate = null;
var contextSuperAdmin = "";
var contextAdmin = "";



var getAllNotificationSuperAdmin = function () {
    $("#empty-notification-notice-superadmin").hide();
    $(".main-message-container").remove();
    $(".full-message-container").hide();
    $(".back-button").hide();

    $.ajax({
        type: "POST",
        url: apiUrl + "Notification/GetAllNotification/" + userGroup + "/" + roleName,
        success: function (data) {
            context = data;
            data.forEach(function (item) {

                //converts utc date to mm/dd/yyyy format
                //var startDate = new Date(item.StartDate.toString());
                //startDate = ((startDate.getMonth() > 8) ? (startDate.getMonth() + 1) : ('0' + (startDate.getMonth() + 1))) + '/' + ((startDate.getDate() > 9) ? startDate.getDate() : ('0' + startDate.getDate())) + '/' + startDate.getFullYear();

                //var endDate = new Date(item.EndDate.toString());
                //endDate = ((endDate.getMonth() > 8) ? (endDate.getMonth() + 1) : ('0' + (endDate.getMonth() + 1))) + '/' + ((endDate.getDate() > 9) ? endDate.getDate() : ('0' + endDate.getDate())) + '/' + endDate.getFullYear();

                var options = { month: 'short', day: 'numeric', year: 'numeric' };
                var startDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", options);
                var endDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", options);

                var createdOrEditedBy = "";
                if (item.EditedBy === null) {
                    createdOrEditedBy = "Created By: " + item.CreatedBy;
                } else {
                    createdOrEditedBy = "Edited By: " + item.EditedBy;
                }
                var allowModalToOpen = "";
                if (Date.parse(item.EndDate) > Date.parse(currentDate)) {
                    allowModalToOpen = `data-toggle="modal" data-target="#editMessage"`
                }

                var addSeeMoreButton = "";

                if (item.Message.length > 200) {
                    addSeeMoreButton = "<button>See more</button>"
                }

                divElement = `<div class="main-message-container">
                    <span class="object-item" style="display: none;" > ${item.NotificationId} </span>
                    <div class="main-message-input" style="width: 450px; " >
                    <div style="display:flex;">
                    
                    <div style="padding: 0 10px 0 10px; margin: 0 7px 0 10px; overflow-y: auto; height: 40px;">
                    <span class="message-span" style="font-size: 14px; font-weight: 500;"> ${item.Message} </span>
                    </div>
                    </div>
                    <div style="display: flex; justify-content: space-between; margin-left: 20px; font-size: 10px; margin-top: 0;">
                    <div style="display: flex; font-style: italic; margin-top: 5px;">
                    <span class="startdate-span" style="font-size: 10px !important;"> ${startDate}</span>
                    <p style="margin-left: 5px; margin-right: 5px;">-</p>
                    <span class="enddate-span" style="font-size: 10px !important;">${endDate}</span>
                    </div>
                    <div style="margin-right: 20px;">
                    <button class="edit-button" ${allowModalToOpen} style="border: none; background-color: inherit; color: #24AF75; font-weight: bold;">Edit</button>
                    <button class="delete-button" data-toggle="modal" data-target="#deleteMessage" style="border: none; background-color: inherit; color: #AEAEAE; font-weight: bold;">Delete</button>
                    </div>
                    </div>
                    </div >
                     <hr style="margin: 5px 0 0 !important;">
                    </div >
                    `;
                $('.output-data-bind').prepend(divElement);


            });

        }
    })
        .done(function () {
            if (contextSuperAdmin.length === 0) {
                $(".dropdownMainContainer")[0].style.width = "282px";
                $("#empty-notification-notice-superadmin").show();
                $("#header-title-notification").css("display", "none");
            } else {
                $(".dropdownMainContainer")[0].style.width = "457px";
                $("#empty-notification-notice-superadmin").hide();
            }
        });

};

var getAllNotificationAdmin = function () {

    $(".main-message-container").remove();
    $(".full-message-container").hide();
    $("#empty-notification-notice").hide();
    $(".back-button").hide();

    $.ajax({
        type: "POST",
        url: apiUrl + "Notification/GetAllNotification/" + userGroup + "/" + roleName,
        success: function (data) {
            contextAdmin = data;
            data.forEach(function (item) {




                var options = { month: 'short', day: 'numeric', year: 'numeric' };
                var startDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", options);
                var endDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", options);

                var option2 = { month: 'long', day: 'numeric', year: 'numeric' };
                var longStartDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", option2);
                var longEndDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", option2);

                var createdOrEditedBy = "";
                var createdOrEditedByUserName = ""
                if (item.EditedBy === null) {
                    createdOrEditedByUserName = item.CreatedBy;
                    createdOrEditedBy = "Created By: " + item.CreatedBy;
                } else {
                    createdOrEditedByUserName = item.EditedBy;
                    createdOrEditedBy = "Edited By: " + item.EditedBy;
                }



                divElement = `<div class="main-message-container" style="padding-top: 10px;">
                    <span class="object-item" style="display: none;" > ${item.NotificationId} </span>
                    <div class="main-message-input" style="width: 450px;">
                    <div style="display:flex;">
                    <i class="fa fa-circle" aria-hidden="true" style="margin-left: 22px; color: #125282; margin-top: 5px;"></i>
                    <div class="message-span-container" style="margin: 0 20px 0 10px; height: 40px;">
                    <span class="full-message" style="display: none;">${item.Message}</span>
                    <span class="message-span" style="font-size: 14px; font-weight: 500;" title="${item.Message}"> ${item.Message} </span>
                    </div>
                   
                    </div>
                    <div style="display: flex; justify-content: space-between; color: #AEAEAE; margin-left: 20px; font-size: 10px; margin-top: 0px; ">
                    <span style="font-style: italic; margin: auto 10px auto 20px; text-overflow:ellipsis; overflow: hidden; white-space: nowrap;  max-width: 180px;" title="${createdOrEditedByUserName}">${createdOrEditedBy}</span >
                    <div style="display: flex; font-style: italic; margin-top: 5px; margin-right: 20px;">
                    <span class="startdate-span" style="margin-top: auto; margin-bottom: auto;" title="${longStartDate}"> ${startDate}</span>
                    <p style="margin: auto 5px; ">-</p>
                    <span class="enddate-span" style="margin-top: auto; margin-bottom: auto;" title="${longEndDate}">${endDate}</span>
                    </div>
                    </div>
                    </div >
                    <hr style="margin: 5px 0 0px !important; border: 0 ;border-top: 1px solid #AEAEAE;">
                    </div >
                    `;
                if (Date.parse(item.EndDate) > Date.parse(currentDate) && Date.parse(item.StartDate) < currentDate) {
                    $('.admin-data-container').prepend(divElement);
                }
            });
        }
    })
        .done(function () {
            var notificationCount = $(".admin-data-container")[0].childElementCount;
            $(".admin-notification-count")[0].innerHTML = notificationCount;
            if ($(".admin-notification-count")[0].innerHTML === "0") {
                $(".admin-notification-count-container").remove();
            }

            if (contextAdmin.length === 0) {
                $(".dropdownMainContainer")[0].style.width = "282px";
                $("#empty-notification-notice").show();
                $(".notification-header").hide();
            } else {
                $(".dropdownMainContainer")[0].style.width = "457px";
                $("#empty-notification-notice").hide();
            }

            $(".message-span-container").click(function () {
                var index = $(".message-span-container").index(this);
                var fullMessage = $(".full-message")[index].innerHTML
                $("#overflow-message")[0].innerHTML = fullMessage;
                $(".main-message-container").hide();
                $(".back-button").show();
                $(".full-message-container").show();
                $('#notification-container').addClass('open');
            });

            $(".back-button").click(function () {
                $(".main-message-container").show();
                $(".back-button").hide();
                $(".full-message-container").hide();
            });
        });

};

var disableEditButton = function () {
    var editButtons = $(".main-message-container .edit-button");


    for (var i = 0; i < editButtons.length; i++) {
        var editDate = new Date($(".main-message-container .enddate-span")[i].innerHTML);
        editDate.setHours(editDate.getHours() + 23);
        editDate.setMinutes(editDate.getMinutes() + 59);
        editDate.setSeconds(editDate.getSeconds() + 59);
        editDate.setMilliseconds(editDate.getMilliseconds() + 999);


        if (editDate < currentDate) {
            $(".main-message-container .edit-button")[i].style.display = "none";
        }
    }

};

var sendMessage = function () {

    var model = {
        Message: $(".create-message-container .input-model").val(),
        StartDate: $(".create-message-container #create-startdate").val(),
        EndDate: $(".create-message-container #create-enddate").val(),
        UserGroup: userGroup,
        CreatedBy: userEmail
    };

    $.ajax({
        type: "POST",
        url: apiUrl + "Notification/InsertNotification",
        data: model,
        success: function (data) {

            console.log(data);
        }
    })
        .done(function () {
            refreshSuperAdminList();
        });

};


var updateMessage = function () {


    var model = {
        Message: $(".edit-message-container .input-model").val(),
        StartDate: $(".edit-message-container #edit-startdate").val(),
        EndDate: $(".edit-message-container #edit-enddate").val(),
        UserGroup: userGroup,
        EditedBy: userEmail
    };

    var notificationId = $(".main-message-container .object-item")[index].innerHTML.toString();

    $.ajax({
        type: "POST",
        url: apiUrl + "Notification/UpdateNotification/" + notificationId,
        data: model,
        success: function (data) {
            console.log(data);
        }
    })
        .done(function () {
            refreshSuperAdminList();
        });


};

var refreshSuperAdminList = function () {
    $("#empty-notification-notice-superadmin").hide();
    $(".main-message-container").remove();
    $(".full-message-container").hide();
    $(".back-button").hide();

    $.ajax({
        type: "POST",
        url: apiUrl + "Notification/GetAllNotification/" + userGroup + "/" + roleName,
        success: function (data) {
            contextSuperAdmin = data;


            data.forEach(function (item) {

                var options = { month: 'short', day: 'numeric', year: 'numeric' };
                var startDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", options);
                var endDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", options);

                var option2 = { month: 'long', day: 'numeric', year: 'numeric' };
                var longStartDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", option2);
                var longEndDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", option2);

                var createdOrEditedBy = "";
                var createdOrEditedByUserName = ""
                if (item.EditedBy === null) {
                    createdOrEditedByUserName = item.CreatedBy;
                    createdOrEditedBy = "Created By: " + item.CreatedBy;
                } else {
                    createdOrEditedByUserName = item.EditedBy;
                    createdOrEditedBy = "Edited By: " + item.EditedBy;
                }


                var allowModalToOpen = "";
                if (Date.parse(item.EndDate) > Date.parse(currentDate)) {
                    allowModalToOpen = `data-toggle="modal" data-target="#editMessage"`
                }


                divElement = `<div class="main-message-container" style="padding-top: 10px;">
                    <span class="object-item" style="display: none;" > ${item.NotificationId} </span>
                    <div class="main-message-input" style="width: 450px;" >
                    <div style="display:flex;">
                    
                    <div class="message-span-container" style="padding: 0 10px 0 10px; margin: 0 7px 0 10px; height: 40px;">
                    <span class="full-message" style="display: none;">${item.Message}</span>
                    <span class="message-span" title="${item.Message}"> ${item.Message} </span>
                    </div>
                    
                    </div>
                    <div style="display: flex; justify-content: space-between; margin-left: 20px; font-size: 10px; margin-top: 0px;">
                    <div style="display: flex; font-style: italic; margin-top: 5px; font-size: 7px;">
                    <span class="" style="font-size: 10px !important; color: #AEAEAE; margin-right: 10px; margin-top: auto; margin-bottom: auto; text-overflow:ellipsis;  overflow: hidden; white-space: nowrap;  max-width: 120px; min-width: 120px; width: 120px" title="${createdOrEditedByUserName}"> ${createdOrEditedBy}</span>
                    <i class="fa fa-circle" aria-hidden="true" style="margin-right: 10px; color: #AEAEAE; margin-top: auto; margin-bottom: auto;"></i>
                    <span class="startdate-span" style="font-size: 10px !important; color: #AEAEAE; margin-top: auto; margin-bottom: auto;" title="${longStartDate}"> ${startDate}</span>
                    <p style="margin-left: 5px; margin-right: 5px; margin-top: auto; margin-bottom: auto;">-</p>
                    <span class="enddate-span" style="font-size: 10px !important; color: #AEAEAE; margin-top: auto; margin-bottom: auto;" title="${longEndDate}">${endDate}</span>
                    </div>
                    <div style="margin-right: 20px; margin-top: auto;">
                    <button class="edit-button" ${allowModalToOpen} style="border: none; background-color: inherit; color: #24AF75; font-weight: bold; margin-top: auto; margin-bottom: auto;">Edit</button>
                    <button class="delete-button" data-toggle="modal" data-target="#deleteMessage" style="margin-top: auto; margin-bottom: auto;border: none; background-color: inherit; color: #d9534f; font-weight: bold;">Delete</button>
                    </div>
                    </div>
                    </div >
                    <hr style="margin: 5px 0 0px !important; border: 0 ;border-top: 1px solid #AEAEAE">
                    </div >
                    `;
                $('.output-data-bind').prepend(divElement);


            });

        }
    })
        .done(function () {
            if (contextSuperAdmin.length === 0) {
                $(".dropdownMainContainer")[0].style.width = "282px";
                $("#empty-notification-notice-superadmin").show();
                $("#header-title-notification").css("display", "none");
            } else {
                $(".dropdownMainContainer")[0].style.width = "457px";
                $("#empty-notification-notice-superadmin").hide();
                $("#header-title-notification").css("display", "block");
            }

            $(".message-span-container").click(function () {
                var index = $(".message-span-container").index(this);
                var fullMessage = $(".full-message")[index].innerHTML
                $("#overflow-message")[0].innerHTML = fullMessage;
                $(".main-message-container").hide();
                $(".create-button").hide();
                $(".back-button").show();
                $(".full-message-container").show();
            });

            $(".back-button").click(function () {
                $(".main-message-container").show();
                $(".create-button").show();
                $(".back-button").hide();
                $(".full-message-container").hide();
            });

            disableEditButton();

            $(".main-message-container .delete-button").click(function () {
                index = $(".main-message-container .delete-button").index(this);
                $(".dropdownMainContainer").slideUp("fast");
            });

            $(".edit-button").click(function () {
                if (isCreateButton) {
                    return;
                }

                $(".dropdownMainContainer").slideUp("fast");
                $(".edit-message-container #edit-enddate").datepicker("destroy");

                index = $(".main-message-container .edit-button").index(this);

                editStartDate = Date.parse($(".main-message-container .startdate-span")[index].innerHTML);

                editEndDate = new Date($(".main-message-container .enddate-span")[index].innerHTML);
                editEndDate.setHours(editEndDate.getHours() + 23);
                editEndDate.setMinutes(editEndDate.getMinutes() + 59);
                editEndDate.setSeconds(editEndDate.getSeconds() + 59);
                editEndDate.setMilliseconds(editEndDate.getMilliseconds() + 999);


                var editMessage = $(".main-message-container .message-span")[index].innerHTML;

                if (editEndDate < currentDate) {
                    return;
                }

                $("#edit-startdate").datepicker("enable");

                $(".edit-message-container #edit-startdate")[0].value = $(".main-message-container .startdate-span")[index].innerHTML;
                $(".edit-message-container #edit-enddate")[0].value = $(".main-message-container .enddate-span")[index].innerHTML;
                $(".edit-message-container .input-model")[0].innerHTML = $(".main-message-container .full-message")[index].innerHTML;

                var editStartDateValue = new Date($(".main-message-container .startdate-span")[index].innerHTML);




                $(function () {
                    $(".edit-message-container #edit-startdate").datepicker({
                        showButtonPanel: true,
                        minDate: currentDate,
                        dateFormat: "M-dd-yy",
                        showOn: "both",
                        buttonImage: "../../../Content/Image/calendar-icon.png",
                        buttonImageOnly: true,
                        buttonText: "Select date"
                    });
                });
                $(function () {
                    $(".edit-message-container #edit-enddate").datepicker({
                        showButtonPanel: true,
                        minDate: editStartDateValue,
                        dateFormat: "M-dd-yy",
                        showOn: "both",
                        buttonImage: "../../../Content/Image/calendar-icon.png",
                        buttonImageOnly: true,
                        buttonText: "Select date"
                    });
                });

                $(".edit-startdate-container .ui-datepicker-trigger").addClass("calendar-startdate");
                $(".edit-enddate-container .ui-datepicker-trigger").addClass("calendar-enddate");

                isEditButton = true;

                if (editStartDate < currentDate && currentDate < editEndDate) {
                    $("#edit-startdate").attr("disabled", "disabled");
                    $(".edit-message-container .input-model").attr("disabled", "disabled");
                    var isDatePickerDisabled = $("#edit-startdate").datepicker("disable");
                    if (isDatePickerDisabled) {
                        $(".edit-startdate-container .calendar-startdate").css("cursor", "no-drop")
                    }
                } else {
                    $("#edit-startdate").removeAttr("disabled");
                    $(".edit-message-container .input-model").removeAttr("disabled");
                }
            });
        });




};

var refreshAdminOrStudentList = function () {
    $(".main-message-container").remove();
    $(".full-message-container").hide();
    $(".back-button").hide();

    $.ajax({
        type: "POST",
        url: apiUrl + "Notification/GetAllNotification/" + userGroup + "/" + roleName,
        success: function (data) {
            contextAdmin = data;
            data.forEach(function (item) {




                var options = { month: 'short', day: 'numeric', year: 'numeric' };
                var startDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", options);
                var endDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", options);

                var option2 = { month: 'long', day: 'numeric', year: 'numeric' };
                var longStartDate = new Date(item.StartDate.toString()).toLocaleDateString("en-US", option2);
                var longEndDate = new Date(item.EndDate.toString()).toLocaleDateString("en-US", option2);

                var createdOrEditedBy = "";
                var createdOrEditedByUserName = ""
                if (item.EditedBy === null) {
                    createdOrEditedByUserName = item.CreatedBy;
                    createdOrEditedBy = "Created By: " + item.CreatedBy;
                } else {
                    createdOrEditedByUserName = item.EditedBy;
                    createdOrEditedBy = "Edited By: " + item.EditedBy;
                }



                divElement = `<div class="main-message-container" style="padding-top: 10px;">
                    <span class="object-item" style="display: none;" > ${item.NotificationId} </span>
                    <div class="main-message-input" style="width: 450px;">
                    <div style="display:flex;">
                    <i class="fa fa-circle" aria-hidden="true" style="margin-left: 22px; color: #125282; margin-top: 5px;"></i>
                    <div class="message-span-container" style="margin: 0 20px 0 10px; height: 40px;">
                    <span class="full-message" style="display: none;">${item.Message}</span>
                    <span class="message-span" style="font-size: 14px; font-weight: 500;" title="${item.Message}"> ${item.Message} </span>
                    </div>
                   
                    </div>
                    <div style="display: flex; justify-content: space-between; color: #AEAEAE; margin-left: 20px; font-size: 10px; margin-top: 0px; ">
                    <span style="font-style: italic; margin: auto 10px auto 20px; text-overflow:ellipsis; overflow: hidden; white-space: nowrap;  max-width: 180px;" title="${createdOrEditedByUserName}">${createdOrEditedBy}</span >
                    <div style="display: flex; font-style: italic; margin-top: 5px; margin-right: 20px;">
                    <span class="startdate-span" style="margin-top: auto; margin-bottom: auto;" title="${longStartDate}"> ${startDate}</span>
                    <p style="margin: auto 5px; ">-</p>
                    <span class="enddate-span" style="margin-top: auto; margin-bottom: auto;" title="${longEndDate}">${endDate}</span>
                    </div>
                    </div>
                    </div >
                    <hr style="margin: 5px 0 0px !important; border: 0 ;border-top: 1px solid #AEAEAE;">
                    </div >
                    `;
                if (Date.parse(item.EndDate) > Date.parse(currentDate) && Date.parse(item.StartDate) < currentDate) {
                    $('.admin-data-container').prepend(divElement);
                }
            });
        }
    })
        .done(function () {
            if (contextAdmin.length === 0) {
                $("#empty-notification-notice").show();
                $(".notification-header").hide();
            } else {
                $("#empty-notification-notice").hide();
            }

            $(".message-span-container").click(function () {
                var index = $(".message-span-container").index(this);
                var fullMessage = $(".full-message")[index].innerHTML
                $("#overflow-message")[0].innerHTML = fullMessage;
                $(".main-message-container").hide();
                $(".back-button").show();
                $(".full-message-container").show();
                $('#notification-container').addClass('open');
            });

            $(".back-button").click(function () {
                $(".main-message-container").show();
                $(".back-button").hide();
                $(".full-message-container").hide();
            });
        });
};


if (roleName === "SuperAdmin") {
    getAllNotificationSuperAdmin();

    $("#notification-icon").click(function () {
        refreshSuperAdminList();
        $(this).next(".dropdownMainContainer").slideToggle("fast");
        $(".message-span-container").mouseenter(function () {
            var myIndex = $(".main-span-container").index(this);
            console.log(myIndex);
        });
    });

    // Hide dropdown menu on click outside
    $(document).on("click", function (event) {
        if (!$(event.target).closest("#notification-container").length) {
            $(".dropdownMainContainer").slideUp("fast");
        }
    });

    $('#createMessage').on('hidden.bs.modal', function () {
        $(".dropdownMainContainer").slideDown("fast");
    });

    $("#editMessage").on('hidden.bs.modal', function () {
        $(".dropdownMainContainer").slideDown("fast");
    });

    $("#deleteMessage").on('hidden.bs.modal', function () {
        $(".dropdownMainContainer").slideDown("fast");
    });






} else {
    getAllNotificationAdmin();

    $("#admin-notification-icon").click(function () {
        var isTrue = $(".dropdownMainContainer")[0].style.display;
        if (isTrue === "none") {
            refreshAdminOrStudentList();
        }

        $(this).next(".dropdownMainContainer").slideToggle("fast");

        $(".admin-notification-count-container").remove();
    });

    // Hide dropdown menu on click outside
    $(document).on("click", function (event) {
        if (!$(event.target).closest("#notification-container").length) {
            $(".dropdownMainContainer").slideUp("fast");
        }
    });

}



$('.create-button').click(function () {
    isEditButton = false;
    isCreateButton = true;
    $(".dropdownMainContainer").slideUp("fast");
    $(".create-message-container #create-startdate").datepicker("destroy");
    $(".create-message-container #create-enddate").datepicker("destroy");
    $("#createMessage .send-message-button").attr("disabled", "disabled");
    $(function () {
        $(".create-message-container #create-startdate").datepicker({
            showButtonPanel: true,
            minDate: new Date(),
            dateFormat: "M-dd-yy",
            showOn: "both",
            buttonImage: "../../../Content/Image/calendar-icon.png",
            buttonImageOnly: true,
            buttonText: "Select date"
        });
    });
    $(function () {
        $(".create-message-container #create-enddate").datepicker({
            showButtonPanel: true,
            minDate: currentDate,
            dateFormat: "M-dd-yy",
            showOn: "both",
            buttonImage: "../../../Content/Image/calendar-icon.png",
            buttonImageOnly: true,
            buttonText: "Select date"
        });
    });
    $(".create-startdate-container .ui-datepicker-trigger").addClass("calendar-startdate");
    $(".create-enddate-container .ui-datepicker-trigger").addClass("calendar-enddate");

    if (isEditButton) {
        return;
    }



});

$("#confirmDeletionButton").click(function () {
    console.log(index);
    var notificationId = $(".main-message-container .object-item")[index].innerHTML;

    $.ajax({
        type: "DELETE",
        url: apiUrl + "Notification/DeleteNotification/" + notificationId,
        success: function (data) {

        }
    })
        .done(function () {
            refreshSuperAdminList();
        });
});

$('.message-cancel-button').click(function () {
    if (isCreateButton) {

        $('.main-message-input .create-message-container').hide();
        if ($("#create-startdate")[0].value) {
            $("#create-startdate")[0].value = null;
        }
        if ($("#create-enddate")[0].value) {
            $("#create-enddate")[0].value = null;
        }
        if ($(".create-message-container .input-model").val()) {
            $(".create-message-container .input-model")[0].value = "";
        }
    }
    isCreateButton = false;
    isEditButton = false;
});



$('.send-message-button').click(function () {

    if (isCreateButton) {
        var startDate = $("#create-startdate").val();
        var endDate = $("#create-enddate").val();
        var message = $(".create-message-container .input-model").val();
        if (startDate && endDate && message) {

            sendMessage();
        } else {
            return;
        }
        if ($("#create-startdate")[0].value) {
            $("#create-startdate")[0].value = null;
        }
        if ($("#create-enddate")[0].value) {
            $("#create-enddate")[0].value = null;
        }
        if ($(".create-message-container .input-model").val()) {
            $(".create-message-container .input-model")[0].value = "";
        }

    } else if (isEditButton) {
        var editStartDateValue = $("#edit-startdate").val();
        var editEndDateValue = $("#edit-enddate").val();
        var editMessageValue = $(".edit-message-container .input-model").val();

        if (editStartDateValue && editEndDateValue && editMessageValue) {
            updateMessage();
        } else {
            return;
        }

    }
    isCreateButton = false;
    isEditButton = false;
});





var createDateValidation = function () {
    $(".create-message-container #create-enddate").datepicker("destroy");
    var endDateMinValue = $("#create-startdate").val();

    var options = { month: 'short', day: 'numeric', year: 'numeric' };

    var isCreateStartDateGreaterThanEndDate = Date.parse($("#create-startdate").val()) >= Date.parse($("#create-enddate").val());

    if (isCreateStartDateGreaterThanEndDate) {
        $("#create-enddate")[0].value = endDateMinValue;
    }
    $(function () {
        $(".create-message-container #create-enddate").datepicker({
            showButtonPanel: true,
            minDate: endDateMinValue,
            dateFormat: "M-dd-yy",
            showOn: "both",
            buttonImage: "../../../Content/Image/calendar-icon.png",
            buttonImageOnly: true,
            buttonText: "Select date"
        });
    });
    $(".create-startdate-container .ui-datepicker-trigger").addClass("calendar-startdate");
    $(".create-enddate-container .ui-datepicker-trigger").addClass("calendar-enddate");
};


var editDateValidation = function () {
    $(".edit-message-container #edit-enddate").datepicker("destroy");
    var notificationIsInDateRange = editStartDate < currentDate;

    if (notificationIsInDateRange) {
        //start date cannot be changed.
        $(".edit-message-container #edit-startdate")[0].value = $(".main-message-container .startdate-span")[index].innerHTML;


        $(".edit-message-container #edit-enddate").datepicker({
            showButtonPanel: true,
            minDate: editStartDateValue,
            dateFormat: "M-dd-yy",
            showOn: "both",
            buttonImage: "../../../Content/Image/calendar-icon.png",
            buttonImageOnly: true,
            buttonText: "Select date"
        });

        $(".edit-startdate-container .ui-datepicker-trigger").addClass("calendar-startdate");
        $(".edit-enddate-container .ui-datepicker-trigger").addClass("calendar-enddate");
    } else if (!notificationIsInDateRange) {
        var editEndDateMinValue = $("#edit-startdate").val();
        var options = { month: 'short', day: 'numeric', year: 'numeric' };


        var isEditStartDateGreaterThanEndDate = Date.parse($("#edit-startdate").val()) >= Date.parse($("#edit-enddate").val());

        if (isEditStartDateGreaterThanEndDate) {
            $("#edit-enddate")[0].value = editEndDateMinValue;
        }


        $(".edit-message-container #edit-enddate").datepicker({
            showButtonPanel: true,
            minDate: editEndDateMinValue,
            dateFormat: "M-dd-yy",
            showOn: "both",
            buttonImage: "../../../Content/Image/calendar-icon.png",
            buttonImageOnly: true,
            buttonText: "Select date"

        });
        $(".edit-startdate-container .ui-datepicker-trigger").addClass("calendar-startdate");
        $(".edit-enddate-container .ui-datepicker-trigger").addClass("calendar-enddate");
    }
};

var allowSubmitButton = function () {
    if (isCreateButton) {
        var createStartDate = $("#create-startdate").val();
        var createEndDate = $("#create-enddate").val();
        var createNotificationMessage = $(".create-message-container .input-model").val();

        if (createStartDate && createEndDate && createNotificationMessage) {
            $("#createMessage .send-message-button").removeAttr("disabled");
        } else {
            $("#createMessage .send-message-button").attr("disabled", "disabled");
        }
    } else {
        var editNotificationMessage = $(".edit-message-container .input-model").val();
        if (editNotificationMessage) {
            $("#editMessage .send-message-button").removeAttr("disabled");
        } else {
            $("#editMessage .send-message-button").attr("disabled", "disabled");
        }
    }

};





