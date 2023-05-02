(function () {
    "use strict";
    angular.module('app-configuration')
        .controller('configurationModalController', configurationModalController);

    configurationModalController.$inject = ['$scope', '$uibModalInstance', 'svc', 'data', '$uibModal', '$filter'];

    function configurationModalController($scope, $uibModalInstance, svc, data, $uibModal, $filter) {
        //$scope.isEditAutoDeletion = null;
        $scope.apiurl = apiUrl;
        $scope.min = 1;
        $scope.max = 1000;
        var userGroupState;
        $scope.env = {};
        $scope.IsCourseByGroupProvisionedFlag = true;
        $scope.currentTotalLabHoursCredit;
        $scope.TotalRemainingHours;
        var currentTenantId = parseInt(userTenantId);
        $scope.isInputLower = true;
        $scope.names = [{ "name": "Staging", "code": "D" }, { "name": "QA", "code": "QA" }, { "name": "Demo", "code": "DMO" }, { "name": "Prod", "code": "PRD" }];
        //$scope.SubscriptionHourCredits = "";
        $scope.$on('LOAD', function () { $scope.loader = true; $scope.oppositeLoader = false; });
        $scope.$on('UNLOAD', function () { $scope.loader = false; $scope.oppositeLoader = true; });

        $scope.NeedConfirmation = true;
        $scope.tempArray = [];
        $scope.prefix = [];
        $scope.alwaysTrue = true;
        $scope.GuacConnection = "";
        $scope.GuacamoleURL = "";
        $scope.RegionName = "";
        $scope.BusinessEditName = "";
        $scope.RegionsGCPEdit = "";
        $scope.ProjectNameEdit = "";
        $scope.VPCNetworkNameEdit = "";
        $scope.SubNetworkEdit = "";
        //$scope.GuacConnection = "user id=guacamole_user;password=CloudSwyft2020!;server=csguacservercray.southeastasia.cloudapp.azure.com;port=3306;database=guacamole_db;connectiontimeout=3000;defaultcommandtimeout=3000;protocol=Socket";
        //$scope.GuacamoleURL = "https://csguacservercray.cloudswyft.com";

        $scope.environmentImageLoad = false;
        //$scope.locs = [{ "location": "SEA", "region": "southeastasia" },
        //{ "location": "AUE", "region": "australiaeast" },
        //{ "location": "EUS2", "region": "eastus2" },
        //{ "location": "CUS", "region": "centralus2" },
        //{ "location": "WUS2", "region": "westus2" },
        //{ "location": "NEU", "region": "northeurope" }];
        var isCSaccountAdmin = true;

        $scope.isVMExist = true;
        $scope.isDisabled = true;
        $scope.isValidityDisabled = true;
        $scope.isDisabledTypeOfBusiness = false;


        $scope.businessType = [];
        $scope.businessTypeList = [];
        $scope.businessGroup = [];
        $scope.businessUserGroupName = "";
        $scope.businessName = "";
        $scope.validity = "";
        var tempValidity;
        $scope.tempIsDisabled = true;

        $scope.isAzure = false;
        $scope.isAWS = false;
        $scope.isGCP = false;
        $scope.addDisabled = false;
        $scope.createDisabled = false;
        $scope.noGCPDisabled = false;
        $scope.GCPDisabled = false;
        $scope.VEDescriptionGCP = [];
        $scope.environment = [{ "EnvCode": "D", "EnvName": "Staging" }, { "EnvCode": "Q", "EnvName": "QA" }, { "EnvCode": "U", "EnvName": "Demo" }, { "EnvCode": "P", "EnvName": "Prod" }];

        if (userIsSuperAdmin && currentEmail.includes("cloudswyft.com"))
            isCSaccountAdmin = true;

        $scope.getTenantCodes = function () {
            svc.getTenantCodes().then(
                function (response) {
                    $scope.tCodes = response;
                });
        };
        
        $scope.getTenantValues = function (clientCode) {
            svc.getTenantValues(clientCode).then(
                function (response) {
                    //$scope.LocationEdit = response[0].Location;
                    $scope.GuacamoleURLTenantEdit = response[0].GuacamoleURL;
                    $scope.GuacConnectionTenantEdit = response[0].GuacConnection;
                    $scope.ApplicationTenantId = response[0].ApplicationTenantId;
                    $scope.SubscriptionId = response[0].SubscriptionId;
                    $scope.ApplicationId = response[0].ApplicationId;
                    $scope.ApplicationTenantId = response[0].ApplicationTenantId;
                    $scope.ApplicationSecretKey = response[0].ApplicationSecretKey;
                    $scope.RegionName = response[0].Regions;
                    $scope.bTypes = response[0].BusinessTypes;
                    $scope.RegionsGCPEdit = response[0].RegionGCP;
                    $scope.ProjectNameEdit = response[0].ProjectName;
                    $scope.VPCNetworkNameEdit = response[0].VPCNetworkGCP;
                    $scope.SubNetworkEdit = response[0].VPCSubNetworkGCP;

                    angular.forEach($scope.businessType, function (value, key) {
                        if (value.BusinessType == response[0].BusinessTypes.BusinessType)
                            $scope.BusinessEditName = $scope.businessType[key];
                    })

                    svc.checkIfTheresMachineByClientCode(clientCode).then(
                        function (response) {
                            $scope.isDisabledTypeOfBusiness = response;
                        });

                    svc.getBusinessIdById(response[0].BusinessId).then(
                        function (response) {
                            $scope.businessName = response;
                        });
                });
        };
        $scope.getPrefix = function (environment) {
            $scope.CLPrefix = null;
            svc.getUserGroupAzCode(environment).then(
                function (response) {
                    $scope.clPrefix = response;
                });
        };

        $scope.getTenantCodes();
        $scope.loadAll = function () {
            if (data.purpose() != "configurationModalEditVirtualEnvironmentImagesController") {
                $scope.$emit('LOAD');
                svc.getVETypes().then(
                    function (response) {
                        response = $filter('orderBy')(response, 'Name');
                        $scope.VEType = response;
                        svc.getRegions().then(
                            function (resp) {
                                $scope.Regions = resp;
                                svc.getGCPRegions().then(
                                    function (respGCP) {
                                        $scope.RegionGCP = respGCP;
                                        svc.getCloudProviders().then(
                                            function (response) {
                                                response = $filter('orderBy')(response, 'Name');
                                                $scope.CloudProvider = response;
                                                svc.getAspNetRole().then(
                                                    function (response) {
                                                        response = $filter('orderBy')(response, 'Name');
                                                        $scope.roleConfig = response;
                                                        //svc.getVMConfig().then(
                                                        //    function (response) {
                                                        //        $scope.timeConfig = response;
                                                        //        $scope.MaxJobIdleTime = $scope.timeConfig.MaxJobIdleTime;
                                                        //        $scope.MaxIdleTime = $scope.timeConfig.MaxIdleTime;
                                                        //        $scope.HeartbeatInterval = $scope.timeConfig.HeartbeatInterval;
                                                        svc.getUserGroups().then(
                                                            function (response) {
                                                                response = $filter('orderBy')(response, 'GroupName');
                                                                $scope.UserGroups = response;
                                                                $scope.tempArray = $scope.UserGroups.map(a => a.GroupName);
                                                                $scope.prefix = $scope.UserGroups.map(b => b.CLPrefix);
                                                                svc.getAllLabProfiles().then(
                                                                    function (response) {
                                                                        response = $filter('orderBy')(response, 'GroupName');
                                                                        $scope.VEProfiles = response;
                                                                        $scope.$emit('UNLOAD');
                                                                        //svc.getRegionDetails().then(
                                                                        //    function (response) {
                                                                        //        $scope.locs = response;
                                                                        //    });
                                                                        //svc.getTenantEntries().then(
                                                                        //    function (response) {
                                                                        //        response = $filter('orderBy')(response, 'TenantName');
                                                                        //        $scope.TenantEntries = response;
                                                                        //        $scope.$emit('UNLOAD');
                                                                        //    });
                                                                    });
                                                            });
                                                    }); //})
                                            });
                                    });
                            })
                    });
            }
            else if (data.purpose() == "configurationModalEditVirtualEnvironmentImagesController") {
                $scope.$emit('LOAD');
                svc.getStorageAccountName().then(
                    function (response) {
                        $scope.StorageAccountName = response;
                        $scope.$emit('UNLOAD');
                    });
            }
            if (data.purpose() == "configurationModalAddTenantNewController") {
                svc.getProject().then(
                    function (respGCP) {
                        $scope.ProjectGCP = respGCP;
                        svc.getVPCGCP().then(
                            function (respGCP) {
                                $scope.VPCGCP = respGCP;
                                svc.getSubGCP().then(
                                    function (respGCP) {
                                        $scope.SubGCP = respGCP;
                                    });
                            });
                    });
            }
            if (data.purpose() == "configurationModalEditTenantNewController") {
                svc.getProject().then(
                    function (respGCP) {
                        $scope.ProjectGCP = respGCP;
                        svc.getVPCGCP().then(
                            function (respGCP) {
                                $scope.VPCGCP = respGCP;
                                svc.getSubGCP().then(
                                    function (respGCP) {
                                        $scope.SubGCP = respGCP;
                                    });
                            });
                    });
            }

            if (data.purpose() == "configurationModalEditVirtualEnvironmentImagesController") {
                svc.getProjectFamily().then(
                    function (respGCP) {
                        $scope.StorageTemplateNameGCP = respGCP;                       
                    });
            }

        };

        $scope.loadAll();

        $scope.DuplicateChecker = false;

        $scope.EnableSaveButton = function () {
            var keepGoing = true;
            angular.forEach($scope.tempArray, function (value) {
                if (keepGoing) {
                    if (value !== null && $scope.UserGroupName !== null) {
                        if (value.toLowerCase() === $scope.UserGroupName.toLowerCase()) {
                            $scope.DuplicateChecker = true;
                            keepGoing = false;
                        } else {
                            $scope.DuplicateChecker = false;
                        }
                    } else {
                        $scope.DuplicateChecker = false;
                    }
                }
            });
        };


        //business types

        $scope.changeBusiness = function () {
            angular.forEach($scope.businessTypeList, function (value, key) {
                if (value.BusinessType.toLowerCase() == $scope.businessName.toLowerCase()) {
                    if ($scope.tempIsDisabled) {
                        if (value.IsCustomizable) {
                            $scope.validity = tempValidity;
                            $scope.isDisabled = true;
                        }
                        else
                            $scope.validity = null;
                    }
                    else if (value.IsCustomizable) {
                        $scope.isValidityDisabled = false;
                        $scope.validity = tempValidity;
                    }
                    else {
                        $scope.isValidityDisabled = true;
                        $scope.validity = null;
                    }

                         
                }
            })
        };


        svc.getBusinessType().then(
            function (response) {
                $scope.businessTypeList = response;
                angular.forEach(response, function (value) {
                    $scope.businessType.push(value);
                });
            });

        svc.getBusinessGroup().then(
            function (response) {
                $scope.businessGroup = response;
            });



        $scope.$watch("businessGroupSelected", function () {    
            $scope.businessName = $scope.businessGroupSelected.BusinessType;
            $scope.validity = $scope.businessGroupSelected.ModifiedValidity;
            $scope.businessUserGroupName = $scope.businessGroupSelected.UserGroupName;
            if ($scope.businessName != null) {
                angular.forEach($scope.businessTypeList, function (value, key) {
                    if (value.BusinessType.toLowerCase() == $scope.businessName.toLowerCase()) {
                        if (value.IsCustomizable)
                            $scope.isDisabled = true;
                        else
                            $scope.isDisabled = false;
                    }
                })
            }
            tempValidity = $scope.validity;
            svc.checkIfTheresMachine($scope.businessUserGroupName).then(
                function (response) {
                    $scope.isDisabled = response;
                    $scope.tempIsDisabled = response;
                });
        });

        ////////

        $scope.close = function () {
            $uibModalInstance.close();
        };



        $scope.save = function () {
            $scope.NeedConfirmation = true;
            var VEInfo;
            if (data.purpose() === "configurationModalController") {
                VEInfo = {
                    Description: $scope.Description,
                    ThumbnailURL: null,
                    VETypeID: $scope.VETypeID.VETypeID,
                    Title: $scope.Title,
                    //CloudProviderID: $scope.CloudProviderID.CloudProviderID,
                    Name: "",
                    //--------info for confirmation modal----------
                    headTitle: "CREATE VIRTUAL ENVIRONMENT",
                    message: "Do you want to save?",
                    type: "create",
                    reason: "createVirtualEnvironment"
                };
            }
            //else if (data.purpose() === "configurationModalAutoDeletion") {
            //    if ($scope.isEditAutoDeletion) {
            //        VEInfo = {
            //            UserGroupId: $scope.userGroupIdForAutoDeletion,
            //            NumberOfDays: $scope.NumberOfDays,
            //            EditedBy: currentUserGroup,
            //            //--------info for confirmation modal----------
            //            headTitle: "EDIT AUTO DELETION",
            //            message: "Update changes?",
            //            type: "edit",
            //            reason: "editAutoDeletion"
            //        }
            //    } else {
            //        VEInfo = {
            //            UserGroupId: $scope.userGroupIdForAutoDeletion,
            //            NumberOfDays: $scope.NumberOfDays,
            //            CreatedBy: currentUserGroup,
            //            //--------info for confirmation modal----------
            //            headTitle: "CREATE AUTO DELETION",
            //            message: "Do you want to save?",
            //            type: "create",
            //            reason: "createAutoDeletion"
            //        }
            //    }



            //}
            else if (data.purpose() === "configurationModalEditVirtualEnvironmentsController") {
                VEInfo = {
                    VirtualEnvironmentID: $scope.VirtualEnvironmentID,
                    Description: $scope.description,
                    ThumbnailURL: null,
                    VETypeID: $scope.VETypeSelectedEdit.VETypeID,
                    Title: $scope.title.Title,
                    //CloudProviderID: $scope.CloudProviderSelectedEdit.CloudProviderID,
                    Name: "",
                    //--------info for confirmation modal----------
                    headTitle: "EDIT VIRTUAL ENVIRONMENT",
                    message: "Update changes?",
                    type: "edit",
                    reason: "editVirtualEnvironment"
                };
            }
            else if (data.purpose() === "configurationModalEditTimeController") {
                VEInfo = {
                    MaxJobIdleTime: $scope.MaxJobIdleTime,
                    MaxIdleTime: $scope.MaxIdleTime,
                    HeartbeatInterval: $scope.HeartbeatInterval,
                    //--------info for confirmation modal----------
                    headTitle: "EDIT TIME",
                    message: "Update changes?",
                    type: "edit",
                    reason: "editTime"
                };
            }
            else if (data.purpose() === "configurationModalRoleController") {
                VEInfo = {
                    Id: $scope.currentRoleName.Id,
                    Name: $scope.newRoleName,
                    //--------info for confirmation modal----------
                    headTitle: "EDIT ROLE",
                    message: "Update changes?",
                    type: "edit",
                    reason: "editRole"
                };
            }
            else if (data.purpose() === "configurationModalEditVirtualEnvironmentImagesController") {
                //if ($scope.VirtualEnvironmentImagesPurpose === "AddImage") {
                //    VEInfo = {
                //        VirtualEnvironmentID: $scope.TitleImages.VirtualEnvironmentID,
                //        //Name: $scope.URLImages[0],
                //        Name: $scope.URLImages,
                //        GroupId: currentUserGroup,
                //        Size: 'small',
                //        Protocol: 'rdp',
                //        Type: 'Windows',
                //        //--------info for confirmation modal----------
                //        headTitle: "VIRTUAL ENVIRONMENT IMAGES",
                //        message: "Create Virtual Environment Images?",
                //        type: "create",
                //        reason: "createVirtualEnvironmentImages"
                //    };
                //}
                //else if (
                    //$scope.VirtualEnvironmentImagesPurpose === "EditImage"//) {
                    VEInfo = {
                        VirtualEnvironmentID: $scope.TitleImages == undefined ? $scope.TitleImagesGCP.VirtualEnvironmentID : $scope.TitleImages.VirtualEnvironmentID,
                        //Name: $scope.URLImages[0],
                        Name: $scope.URLImages == undefined ? "GCP" : $scope.URLImages,
                        ProjectFamily: $scope.ProjFamily == undefined ? "" : $scope.ProjFamily.name,
                        ImageFamily: $scope.AMI == undefined ? "" : $scope.AMI.name,
                        ImageFamilyMinDiskSize: $scope.AMI == undefined ? "" : $scope.AMI.disk_size_gb,
                        GroupId: currentUserGroup,
                        //--------info for confirmation modal----------
                        headTitle: "VIRTUAL ENVIRONMENT IMAGES",
                        message: "Update Virtual Environment Images?",
                        type: "edit",
                        reason: "editVirtualEnvironmentImages"
                    };
              //  }
            }
            else if (data.purpose() === "configurationModalAddTenantController") {
                VEInfo = {
                    ApiUrl: apiUrl,
                    AuthConnectionString: $scope.AuthConnectionString,
                    Code: $scope.SelectTenantName.name === "Staging" ? 'KEN' : $scope.SelectTenantName.name === "QA" ? 'QA' : $scope.SelectTenantName.name === "DEMO" ? 'DMO' : $scope.SelectTenantName.name === "Prod" ? 'PRD' : '',
                    EdxUrl: $scope.EdxUrl,
                    TenantName: $scope.SelectTenantName.name,
                    GuacConnection: $scope.GuacConnection,
                    GuacamoleURL: $scope.GuacamoleURL,                   
                    Regions: $scope.RegionName,                   
                    ApplicationTenantId: $scope.ApplicationTenantId,                   
                    ApplicationId: $scope.ApplicationId,                   
                    ApplicationSecretKey: $scope.ApplicationSecretKey,                   
                    SubscriptionId: $scope.SubscriptionId,
                    BusinessId: $scope.businessName,
                    //--------info for confirmation modal----------
                    headTitle: "TENANT",
                    message: "Create Tenant Info?",
                    type: "create",
                    reason: "createTenant"
                };
            }
            else if (data.purpose() === "configurationModalEditTenantController") {
                VEInfo = {
                    TenantId: $scope.SelectTenantName.TenantId,
                    ApiUrl: $scope.TenantAPIURL,
                    AuthConnectionString: $scope.AuthConnectionString,
                    Code: angular.uppercase($scope.SelectTenantName.code),
                    EdxUrl: $scope.EdxUrl,
                    Regions: $scope.RegionName,   
                    TenantName: $scope.TenantName,
                    GuacConnection: $scope.TenantGuacamoleConnection,
                    GuacamoleURL: $scope.TenantGuacamoleURL,
                    SubscriptionMinutes: 900,
                    AzurePort: 43000,
                    SubscriptionId: $scope.TenantSubscriptionID,
                    //--------info for confirmation modal----------
                    headTitle: "TENANT",
                    message: "Update Tenant Info?",
                    type: "edit",
                    reason: "editTenant"
                };
            }
            else if (data.purpose() === "configurationModalAddUserGroupController") {
                VEInfo = {
                    GroupName: $scope.UserGroupName,
                    OldGroupName: "",
                    EdxUrl: $scope.EdxUrl,
                    OldEdxUrl: "",
                    CLUrl: $scope.CLUrl,
                    OldCLUrl: "",
                    TenantId: currentTenantId,
                    CLPrefix: $scope.CLPrefix.toUpperCase(),
                    ApiPrefix: $scope.ApiPrefix,
                    Environment: $scope.Environment,
                    DateCreated: new Date(),
                    CreatedBy: currentEmail,
                    //TypeOfBusinessId: $scope.SelectedTypeOfBusiness,
                    //--------info for confirmation modal----------
                    headTitle: "USER GROUP",
                    message: "Add User Group?",
                    type: "create",
                    reason: "addUserGroup"
                };

                if ($scope.AddAnotherUser) {
                    $scope.tempArray.push($scope.UserGroupName);
                    $scope.NeedConfirmation = false;
                    //------------Saving code----------------
                    svc.addUserGroup(VEInfo).then(
                        function (response) {
                            $scope.UserGroupName = null;
                            $scope.EdxUrl = null;
                            $scope.CLUrl = null;
                            $scope.CLPrefix = null;
                          //  $scope.SelectedTypeOfBusiness = null;
                        });
                    ///-----------------------------------
                }
                else { $scope.NeedConfirmation = true; }
            }
            else if (data.purpose() === "configurationModalEditUserGroupController") {
                VEInfo = {
                    CloudLabsGroupID: $scope.SelectUserGroupEditName.CloudLabsGroupID,
                    OldGroupName: $scope.SelectUserGroupEditName.GroupName,
                    OldEdxUrl: $scope.SelectUserGroupEditName.EdxUrl,
                    GroupName: $scope.UserGroupName,
                    EdxUrl: $scope.EdxUrl,
                    CLUrl: $scope.CLUrl,
                    CLPrefix: $scope.CLPrefix.toUpperCase(),
                    ApiPrefix: $scope.ApiPrefix,
                   // TypeOfBusinessId: $scope.oldTypeOfBusinessId,
                    //--------info for confirmation modal----------
                    headTitle: "USER GROUP",
                    message: "Edit User Group?",
                    type: "edit",
                    reason: "editUserGroup"
                };
            }
            else if (data.purpose() === "configurationModalLabHoursCreditsController") {
                if (userGroupState === "InitialState") {
                    VEInfo = {
                        CloudLabsGroupID: $scope.SelectSubscriptionCredit.CloudLabsGroupID,
                        GroupName: $scope.SelectSubscriptionCredit.GroupName,
                        SubscriptionHourCredits: $scope.SubscriptionHourCredits,
                        //--------info for confirmation modal----------
                        headTitle: "LAB HOURS CREDIT",
                        message: "Add Lab Credit?",
                        type: "create",
                        reason: "addSubsciptionCredit"
                    };
                }
                else {
                    var ReloadDifference = $scope.TotalLabHours - $scope.currentTotalLabHoursCredit;
                    VEInfo = {
                        CloudLabsGroupID: $scope.SelectSubscriptionCredit.CloudLabsGroupID,
                        GroupName: $scope.SelectSubscriptionCredit.GroupName,
                        SubscriptionHourCredits: $scope.SubscriptionHourCredits,
                        //--------info for confirmation modal----------
                        headTitle: "LAB HOURS CREDIT",
                        message: "Edit Lab Credit?",
                        type: "edit",
                        reason: "editSubsciptionCredit"
                    };
                }
            }
            else if (data.purpose() === "configurationModalAddTenantNewController") {
                VEInfo = {
                    ClientCode: $scope.ClientCode,
                    //Location: $scope.Location,
                    EnvironmentCode: $scope.Environment,
                    CreatedBy: currentEmail, //$scope.ContactEmail,
                    SubscriptionKey: $scope.Primary,
                    TenantKey: $scope.TenantKey,
                    GuacConnection: $scope.GuacConnection,
                    GuacamoleURL: $scope.GuacamoleURL,
                    Regions: $scope.RegionName,
                    ApplicationTenantId: $scope.ApplicationTenantId,
                    ApplicationId: $scope.ApplicationId,
                    ApplicationSecretKey: $scope.ApplicationSecretKey,
                    SubscriptionId: $scope.SubscriptionId,
                    BusinessId: $scope.businessName.BusinessId,
                    ProjectName: $scope.PrjName == "" ? $scope.ProjectName.project_id : $scope.PrjName,
                    VPCNetworkGCP: $scope.VPCName == "" ? $scope.VPCNetworkName.name : $scope.VPCName,
                    VPCSubNetworkGCP: $scope.VPCSubName == "" ? $scope.SubNetwork.name : $scope.VPCSubName,
                    RegionGCP: $scope.RegionsGCP,
                    //--------info for confirmation modal----------
                    headTitle: "TENANT",
                    message: "Create Tenant Info?",
                    type: "create",
                    reason: "createTenantNew"
                };
            }
            else if (data.purpose() === "configurationModalEditTenantNewController") {
                VEInfo = {
                    ClientCode: $scope.ClientCode,
                    GuacConnection: $scope.GuacConnectionTenantEdit,
                    GuacamoleURL: $scope.GuacamoleURLTenantEdit,
                    Regions: $scope.RegionName,
                    ApplicationTenantId: $scope.ApplicationTenantId,
                    ApplicationId: $scope.ApplicationId,
                    ApplicationSecretKey: $scope.ApplicationSecretKey,
                    SubscriptionId: $scope.SubscriptionId,
                    BusinessId: $scope.BusinessEditName.BusinessId,
                    ProjectName: $scope.ProjectNameEdit,
                    VPCNetworkGCP: $scope.VPCNetworkNameEdit ,
                    VPCSubNetworkGCP: $scope.SubNetworkEdit,
                    RegionGCP: $scope.RegionsGCPEdit,
                    //--------info for confirmation modal----------
                    headTitle: "Edit TENANT",
                    message: "Edit Tenant Info?",
                    type: "edit",
                    reason: "editTenantNew"
                };
            }
            else if (data.purpose() === "configurationModalBusinessController") {
                VEInfo = {
                    UserGroupName: $scope.businessUserGroupName,
                    BusinessGroup: $scope.businessName,
                    Validity: $scope.validity,
                    CreatedBy: currentEmail,
                    //--------info for confirmation modal----------
                    headTitle: "Business Type",
                    message: "Are you sure you want to proceed?",
                    type: "create",
                    reason: "createBusinessType"
                };
            }
            if ($scope.NeedConfirmation) {
                var modal = $uibModal.open({
                    templateUrl: '/app/Configuration/Modal/confirmationModal.html',
                    windowClass: 'my-modal-class',
                    controller: "confirmationModalController",
                    controllerAs: '$confmodal',
                    backdrop: 'static',
                    keyboard: true,
                    resolve: {
                        items: function () {
                            return VEInfo;
                        }
                    }
                });
            }
        };

        $scope.TenantAzurePort = "43000";
        $scope.AuthConnectionString = "Data Source=cloudswyft-server.database.windows.net;Initial Catalog=clmp-prod;User ID=cloudswyft;Password=pr0v3byd01n6!";
        //$scope.SubscriptionId = "d2fd4772-79f4-476c-ac6d-9dc11365db22";
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------

        if (data.purpose() === "configurationModalController") {
            $scope.isCreateVirtualEnvironmentVisible = true;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
           // $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "CREATE VIRTUAL ENVIRONMENTS";
        }
        else if (data.purpose() === "configurationModalEditVirtualEnvironmentsController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = true;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
           // $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "EDIT/DELETE VIRTUAL ENVIRONMENTS";
        }
        else if (data.purpose() === "configurationModalEditTimeController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = true;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
          //  $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "EDIT TIME";
        }
        else if (data.purpose() === "configurationModalRoleController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = true;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
           // $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "EDIT ROLE NAME";
        }
        else if (data.purpose() === "configurationModalEditVirtualEnvironmentImagesController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = true;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
          //  $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "VIRTUAL ENVIRONMENT IMAGES";
        }
        else if (data.purpose() === "configurationModalAddTenantController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = true;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
         //   $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "ADD TENANTS";
        }
        else if (data.purpose() === "configurationModalEditTenantController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = true;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
         //   $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "EDIT/DELETE TENANTS";
        }
        else if (data.purpose() === "configurationModalAddUserGroupController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = true;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
           // $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "ADD USER GROUP";
        }
        else if (data.purpose() === "configurationModalEditUserGroupController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = true;
            $scope.isLabHoursCreditVisible = false;
         //   $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "EDIT USER GROUP";
        }
        else if (data.purpose() === "configurationModalLabHoursCreditsController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = true;
           // $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "LAB HOURS CREDIT";
        }
        else if (data.purpose() === "configurationModalAddTenantNewController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isAddTenantNewVisible = true;
            $scope.isEditTenantNewVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
          //  $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "ADD TENANTS";
        }
        else if (data.purpose() === "configurationModalEditTenantNewController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isAddTenantNewVisible = false;
            $scope.isEditTenantNewVisible = true;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
          //  $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = false;
            $scope.header = "EDIT TENANTS";
        }
        else if (data.purpose() === "configurationModalBusinessController") {
            $scope.isCreateVirtualEnvironmentVisible = false;
            $scope.isEditVirtualEnvironmentVisible = false;
            $scope.isEditRoleNameVisible = false;
            $scope.isEditTimeVisible = false;
            $scope.isEditVirtualEnvironmentImagesVisible = false;
            $scope.isAddTenantVisible = false;
            $scope.isAddTenantNewVisible = false;
            $scope.isEditTenantNewVisible = false;
            $scope.isEditTenantVisible = false;
            $scope.isAddUserGroupVisible = false;
            $scope.isEditUserGroupVisible = false;
            $scope.isLabHoursCreditVisible = false;
            //  $scope.isAutoDeletionVisible = false;
            $scope.isTypeOfBusinessesVisible = isCSaccountAdmin;
            $scope.header = "Business Type";
        }
        //else if (data.purpose() === "configurationModalAutoDeletion") {
        //    $scope.isCreateVirtualEnvironmentVisible = false;
        //    $scope.isEditVirtualEnvironmentVisible = false;
        //    $scope.isEditRoleNameVisible = false;
        //    $scope.isEditTimeVisible = false;
        //    $scope.isEditVirtualEnvironmentImagesVisible = false;
        //    $scope.isAddTenantVisible = false;
        //    $scope.isAddTenantNewVisible = false;
        //    $scope.isEditTenantNewVisible = false;
        //    $scope.isEditTenantVisible = false;
        //    $scope.isAddUserGroupVisible = false;
        //    $scope.isEditUserGroupVisible = false;
        //    $scope.isLabHoursCreditVisible = false;
        //    $scope.isAutoDeletionVisible = true;
        ////    $scope.isTypeOfBusinessesVisible = false;
        //    $scope.header = "AUTO DELETION";
        //}
        //else if (data.purpose() === "createOrEditTypeOfBusinesses") {
        //    $scope.isCreateVirtualEnvironmentVisible = false;
        //    $scope.isEditVirtualEnvironmentVisible = false;
        //    $scope.isEditRoleNameVisible = false;
        //    $scope.isEditTimeVisible = false;
        //    $scope.isEditVirtualEnvironmentImagesVisible = false;
        //    $scope.isAddTenantVisible = false;
        //    $scope.isAddTenantNewVisible = false;
        //    $scope.isEditTenantNewVisible = false;
        //    $scope.isEditTenantVisible = false;
        //    $scope.isAddUserGroupVisible = false;
        //    $scope.isEditUserGroupVisible = false;
        //    $scope.isLabHoursCreditVisible = false;
        //    $scope.isAutoDeletionVisible = false;
        //    $scope.isTypeOfBusinessesVisible = true;
        //   // $scope.header = "Type of Businesses";
        //}

        $scope.$watch("title", function () {
            if ($scope.title === null || $scope.title === undefined) {
                $scope.DisabledThumbnailURL = true;
                $scope.DisabledVETypeID = true;
                $scope.DisabledTitle = true;
                $scope.DisabledCloudProviderID = true;
                $scope.DisabledName = true;
                $scope.DisabledDelete = true;

                $scope.VirtualEnvironmentID = null;
                $scope.ThumbnailURL = "";
                $scope.VETypeID = $scope.title;
                $scope.currentVEType = "";
                $scope.description = "";
                $scope.CloudProviderID = $scope.title;
                $scope.currentCloudProvider = "";
                $scope.Name = "";
                $scope.VETypeSelectedEdit = "";
                $scope.CloudProviderSelectedEdit = "";
            } else {
                $scope.DisabledThumbnailURL = false;
                $scope.DisabledVETypeID = false;
                $scope.DisabledDelete = false;
                $scope.DisabledTitle = false;
                $scope.DisabledCloudProviderID = false;
                $scope.DisabledName = false;
                angular.element(document.querySelector("IDSelectorDelete")).addClass("close");
                $scope.VirtualEnvironmentID = $scope.title.VirtualEnvironmentID;
                $scope.ThumbnailURL = $scope.title.ThumbnailURL;
                $scope.currentVEType = $scope.title.VETypeID;
                $scope.description = $scope.title.Description;
                $scope.currentCloudProvider = $scope.title.CloudProviderID;
                $scope.Name = $scope.title.Name;

                $scope.getCurrentSelectedVEType();
                $scope.VETypeSelectedEdit = $scope.CurrentSelectedVEType;

                $scope.getCurrentSelectedCloudProvider();
                $scope.CloudProviderSelectedEdit = $scope.CurrentSelectedCloudProvider;
            }
        });


        $scope.CurrentSelectedCloudProvider = "";
        $scope.getCurrentSelectedCloudProvider = function () {
            angular.forEach($scope.CloudProvider, function (value, key) {
                if (value.CloudProviderID === $scope.title.CloudProviderID) {
                    $scope.CurrentSelectedCloudProvider = value;
                }
            });
        };
        $scope.CurrentSelectedVEType = "";
        $scope.getCurrentSelectedVEType = function () {
            angular.forEach($scope.VEType, function (value, key) {
                if (value.VETypeID === $scope.title.VETypeID) {
                    $scope.CurrentSelectedVEType = value;
                }
            });
        };

        $scope.$watch("currentRoleName", function () {
            if ($scope.currentRoleName === null) {
                $scope.newRoleName = "";
                $scope.DisabledNewRoleName = true;
            } else {
                $scope.DisabledNewRoleName = false;
            }
        });

        $scope.$watch("ClientCode", function () {
            svc.checkIfClientExist($scope.ClientCode).then(function (response) {
                if (response == true)
                    $scope.show = true;
                else
                    $scope.show = false;
            });
        });

        $scope.$watch("ProjFamily", function () {
            svc.getAMI($scope.ProjFamily.name).then(
                function (respGCP) {
                    $scope.ProjAMIFamily = respGCP;
                });
        });

        $scope.$watch("PrjName", function () {
            svc.checkIfClientExist($scope.ClientCode, $scope.PrjName).then(function (response) {
                if (response == true)
                    $scope.show = true;
                else
                    $scope.show = false;
            });
        });

        $scope.deleteVE = function () {
            var VEInfo = {
                id: $scope.title.VirtualEnvironmentID,
                //--------info for confirmation modal----------
                headTitle: "DELETE VIRTUAL ENVIRONMENT",
                message: "Delete Virtual Environment?",
                type: "delete",
                reason: "deleteVirtualEnvironment"
            };

            var modal = $uibModal.open({
                templateUrl: '/app/Configuration/Modal/confirmationModal.html',
                windowClass: 'my-modal-class',
                controller: "confirmationModalController",
                controllerAs: '$confmodal',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return VEInfo;
                    }
                }
            });
        };

        // Edit TIME

        // Edit ROLE
        //bind Roles from database to the dropdown



        $scope.deleteVEImage = function () {
            var VEInfo = {
                id: $scope.TitleImages.VirtualEnvironmentID,
                //--------info for confirmation modal----------
                headTitle: "DELETE VIRTUAL ENVIRONMENT IMAGE",
                message: "Delete Virtual Environment Image?",
                type: "delete",
                reason: "deleteVirtualEnvironmentImage"
            };

            var modal = $uibModal.open({
                templateUrl: '/app/Configuration/Modal/confirmationModal.html',
                windowClass: 'my-modal-class',
                controller: "confirmationModalController",
                controllerAs: '$confmodal',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return VEInfo;
                    }
                }
            });
        };

        $scope.deleteUserGroup = function () {
            //alert($scope.SelectUserGroup.GroupName);
            var VEInfo = {
                id: $scope.SelectUserGroupEditName.CloudLabsGroupID,
                GroupName: $scope.SelectUserGroupEditName.GroupName,
                //--------info for confirmation modal----------
                headTitle: "USER GROUP",
                message: "Delete User Group?",
                type: "delete",
                reason: "deleteUserGroup"
            };

            var modal = $uibModal.open({
                templateUrl: '/app/Configuration/Modal/confirmationModal.html',
                windowClass: 'my-modal-class',
                controller: "confirmationModalController",
                controllerAs: '$confmodal',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return VEInfo;
                    }
                }
            });
        };

        $scope.DisableImageURL = true;
        $scope.DisableTitle = true;
        $scope.VirtualEnvironmentImagesPurpose = "";
        $scope.DisabledDeleteImage = false;
        $scope.disableSaveButton = true;
        //$scope.$watch("TitleImages", function () {
        //    if ($scope.TitleImages === null || $scope.TitleImages === undefined) {
        //        $scope.DisableImageURL = true;
        //        $scope.DisabledDeleteImage = true;
        //        $scope.URLImages = undefined;
        //    } else {
        //        $scope.URLImages = undefined;
        //        $scope.DisableImageURL = false;
        //        $scope.URLImages = '';
        //        $scope.fillFilteredImages = function () {
        //            $scope.$emit('LOAD');
        //            svc.getFilteredVirtualEnvironmentImages($scope.TitleImages.Title).then(
        //                function (response) {
        //                    $scope.Images = response;
        //                    angular.forEach($scope.Images, function (value, key) {
        //                        if ($scope.ImageInfo !== null) {
        //                            if (value === $scope.ImageInfo.Name) {
        //                                $scope.URLImages = value;
        //                            }
        //                        }
        //                    });
        //                    $scope.$emit('UNLOAD');
        //                });
        //        };
        //        $scope.ImageInfo;
        //        $scope.getVirtualEnvironmentIDFromVEImages = function () {                    
        //            svc.getVEIDFromVEImages($scope.TitleImages.VirtualEnvironmentID).then(
        //                function (response) {

        //                    $scope.ImageInfo = response;
        //                    if ($scope.ImageInfo !== null) {
        //                        $scope.VirtualEnvironmentImagesPurpose = "EditImage";
        //                        $scope.DisabledDeleteImage = false;
        //                        $scope.header = "EDIT VIRTUAL ENVIRONMENT IMAGES";
        //                    }
        //                    else {
        //                        $scope.VirtualEnvironmentImagesPurpose = "AddImage";
        //                        $scope.DisabledDeleteImage = true;
        //                        $scope.header = "ADD VIRTUAL ENVIRONMENT IMAGES";
        //                    }
        //                }
        //            );
        //        };
        //        $scope.DisabledDeleteImage = false;

        //        $scope.fillFilteredImages();
        //        $scope.getVirtualEnvironmentIDFromVEImages();
        //    }
        //}); 


        $scope.$watch("TitleImages", function () {
            if ($scope.TitleImages === null || $scope.TitleImages === undefined || $scope.TitleImages === "") {
                $scope.DisableImageURL = true;
                $scope.DisabledDeleteImage = false;
                $scope.URLImages = undefined;
            }
            else {
                $scope.DisableImageURL = false;
                $scope.DisabledDeleteImage = true;
                angular.forEach($scope.StorageTemplateName, function (value, key) {
                    svc.getImageTemplate($scope.TitleImages.Description, $scope.StorageTemplateName["connectionstring"], decodeURI($scope.StorageTemplateName["storageKey"].replace("+", "%2B")), decodeURI($scope.StorageTemplateName["storageAccountName"].replace("+", "%2B"))).then(
                        function (response) {
                            $scope.Images = response;
                            //$scope.URLImages = response;
                        })
                });
                $scope.URLImages = undefined;
            
            }

        });
        $scope.$watch("StorageTemplateName", function () {
            if ($scope.StorageTemplateName === null || $scope.StorageTemplateName === undefined) {
                $scope.DisableTitle = true;
                $scope.DisabledDeleteImage = false;
                svc.readVirtualEnvironments().then(
                    function (response) {
                        response = $filter('orderBy')(response, 'Title');
                        $scope.VEDescription = response;
                    });
            } else {
                $scope.DisableTitle = false;
                svc.readVirtualEnvironments().then(
                    function (response) {
                        response = $filter('orderBy')(response, 'Title');
                        $scope.VEDescription = response;
                    });
            }
        });

        svc.readVirtualEnvironments().then(
            function (response) {
                response = $filter('orderBy')(response, 'Title');
                angular.forEach(response, function (value, key) {
                    if (value.VETypeID == 10)
                        $scope.VEDescriptionGCP.push(value);
                })
            });
      

        //------------------------------------------------------------------------------------------------



        $scope.monitorLength = function (maxLength) {
            if ($scope.TenantCode.length > maxLength) {
                $scope.TenantCode = $scope.TenantCode.substring(0, maxLength);
            }
        };

        $scope.$watch("SelectTenantName", function () {
            $scope.isTenantNameSelected = true;
            $scope.Code = "";
            $scope.ApiUrl = "";
            $scope.EdxUrl = "";//if ($scope.SelectTenantName === null) {
            //    $scope.isTenantNameSelected = true;
            //    $scope.Code = "";
            //    $scope.ApiUrl = "";
            //    $scope.GuacConnection = "";
            //    $scope.GuacamoleURL = "";
            //    $scope.SubscriptionID = "";
            //    $scope.EdxUrl = "";
            //    $scope.AuthConnectionString = "";
            //} else {
            //    $scope.isTenantNameSelected = false;

            //    $scope.TenantCode = $scope.SelectTenantName.Code;
            //    $scope.TenantDatabaseName = $scope.SelectTenantName.DbName;
            //    $scope.TenantDatabaseHost = $scope.SelectTenantName.DbHost;
            //    $scope.TenantDatabaseUser = $scope.SelectTenantName.DbUser;
            //    $scope.TenantDatabasePassword = $scope.SelectTenantName.DbPass;
            //    $scope.TenantAPIURL = $scope.SelectTenantName.ApiUrl;
            //    $scope.TenantGuacamoleConnection = $scope.SelectTenantName.GuacConnection;
            //    $scope.TenantGuacamoleURL = $scope.SelectTenantName.GuacamoleURL;
            //    $scope.TenantSubscriptionID = $scope.SelectTenantName.SubscriptionId;

            //}
        });


        $scope.deleteTenants = function () {
            var VEInfo = {
                id: $scope.SelectTenantName.TenantId,
                //--------info for confirmation modal----------
                headTitle: "DELETE TENANT",
                message: "Delete Tenant Record?",
                type: "delete",
                reason: "deleteTenant"
            };

            var modal = $uibModal.open({
                templateUrl: '/app/Configuration/Modal/confirmationModal.html',
                windowClass: 'my-modal-class',
                controller: "confirmationModalController",
                controllerAs: '$confmodal',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return VEInfo;
                    }
                }
            });

        };

        $scope.$watch("SelectUserGroup", function () {
            $scope.IsCourseByGroupProvisionedFlag = true;
            $scope.isInputLower = true;
            if ($scope.SelectUserGroup === null) {
                $scope.isUserGroupSelectTotalLabMins = true;
                $scope.isUserGroupSelectLabMinsPerCourse = true;
                $scope.isUserGroupSelectVEProfile = true;
                $scope.SelectVEProfiles = null;
                $scope.TotalLabHours = "";
                $scope.LabHoursPerCourse = "";
            } else {
                $scope.isUserGroupSelectVEProfile = false;
                $scope.SelectVEProfiles = null;
                $scope.TotalLabHours = "";
                $scope.LabHoursPerCourse = "";

            }
        });

        $scope.$watch("SelectVEProfiles", function () {
            $scope.isInputLower = true;
            if ($scope.SelectVEProfiles === null) {
                $scope.IsCourseByGroupProvisionedFlag = true;
                $scope.isUserGroupSelectTotalLabMins = true;
                $scope.isUserGroupSelectLabMinsPerCourse = true;
                $scope.TotalLabHours = "";
                $scope.LabHoursPerCourse = "";
            } else {
                $scope.isUserGroupSelectTotalLabMins = false;
                $scope.isUserGroupSelectLabMinsPerCourse = false;
                $scope.$emit('LOAD');
                svc.checkTotalLabCredits($scope.SelectUserGroup.CloudLabsGroupID, $scope.SelectVEProfiles.VEProfileID).then(
                    function (response) {

                        $scope.TotalLabHours = response.TotalLabHours;
                        $scope.LabHoursPerCourse = response.LabHoursPerCourse;
                        $scope.TotalRemainingHours = response.TotalRemainingHours;
                        if (response.TotalLabHours === 0 && response.LabHoursPerCourse === 0) {
                            userGroupState = "InitialState";
                        } else {
                            userGroupState = "ReloadState";
                            $scope.currentTotalLabHoursCredit = response.TotalLabHours;
                        }

                        svc.IsCourseByGroupProvisioned($scope.SelectUserGroup.CloudLabsGroupID, $scope.SelectVEProfiles.VEProfileID).then(
                            function (response) {
                                $scope.IsCourseByGroupProvisionedFlag = response;
                                $scope.$emit('UNLOAD');
                            });
                    });
            }
        });

        $scope.$watch("SelectSubscriptionCredit", function () {
            //$scope.$emit('LOAD');
            if ($scope.SelectSubscriptionCredit !== undefined) {
                svc.CheckTotalSubscriptionCredit($scope.SelectSubscriptionCredit.CloudLabsGroupID).then(
                    function (response) {
                        if (response.SubscriptionHourCredits === 0 || response.SubscriptionHourCredits === null) {
                            userGroupState = "InitialState";
                        } else {
                            userGroupState = "ReloadState";
                        }
                    });
            }
        });

        $scope.$watch("SelectUserGroupEditName", function () {
            $scope.DuplicateChecker = false;

            $scope.UserGroupName = "";
            $scope.EdxUrl = "";
            $scope.CLUrl = "";
          //  $scope.oldTypeOfBusinessName = "";
            if ($scope.SelectUserGroupEditName === null) {
                $scope.isUserGroupSelect = true;
            } else {
                //angular.forEach($scope.UserGroups, function (item) {
                //    if ($scope.SelectUserGroupEditName.GroupName === item.GroupName) {
                //        $scope.oldTypeOfBusinessNameValue = item.TypeOfBusinessName;
                //        angular.forEach($scope.TypeOfBusiness, function (items) {
                //            if ($scope.oldTypeOfBusinessNameValue === items.TypeOfBusinessName) {
                //                $scope.oldTypeOfBusinessId = items.Id;
                //            }
                //        });
                //    }
                //});
                if ($scope.SelectUserGroupEditName != undefined) {
                    //svc.getUserGroupAzCode($scope.SelectUserGroupEditName.Environment, $scope.SelectUserGroupEditName.CLPrefix).then(
                    //    function (response) {
                    //        $scope.clPrefix = response;
                    //    });
                    var environment = $scope.SelectUserGroupEditName.Environment == "Staging" ? "D" : $scope.SelectUserGroupEditName.Environment == "QA" ? "Q" : $scope.SelectUserGroupEditName.Environment == "DMO" ? "U" : "P";
                    svc.getUserGroupAzCode(environment).then(
                        function (response) {
                            response.push($scope.SelectUserGroupEditName.CLPrefix);
                            $scope.clPrefix = response;
                        });

                    $scope.UserGroupName = $scope.SelectUserGroupEditName.GroupName;
                    $scope.EdxUrl = $scope.SelectUserGroupEditName.EdxUrl;
                    $scope.CLUrl = $scope.SelectUserGroupEditName.CLUrl;
                    $scope.CLPrefix = $scope.SelectUserGroupEditName.CLPrefix;
                    $scope.Environment = $scope.SelectUserGroupEditName.Environment;
                    $scope.CLPrefix = $scope.SelectUserGroupEditName.CLPrefix;
                    $scope.ApiPrefix = $scope.SelectUserGroupEditName.ApiPrefix;
                    svc.isUserGroupActive($scope.SelectUserGroupEditName.CloudLabsGroupID).then(
                        function (response) {
                            if (response) {
                                $scope.isUserGroupCurrentActive = false;
                            } else {
                                $scope.isUserGroupCurrentActive = true;
                            }
                        });
                }
                $scope.isUserGroupSelect = false;
                
               // $scope.oldTypeOfBusinessName = $scope.oldTypeOfBusinessNameValue;
            }
            for (var i = 0; i < $scope.tempArray.length; i++) {
                if ($scope.tempArray[i] === $scope.SelectUserGroupEditName.GroupName) {
                    var k = $scope.tempArray.splice(i, 1);
                }
            }
        });

        $scope.DuplicatePrefix = false;

        $scope.Duplicate = function () {
            var keepGoing = true;
            angular.forEach($scope.prefix, function (value) {
                if (keepGoing) {
                    if (value !== null && $scope.CLPrefix !== null) {
                        if (value.toLowerCase() === $scope.CLPrefix.toLowerCase()) {
                            $scope.DuplicatePrefix = true;
                            keepGoing = false;
                        } else {
                            $scope.DuplicatePrefix = false;
                        }
                    } else {
                        $scope.DuplicatePrefix = false;
                    }
                }
            });
        };

        $scope.changeCloud = function () {
            if ($scope.CloudProviderID.CloudProviderID == 1) {
                $scope.isAzure = true;
                $scope.isAWS = false;
                $scope.isGCP = false;
            }
            if ($scope.CloudProviderID.CloudProviderID == 2) {
                $scope.isAzure = false;
                $scope.isAWS = true;
                $scope.isGCP = false;
            }
            if ($scope.CloudProviderID.CloudProviderID == 3) {
                $scope.isAzure = false;
                $scope.isAWS = false;
                $scope.isGCP = true;
            }
        };

        $scope.changeEnvi = function () {
            if ($scope.CloudProviderID.CloudProviderID == 1) {
                $scope.isAzure = true;
                $scope.isAWS = false;
                $scope.isGCP = false;
            }
            if ($scope.CloudProviderID.CloudProviderID == 2) {
                $scope.isAzure = false;
                $scope.isAWS = true;
                $scope.isGCP = false;
            }
            if ($scope.CloudProviderID.CloudProviderID == 3) {
                $scope.isAzure = false;
                $scope.isAWS = false;
                $scope.isGCP = true;
            }
        };

        $scope.btnClicked = function () {
            if ($scope.btn == "Add") {
                $scope.createDisabled = false;
                $scope.addDisabled = true;
                $scope.PrjName = "";
                $scope.VPCName = "";
                $scope.VPCSubName = "";

            }
            if ($scope.btn == "Create") {
                $scope.addDisabled = false;
                $scope.createDisabled = true;
                $scope.ProjectName = "";
                $scope.VPCNetworkName = "";
                $scope.SubNetwork = "";
            }            
        }

        $scope.clickVE = function () {
            if ($scope.btn == "NonGCP") {
                $scope.GCPDisabled = false;
                $scope.noGCPDisabled = true;
                $scope.StorageTemplateName = "";
                $scope.TitleImages = "";
            }
            if ($scope.btn == "GCP") {
                $scope.noGCPDisabled = false;
                $scope.GCPDisabled = true;
                $scope.ProjFamily = "";
                $scope.AMI = "";
            }
        }

    }

})();
