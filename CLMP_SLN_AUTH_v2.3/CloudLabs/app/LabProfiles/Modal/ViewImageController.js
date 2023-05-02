(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('ViewImageController', ViewImageController);

    ViewImageController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'thumbnail', 'svc', '$window', '$filter'];

    function ViewImageController($uibModalInstance, $rootScope, $scope, thumbnail, svc, $window, $filter) {
        $scope.image = thumbnail;
        $scope.close = function () {
            $uibModalInstance.dismiss('close');
        };
    }
})();