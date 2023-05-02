(function () {

    "use strict";

    angular.module('app-labactivity')
        .controller('uploadModalController', uploadModalController);
    uploadModalController.$inject = ['$uibModalInstance', 'svc', '$uibModalStack', '$scope', '$rootScope', 'courseCode', 'labactivityid'];

    function uploadModalController($uibModalInstance, svc, $uibModalStack, $scope, $rootScope, courseCode, labactivityid) {

        $scope.status = 'upload';
        var isReturn = false;
        $scope.title = 'Upload';
        $scope.message = 'are you sure this is the right image?';
        $scope.imgPreview = true;
        $scope.hasPdf = true;
        $scope.showUpload = true;
        $scope.showError = false;
        $scope.alwaysFalse = false;
        $scope.showMsg = true;
        $scope.showConfirm = false;
        var imageInfo = {};
        $scope.confirm = function () {
            if ($scope.status === 'upload') {
                
                $uibModalStack.dismissAll();
                
                svc.UploadLabAnswerKey(imageInfo, imageInfo.pdfFilename, labactivityid).then(function (response) {
                    $rootScope.$emit("pageLoading", {});
                    $rootScope.$emit("loadPageLabActivity", {});
                });
            }
        };

        $scope.cancel = function () {

            $("html").removeAttr('style');
            $scope.picFile = null;
            $scope.hasPdf = null;
            $(".modal-sm").removeClass('transition-upload');
            $scope.index = 0;
            $uibModalInstance.close();
        };

        $scope.retry = function () {
            $scope.hasPdf = true;
            $scope.showError = false;
            $scope.picFile = null;
            $scope.showBtn = false;
            $scope.showUpload = true;
            $scope.showError = false;
            $scope.showMsg = false;
            $scope.showConfirm = false;
            $scope.f = null;
        };

        $scope.uploadFile = function (file, errFiles) {
            if (file !== null) {
                $scope.imgPreview = true;
                $scope.showBtn = true;
                $scope.showUpload = false;
                $scope.showRemove = false;
                $scope.showError = false;
                $scope.showMsg = true;
                $scope.showConfirm = true;
                $scope.hasPdf = true;
                $scope.f = file;
                $scope.errFile = errFiles && errFiles[0];
                if (file.type === "application/pdf") {

                    imageInfo = {
                        type: "pdf",
                        file: file,
                        pdfFilename: courseCode + "_LabAnswerKey.pdf"
                    };
                    
                    //var imageFilename = userId + '-' + profileId + '-' + Date.now();
                    $scope.showRemove = true;
                    $scope.showUpload = false;
                }
                else {
                    $scope.hasPdf = false;
                    $scope.showRemove = false;
                    $scope.showError = true;
                    $scope.picFile = null;
                    $scope.showUpload = false;
                    $scope.errorMsg = "Please select .pdf file to upload";
                }
            }
            else {
                $scope.hasPdf = true;
                $scope.errorMsg = "Please select .pdf file to upload";
                $scope.showError = true;
                $scope.showBtn = true;
                $scope.showMsg = false;
                $scope.picFile = null;
                $scope.showConfirm = false;
            }

        };
    }
})();