(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('ImageGradeController', ImageGradeController);

    ImageGradeController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'thumbnail', 'svc', '$window', 'name', '$filter', '$uibModal', 'VEDescription', 'veprofileid', 'Id'];

    function ImageGradeController($uibModalInstance, $rootScope, $scope, thumbnail, svc, $window, name, $filter, $uibModal, VEDescription, veprofileid, Id) {
        $scope.name = name;
        $scope.thumbnail = [];

        svc.getUserImage(Id, groupCode, VEDescription, veprofileid).then(function (response) {
            angular.copy(response, $scope.thumbnail);
        })
       

        $scope.imageName = function (image) {
            var name = image.substring(image.indexOf("_") + 1);

            //var date = $filter('date')(new Date(parseInt(image.substring(image.lastIndexOf("_") + 1, image.length))), 'MMMM dd, yyyy HH:mm:ss');
            return name;
            //return new Date(parseInt(image.substring(image.lastIndexOf("-") + 1, image.length)));
        };

        $scope.imageView = function (thumbnail) {
                $uibModal.open({
                    animation: true,
                    templateUrl: '/app/LabProfiles/Modal/ViewImage.html',
                    controller: 'ViewImageController',
                    windowClass: 'create-modal-design',
                    size: 'l',
                    //backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        thumbnail: function () {
                            return thumbnail;
                        }
                    }
                });
        };

        $scope.close = function () {
            $uibModalInstance.dismiss('close');
        };
    }
})();