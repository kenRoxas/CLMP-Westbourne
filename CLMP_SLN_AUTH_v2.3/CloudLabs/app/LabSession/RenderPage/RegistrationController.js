(function (angular) {
    'use strict';
    angular.module('app-labsession')
        .controller('RegistrationController', RegistrationController).directive('unique', unique).directive('pwCheck', function () {
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

    RegistrationController.$inject = ['$scope', '$uibModalInstance', '$rootScope', '$route', '$uibModalStack', 'svc', '$uibModal'];

    function RegistrationController($scope, $uibModalInstance, $rootScope, $route, $uibModalStack, svc, imageInfo, $uibModal) {
        $scope.emailPattern = /^(([^<>()\[\]\\.,;:\s@\"]+(\.[^<>()\[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

        $scope.email = currentEmail;
        $scope.loading = false;
        $scope.glyph = false;
        $scope.glyphPass = false;

        $scope.showOrHidePass = 'password';
        $scope.showOrHideConfirmPass = 'password';

        $scope.regions = ['Australia East', 'Brazil South', 'East Asia' , 'South Africa North', 'South Africa West', 'Southeast Asia', 'UK South', 'UK West'];

        if (currentEmail.includes("temporary"))
            $scope.registrationLayout = true;
        else
            $scope.registrationLayout = false;

        $scope.register = function (firstname, lastname, email, password) {
            var user = {
                "Firstname": firstname,
                "Lastname": lastname,
                "Email": email,
                "Id": currentId,
                "Password": password,
                "UserIdLTI": currentUserIdLTI
            };

            $scope.loading = true;

            if ($scope.registrationLayout) {
                svc.editUserLTI(user)
                    .then(function (response) {
                        location.reload();
                    });
            }

        };

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
    }
})(window.angular);