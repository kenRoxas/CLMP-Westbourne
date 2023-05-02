
(function () {

    "use strict";

    angular.module("app-dashboard")
        .controller("DashboardController", DashboardController)
        .filter('filtername', function () {
            return function (userSchedulesperCourse, searchUser) {
                var filtered = [];
                var letterMatch = new RegExp(searchUser, 'i');
                for (var i = 0; i < userSchedulesperCourse.length; i++) {
                    var item = userSchedulesperCourse[i];
                    if (letterMatch.test(item.FullNameEmail)) {
                        filtered.push(item);
                    }
                }
                return filtered;
            };
        });

    DashboardController.$inject = ['svc', '$scope', '$interval', '$timeout', '$filter', '$q'];

    function DashboardController(svc, $scope, $interval, $timeout, $filter, $q) {
        $scope.showSpecificStudent = false;
        $scope.courses = [];
        $scope.userGroup = [];
        $scope.currentUserDetails = [];
        $scope.userSchedulesperCourse = [];
        $scope.isLoading = false;
        $scope.loadingContent = false;
        $scope.loadingVEProfile = true;
        $scope.activeRow = true;
        $scope.groupLabDetails = [];
        $scope.isUserSuperAdmin = userIsSuperAdmin;
        $scope.isUserAdmin = userIsAdmin;
        $scope.isUserInstructor = userIsInstructor;
        $scope.isUserStaff = userIsStaff;
        $scope.continueIncrease = true;
        $scope.continueDecrease = true;
        $scope.TotalLabHoursPerCourse = 0;
        $scope.RemainingLabHourCredits = 0;
        $scope.RemainingLabMinCredits = 0;
        $scope.LabHoursPerUser = 0;
        $scope.progress_firstcircle = 0;
        $scope.progress_secondcircle = 0;
        $scope.progress_thirdcircle = 0;
        $scope.progress_fourthcircle = 0;
        $scope.VEType = 0;
        $scope.CurrentUserGroup = groupName;
        $scope.hasContainer = false;
        $scope.totalContainers = 0;
        $scope.isVETypeSeven = false;
        $scope.isStudentSpecific = false;
        $scope.openChart = false;
        $scope.downloadingCSV = false;
        $scope.isTrue = true;
        $scope.consoleLabs = [];
        $scope.DataAWSUsers = [];
        $scope.newLimitType = '';
        $scope.newLimitValue = null;
        $scope.min = 1;
        $scope.max = 1000;
        $scope.limitValue = null;
        $scope.minLength = 1;
        $scope.maxLength = 3;
        $scope.csv = [];
        var data = [];
        $scope.isTrue = true;
        var currentUserGroupId = currentUserGroup;
        $scope.hours = 'hours';
        $scope.minutes = 'minutes';
        //****************************************
        $scope.fromOptions = {
            // dateDisabled: false,
            customClass: getDayClass,
            formatYear: 'yy',
            maxDate: new Date(),
            minDate: new Date(2020, 5, 22),
            showWeeks: true
            //startingDay: 1
        };
        $scope.toOptions = {
            customClass: getDayClass,
            //dateDisabled: disabled,
            formatYear: 'yy',
            // startingDay: 1
        };

        $scope.fromOpen = function () {
            $scope.popup1.opened = true;
        };

        $scope.toOpen = function () {
            $scope.toOptions.minDate = $scope.dt;
            $scope.toOptions.maxDate = new Date();
            $scope.popup2.opened = true;
        };

        $scope.popup1 = {
            opened: false
        };

        $scope.popup2 = {
            opened: false
        };

        $scope.events = [
            {
                date: new Date(),
                status: 'full'
            }
        ];
        function getDayClass(data) {
            var date = data.date,
                mode = data.mode;
            if (mode === 'day') {
                var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

                for (var i = 0; i < $scope.events.length; i++) {
                    var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                    if (dayToCheck === currentDay) {
                        return $scope.events[i].status;
                    }
                }
            }

            return '';
        }

        //******************************
        $scope.saveData = function () {
            $scope.data1 = $scope.data;
            $scope.setConfirmUnload(false);
        };
        $scope.DataUsers = [];

        $scope.labMinutes = 0;
        $scope.labHours = 0;

        $scope.isCourseActivated = true;
        $scope.hidetitle = true;


        $scope.setDefault = function () { // for returning the circle in 0 state
            $scope.continueIncrease = false;
            $scope.continueDecrease = true;
            $interval.cancel($scope._sleepCheckRemainingLabHourCreditsPerMinutes);
            var subtractorFirstCircle = 0;
            var subtractorSecondCircle = 0;
            var subtractorThirdCircle = 0;


            var _sleepFirstCircleDecrease = $interval(function () {   // First Circle Animation (regression) 
                $scope._animateFirstCircleDecrease();
            }, 1);

            $scope._animateFirstCircleDecrease = function () {      // First Circle Animation (regression) 
                subtractorFirstCircle = subtractorFirstCircle + ($scope.TotalLabHoursPerCourse / 176);

                if ($scope.progress_firstcircle <= 0) {
                    $interval.cancel(_sleepFirstCircleDecrease);
                    $scope.TotalLabHoursPerCourse = 0;
                    _sleepFirstCircleDecrease = undefined;
                } else {
                    if (Math.floor(subtractorFirstCircle) >= 1) {
                        $scope.TotalLabHoursPerCourse -= Math.floor(subtractorFirstCircle);
                        subtractorFirstCircle = 0;
                    } else {
                        //subtractorFirstCircle += subtractorFirstCircle;
                    }
                    $scope.progress_firstcircle -= 4;
                }
                if (!$scope.continueDecrease)
                    $interval.cancel(_sleepFirstCircleDecrease);
            };

            var _sleepSecondCircleDecrease = $interval(function () {    // Second Circle Animation (regression) 
                $scope._animateSecondCircleDecrease();
            }, 1);

            $scope._animateSecondCircleDecrease = function () {    // Second Circle Animation (regression) 
                subtractorSecondCircle = subtractorSecondCircle + ($scope.RemainingLabHourCredits / 176);

                if ($scope.progress_secondcircle <= 0) {
                    $interval.cancel(_sleepSecondCircleDecrease);
                    _sleepSecondCircleDecrease = undefined;
                    $scope.RemainingLabHourCredits = 0;
                } else {
                    if (Math.floor(subtractorSecondCircle) >= 1) {
                        $scope.RemainingLabHourCredits -= Math.floor(subtractorSecondCircle);
                        subtractorSecondCircle = 0;
                    } else {
                        //subtractorSecondCircle += subtractorSecondCircle;
                    }
                    $scope.progress_secondcircle -= 4;
                }
                if (!$scope.continueDecrease)
                    $interval.cancel(_sleepSecondCircleDecrease);
            };

            var _sleepThirdCircleDecrease = $interval(function () {    // Second Circle Animation (regression) 
                $scope._animateThirdCircleDecrease();
            }, 1);

            $scope._animateThirdCircleDecrease = function () {    // Second Circle Animation (regression) 
                subtractorThirdCircle = subtractorThirdCircle + ($scope.LabHoursPerUser / 176);

                if ($scope.progress_thirdcircle <= 0) {
                    $interval.cancel(_sleepThirdCircleDecrease);
                    _sleepThirdCircleDecrease = undefined;
                    $scope.LabHoursPerUser = 0;
                } else {
                    if (Math.floor(subtractorThirdCircle) >= 1) {
                        $scope.LabHoursPerUser -= Math.floor(subtractorThirdCircle);
                        subtractorThirdCircle = 0;
                    } else {
                        //subtractorThirdCircle += subtractorThirdCircle;
                    }
                    $scope.progress_thirdcircle -= 4;
                }
                if (!$scope.continueDecrease)
                    $interval.cancel(_sleepThirdCircleDecrease);
            };
        };



        $scope.openStudentDetails = function (consoleLab) {

            if ($scope.selectedUser !== consoleLab) {
                $scope.selectedUser = consoleLab;
            }

            $scope.accountName = consoleLab.AccountName;
            $scope.accountID = consoleLab.AccountID;
            $scope.accountEmail = consoleLab.AccountEmail;
            $scope.email = consoleLab.Email;
            $scope.accountPassword = consoleLab.AccountPassword;
            $scope.openChart = true;
            $scope.loadingVEProfile = true;
            $scope.costSpend = parseFloat(consoleLab.Actual_costs_spend.ActualSpend.Amount);
            $scope.costSpendUnit = consoleLab.Actual_costs_spend.ActualSpend.Unit;
            $scope.budgetLimit = parseFloat(consoleLab.Cost_budget_limit.Amount);
            $scope.budgetLimitUnit = consoleLab.Cost_budget_limit.Unit;
            $scope.transferSpend = parseFloat(consoleLab.Actual_data_transfer_spend.ActualSpend.Amount);
            $scope.transferSpendUnit = consoleLab.Actual_data_transfer_spend.ActualSpend.Unit;
            $scope.dataTransferLimit = parseFloat(consoleLab.Data_transfer_budget_limit.Amount);
            $scope.dataTransferLimitUnit = consoleLab.Data_transfer_budget_limit.Unit;
            $scope.isSuspended = consoleLab.Is_suspended;

            function getBudgetLimit() {
                return ($scope.budgetLimit < $scope.costSpend) ? 0 : ($scope.budgetLimit - $scope.costSpend);
            }

            function getTransferLimit() {
                return ($scope.dataTransferLimit < $scope.transferSpend) ? 0 : ($scope.dataTransferLimit - $scope.transferSpend);
            }

            $scope.ctx1 = document.getElementById('myChart1').getContext('2d');
            $scope.myChart1 = new Chart($scope.ctx1, {
                type: 'doughnut',
                data: {
                    labels: ['Spending Limit', 'Cost Spending'],
                    datasets: [{
                        data: [getBudgetLimit(), $scope.costSpend],
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
                options: {
                    legend: {
                        display: true,
                        labels: {
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
                        data: [getTransferLimit(), $scope.transferSpend],
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
                options: {
                    legend: {
                        display: true,
                        labels: {
                            fontColor: 'rgba(255, 255, 255, 1)'
                        }
                    }
                }
            });

            $scope.loadingVEProfile = false;
        }
        $scope.SuspendUser = function (x) {

            var isSuspend = !x.Is_suspended;
            var account_id = x.AccountID;

            var userSuspend = {
                account_id: account_id.toString(),
                suspend: isSuspend.toString()
            }

            x.SuspendProgress = true;
            svc.triggerIsSuspended(userSuspend).then(function (response) {
                x.SuspendProgress = false;
                x.Is_suspended = !x.Is_suspended;
            })
        };

        $scope.revertCourse = function (name, veprofileid, VETypeID) {
            $scope.showSpecificStudent = false;
            $scope.loadingVEProfile = true;
            $scope.isCourseActivated = !$scope.isCourseActivated;
            $scope.subtitle = name;
            $scope.hidetitle = !$scope.hidetitle;
            $scope.VEType = VETypeID;
            if ($scope.VEType == 7 || $scope.VEType == 6) {
                $scope.usersInLabProfiles = false;
                $scope.isStudentSpecific = false;
                $scope.isVETypeSeven = !$scope.isVETypeSeven;

                svc.getConsoleLab(currentUserGroup).then(function (response) {
                    angular.copy(response, $scope.consoleLabs);
                    if (response.length === 0) {
                        $scope.usersInConsoleLab = false;
                    } else {
                        $scope.usersInConsoleLab = true;
                    }
                    $scope.loadingVEProfile = false;

                    angular.forEach($scope.consoleLabs, function (value) {
                        data.push(String(value.AccountID));
                    });

                    for (var i = 0; i < $scope.consoleLabs.length; i++) {
                        $scope.consoleLabs[i].limitType = null;
                        $scope.consoleLabs[i].UpdateLimitProgress = false;
                    }
                })

            } else if (veprofileid) {
                svc.GetMappingLabProfileUserGroup(currentUserGroup, veprofileid)
                    .then(function (response) {
                        if (response !== undefined) {
                            if ($scope.VEType == 5) {
                                $scope.hasContainer = true;
                            }
                            $scope.continueIncrease = true;
                            $scope.continueDecrease = false;
                            var addendFirst = 0;
                            var addendSecond = 0;
                            var addendThird = 0;
                            var firstcircle = angular.element(document.querySelector('firstcircle'));

                            var secondcircle = angular.element(document.querySelector('secondcircle'));

                            var _sleepFirstCircleIncrease = $interval(function () {        // First Circle Animation (progression)
                                $scope._animateFirstCircleIncrease();
                            }, 1);


                            $scope._animateFirstCircleIncrease = function () {        // First Circle Animation   (progression)
                                var limit_firstcircle = (704 * response.TotalCourseHours) / response.TotalCourseHours;
                                addendFirst = addendFirst + (response.TotalCourseHours / 60 / 176);

                                if (limit_firstcircle >= 0) {
                                    if ($scope.progress_firstcircle === limit_firstcircle) {
                                        $interval.cancel(_sleepFirstCircleIncrease);
                                        _sleepFirstCircleIncrease = undefined;
                                        //$scope.TotalLabHoursPerCourse = parseInt(response.TotalCourseHours / 60);
                                        $scope.TotalLabHoursPerCourse = parseInt(response.TotalCourseHours);
                                    } else {
                                        $scope.progress_firstcircle += 4;
                                        if (Math.floor(addendFirst) >= 1) {
                                            $scope.TotalLabHoursPerCourse += Math.floor(addendFirst);
                                            addendFirst = 0;
                                        } else {
                                            //addendFirst += addendFirst;
                                        }
                                    }
                                    if (!$scope.continueIncrease)
                                        $interval.cancel(_sleepFirstCircleIncrease);
                                } else
                                    $interval.cancel(_sleepFirstCircleIncrease);
                            };

                            var _sleepSecondCircleIncrease = $interval(function () {        // Second Circle Animation  (progression)
                                $scope._animateSecondCircleIncrease();
                            }, 1);

                            $scope._animateSecondCircleIncrease = function () {          // Second Circle Animation  (progression)
                                var limit_secondcircle = (704 * response.TotalRemainingCourseHours) / response.TotalRemainingCourseHours;
                                addendSecond = addendSecond + (response.TotalRemainingCourseHours / 176 / 60);

                                if (limit_secondcircle >= 0) {
                                    if ($scope.progress_secondcircle > limit_secondcircle - 3) {
                                        $interval.cancel(_sleepSecondCircleIncrease);
                                        //$scope.RemainingLabHourCredits = parseInt(response.TotalRemainingCourseHours / 60);
                                        $scope.RemainingLabHourCredits = parseInt(response.TotalRemainingCourseHours);
                                        _sleepSecondCircleIncrease = undefined;
                                    } else {
                                        $scope.progress_secondcircle += 4;
                                        if (Math.floor(addendSecond) >= 1) {
                                            $scope.RemainingLabHourCredits += Math.floor(addendSecond);
                                            addendSecond = 0;
                                        } else {
                                            //addendSecond += addendSecond;
                                        }
                                    }
                                    if (!$scope.continueIncrease)
                                        $interval.cancel(_sleepSecondCircleIncrease);
                                } else
                                    $interval.cancel(_sleepSecondCircleIncrease);
                            };

                            var _sleepThirdCircleIncrease = $interval(function () {        // Second Circle Animation  (progression)
                                $scope._animateThirdCircleIncrease();
                            }, 1);

                            $scope._animateThirdCircleIncrease = function () {          // Second Circle Animation  (progression)
                                var limit_thirdcircle = (704 * response.CourseHours) / response.CourseHours;
                                addendThird = addendThird + ((response.CourseHours) / 176 / 60);

                                if (limit_thirdcircle >= 0) {
                                    if ($scope.progress_thirdcircle > limit_thirdcircle - 3) {
                                        $interval.cancel(_sleepThirdCircleIncrease);
                                        $scope.LabHoursPerUser = parseInt(response.CourseHours);
                                        //$scope.LabHoursPerUser = parseInt(response.CourseHours / 60);
                                        _sleepThirdCircleIncrease = undefined;
                                    } else {
                                        $scope.progress_thirdcircle += 4;
                                        if (Math.floor(addendThird) >= 1) {
                                            $scope.LabHoursPerUser += Math.floor(addendThird);
                                            addendThird = 0;
                                        } else {
                                            //addendThird += addendThird;
                                        }
                                    }
                                    if (!$scope.continueIncrease)
                                        $interval.cancel(_sleepThirdCircleIncrease);
                                } else
                                    $interval.cancel(_sleepThirdCircleIncrease);
                            };
                        }

                        svc.GetUserSchedulePerUserGroup(currentUserGroup, veprofileid)
                            .then(function (response) {
                                angular.copy(response, $scope.userSchedulesperCourse);
                                $scope.totalContainers = response.length;
                                angular.forEach($scope.userSchedulesperCourse, function (value, key) {
                                    var date2 = new Date();
                                    var date1 = new Date($scope.userSchedulesperCourse[key].DaysRemaining);
                                    var timeDiff = Math.ceil(date2.getTime() - date1.getTime());
                                    //$scope.userSchedulesperCourse[key].DaysRemaining = Math.ceil(30 - Math.ceil(timeDiff / (1000 * 3600 * 24)));
                                });
                                svc.DataUsers(currentUserGroup, veprofileid)
                                    .then(function (response) {
                                        angular.copy(response, $scope.DataUsers);
                                        angular.forEach($scope.DataUsers, function (value, key) {
                                            var date2 = new Date();
                                            var date1 = new Date($scope.DataUsers[key].DaysRemaining);
                                            var timeDiff = Math.ceil(date2.getTime() - date1.getTime());
                                           // $scope.DataUsers[key].DaysRemaining = Math.ceil(30 - Math.ceil(timeDiff / (1000 * 3600 * 24)));
                                        });
                                    });
                                if (response.length === 0) {
                                    $scope.usersInLabProfiles = true;
                                } else {
                                    $scope.usersInLabProfiles = false;
                                }
                                $scope.loadingVEProfile = false;
                            });
                    });


            }
        };

        $scope.back = function () {
            $scope.openChart = false;
            $scope.isVETypeSeven = !$scope.isVETypeSeven;
            $scope.VEType = 0;
            $scope.loadingVEProfile = true;
            $scope.isCourseActivated = !$scope.isCourseActivated;
            $scope.subtitle = name;
            $scope.showSpecificStudent = false;
            $scope.isStudentSpecific = false;
        }

        $scope.getLabCreditMinutes = function (x, y) {
            var r = x % y;
            return r;
        };

        $scope.getLabCreditHours = function (x, y) {
            //if (y == 'hours' && x.TimeRemaining <= 60)
            //    var q = Math.floor((x.LabHoursTotal) - (x.TimeRemaining));
            //else
            //    var q = 0;
                //var q = Math.floor((x.LabHoursTotal / 3600) - (x.TimeRemaining / 3600));
            if (y == 'hours') {
                var q = Math.floor(((x.LabHoursTotal * 60) - (x.TimeRemaining / 60)) / 60); 
            }
            
            if (y == 'minutes')
                if (x.TimeRemaining <= 0)
                    var q = 0;
                else
                    var q = 60 - Math.floor(x.TimeRemaining % 3600 / 60) == 60 ? 0 : 60 - Math.floor(x.TimeRemaining % 3600 / 60);
                    //var q = Math.floor(((x.LabHoursTotal * 60) - x.TimeRemaining) / 60);
                   // var q = Math.floor(60 - (x.TimeRemaining % 3600 / 60));
            return q;
        };

        $scope.loadPage = function () {
            svc.getLabProfiles(currentUserGroup)
                .then(function (responseCourses) {
                    if (responseCourses.length === 0) {
                        $scope.empty = true;
                    } else {
                        $scope.empty = false;
                    }
                    for (var i = 0; i < responseCourses.length; i++) {
                        //if (($scope.isUserInstructor || $scope.isUserStaff) && (responseCourses[i].VirtualEnvironment.VETypeID === 1 || responseCourses[i].VirtualEnvironment.VETypeID === 2 || responseCourses[i].VirtualEnvironment.VETypeID === 5))
                        //    $scope.courses.push(responseCourses[i]);
                        if (responseCourses[i].VirtualEnvironment.VETypeID !== 5)
                            $scope.courses.push(responseCourses[i]);
                    }
                });
            svc.DataCourse(currentUserGroupId).then(function (response) {
                $scope.DataCourses = response;
            });
            $timeout(function () { $scope.isLoading = true; }, 1000);
        };

        $scope.loadPage();

        var filterUsers = function (a, b) {
            if ($scope.userManagementUsers.length === 0)
                $scope.searchMsg = "No user profiles found.";
            else
                $scope.userManagementUsers = $filter('orderBy')(userManagementFiltered, a, b);
        };

        $scope.openSpecificStudent = function (user) {
            $scope.showSpecificStudent = true;
            if ($scope.selectedUser !== user) {
                $scope.selectedUser = user;
                $scope.activeRow = false;
            }
            $interval.cancel($scope._sleepFourthCircleIncrease);
            $interval.cancel($scope._sleepCheckRemainingLabHourCreditsPerMinutes);
            $scope.progress_fourthcircle = 0;
            $scope.LabHoursRemaining = 0;
            $scope.LabMinsRemaining = 0;
            //var addendFourth = (user.LabHoursRemaining/60) / 704;
            var addendFourth = 0;
            $scope._sleepFourthCircleIncrease = $interval(function () {        // First Circle Animation (progression)
                $scope._animateFourthCircleIncrease();
            }, 1);

            $scope._animateFourthCircleIncrease = function () {        // First Circle Animation   (progression)
                //var limit_fourthcircle = (704 * (user.TimeRemaining / 60)) / user.LabHoursTotal;
                var limit_fourthcircle = (704 * (user.TimeRemaining / 3600)) / user.LabHoursTotal;
                addendFourth = addendFourth + ((user.TimeRemaining / 3600) / 176);

                if ($scope.progress_fourthcircle >= limit_fourthcircle) {
                    $scope.progress_fourthcircle = limit_fourthcircle;
                    $interval.cancel($scope._sleepFourthCircleIncrease);
                    $scope.LabHoursRemaining = Math.floor(user.TimeRemaining / 3600);
                    if ($scope.LabHoursRemaining <= 0)
                        $scope.LabHoursRemaining = 0;
                    $scope.LabMinsRemaining = Math.floor(user.TimeRemaining % 3600 / 60);
                    if ($scope.LabMinsRemaining <= 0)
                        $scope.LabMinsRemaining = 0;
                    $scope._animateFourthCircleDecreaseFunction(user, addendFourth);
                } else {
                    $scope.progress_fourthcircle += 4;
                    if (Math.floor(addendFourth) >= 1) {
                        $scope.LabMinsRemaining = (Math.floor(addendFourth)) % 60;
                        $scope.LabHoursRemaining = $scope.LabHoursRemaining + Math.floor((Math.floor(addendFourth)) / 60);
                        if (((Math.floor(addendFourth)) / 60) >= 1) {
                            addendFourth = 0;
                        }
                    }
                }
            };
        };

        $scope._animateFourthCircleDecreaseFunction = function (user, addendFourth) {
            var flagRemaining = true;
            var pastLabHoursRemaining = 0;
            $scope.temp = 0;

            svc.getLabSchedule(user.VEProfileId, user.UserId)
                .then(function (response) {
                    $scope.temp = response.LabHoursRemaining;
                    pastLabHoursRemaining = response.LabHoursRemaining;
                });
            $scope._sleepCheckRemainingLabHourCreditsPerMinutes = $interval(function () {   // Fourth Circle Animation (regression) 
                svc.getLabSchedule(user.VEProfileId, user.UserId)
                    .then(function (response) {
                        if ($scope.temp === response.LabHoursRemaining) {
                            pastLabHoursRemaining = response.LabHoursRemaining;
                        }
                        var subtractor = ((pastLabHoursRemaining - response.LabHoursRemaining) * 704) / response.LabHoursTotal;
                        $scope.LabHoursRemaining = Math.floor(response.LabHoursRemaining / 60);
                        $scope.LabMinsRemaining = response.LabHoursRemaining % 60;
                        if (subtractor !== 0) {
                            $scope.progress_fourthcircle -= subtractor;
                            subtractor = 0;
                            pastLabHoursRemaining = response.LabHoursRemaining;
                            $scope.temp = response.LabHoursRemaining;
                        }
                    });
                svc.GetUserSchedulePerUserGroup(currentUserGroup, user.VEProfileId)
                    .then(function (response) {
                        angular.copy(response, $scope.userSchedulesperCourse);
                        if (response.length === 0) {
                            $scope.usersInLabProfiles = true;
                        } else {
                            $scope.usersInLabProfiles = false;
                        }
                    });
            }, 60000);
        };

        $scope.isActiveUser = function (user) {
            return $scope.selectedUser === user;
        };

        $scope.isActiveConsoleUser = function (user) {
            return $scope.selectedUser === user;
        };

        $scope.cname = function (cName) {
            if (cName != undefined && cName.length > 52) {
                var wordChop = cName.trim().substring(0, 52).split(" ").slice(0, -1).join(" ") + "…";
                return wordChop;
            }
            else
                return cName;
        };

        var date = $filter('date')(new Date(), 'MM/dd/yyyy');

        var myStyleCourses = {
            sheetid: 'Course Data',
            headers: true,
            caption: '<br><font size="3"><b>Course List</b></font></br><br><font size="2"><b> ' + groupName + '</b></font></br> <br></br><font size="10"></font> ' + 'As of ' + date + '<br></br><br></br>',
            style: 'font-weight:bold;',
            column: {
                style: 'font-size:12px;border:solid black;'
            },

            columns: [
                { columnid: 'Count', title: 'Count', width: 100, cell: { style: 'text-align:center;' } },
                { columnid: 'CourseName', title: 'Courses', width: 420 },
                { columnid: 'NumberOfUsers', title: 'Number of Users', width: 100, cell: { style: 'text-align:center;' } },
                { columnid: 'TotalCourseHours', title: 'Total Lab Hours per Course', width: 200, cell: { style: 'text-align:center;' } },
                { columnid: 'TotalRemainingCourseHours', title: 'Remaining Lab Hour Credits', width: 200, cell: { style: 'text-align:center;' } },
                { columnid: 'CourseHours', title: 'Lab Hours per Users', width: 150, cell: { style: 'text-align:center;' } }
            ],

            row: {
                style: 'border:solid black'
            }
        };

        $scope.styleDataUsers = function (courseName) {
            var myStyleUsers = {
                sheetid: 'Users Data per Course',
                headers: true,
                caption: '<br><font size="3"><b>Enrolled Users List</b></font></br><br><font size="2"><b>' + groupName + '</b></font></br> <br></br><font size="10"></font> ' + 'As of ' + date + '</br> <br></br><br></br><font size="10"></font><b> Course: ' + courseName + '</b><br></br><br></br>',
                column: {
                    style: 'font-size:12px;border:solid black;'
                },

                columns: [
                    //{
                    //    columnid: 'Name',
                    //    title: 'Count',
                    //    cell: {
                    //        value: function (value) { return value.length }
                    //    },
                    //},                
                    { columnid: 'Count', title: 'Count', width: 100, cell: { style: 'text-align:center;' } },
                    { columnid: 'Name', title: 'Users', width: 450 },
                    { columnid: 'Email', title: 'Email', width: 450 },
                    { columnid: 'ConsumedLabHours', title: 'Consumed Lab Hours', width: 150, cell: { style: 'text-align:center;' } },
                    { columnid: 'RemainingLabHours', title: 'Remaining Lab Hour Credits', width: 200, cell: { style: 'text-align:center;' } },
                    { columnid: 'DaysRemaining', title: 'Days Remaining', width: 100, cell: { style: 'text-align:center;' } }
                ],

                row: {
                    style: 'border:solid black'
                }
            };
            return myStyleUsers;
        };

        $scope.exportDataUsers = function (name) {
            alasql('SELECT * INTO XLS("Enrolled Users List.xls",?) FROM ?', [$scope.styleDataUsers($scope.subtitle), $scope.DataUsers]);
        };

        $scope.exportDataCourses = function () {
            alasql('SELECT * INTO XLS("Course List.xls",?) FROM ?', [myStyleCourses, $scope.DataCourses]);
        };

        $scope.awsData = function () {
            $scope.csv = [];
            $scope.downloadingCSV = true;
            var deferred = $q.defer();
            var dateFrom = $scope.dt.getFullYear() + '-' + ('0' + ($scope.dt.getMonth() + 1)).slice(-2) + '-' + ('0' + $scope.dt.getDate()).slice(-2);
            var dateTo = $scope.dt1.getFullYear() + '-' + ('0' + ($scope.dt1.getMonth() + 1)).slice(-2) + '-' + ('0' + $scope.dt1.getDate()).slice(-2);


            svc.getAWSConsoleDetails(JSON.stringify(data), dateFrom, dateTo).then(function (response) {
                angular.copy(response, $scope.DataAWSUsers);
                $scope.csvAdditional = [{
                    Name: 'Start Date',
                    id: $scope.dt.getDate() + '-' + String($scope.dt.getMonth() + 1).padStart(2, '0') + '-' + $scope.dt.getFullYear()
                }, {
                    Name: 'End Date',
                    id: $scope.dt1.getDate() + '-' + String($scope.dt1.getMonth() + 1).padStart(2, '0') + '-' + $scope.dt1.getFullYear()
                }, {
                    Name: 'Report Name',
                    id: 'Account Consolidated Usage Report',
                }, {
                    Name: 'Description',
                    id: 'Consolidated cost for the accounts present for a tenant',
                }, {
                    Name: 'Downloaded On',
                    id: new Date().toUTCString(),
                }, {
                    Name: 'Tenant Name',
                    id: groupName,
                }

                ];
                $scope.csvHeader = [{
                    TeamName: 'Team Name',
                    Platform: 'Platform',
                    Email: 'Email',
                    LabId: 'Lab Id',
                    Suspended: 'Suspended',
                    price_ytd: 'Aggregated Cloud Cost',
                    price_by_date_range: 'Cloud Cost for selected Duration',
                    Comments: 'Comments',
                }];

                $scope.csv = $scope.csv.concat($scope.csvAdditional);
                $scope.csv = $scope.csv.concat($scope.csvHeader);
                $scope.csv = $scope.csv.concat($scope.DataAWSUsers);
                deferred.resolve($scope.csv);
                $scope.downloadingCSV = false;
            })
            return deferred.promise;
        };

        $scope.limits = [
            { id: 1, name: "DataTransferOutLimit" },
            { id: 2, name: "PriceBudgetLimit" }
        ];

        $scope.changedType = function (item) {
            $scope.newLimitType = item;
        };
        $scope.changedValue = function (item) {
            $scope.newLimitValue = item;
        };


        $scope.UpdateLimitUser = function (x) {
            var account_id = x.AccountID;
            var userUpdateLimit = {
                new_budget_limit: $scope.newLimitValue.toString(),
                account_id: account_id.toString(),
                //account_id: "906875069002",
                limit_type: $scope.newLimitType.toString()
            }
            x.UpdateLimitProgress = true;

            svc.triggerIsUpdateLimit(userUpdateLimit.account_id, userUpdateLimit.new_budget_limit, userUpdateLimit.limit_type).then(function (response) {
                x.UpdateLimitProgress = false;
            })
        };
    }
})();
