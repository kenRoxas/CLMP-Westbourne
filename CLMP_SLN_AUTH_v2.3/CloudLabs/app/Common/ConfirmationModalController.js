(function () {
    "use strict";

    angular.module('app-labprofiles')
        .controller('ConfirmationModalController', ConfirmationModalController);
    
    ConfirmationModalController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'items', 'svc', '$uibModalStack', 'userGroup', 'VEProfileID', 'userGrade', 'VEName', 'userid', 'resourceId', 'labhourdata','$window'];
    
    function ConfirmationModalController($uibModalInstance, $rootScope, $scope, items, svc, $uibModalStack, userGroup, VEProfileID, userGrade, VEName, userid, resourceId, labhourdata, $window) {
        $scope.title = items.title;
        $scope.message = items.message;     
        $scope.messageType = items.messageType;
        $scope.isBulk = false;
        if (items.title == "Bulk Provision")
            $scope.isBulk = true;
        var GroupID = userGroup;     
        $scope.name = "";

        if (items.Names != undefined) {
            angular.forEach(items.Names, function (value, key) {
                if (key + 1 == items.Names.length)
                    $scope.name += value;
                else
                    $scope.name += value + ', ';
            });
        }

        $scope.confirm = function () {
            $uibModalInstance.close(true);
            $uibModalStack.dismissAll();
            $rootScope.$emit('fauxLoad');
            if (items.type === 'delete') {
                //$scope.centered = true;
                svc.deleteLabProfile(items).then(function (response) {
                    $("html").removeAttr('style');
                    $rootScope.$emit('loadContent');
                })
                    .finally(function () {
                    });
            }
            else if (items.type === 'create') {
                Save("", items);    
            }
            else if (items.type === 'edit') {
                Save(items.VEProfileID, items);
            }
            else if (items.type === 'shutdown' || $scope.messageType === 'clickClose') {
                svc.machineLogs(resourceId, "", "StopLABREMOTE").then(function (response) {
                    svc.vmOperation(resourceId, "Stop", "Student").then(function (response) { });
                    $rootScope.$emit('loadContent');

                });
                //svc.shutdownVm(1, VEProfileID, userid)
                //    .then(function (response) {
                //    }); $rootScope.$emit('loadContent');  
            }
            else { // grade
                svc.createUserGrade(VEProfileID, userGrade)
                    .then(function (response) {
                        $rootScope.$emit('loadContent');
                    })
                    .finally(function () {

                    });
            }                
        };

        $scope.confirmSaveLabHourExtension = function () {
            if (items.type === 'Extend') {
                $uibModalInstance.dismiss('cancel');
                svc.saveLabHourExtension(labhourdata)
                    .then(function (response) {
                        $rootScope.$emit('extensionUserLoading');
                    });
            }
        };

        $scope.confirmDeleteLabHourExtension = function () {
            if (items.type === 'DeleteExtend') {
                $uibModalInstance.dismiss('cancel');
                svc.deleteLabHourExtensionById(labhourdata)
                    .then(function (response) {
                        $rootScope.$emit('extensionUserLoading');
                    });
            }
        };

        $scope.confirmUpdateLabHourExtension = function () {
            if (items.type === 'UpdateExtend') {
                $uibModalInstance.dismiss('cancel');
                svc.updateLabHourExtension(labhourdata)
                    .then(function (response) {
                        $rootScope.$emit('extensionUserLoading');
                    });
            }
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
            if (items.type === 'ExtendSingle') {
                $rootScope.$emit('extensionUserLoading');
            }
        };
        $scope.close = function () {
            angular.element('.modal-dialog').hide();
            $window.location.reload();
            $uibModalInstance.dismiss('cancel');
        };

        var Save = function (ProfileId, info) {
            var labProfileDetails;
            if (info.type === 'create') {
                labProfileDetails =
                {
                    VEProfileID: ProfileId,
                    VirtualEnvironmentID: info.VirtualEnvironmentID,
                    Name: info.Name,
                    Description: info.Description,
                    ConnectionLimit: 100,
                    CourseID: 1,
                    ThumbnailUrl: null,
                    DateProvisionTrigger: "2016-04-01 00:00:00",
                    IsEnabled: 1,
                    Status: 0,
                    Remarks: null,
                    IsEmailEnabled: 0,
                    PassingRate: 0,
                    ExamPassingRate: 0,
                    ShowExamPassingRate: 0
                };
            }

            else if (info.type === 'edit') {
                labProfileDetails =
                {
                    VEProfileID: ProfileId,
                    VirtualEnvironmentID: info.VirtualEnvironmentID,
                    Name: info.Name,
                    Description: info.Description,
                    ConnectionLimit: info.ConnectionLimit,
                    CourseID: info.CourseID,
                    ThumbnailUrl: info.ThumbnailUrl,
                    DateProvisionTrigger: info.DateProvisionTrigger,
                    IsEnabled: info.IsEnabled,
                    Status: info.Status,
                    Remarks: info.Remarks,
                    IsEmailEnabled: info.IsEmailEnabled,
                    PassingRate: info.PassingRate,
                    ExamPassingRate: info.ExamPassingRate,
                    ShowExamPassingRate: info.ShowExamPassingRate
                };
            }


            if (!angular.isString(info.picFile)) {
                if (info.picFile === undefined)
                    info.picFile = "";
                svc.uploadImage(info.picFile).then(
                    function (response) {
                        labProfileDetails.ThumbnailUrl = response;
                        createLabProfile(labProfileDetails, info.type);
                    }
                );
            }
            else {
                labProfileDetails.ThumbnailUrl = info.ThumbnailUrl;
                createLabProfile(labProfileDetails, info.type);
            }

        };

        var createLabProfile = function (labProfileDetails, type) {
            svc.createLabProfile(labProfileDetails, type, GroupID)
                .then(function (response) {
                    $uibModalInstance.close(response);
                    $("html").removeAttr('style');
                    $uibModalStack.dismissAll();
                    $rootScope.$emit('fauxLoad');

                    var addLabActivity = {
                        VEProfileID: response.VEProfileID,
                        LabActivities: items.labActivities
                    };

                    if (type === 'create') {
                        svc.bindLabActivities(addLabActivity)
                            .then(function (response) {
                                return response;
                            });
                    }

                    else if (type === 'edit') {
                        svc.updateLabActivities(addLabActivity)
                            .then(function (response) {
                                return response;
                            });
                    }
                    $rootScope.$emit('loadContent');
                });
        };


     }

  

})();