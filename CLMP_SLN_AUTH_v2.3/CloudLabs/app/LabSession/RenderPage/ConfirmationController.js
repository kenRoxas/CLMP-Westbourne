(function (angular) {
    'use strict';
    angular.module('app-labsession')
        .controller('ConfirmationController', ConfirmationController);

    ConfirmationController.$inject = ['$scope', '$uibModalInstance'];

    function ConfirmationController($scope, $uibModalInstance) {

        $scope.close = function () {
            $uibModalInstance.dismiss();
        };
    }
})(window.angular);
