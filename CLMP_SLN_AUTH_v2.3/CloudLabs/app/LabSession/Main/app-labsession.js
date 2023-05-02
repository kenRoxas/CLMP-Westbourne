(function (angular) {

    "use strict";
   
    angular.module("app-labsession", ["main-app", "ngRoute", "ui.bootstrap", "textAngular", "angular-screenshot", "ngFileUpload", "ngclipboard"] )
        .config(config);

    config.$inject = ['$routeProvider'];

    function config($routeProvider) {

            $routeProvider.when("/", {
                controller: "LabSessionController",
                templateUrl: "/app/LabSession/Main/LabSessionView.html"
            });

            $routeProvider.otherwise({ redirectTo: "/" });
        }
})(window.angular);