
(function () {

    "use strict";

    angular.module("app-dashboard", ["main-app", "ngRoute", "ui.bootstrap", "ngCsv", "ngclipboard"])
        .config(config);

    config.$inject = ['$routeProvider'];

    function config($routeProvider) {

        $routeProvider.when("/", {
            controller: "DashboardController",
            templateUrl: "/app/Dashboard/Main/DashboardView.html"
        });
        $routeProvider.otherwise({ redirectTo: "/" });
    }
})();


