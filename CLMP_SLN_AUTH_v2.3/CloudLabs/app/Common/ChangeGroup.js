var groupData = [];

$(document).ready(function () {

    //var parentMain = document.getElementById('main');
    //var newChildMain = '<div id="submain">'+ @RenderBody() +'</div >';
    //parentMain.insertAdjacentHTML('beforeend', newChildMain);
    $('#changePassLoading').hide();
    $.ajax({
        method: "GET",
        url: apiUrl + "/CloudLabsGroups/GetCurrentUserGroup?userGroupId=" + currentUserGroup
    }).done(function (data) {
        groupData = data;
        window.value = groupData;
        if (groupData.length !== 1) {
            for (var i = 0; i < groupData.length; i++) {
                var parent = document.getElementById('addGroups');
                if (i === 0)
                    var newChild = '<option value=' + [i] + ' selected="selected">' + groupData[i].UserCurrentGroup + '</option>' + '<option value=' + [i + 1] + '>' + groupData[i].UserGroupName + '</option>';
                else
                    var newChild = '<option value=' + [i + 1] + '>' + groupData[i].UserGroupName + '</option>';

                parent.insertAdjacentHTML('beforeend', newChild);
            }
        }
        else {
            if (groupData[0].UserCurrentGroup == groupData[0].UserGroupName) {
                var parent = document.getElementById('addGroups');
                var newChild = '<option value= 1 selected="selected">' + groupData[0].UserCurrentGroup;
                parent.insertAdjacentHTML('beforeend', newChild);
            }
            else {
                var parent = document.getElementById('addGroups');
                var newChild = '<option value= 0 selected="selected">' + groupData[0].UserCurrentGroup + '</option>' + '<option value=' + [1] + '>' + groupData[0].UserGroupName + '</option>';
                parent.insertAdjacentHTML('beforeend', newChild);
            }
        }

    });
    $('.dropdown-submenu a').on("click", function (e) {
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });
    $('.dropdown-submenu select').on("click", function (e) {
        $(this).next('ul').toggle();
        e.stopPropagation();
        e.preventDefault();
    });
    //$('#changePass').on("click", function () {
    //    $('#myModal').modal('toggle');
    //});

    $("#addGroups").on('change', function () {
        $('.dropdown-toggle').next('ul').hide();
        $('.dropdown-submenu a').next('ul').hide();

        var elem = document.getElementById('submain');
        elem.parentNode.removeChild(elem);

        var parentMain = document.getElementById('main');

        var newChildMain = '<img class="loading-center" src="../../../Content/Image/loading2.gif" /> <span style="color: #25a9c0; text-align: center; display: block;" aria-busy="true" aria-live="polite" aria-label="Loading. Please Wait..">Loading. Please Wait..</span>';
        parentMain.insertAdjacentHTML('beforeend', newChildMain);

        window.location.reload(true);
        $.ajax({
            method: "POST",
            url: apiUrl + "/CloudLabsGroups/ChangeUserGroup?userGroup=" + $("#addGroups option:selected").text() + '&userid=' + currentId
        }).done(function (data) {
        });
    });

});