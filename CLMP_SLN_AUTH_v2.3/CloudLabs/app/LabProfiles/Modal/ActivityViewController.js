(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('ActivityViewController', ActivityViewController);

    ActivityViewController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'items', 'svc', '$window'];

    function ActivityViewController($uibModalInstance, $rootScope, $scope, items, svc, $window) {
        var $viewmodal = this;
        $viewmodal.title = items.Name;
        $scope.message = items.TasksHtml;


        $viewmodal.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }



})();