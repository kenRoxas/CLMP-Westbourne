(function (angular) {
    'use strict';
    angular.module('app-labsession')
        .controller('UserNotificationController', UserNotificationController);

    UserNotificationController.$inject = ['$scope', '$uibModalInstance', '$rootScope', '$route', '$uibModalStack', 'message', 'type', 'UserId', 'ProfileId', 'svc', 'imageInfo', '$uibModal', 'resourceId'];

    function UserNotificationController($scope, $uibModalInstance, $rootScope, $route, $uibModalStack, message, type, UserId, ProfileId, svc, imageInfo, $uibModal, resourceId) {
        $scope.message = message;
        $scope.type = type;
        $scope.image = imageInfo;

        $scope.close = function () {
            if ($scope.type === "close" || $scope.type === "inactive") {
                $rootScope.$emit('reload');
                $uibModalStack.dismissAll();
            }
            else if ($scope.type === "fifteen") {
                $uibModalInstance.dismiss();
            }
            else if ($scope.type === "shutdown") {
                //svc.toggleVM(UserId, ProfileId, 2)
                //    .then(function (response) { });
                svc.machineLogs(resourceId, "", "Trigger STOP").then(function (response) {

                });

                svc.vmOperation(resourceId, "Stop", "Student")
                    .then(function (response) {
                        $route.reload();
                    });

                $rootScope.$emit('reload');
                //$rootScope.$emit('shutdown');
                //$("#wrapper").css({ "display": "block" });
                //$("#sidebar").css({ "display": "block" });
                $uibModalStack.dismissAll();
            }
            else if ($scope.type === "nohours") {
                //svc.toggleVM(UserId, ProfileId, 2)
                //    .then(function (response) { });
                svc.machineLogs(resourceId, "", "NOHOURS").then(function (response) { });

                svc.vmOperation(resourceId, "Stop", "Student")
                    .then(function (response) { });

                $rootScope.$emit('shutdown');
                $("#wrapper").css({ "display": "block" });
                $("#sidebar").css({ "display": "block" });
                $uibModalStack.dismissAll();
            }
            if ($scope.type === "Provisioning") {
                $rootScope.$emit('reload');
                $uibModalStack.dismissAll();
            }
            if ($scope.type === "Starting") {
                $rootScope.$emit('reload');
                $uibModalStack.dismissAll();
            }
            else {
                $uibModalInstance.dismiss();
            }           
        };

        $scope.uploadFinish = function (type) {
            $rootScope.$emit('upload-finish');

            $uibModalInstance.dismiss();
            if (type == 'console')
                $rootScope.$emit('reload');
        };

        $scope.upload = function () {
            svc.imageUpload(imageInfo.file, imageInfo.imageFilename, currentId, groupCode).then(
                function (response) {
                    if (response) {
                        $uibModal.open({
                            templateUrl: '/app/LabSession/RenderPage/UserNotificationView.html',
                            controller: "UserNotificationController",
                            size: 'md',
                            backdrop: 'static',
                            keyboard: false,
                            windowClass: 'notification-modal',
                            resolve: {
                                imageInfo: function () {
                                    return null;
                                },
                                message: function () {
                                    return "Your image is now uploaded. You may now contact your instructor for the results.";
                                },
                                type: function () {
                                    return null;
                                },
                                UserId: function () {
                                    return null;
                                },
                                ProfileId: function () {
                                    return null;
                                },
                                resourceId: function () {
                                    return resourceId;
                                }
                            }
                        });
                    }
                }
            );
            $uibModalInstance.dismiss();

        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss();
        };
    }
})(window.angular);
