
(function () {

    "use strict";

    angular.module("app-register", ["main-app", "ngRoute"])

        .config(config);
    config.$inject = ['$routeProvider'];

    function config($routeProvider) {

            $routeProvider.when("/", {
                controller: "RegisterController",
                templateUrl: "/app/RegisterPage/RegisterView.html"
            });
            $routeProvider.otherwise({ redirectTo: "/" });
        }
})();