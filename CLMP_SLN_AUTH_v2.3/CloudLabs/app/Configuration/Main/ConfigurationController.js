(function () {
    "use strict";
    angular.module("app-configuration")
        .controller("ConfigurationController", ConfigurationController);

    ConfigurationController.$inject = ['svc', '$scope', '$uibModal'];
    function ConfigurationController(svc, $scope, $uibModal) {
        $scope.isUserAdmin = userIsAdmin;
        $scope.isUserSuperAdmin = userIsSuperAdmin;
        if (apiUrl === "http://localhost:36135/api/" || apiUrl === "https://staging-cloudlabs.cloudswyft.com/scl/api/") {
            $scope.isTenantLayerShow = true;
        } else {
            $scope.isTenantLayerShow = false;
        }

        $scope.toShow = false;
        $scope.VETypeHolder = null;
        $scope.CloudProviderHolder = null;
        $scope.fillVEType = function () {
            $scope.$emit('LOAD');
            svc.getVETypes().then(
                function (response) {
                    $scope.VETypeHolder = response;
                });
        };
        $scope.fillVEType();

        $scope.fillCloudProvider = function () {
            svc.getCloudProviders().then(
                function (response) {
                    $scope.CloudProviderHolder = response;
                    $scope.$emit('UNLOAD');
                });
        };
        $scope.fillCloudProvider();

        if (userIsSuperAdmin && currentEmail.includes("cloudswyft.com"))
            $scope.toShow = true;

        $scope.openModal = function (controller_purpose) {

            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Configuration/Modal/configurationModal.html',
                windowClass: 'my-modal-class',
                backdrop: 'static',
                keyboard: false,
                controller: 'configurationModalController',
                resolve: {
                    data: function () {
                        return {
                            purpose: function () {
                                return controller_purpose;
                            },
                            CloudProvidersCarrier: function () {
                                return $scope.CloudProviderHolder;
                            },
                            VETypesCarrier: function () {
                                return $scope.VETypeHolder;
                            }
                        };
                    }
                }
            });

        };
        $scope.openModalTenant = function (controller_purpose) {

            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Configuration/Modal/configurationModal.html',
                windowClass: 'my-modal-class-tenant',
                backdrop: 'static',
                keyboard: false,
                controller: 'configurationModalController',
                resolve: {
                    data: function () {
                        return {
                            purpose: function () {
                                return controller_purpose;
                            }
                        };
                    }
                }
            });

        };
        $scope.openModalUserGroup = function (controller_purpose) {

            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Configuration/Modal/configurationModal.html',
                windowClass: 'my-modal-class',
                backdrop: 'static',
                keyboard: false,
                controller: 'configurationModalController',
                resolve: {
                    data: function () {
                        return {
                            purpose: function () {
                                return controller_purpose;
                            }
                        };
                    }
                }
            });

        };
        $scope.openModalTenantNew = function (controller_purpose) {
            var modal = $uibModal.open({
                animation: true,
                templateUrl: '/app/Configuration/Modal/configurationModal.html',
                windowClass: 'my-modal-class',
                backdrop: 'static',
                keyboard: false,
                controller: 'configurationModalController',
                resolve: {
                    data: function () {
                        return {
                            purpose: function () {
                                return controller_purpose;
                            }
                        };
                    }
                }
            });

        };
    }
})();




