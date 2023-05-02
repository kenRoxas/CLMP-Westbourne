(function () {

    "use strict";

    angular.module("app-Usermanagement", ["main-app", "ngRoute", "ui.bootstrap","ngCsv"])
    .config(config);
    config.$inject = ['$routeProvider'];
    function config($routeProvider) {
        $routeProvider.when("/", {
            controller: "UsermanagementController",            
            templateUrl: "/app/Usermanagement/Main/UsermanagementView.html"
        });
    }
})();
