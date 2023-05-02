(function () {

    "use strict";
    angular.module("app-Usermanagement")
        .controller("createUserController", createUserController).directive('unique', unique).directive('pwCheck', function () {
            return {
                require: 'ngModel',
                link: function (scope, elem, attrs, ctrl) {
                    var firstPassword = '#' + attrs.pwCheck;
                    elem.add(firstPassword).on('keyup', function () {
                        scope.$apply(function () {

                            ctrl.$setValidity('pwmatch', elem.val() === $(firstPassword).val());
                            if (elem.val() === $(firstPassword).val())
                                return true;
                            else
                                return false;
                        });
                    });
                }
            };

        });

    unique.$inject = ['$q', '$timeout', 'svc'];
    function unique($q, $timeout, svc) {
            return {
                require: 'ngModel',
                link: function (scope, elm, attrs, ctrl) {

                    ctrl.$asyncValidators.unique = function (modelValue, viewValue) {

                        if (ctrl.$isEmpty(modelValue)) {
                            return $q.resolve();
                        }

                        var def = $q.defer();

                        svc.getEmailAddressExist(modelValue).then(
                            function (response) {
                                if (response === true)
                                    def.reject();
                                else
                                    def.resolve();
                            });

                        return def.promise;
                    };
                }
            };
        }

    createUserController.$inject = ['$scope', 'items', '$uibModalInstance', '$uibModal', '$http', 'svc', '$rootScope', '$q'];

    function createUserController($scope, items, $uibModalInstance, $uibModal, $http, svc, $rootScope, $q) {
        $scope.$on('LOAD', function () { $scope.loader = true; $scope.oppositeLoader = false; });
        $scope.$on('UNLOAD', function () { $scope.loader = false; $scope.oppositeLoader = true; });
        $scope.required_control = true;

        //$scope.$emit('LOAD');
        $scope.createEditModalHeader = items.createEditModalHeader;
        $scope.showEdit = items.showEdit;
        $scope.isShowCreate = items.isShowCreate;
        $scope.isShowEdit = items.isShowEdit;
        $scope.saveOrCreate = items.saveOrCreate;
        $scope.showPassword = items.showPassword;
        $scope.showOrHidePass = 'password';
        $scope.showOrHideConfirmPass = 'password';
        $scope.emailPattern = /^(([^<>()\[\]\\.,;:\s@\"]+(\.[^<>()\[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        $scope.passPattern = /^(([\d\w\W])*)$/;
        const r = items.userDetails.Role;
        $scope.id = items.userDetails.Id;
        $scope.firstname = items.userDetails.Firstname;
        $scope.lastname = items.userDetails.Lastname;
        $scope.email1 = items.userDetails.Email;
        $scope.groups = [];
        $scope.isSuperAdmin = userIsSuperAdmin;
        //$scope.role = items.userDetails.RoleName;
        //$scope.roleSelected = items.userDetails.RoleName;
        //$scope.userGroupSelected = items.groupOptions.find(x => x.GroupName === items.userDetails.GroupName);                 
        //items.userDetails.UserGroup;
        $scope.roles = items.roleOptions;
        if (userRoleName == "SuperAdmin")
            $scope.groups = items.groupOptions;
        else {
            angular.forEach(items.groupOptions, function (value, key) {
                if (value.GroupName == groupName)
                    $scope.groups.push(value);
            });
        }
        $scope.isCreatedUser = items.isCreatedUser;
        $scope.isBulk = items.isBulk;
        $scope.isExport = items.isExport;
        $scope.showError = true;
        $scope.bulkFile = "";
        $scope.csv = [];
        $scope.DataUserLists = [];
        $scope.onChangePic = function () {
            return "../../Content/Image/default_profile_picture.png";
        };

        $scope.glyph = false;
        $scope.glyphPass = false;
        //$scope.email = {};
        //$scope.$watch('email', function (newVal, oldVal) {
        //    alert("NewVal" + newVal);
        //    alert("oldVal" + oldVal);
        //});
        $scope.temp = "";
        //$scope.getInput = function () {
        //    //$scope.email = "";
        //    alert($scope.email.qqq);
        //}
        $scope.disableHyphen = function (e) {
            if (e.keyCode === 45) {
                e.preventDefault();
            }
        };

        svc.getUserGroups().then(
            function (response) {
                $scope.UserGroups = response;
            });
      

            $scope.mainLoad = function () {
                if ($scope.roles !== undefined && $scope.groups !== undefined) {
                    $scope.$emit('UNLOAD');
                }
                else {
                    if (items.saveOrCreate === 'Create') {
                        $scope.$emit('LOAD');
                        svc.roleCloudOptions(userRoleName).then(
                            function (responseResult) {
                                $scope.roles = responseResult;
                            });

                        svc.getUserGroups().then(
                            function (responseGroup) {
                                var temp = responseGroup;
                                $scope.groups = temp;
                                $scope.$emit('UNLOAD');
                            });
                    }
                }

                if (items.saveOrCreate === 'Save') {
                    $scope.$emit('UNLOAD');
                    $scope.roleSelected = items.userDetails.RoleName;
                    $scope.userGroupSelected = items.groupOptions.find(x => x.GroupName === items.userDetails.GroupName);
                }
            };
            $scope.mainLoad();
            $scope.showPass = function () {
                if ($scope.showOrHidePass === 'password') {
                    $scope.showOrHidePass = 'text';
                    $scope.glyphPass = true;
                }
                else {
                    $scope.showOrHidePass = 'password';
                    $scope.glyphPass = false;
                }
            };

            $scope.showConfirmPass = function () {
                if ($scope.showOrHideConfirmPass === 'password') {
                    $scope.glyph = true;
                    $scope.showOrHideConfirmPass = 'text';
                }
                else {
                    $scope.showOrHideConfirmPass = 'password';
                    $scope.glyph = false;
                }
            };

            $scope.close = function () {
                $uibModalInstance.close();
            };

            $scope.submit = function (userGroup, email, role, firstname, lastname, password, passwordCompare, tenantId, type, id) {
                var user = {
                    FirstName: firstname,
                    LastName: lastname,
                    Email: email,
                    Password: password,
                    ConfirmPassword: passwordCompare,
                    Roles: role,
                    id: id,
                    UserGroup: userGroup,
                    TenantId: tenantId
                };

                $scope.alertModal(type, user, null);

            };

            $scope.alertModal = function (type, user, data) {
                var details;
                switch (type) {
                    case 'Alert':
                        details = {
                            message: user.email + " already exist. Please use a different email.",
                            title: type,
                            user: user
                        };
                        break;
                    case 'Create':
                        details = {
                            message: "User profile was created successfully.",
                            title: 'Information',
                            type: type,
                            user: user
                        };
                        break;
                    case 'Edit':
                        details = {
                            message: "User profile has been sucessfully updated.",
                            title: 'Information',
                            type: type,
                            user: user,
                            role: r
                        };
                        break;
                    case 'Add another':
                        var vm = this;
                        details = {
                            message: "User profile was created successfully.",
                            title: 'Information',
                            type: type,
                            user: user
                        };
                        vm.myForm.$setPristine();
                        vm.myForm.$setUntouched();
                        $scope.firstname = "";
                        $scope.lastname = "";
                        $scope.email = "";
                        $scope.role = "";
                        $scope.userGroup = "";
                        $scope.password = "";
                        $scope.confirmPassword = "";
                        break;
                    case 'Reactivate User':
                        details = {
                            message: "User is currently deactivated. Do you want to reactivate and continue on user creation " + user.email,
                            title: type,
                            user: user
                        };
                        break;
                    case 'Bulk':
                        details = {
                            message: "Uploading user . . .",
                            title: "Bulk Create",
                            type: type,
                            user: user,
                            file: data
                        };
                        break;
                }

                var modal = $uibModal.open({
                    animation: true,
                    templateUrl: '/app/Usermanagement/Modal/alertModal.html',
                    controller: 'alertModalController',
                    size: 'md',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        items: function () {
                            return details;
                        }
                    }
                });
                modal.result.then(function (result) {
                    if (result) {
                        $uibModalInstance.close();                                  
                    }

                }, function () {
                });
            };

            $scope.ifEmailExist = function (id, email) {
                $scope.isExist = false;

                svc.getEmailAddressExist(email).then(
                    function (response) {
                        if (response === true)
                            $scope.isExist = false;
                        else
                            $scope.isExist = true;

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
                    else
                        {
                            $scope.showError = true;
                            $scope.errorMsg = "Please select .csv or .xlsx file to upload";
                        }         
                }                
            };

        $scope.upload = function () {
            $scope.alertModal("Bulk", null, $scope.bulkFile);

                //svc.uploadBulk($scope.bulkFile).then(
                //    function (response) {
                //        $rootScope.$emit("CallParentMethod", {});
                //        $uibModalInstance.close(true);
                //    }
                //);
        }

        $scope.userList = function () {
            var deferred = $q.defer();

            svc.getUserByUserGroupId(
                $scope.SelectExportUser.CloudLabsGroupID).then(function (response) {
                angular.copy(response, $scope.DataUserLists);
               
                $scope.csvHeader = [{
                    Email: 'Email',
                    Firstname: 'Firstname',
                    Lastname: 'Lastname',
                    ClientName: 'Client Name',
                    Role : 'Role',
                    DateCreated: 'Date Created'                   
                }];

                $scope.csv = $scope.csv.concat($scope.csvHeader);
                $scope.csv = $scope.csv.concat($scope.DataUserLists);
                deferred.resolve($scope.csv);
                $scope.downloadingCSV = false;
            })
            $uibModalInstance.close();
            return deferred.promise;
        };   


        $scope.order = ['b', 'a', 'c'];
        $scope.testDelim = [{ a: "sample@cloudswyft.com", b: "Password1!", c: "No special character or symbol", d: "No special character or symbol" }, { a: "sample@cloudswyft.com", b: "Password1!", c: "No special character or symbol", d: "No special character or symbol" }];
    }
          
        

})();