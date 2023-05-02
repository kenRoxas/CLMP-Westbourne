
(function () {

    "use strict";

    angular.module("app-configuration", ["main-app", "ngRoute", "ui.bootstrap", "ngPatternRestrict"])
        .config(config);
    config.$inject = ['$routeProvider'];
    function config($routeProvider) {

        $routeProvider.when("/", {
            controller: "ConfigurationController",
            templateUrl: "/app/Configuration/Main/ConfigurationView.html"
        });

        $routeProvider.otherwise({ redirectTo: "/" });
    }
})();