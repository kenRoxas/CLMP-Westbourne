(function () {

    "use strict";

    angular.module('app-labsession')
        .controller('WhiteListController', WhiteListController).directive('unique', unique);

    unique.$inject = ['$q', '$timeout', 'svc'];
    function unique($q, $timeout, svc) {
        return {
            restrict: "A",
            scope: {},
            replace: false,
            link: function (scope, element, attrs, ctrl) {
                element.bind('keyrelease', function (event) {
                    var regex = new RegExp(/^([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})$/);
                    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
                    if (!regex.test(key)) {
                        event.preventDefault();
                        return false;
                    }
                });
            }
        };
    }   
 
    WhiteListController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'svc', '$window', 'resourceId'];

    function WhiteListController($uibModalInstance, $rootScope, $scope, svc, $window, resourceId) {
        $scope.ipAddress = "";


        $scope.close = function () {
            $uibModalInstance.dismiss('close');
        };
        $scope.save = function () {
            
            svc.setIpAddress(resourceId, $scope.ipAddress).then(function (response) {

                $uibModalInstance.dismiss('close');
                $rootScope.$emit('reload');
            })
        }
    }
})();