(function () {

    "use strict";

    angular.module('app-labactivity').controller('confirmationModalController', confirmationModalController);

    confirmationModalController.$inject = ['$uibModalInstance', 'items', 'svc', '$window', '$route', '$uibModalStack', '$scope', '$uibModal', '$rootScope'];

    function confirmationModalController($uibModalInstance, items, svc, $window, $route, $uibModalStack, $scope, $uibModal, $rootScope) {

        var $confmodal = this;
        $scope.status = items.status();
        var labActs = items.labActivity();
        var id = items.labActID();
        var isReturn = false;
        $confmodal.title = items.title();
        $confmodal.message = items.message();
        if (items.centered === null)
            $confmodal.centered = true;

        $confmodal.confirm = function (x) {
            
            if ($scope.status === 'delete') {
                $rootScope.$emit("pageLoading", {});
                $uibModalStack.dismissAll();
                    svc.deleteLabActivities(id)
                        .then(function (response) {
                            isReturn = true;
                            if (isReturn)
                                $rootScope.$emit("loadPageLabActivity", {});
                        });
                
                
            }
            else if ($scope.status === 'create' || $scope.status === 'edit') {
                $rootScope.$emit("pageLoading", {});
                $uibModalStack.dismissAll();
                svc.postLabActivity(labActs).then(function (response) {
                    isReturn = true;
                    if(isReturn)
                        $rootScope.$emit("loadPageLabActivity", {});
                });
                

            }
            else if ($scope.status === 'size') {
                $uibModalInstance.close();
                $scope.index = 0;
                $(".modal-dialog.modal-md").removeAttr('style');
                $("#mce-modal-block").css({ "z-index": "65535" });
                $(".mce-container.mce-panel.mce-floatpanel.mce-window.mce-in").css({ "opacity": "1" });
                $(".mce-container.mce-panel.mce-floatpanel.mce-window.mce-in").css({ "z-index": "65536" });
                $("mce-widget.mce-tooltip.mce-tooltip-n").css({ "z-index": "131070" });                
            }
        };        

        $confmodal.cancel = function () {
           
            $("html").removeAttr('style');

            $uibModalInstance.close();
            $scope.index = 0;
        };
    }
})();