(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('RemoteLabAccessController', RemoteLabAccessController);

    RemoteLabAccessController.$inject = ['$scope', 'svc', 'guacsrc', '$uibModal', 'veprofileid', 'userid', '$timeout', 'hoursRemaining', '$sce', 'isStarted', '$uibModalInstance', 'resourceId'];

    function RemoteLabAccessController($scope, svc, guacsrc, $uibModal, veprofileid, userid, $timeout, hoursRemaining, $sce, isStarted, $uibModalInstance, resourceId) {

        $scope.guacsrc = '';
        var ve = veprofileid;

        $scope.frameIsOpen = true;
        var fifteennotif = false;
        var zeronotif = false;
        $scope.hoursRemaining = hoursRemaining;
        $scope.inactive = false;
        $scope.stopLog = false;
        $scope.noCredits = false;
        $scope.loading = isStarted;
        $scope.isRenderFocus = false;
        $scope.status = "";
        $scope.focusMove = function () {
            angular.element('#iRender').focus();
            $scope.isRenderFocus = true;
        };

        $scope.notRenderFocus = function ($event) {
            var keyCode = $event.which || $event.keyCode;
            if (keyCode === 9) {
                $scope.isRenderFocus = false;   
            }
        }

        $scope.close = function (isLoading) {
            $scope.isRenderFocus = false;
            if (isLoading === 2 || isLoading === 0)
                $uibModalInstance.dismiss('cancel');
            else {
                var message = "Closing this will end your session and shutdown the machine. Do you wish to continue?";
                var messagetype = "clickClose";
                $scope.userNotification(message, messagetype, resourceId);
            }
        };

        $scope.ifStarted = function () {
            $scope.loading = 1;
            $scope.status = 'Starting';
            svc.getCourseVMs(userid)
                .then(function (response) {
                    angular.forEach(response, function (value) {
                        if (value.veprofileid == veprofileid) {
                            $scope.status = value.MachineStatus.toLowerCase();
                            if (value.IsStarted === 1 && value.MachineStatus.toLowerCase() == 'running') {
                                svc.setRunningBy(resourceId, 2).then(function (response) { });

                                svc.machineLogs(resourceId, "", "LaunchLABREMOTE").then(function (response) { });

                                $scope.loading = 1;
                                $scope.status = value.MachineStatus.toLowerCase();
                                //$scope.logTime();
                                $scope.guacsrc = $sce.trustAsResourceUrl(guacsrc);
                            }
                            else {
                                $scope.loading = value.IsStarted;
                                $scope.status = value.MachineStatus;
                                if (value.MachineStatus.toLowerCase() == 'deallocated' || value.MachineStatus.toLowerCase() == 'shutdown' || value.MachineStatus.toLowerCase() == 'stopped') {
                                    svc.machineLogs(resourceId, "", "StartLABREMOTE").then(function (response) {
                                        svc.vmOperation(resourceId, "Start", "Instructor").then(function (response) {
                                            //$scope.guacsrc = $sce.trustAsResourceUrl(guacsrc);
                                        });
                                    });
                                }
                                $timeout(function () {
                                    $scope.ifStarted();
                                }, 30000);
                                //}, 180000);
                            }
                        }
                    })

                });

            //svc.getProvisionedDetails(userid, veprofileid).then(function (response) {
            //    if (response[0]["IsStarted"] === 1) {
            //        $scope.loading = 1;
            //        $scope.logTime();
            //    }
            //    else {
            //        $scope.loading = response[0]["IsStarted"];
            //        $scope.startVM(userid, veprofileid);
            //        $timeout(function () {
            //            $scope.ifStarted();
            //        }, 15000);
            //    }

            //});

        };

        //$scope.startVM = function (userid, veprofileid) {
        //    svc.machineLogs(resourceId, "", "StartLABREMOTE").then(function (response) { });

        //    svc.vmOperation(i.ResourceId, "", "Start").then(function (response) { });
        //        //svc.startVM(userid, veprofileid)
        //    //    .then(function (response) {
        //    //    });
        //};

        $scope.ifStarted();

        $scope.userNotification = function (message, messagetype, resourceId) {
            var items = {
                message: message,
                messageType: messagetype,
                title: "Notification"
            };
            $("html").css({ "overflow": "hidden" });
            $uibModal.open({
                templateUrl: '/app/Common/ConfirmationModal.html',
                controller: "ConfirmationModalController",
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return items;
                    },
                    userGroup: function () {
                        return null;
                    },
                    VEProfileID: function () {
                        return veprofileid;
                    },
                    userGrade: function () {
                        return null;
                    },
                    VEName: function () {
                        return null;
                    },
                    userid: function () {
                        return userid;
                    },
                    resourceId: function () {
                        return resourceId;
                    },
                    labhourdata: function () {
                        return null;
                    }

                }
            });
        };

        //$scope.logTime = function () {
        //    if ($scope.frameIsOpen === true && $scope.stopLog === false) {
        //        svc.logTime(veprofileid, userid, true)
        //            .then(function (response) {
        //                hoursRemaining = response;
        //            });
        //    }
        //    $timeout(function () {
        //        $scope.logTime();
        //    }, 60000);
        //};

        $scope.shutdownVM = function (status) {
            if ($scope.stopLog === false) {
                //svc.logTime(veprofileid, userid, true)
                //    .then(function (response) {
                //    });
            }

            if ($scope.inactive === false) {
                svc.machineLogs(resourceId, "", "StopLABREMOTE(" + status + ")").then(function (response) { });

                svc.vmOperation(i.ResourceId, "Stop", "Instructor").then(function (response) { });
                //svc.startVM(userid, veprofileid)
            //    .then(function (response) {
            //    });
                //svc.shutdownVm(1, veprofileid, userid)
                //    .then(function (response) {
                //    });
                $scope.frameIsOpen = false;
            }
        };

        //$scope.checkLabCredits = function () {
        //    var message;
        //    var messagetype;
        //    $scope.isRenderFocus = false;
        //    if ($scope.frameIsOpen) {
        //        if ((hoursRemaining <= 15) && fifteennotif === false) {
        //            if (hoursRemaining === 1)
        //                message = "You only have " + hoursRemaining + " minute left. Please save your work";
        //            else
        //                message = "You only have " + hoursRemaining + " minutes left. Please save your work";
        //            messagetype = "warning";
        //            fifteennotif = true;
        //            $scope.userNotification(message, messagetype);
        //        }
        //        else if ((hoursRemaining <= 0) && zeronotif === false && $scope.inactive === false) {
        //            $scope.shutdownVM("nohours");
        //            $scope.stopLog = true;
        //            $scope.noCredits = true;
        //            message = "Your lab credits have ran out. Session is now closing.";
        //            messagetype = "shutdown";
        //            zeronotif = true;
        //            $scope.userNotification(message, messagetype);
        //        }
        //    }
        //    else if (hoursRemaining >= $scope.elapsed) {
        //        message = "Your lab credits have ran out. Session is now closing.";
        //        messagetype = "close";
        //        $scope.userNotification(message, messagetype);
        //    }
        //    $timeout(function () {
        //        $scope.checkLabCredits();
        //    }, 180000);
        //};


        //$timeout(function () {
        //    if (!$scope.loading) {
        //        //$scope.logTime();
        //        $scope.checkLabCredits();
        //    }
        //    //}, 60000);
        //}, 180000);
    }



})();