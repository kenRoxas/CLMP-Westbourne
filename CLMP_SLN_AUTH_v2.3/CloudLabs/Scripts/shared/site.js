var token = "";
var GCPAccToken = "";
var GCPReftoken = "";
(function () {
    var app = angular.module("main-app", ["ngFileUpload", "ngCookies", "infinite-scroll"]);

    app.run(runFunction);

    runFunction.$inject = ['$http', '$cookies'];

    function runFunction($http, $cookies) {
        var tok = $cookies.get('CloudSwyftToken');
        var gcpAccessTok = $cookies.get('CloudSwyftGCPAccessToken');
        var gcpRefreshTok = $cookies.get('CloudSwyftGCPRefreshToken');

        $http.defaults.headers.common.Authorization = 'Bearer ' + tok;

        token = tok;
        GCPAccToken = gcpAccessTok;
        GCPReftoken = gcpRefreshTok;
    }

    app.service('svc', svcFunction);

    svcFunction.$inject = ['$http', 'Upload', '$timeout', '$q', '$filter'];

    function svcFunction($http, Upload, $timeout, $q, $filter) {
        var service = this;

        this.getEmailAddressExist = function (email) {
            return $http.get(apiUrl + "/UserManagement//GetEmailAddressExist?email=" + encodeURIComponent(email)).then(
                function (response) { return response.data; });
        };

        this.gotoRegister = function (userModel) {
            return $http.post(authUrl + "/api/Account/Register", userModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                });
        };

        this.GetProvisionedVeprofilesByUser = function (userId) {
            return $http.get(apiUrl + "/VEProfiles/GetProvisionedVeprofilesByUser?userId=" + userId).then(
                function (response) { return response.data; });
        };

        this.getMe = function () {
            return $http({
                method: 'GET',
                url: authUrl + "/api/account/me",
                //data: user,
                headers: { 'Authorization': 'Bearer ' + token }
            });
        };

        //this.getMe = function () {
        //    //$http.defaults.headers.common.Authorization = 'Bearer ' + token;
        //    //return $http.get(authUrl + "/api/account/me").then(function (response) {
        //    //    return response.data;
        //    //});
        //    return $http.get(authUrl + "/api/account/me", {
        //        headers: {
        //            "Authorization": 'Bearer ' + token
        //        }
        //    }).then(function (reesponse) {
        //        return response.data;
        //    })
        //};


        this.getProvisionedDetails = function (userId, veprofileId) {
            return $http.get(apiUrl + "/Virtualmachines/ByVeProfileUser?userID=" + userId + "&veProfileId=" + veprofileId).then(
                function (response) {
                    return response.data;
                });
        };

        this.GetActivityLabList = function (pageSize, pageNum, labacts) {
            return $http.get(apiUrl + "/LabActivities?q=&pageSize=" + pageSize + "&activePage=" + pageNum + "&labacts=" + labacts).then(
                function (response) { return response.data; });
        };
        this.GetLabActivity = function (labActivityID) {
            return $http.get(apiUrl + "LabActivities/" + labActivityID).then(
                function (response) { return response.data; });
        };
        this.postLabActivity = function (labActs) {
            return $http.post(apiUrl + "/LabActivities/CreateLabActivities", labActs, {
                headers: { 'Content-Type': 'application/json' }
            }).then(function (response) { return response.data; });
        };
        this.deleteLabActivities = function (id) {
            return $http.delete(apiUrl + "LabActivities/DeleteLabActivities?id=" + id).then(
                function (response) { return response.data; });
        };

        this.getLabProfiles = function (UserGroupID) {
            return $http.get(apiUrl + "/VEProfiles/GetVEProfiles?UserGroupID=" + UserGroupID + "&isSuperAdmin=" + userIsSuperAdmin).then(
                function (response) { return response.data; });
        };

        this.getAllLabProfiles = function () {
            return $http.get(apiUrl + "/VEProfiles/GetAllVEProfiles").then(
                function (response) { return response.data; });
        };

        this.getVETemplates = function (Id) {
            return $http.get(apiUrl + "/VirtualEnvironments/ByVEType?veTypeID=" + Id + "&userGroup=" + currentUserGroup)
                .then(function (response) { return response.data; });
        };

        this.getLabActivities = function () {
            return $http.get(apiUrl + "/LabActivities")
                .then(function (response) { return response.data; });
        };

        this.getProfileActivities = function (labProfileId) {
            return $http.get(apiUrl + "/LabActivities/ByVEProfile?veProfileID=" + labProfileId)
                .then(function (response) { return response.data; });
        };

        this.searchLabProfiles = function (entry, pageSize) {
            return $http.get(apiUrl + "/VEProfiles?q=" + entry + "&pageSize=" + pageSize).then(
                function (response) { return response.data; });
        };

        this.createLabProfile = function (profileDetails, type, GroupID) {
            return $http.post(apiUrl + "/VEProfiles/CreateLabProfile?Type=" + type + "&GroupID=" + GroupID, profileDetails,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.bindLabActivities = function (addLabActivity) {
            return $http.post(apiUrl + "/VEProfiles/AddLabActivities", addLabActivity)
                .then(function (response) { return response.data; });
        };

        this.updateLabActivities = function (addLabActivity) {
            return $http.put(apiUrl + "/VEProfiles/UpdateLabActivities", addLabActivity)
                .then(function (response) { return response.data; });
        };

        this.deleteLabProfile = function (modalData) {
            return $http.delete(apiUrl + "/VEProfiles/DeleteLabProfile?VEProfileID=" + modalData.VEProfileID + "&GroupID=" + modalData.GroupID)
                .then(function (response) { return response.data; });
        };

        this.uploadImage = function (image) {
            return Upload.upload({
                url: apiUrl + '/File',
                data: { file: image }
            }).then(function (response) {
                return response.data;
            });
        };

        this.getLabProfileById = function (labProfileId) {
            return $http.get(apiUrl + "/VEProfiles/GetProfile?VEProfileID=" + labProfileId)
                .then(function (response) { return response.data; });
        };

        this.toggleVM = function (veProfileId, userID, started) {
            return $http.get(apiUrl + "/VirtualMachines/ToggleVM?userID=" + veProfileId + "&veProfileId=" + userID + "&started=" + started)
                .then(function (response) { return response.data; });
        };

        this.startVM = function (userID, veProfileId) {
            return $http.get(apiUrl + "/VirtualMachines/StartVM?userID=" + userID + "&veProfileId=" + veProfileId + "&groupId=" + currentUserGroup)
                .then(function (response) { return response.data; });
        };

        this.roleCloudOptions = function (role) {
            return $http.get(apiUrl + "/UserManagement/GetUserRoleCloudLabs?role=" + role)
                .then(
                    function mySuccess(response) {
                        return response.data;
                    });
        };

        this.GetUsersCloudLabs = function (id, role, groupId) {
            return $http.get(apiUrl + "/UserManagement/GetUsersCloudLabs?id=" + id + "&role=" + role + "&groupId=" + groupId).then(
                function (response) {
                    return response.data;
                });
        };

        this.createUser = function (user) {
            return $http.post(authUrl + "/api/Account/create?model=", user,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(
                    function (response) { return response.data; });
        };

        this.editUser = function (user) {
            return $http.post(authUrl + "/api/Account/EditProfile?editModel=", user,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(
                    function (response) { return response.data; });
        };

        this.disableUser = function (id, isDisable) {
            return $http.delete(authUrl + "/api/Account/Disable?userId=" + id + "&isDisable=" + isDisable).then(
                function (response) { return response.data; });
        };

        this.deleteUser = function (id) {
            return $http.delete(authUrl + "/api/Account/Delete?userId=" + id).then(
                function (response) { return response.data; });
        };

        this.checkMachineLogs = function (comment, userId, veProfileId) {
            return $http.delete(authUrl + "/api/VirtualMachines/GetLogs?Comment=" + comment + "&userID=" + userId + "&veProfileId=" + veProfileId).then(
                function (response) { return response.data; });
        };

        this.shutdownVm = function (courseId, veProfileId, userId) {
            return $http.post(apiUrl + "/VirtualMachines/ShutdownVM?courseId=" + courseId + "&veProfileId=" + veProfileId + "&userId=" + userId + "&groupId=" + currentUserGroup).then(
                function (response) { return response.data; });
        };

        this.getLabSchedule = function (veProfileId, userId) {
            return $http.get(apiUrl + "/VEProfiles/GetLabSchedules?veprofileId=" + veProfileId + "&userId=" + userId).then(
                function (response) { return response.data; });
        };

        this.logTime = function (veProfileId, userId, isInstructor) {
            return $http.post(apiUrl + "/VEProfiles/LogTime?veprofileId=" + veProfileId + "&userId=" + userId + "&isInstructor=" + isInstructor).then(
                function (response) { return response.data; });
        };

        this.getRoleIdByName = function (roleNames) {
            return $http.get(apiUrl + "/UserManagement/GetRoleIdByName?roleName=" + roleNames).then(
                function (response) { return response.data; });
        };

        this.getUsersByRoleId = function (roleId, VEProfileID, GroupID) {
            return $http.get(apiUrl + "/UserManagement/GetUsersByRoleId?roleId=" + roleId + "&VEProfileID=" + VEProfileID + "&GroupID=" + GroupID).then(
                function (response) { return response.data; });
        };

        this.checkTotalLabCredits = function (GroupID, VEProfileID) {
            return $http.get(apiUrl + "/VEProfiles/GetTotalLabCredits/?GroupID=" + GroupID + "&VEProfileID=" + VEProfileID).then(
                function (response) { return response.data; });
        };

        this.getRemainingLabCredits = function (VEProfileId) {
            return $http.get(apiUrl + "/VEProfiles/GetRemainingLabCredits/?VEProfileID=" + VEProfileId).then(
                function (response) { return response.data; });
        };

        this.getLabHoursById = function (userId, VEProfileID) {
            return $http.get(apiUrl + "/VEProfiles/GetLabHoursByUserId/?userId=" + userId + "&VEProfileID=" + VEProfileID).then(
                function (response) { return response.data; });
        };

        //this.createCloudLabsSchedule = function (contents, isLaas) {
        //    return $http.post(apiUrl + "/VEProfiles/ProvisionMachines/?isLaas="+ isLaas +"&contents=", contents,
        //        {
        //            headers: { 'Content-Type': 'application/json' }
        //        }
        //    ).then(
        //        function (response) { return response.data; });
        //};
        this.createCloudLabsSchedule = function (contents, isLaas, currentEmail) {
            return $http.post(apiUrl + "/VEProfiles/MachineProvision/?isLaas=" + isLaas + "&schedBy=" + currentEmail + "&contents=", contents,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };

        this.grantConsoleAccess = function (contents) {
            return $http.post(apiUrl + "/VEProfiles/GrantConsoleAccess/?contents=", contents,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        }

        this.uploadImage1 = function (image, imageFileName) {
            return Upload.upload({
                url: apiUrl + 'LabActivities/UploadThumbnailLabActivities?imageFilename=' + imageFileName,
                data: {
                    file: image
                }
            }).then(function (response) {
                return response.data;
            });
        };

        //--------------------------------My Functions--------------------------
        this.createVirtualEnvironments = function (VEInfo) {
            return $http.post(apiUrl + "/virtualEnvironments/CreateVirtualEnvironments", VEInfo,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.readVirtualEnvironments = function () {
            return $http.get(apiUrl + "/virtualEnvironments/ReadVirtualEnvironments")
                .then(function (response) { return response.data; });
        };

        this.updateVirtualEnvironments = function (VEInfo) {
            return $http.post(apiUrl + "/virtualEnvironments/UpdateVirtualEnvironments", VEInfo,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.getAspNetRole = function () {
            return $http.get(apiUrl + "/UserManagement/GetAspNetRole").then(
                function (response) { return response.data; });
        };



        this.updateAspNetRole = function (AspNetRole) {
            return $http.post(apiUrl + "/UserManagement/UpdateAspNetRole", AspNetRole,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.getVETypes = function () {
            return $http.get(apiUrl + "/VETypes")
                .then(function (response) { return response.data; });
        };

        this.getCloudProviders = function () {
            return $http.get(apiUrl + "/CloudProviders")
                .then(function (response) { return response.data; });
        };

        this.getVMConfig = function () {

            return $http.get(apiUrl + "/VirtualMachines/GetVMConfig")
                .then(function (response) { return response.data; });
        };

        this.setVMConfig = function (TimeInfo) {
            return $http.post(apiUrl + "/VirtualMachines/SetVMConfig", TimeInfo,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.deleteVirtualEnvironments = function (ID) {
            return $http.delete(apiUrl + "/virtualEnvironments/DeleteVirtualEnvironments?id=" + ID)
                .then(function (response) { return response.data; });
        };

        this.getVirtualEnvironmentImages = function () {
            return $http.get(apiUrl + "/VirtualEnvironmentImages/GetImagesFromAzure")
                .then(function (response) { return response.data; });
        };

        this.getFilteredVirtualEnvironmentImages = function (Title) {
            return $http.get(apiUrl + "/VirtualEnvironmentImages/GetFilteredImagesFromAzure?Title=" + Title + "&userGroup=" + currentUserGroup)
                .then(function (response) { return response.data; });
        };

        this.addVirtualEnvironmentImages = function (ImageInfo) {
            return $http.post(apiUrl + "/VirtualEnvironmentImages/AddImage", ImageInfo,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.editVirtualEnvironmentImages = function (ImageInfo) {
            return $http.post(apiUrl + "/VirtualEnvironmentImages/EditImage", ImageInfo,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.deleteVirtualEnvironmentImages = function (VirtualEnvironmentID) {
            return $http.delete(apiUrl + "/VirtualEnvironmentImages/DeleteImage?VirtualEnvironmentID=" + VirtualEnvironmentID)
                .then(function (response) { return response.data; });
        };

        this.getVEIDFromVEImages = function (VirtualEnvironmentID) {
            return $http.get(apiUrl + "/VirtualEnvironmentImages/GetVirtualEnvironmentImagesID?VirtualEnvironmentID=" + VirtualEnvironmentID)
                .then(function (response) { return response.data; });
        };

        this.addTenant = function (TenantModel) {
            return $http.post(apiUrl + "/Tenant/AddTenant", TenantModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.getTenantEntries = function (code) {
            return $http.get(apiUrl + "/Tenant/GetTenant?code=" + code)
                .then(function (response) { return response.data; });
        };

        this.editTenant = function (TenantModel) {
            return $http.post(apiUrl + "/Tenant/EditTenant", TenantModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.deleteTenant = function (TenantId) {
            return $http.delete(apiUrl + "/Tenant/DeleteTenant?TenantId=" + TenantId)
                .then(function (response) { return response.data; });
        };

        this.addUserGroup = function (UserGroupModel) {
            return $http.post(apiUrl + "/CloudLabsGroups/CreateCloudLabsGroup", UserGroupModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.editUserGroup = function (UserGroupModel) {
            return $http.post(apiUrl + "/CloudLabsGroups/EditCloudLabsGroup", UserGroupModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };
        this.deleteUserGroup = function (UserGroupId, GroupName) {
            return $http.delete(apiUrl + "/CloudLabsGroups/DeleteCloudLabsGroup?CloudlabsGroupID=" + UserGroupId + "&GroupName=" + GroupName)
                .then(function (response) { return response.data; });
        };

        this.getUserGroups = function () {
            return $http.get(apiUrl + "/CloudLabsGroups/GetCloudLabsGroup")
                .then(function (response) { return response.data; });
        };

        this.editLabCreditRecord = function (LabCreditModel) {
            return $http.post(apiUrl + "/VEProfiles/EditLabCreditRecord", LabCreditModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.isUserGroupActive = function (id) {
            return $http.get(apiUrl + "/UserManagement/GetUserGroupExist?id=" + id)
                .then(function (response) { return response.data; });
        };

        this.getTenantID = function (ApiUrl) {
            return $http.get(apiUrl + "/Tenant/GetTenantID?ApiUrl=" + ApiUrl)
                .then(function (response) { return response.data; });
        };

        this.GetUserSchedulePerUserGroup = function (groupid, veprofileid) {
            return $http.get(apiUrl + "/CloudLabsGroups/GetUserSchedulePerUserGroup?CloudlabsGroupID=" + groupid + "&veprofileid=" + veprofileid)
                .then(function (response) { return response.data; });
        };

        this.GetMappingLabProfileUserGroup = function (GroupID, VEProfileID) {
            return $http.get(apiUrl + "/VEProfiles/GetMappingLabProfileUserGroup?GroupID=" + GroupID + "&VEProfileID=" + VEProfileID)
                .then(function (response) { return response.data; });
        };

        this.timeTrigger = function (UserId, VEProfileId, DateTime, Mode) {
            return $http.post(apiUrl + "/VirtualMachines/TimeTrigger?UserId=" + UserId + "&VEProfileID=" + VEProfileId + "&DateTime=" + DateTime + "&Mode=" + Mode)
                .then(function (response) { return response.data; });
        };

        this.IsCourseByGroupProvisioned = function (GroupID, VEProfileID) {
            return $http.get(apiUrl + "/VEProfiles/IsCourseByGroupProvisioned?GroupID=" + GroupID + "&VEProfileID=" + VEProfileID)
                .then(function (response) { return response.data; });
        };

        this.groupLabs = function (cloudlabsgroupid) {
            return $http.get(apiUrl + "/CloudLabsGroups/GroupLabs?CloudLabsGroupId=" + cloudlabsgroupid)
                .then(function (response) { return response.data; });
        };

        this.CheckTotalSubscriptionCredit = function (cloudlabsgroupid) {
            return $http.get(apiUrl + "/CloudLabsGroups/CheckTotalSubscriptionCredit?CloudLabsGroupID=" + cloudlabsgroupid)
                .then(function (response) { return response.data; });
        };

        this.editSubsciptionCredit = function (LabCreditModel) {
            return $http.post(apiUrl + "/VEProfiles/EditSubsciptionCredit", LabCreditModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.addSubsciptionCredit = function (LabCreditModel) {
            return $http.post(apiUrl + "/VEProfiles/AddSubsciptionCredit", LabCreditModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.saveGroupLabs = function (creditGroupModel, subscriptionRemaining) {
            return $http.post(apiUrl + "/CloudLabsGroups/SaveGroupLabs?subscriptionRemaining=" + subscriptionRemaining, creditGroupModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };

        this.getUserGroupName = function (CloudLabsGroupID) {
            return $http.get(apiUrl + "/CloudLabsGroups/GetUserGroupName?CloudLabsGroupID=" + CloudLabsGroupID)
                .then(function (response) { return response.data; });
        };
        this.DataUsers = function (CloudLabsGroupID, VEProfileID) {
            return $http.get(apiUrl + "/CloudLabsGroups/DataUsers?CloudlabsGroupID=" + CloudLabsGroupID + "&VEProfileID=" + VEProfileID)
                .then(function (response) { return response.data; });
        };


        this.DataCourse = function (groupid, veprofileid) {
            return $http.get(apiUrl + "/CloudLabsGroups/DataCourse?CloudLabsGroupId=" + groupid + "&isSuperAdmin=" + userIsSuperAdmin)
                .then(function (response) { return response.data; });
        };
        this.CheckIfShuttingDown = function (userId) {
            return $http.get(apiUrl + "/VirtualMachines/CheckIfShuttingDown?userId=" + userId + "&veprofileId=" + veprofileId).then(
                function (response) { return response.data; });
        };

        this.getGradeUser = function (role, veprofileid, usergroupid, groupCode, VEDescription) {
            return $http.get(apiUrl + "/CourseGrade/GetGradeUser?role=" + role + "&veprofileId=" + veprofileid + "&usergroupid=" + usergroupid + "&groupCode=" + groupCode + "&VEDescription=" + VEDescription).then(
                function (response) { return response.data; });
        };

        this.createUserGrade = function (veprofileid, usersGrade) {
            return $http.post(apiUrl + "/CourseGrade/CreateUserGrade?veprofileid=" + veprofileid, usersGrade,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data; });
        };
        this.imageUpload = function (image, imageFileName, id, groupCode) {
            return Upload.upload({
                url: apiUrl + 'CourseGrade/ImageUpload?imageFilename=' + imageFileName + '&id=' + id + "&groupCode=" + groupCode,
                data: {
                    file: image
                }
            }).then(function (response) {
                return response.data;
            });
        };
        this.UploadLabAnswerKey = function (image, PDFFilename, labactivityid) {
            return Upload.upload({
                url: apiUrl + 'LabActivities/UploadLabAnswerKey?PDFFilename=' + PDFFilename + "&labactivityid=" + labactivityid,
                data: {
                    file: image
                }
            }).then(function (response) {
                return response.data;
            });
        };
        this.uploadtestken = function (image) {
            return Upload.upload({
                url: 'http://localhost:56067/api/account/BulkUserCreation?strFilePath=dsadas',
                data: {
                    file: image
                }
            }).then(function (response) {
                return response.data;
            });
        };
        this.getRegions = function () {
            return $http.get(apiUrl + "Regions/GetRegions")
                .then(function (response) {
                    return response.data;
                });
        };
        this.getGCPRegions = function () {
            return $http.get(apiUrl + "GCP/GetRegions")
                .then(function (response) {
                    return response.data;
                });
        };

        this.provisionAWSConsole = function (veProfileId, veTypeId) {
            return $http.get(apiUrl + "VEProfiles/ProvisionAWSConsole?email=" + currentEmail + "&userId=" + currentUserId + "&veprofileId=" + veProfileId)
                .then(function (response) {
                    return response.data;
                });
        };

        this.getConsoleLab = function () {
            return $http.get(apiUrl + "VEProfiles/GetConsoleLab")
                .then(function (response) {
                    return response.data;
                });
        };

        this.provisionAlibabaConsole = function (veProfileId, veTypeId) {
            return $http.get(apiUrl + "VEProfiles/ProvisionAlibabaConsole?email=" + currentEmail + "&userId=" + currentUserId + "&veprofileId=" + veProfileId)
                .then(function (response) {
                    return response.data;
                });
        };

        this.getLocations = function () {
            return $http.get("https://cs-apac-devopsassets-d-core-api.azurewebsites.net/labs/location")
                .then(function (response) {
                    return response.data;
                });
        };

        this.checkIfClientExist = function (clientCode, projectName) {
            return $http.get(apiUrl + "/Tenant/CheckTenant?clientCode=" + clientCode + "&projectName=" + projectName)
                .then(function (response) {
                    return response.data;
                });
        };

        this.addTenantNew = function (tenant) {
            return $http.post(apiUrl + "/Tenant/CreateTenant", tenant,
                {
                    headers: { 'Content-Type': 'application/json' }
                });
        }

        this.updateTenantNew = function (tenant) {
            return $http.post(apiUrl + "/Tenant/UpdateTenant", tenant,
                {
                    headers: { 'Content-Type': 'application/json' }
                });
        }

        this.getTenantCodes = function (tenant) {
            return $http.get(apiUrl + "/Tenant/GetTenantCodes")
                .then(function (response) {
                    return response.data;
                });
        };

        this.getTenantValues = function (clientCode) {
            return $http.get(apiUrl + "/Tenant/GetTenantValues?clientCode=" + clientCode)
                .then(function (response) {
                    return response.data;
                });
        };

        this.getUserGroupAzCode = function (environment, currentCode) {
            return $http.get(apiUrl + "/CloudLabsGroups/GetUserGroupAzCode?Environment=" + environment + "&currentCode=" + currentCode)
                .then(function (response) { return response.data; });
        };

        this.provisionVM = function (dataContents, isSelfProv) {
            return $http.post(apiUrl + "/MachineLabs/ProvisionVM?schedBy=" + currentEmail + "&tenantId=" + userTenantId + "&selfProv=" + isSelfProv + "&token=" + GCPAccToken + "&data=", dataContents,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };

        this.checkConsoleUser = function (consoleUsers) {
            return $http.post(apiUrl + "/VEProfiles/CheckConsoleUser?consoleUsers=", consoleUsers,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.saveGrantUser = function (contents) {
            return $http.post(apiUrl + "/VEProfiles/SaveGrantUser?GrantedBy=" + currentUserId + "&contents=", contents,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.saveVMUser = function (contents) {
            return $http.post(apiUrl + "/VEProfiles/saveVMUser?contents=", contents,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.getUserConsoleConsumption = function (userId, veProfileId) {
            return $http.get(apiUrl + "ConsoleSchedules/SpecificConsoleUser/?userId=" + userId + "&veprofileId=" + veProfileId)
                .then(function (response) {
                    return response.data;
                });
        };
        this.triggerIsSuspended = function (contents) {
            return $http.post(apiUrl + "ConsoleSchedules/IsSuspendedTriggered/?contents=", contents,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.getAWSConsoleDetails = function (accountId, startDate, endDate) {
            return $http.get(apiUrl + "ConsoleSchedules/getAWSConsoleDetails?accountId=" + accountId + "&startDate=" + startDate + "&endDate=" + endDate)
                .then(function (response) {
                    return response.data;
                });
        };
        this.triggerIsUpdateLimit = function (accountId, budgetLimit, limitType) {
            return $http.post(apiUrl + "Consoleschedules/UpdateBudget?accountId=" + accountId + "&budgetLimit=" + budgetLimit + "&limitType=" + limitType,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };

        this.getCourseVMs = function (userId) {
            return $http.get(apiUrl + "/MachineLabs/GetCourseVMs?userId=" + userId + "&tenantId=" + userTenantId).then(
                function (response) { return response.data; });
        };
        this.vmOperation = function (resourceId, operation, role) {
            return $http.post(apiUrl + "/MachineLabs/VMOperation?resourceID=" + resourceId + "&operation=" + operation + "&tenantId=" + userTenantId + "&role=" + role +"&isClick=true",
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.machineLogs = function (resourceId, requestId, status) {
            return $http.post(apiUrl + "/MachineLogs/Logs?resourceID=" + resourceId + "&requestId=" + requestId + "&status=" + status,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.checkVMUser = function (VEProfileID, pageIndex, pageSizeSelected, VMStatus, Search) {
            return $http.post(apiUrl + "/MachineLabs/checkVMUser?groupId= " + currentUserGroup + "&pageIndex= " + pageIndex + "&pageSize= " + pageSizeSelected + "&VEProfileID=" + VEProfileID + "&VMStatus=" + VMStatus + "&Search=" + Search,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.checkVMAdminStaff = function (VEProfileID, pageIndex, pageSizeSelected) {
            return $http.post(apiUrl + "/MachineLabs/checkVMAdminStaff?groupId= " + currentUserGroup + "&pageIndex= " + pageIndex + "&pageSize= " + pageSizeSelected + "&VEProfileID=" + VEProfileID,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };
        this.getVMSize = function (userId) {
            return $http.get(apiUrl + "/MachineLabs/GetVMSize?tenantId=" + userTenantId).then(
                function (response) { return response.data; });
        };

        this.toggleEmailVerification = function (userid) {
            return $http.post(apiUrl + "UserManagement/ConfirmEmail?userid=" + userid,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        }

        this.courseAvailable = function (groupId) {
            return $http.get(apiUrl + "/VEProfiles/CourseAvailable?groupId=" + groupId).then(
                function (response) { return response.data; });
        };

        this.sendRequestCourse = function (course, note) {
            return $http.get(apiUrl + "/VEProfiles/SendRequestCourse?email=" + currentEmail + "&course=" + course + "&groupName=" + groupName + "&note=" + note).then(
                function (response) { return response.data; });
        };

        /**
         * Retrieves the list of lab hour extension types
         * 
         */
        this.getExtensionTypes = function () {
            return $http.get(apiUrl + "/LabHourExtension/GetExtensionTypes").then(
                function (response) { return response.data; });
        }

        /**
         * Retrieves a list of users with machines (users enrolled to a course)
         * @param {any} veprofileid
         */
        this.getUsersWithLabHourExtensions = function (veprofileid) {
            return $http.get(apiUrl + "/LabHourExtension/GetUsersWithLabHourExtensions?veprofileid=" + veprofileid).then(
                function (response) { return response.data; });
        }

        /**
         * Retrieves a list of users enrolled to a course and their custom lab hour extensions
         * parameter: { 
         *              EearchText
         *              StartDate
         *              EndDate
         *              SortDirection
         *              SortField
         *              VEProfileId
         *            }
         */
        this.getUsersWithCustomLabHourExtensions = function (request) {
            return $http.post(apiUrl + "/LabHourExtension/GetUsersWithCustomLabHourExtensions",
                request,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(function (response) { return response.data; });
        }

        /**
         * Retrieves a list of users enrolled to a course and their fixed lab hour extensions
         * parameter: {
         *              SearchText
         *              StartDate
         *              EndDate
         *              SortDirection
         *              SortField
         *              VEProfileId
         *            }
         */
        this.getUsersWithFixedLabHourExtensions = function (request) {
            return $http.post(apiUrl + "/LabHourExtension/GetUsersWithFixedLabHourExtensions",
                request,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(function (response) { return response.data; });
        }

        /**
         * Saves the lab hour extension for the course
         * @param {any} request
         */
        this.saveLabHourExtension = function (request) {
            return $http.post(apiUrl + "/LabHourExtension/Save",
                request,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(function (response) { return response.data; });
        }

        /**
         * Deletes the lab hour extension by Id
         * @param {any} labHourExtensionId
         */
        this.deleteLabHourExtensionById = function (request) {
            return $http.post(apiUrl + "/LabHourExtension/Delete",
                {
                    LabHourExtensionId: request.LabHourExtensionIds[0],
                    EditedByUserId: request.EditedByUserId
                },
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(function (response) { return response.data; });
        }

        /**
         * Updates the lab hour extension
         * @param {any} request
         */
        this.updateLabHourExtension = function (request) {
            return $http.post(apiUrl + "/LabHourExtension/UpdateV2",
                request,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(function (response) { return response.data; });
        }

        this.getLabHours = function (resourceId) {
            return $http.get(apiUrl + "/MachineLabs/GetLabHours?resourceId=" + resourceId).then(
                function (response) { return response.data; });
        }

        this.getUserIfUserIsValid = function (currentUserIdLTI) {
            return $http.get(apiUrl + "/UserManagement/GetUserIfUserIsValid?userIdLTI=" + currentUserIdLTI).then(
                function (response) { return response.data; });
        };
        this.editUserLTI = function (user) {
            return $http.post(authUrl + "/api/Account/EditFromLTI?editModel=", user,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(
                    function (response) { return response.data; });
        };

        this.setRunningBy = function (resourceId, runningBy) {
            return $http.post(apiUrl + "/MachineLabs/setRunningBy?ResourceId=" + resourceId + "&runningBy="+ runningBy).then(
                function (response) { return response.data; });
        };
        this.getAWSVMSize = function (userId) {
            return $http.get(apiUrl + "/MachineLabs/GetAWSVMSize").then(
                function (response) { return response.data; });
        };
        this.getUserImage = function (id, groupCode, VEDescription, veprofileid) {
            return $http.get(apiUrl + "/CourseGrade/GetUserImage?id=" + id + "&groupCode=" + groupCode + "&VEDescription=" + VEDescription + "&veprofileid=" + veprofileid).then(
                function (response) { return response.data; });
        };
        this.getGuacDNS = function (resourceId) {
            return $http.get(apiUrl + "/MachineLabs/GetGuacDNS?resourceId=" + resourceId).then(
                function (response) { return response.data; });
        };
        this.GetCourseNoStatus = function (userId) {
            return $http.get(apiUrl + "/MachineLabs/GetCourseVMs?userId=" + userId + "&tenantId=" + userTenantId).then(
                function (response) { return response.data; });
        };
        this.setIpAddress = function (resourceId, ipAddress) {
            return $http.post(apiUrl + "/MachineLabs/SetIpAddress?resourceId=" + resourceId + "&ipAddress=" + ipAddress).then(
                function (response) { return response.data; });
        };

        this.getIpIfExist= function (resourceId, ipAddress) {
            return $http.get(apiUrl + "/MachineLabs/IsIpExist?resourceId=" + resourceId).then(
                function (response) { return response.data; });
        };

        this.getCloudLabsForAutoDeletion = function (UserGroupModel) {
            return $http.get(apiUrl + "AutoDeletions/GetCloudLabsForAutoDeletion")
                .then(function (response) { return response.data; });
        };

        this.addorEditScheduleForAutoDeletion = function (AutoDeletionModel) {
            return $http.post(apiUrl + "AutoDeletions/AddorEditScheduleForAutoDeletion", AutoDeletionModel,
                {
                    headers: { 'Content-Type': 'application/json' }
                })
                .then(function (response) { return response.data });
        };

        this.getBusinessType = function () {
            return $http.get(apiUrl + "/BusinessType/GetBusinessType")
                .then(function (response) { return response.data; });
        };

        this.checkIfUserIdHasScheduleForAutoDeletion = function (userGroupId) {
            return $http.post(apiUrl + "AutoDeletions/CheckIfUserIdHasScheduleForAutoDeletion/" + userGroupId)
                .then(function (response) { return response.data });
        }
        this.getStorageAccountName = function () {
            return $http.get(apiUrl + "VirtualEnvironmentImages/GetStorageAccountName?tenantId=" + userTenantId)
                .then(function (response) { return response.data });
        }

        this.getImageTemplate = function (description, storageConnectionString, storageKey, storageAccountName) {
            return $http.get(apiUrl + "/VirtualEnvironmentImages/getImageTemplate?Description=" + description + "&storageConnectionString=" + storageConnectionString + "&storageKey=" + storageKey +"&storageAccountName=" + storageAccountName)
                .then(function (response) { return response.data; });
        };

        this.uploadBulkCreateUser = function (bulkfile) {
            return Upload.upload({
                url: authUrl + '/api/account/BulkCreate?createdBy=' + name + "&userGroupId=" + currentUserGroup + "&tenantId=" + userTenantId,
                data: { file: bulkfile }
            }).then(function (response) {
                return response.data;
            });
        };

        this.reProvisionVM = function (userId, veProfileId) {
            return $http.post(apiUrl + "/MachineLabs/ReProvisionVM?userId=" + userId + "&veProfileId=" + veProfileId + "&tenantId=" + userTenantId ,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };

        this.uploadBulkProvision = function (bulkfile, veprofileId) {
            return Upload.upload({
                url: apiUrl + "/MachineLabs/BulkProvisionVM?veprofileId=" + veprofileId + "&schedBy=" + currentEmail,
                data: { file: bulkfile }
            }).then(function (response) {
                return response.data;
            });
        };

        this.getBusinessGroup = function () {
            return $http.get(apiUrl + "/BusinessGroup/GetBusinessGroup")
                .then(function (response) { return response.data; });
        };

        this.saveBusinessGroup = function (CreateEditBusinessGroup) {
            return $http.post(apiUrl + "/BusinessGroup/saveBusinessGroup?bg=", CreateEditBusinessGroup,
                {
                    headers: { 'Content-Type': 'application/json' }
                }
            ).then(
                function (response) { return response.data; });
        };

        this.checkIfTheresMachine = function (userGroupName) {
            return $http.get(apiUrl + "/BusinessGroup/CheckIfTheresMachine?userGroupName=" + userGroupName)
                .then(function (response) { return response.data; });
        };

        this.checkIfTheresMachineByClientCode = function (clientCode) {
            return $http.get(apiUrl + "/BusinessGroup/checkIfTheresMachineByClientCode?clientCode=" + clientCode)
                .then(function (response) { return response.data; });
        };
        this.getBusinessIdById = function (businessId) {
            return $http.get(apiUrl + "/BusinessType/getBusinessIdById?businessId=" + businessId)
                .then(function (response) { return response.data; });
        };

        this.getUserByUserGroupId = function (userGroupId) {
            return $http.get(apiUrl + "/CloudLabsGroups/getUserByUserGroupId?CloudlabsGroupID=" + userGroupId)
                .then(function (response) { return response.data; });
        };

        this.getProject = function () {
            return $http.get(apiUrl + "/gcp/GetProject")
                .then(function (response) { return response.data; });
        };
        this.getVPCGCP = function () {
            return $http.get(apiUrl + "/gcp/GetVPCGCP")
                .then(function (response) { return response.data; });
        };
        this.getSubGCP = function () {
            return $http.get(apiUrl + "/gcp/GetSubGCP")
                .then(function (response) { return response.data; });
        };
        this.getProjectFamily = function () {
            return $http.get(apiUrl + "/gcp/GetProjectFamily?tenantId=" + userTenantId)
                .then(function (response) { return response.data; });
        };
        this.getAMI = function (family) {
            return $http.get(apiUrl + "/gcp/GetAMI?family=" + family)


                .then(function (response) { return response.data; });
        };
        //this.checkDiskSize = function (VirtualEnvironmentID) {
        //    return $http.get(apiUrl + "/VirtualEnvironmentImages/CheckDiskSize?VirtualEnvironmentID=" + VirtualEnvironmentID)
        //        .then(function (response) { return response.data; });
        //};
        this.getMachineType = function (zone) {
            return $http.get(apiUrl + "/gcp/GetMachineType?zone=" + zone)
                .then(function (response) { return response.data; });
        };
        this.getTenantGCPRegion = function (TenantId) {
            return $http.get(apiUrl + "/Tenant/GetTenantGCPRegion?TenantId=" + TenantId)
                .then(function (response) { return response.data; });
        };
        this.getVirtualEnvironmentsByVeId = function (vetypeId) {
            return $http.get(apiUrl + "/VirtualEnvironments/GetVirtualEnvironmentsByVeId?vetypeId=" + vetypeId)
                .then(function (response) { return response.data; });
        };
    }

})();