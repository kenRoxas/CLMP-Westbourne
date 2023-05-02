(function () {

    "use strict";
    angular.module("app-labprofiles")
        .controller("GrantLabAccessController", GrantLabAccessController);

    GrantLabAccessController.$inject = ['$scope', '$uibModal', '$uibModalInstance', '$route', '$http', 'svc', '$filter', '$window', '$cookies', '$rootScope', '$timeout', '$document', 'VEProfileID', 'VEType', 'VEName'];

    function GrantLabAccessController($scope, $uibModal, $uibModalInstance, $route, $http, svc, $filter, $window, $cookies, $rootScope, $timeout, $document, VEProfileID, VEType, VEName) {

        $scope.loading = true;
        $scope.allUsers = [];
        $scope.users = [];
        $scope.totalCourseHours = 0;
        $scope.remainingLabCredits = 0;
        $scope.totalRemainingCurrentCourseHours = 0;
        $scope.courseHours = 0;
        $scope.length = 0;
        $scope.sortReverse = false;
        $scope.sortType = '';
        $scope.Me = "";
        $scope.userGroupID = 0;
        $scope.model = [];
        $scope.creditMappings = {};
        $scope.totalRemainingCourseHours = 0;
        var roles = ["Student", "Instructor", "Admin", "Staff"];
        var modelJSON = [];
        var modelConsoleJSON = [];
        $scope.VEProfileID = VEProfileID;
        $scope.VEType = VEType;
        //$scope.currentPage = 1;
        $scope.itemsPerPage = 12;
        //$scope.maxSize = 10;
        $scope.usersFiltered = [];
        $scope.tempUsers = [];
        $scope.isTrue = true;
        $scope.Me = name;
        $scope.userGroupID = currentUserGroup;
        $scope.VEName = VEName;
        //$scope.maxSize = 5;
        $scope.currentPage = 1;
        //$scope.itemsPerPage = 10;
        $scope.selectedUsers = [];
        $scope.isAllSelected = false;
        $scope.newRegion = '';
        $scope.userId = [];
        $scope.bulkShow = false;
        $scope.newSize = '';
        $scope.sizes = ["StandardB1ms", "StandardB1s", "StandardB2ms", "StandardB2s", "StandardB4ms", "StandardB8ms", "StandardF1", "StandardF16", "StandardF16s", "StandardF16sV2", "StandardF1s", "StandardF2", "StandardF2s", "StandardF2sV2", "StandardF32sV2", "StandardF4", "StandardF4s", "StandardF4sV2", "StandardF64sV2", "StandardF7sV2", "StandardF8", "StandardF8s", "StandardF8sV2"];
        $scope.isUserSuperAdmin = userIsSuperAdmin;
        var userid = [];
        $scope.tempForAllUsers = [];
        $scope.maxSize = 5;     // Limit number for pagination display number.
        $scope.totalCount = 0;  // Total number of items in all pages. initialize as a zero
        $scope.status = "";
        $scope.tempUsers1 = [];
        $scope.pagination = {
            currentPage: 1,
            maxSize: 10,
            pageSizeSelected: 15
        };

        $scope.getAllUsers = function (filterStatus, search) {
            svc.checkVMUser(VEProfileID, $scope.pagination.currentPage, $scope.pagination.pageSizeSelected, filterStatus, search).then(function (response) {
                angular.copy(response.MachineUsers, $scope.allUsers);
                $scope.tempForAllUsers = $scope.allUsers;
                $scope.tempUsers1 = response.AllUsers;
                $scope.Userlength = response.totalCount;
                $scope.loading = false;
                //                $scope.users = $scope.allUsers.slice(0, $scope.itemsPerPage);
                $scope.totalCount = response.totalCount;
            });
        };


        $scope.getAllAdminStaff = function () {
            svc.checkVMAdminStaff(VEProfileID, $scope.pagination.currentPage, $scope.pagination.pageSizeSelected).then(function (response) {
                angular.copy(response.MachineUsers, $scope.allUsers);

                $scope.Userlength = response.totalCount;
                $scope.loading = false;
                //                $scope.users = $scope.allUsers.slice(0, $scope.itemsPerPage);
                $scope.totalCount = response.totalCount;
            });
        };

        var loadPage = function () {
            if (userIsSuperAdmin && currentEmail.includes("cloudswyft.com") && (VEType == 1 || VEType == 2 || VEType == 9))
                $scope.bulkShow = true;
            svc.checkTotalLabCredits($scope.userGroupID, $scope.VEProfileID)
                .then(function (response) {
                    $scope.creditMappings = response;
                    $scope.totalCourseHours = response.TotalCourseHours;
                    $scope.totalRemainingCurrentCourseHours = response.TotalRemainingCourseHours;
                    $scope.totalRemainingCourseHours = response.TotalRemainingCourseHours;
                    $scope.totalRemainingContainers = response.TotalRemainingContainers;
                    $scope.numberOfUsers = response.NumberOfUsers;
                    $scope.courseHours = response.CourseHours;
                });

            if ((VEType === 1 || VEType === 2 || VEType === 3 || VEType === 4 || VEType === 5 || VEType === 8 || VEType === 9 || VEType == 10) && $scope.isUserSuperAdmin) {
                if (VEType === 1 || VEType === 2 || VEType === 5 || VEType == 8 || VEType == 9 || VEType == 10)
                    $scope.getAllUsers();
                else if (VEType == 3 || VEType == 4)
                    $scope.getAllAdminStaff();

                $scope.loading = false;

                //svc.getRoleIdByName(roles)
                //    .then(function (response3) {
                //        svc.getUsersByRoleId(response3, $scope.VEProfileID, $scope.userGroupID)
                //            .then(function (response4) {
                //                if (VEType === 2 || VEType === 1 || VEType === 5 || VEType == 8 || VEType == 9) {
                //                    angular.copy(response4, $scope.allUsers);
                //                } else if (VEType == 3 || VEType == 4) {
                //                    angular.forEach(response4, function (value, key) {
                //                        if (response4[key].roleid === "B06590AB-57DA-436F-A958-B282550CAB13" || response4[key].roleid === "93b66799-f60c-4877-9e65-fbdb50a9e82b") { //for instructor and admin
                //                            $scope.allUsers.push(value);
                //                        }
                //                    });

                //                }
                //                $scope.length = $scope.allUsers.length;
                //                $scope.loading = false;
                //                $scope.users = $scope.allUsers.slice(0, $scope.itemsPerPage);
                //            });
                //    });
            }
            else if (VEType === 6 || VEType === 7 || !$scope.isUserSuperAdmin) {
                // if (VEType === 9)
                $scope.getAllUsers();

                //else {
                //    svc.getRoleIdByName(roles).then(function (response3) {
                //        svc.getUsersByRoleId(response3, $scope.VEProfileID, $scope.userGroupID)
                //            .then(function (response4) {
                //                var userIdList = [];
                //                var ConsoleUserContent = {
                //                    "ConsoleUserID": response4,
                //                    "VEType": VEType,
                //                    "VEProfileID": $scope.VEProfileID
                //                };
                //                angular.forEach(response4, function (value, key) {
                //                    userIdList.push(value.UserId);
                //                });
                //                var VMUserContent = {
                //                    "UserId": userIdList,
                //                    "VEProfileID": $scope.VEProfileID
                //                };
                //                if (VEType == 6 || VEType == 7) {
                //                    svc.checkConsoleUser(ConsoleUserContent).then(function (response) {
                //                        angular.copy(response, $scope.allUsers);
                //                        $scope.length = $scope.allUsers.length;
                //                        $scope.loading = false;
                //                        $scope.users = $scope.allUsers.slice(0, $scope.itemsPerPage);
                //                    });
                //                }
                //                //else {
                //                //        svc.checkVMUser(VMUserContent).then(function (response) {
                //                //        angular.copy(response, $scope.allUsers);
                //                //        $scope.length = $scope.allUsers.length;
                //                //        $scope.loading = false;
                //                //        $scope.users = $scope.allUsers.slice(0, $scope.itemsPerPage);
                //                //    });
                //                //}
                //            }
                //            );
                //    });
                //}


            }
        };

        loadPage();
        $scope.$watch('sizes.size', function (newValue, oldValue) {
            $scope.newSize = newValue;
        });

        $scope.$watch('regions.region', function (newValue, oldValue) {
            $scope.newRegion = newValue;
            $scope.newRegionPrefix = newValue;
            if (newValue !== undefined && newValue !== null) {
                $scope.newRegion = newValue.RegionName;
                $scope.newRegionPrefix = newValue.RegionPrefix;
            }
        });

        $scope.isStatusTitle = function (user) {
            if (user.IsCourseGranted == true && user.IsProvisioned == 0) {
                return 'Granted';
            }
            else if (user.IsCourseGranted == false) {
                return 'Not Granted';
            }
            else if (user.IsCourseGranted == true && user.IsProvisioned == 1) {
                return 'Provisioned';
            }
        };

        var cols = [{
            name: 'selected',
            orderDesc: false
        }, {
            name: 'LastName',
            orderDesc: false
        }, {
            name: 'Email',
            orderDesc: false
        }, {
            name: 'hasHours',
            orderDesc: false
        }, {
            name: 'IsCourseGranted',
            orderDesc: false
        }];

        $scope.sortData = function (sortCol) {

            // make sure it a valid column
            var column = cols.find(function (col) {
                return col.name === sortCol;
            });

            if (!column) return;

            column.orderDesc = !column.orderDesc;

            caretChange(sortCol);
            switch (sortCol) {
                case 'selected': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'LastName': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'LastName';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'Email': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'Email';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'hasHours': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'hasHours';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                case 'IsCourseGranted': {
                    $scope.caretShow = !$scope.caretShow;
                    $scope.sortType = 'IsCourseGranted';
                    $scope.sortReverse = !$scope.sortReverse;
                    break;
                }
                default:
            }

            filterUsers(sortCol, $scope.sortReverse);
            //setPagingData($scope.currentPage);
        };

        var filterUsers = function (a, b) {
            if (a === 'Name')
                a = ['FirstName', 'LastName'];
            $scope.allUsers = $filter('orderBy')($scope.allUsers, a, b);

        };
        var htmlRef = angular.element($document[0].html);

        $scope.caretShow = true;
        $scope.caretChange = {
            FirstName: false,
            Email: false,
            LabHoursTotal: false,
            IsCourseGranted: false,
            UpSideDown: false
        };

        $scope.defaultCaretChange = {
            FirstName: false,
            Email: true,
            Role: true,
            LabHoursTotal: true,
            IsCourseGranted: true,
            UpSideDown: true
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

        $scope.Save = function (countSelected) {
            userid = [];

            $scope.tempUsers = [];
            if ($scope.newRegionPrefix !== undefined && $scope.newRegionPrefix !== null) {
                $scope.creditMappings.region = $scope.newRegionPrefix;
            }

            if ($scope.newSize !== undefined && $scope.newSize !== null) {
                $scope.creditMappings.size = $scope.newSize;
            }
            angular.forEach(countSelected, function (value, key) {
                angular.forEach($scope.allUsers, function (value1) {
                    if (countSelected[key].UserId === value1.UserId) {
                        $scope.tempUsers.push(value1);
                    }
                });
            });

            var items = {
                title: "Grant Lab Access",
                message: "Are you sure you want to grant lab access to the selected users?",
                type: "create",
                isBulkCreate: false,
                isGrant: true
            };

            var consoleItems = {
                title: "Schedule Format",
                message: "Are you sure you want to schedule the course to the selected users?",
                type: "create",
                isGrant: true
            };
            angular.forEach($scope.selectedUsers, function (value) {
                userid.push(parseInt(value.UserId));
            })
            if (countSelected.length != 0) {
                if ((VEType === 1 || VEType === 2 || VEType === 5 || VEType === 8 || VEType === 9 || VEType === 10) && userIsSuperAdmin) {
                    var modal = $uibModal.open({
                        templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                        controller: "GrantConfirmationModalController",
                        controllerAs: '$confmodal',
                        size: 'xs',
                        backdrop: 'static',
                        keyboard: true,
                        resolve: {
                            content: function () {
                                return userid;
                            },
                            creditMapping: function () {
                                return $scope.creditMappings;
                            },
                            items: function () {
                                return items;
                            },
                            VEType: function () {
                                return VEType;
                            },
                            VMEmptyData: function () {
                                return null;
                            }

                        }
                    });
                }
                else if ((VEType === 1 || VEType === 2 || VEType === 5 || VEType === 6 || VEType === 7 || VEType === 8 || VEType === 9 || VEType === 10) && !userIsSuperAdmin) {
                    var modal = $uibModal.open({
                        templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                        controller: "GrantConfirmationModalController",
                        controllerAs: '$confmodal',
                        size: 'xs',
                        backdrop: 'static',
                        keyboard: true,
                        resolve: {
                            content: function () {
                                return $scope.selectedUsers;
                            },
                            creditMapping: function () {
                                return $scope.creditMappings;
                            },
                            items: function () {
                                return consoleItems;
                            },
                            VEType: function () {
                                return VEType;
                            },
                            VMEmptyData: function () {
                                return null;
                            }
                        }
                    });
                }
                else if ((VEType === 6 || VEType === 7) && userIsSuperAdmin) {
                    var modal = $uibModal.open({
                        templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                        controller: "GrantConfirmationModalController",
                        controllerAs: '$confmodal',
                        size: 'xs',
                        backdrop: 'static',
                        keyboard: true,
                        resolve: {
                            content: function () {
                                return $scope.selectedUsers;
                            },
                            creditMapping: function () {
                                return $scope.creditMappings;
                            },
                            items: function () {
                                return consoleItems;
                            },
                            VEType: function () {
                                return VEType;
                            },
                            VMEmptyData: function () {
                                return null;
                            }
                        }
                    });
                }
                //else if (VEType == 3 || VEType == 4 && userIsSuperAdmin) {
                //    var modal = $uibModal.open({
                //        templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                //        controller: "GrantConfirmationModalController",
                //        controllerAs: '$confmodal',
                //        size: 'xs',
                //        backdrop: 'static',
                //        keyboard: true,
                //        resolve: {
                //            content: function () {
                //                return userid;
                //            },
                //            creditMapping: function () {
                //                return $scope.creditMappings;
                //            },
                //            items: function () {
                //                return items;
                //            },
                //            VEType: function () {
                //                return VEType;
                //            },
                //            VMEmptyData: function () {
                //                return null;
                //            }

                //        }
                //    });
                //}
                else {
                    var modal2 = $uibModal.open({
                        templateUrl: '/app/LabProfiles/Modal/GrantAdditionalModal.html',
                        controller: "GrantAdditionalModalController",
                        size: 'xs',
                        backdrop: 'static',
                        keyboard: false,
                        resolve: {
                            users: function () {
                                return $scope.tempUsers;
                            },
                            creditMapping: function () {
                                return $scope.creditMappings;
                            },
                            items: function () {
                                return items;
                            },
                            VEType: function () {
                                return VEType;
                            }
                        }
                    });
                }
            }
            else {
                items.isBulkCreate = true;
                items.message = "Note: Only .csv or .xlsx file type to upload";

                var modal = $uibModal.open({
                    templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                    controller: "GrantConfirmationModalController",
                    controllerAs: '$confmodal',
                    size: 'xs',
                    backdrop: 'static',
                    keyboard: true,
                    resolve: {
                        content: function () {
                            return userid;
                        },
                        creditMapping: function () {
                            return $scope.creditMappings;
                        },
                        items: function () {
                            return items;
                        },
                        VEType: function () {
                            return VEType;
                        },
                        VMEmptyData: function () {
                            return null;
                        }

                    }
                });
            }
        };

        $scope.close = function () {
            $("html").removeAttr('style');
            $uibModalInstance.close();
        };

        $scope.SelectUser = function (x, isRow) {
            if (isRow) {
                if (x.selected === null)
                    x.selected = true;
                else
                    x.selected = !x.selected;
            }
            var index;
            modelJSON = {
                UserId: x.UserId,
                VEProfileId: x.VEProfileId,
                VEType: $scope.VEType,
                LabHoursRemaining: x.LabHoursRemaining + $scope.courseHours,
                LabHoursTotal: x.LabHoursTotal + $scope.courseHours,
                IsCourseGrant: !x.IsCourseGranted,
                UserGroup: currentUserGroup,
            };

            if ($scope.VEType === 1 || $scope.VEType === 2 || $scope.VEType === 3 || $scope.VEType === 4 || $scope.VEType === 5 || VEType === 8 || VEType === 9 || VEType === 10) {
                if (x.selected) {
                    if (x.IsStarted != 3)
                        $scope.totalRemainingCourseHours -= $scope.courseHours

                    //$scope.totalRemainingCourseHours += $scope.courseHours

                    x.LabHoursTotal += $scope.courseHours;
                    $scope.selectedUsers.push(modelJSON);

                }
                else {
                    if (x.IsStarted != 3)
                        $scope.totalRemainingCourseHours += $scope.courseHours
                    //if (x.MachineStatus != 3) {
                    //    if (x.IsCourseGranted)
                    //        $scope.totalRemainingCourseHours -= $scope.courseHours
                    //    else
                    //        $scope.totalRemainingCourseHours += $scope.courseHours;
                    //}
                    angular.forEach($scope.selectedUsers, function (value, key) {
                        if ($scope.selectedUsers[key].UserId === modelJSON.UserId) {
                            index = key;
                        }
                    });
                    if (index > -1)
                        $scope.selectedUsers.splice(index, 1);
                    x.LabHoursTotal -= $scope.courseHours;

                }
            }

            if ($scope.VEType === 6 || $scope.VEType === 7) {
                if (x.selected) {
                    $scope.selectedUsers.push(modelJSON);
                    x.LabHoursTotal += $scope.courseHours;

                }
                else {
                    angular.forEach($scope.selectedUsers, function (value, key) {
                        if ($scope.selectedUsers[key].UserId === modelJSON.UserId) {
                            index = key;
                        }
                    });
                    if (index > -1)
                        $scope.selectedUsers.splice(index, 1);
                }
            }
        }

        $scope.toggleAll = function (filterData) {
            $scope.isAllSelected = !$scope.isAllSelected;
            var toggleStatus = $scope.isAllSelected;

            if (!toggleStatus) {
                angular.forEach(filterData, function (user) {
                    if (user.selected) {
                        user.selected = toggleStatus;
                        $scope.SelectUser(user, toggleStatus);
                    }
                });
            }
            else {
                var totalUserCourse = Math.floor($scope.totalRemainingCourseHours / $scope.courseHours);
                
                angular.forEach(filterData, function (user) {
                    if ($scope.VEType === 3 || $scope.VEType === 4) {
                        if (totalUserCourse > 0) {
                            totalUserCourse--;
                            user.selected = toggleStatus;
                            $scope.SelectUser(user);
                        }
                    }
                    else if (($scope.VEType === 6 || $scope.VEType === 7) && user.IsProvisioned == 0 && !user.selected) {
                        user.selected = toggleStatus;
                        $scope.SelectUser(user);
                    }
                    else {
                        if (user.IsStarted == 3) {
                            user.selected = toggleStatus;
                            $scope.SelectUser(user);
                        }
                        else if (user.IsStarted == 7 || user.hasHours == "Provisioned") {
                            //none
                        }
                        else if (!user.selected && totalUserCourse > 0) {
                            totalUserCourse--;
                            user.selected = toggleStatus;
                            $scope.SelectUser(user);
                        }
                        else if (user.MachineStatus == 3)
                            $scope.SelectUser(user);
                    }
                });
            }
        };

        //$scope.toggleAllConsole = function (filterData) {
        //    $scope.isAllSelected = !$scope.isAllSelected;
        //    var toggleStatus = $scope.isAllSelected;

        //    if (!toggleStatus) {
        //        angular.forEach(filterData, function (user) {
        //            if (user.selected) {
        //                user.selected = toggleStatus;
        //                $scope.SelectUserConsole(user, toggleStatus);
        //            }
        //        });
        //    }
        //    else {
        //        angular.forEach(filterData, function (user) {
        //            if (user.IsProvisioned == 0 && !user.selected) {
        //                user.selected = toggleStatus;
        //                $scope.SelectUserConsole(user);
        //            }
        //        });
        //    }
        //};

        //$scope.SelectUserConsole = function (x, isRow) {
        //    if (isRow) {
        //        if (x.selected === null)
        //            x.selected = true;
        //        else
        //            x.selected = !x.selected;
        //    }
        //    var index;

        //    modelConsoleJSON = {
        //        UserId: x.UserId,
        //        VEProfileId: x.VEProfileId,
        //        VEType: x.VEType,
        //        IsConsoleGranted: !x.IsConsoleGranted
        //    }

        //    //if ($scope.VEType === 6 || $scope.VEType === 7) {
        //        if (x.selected) {
        //            $scope.selectedUsers.push(modelConsoleJSON);
        //            $scope.selectedUsers.push(modelJSON);
        //            x.LabHoursTotal += $scope.courseHours;

        //        }
        //        else {
        //            angular.forEach($scope.selectedUsers, function (value, key) {
        //                if ($scope.selectedUsers[key].UserId === modelConsoleJSON.UserId) {
        //                    index = key;
        //                }
        //            });
        //            if (index > -1)
        //                $scope.selectedUsers.splice(index, 1);
        //        }
        //    //}
        //};

        $scope.toggled = function (user) {
            if (user.IsStarted == 4)
                return true;
            if (user.IsStarted == 7)
                return true;
            else if (user.MachineStatus == 3)
                return true;
            else if ((($scope.totalRemainingCourseHours - $scope.courseHours) < 0) && user.selected)
                return false;
            else if ($scope.VEType === 3 || $scope.VEType === 4) {
                if ((($scope.totalRemainingCourseHours - $scope.courseHours) < 0) || ($scope.totalRemainingCourseHours === 0)) {
                    //if ((user.IsStarted == null || user.IsStarted == 3) && user.IsCourseGranted == 1)
                    if ((user.IsStarted == null) && user.IsCourseGranted == 1)
                        return false;
                    else
                        return true;
                }
                else if ((user.IsStarted == 0 || user.IsStarted == 1 || user.IsStarted == 2 || user.IsStarted == 4)){
                    return true;
                }
            }
            else if ((user.IsStarted == null || user.IsStarted == 3) && user.IsGranted == 1)
                return false;
            //else if (((($scope.totalRemainingCourseHours - $scope.courseHours) < 0) && !user.selected) || ($scope.totalRemainingCourseHours === 0) || (user.hasHours && ($scope.VEType === 3 || $scope.VEType === 4)))
            //    return true;
            else if ((($scope.totalRemainingCourseHours - $scope.courseHours) < 0) || ($scope.totalRemainingCourseHours === 0)) {
                if ((user.IsStarted == null || user.IsStarted == 3) && user.IsCourseGranted == 1)
                    if ($scope.isUserSuperAdmin)
                        return true;
                    else
                        return false;
                else
                    return true;
            }
            else if ((($scope.totalRemainingContainers - $scope.courseHours) < 0) && user.selected)
                return false;
            else if ($scope.totalRemainingContainers === 0)
                return true;
            else if ((user.IsStarted == 0 || user.IsStarted == 1 || user.IsStarted == 2 || user.IsStarted == 3 || user.IsStarted == 4) && (userIsSuperAdmin && currentEmail.includes("cloudswyft.com")))
                return false;
            else if ((user.IsStarted == 0 || user.IsStarted == 1 || user.IsStarted == 2 || user.IsStarted == 3 || user.IsStarted == 4) && (userIsSuperAdmin && !currentEmail.includes("cloudswyft.com")))
                return true;
            else if ((user.IsStarted == 0 || user.IsStarted == 1 || user.IsStarted == 2 || user.IsStarted == 3 || user.IsStarted == 4) && (!userIsSuperAdmin))
                return true;
            else
                return false;
        };

        $scope.toggledConsole = function (user) {
            user.selected ? true : false;
        }

        //$scope.pageChanged = function () {
        //    $scope.users = ($scope.currentPage - 1) * $scope.itemsPerPage;
        //};


        $scope.checkStatusProv = function () {
            var i = $scope.allUsers.length;
            if ($scope.allUsers.length == 0) {
                $timeout(function () {

                    $scope.checkStatusProv();
                }, 180000);
            }
            angular.forEach($scope.allUsers, function (value, key) {
                if (value.MachineStatus == 4) {
                    svc.GetCourseNoStatus(value.UserId)
                        .then(function (response) {
                            angular.forEach(response, function (value1) {
                                if (value.UserId == $scope.allUsers[key].UserId && value1.IsStarted == 1) {
                                    $scope.allUsers[key].MachineStatus = value1.IsStarted;
                                    $scope.allUsers[key].LabHoursRemaining = value1.LabHoursRemaining;
                                    $scope.allUsers[key].LabHoursTotal = value1.LabHoursTotal;
                                    //angular.copy($scope.allUsers[key].MachineStatus, value1.IsStarted);
                                }

                            })

                        })

                };
                if (key + 1 == i) {
                    $timeout(function () {
                        $scope.checkStatusProv();
                    }, 18000);
                }
            });
        }

        $scope.checkStatusProv();

        $scope.pageChanged = function () {
            $scope.getAllUsers();
        };

        //This method is calling from dropDown
        $scope.changePageSize = function () {
            $scope.pagination.currentPage = 1
            $scope.getAllUsers();
        };

        $scope.opt = [
            { name: 'All User' },
            { name: 'Available' },
            { name: 'Granted' },
            { name: 'Provisioned' },
            { name: 'Provisioning' },
            { name: 'Failed' },
            { name: 'Deleted' }
        ];

        $scope.myOpt = $scope.opt[0];
        $scope.filterStatus = function (filterStatus, search) {
            $scope.getSearchUsers(filterStatus, search);
        }

        $scope.getSearchUsers = function (filterStatus, search) {
            $scope.temmp = [];
            var ss = $scope.status;
            //if (filterStatus != null && (search != "" || search != null)) {
            //    $scope.temmp2 = [];
            //    angular.forEach($scope.tempForAllUsers, function (value, key) {
            //        if (value.hasHours.toLowerCase().includes(filterStatus.toLowerCase()))
            //            $scope.temmp.push(value);
            //    });

            //    angular.forEach($scope.temmp, function (value, key) {
            //        if (value.FullNameEmail.toLowerCase().includes(search.toLowerCase()))
            //            $scope.temmp2.push(value);
            //    });

            //    $scope.Userlength = $scope.temmp2.length;
            //    $scope.totalCount = $scope.temmp2.length;
            //    $scope.loading = false;
            //    $scope.allUsers = $scope.temmp2;
            //}
            if (filterStatus.toLowerCase() == "all user" || filterStatus == "") {
                if (search != undefined && search != "" && search != null) {
                    angular.forEach($scope.tempUsers1, function (value, key) {
                        if (value.FullNameEmail.toLowerCase().includes(search.toLowerCase()))
                            $scope.temmp.push(value);
                    });

                    $scope.Userlength = $scope.temmp.length;
                    $scope.totalCount = $scope.temmp.length;
                    $scope.loading = false;
                    $scope.allUsers = $scope.temmp;
                }
                else {
                    angular.forEach($scope.tempUsers1, function (value, key) {
                        angular.forEach($scope.tempForAllUsers, function (value2, key2) {
                            if (value2.selected == true && value2.Email == value.Email)
                                $scope.tempUsers1[key].selected = true;
                        });
                    });

                    $scope.Userlength = $scope.tempUsers1.length;
                    $scope.totalCount = $scope.tempUsers1.length;
                    $scope.loading = false;
                    $scope.allUsers = $filter('orderBy')($scope.tempUsers1, 'Email', false) ;
                }
            }
            else if ((filterStatus != undefined || filterStatus != null) && (search == undefined || search == "" || search == null)) {
                angular.forEach($scope.tempUsers1, function (value, key) {
                    if (value.hasHours.toLowerCase().includes(filterStatus.toLowerCase()))
                        $scope.temmp.push(value);
                });
                //angular.forEach($scope.tempForAllUsers, function (value, key) {
                //    if (value.hasHours.toLowerCase().includes(filterStatus.toLowerCase()))
                //        $scope.temmp.push(value);
                //});
               
                $scope.Userlength = $scope.temmp.length;

                $scope.totalCount = $scope.temmp.length;
                $scope.loading = false;
                $scope.allUsers = $scope.temmp;
            }
            else if (search != undefined || search != "" || search != null) {
                if (filterStatus != undefined || filterStatus != null) {
                    angular.forEach($scope.tempUsers1, function (value, key) {
                        if (value.hasHours.toLowerCase().includes(filterStatus.toLowerCase()) && value.FullNameEmail.toLowerCase().includes(search.toLowerCase()))
                            $scope.temmp.push(value);
                    });
                    $scope.Userlength = $scope.temmp.length;

                    $scope.totalCount = $scope.temmp.length;
                    $scope.loading = false;
                    $scope.allUsers = $scope.temmp;
                } else {
                    $scope.temmp2 = [];
                    angular.forEach($scope.tempForAllUsers, function (value, key) {
                        if (value.FullNameEmail.toLowerCase().includes(search.toLowerCase()))
                            $scope.temmp.push(value);
                    });

                    angular.forEach($scope.temmp, function (value, key) {
                        if (value.hasHours.toLowerCase().includes(filterStatus.toLowerCase()))
                            $scope.temmp2.push(value);
                    });

                    $scope.Userlength = $scope.temmp2.length;
                    $scope.totalCount = $scope.temmp2.length;
                    $scope.loading = false;
                    $scope.allUsers = $scope.temmp2;
                }
            }
            //else if (filterStatus == null || search == "") {

            //    $scope.Userlength = $scope.tempForAllUsers.length;
            //    $scope.totalCount = $scope.tempForAllUsers.length;
            //    $scope.loading = false;
            //    $scope.allUsers = $scope.tempForAllUsers;
            //}
            else {
                $scope.allUsers = [];

                svc.checkVMUser(VEProfileID, $scope.pagination.currentPage, $scope.pagination.pageSizeSelected, filterStatus, search).then(function (response) {
                    angular.forEach(response.MachineUsers, function (value, key) {
                        if (response.MachineUsers[key].FullNameEmail.toLowerCase().indexOf(search.toLowerCase()) != -1) {
                            $scope.allUsers.push(value);


                        }
                    });
                    const key = 'UserId';
                    $scope.allUsers = [...new Map($scope.allUsers.map(item =>
                        [item[key], item])).values()];


                    $scope.Userlength = $scope.allUsers.length;
                    $scope.loading = false;
                    $scope.totalCount = $scope.allUsers.length;
                });

            }

        }
    }
})();