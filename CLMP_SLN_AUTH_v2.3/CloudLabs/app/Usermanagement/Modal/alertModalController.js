(function () {

    "use strict";

    angular.module('app-Usermanagement').controller('alertModalController', alertModalController);
    
    alertModalController.$inject = ['$uibModalInstance', 'items', 'svc', '$scope', '$uibModal', '$http', '$route', '$window', '$rootScope'];

    function alertModalController($uibModalInstance, items, svc, $scope, $uibModal, $http, $route, $window, $rootScope) {
        $scope.message = items.message;
        $scope.title = items.title;
        $scope.noCloseCancel = items.noCloseCancel;
        var type = items.type;
        $scope.isBulk = false;
        if (type == "Bulk")
            $scope.isBulk = true;

        $scope.confirm = function () {
            switch (type) {
                case "delete": {
                    svc.deleteUser($scope.id)
                        .then(function (response) {
                        });
                    $uibModalInstance.close(true);
                    $route.reload();
                    break;
                }
                case "enable": {
                    $rootScope.$emit("loadPage", {});
                    svc.disableUser($scope.id, false)
                        .then(function (response) {
                            $rootScope.$emit("CallParentMethod", {});
                        });

                    $uibModalInstance.close(true);
                    break;
                } 
                case "disable": {
                    $rootScope.$emit("loadPage", {});

                    svc.disableUser($scope.id, true)
                        .then(function (response) {
                            $rootScope.$emit("CallParentMethod", {});
                        });
                    $uibModalInstance.close(true);
                    break;
                }
                case "Create": {
                    $rootScope.$emit("CallParentMethod", {});
                    $uibModalInstance.close(true);
                    break;
                } 
                case "Edit": {
                    $rootScope.$emit("CallParentMethod", {});
                    $uibModalInstance.close(true);                     
                    break;
                }
                case "Add another": {
                    $rootScope.$emit("CallParent1Method", {});
                    $uibModalInstance.close();
                    $route.reload();
                    break;
                }
                case "DeletedDisable": {
                    $window.location.href = '/Account/Login';
                    break;
                }
                case "Bulk": {
                    $rootScope.$emit("CallParentMethod", {});
                    $uibModalInstance.close(true);  
                    break;
                }
            }
        };

        $scope.load = function () {
            switch (type) {
                case "delete": {
                    $scope.firstname = items.userDetails.Firstname;
                    $scope.lastname = items.userDetails.Lastname;
                    $scope.warnMessage = items.warnMessage;
                    $scope.email = items.userDetails.Email;
                    $scope.id = items.userDetails.Id;
                    $scope.change = false;
                    $scope.isShow = true;
                    $scope.toShow = true;
                    $scope.noCloseCancel = "No";
                    break;
                }
                case "enable":
                case "disable": {
                    $scope.firstname = items.userDetails.Firstname;
                    $scope.lastname = items.userDetails.Lastname;
                    $scope.email = items.userDetails.Email;
                    $scope.id = items.userDetails.Id;
                    $scope.change = false;
                    $scope.isShow = true;
                    $scope.toShow = true;
                    $scope.noCloseCancel = "No";
                    break;
                }                
                case "Create": {
                    $scope.isShow = false;
                    $scope.toShow = false;
                    $scope.noCloseCancel = "Close";
                    $scope.change = true;

                    svc.createUser(items.user)
                        .then(function (response) {
                    });
                    break;
                }
                case "Edit": {
                    $scope.isShow = false;
                    $scope.toShow = false;
                    $scope.noCloseCancel = "Close";
                    $scope.change = true;
                    svc.editUser(items.user)
                        .then(function (response) {                            
                        });
                    
                    break;
                }
                case "Add another": {
                    $scope.isShow = false;
                    $scope.toShow = false;
                    $scope.change = true;
                    $scope.noCloseCancel = "Close";
                    svc.createUser(items.user)
                        .then(function (response) {
                        });

                    break;
                }
                case "DeletedDisable": {
                    $scope.isShow = false;
                    $scope.toShow = false;
                    $scope.change = false;
                    $scope.noCloseCancel = "Close";
                    break;
                }
                case "Bulk": {
                    svc.uploadBulkCreateUser(items.file).then(
                    function (response) {
                    }
                );
                    break;
                }
            }
        };

        $scope.load();

        $scope.close = function () {
            $uibModalInstance.close(true);  
        };

    }
})();