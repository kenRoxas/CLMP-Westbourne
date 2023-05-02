(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('CourseGradeModalController', CourseGradeModalController);

    CourseGradeModalController.$inject = ['$uibModalInstance', '$scope', 'svc', 'VEProfileID', 'VETypeID', '$uibModal', 'VEName', 'VEDescription', '$timeout', '$interval'];

    function CourseGradeModalController($uibModalInstance, $scope, svc, VEProfileID, VETypeID, $uibModal, VEName, VEDescription, $timeout, $interval) {

        var roles = ["Student"];
        $scope.allUsers = [];
        $scope.users = [];
        $scope.VEName = VEName;
        $scope.loading = true;
        

        $scope.roleSelected = ["YES", "NO"];
        var loadPage = function () {
            svc.getGradeUser(roles, VEProfileID, currentUserGroup, groupCode, VEDescription)
                .then(function (response) {
                    angular.copy(response, $scope.allUsers);
                    $scope.length = $scope.allUsers.length;
                    $scope.users = $scope.allUsers.slice(0, $scope.itemsPerPage);
                    //svc.getUsersWithLabHourExtensions(VEProfileID)
                    //    .then(function (resp) {
                    //        for (var i = 0; i < length; i++) {
                    //            if ((new Date(resp[i].StartDate).toISOString() <= new Date().toISOString()) && (new Date(resp[i].EndDate).toISOString() > new Date().toISOString()) && resp[i].ExtensionTypeId == 2) {
                    //                $scope.users[i].isExtend = true;
                    //                //$scope.allUsers[i].isExtend = true;
                    //            }
                    //        }
                    //    });

                    $scope.loading = false;

                    if (response.length === 0) {
                        $scope.noUsersinLabProfiles = true;
                    } else {
                        $scope.noUsersinLabProfiles = false;
                    }
                });

            $timeout(function () {
                loadPage();
            }, 60000);
        };

        loadPage();

        $scope.close = function () {
            $uibModalInstance.close();
        };
        $scope.changeUsers = [];


        var userGrade = [];
        $scope.changeIsComing = function (user) {
            var index;
            angular.forEach(userGrade, function (value, key) {
                if (value.Email === user.Email) {
                    index = key;
                }
            });

            if (index > -1)
                userGrade.splice(index, 1);
            userGrade.push(user);
        };
        var items = {
            title: "Lab Grade",
            message: "Are you sure you want to save?"
        };
       
        $scope.data = {
            grade: [
                { grade: -1, Name: 'Not Graded' },

                { grade: 1, Name: 'Passed' },

                { grade: -0, Name: 'Failed' }
            ]
        };

        $scope.save = function () {
            var modal = $uibModal.open({
                templateUrl: '/app/Common/ConfirmationModal.html',
                controller: "ConfirmationModalController",
                controllerAs: '$confmodal',
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return items;
                    },
                    userGroup: function () {
                        return 0;
                    },
                    VEProfileID: function () {
                        return VEProfileID;
                    },
                    userGrade: function () {
                        return userGrade;
                    },
                    VEName: function () {
                        return VEName;
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
        $scope.currentPage = 1;
        $scope.itemsPerPage = 12;
        $scope.maxSize = 10;
        $scope.pageChanged = function () {
            $scope.users = ($scope.currentPage - 1) * $scope.itemsPerPage;
            //$scope.allUsers = ($scope.currentPage - 1) * $scope.itemsPerPage;
        };

        $scope.openImage = function (Id, thumbnail, name) {
            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabProfiles/Modal/ImageGradeView.html',
                controller: 'ImageGradeController',
                windowClass: 'create-modal-design',
                size: 'xl',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    thumbnail: function () {
                        return thumbnail;
                    },
                    name: function () {
                        return name;
                    },
                    VEDescription: function () {
                        return VEDescription;
                    },
                    veprofileid: function () {
                        return VEProfileID;
                    },
                    Id: function () {
                        return Id;
                    }
                }
            });
        };

        $scope.startVM = function (resourceId) {
            svc.machineLogs(resourceId, "", "StartLABREMOTE").then(function (response) {
                svc.vmOperation(resourceId, "Start", "Instructor").then(function (response) { });
             });

            //svc.startVM(userid, veProfileId)
            //    .then(function (response) {
            //    });
        };


        $scope.openRemote = function (guacSrc, veprofileid, userid, hoursRemaining, isStarted, resourceId) {
            if (VETypeID === 5)
                window.open(guacSrc);
            else {
                $scope.startVM(resourceId);
                $uibModal.open({
                    templateUrl: '/app/LabProfiles/Modal/RemoteLabAccessView.html',
                    controller: "RemoteLabAccessController",
                    size: 'md',
                    backdrop: 'static',
                    windowClass: 'app-modal-window',
                    keyboard: true,
                    resolve: {
                        guacsrc: function () {
                            return guacSrc;
                        },
                        userid: function () {
                            return userid;
                        },
                        veprofileid: function () {
                            return veprofileid;
                        },
                        hoursRemaining: function () {
                            return hoursRemaining;
                        },
                        isStarted: function () {
                            return isStarted;
                        },
                        resourceId: function () {
                            return resourceId;
                        }
                    }
                });
            }
           
        };

        //$scope.checkIfShuttingDown = function () {
        //    svc.getGradeUser(roles, VEProfileID, currentUserGroup, VEName, groupCode)
        //        .then(function (response) {
        //            if (response.length !== 0) {
        //                angular.copy(response, $scope.allUsers);
        //            }
        //        });
        //};

        //$scope.checkIfShuttingDown();
        //$interval($scope.checkIfShuttingDown, 5000);

    }

})();