
(function () {

    "use strict";
    angular.module("app-labprofiles")
        .controller("LabHoursController", LabHoursController)
        .directive("preventTyping", function () {
            return {
                link: function (scope, element, attributes) {
                    element.on("keydown keyup", function (e) {
                        if (e.keyCode === 189 || e.keyCode === 187 || e.keyCode === 190 || e.keyCode === 109 || e.keyCode === 107 || e.keyCode === 69) {
                            e.preventDefault();
                        }
                    });
                }
            };
        })
        .directive("limitStd", function () {
            return {
                require: 'ngModel',
                link: function (scope, elem, attrs, ngModel) {
                    //attrs.$set("ngTrim", "false");
                    scope.$watch(attrs.ngModel, function (newValue) {
                        if (ngModel.$viewValue > 9999) {
                            ngModel.$setViewValue("9999");
                            ngModel.$render();
                        }
                    });
                }
            };
        })
        .directive("limitHrs", function () {
            return {
                require: 'ngModel',
                link: function (scope, elem, attrs, ngModel) {
                    //attrs.$set("ngTrim", "false");
                    scope.$watch(attrs.ngModel, function (newValue) {
                        if (ngModel.$viewValue > 9999999) {
                            ngModel.$setViewValue("9999999");
                            ngModel.$render();
                        }
                    });
                }
            };
        });   

    LabHoursController.$inject = ['$scope', '$uibModal', '$uibModalInstance', '$route', '$http', 'svc', '$filter', '$window', '$cookies', '$rootScope', '$timeout', '$document', 'data', '$log', 'region'];

    function LabHoursController($scope, $uibModal, $uibModalInstance, $route, $http, svc, $filter, $window, $cookies, $rootScope, $timeout, $document, data, $log, region) {

        $scope.groupLabDetails = [];

        $scope.maxSize = 10;
        $scope.currentPage = 1;
        $scope.itemsPerPage = 10;
        $scope.viewby = 10;
        $scope.coursePerHours = 0;
        $scope.courseChanged = [];
        //$scope.remainingCredits = 0;
        $scope.isSaveDisabled = false;
        $scope.isAWSConsole = false;
        $scope.diskMin = 0;
        var courseHoursTemp = 0;
        var courseRemaining = 0;
        var cloudlabsgroupid = data.cloudlabsgroupsid();

        $scope.vmSize = [];
        $scope.vmAWSSize = [];
        $scope.vmGCPSize = [];
        var data = [];
        $scope.minDiskSize = 0;
        $scope.diskSizes;

        var load = function () {
            svc.groupLabs(cloudlabsgroupid).then(function (response) {
                if (userIsSuperAdmin) {
                    angular.copy(response, $scope.groupLabDetails);
                    //angular.forEach(response, function (value, key) {
                    //    if (value.VETypeID == 10) {
                    //        svc.checkDiskSize(value.VirtualEnvironmentId).then(function (respGCPSize) {
                    //            $scope.groupLabDetails[key].diskSizeMin = respGCPSize;
                    //        })
                    //    }
                            
                    //})
                }
                else {
                    angular.forEach(response, function (value, key) {
                        if ((value.VETypeID <= 2 || value.VETypeID > 4))
                            $scope.groupLabDetails.push(value);
                        if (value.VETypeID == 10) {
                            $scope.sss = "5";
                            //svc.checkDiskSize(value.VirtualEnvironmentId).then(function (respGCPSize) {
                            //    $scope.groupLabDetails[key].diskSizeMin = respGCPSize;
                            //})
                        }
                    });
                }

                $scope.tempGroupLabDetails = response;  
                $scope.length = $scope.groupLabDetails.length;

                svc.getVMSize().then(function (response) {
                    angular.copy(response, $scope.vmSize);
                });

                svc.getAWSVMSize().then(function (response) {
                    angular.copy(response, $scope.vmAWSSize);
                });

                svc.getMachineType().then(function (response) {
                    angular.copy(response, $scope.vmGCPSize);
                });

                svc.getMachineType(region).then(function (response) {
                    angular.forEach(response, function (value, key) {
                        $scope.vmGCPSize.push(value.name);
                    });
//                    angular.copy(response, $scope.vmGCPSize);
                })
                //for (var i = 0; i < $scope.groupLabDetails.length; i++) {
                //    if ($scope.groupLabDetails[i].VETypeID == 5) {
                //        $scope.groupLabDetails[i].CourseHours = 1;
                //    }
                //}
            });
        };

        load();

        $scope.close = function () {
            $("html").removeAttr('style');
            $uibModalInstance.close();
        };

        svc.CheckTotalSubscriptionCredit(cloudlabsgroupid).then(function (response) {
            $scope.totalCredits = response.SubscriptionHourCredits;
            $scope.remainingCredits = response.SubscriptionRemainingHourCredits;
        });

        var invalidUsers = [];

        $scope.courseStudentChanged = function (TotalCourseHours, courseHours, numberStudent, x, i) {
            if (numberStudent === null)
                numberStudent = 0;
            if (courseHours === null)
                courseHours = 0;
            if (TotalCourseHours === null)
                TotalCourseHours = 0;
            $scope.isSaveDisabled = false;

            angular.forEach($scope.groupLabDetails, function (value) {
                var LabHoursConsumed = (value.TotalCourseHours - value.TotalRemainingCourseHours);
                //var LabHoursConsumed = (value.TotalCourseHours - (value.TotalRemainingCourseHours / 60));
                if (!$scope.isSaveDisabled) {//if (value.ProvisionCount > value.NumberOfUsers)
                    if (LabHoursConsumed > (value.CourseHours * value.NumberOfUsers))
                        $scope.isSaveDisabled = true;
                    if (value.MachineSize != null)
                        if (value.CourseHours == 0 || value.NumberOfUsers == 0)
                            $scope.isSaveDisabled = true;
                    if (value.CourseHours != 0 || value.NumberOfUsers != 0)
                        if (value.MachineSize == null)
                            $scope.isSaveDisabled = true;
                    if (value.VETypeID == 11 && value.NumberOfUsers != 0 && value.CourseHours != 0)
                        $scope.isSaveDisabled = true;
                }
            });

         

            //for (k = 0; k < $scope.groupLabDetails.length; k++) {
            //    if($scope.groupLabDetails[k].ProvisionCount > $scope.groupLabDetails[k].NumberOfUsers){
            //        $scope.isSaveDisabled = true;
            //    } { $scope.isSaveDisabled = false; }
            //}

            //for (i = 0; i < $scope.groupLabDetails.length; i++) {
            //    if (($scope.groupLabDetails[i].ProvisionCount > $scope.groupLabDetails[i].NumberOfUsers))
            //        $scope.isSaveDisabled = true
            //    else
            //        $scope.isSaveDisabled = false;
            //}

            x.TotalCourseHours = courseHours * numberStudent;

            if (TotalCourseHours < x.TotalCourseHours) {
                courseHoursTemp = TotalCourseHours - x.TotalCourseHours;
                $scope.remainingCredits += courseHoursTemp;

                x.TotalRemainingCourseHours -= courseHoursTemp;
                //x.TotalRemainingCourseHours -= courseHoursTemp * 60;
                //courseRemaining -= courseHoursTemp;
            }
            else {
                courseHoursTemp = x.TotalCourseHours - TotalCourseHours;
                $scope.remainingCredits -= courseHoursTemp;
                x.TotalRemainingCourseHours += courseHoursTemp;
                //x.TotalRemainingCourseHours += courseHoursTemp * 60;
                //courseRemaining += courseHoursTemp;
            }

            courseRemaining = courseRemaining;
            //courseRemaining = courseRemaining * 60;

            x.TotalRemainingCourseHours += courseRemaining;

            var index = invalidUsers.indexOf(i);
            if (x.ProvisionCount > x.NumberOfUsers) {
                if (index === -1)
                    invalidUsers.push(i);
            }
            else
                invalidUsers.splice(index, 1);

            //if ($scope.remainingCredits < 0 || invalidUsers.length > 0 || ((x.TotalRemainingCourseHours / 6) / x.CourseHours) < 0)
            //    $scope.isSaveDisabled = true;
            //else
            //    $scope.isSaveDisabled = false;
        };

        $scope.changeNumber = function (key, size) {
            $scope.groupLabDetails[key].diskSize = size;
            $scope.groupLabDetails[key].diskMin = size;
        };

        $scope.save = function () {

            angular.forEach($scope.groupLabDetails, function (value, key) {
                if (value.DiskSize == null)
                    $scope.groupLabDetails[key].DiskSize = $scope.groupLabDetails[key].DiskSizeMin;
            });


            var items = {
                title: "Lab Hours",
                message: "Are you sure you want to credit to the selected course?",
                type: "create",
                isGrant: false,
                groupLabDetails: $scope.groupLabDetails,
                remainingCredits: $scope.remainingCredits
            };
            //for (var i = 0; i < items.groupLabDetails.length; i++) {
            //    if (items.groupLabDetails[i].VETypeID == 5) {
            //        items.groupLabDetails[i].CourseHours = 1;
            //        items.groupLabDetails[i].TotalRemainingContainers = items.groupLabDetails[i].NumberOfUsers;
            //    }
            //}

            var modal = $uibModal.open({
                templateUrl: '/app/LabProfiles/Modal/GrantConfirmationModal.html',
                controller: "GrantConfirmationModalController",
                controllerAs: '$confmodal',
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    content: function () {
                        return "LAB HOURS";
                    },
                    creditMapping: function () {
                        return '';
                    },
                    items: function () {
                        return items;
                    },
                    VEType: function () {
                        return null;
                    },
                    VMEmptyData: function () {
                        return null;
                    }
                }
            });

        };

        $scope.consumedLab = function (course) {
            return course.TotalCourseHours - (course.TotalRemainingCourseHours) / course.CourseHours;
            //return course.TotalCourseHours - (course.TotalRemainingCourseHours / 60) / course.CourseHours;
        };
        $scope.isNumber = function (number) {
            return angular.isNumber(number) && !isNaN(number);
        };

        var modelJSON = [];
        $scope.selectedCourse = [];

        $scope.change = function (isValid, veId, val) {
            modelJSON = {
                veId: veId,
                isValid: isValid                
            };
            if (isValid && val != null) {
                if ($scope.selectedCourse.length != 0) {
                    if ($scope.selectedCourse.filter(x => x.veId === veId).length != 0) {
                        var index = $scope.selectedCourse.findIndex(obj => obj.veId == veId);
                        $scope.selectedCourse.splice(index, 1);
                    }
                }
            }
            else {
                if ($scope.selectedCourse.length != 0) {
                    if ($scope.selectedCourse.filter(x => x.veId === veId).length == 0)
                        $scope.selectedCourse.push(modelJSON);
                }
                else
                    $scope.selectedCourse.push(modelJSON);
            }
            if ($scope.selectedCourse.length == 0)
                $scope.isValid = false;
            else
                $scope.isValid = true;
            
        }
    }

})();