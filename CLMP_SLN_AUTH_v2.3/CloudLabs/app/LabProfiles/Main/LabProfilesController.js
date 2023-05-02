
(function (angular) {
    'use strict';
    angular.module('app-labprofiles')
        .controller('LabProfilesController', LabProfilesController);
    LabProfilesController.$inject = ['$scope', '$filter', '$window', 'svc', '$uibModal', '$route', '$rootScope', '$document', '$timeout', '$interval'];


    function LabProfilesController($scope, $filter, $window, svc, $uibModal, $route, $rootScope, $document, $timeout, $interval) {

        $scope.isUserSuperAdmin = userIsSuperAdmin;
        $scope.isUserAdmin = userIsAdmin;
        $scope.isUserInstructor = userIsInstructor;
        $scope.isUserStaff = userIsStaff;
        $scope.VETypes = [];
        $scope.VETemplates = [];
        $scope.labActivities = [];
        $scope.courseIds = [];
        $scope.profiles = [];
        $scope.empty = false;
        $scope.view = false;
        $scope.info = [];
        $scope.loading = true;
        $scope.loadingContent = false;
        var UserGroup = currentUserGroup;
        $scope.title = '';
        var backup = [];
        var query = "";
        var pageSize = 0;
        var pageNum = 1;
        var chosenName = [];
        var chosenIds = [];
        var chosenType = [];
        var courseIds = 0;
        var activities = [];
        var chosenActivities = [];
        var startIndex = 0;
        var right = [];
        var length = 0;
        var htmlRef = angular.element($document[0].html);
        $scope.regionGCP = "";

        $scope.search = function (item) {
            if (!$scope.searchEntry || (item.Name.toUpperCase().indexOf($scope.searchEntry.toUpperCase()) !== -1)) {
                return true;
            }
            return false;
        };

        $scope.filterDeleted = function (el) {
            return !el.flagDeleted;
        };

       
        var onLoadPage = function () {

            $scope.VETypes = [];
            $scope.VETemplates = [];
            $scope.labActivities = [];
            $scope.courseIds = [];
            $scope.profiles = [];
            $scope.empty = false;
            $scope.view = false;
            $scope.info = [];
            //
            var query = "";

            svc.getTenantGCPRegion(userTenantId).then(function (resp) {
                //angular.copy(resp, $scope.regionGCP);
                $scope.regionGCP = resp;
            });

            right = [];
            length = 0;
            svc.getLabProfiles(UserGroup)
                .then(function (response) {
                    angular.copy(response, $scope.courseIds);
                    svc.getVETypes()
                        .then(function (response) {
                            angular.copy(response, $scope.VETypes);
                            angular.forEach($scope.VETypes, function (key, value) {
                                svc.getVETemplates(key["VETypeID"], currentUserGroup)
                                    .then(function (response) {
                                        $scope.VETemplates.push(response);
                                    });
                            });

                            svc.getLabActivities()
                                .then(function (response) {
                                    angular.copy(response.LabActivities, $scope.labActivities);

                                    right = $scope.labActivities;
                                    length = $scope.labActivities.length;
                                    angular.copy(response.LabActivities, backup);
                                    if ($scope.VETemplates.length !== 0 && $scope.courseIds.length !== 0) {
                                        if ($scope.courseIds.length > 0) {
                                            for (var i = 0; i < $scope.courseIds.length; i++) {
                                                if (($scope.isUserInstructor || $scope.isUserStaff) && ($scope.courseIds[i].VirtualEnvironment.VETypeID === 1 || $scope.courseIds[i].VirtualEnvironment.VETypeID === 2 || $scope.courseIds[i].VirtualEnvironment.VETypeID === 5 || $scope.courseIds[i].VirtualEnvironment.VETypeID === 8 || $scope.courseIds[i].VirtualEnvironment.VETypeID === 9 || $scope.courseIds[i].VirtualEnvironment.VETypeID === 10))
                                                    $scope.profiles.push($scope.courseIds[i]);
                                                if ($scope.isUserSuperAdmin || $scope.isUserAdmin)
                                                    $scope.profiles.push($scope.courseIds[i]);
                                                //if () {
                                                //    if ($scope.courseIds[i].VirtualEnvironment.VETypeID !== 3 && $scope.courseIds[i].VirtualEnvironment.VETypeID !== 4)
                                                //        $scope.profiles.push($scope.courseIds[i]);
                                                //}
                                            }
                                        }
                                        //else {
                                        //    for (var i = 0; i < 12; i++) {
                                        //        if ($scope.isUserInstructor && ($scope.courseIds[i].VirtualEnvironment.VETypeID === 1 || $scope.courseIds[i].VirtualEnvironment.VETypeID === 2))
                                        //            $scope.profiles.push($scope.courseIds[i]);
                                        //        if ($scope.isUserAdmin || $scope.isUserSuperAdmin)
                                        //            $scope.profiles.push($scope.courseIds[i]);
                                        //    }
                                        //}
                                    }

                                    $scope.loading = false;
                                    $scope.loadingContent = false;
                                    if ($scope.profiles.length === 0 && $scope.loading === false) {
                                        $scope.empty = true;
                                    }
                                });
                        });
                });
        };


        onLoadPage();

        $scope.cname = function (cName) {
            if (cName.length > 42) {
                var wordChop = cName.trim().substring(0, 42).split(" ").slice(0, -1).join(" ") + "…";
                return wordChop;
            }
            else
                return cName;
        };

        $scope.loadMore = function () {
            var last = $scope.profiles.length;
            for (var i = 0; i < 4; i++) {
                if (last + i === $scope.courseIds.length)
                    return;
                $scope.profiles.push($scope.courseIds[last + i]);
            }
        };

        $scope.searchLabProfile = function () {
            if ($scope.entry !== null) {
                svc.searchLabProfiles($scope.entry, pageSize)
                    .then(function (response) {
                        angular.copy(response.VEProfiles, $scope.courseIds);
                    });
            }
            else {
                svc.getLabProfiles(pageSize, pageNum, courseIds)
                    .then(function (response) {
                        angular.copy(response.VEProfiles, $scope.courseIds);
                    });
            }

        };

        $scope.openModal = function (type, info, VETypes, VETemplates, labActivities) {
            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabProfiles/Modal/CreateModal.html',
                controller: 'CreateModalController',
                windowClass: 'create-modal-design',
                size: 'xl',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    type: function () {
                        return type;
                    },
                    info: function () {
                        return info;
                    },
                    types: function () {
                        return VETypes;
                    },
                    templates: function () {
                        return VETemplates;
                    },
                    activities: function () {
                        return activities;
                    },
                    activityNames: function () {
                        return chosenName;
                    },
                    chosenVEType: function () {
                        return chosenType;
                    },
                    startIndex: function () {
                        return startIndex;
                    },
                    groupID: function () {
                        return UserGroup;
                    }
                }
            });
        };

        var info = [{ "name": "", "description": "" }];
        $scope.openCreateModal = function (type, index) {
            chosenName.length = 0;
            chosenIds.length = 0;
            chosenType.length = 0;
            chosenActivities.length = 0;

            $("html").css({ "overflow": "hidden" });
            htmlRef.addClass('ovh');
            angular.copy(backup, $scope.labActivities);
            if (type === 'create') {
                info = [];
                $scope.labActivities = $filter('orderBy')($scope.labActivities, 'Name', false);
                activities = [[], $scope.labActivities];

                startIndex = 0;
            }
            else {

                if (!$scope.searchEntry) {
                    var x = $scope.courseIds[index];
                    info = x;
                }
                else {
                    var x = index;
                    info = x;
                }


                svc.getProfileActivities(x.VEProfileID)
                    .then(function (response) {
                        angular.copy(response, chosenActivities);
                        angular.forEach(response, function (key, value) {
                            chosenName.push(activities[0][value].Name);
                            chosenIds.push(activities[0][value].LabActivityID);
                        });
                        angular.forEach(chosenActivities, function (value1, key1) {
                            angular.forEach($scope.labActivities, function (value2, key2) {
                                if (value1.LabActivityID === value2.LabActivityID) {
                                    $scope.labActivities.splice(key2, 1);
                                }
                            });
                        });
                        $rootScope.$emit('loaded');
                    });


                angular.forEach($scope.VETypes, function (key, value) {
                    if (key.VETypeID === info.VirtualEnvironment.VETypeID)
                        chosenType.push(key);
                });


                $scope.labActivities = $filter('orderBy')($scope.labActivities, 'Name', false);
                activities = [chosenActivities, $scope.labActivities];

                if (type === 'edit')
                    startIndex = 0;
                else if (type === 'view')
                    startIndex = 3;


            }

            $scope.openModal(type, info, $scope.VETypes, $scope.VETemplates, $scope.labActivities);

        };

        $scope.openConfirmationModal = function (type, para1, para2) {
            var modalData = {};

            switch (type) {
                case 'delete':
                    modalData = {
                        type: type,
                        message: "Are you sure you want to delete " + para2 + "?",
                        title: "Delete Lab Profile",
                        VEProfileID: para1,
                        GroupID: UserGroup,
                        Name: para2
                    };
                    break;
            }

            var modal = $uibModal.open({
                templateUrl: '/app/Common/ConfirmationModal.html',
                controller: "ConfirmationModalController",
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return modalData;
                    },
                    userGroup: function () {
                        return 0;
                    },
                    VEProfileID: function () {
                        return 0;
                    },
                    userGrade: function () {
                        return null;
                    },
                    VEName: function () {
                        return null;
                    },
                    userid: function () {
                        return null;
                    },
                    resourceId: function () {
                        return null;
                    },
                    labhourdata: function () {
                        return null;
                    }

                }
            });
        };

        $scope.openGrantLabModal = function (x, VEType, VEName) {
            $("html").css({ "overflow": "hidden" });
            htmlRef.addClass('ovh');
            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabProfiles/Modal/GrantLabAccessView.html',
                controller: 'GrantLabAccessController',
                size: 'xl',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    VEProfileID: function () {
                        return x;
                    },
                    VEType: function () {
                        return VEType;
                    },
                    VEName: function () {
                        return VEName;
                    }
                }

            });
        };

        $rootScope.$on('loadContent', function () {
            $scope.loadingContent = true;
            //$timeout(function () { onLoadPage(); }, 3000);
            $timeout(function () { $route.reload(); }, 3000);

            //$route.reload();

        });
        $rootScope.$on('fauxLoad', function () {
            $scope.loadingContent = true;
            $scope.empty = false;
        });

        //var checkIfProvisioning = function () {
        //    svc.getVirtualMachineMappings()
        //        .then(function (response) {
        //            var d = new Date();
        //            if (response === 0 || response === null) {
        //                $scope.isProvisioning = false;
        //                $scope.isDisabled = false;
        //                $scope.title = ''; 
        //            } else {
        //                $scope.isProvisioning = true;
        //                $scope.isDisabled = true;
        //                $scope.title = 'Machine Provision is Ongoing.'; 
        //            }
        //        });

        //};
        //checkIfProvisioning();
        //$interval(checkIfProvisioning, 15000);

        $scope.openLabHours = function () {
            $("html").css({ "overflow": "hidden" });
            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabProfiles/Modal/LabHoursView.html',
                controller: 'LabHoursController',
                size: 'xl',
                backdrop: 'static',
                keyboard: false,
                windowClass: 'labhours-modal-window',
                resolve: {
                    data: {
                        cloudlabsgroupsid: function () {
                            return UserGroup;
                        }
                    },
                    region: function () {
                        return $scope.regionGCP;
                    }
                }
            });
        };


        $scope.courseGradeModal = function (VEProfileID, VETypeID, VEName, VEDescription) {
            $("html").css({ "overflow": "hidden" });

            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabProfiles/Modal/CourseGradeModal.html',
                controller: 'CourseGradeModalController',
                backdrop: 'static',
                size: 'xl',
                keyboard: false,
                resolve: {
                    VEProfileID: function () {
                        return VEProfileID;
                    },
                    VETypeID: function () {
                        return VETypeID;
                    },
                    VEName: function () {
                        return VEName;
                    },
                    VEDescription: function () {
                        return VEDescription;
                    }
                }

            });
        };

        $scope.openLabHourExtension = function (veprofileId, VEName) {

            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabProfiles/Modal/LabHoursExtensionView.html',
                controller: 'LabHourExtensionController',
                size: 'xl',
                windowClass: 'lab-hours-extension-modal',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    veprofileId: function () {
                        return veprofileId;
                    },
                    VEName: function () {
                        return VEName;
                    }
                }
            });
        };


        $scope.mytime = new Date();

        $scope.hstep = 1;
        $scope.mstep = 15;

        $scope.options = {
            hstep: [1, 2, 3],
            mstep: [1, 5, 10, 15, 25, 30]
        };

        $scope.ismeridian = true;
        $scope.toggleMode = function () {
            $scope.ismeridian = !$scope.ismeridian;
        };

        $scope.update = function () {
            var d = new Date();
            d.setHours(14);
            d.setMinutes(0);
            $scope.mytime = d;
        };

        $scope.changed = function () {
            $log.log('Time changed to: ' + $scope.mytime);
        };

        $scope.clear = function () {
            $scope.mytime = null;
        };

    }

})(window.angular);


