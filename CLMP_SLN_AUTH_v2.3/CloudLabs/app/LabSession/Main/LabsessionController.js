(function (angular) {
    'use strict';

    angular.module('app-labsession')
        .controller('LabSessionController', LabSessionController);

    LabSessionController.$inject = ['$scope', '$window', 'svc', '$timeout', '$uibModal', '$document', '$rootScope', '$route', '$filter', '$interval'];


    function LabSessionController($scope, $window, svc, $timeout, $uibModal, $document, $rootScope, $route, $filter, $interval) {

        $scope.empty = true;
        $scope.loading = true;
        $scope.labs = [];
        $scope.labscheck = [];
        $scope.labContent = [];
        $scope.showLabStart = [];
        $scope.labveprofileid = 0;
        $scope.timeleft = 0;
        $scope.calledStartVM = false;
        $scope.labIds = [];
        $scope.isAriaBusy = true;
        var windowHeight = $(window).height();
        var machineStatus = null;
        var contents = [];
        var htmlRef = angular.element($document[0].html);
        $scope.timeStamp = null;
        $scope.userId = parseInt(currentUserId);
        $scope.courseCode = "";
        $scope.courseName = "";
        $scope.isTrue = true;
        $scope.isFalse = false;
        $scope.isClicked = false;
        $scope.iteration = 0;
        var _currentDateTime = new Date();
        $scope.extension = [];
        var guac = "";
        $scope.allowPopover = false;
        $scope.popover = "none";
        $scope.LinuxUsername = "cloudswyft";
        $scope.LinuxPass = "Cloudswyft@123$$";

        $scope.userProvision = function (userid) {
            if (currentEmail.includes("temporaryemail")) {
                $scope.loading = false;
                $scope.loading2 = false;
                $scope.checkIfUserIsValid();

            }
            else {
                svc.getCourseVMs($scope.userId)
                    .then(function (response) {
                        angular.copy(response, $scope.labs);
                        angular.forEach(response, function (value, key) {
                            //svc.getUsersWithLabHourExtensions(value.veprofileid)
                            //    .then(function (resp) {
                            //        angular.forEach(resp, function (value, key) {
                            //            if ((new Date(value.StartDate).toISOString() <= new Date().toISOString()) && (new Date(value.EndDate).toISOString() > new Date().toISOString()) && value.ExtensionTypeId == 1) {
                            //                $scope.labs[key].isExtend = true;
                            //            }
                            //        })
                            //    });
                        })

                        if (response.length !== 0) {
                            for (var i = 0; i < $scope.labs.length; i++) {
                                if ($scope.labs[i].TimeRemaining !== null) {
                                    if ($scope.labs[i].TimeRemaining != undefined) {
                                        var d = Number($scope.labs[i].TimeRemaining);
                                        var h = Math.floor(d / 3600);
                                        var m = Math.floor(d % 3600 / 60);
                                        var s = Math.floor(d % 3600 % 60);

                                        $scope.labs[i].remainingMins = m;
                                        $scope.labs[i].remainingHours = h;

                                    }
                                }
                                
                            }
                            $scope.empty = false;
                        }
                        $scope.loading = false;
                    });
            }
        };    

        $scope.checkIfUserIsValid = function () {
            svc.getUserIfUserIsValid(currentUserIdLTI)
                .then(function (response) {
                    if (response === true) {
                        $uibModal.open({
                            templateUrl: '/app/LabSession/RenderPage/RegistrationView.html',
                            controller: "RegistrationController",
                            windowClass: 'reg-window',
                            size: 'xs',
                            backdrop: 'static',
                            keyboard: false
                        });
                    }
                });

        };

        $scope.showCharts = function (data) {
            $scope.CostBudgetLimit = parseFloat(data.Cost_budget_limit.Amount);
            $scope.ActualCostSpend = parseFloat(data.Actual_costs_spend.ActualSpend.Amount);
            $scope.TransferBudgetLimit = parseFloat(data.Data_transfer_budget_limit.Amount);
            $scope.ActualTransferSpend = parseFloat(data.Actual_data_transfer_spend.ActualSpend.Amount);

            function getBudgetLimit() {
                return ($scope.CostBudgetLimit < $scope.ActualCostSpend) ? 0 : ($scope.CostBudgetLimit - $scope.ActualCostSpend);
            }

            function getTransferLimit() {
                return ($scope.TransferBudgetLimit < $scope.ActualTransferSpend) ? 0 : ($scope.TransferBudgetLimit - $scope.ActualTransferSpend);
            }


            $scope.ctx1 = document.getElementById('myChart1').getContext('2d');
            $scope.myChart1 = new Chart($scope.ctx1, {
                type: 'doughnut',
                data: {
                    labels: ['Cost Limit', 'Actual Costing'],
                    datasets: [{
                        data: [getBudgetLimit(), $scope.ActualCostSpend],
                        backgroundColor: [
                            'rgba(129, 0, 32, .7)',
                            'rgba(37, 170, 225, .7)',
                        ],
                        borderColor: [
                            'rgba(129, 0, 32, 1)',
                            'rgba(37, 170, 225, 1)',
                        ],
                        borderWidth: 1
                    }]
                },
                responsive: true,
                options: {
                    tooltips: {
                        enabled: false
                    },
                    legend: {
                        display: false,
                        labels: {
                            fontSize: 10,
                            boxWidth: 10,
                            fontColor: 'rgba(255, 255, 255, 1)'
                        }
                    }
                }
            });

            $scope.ctx2 = document.getElementById('myChart2').getContext('2d');
            $scope.myChart2 = new Chart($scope.ctx2, {
                type: 'doughnut',
                data: {
                    labels: ['Transfer Limit', 'Transfer Spending'],
                    datasets: [{
                        data: [getTransferLimit(), $scope.ActualTransferSpend],
                        backgroundColor: [
                            'rgba(129, 0, 32, .7)',
                            'rgba(37, 170, 225, .7)',
                        ],
                        borderColor: [
                            'rgba(129, 0, 32, 1)',
                            'rgba(37, 170, 225, 1)',
                        ],
                        borderWidth: 1
                    }]
                },
                responsive: true,
                options: {
                    tooltips: {
                        enabled: false
                    },
                    legend: {
                        display: false,
                        labels: {
                            fontSize: 10,
                            boxWidth: 10,
                            fontColor: 'rgba(255, 255, 255, 1)'
                        }
                    }
                }
            });
        }

        $scope.userProvision($scope.userId);

        $scope.startVM = function (veProfileId) {
            svc.startVM($scope.userId, veProfileId)
                .then(function (response) {
                });

            $scope.calledStartVM = true;
        };

        $scope.loadModal = function (i, guacURL, resourceId) {
            //svc.timeTrigger($scope.userId, $scope.labveprofileid, $scope.timeStamp, "Render")
            //    .then(function (response) {
            //    });

            $("html").css({ "overflow": "hidden" });
            htmlRef.addClass('ovh');
            svc.getProfileActivities(i)
                .then(function (response) {
                    contents.push(response);
                    if (contents.length !== 0) {
                        $scope.loading2 = false;
                        $scope.loading = false;
                        $uibModal.open({
                            templateUrl: '/app/LabSession/RenderPage/RenderPageView.html',
                            controller: "RenderPageController",
                            size: 'md',
                            backdrop: 'static',
                            keyboard: false,
                            windowClass: 'app-modal-window',
                            resolve: {
                                item: function () {
                                    return guacURL;
                                },
                                content: function () {
                                    return contents;
                                },
                                userId: function () {
                                    return $scope.userId;
                                },
                                profileId: function () {
                                    return $scope.labveprofileid;
                                },
                                hoursRemaining: function () {
                                    return $scope.timeleft;
                                },
                                labIds: function () {
                                    return $scope.labIds;
                                },
                                coursecode: function () {
                                    return $scope.courseCode;
                                },
                                coursename: function () {
                                    return $scope.courseName;
                                },
                                resourceId: function () {
                                    return resourceId;
                                }
                            }
                        });
                    }
                });
        };

        //$scope.getProvision = function (userId, veProfileId, veTypeId = 0, guacamoleUrl = "", ConsoleLink = "", isProvision = 0) {
        //    if (veTypeId === 1 || veTypeId === 2 || veTypeId === 3 || veTypeId === 4) {
        //        svc.getProvisionedDetails(userId, veProfileId).then(function (response) {
        //            if (response[0]["IsStarted"] === 1) {
        //                $scope.ifStarted = true;
        //                if ($scope.calledStartVM) {
        //                    $timeout(function () {
        //                        $(".navbar").css({ "display": "none" });
        //                        $("#wrapper").css({ "display": "none" });
        //                        $("#sidebar").css({ "display": "none" });
        //                        $scope.loadModal(veProfileId);
        //                    }, 60000);
        //                }
        //                else {
        //                    $(".navbar").css({ "display": "none" });
        //                    $("#wrapper").css({ "display": "none" });
        //                    $("#sidebar").css({ "display": "none" });
        //                    $scope.loadModal(veProfileId);
        //                }
        //            }
        //            else if (response[0]["IsStarted"] === 6) {
        //                $scope.ifStarted = false;
        //                $scope.loading2 = true;
        //                $scope.getProvision($scope.userId, veProfileId, veTypeId);
        //            }
        //            else {
        //                if ($scope.calledStartVM === false) {
        //                    $scope.startVM(veProfileId);
        //                }
        //                $scope.ifStarted = false;
        //                $scope.loading2 = true;
        //                $scope.getProvision($scope.userId, veProfileId, veTypeId);
        //            }

        //        });
        //    }
        //    else if (veTypeId === 5 && guacamoleUrl !== "") {
        //        $scope.loading2 = false;
        //        window.open(guacamoleUrl);
        //    }
        //    else if ((veTypeId === 6 || veTypeId === 7) && guacamoleUrl !== "" && isProvision == 1) {
        //        //svc.consoleProvisions(veProfileId, veTypeId).then(function (response) {
        //        //    $scope.Response = response;
        //        //    alert(response);
        //        //})
        //        $scope.loading2 = false;
        //        window.open(ConsoleLink);
        //    }

        //};

        $scope.checkStatus = function (i) {
            if (i.IsProvisioned == 0) {
                $scope.clickCourse(i);
            } else if (i.IsProvisioned == 1 && i.Is_suspended == true) {
                return '';
            } else {
                $scope.openLab(i, null);
            }
        };

        //$scope.clickCourse = function (i) {
        //    if (i.VEType == 6) //AWS Console
        //    {
        //        $scope.loading2 = false;
        //        if (i.IsProvisioned == 0 && i.IsStarted == null) {
        //            $scope.userNotification();
        //            svc.provisionAWSConsole(i.veprofileid).then(function (response) {
        //            })
        //        }
        //    }
        //    if (i.VEType == 7) //Alibaba Console
        //    {
        //        $scope.loading2 = false;
        //        if (i.IsProvisioned == 0 && i.IsStarted == null) {
        //            $scope.userNotification();
        //            svc.provisionAlibabaConsole(i.veprofileid).then(function (response) {
        //            })
        //        }
        //    }
        //    if (i.VEType != 7 && i.VEType != 6) //Windows or Linux
        //    {
        //        var contents = {
        //            "UserId": [i.UserId],
        //            "VEProfileID": i.veprofileid,
        //            "CourseHours": null,
        //            "NumberOfUsers": null,
        //            "TotalCourseHours": null,
        //            "TotalRemainingCourseHours": null
        //        };

        //        $scope.loading2 = false;

        //        if (i.IsProvisioned == 0 && i.IsStarted == null) {
        //            $scope.userNotification();
        //            svc.provisionVM(contents).then(function (response) {

        //            });
        //        }
        //    }
        //};

        //$scope.clickAlibabaConsole = function (i) {
        //    $scope.loading2 = false;
        //    if (i.IsProvisioned == 0 && i.IsStarted == null) {
        //        $scope.userNotification();
        //        svc.provisionAlibabaConsole(i.veprofileid).then(function (response) {
        //        })
        //    }
        //};

        $scope.openLab = function openLab(i, item) {
            $scope.isClicked = true;
            if (i.IsStarted != 1)
                item.currentTarget.setAttribute("disabled", "true")

            if (!i.IsProvisioning) {
                angular.forEach($scope.labs, function (value, key) {
                    if (i.UserId == value.UserId && i.veprofileid == value.veprofileid)
                        value.IsProvisioning = true;
                })
                var s = $scope.iteration;
                $scope.labContent.length = 0;
                //$scope.ifStarted = true;
                $scope.calledStartVM = false;
                $scope.labveprofileid = i.veprofileid;
                $scope.timeleft = i.TimeRemaining;
                $scope.courseCode = i.CourseCode;
                $scope.courseName = i.Name;
                $scope.timeStamp = new Date();
                $scope.timeStamp = $filter('date')($scope.timeStamp, "M/d/yy h:mm a", "UTC");


                if (i.VEType === 5 || i.VEType === 6) {
                    $scope.loading2 = false;
                }

                else {
                    if (i.IsStarted == 0) {      
                       // $scope.userNotification("Please wait.. Machine is about to start...", null, "Starting", null, null, null);
                        $scope.VMCourse($scope.userId, $scope.labveprofileid, i.VEType, i.GuacamoleUrl, i.ConsoleLink, i.IsProvisioned, i.IsStarted, i.ResourceId, i.MachineStatus, i.RunningBy);
                        //$scope.loading2 = true;
                        svc.vmOperation(i.ResourceId, "Start", "Student").then(function (response) {
                        });

                        // api to start vm
                    }
                    else if (i.IsStarted == 1 && i.MachineStatus == 'Starting') {
                        $scope.loading2 = true;
                        $scope.VMCourse($scope.userId, $scope.labveprofileid, i.VEType, i.GuacamoleUrl, i.ConsoleLink, i.IsProvisioned, i.IsStarted, i.ResourceId, i.MachineStatus, i.RunningBy);

                        // api to open machine
                    }
                    else if (i.IsStarted == 1 && i.MachineStatus == 'Running') {
                        $scope.VMCourse($scope.userId, $scope.labveprofileid, i.VEType, i.GuacamoleUrl, i.ConsoleLink, i.IsProvisioned, i.IsStarted, i.ResourceId, i.MachineStatus, i.RunningBy);
                        // waiting to shutdown
                    }
                    else if (i.IsStarted == 3) {
                        //var contents = {
                        //    "UserId": [i.UserId],
                        //    "VEProfileID": i.veprofileid,
                        //    "CourseHours": null,
                        //    "NumberOfUsers": null,
                        //    "TotalCourseHours": null,
                        //    "TotalRemainingCourseHours": null,
                        //    "MachineSize": i.MachineSize
                        //};
                        //svc.provisionVM(contents, true).then(function (response) {
                        //    $scope.userNotification("Please wait.. Provision is on going...", null, "Provisioning", null, null, null);
                        //});                    
                        svc.reProvisionVM($scope.userId, $scope.labveprofileid).then(function (response) {
                            $scope.userNotification("Please wait.. Provision is on going...", null, "Provisioning", null, null, null);
                        });
                    }
                    //else if (i.IsStarted == 4) {
                    //    // Provisioning   
                    //}
                    else if (i.IsStarted == null) {
                        var contents = {
                            "UserId": [i.UserId],
                            "VEProfileID": i.veprofileid,
                            "CourseHours": null,
                            "NumberOfUsers": null,
                            "TotalCourseHours": null,
                            "TotalRemainingCourseHours": null,
                            "MachineSize": i.MachineSize
                        };

                        
                        svc.provisionVM(contents, true).then(function (response) {
                            $scope.userNotification("Please wait.. Provision is on going...", null, "Provisioning", null, null, null);
                        });
                    }
                }
            }          
        };

        $scope.VMCourse = function (userId, veProfileId, veTypeId = 0, guacamoleUrl = "", ConsoleLink = "", isProvision = 0, isIstarted = 0, resourceId, machineStatus = "", runningBy = 0) {

            if ((veTypeId === 1 || veTypeId === 2 || veTypeId === 3 || veTypeId === 4 || veTypeId === 8 || veTypeId === 9 || veTypeId === 10) && isIstarted == 0) {
                svc.getCourseVMs($scope.userId)
                    .then(function (response) {
                        angular.copy(response, $scope.labs);
                        for (var i = 0; i < $scope.labs.length; i++) {
                            if ($scope.labs[i].TimeRemaining !== null) {
                                if ($scope.labs[i].TimeRemaining != undefined) {  
                                    var d = Number($scope.labs[i].TimeRemaining);
                                    var h = Math.floor(d / 3600);
                                    var m = Math.floor(d % 3600 / 60);
                                    var s = Math.floor(d % 3600 % 60);

                                    $scope.labs[i].remainingMins = m;
                                    $scope.labs[i].remainingHours = h;
                                }
                            }
                        }
                        angular.forEach(response, function (value) {
                            if (value.veprofileid == veProfileId) {
                                if (value.IsStarted === 1) {
                                    $timeout(function () {
                                        svc.setRunningBy(resourceId, 1).then(function (response) { });
                                        svc.machineLogs(resourceId, "", "Launch VM").then(function (response) { });
                                        if (veTypeId == 9 && guacamoleUrl == null) {
                                            svc.getGuacDNS(resourceId).then(function (response) {
                                                guac = response;
                                               // $scope.loadModal(veProfileId, guac, resourceId);
                                            });
                                        }
                                        //else
                                        //    $scope.loadModal(veProfileId, guacamoleUrl, resourceId);

                                    }, 50000);
                                }
                                else {
                                    $scope.VMCourse(userId, veProfileId, veTypeId, guacamoleUrl, ConsoleLink, isProvision, value.IsStarted, resourceId, machineStatus, runningBy);
                                }
                            }
                        })
                    });
            }
            else if ((veTypeId === 1 || veTypeId === 2 || veTypeId === 3 || veTypeId === 4 || veTypeId === 8 || veTypeId === 9 || veTypeId === 10) && (isIstarted == 1 && machineStatus == 'Running')) {
                svc.setRunningBy(resourceId, 1).then(function (response) { });

                svc.machineLogs(resourceId, "", "Launch VM").then(function (response) { });
                $scope.loadModal(veProfileId, guacamoleUrl, resourceId);
            }
            else if ((veTypeId === 1 || veTypeId === 2 || veTypeId === 3 || veTypeId === 4 || veTypeId === 8 || veTypeId === 9 || veTypeId === 10) && (isIstarted == 1 && machineStatus == 'Starting')) {
                $scope.loading2 = true;
                $scope.VMCourse(userId, veProfileId, veTypeId, guacamoleUrl, ConsoleLink, isProvision, 3, resourceId, machineStatus, runningBy);

            }
            else if (veTypeId === 5) {
                $scope.loading2 = false;
                window.open(guacamoleUrl);
            }
            else if ((veTypeId === 6 || veTypeId === 7)) {
                $scope.loading2 = false;
                window.open(ConsoleLink);
            }
        };

        $scope.userNotification = function (message, imageInfo, type, userId, profileId, resourceId) {
            $("html").css({ "overflow": "hidden" });
            htmlRef.addClass('ovh');
            $uibModal.open({
                templateUrl: '/app/LabSession/RenderPage/UserNotificationView.html',
                controller: "UserNotificationController",
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                windowClass: 'notification-modal',
                resolve: {
                    message: function () {
                        return message;
                    },
                    imageInfo: function () {
                        return imageInfo;
                    },
                    type: function () {
                        return type;
                    },
                    UserId: function () {
                        return userId;
                    },
                    ProfileId: function () {
                        return profileId;
                    },
                    resourceId: function () {
                        return resourceId;
                    }
                }
            });
        };

        $scope.checkIfShuttingDown = function () {

            svc.getCourseVMs($scope.userId)
                .then(function (response) {             
                    angular.copy(response, $scope.labs);
                    if (response.length !== 0) {
                        for (var i = 0; i < $scope.labs.length; i++) {
                            if ($scope.labs[i].TimeRemaining !== null) {
                                if ($scope.labs[i].TimeRemaining != undefined) {
                                    //angular.forEach(response, function (value, key) {
                                    //    svc.getUsersWithLabHourExtensions(value.veprofileid)
                                    //        .then(function (resp) {
                                    //            angular.forEach(resp, function (value, key) {
                                    //                if ((new Date(value.StartDate).toISOString() <= new Date().toISOString()) && (new Date(value.EndDate).toISOString() > new Date().toISOString()) && value.ExtensionTypeId == 1) {

                                    //                    $scope.labs[key].isExtend = true;
                                    //                }
                                    //            })
                                    //        });
                                    //})

                                    var d = Number($scope.labs[i].TimeRemaining);
                                    var h = Math.floor(d / 3600);
                                    var m = Math.floor(d % 3600 / 60);
                                    var s = Math.floor(d % 3600 % 60);

                                    $scope.labs[i].remainingMins = m;
                                    $scope.labs[i].remainingHours = h;
                                }
                            }
                        }
                        $scope.empty = false;
                    }
                });
        };
        ////$scope.checkIfShuttingDown();
        $interval($scope.checkIfShuttingDown, 30000);

        $scope.showStartLab = function showStartLab(i, hours) {
            if (hours > 0)
                $scope.showLabStart[i] = true;
        };

        $scope.hideStartLab = function hideStartLab(i) {
            $scope.showLabStart[i] = false;
        };

        $rootScope.$on('reload', function () {
            $route.reload();
        });

        $rootScope.$on('reloadLab', function () {
            svc.getCourseVMs($scope.userId)
                .then(function (response) {
                    angular.copy(response, $scope.labs);
                    angular.forEach(response, function (value, key) {

                    })

                    if (response.length !== 0) {
                        for (var i = 0; i < $scope.labs.length; i++) {
                            if ($scope.labs[i].TimeRemaining !== null) {
                                if ($scope.labs[i].TimeRemaining != undefined) {
                                    var d = Number($scope.labs[i].TimeRemaining);
                                    var h = Math.floor(d / 3600);
                                    var m = Math.floor(d % 3600 / 60);
                                    var s = Math.floor(d % 3600 % 60);

                                    $scope.labs[i].remainingMins = m;
                                    $scope.labs[i].remainingHours = h;

                                }
                            }

                        }
                        $scope.empty = false;
                    }
                    $scope.loading = false;
                });        });

        $scope.veContainer = function (a) {
            if (a == 0)
                return 'veprofileContainerProvisioned';
            else if (a == 1)
                return 'veprofileConsoleContainer';
            else if (a == 4)
                return 'loadingProvisioning';
            else if (a == 3)
                return 'failedProvisioning';
            else
                return '';
        }

        $scope.blurImage = function (b) {
            if (b == 3)
                return 'failedLabContainer';
            else
                return 'labContainer';
        }

        $scope.dynamicPopover = {
            templateUrl1: 'graphTemplate.html',
            templateUrl2: 'courseTemplate.html',
            templateUrl3: 'courseVMTemplate.html'
        };

        $scope.setConsoleCourseTheme = function (x, y) {
            if (x == 0 && y == false) {
                return 'card-provision';
            } else if (x == 1 && y == false) {
                return 'card-start';
                //return 'card-starting';
            } else if (x == 1 && y == true) {
                return 'card-suspend';
            } else if (x == 2 && y == false) {
                return 'card-provisioning';
            } else if (x == 3 && y == false) {
                return 'card-failed';
            } else {
                return '';
            }
        }

        $scope.setCourseTheme = function (x, y, item) {
            //if (x <= 0 && y != null) {
            //    return 'card-ended';
            //} else
            if (y == 2) {
                return 'card-shutdown';
            } else if (y == 3) {
                return 'card-failed';
            } else if (y == 4) {
                return 'card-provisioning';
            } else if (y == null) {
                return 'card-provision';
            } else if (y == 5) {
                return 'card-starting';
            } else if (y == 6) {
                return 'card-deleting';
            } else if (y == 7) {
                return 'card-deleted';
            } else {
                return 'card-start';
            }
        }

        $scope.applyOpacity = function (x, y, z, a, c) {

            if (c == true && x <= 0)
                return;
            if ((x <= 0 && x != null && y != null) || (y == 2) || (z == 1 && a == true)) {
                return 'applyOpacity';
            }
        };

        $scope.setPopoverColor = function (x, y) {
            if (x == 1 && y == true) {
                return 'popover-gray';
            }
        }

        $scope.requestLab = function () {
            $uibModal.open({
                templateUrl: '/app/LabSession/RenderPage/RequestMachineView.html',
                controller: "RequestMachineController",
                windowClass: 'req-window',
                size: 'xs',
                backdrop: 'static',
                keyboard: false
            });
        };

        $scope.startRDP = function (user) {
            if (user.IsStarted == 0) {
                $scope.userNotification("Please wait.. Machine is about to start...", null, "Starting", null, null, user.ResourceId);
            }
        };

        $scope.openRDP = function (user) {
            $uibModal.open({
                templateUrl: '/app/LabSession/RenderPage/WhiteListView.html',
                controller: "WhiteListController",
                windowClass: 'reg-window',
                size: 'xs',
                backdrop: 'static',
                keyboard: false,
                resolve: {
                    resourceId: function () {
                        return user.ResourceId;
                    }
                }
            });
        };

        $scope.checkIfIpExist = function (user, item) {
            if (user.IsStarted != 1)
                item.currentTarget.setAttribute("disabled", "true")

            if ((user.IpAddress == null || user.IpAddress == undefined) && user.VEType == 9) {
                $scope.openRDP(user);
                $scope.popover = "none";

            } else {
                $scope.popover = "courseVMTemplate.html";
                }
        };

        $scope.shutdown = function (user, item) {
            if (user.IsStarted != 1)
                item.currentTarget.setAttribute("disabled", "true")

            $scope.isClicked = true;
            svc.machineLogs(user.ResourceId, "", "StopUIRDP").then(function (response) {
                svc.vmOperation(user.ResourceId, "Stop", "Student").then(function (response) { });
                $scope.userNotification("Please wait.. Machine is shutting down...", null, "Starting", null, null, user.ResourceId);

            });
        };
    }

})(window.angular);