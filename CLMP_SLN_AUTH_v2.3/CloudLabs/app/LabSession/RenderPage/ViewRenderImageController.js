
(function (angular) {
    'use strict';
    angular.module('app-labsession')
        .controller('ViewRenderImageController', ViewRenderImageController);
    ViewRenderImageController.$inject = ['$scope', '$uibModalInstance', 'source'];
    function ViewRenderImageController($scope, $uibModalInstance, source) {


            $scope.source = source;

        $scope.close = function () {
            $uibModalInstance.close();
        };


        }
})(window.angular);
