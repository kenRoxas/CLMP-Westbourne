(function () {

    "use strict";

    angular.module("app-register")
        .controller("RegisterController",RegisterController).directive('pwCheck', function () {
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

        }).directive('unique',unique);
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
                        svc.getEmailAddressExist(modelValue)
                            .then(
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


    RegisterController.$inject = ['$scope', '$http', '$window', 'svc', '$timeout'];


    function RegisterController($scope, $http, $window, svc, $timeout) {

        $scope.emailPattern = /^(([^<>()\[\]\\.,;:\s@\"]+(\.[^<>()\[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            $scope.passwordPattern = /^[\w\d\W]*$/;
            $scope.isLoading = false;
            $scope.submitForm = function () {
                var userModel = {
                    firstname: $scope.firstname,
                    lastname: $scope.lastname,
                    email: $scope.email,
                    password: $scope.password,
                    confirmpassword: $scope.confirmpassword1,
                    username: $scope.email
                };
                svc.gotoRegister(userModel)
                    .then(function (response) {
                        $scope.isLoading = true;
                        $scope.gotoLogin();
                    });
                $scope.isLoading = true;
            };

            $scope.gotoLogin = function () {
                $window.location.href = '/Account/Login';
        };
        var imageInfo = {};
        $scope.uploadFile = function (file, errFiles) {
            var fd = new FormData();
                               
            imageInfo = {
                type: "pdf",
                file: file
            };


            svc.uploadtestken(imageInfo).then(function (response) {
            });
            //$http({
            //    method: 'POST',
            //    url: 'http://localhost:56067/api/account/BulkUserCreation?strFilePath=dsadas',
            //    data: {
            //        file: imageInfo
            //    }
            //});
        };
    }

   
        
})();