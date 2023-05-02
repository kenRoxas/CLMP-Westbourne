(function () {

    "use strict";
    angular.module("app-labprofiles")
        .controller("LabHourExtensionController", LabHourExtensionController)
        .filter('numberOfHours', ['$filter',
            function ($filter) {
                return function (input) {
                    var cd = 24 * 60 * 60 * 1000,
                        ch = 60 * 60 * 1000,
                        d = Math.floor(input / cd),
                        h = Math.floor((input - d * cd) / ch),
                        m = Math.round((input - d * cd - h * ch) / 60000),
                        pad = function (n) { return n < 10 ? '0' + n : n; };
                    if (m === 60) {
                        h++;
                        m = 0;
                    }
                    if (h === 24) {
                        d++;
                        h = 0;
                    }
                    return [pad(d), pad(h), pad(m)].join(':');
                }
            }
        ]);


    LabHourExtensionController.$inject = ['$scope', '$uibModal', '$uibModalInstance', 'svc', '$filter', '$rootScope', 'veprofileId', 'VEName'];

    function LabHourExtensionController($scope, $uibModal, $uibModalInstance, svc, $filter, $rootScope, veprofileId, VEName) {

        var _modalTemplateUrl = '/app/Common/ConfirmationModal.html';
        var _modalController = 'ConfirmationModalController';

        var _currentDateTime = new Date();


        /**
         * Saves the lab hour extension for the whole course or for an individual
         * @param {any} user
         */
        var _onSaveLabHourExtension = function (isFixedLabHourExtension) {
            var vm = $scope.vm;

            var modalData = {
                message: "Continue saving of new lab hour extension?",
                title: "Create Lab Hour Extension",
                type: "Extend",
                messageType: "Extend"
            };

            var userNames = [];
            var scheduleIdList = [];
            var labhourdata = [];

            var inputStartDate = new Date(vm.startSubmittedDate);
            var inputEndDate = new Date(vm.endSubmittedDate);

            var usersList = isFixedLabHourExtension ? vm.usersWithFixedLabHourExtensions : vm.usersWithCustomLabHourExtensions;

            //check whether specified lab hour extension schedule overlaps with any of the users' existing lab hour extensions
            if (usersList.length > 0) {
                angular.forEach(usersList, function (userWithLabExt) {

                    //skip entry if a user object is specified and the Id does not match the current userWithLabExt's UserId
                    if (vm.selectedUser !== null && vm.selectedUser.UserId != userWithLabExt.UserId) {
                        return;
                    }

                    var hasOverlap = false;
                    var storedStartDate = new Date(userWithLabExt.StartDate);
                    var storedEndDate = new Date(userWithLabExt.EndDate);

                    //check whether input dates overlap with any of the users' lab hour extensions
                    hasOverlap = userWithLabExt.ExtensionTypeId == vm.selectedExtensionType.Id && ((inputStartDate >= storedStartDate && inputStartDate <= storedEndDate)
                        || (inputStartDate < storedStartDate && inputEndDate > storedStartDate));

                    if (hasOverlap) {
                        userNames.push(userWithLabExt.Name);
                        scheduleIdList.push(userWithLabExt.LabHourExtensionId);
                    }
                });

                //change modal data info when there are overlaps
                if (scheduleIdList.length > 0) {
                    modalData = {
                        message: "Are you sure you want to overwrite the lab extension hours for the following user/s: ",
                        title: "Create Lab Hour Extension",
                        Names: userNames,
                        type: "Extend",
                        messageType: "Extend"
                    };
                }
            }

            var labhourdata = {
                LabHourExtensionIds: scheduleIdList.length > 0 ? scheduleIdList : null,
                VEProfileId: veprofileId,
                StartDate: inputStartDate,
                EndDate: inputEndDate,
                ExtensionTypeId: vm.selectedExtensionType.Id,
                UserId: vm.selectedUser !== null ? vm.selectedUser.UserId : null,
                CreatedByUserId: currentUserId,
                TotalHours: isFixedLabHourExtension ? parseFloat(vm.selectedHour) + (parseFloat(vm.selectedMinute) / 60) : null,
                IsFixedLabHourExtension: isFixedLabHourExtension
            };

            $uibModal.open({
                templateUrl: _modalTemplateUrl,
                controller: _modalController,
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return modalData;
                    },
                    userGroup: function () {
                        return 0;
                    },
                    VEProfileID: function () {
                        return veprofileId;
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
                        return labhourdata;
                    }
                }
            });
        };

        /**
         * Updates the lab hour extension
         */
        var _onUpdateLabHourExtension = function (isFixedLabHourExtension) {

            var vm = $scope.vm;

            var inputStartDate = new Date(vm.startSubmittedDate);
            var inputEndDate = new Date(vm.endSubmittedDate);

            var modalData = {
                message: "Proceed with updating of the lab hour extension?",
                title: "Update Lab Hour Extension",
                type: "UpdateExtend",
                messageType: "UpdateExtend"
            };

            var scheduleIdList = [];

            var usersList = isFixedLabHourExtension ? vm.usersWithFixedLabHourExtensions : vm.usersWithCustomLabHourExtensions;
            //check whether specified lab hour extension schedule overlaps with any of the user's existing lab hour extensions
            if (usersList.length > 0) {

                var selectedUserRecords = $filter('filter')(usersList, { 'UserId': vm.selectedUser.UserId });
                angular.forEach(selectedUserRecords, function (userWithLabExt) {

                    var hasOverlap = false;
                    var storedStartDate = new Date(userWithLabExt.StartDate);
                    var storedEndDate = new Date(userWithLabExt.EndDate);

                    //check whether input dates overlap with any of the users' lab hour extensions
                    hasOverlap = userWithLabExt.ExtensionTypeId == vm.selectedExtensionType.Id
                        && userWithLabExt.LabHourExtensionId != vm.selectedUser.LabHourExtensionId
                        && ((inputStartDate >= storedStartDate && inputStartDate <= storedEndDate)
                            || (inputStartDate < storedStartDate && inputEndDate > storedStartDate));

                    if (hasOverlap) {
                        scheduleIdList.push(userWithLabExt.LabHourExtensionId);
                    }
                });

                if (scheduleIdList.length > 0) {
                    modalData.message = "The specified schedule coincides with 1 or more of the user's lab hour extension/s. Saving this will override existing data. Do you want to proceed?";
                }
            }

            var labhourdata = {
                OverlappingExtensionIds: scheduleIdList.length > 0 ? scheduleIdList : null,
                LabHourExtensionId: vm.selectedUser.LabHourExtensionId,
                VEProfileId: veprofileId,
                StartDate: inputStartDate,
                EndDate: inputEndDate,
                ExtensionTypeId: vm.selectedExtensionType.Id,
                UserId: vm.selectedUser.UserId,
                EditedByUserId: currentUserId,
                TotalHours: isFixedLabHourExtension ? parseFloat(vm.selectedHour) + (parseFloat(vm.selectedMinute) / 60) : null,
                IsFixedLabHourExtension: isFixedLabHourExtension
            };

            $uibModal.open({
                templateUrl: _modalTemplateUrl,
                controller: _modalController,
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return modalData;
                    },
                    userGroup: function () {
                        return 0;
                    },
                    VEProfileID: function () {
                        return veprofileId;
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
                        return labhourdata;
                    }
                }
            });
        };

        /**
         * Deletes a lab hour extension
         * @param {any} labHourExtensionId
         */
        var _onDeleteLabHourExtension = function (labHourExtensionId) {

            var labhourData = {
                LabHourExtensionIds: [labHourExtensionId],
                EditedByUserId: currentUserId
            };

            var modalData = {
                message: "Are you sure you want to delete the lab hour extension for the user?",
                title: "Delete Lab Hour Extension",
                type: "DeleteExtend",
                messageType: "DeleteExtend"
            };

            $uibModal.open({
                templateUrl: _modalTemplateUrl,
                controller: _modalController,
                size: 'xs',
                backdrop: 'static',
                keyboard: true,
                resolve: {
                    items: function () {
                        return modalData;
                    },
                    userGroup: function () {
                        return 0;
                    },
                    VEProfileID: function () {
                        return null;
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
                        return labhourData;
                    }
                }
            });
        };

        /**
         * Resets some viewModel properties.
         * **/
        var _resetProperties = function () {
            $scope.vm.selectedExtensionType = null;
            $scope.vm.isCreateUpdateMode = false;
            $scope.vm.isForIndividual = false;
            $scope.vm.isEditRecord = false;
            $scope.vm.selectedUser = null;

            //reset data for on going schedule of user

            $scope.vm.originalLabHourExtensionData.startDate = null;
            $scope.vm.originalLabHourExtensionData.endDate = null;
            $scope.vm.originalLabHourExtensionData.totalHour = null;
            $scope.vm.originalLabHourExtensionData.totalMinute = null;
            $scope.vm.originalLabHourExtensionData.isScheduleOnGoing = false;

            // reset values for fixed lab hour extension validation
            $scope.vm.originalTotalTimeValue = null;
            $scope.vm.totalTimeValue = null;
            $scope.vm.initialDateDifference = null;
            $scope.vm.isInputInvalid = false;

            // reset value for invalid input message
            $scope.vm.errorMessage = "";

            //reset min dates for the start and end datetime pickers
            $scope.vm.startSubmittedDate = new Date();
            $scope.vm.endSubmittedDate = $scope.vm.startSubmittedDate;

            $scope.vm.selectedHour = 0;
            $scope.vm.selectedMinute = 0;

            $scope.vm.startSubmittedDateOptions.minDate = $scope.vm.endSubmittedDateOptions.minDate = $scope.vm.startSubmittedDate;
        };

        /**
         * Verifies if the lab hour extension schedule is on-going
         * @param {any} user
         */
        var _isLabHourExtensionOnGoing = function (user) {
            var startDate = new Date(user.StartDate);
            var endDate = new Date(user.EndDate);

            var currentDate = new Date();

            return startDate <= currentDate && currentDate <= endDate;
        };

        /**
         * Verifies if the lab hour extension schedule is in the past
         * @param {any} user
         */
        var _isInLabHourExtensionIsInThePast = function (user) {
            var startDate = new Date(user.StartDate);
            var endDate = new Date(user.EndDate);

            var currentDate = new Date();

            return startDate < currentDate && endDate < currentDate;
        };

        /**
         * Initialize the types of lab hour extensions and users list.
         * **/
        var _initializeViewModel = function () {
            _resetProperties();
            var vm = $scope.vm;
            vm.pageProperties.loading = true;
            vm.pageProperties.loadingCustomLabHourExtensions = true;
            vm.isFixedLabHourExtension = false;

            svc.getExtensionTypes()
                .then(function (response) {
                    angular.copy(response, vm.extensionTypes);

                    var searchOptions = {
                        searchText: vm.searchTextCustom,
                        startDate: vm.startDateFilterCustom,
                        endDate: vm.endDateFilterCustom,
                        sortDirection: vm.sortDirectionCustom,
                        sortField: vm.sortFieldCustom,
                        veprofileId: veprofileId,
                        showAllRecords: vm.showAllRecordsCustom
                    };

                    return svc.getUsersWithCustomLabHourExtensions(searchOptions);
                })
                .then(function (response) {

                    _mapGetUsersResponse(response, false);

                    vm.pageProperties.loading = false;
                    vm.pageProperties.loadingCustomLabHourExtensions = false;
                });

            vm.VEName = VEName;
        };

        var _refreshLists = function () {
            var vm = $scope.vm;
            _resetProperties();

            if (vm.isFixedLabHourExtension) {
                var searchOptions = {
                    searchText: vm.searchTextFixed,
                    startDate: vm.startDateFilterFixed,
                    endDate: vm.endDateFilterFixed,
                    sortDirection: vm.sortDirectionFixed,
                    sortField: vm.sortFieldFixed,
                    veprofileId: veprofileId,
                    showAllRecords: vm.showAllRecordsFixed
                };

                if (!vm.pageProperties.loadingFixedLabHourExtensions) {
                    vm.pageProperties.loadingFixedLabHourExtensions = true;

                    svc.getUsersWithFixedLabHourExtensions(searchOptions)
                        .then(function (response) {
                            _mapGetUsersResponse(response, true);
                            vm.pageProperties.loadingFixedLabHourExtensions = false;
                        });
                }

            }
            else {
                var searchOptions = {
                    searchText: vm.searchTextCustom,
                    startDate: vm.startDateFilterCustom,
                    endDate: vm.endDateFilterCustom,
                    sortDirection: vm.sortDirectionCustom,
                    sortField: vm.sortFieldCustom,
                    veprofileId: veprofileId,
                    showAllRecords: vm.showAllRecordsCustom
                };

                if (!vm.pageProperties.loadingCustomLabHourExtensions) {
                    vm.pageProperties.loadingCustomLabHourExtensions = true;

                    svc.getUsersWithCustomLabHourExtensions(searchOptions)
                        .then(function (response) {
                            _mapGetUsersResponse(response, false);
                            vm.pageProperties.loadingCustomLabHourExtensions = false;
                        });
                }
            }
        };

        var _mapGetUsersResponse = function (response, isFixedLabHourExtension) {

            var usersList = isFixedLabHourExtension ? $scope.vm.usersWithFixedLabHourExtensions : $scope.vm.usersWithCustomLabHourExtensions;

            angular.copy(response, usersList);
            angular.forEach(usersList, function (user) {

                user.ExtensionIsOnGoing = _isLabHourExtensionOnGoing(user);
                user.ExtensionIsInThePast = _isInLabHourExtensionIsInThePast(user);
                user.SelectedExtensionType = $filter('filter')($scope.vm.extensionTypes, { 'Id': user.ExtensionTypeId })[0];
                user.SelectedExtensionTypeValue = user.SelectedExtensionType ? user.SelectedExtensionType.Label : '';

                if ($scope.vm.userOptions.length === 0
                    || $filter('filter')($scope.vm.userOptions, { 'UserId': user.UserId }).length === 0) {
                    $scope.vm.userOptions.push(user);
                }
            });
        };


        /*
         Event handler for invalid input message.
         */



        var _validateFixedLabHourExtensionValues = function () {
            var vm = $scope.vm;

            vm.errorMessage = "";

            // sum of total hour and total minute of user with schedule on going
            var _originalTotalTimeValue = parseFloat(vm.originalLabHourExtensionData.totalHour) + (parseFloat(vm.originalLabHourExtensionData.totalMinute));

            // actual difference of endSubmittedDate and startSubmittedDate
            var _timeDifference = (vm.endSubmittedDate - vm.startSubmittedDate) / (1000 * 3600);

            // sum of new total hour and total minute input by user
            var _newTotalTimeValue = parseFloat(vm.selectedHour) + (parseFloat(vm.selectedMinute) / 60);

            var _compareEndDateToOriginalEndDate = ((vm.endSubmittedDate) / (1000 * 3600)) < ((vm.originalLabHourExtensionData.endDate) / (1000 * 3600));

            var _compareEndDateToCurrentDate = ((vm.endSubmittedDate) / (1000 * 3600)) < ((_currentDateTime) / (1000 * 3600));




            if (vm.isEditRecord && vm.originalLabHourExtensionData.isScheduleOnGoing && _newTotalTimeValue >= _timeDifference) {
                vm.errorMessage = "Total hours cannot exceed the difference between End Date and Start Date.";
                return false;
            } else if (vm.isEditRecord && vm.originalLabHourExtensionData.isScheduleOnGoing && _newTotalTimeValue < _originalTotalTimeValue) {
                vm.errorMessage = "The new total hours cannot be lesser than the initial total hour value.";
            } else if ((vm.isEditRecord && _newTotalTimeValue > _timeDifference && !vm.originalLabHourExtensionData.isScheduleOnGoing) ||
                (!vm.isEditRecord && _newTotalTimeValue > _timeDifference)) {
                vm.errorMessage = "Total hours cannot exceed the difference between End Date and Start Date.";
                return false;
            } else if ((vm.isEditRecord && _compareEndDateToOriginalEndDate) || _compareEndDateToCurrentDate) {
                vm.errorMessage = "The new end date value cannot be earlier than the current date and time or the previous end date value of " + vm.originalLabHourExtensionData.endDate + ".";
                return false;
            } else {
                return true;
            }
        };


        var _onSearchDateChange = function () {
            var vm = $scope.vm;
            var startDateFilter = vm.isFixedLabHourExtension ? vm.startDateFilterFixed : vm.startDateFilterCustom;
            var endDateFilter = vm.isFixedLabHourExtension ? vm.endDateFilterFixed : vm.endDateFilterCustom;

            if (startDateFilter !== null && endDateFilter !== null && startDateFilter > endDateFilter) {
                endDateFilter = startDateFilter;
            }

            _refreshLists();
        };

        /**
         * Event handler for the date change.
         * **/
        var _onDateChange = function () {
            var dateNow = new Date();

            if ($scope.vm.isFixedLabHourExtension && $scope.vm.originalLabHourExtensionData.isScheduleOnGoing) {

                $scope.vm.endSubmittedDateOptions.minDate = $scope.vm.originalLabHourExtensionData.endDate;

            } else {


                if ($scope.vm.startSubmittedDate !== null && $scope.vm.startSubmittedDate < dateNow) {
                    $scope.vm.startSubmittedDate = new Date();

                    //reset min dates for the start and end datetime pickers
                    $scope.vm.startSubmittedDateOptions.minDate = $scope.vm.endSubmittedDateOptions.minDate = $scope.vm.startSubmittedDate;
                }

                //reset end datetime to start datetime if start datetime is later than end datetime
                if ($scope.vm.endSubmittedDate !== null && $scope.vm.startSubmittedDate !== null && $scope.vm.startSubmittedDate > $scope.vm.endSubmittedDate) {
                    $scope.vm.endSubmittedDate = $scope.vm.startSubmittedDate;
                }
                //reset end datetime to current datetime if inputted end date is less than current datetime
                else if ($scope.vm.endSubmittedDate !== null && $scope.vm.startSubmittedDate == null && $scope.vm.endSubmittedDate < (new Date())) {

                    dateNow = new Date();
                    $scope.vm.startSubmittedDate = $scope.vm.endSubmittedDate = dateNow;
                    //reset min dates for the start and end datetime pickers
                    $scope.vm.startSubmittedDateOptions.minDate = $scope.vm.endSubmittedDateOptions.minDate = $scope.vm.startSubmittedDate;
                }
                //reset start datetime to end datetime if start datetime is null
                else if ($scope.vm.endSubmittedDate !== null && $scope.vm.startSubmittedDate == null) {
                    $scope.vm.startSubmittedDate = $scope.vm.endSubmittedDate;
                }
            }
        };

        // Export to Excel
        var _date = $filter('date')(new Date(), 'MM/dd/yyyy');

        var _fixedLabHourExtensionReportColumns = [
            { columnid: 'Name', title: 'Name', cell: { style: 'text-align:left;' } },
            { columnid: 'Email', title: 'Email', cell: { style: 'text-align:left;' } },
            { columnid: 'TotalHoursDisplay', title: 'Total Time (hh:mm)', cell: { style: 'text-align:right;' } },
            { columnid: 'DateValidDisplay', title: 'Date Valid', cell: { style: 'text-align:right;' } },
            { columnid: 'SelectedExtensionTypeValue', title: 'Type of Extension', cell: { style: 'text-align:left;' } },
            { columnid: 'IsDeleted', title: 'Deleted', cell: { style: 'text-align:left;' } }
        ];

        var _customLabHourExtensionReportColumns = [
            { columnid: 'Name', title: 'Name', cell: { style: 'text-align:left;' } },
            { columnid: 'Email', title: 'Email', cell: { style: 'text-align:left;' } },
            { columnid: 'StartDateDisplay', title: 'Start Date', cell: { style: 'text-align:right;' } },
            { columnid: 'EndDateDisplay', title: 'End Date', cell: { style: 'text-align:right;' } },
            { columnid: 'Duration', title: 'Duration <br> (DD:HH:MM)', cell: { style: 'text-align:right;' } },
            { columnid: 'SelectedExtensionTypeValue', title: 'Type of Extension', cell: { style: 'text-align:left;' } },
            { columnid: 'IsDeleted', title: 'Deleted', cell: { style: 'text-align:left;' } }
        ];

        var _myStyleCourses = {

            sheetid: 'Lab Hours Extension Data',
            headers: true,
            caption: '<br><font size="3"><b>Lab Hours Extension Summary</b></font></br><br><font size="2"><b> ' + VEName + '</b></font></br> <br></br><font size="10"></font> ' + 'As of ' + _date + '<br></br><br></br>',
            style: 'font-weight:bold;',
            column: {
                style: 'font-size:12px;border:solid black;'
            },
            columns: [],
            row: {
                style: 'border:solid black'
            }
        };

        var _exportData = function () {

            var usersList = $scope.vm.isFixedLabHourExtension ? $scope.vm.usersWithFixedLabHourExtensions : $scope.vm.usersWithCustomLabHourExtensions;
            _myStyleCourses.columns = $scope.vm.isFixedLabHourExtension ? _fixedLabHourExtensionReportColumns : _customLabHourExtensionReportColumns;

            alasql('SELECT * INTO XLS("Lab Hours Extension Data.xls",?) FROM ?', [_myStyleCourses, usersList]);
        };


        // For table list sorting
        var _caretChangeForFixed = {
            Name: false,
            Email: false,
            StartDate: false,
            ExtensionTypeId: false
        };

        var _caretChangeForCustom = {
            Name: false,
            Email: false,
            StartDate: false,
            EndDate: false,
            ExtensionTypeId: false
        };

        var _defaultCaretChangeForFixed = {
            Name: false,
            Email: true,
            StartDate: true,
            ExtensionTypeId: true
        };

        var _defaultCaretChangeForCustom = {
            Name: false,
            Email: true,
            StartDate: true,
            EndDate: true,
            ExtensionTypeId: true
        };

        var _onCaretChange = function (column) {
            var vm = $scope.vm;
            var caretChange = vm.isFixedLabHourExtension ? vm.caretChangeForFixed : vm.caretChangeForCustom;
            var defaultCaretChange = vm.isFixedLabHourExtension ? vm.defaultCaretChangeForFixed : vm.defaultCaretChangeForCustom;

            for (var x in caretChange) {
                if (x === column)
                    caretChange[x] = !caretChange[x];
                else
                    caretChange[x] = false;
            }
            for (var y in defaultCaretChange) {
                if (y === column)
                    defaultCaretChange[y] = false;
                else
                    defaultCaretChange[y] = true;
            }
        };

        var _tableSort = function (column) {
            _onCaretChange(column);

            var vm = $scope.vm;
            var sortReversed = vm.isFixedLabHourExtension ? vm.sortDirectionFixedReversed : vm.sortDirectionCustomReversed;

            //sortField = column;
            sortReversed = !sortReversed;
            var sortDirection = sortReversed ? 'desc' : 'asc';

            if (vm.isFixedLabHourExtension) {
                vm.sortDirectionFixedReversed = sortReversed;
                vm.sortDirectionFixed = sortDirection;
                vm.sortFieldFixed = column
            }
            else {
                vm.sortDirectionCustomReversed = sortReversed;
                vm.sortDirectionCustom = sortDirection;
                vm.sortFieldCustom = column;
            }

            _refreshLists();
        };


        /**
         * The viewModel
         * **/
        var viewModel = {
            selectedExtensionType: {},
            extensionTypes: [],
            isCreateUpdateMode: false,
            isEditRecord: false,
            VEName: '',
            usersWithCustomLabHourExtensions: [],
            usersWithFixedLabHourExtensions: [],
            userOptions: [],
            pageProperties: {
                loading: true,
                loadingCustomLabHourExtensions: false,
                loadingFixedLabHourExtensions: false
            },
            selectedUser: null,
            isFixedLabHourExtension: false,
            isTotalTimeInputInvalid: false,
            isDateInputInvalid: false,
            isDateInputInvalidForOnGoingExtension: false,



            onViewModelReady: _initializeViewModel,

            close: function () {
                $("html").removeAttr('style');
                $uibModalInstance.close();
            },

            originalLabHourExtensionData: {
                startDate: null,
                endDate: null,
                totalHour: null,
                totalMinute: null,
                isScheduleOnGoing: false
            },

            startSubmittedDateOptions: {
                minDate: _currentDateTime,
                maxDate: null,
                showFlag: false
            },

            startSubmittedDate: _currentDateTime,

            endSubmittedDateOptions: {
                minDate: _currentDateTime,
                maxDate: null,
                showFlag: false
            },

            endSubmittedDate: _currentDateTime,

            openCalendar: function (e, date) {
                $scope.vm.open[date] = true;
            },


            toggleEditLabHourExtensionView: function (user) {

                $scope.vm.selectedUser = user;

                $scope.vm.startSubmittedDate = new Date(user.StartDate);
                $scope.vm.endSubmittedDate = new Date(user.EndDate);
                $scope.vm.selectedExtensionType = user.SelectedExtensionType;

                $scope.vm.isCreateUpdateMode = true;
                $scope.vm.isForIndividual = true;
                $scope.vm.isEditRecord = true;

                $scope.vm.selectedHour = user.TotalHourValue;
                $scope.vm.selectedMinute = user.TotalMinuteValue;

                if (user.ExtensionIsOnGoing) {
                    // preventing user to edit the end date earlier than the original end date
                    $scope.vm.endSubmittedDateOptions.minDate = user.EndDate;

                    // store original data
                    $scope.vm.originalLabHourExtensionData.startDate = user.StartDate;
                    $scope.vm.originalLabHourExtensionData.endDate = user.EndDate;
                    $scope.vm.originalLabHourExtensionData.totalHour = user.TotalHourValue;
                    $scope.vm.originalLabHourExtensionData.totalMinute = user.TotalMinuteValue;
                    $scope.vm.originalLabHourExtensionData.isScheduleOnGoing = user.ExtensionIsOnGoing;
                }


            },

            saveLabHourExtension: function (isFixedLabHourExtension) {
                if (isFixedLabHourExtension && _validateFixedLabHourExtensionValues()) {
                    return _onSaveLabHourExtension(isFixedLabHourExtension);
                }

                if (!isFixedLabHourExtension) {
                    return _onSaveLabHourExtension(isFixedLabHourExtension);
                }

            },

            updateLabHourExtension: function (isFixedLabHourExtension) {
                if (isFixedLabHourExtension && _validateFixedLabHourExtensionValues()) {
                    return _onUpdateLabHourExtension(isFixedLabHourExtension);
                }

                if (!isFixedLabHourExtension) {
                    return _onUpdateLabHourExtension(isFixedLabHourExtension);
                }



            },

            deleteLabHourExtension: function (user) {
                if (user.ExtensionIsOnGoing) {
                    return;
                }

                _onDeleteLabHourExtension(user.LabHourExtensionId);
            },

            cancelCreation: _resetProperties,

            toggleLabHoursExtensionView: function (isFixedLabHourExtension) {
                $scope.vm.isFixedLabHourExtension = isFixedLabHourExtension;
                _refreshLists();
            },

            toggleCreateLabHourExtensionView: function (isForIndividual) {
                $scope.vm.isCreateUpdateMode = true;
                $scope.vm.isForIndividual = isForIndividual;
            },



            onExtensionTypeChange: function (user) {
                user.ExtensionTypeId = user.SelectedExtensionType.Id;
            },

            onDateChange: _onDateChange,

            isLabHourExtensionOnGoing: _isLabHourExtensionOnGoing,

            onSearch: _refreshLists,

            onSearchDateChange: _onSearchDateChange,

            /* onSelectedTimeChange: _onSelectedTimeChange,*/

            exportDataToExcel: _exportData,

            caretChangeForFixed: _caretChangeForFixed,
            caretChangeForCustom: _caretChangeForCustom,

            defaultCaretChangeForFixed: _defaultCaretChangeForFixed,
            defaultCaretChangeForCustom: _defaultCaretChangeForCustom,

            sortList: _tableSort,

            searchTextCustom: '',
            searchTextFixed: '',

            startDateFilterFixed: _currentDateTime,
            startDateFilterCustom: _currentDateTime,

            endDateFilterFixed: null,
            endDateCustomFixed: null,

            sortDirectionCustom: 'asc',
            sortDirectionFixed: 'asc',

            sortDirectionCustomReversed: false,
            sortDirectionFixedReversed: false,

            sortFieldCustom: 'Name',
            sortFieldFixed: 'Name',

            searchStartDateOptionsForCustom: {
                minDate: null,
                maxDate: null,
                showFlag: false
            },

            searchEndDateOptionsForCustom: {
                minDate: null,
                maxDate: null,
                showFlag: false
            },

            searchStartDateOptionsForFixed: {
                minDate: null,
                maxDate: null,
                showFlag: false
            },

            searchEndDateOptionsForFixed: {
                minDate: null,
                maxDate: null,
                showFlag: false
            },



            showAllRecordsCustom: false,
            showAllRecordsFixed: false,

            selectedHour: 0,
            selectedMinute: 0,

            hoursOptions: [
                { hourValue: 0, hourDisplay: '00' },
                { hourValue: 1, hourDisplay: '01' },
                { hourValue: 2, hourDisplay: '02' },
                { hourValue: 3, hourDisplay: '03' },
                { hourValue: 4, hourDisplay: '04' },
                { hourValue: 5, hourDisplay: '05' },
                { hourValue: 6, hourDisplay: '06' },
                { hourValue: 7, hourDisplay: '07' },
                { hourValue: 8, hourDisplay: '08' },
                { hourValue: 9, hourDisplay: '09' },
                { hourValue: 10, hourDisplay: '10' },
                { hourValue: 11, hourDisplay: '11' },
                { hourValue: 12, hourDisplay: '12' },
                { hourValue: 13, hourDisplay: '13' },
                { hourValue: 14, hourDisplay: '14' },
                { hourValue: 15, hourDisplay: '15' },
                { hourValue: 16, hourDisplay: '16' },
                { hourValue: 17, hourDisplay: '17' },
                { hourValue: 18, hourDisplay: '18' },
                { hourValue: 19, hourDisplay: '19' },
                { hourValue: 20, hourDisplay: '20' },
                { hourValue: 21, hourDisplay: '21' },
                { hourValue: 22, hourDisplay: '22' },
                { hourValue: 23, hourDisplay: '23' },
            ],

            minutesOptions: [
                { minuteValue: 0, minuteDisplay: '00' },
                { minuteValue: 1, minuteDisplay: '01' },
                { minuteValue: 2, minuteDisplay: '02' },
                { minuteValue: 3, minuteDisplay: '03' },
                { minuteValue: 4, minuteDisplay: '04' },
                { minuteValue: 5, minuteDisplay: '05' },
                { minuteValue: 6, minuteDisplay: '06' },
                { minuteValue: 7, minuteDisplay: '07' },
                { minuteValue: 8, minuteDisplay: '08' },
                { minuteValue: 9, minuteDisplay: '09' },
                { minuteValue: 10, minuteDisplay: '10' },
                { minuteValue: 11, minuteDisplay: '11' },
                { minuteValue: 12, minuteDisplay: '12' },
                { minuteValue: 13, minuteDisplay: '13' },
                { minuteValue: 14, minuteDisplay: '14' },
                { minuteValue: 15, minuteDisplay: '15' },
                { minuteValue: 16, minuteDisplay: '16' },
                { minuteValue: 17, minuteDisplay: '17' },
                { minuteValue: 18, minuteDisplay: '18' },
                { minuteValue: 19, minuteDisplay: '19' },
                { minuteValue: 20, minuteDisplay: '20' },
                { minuteValue: 21, minuteDisplay: '21' },
                { minuteValue: 22, minuteDisplay: '22' },
                { minuteValue: 23, minuteDisplay: '23' },
                { minuteValue: 24, minuteDisplay: '24' },
                { minuteValue: 25, minuteDisplay: '25' },
                { minuteValue: 26, minuteDisplay: '26' },
                { minuteValue: 27, minuteDisplay: '27' },
                { minuteValue: 28, minuteDisplay: '28' },
                { minuteValue: 29, minuteDisplay: '29' },
                { minuteValue: 30, minuteDisplay: '30' },
                { minuteValue: 21, minuteDisplay: '31' },
                { minuteValue: 32, minuteDisplay: '32' },
                { minuteValue: 33, minuteDisplay: '33' },
                { minuteValue: 34, minuteDisplay: '34' },
                { minuteValue: 35, minuteDisplay: '35' },
                { minuteValue: 36, minuteDisplay: '36' },
                { minuteValue: 37, minuteDisplay: '37' },
                { minuteValue: 38, minuteDisplay: '38' },
                { minuteValue: 39, minuteDisplay: '39' },
                { minuteValue: 40, minuteDisplay: '40' },
                { minuteValue: 41, minuteDisplay: '41' },
                { minuteValue: 42, minuteDisplay: '42' },
                { minuteValue: 43, minuteDisplay: '43' },
                { minuteValue: 44, minuteDisplay: '44' },
                { minuteValue: 45, minuteDisplay: '45' },
                { minuteValue: 46, minuteDisplay: '46' },
                { minuteValue: 47, minuteDisplay: '47' },
                { minuteValue: 48, minuteDisplay: '48' },
                { minuteValue: 49, minuteDisplay: '49' },
                { minuteValue: 50, minuteDisplay: '50' },
                { minuteValue: 51, minuteDisplay: '51' },
                { minuteValue: 52, minuteDisplay: '52' },
                { minuteValue: 53, minuteDisplay: '53' },
                { minuteValue: 54, minuteDisplay: '54' },
                { minuteValue: 55, minuteDisplay: '55' },
                { minuteValue: 56, minuteDisplay: '56' },
                { minuteValue: 57, minuteDisplay: '57' },
                { minuteValue: 58, minuteDisplay: '58' },
                { minuteValue: 59, minuteDisplay: '59' }
            ]
        };

        //initialize the viewModel
        $scope.vm = viewModel;
        $scope.vm.onViewModelReady();

        $rootScope.$on('extensionUserLoading', _refreshLists);

    }
})();