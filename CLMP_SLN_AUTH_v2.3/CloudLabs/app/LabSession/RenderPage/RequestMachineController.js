(function (angular) {
    'use strict';
    angular.module('app-labsession')
        .controller('RequestMachineController', RequestMachineController);

    RequestMachineController.$inject = ['$scope', '$uibModalInstance', 'svc', '$uibModal' ];

    function RequestMachineController($scope, $uibModalInstance, svc,  $uibModal) {
        $scope.loading = true;
        $scope.email = currentEmail;

        svc.courseAvailable(currentUserGroup).then(
            function (responseResult) {
                $scope.courses = responseResult;
            });

        $scope.close = function () {
            $uibModalInstance.dismiss();
        };

        $scope.send = function () {
            $scope.loading = false;
            svc.sendRequestCourse($scope.course, $scope.noteTag).then(
                function (response) {
                    $uibModal.open({
                        templateUrl: '/app/LabSession/RenderPage/ConfirmationView.html',
                        controller: "ConfirmationController",
                        windowClass: 'notification-modal',
                        size: 'xs',
                        backdrop: 'static',
                        keyboard: false
                    });
                    $uibModalInstance.dismiss();
                });
            //$uibModalInstance.dismiss();
        };
    }
})(window.angular);
