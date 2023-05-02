(function () {

    "use strict";

    angular.module('app-labprofiles')
        .controller('GrantConfirmationModalController', GrantConfirmationModalController);

    GrantConfirmationModalController.$inject = ['$uibModalInstance', '$rootScope', '$scope', 'creditMapping', 'content', 'items', 'svc', '$window', '$route', '$uibModal', 'VEType', 'VMEmptyData','$uibModalStack'];

    function GrantConfirmationModalController($uibModalInstance, $rootScope, $scope, creditMapping, content, items, svc, $window, $route, $uibModal, VEType, VMEmptyData, $uibModalStack) {
        var $confmodal = this;
        $confmodal.title = items.title;
        $confmodal.isGrant = items.isGrant;
        $confmodal.message = items.message;
        $scope.isBulkCreate = items.isBulkCreate;
        $confmodal.contents = content;
        $confmodal.VEType = VEType;
        var groupLabDetails = items.groupLabDetails;
        var remainingCredits = items.remainingCredits;
        $scope.userIsSuperAdmin = userIsSuperAdmin;
        $scope.showError = true;
        $scope.labhourspercourse = creditMapping.CourseHours;
        //var contents = {
        //    "labCreditMapping": creditMapping,
        //    "CLUsers": content
        //};
        var grantContents = {
            "ConsoleUsers": content
        }
        var contents = {
            "UserId": content,
            "VEProfileID": creditMapping.VEProfileID,
            "CourseHours": creditMapping.CourseHours,
            "NumberOfUsers": creditMapping.NumberOfUsers,
            "TotalCourseHours": creditMapping.TotalCourseHours,
            "TotalRemainingCourseHours": creditMapping.TotalRemainingCourseHours,
            "MachineSize": creditMapping.MachineSize,
            "VMEmptyData": VMEmptyData
        };
       

        $confmodal.confirm = function () {
            $uibModalInstance.close(true);
            $uibModalStack.dismissAll();
            $rootScope.$emit('fauxLoad');

            if ($confmodal.isGrant) {
                svc.provisionVM(contents, false).then(function (response) {
                    $rootScope.$emit('loadContent');
                });                //svc.createCloudLabsSchedule(contents, false, currentEmail)
                //    .then(function (response) {
                //        $rootScope.$emit('loadContent');
                //    })
                //    .finally(function () {

                //    });
            }
            else {
                svc.saveGroupLabs(groupLabDetails, remainingCredits)
                    .then(function (response) {
                        $route.reload();

                        //$rootScope.$emit('loadContent');
                    });
            }

            $("html").removeAttr('style');
            //$rootScope.$emit('loadContent');
        };

        $confmodal.confirmConsole = function () {
            $uibModalInstance.close(true);
            $uibModalStack.dismissAll();
            $rootScope.$emit('fauxLoad');

            //if (VEType == 6 || VEType == 7) {
                svc.saveGrantUser(grantContents)
                    .then(function (response) {
                        $rootScope.$emit('loadContent');
                    })
                    .finally(function () {

                    });
            //}
            //else {
                //svc.saveVMUser(consoleContents)
                //    .then(function (response) {
                //        $rootScope.$emit('loadContent');
                //    })
                //    .finally(function () {

                //    });
           // }

            $("html").removeAttr('style');

        };

        $confmodal.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
        $confmodal.upload = function () {
            svc.uploadBulkProvision($scope.bulkFile, creditMapping.VEProfileID).then(function (response) {
            });
            var modalData = {
                message: "Provisioning. . .",
                title: "Bulk Provision",
                VEProfileID: creditMapping.VEProfileID,
                GroupID: null
            };

            var modal = $uibModal.open({
                templateUrl: '/app/Common/ConfirmationModal.html',
                controller: "ConfirmationModalController",
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return modalData;
                    },
                    userGroup: function () {
                        return null;
                    },
                    VEProfileID: function () {
                        return creditMapping.VEProfileID;
                    },
                    userGrade: function () {
                        return null;
                    },
                    VEName: function () {
                        return null;
                    },
                    userid: function () {
                        return null;
                    },
                    resourceId: function () {
                        return null;
                    },
                    labhourdata: function () {
                        return null;
                    }

                }
            });

        };
        
        $scope.uploadFiles = function (file, errFiles) {

            $scope.showError = true;
            if (file !== null) {
                $scope.f = file;
                if (file.type === "text/csv" || file.type === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                    $scope.bulkFile = file;
                    $scope.showError = false;
                }
                else {
                    $scope.showError = true;
                    $scope.errorMsg = "Please select .csv or .xlsx file to upload";
                }
            }
        };
       


    }

})();