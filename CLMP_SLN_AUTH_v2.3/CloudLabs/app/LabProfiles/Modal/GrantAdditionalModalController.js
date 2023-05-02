(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('GrantAdditionalModalController', GrantAdditionalModalController).directive('unique', unique);

    unique.$inject = ['$q', '$timeout', 'svc'];
    function unique($q, $timeout, svc) {
        return {
            restrict: "A",
            scope: {},
            replace: false,
            link: function (scope, element, attrs, ctrl) {
                element.bind('keypress', function (event) {
                    var regex = new RegExp(/(^[a-zA-Z0-9][a-zA-Z0-9\-]{0,15})$/);
                    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
                    if (!regex.test(key)) {
                        event.preventDefault();
                        return false;
                    }
                });
            }
        };
    }   
    GrantAdditionalModalController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'svc', '$window', 'users', '$uibModal', 'items', 'creditMapping', 'VEType'];

    function GrantAdditionalModalController($uibModalInstance, $rootScope, $scope, svc, $window, users, $uibModal, items, creditMapping, VEType) {
        $scope.oldValue = "";
        $scope.allUser = users;
        $scope.userCount = users.length;
        $scope.btnDisabled = false;
        $scope.machines = [];
        $scope.elements = [];
        $scope.pattern = /^(?!-)[a-zA-Z0-9\-]{0,15}(?<!-)$/;
        var length = users.length;
        $scope.VMNames = [];
        $scope.users = [];
        var vmEmptyData = [];
        var userId = [];
        var countUser = users.length - 1;

        $scope.cancel = function () {
            for (var i = 0; i < length; i++)
                $scope.users[i].MachineName = '';
    
            $scope.myForm.$setUntouched;
            $uibModalInstance.close();
        };

        $scope.type = function (name) {
            var validVMName = [];  
            var isExists = false;
            if (name !== undefined)
                isExists = $scope.VMNames.includes(name.toUpperCase());
            for (var q = 0; q <= countUser; q++) {
                isExists = $scope.VMNames.includes(document.getElementById('machineName' + q).value.toUpperCase());
                if (document.getElementById('machineName' + q).value === "")
                    validVMName.push(document.getElementById('machineName' + q).value.toUpperCase());
                if (!validVMName.includes(document.getElementById('machineName' + q).value.toUpperCase()) && !isExists)
                    validVMName.push(document.getElementById('machineName' + q).value.toUpperCase());
            }
            if (validVMName.length === $scope.userCount && !isExists)
                $scope.isValid = false;
            else if (isExists || validVMName.length !== $scope.userCount)
                $scope.isValid = true;
            else
                $scope.isValid = false;
        };

        $scope.readVirtualNames = function() {
            svc.readVirtualEnvironments().then(
                function (response) {
                    for (var i = 0; i <= response.length; i++)
                    $scope.VMNames.push(response[i].Title.toUpperCase());
                });
        };

        $scope.allUserHours = function () {
            angular.forEach($scope.allUser, function (value, key) {
                if (value.hasHours == 'Available' || value.hasHours == 'Granted' || value.hasHours == 'Failed')
                    $scope.users.push(value);
            });
        };

        $scope.allUserHours();
        $scope.readVirtualNames();




        $scope.confirm = function () {
            var items = {
                title: "Grant Lab Access",
                message: "Are you sure you want to grant lab access to the selected users?",
                type: "create",
                isGrant: true
            };
           
            angular.forEach($scope.users, function (value, key) {
                var custData = {
                    userId: value.UserId,
                    machineName: value.MachineName.toUpperCase()
                }
                vmEmptyData.push(custData);
                userId.push(value.UserId);
            });

            var modal = $uibModal.open({
                templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                controller: "GrantConfirmationModalController",
                controllerAs: '$confmodal',
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    content: function () {
                        return userId;
                    },
                    creditMapping: function () {
                        return creditMapping;
                    },
                    items: function () {
                        return items;
                    },
                    VEType: function () {
                        return VEType;
                    },
                    VMEmptyData: function () {
                        return vmEmptyData;
                    }
                }
            });
        };

        $scope.isChange = function (index) {
            var machineName = '[name=machineName'+ index +']';
            var countUser = $scope.users.length - 1;
            $scope.boolChange = false;
            for (var i=0; i <= countUser; i++) {
                var compareTextbox = '[name=machineName' + i + ']';
                if (compareTextbox === machineName)
                    continue;
                else if (angular.equals($(compareTextbox).val().toUpperCase(), $(machineName).val().toUpperCase()))
                    return $scope.boolChange = true;
                else {
                    $scope.myForm.$setUntouched;
                    continue;
                }
            }
        };               
    }    

})();