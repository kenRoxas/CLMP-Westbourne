(function () {

    "use strict";
    angular.module("app-Usermanagement")
        .controller("UsermanagementController", UsermanagementController)
        .filter('dateRange', function () {
            return function (items, fromDate, toDate) {
                var filtered = [];
                var from_date = Date.parse(fromDate);
                var to_date = Date.parse(toDate);
                angular.forEach(items, function (item) {
                    if (Date.parse(item.DateCreated) >= from_date && Date.parse(item.DateCreated) <= to_date) {
                        filtered.push(item);
                    }
                });
                return filtered;
            };
        });

    UsermanagementController.$inject = ['$scope', '$uibModal', '$route', '$http', 'svc', '$filter', '$window', '$cookies', '$rootScope', '$timeout'];

    function UsermanagementController($scope, $uibModal, $route, $http, svc, $filter, $window, $cookies, $rootScope, $timeout) {
        $scope.$on('LOAD', function () { $scope.loader = true; });
        $scope.$on('UNLOAD', function () { $scope.loader = false; });
        $scope.userIsSuperAdmin = userIsSuperAdmin;
        $scope.userIsAdmin = userIsAdmin;
        $scope.bulkShow = false;
        $scope.exportUser = false;
        $scope.sortReverse = false;
        $scope.sortType = '';
        $scope.showEdit = true;
        $scope.sortName = false;
        //$scope.profile = "../../Content/images/profile.png";
        $scope.validRole = false;
        $scope.hideShowClass = "outer-box";

        $scope.showPanel = false;
        $scope.showPanelTitle = "SHOW";
        $scope.createdByUsers = [];
        $scope.isLoading = false;
        $scope.userManagementUsers = [];
        var userObject = $scope.userManagementUsers;
        $scope.roles = [];
        $scope.groups;
        $scope.showPage = false;
        $scope.verified = [{ "Status": "Verified", "StatusBool": "True" }, { "Status" : "UnVerified", "StatusBool" : "False"}];
        $scope.status = [{ "Status": "Active", "StatusBool": "Active" }, { "Status": "InActive", "StatusBool": "InActive"}];
    
        var userManagementFiltered;
        var roleOptions;
        var groupOptions;

        $scope.tempGroup = [];
        $scope.loadMainTable = function () {
            //if (userIsSuperAdmin && currentEmail.includes("cloudswyft.com")) {
            if (userIsSuperAdmin || userIsAdmin) {
                $scope.bulkShow = true;
                if (userIsSuperAdmin)
                    $scope.exportUser = true;
            }
            $scope.searchMsg = "";
            if ($scope.isLoading)
                return;
            $scope.length = 0;
            $scope.isLoading = true;

            svc.roleCloudOptions(userRoleName).then(
                function (responseResult) {
                    angular.copy(responseResult, $scope.roles);
                    $scope.RoleName = userRoleName;
                    roleOptions = $scope.roles;
                });
            if (userRoleName === "SuperAdmin" || userRoleName === "Instructor")
                $scope.validRole = true;
            else
                $scope.validRole = false;

            svc.getUserGroups().then(
                function (responseGroup) {
                    var temp = responseGroup;
                    $scope.groups = temp;
                    if (userRoleName == "SuperAdmin")
                        $scope.tempGroup = temp;
                    else {
                        angular.forEach(temp, function (value, key) {
                            if (value.GroupName == groupName)
                                $scope.tempGroup.push(value);
                        });
                    }
                    $scope.tempGroup = $scope.tempGroup.map(a => a.GroupName);
                    groupOptions = $scope.groups;
                });

            svc.GetUsersCloudLabs(currentId, userRoleName, currentUserGroup).then(
                function (result) {
                    angular.copy(result, $scope.userManagementUsers);
                    if (userRoleName == 'SuperAdmin' || userRoleName == 'Admin') {
                        angular.forEach($scope.userManagementUsers, function (value, key) {
                            if ($scope.userManagementUsers[key].EmailConfirmed == true) {
                                $scope.userManagementUsers[key].EmailVerified = true;
                                $scope.userManagementUsers[key].EmailLoading = false;
                            } else {
                                $scope.userManagementUsers[key].EmailVerified = false;
                                $scope.userManagementUsers[key].EmailLoading = false;
                            }
                            if ($scope.userManagementUsers[key].IsDisabled)
                                $scope.userManagementUsers[key].IsDisabled = "InActive";
                            else
                                $scope.userManagementUsers[key].IsDisabled = "Active";
                        });
                    } else {
                        angular.forEach($scope.userManagementUsers, function (value, key) {
                            if ($scope.userManagementUsers[key].IsDisabled)
                                $scope.userManagementUsers[key].IsDisabled = "InActive";
                            else
                                $scope.userManagementUsers[key].IsDisabled = "Active";
                        });
                    }

                    if ($scope.filterData.length > 0) {
                        $scope.isLoading = false;
                        $scope.length = $scope.filterData.length;
                        userManagementFiltered = $scope.filterData;
                        $scope.filterData = $filter('orderBy')(userManagementFiltered, ['Firstname', 'Lastname'], false);
                        $scope.clear(null, null);

                        $scope.showPage = false;
                        //$scope.loadMainTable();
                    }
                    else {
                        $scope.searchMsg = "No user profiles found.";
                        $scope.isLoading = false;
                        //$scope.userManagementUsers = [];
                        //$scope.filterData = true;
                        //$scope.showPage = true;


                    }
                }
            );
        };

        $scope.loadMainTable();
        $scope.pageChanged = function () {
            $scope.usersManagementUsers = ($scope.currentPage - 1) * $scope.itemsPerPage;
        };

        $rootScope.$on("CallParentMethod", function () {
            $scope.isLoading = false;
            $scope.userManagementUsers = [];
            $scope.loadMainTable();
            $window.location.reload();
        });

        $rootScope.$on("loadPage", function () {
            $scope.userManagementUsers = [];
            $scope.isLoading = true;
        });

        $rootScope.$on("CallParent1Method", function () {
            $scope.isLoading = false;
            $scope.userManagementUsers = [];
            $scope.loadMainTable();
        });

        $scope.caretShow = true;
        $scope.caretChange = {
            Name: false,
            Email: false,
            Role: false,
            Status: false,
            GroupName: false,
            CreatedBy: false,
            DateCreated: false,
            UpSideDown: false,
            EmailConfirmed: false
        };

        $scope.defaultCaretChange = {
            Name: false,
            Email: true,
            Role: true,
            Status: true,
            GroupName: true,
            CreatedBy: true,
            DateCreated: true,
            UpSideDown: true,
            EmailConfirmed: true
        };

        var caretChange = function (a) {
            for (var x in $scope.caretChange) {
                if (x === a)
                    $scope.caretChange[x] = !$scope.caretChange[x];
                else
                    $scope.caretChange[x] = false;
            }
            for (var y in $scope.defaultCaretChange) {
                if (y === a)
                    $scope.defaultCaretChange[y] = false;
                else
                    $scope.defaultCaretChange[y] = true;
            }
        };
        $scope.tableSort = function (a) {
            caretChange(a);
            switch (a) {
                case 'Name': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'Name';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'Email': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'Email';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'Role': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'RoleName';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'Status': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'IsDisabled';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'CreatedBy': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'CreatedBy';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'DateCreated': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'DateCreated';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'GroupName': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'GroupName';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'EmailConfirmed': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'EmailConfirmed';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                default:
            }
            filterUsers(a, $scope.sortReverse);
        };
        var filterUsers = function (a, b) {
            if (a === 'Name')
                a = ['Firstname', 'Lastname'];
            if (a === 'Status')
                a = 'IsDisabled';

            if ($scope.userManagementUsers.length === 0 || $scope.filterUsers === 0)
                $scope.userManagementUsers = $filter('orderBy')(userManagementFiltered, a, b);
            // $scope.searchMsg = "No user profiles found."
            else {
                //{
                //    $scope.itemsPerPage = $scope.pagingUsers;
                //}  $scope.userManagementUsers = $filter('orderBy')(userManagementFiltered,  ['Firstname', 'Lastname'], false);
                $scope.userManagementUsers = $filter('orderBy')(userManagementFiltered, a, b);
            }
        };

        $scope.currentPage = 1;
        $scope.itemsPerPage = 12;
        $scope.maxSize = 10;


        $scope.openModal = function (fileName, size, userDetails, modalNumber, modifiedClass) {
            var details;
            if (modalNumber === 0) {
                details = {
                    createEditModalHeader: "Create User Profile",
                    isShowCreate: true,
                    isShowEdit: false,
                    saveOrCreate: 'Create',
                    showPassword: true,
                    userDetails,
                    showEdit: true,
                    userRole: userDetails.role,
                    roleOptions: roleOptions,
                    groupOptions: groupOptions,
                    isBulk: false,
                    isExport: false,
                    isCreatedUser: true
                };
            }
            else if (modalNumber === 1) {
                details = {
                    createEditModalHeader: "Edit User Profile",
                    isShowCreate: false,
                    isShowEdit: true,
                    saveOrCreate: 'Save',
                    showPassword: false,
                    userDetails,
                    showEdit: false,
                    roleOptions: roleOptions,
                    groupOptions: groupOptions,
                    isBulk: false,
                    isExport: false,
                    isCreatedUser: true
                };
            }
            else if (modalNumber === 2) {
                details = {
                    createEditModalHeader: "Bulk Create",
                    isShowCreate: false,
                    isShowEdit: false,
                    saveOrCreate: 'Save',
                    showPassword: false,
                    userDetails,
                    showEdit: false,
                    roleOptions: roleOptions,
                    groupOptions: groupOptions,
                    isBulk: true,
                    isExport: false,
                    isCreatedUser: false
                };
            }
            else if (modalNumber === 3) {
                details = {
                    createEditModalHeader: "Export User",
                    isShowCreate: false,
                    isShowEdit: false,
                    saveOrCreate: 'Save',
                    showPassword: false,
                    userDetails,
                    showEdit: false,
                    roleOptions: roleOptions,
                    groupOptions: groupOptions,
                    isBulk: false,
                    isExport: true,
                    isCreatedUser: false
                };
            }
            else {
                details = {
                    userDetails
                };
            }

            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Usermanagement/Modal/' + fileName + '.html',
                windowClass: modifiedClass,
                controller: fileName + 'Controller',
                size: size,
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    items: function () {
                        return details;
                    }
                }
            });
        };
        $scope.delete = function (userDetails, type) {
            var details = {
                userDetails,
                type,
                message: "Are you sure you want blac delete the user profile?",
                warnMessage: "Clicking 'Yes' will delete all records associated to the user profile.",
                title: "Delete User Profile"
            };

            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Usermanagement/Modal/alertModal.html',
                controller: 'alertModalController',
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    items: function () {
                        return details;
                    }
                }
            });
            modal.result.then(function (result) {
            }, function () {
                $scope.loadMainTable();//$route.reload();
            });
        };
        $scope.disable = function (userDetails, type) {
            var details;
            if (type === 'disable') {
                details = {
                    userDetails,
                    type,
                    message: "Are you sure you want to disable the user profile?",
                    title: "Disable User Profile"
                };
            }
            else {
                details = {
                    userDetails,
                    type,
                    message: "Are you sure you want to enable the user profile?",
                    title: "Enable User Profile"
                };
            }


            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Usermanagement/Modal/alertModal.html',
                controller: 'alertModalController',
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    items: function () {
                        return details;
                    }
                }
            });
            modal.result.then(function (result) {
            }, function () {
                $scope.loadMainTable();//$route.reload();
            });
        };
        $scope.formatDate = 'MM/dd/yyyy';
        $scope.datePicker = {
            startDateStatus: { opened: false },
            startDateOpen: function ($event) {
                $scope.datePicker.startDateStatus.opened = true;
            },
            startsubmittedDateStatus: { opened: false },
            startSubmittedDateOpen: function ($event) {
                $scope.datePicker.startsubmittedDateStatus.opened = true;
            },
            startDateOptions: {
                maxDate: null
            },
            startSubmittedDateOptions: {
                minDate: null,
                maxDate: new Date()
            },
            endDateStatus: { opened: false },
            endStatusOpen: function ($event) {
                $scope.datePicker.endDateStatus.opened = true;
            },
            endSubmittedDateStatus: { opened: false },
            endSubmittedStatusOpen: function ($event) {
                $scope.datePicker.endSubmittedDateStatus.opened = true;
            },
            endDateOptions: {
                minDate: null
            },
            endSubmittedDateOptions: {
                minDate: null,
                maxDate: new Date()
            }
        };

        $scope.changeSubmittedMinAndMaxDate = function () {
            $scope.datePicker.startSubmittedDateOptions.maxDate = $scope.searchModel.endSubmittedDate;
            $scope.datePicker.endSubmittedDateOptions.minDate = $scope.searchModel.startSubmittedDate;

            $scope.end = new Date(moment.utc($scope.datePicker.startSubmittedDateOptions.maxDate));
            $scope.start = new Date(moment.utc($scope.datePicker.endSubmittedDateOptions.minDate));

            var users = userManagementFiltered;

            if ($scope.datePicker.startSubmittedDateOptions.maxDate && $scope.datePicker.endSubmittedDateOptions.minDate) {
                $scope.userManagementUsers = [];
                angular.forEach(users, function (value, key) {
                    //var receivedDate = new Date(value.DateCreated); // from v1
                    var receivedDate = new Date(value.DateCreated); // from v2

                    if (receivedDate >= $scope.datePicker.endSubmittedDateOptions.minDate && receivedDate <= $scope.datePicker.startSubmittedDateOptions.maxDate) {
                        $scope.userManagementUsers.push(value);
                    }
                });

            }
        };

        $scope.showPanelSearch = function () {
            if ($scope.showPanel) {
                $scope.showPanel = false;
                $scope.showPanelTitle = "SHOW";
                $scope.hideShowClass = "outer-box";
            } else {
                $scope.showPanel = true;
                $scope.showPanelTitle = "HIDE";
                $scope.hideShowClass = "outer-box";
            }
        };

        $scope.clear = function (searchfrm, srch) {
            angular.copy({}, searchfrm);
            angular.copy({}, srch);
            $scope.datePicker.endSubmittedDateOptions.maxDate = new Date();
            $scope.datePicker.startSubmittedDateOptions.maxDate = new Date();
            if ($scope.filterData.length)
                $scope.searchMsg = "No user profiles found.";
            else {
                $scope.searchMsg = "No user profiles found.";
                $scope.filterData = userManagementFiltered;
            }
            filterUsers('Name', $scope.sortReverse);
            //$scope.loadMainTable();
        };

        $scope.emailLoading = false;

        $scope.toggleEmailVerification = function (user) {
            var userid = user.UserId;
            user.EmailLoading = true;

            svc.toggleEmailVerification(userid).then(
                function (response) {
                    user.EmailConfirmed = !user.EmailConfirmed;
                    user.EmailLoading = false;
                }
            )
        }
    }

})();