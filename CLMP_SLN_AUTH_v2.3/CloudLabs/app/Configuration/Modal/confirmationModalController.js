(function () {

    "use strict";

    angular.module('app-configuration').controller('confirmationModalController', confirmationModalController);

    confirmationModalController.$inject = ['$uibModalInstance', 'items', 'svc', '$window', '$route', '$uibModalStack', '$scope', '$uibModal', '$rootScope'];

    function confirmationModalController($uibModalInstance, items, svc, $window, $route, $uibModalStack, $scope, $uibModal, $rootScope) {
        $scope.$on('LOAD', function () { $scope.loader = false; $scope.oppositeLoader = false; });
        $scope.$on('UNLOAD', function () { $scope.loader = true; $scope.oppositeLoader = true; });
        var $confmodal = this;
        $confmodal.title = items.headTitle;
        $confmodal.message = items.message;

        $confmodal.confirm = function () {

            if (items.type === "create") {
                $scope.$emit('LOAD');
                if (items.reason === "createVirtualEnvironment") {
                    svc.createVirtualEnvironments(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "createVirtualEnvironmentImages") {
                    svc.addVirtualEnvironmentImages(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "createTenant") {
                    svc.addTenant(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "addUserGroup") {
                    svc.addUserGroup(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "addSubsciptionCredit") {
                    svc.addSubsciptionCredit(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "createTenantNew") {
                    svc.addTenantNew(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                } else if (items.reason === "createAutoDeletion") {
                    svc.addorEditScheduleForAutoDeletion(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                } else if (items.reason === "createBusinessType") {

                    var CreateEditBusinessGroup = {
                        "BusinessGroup": items.BusinessGroup,
                        "UserGroupName": items.UserGroupName,
                        "ModifiedValidity": items.Validity,
                        "CreatedBy": items.CreatedBy
                    };

                    svc.saveBusinessGroup(CreateEditBusinessGroup).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
            }
            else if (items.type === "edit") {

                $scope.$emit('LOAD');
                if (items.reason === "editVirtualEnvironment") {
                    svc.updateVirtualEnvironments(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "editTime") {
                    svc.setVMConfig(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "editRole") {
                    svc.updateAspNetRole(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "editVirtualEnvironmentImages") {
                    svc.editVirtualEnvironmentImages(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "editTenant") {
                    svc.editTenant(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "editUserGroup") {
                    svc.editUserGroup(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "editSubsciptionCredit") {
                    svc.editSubsciptionCredit(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD'); c
                }
                else if (items.reason === "editTenantNew") {
                    svc.updateTenantNew(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                } else if (items.reason === "editAutoDeletion") {
                    svc.addorEditScheduleForAutoDeletion(items).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        });
                    $scope.$emit('UNLOAD');
                }
            }
            else if (items.type === "delete") {
                $scope.$emit('LOAD');
                if (items.reason === "deleteVirtualEnvironment") {
                    svc.deleteVirtualEnvironments(items.id).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        }
                    );
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "deleteVirtualEnvironmentImage") {
                    svc.deleteVirtualEnvironmentImages(items.id).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        }
                    );
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "deleteTenant") {
                    svc.deleteTenant(items.id).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        }
                    );
                    $scope.$emit('UNLOAD');
                }
                else if (items.reason === "deleteUserGroup") {
                    svc.deleteUserGroup(items.id, items.GroupName).then(
                        function (response) {
                            $uibModalInstance.close(true);
                            $uibModalStack.dismissAll();
                        }
                    );
                    $scope.$emit('UNLOAD');
                }
            }
        };

        $confmodal.cancel = function () {
            $uibModalInstance.close();
        };
    }
})();