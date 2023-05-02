(function (angular) {

    "use strict";

    angular.module("app-labprofiles", ["main-app", "ngRoute", "ui.bootstrap", "ngFileUpload", "dndLists", "slickCarousel", "textAngular", "infinite-scroll", "ui.bootstrap.datetimepicker"])
        .config(config);
    config.$inject = ['$routeProvider'];
    function config($routeProvider) {

        $routeProvider.when("/", {
            controller: "LabProfilesController",
            templateUrl: "/app/LabProfiles/Main/LabProfilesView.html"
        });

        $routeProvider.otherwise({ redirectTo: "/" });
    }
})(window.angular);