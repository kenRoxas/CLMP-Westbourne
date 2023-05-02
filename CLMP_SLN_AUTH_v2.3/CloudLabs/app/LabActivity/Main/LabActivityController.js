(function (angular) {
    'use strict';
    angular.module('app-labactivity')
        .controller('LabActivityController', LabActivityController);

    LabActivityController.$inject = ['$scope', 'svc', '$window', '$uibModal', '$route', '$document', '$rootScope'];

    function LabActivityController($scope, svc, $window, $uibModal, $route, $document, $rootScope) {
        $scope.userIsStaff = userIsStaff;
            var htmlRef = angular.element($document[0].html);
            $scope.savePage = function (entry) {
                $http.post(Data.root_path + 'api/v1/pages', entry).
                    success(function (data, status, headers, config) {
                    }).
                    error(function (data, status, headers, config) {
                    });
            };

            $scope.isUserSuperAdmin = userIsSuperAdmin;
          
            $scope.labacts = [];
            $scope.loader = true;
            $scope.empty = false;
            $scope.ariaHidden = 'true';
            var $ctrl = this;
            var query = "";
            var pageSize = 30;
            var pageNum = 1;
            var labacts = 0;
            var count = 1;

            $scope.loadPage = function () {
                svc.GetActivityLabList(pageSize, pageNum, labacts).
                    then(function (response) {
                        $scope.loader = false;
                        angular.copy(response.LabActivities, $scope.labacts);
                        if ($scope.labacts.length === 0 && $scope.loader === false) {
                            $scope.empty = true;
                        }
                    });
                $("html").removeAttr('style');
            };

            $scope.loadPage();
            
            $scope.loadMore = function () {
                pageSize = + 4;
                svc.GetActivityLabList(pageSize, pageNum, labacts).
                    then(function (response) {
                        angular.copy(response.LabActivities, $scope.labacts);
                    });
            };

            $rootScope.$on("pageLoading", function () {
                $scope.labacts = [];
                $scope.loader = true;
                $scope.empty = false;
            });

            $rootScope.$on("loadPageLabActivity", function () {
                $scope.loadPage();
            });

            $scope.modalUpload = function (courseCode, labactivityid) {
                var modal = $uibModal.open({
                    animation: true,
                    templateUrl: '/app/LabActivity/Modal/uploadModal.html',
                    controller: 'uploadModalController',
                    size: 'sm',
                    backdrop: 'static',
                    keyboard: false,
                    resolve: {
                        courseCode: function () {
                            return courseCode;
                        },
                        labactivityid: function () {
                            return labactivityid;
                        }
                    }
                });
                modal.result.then(function (result) {
                }, function () {
                    $scope.loader = true;
                    $scope.loadMore();
                    $scope.loadPage();
                });
            };
            
            $scope.openModal = function (x, y) {

                $("html").css({ "overflow": "hidden" });
                
                var value = 0;

                if (x === 'create') {
                    value = value;
                    var modal = $uibModal.open({

                        animation: true,
                        templateUrl: '/app/LabActivity/Modal/labActivityModal.html',
                        controller: 'labActivityModalController',
                        size: 'lg',
                        backdrop: 'static',
                        keyboard: false,
                        resolve: {
                            data: function () {
                                return {
                                    Name: function () {
                                        return "";
                                    },
                                    TasksHtml: function () {
                                        return "";
                                    },
                                    LabActivityID: function () {
                                        return "";
                                    },
                                    status: function () {
                                        return x;
                                    },
                                    courseCode: function () {
                                        return "";
                                    },
                                    LabAnswerKey: function () {
                                        return null;
                                    },
                                    LabAnswerKeyName: function () {
                                        return null;
                                    }
                                };
                            }
                        }

                    });
                    modal.result.then(function (result) {
                    }, function () {
                        $scope.loader = true; $scope.loadMore();
                        $scope.loadPage();
                    });
                }
                else if (x === 'edit' || x === 'view') {
                    
                    var index = $scope.labacts.indexOf(y);
                    var labActData = [];
                    $scope.loader = true;

                    svc.GetLabActivity(y.LabActivityID)
                        .then(function (response) {
                            angular.copy(response, labActData);
                            $scope.loader = false;
                            modal = $uibModal.open({
                                animation: true,
                                templateUrl: '/app/LabActivity/Modal/labActivityModal.html',
                                controller: 'labActivityModalController',
                                class: 'modal-lg',
                                size: 'lg',
                                backdrop: 'static',
                                keyboard: false,
                                resolve: {
                                    data: function () {
                                        return {
                                            Name: function () {
                                                return labActData.Name;
                                            },
                                            TasksHtml: function () {
                                                return labActData.TasksHtml;
                                            },
                                            LabActivityID: function () {
                                                return labActData.LabActivityID;
                                            },
                                            status: function () {
                                                return x;
                                            },
                                            courseCode: function () {
                                                return labActData.CourseCode;
                                            },
                                            LabAnswerKey: function () {
                                                return labActData.LabAnswerKey;
                                            },
                                            LabAnswerKeyName: function () {
                                                return labActData.LabAnswerKeyName;
                                            }

                                        };
                                    }


                                }
                            });
                            modal.result.then(function (result) {
                            }, function () {
                                $scope.loader = true;
                                $scope.loadMore();
                                $scope.loadPage();
                               
                            });
                        });

                    value = 1;

                   
                }
                else if (x === 'delete') {
                    
                    modal = $uibModal.open({
                        animation: true,
                        templateUrl: '/app/LabActivity/Modal/confirmationModal.html',
                        controller: 'confirmationModalController',
                        controllerAs: '$confmodal',
                        size: 'md',
                        backdrop: 'static',
                        keyboard: false,
                        resolve: {
                            items: function () {
                                return {

                                    message: function () {
                                        return "Are you sure you want to delete " + y.Name + "?";
                                    },
                                    title: function () {
                                        return "Delete Lab Activity";
                                    },
                                    labActID: function () {
                                        return y.LabActivityID;
                                    },
                                    status: function () {
                                        return x;
                                    },
                                    labActivity: function () {
                                        return "";
                                    }
                                };
                            }


                        }
                    });
                    modal.result.then(function (result) {
                    }, function () {
                        $scope.loader = true;
                        $scope.loadMore();
                        $scope.loadPage();
                    });
                }

            };
                
            $scope.content = "<p> this is custom directive </p>";
            $scope.content_two = "<p> this is ng-ckeditor directive </p>";
        }
        
})(window.angular);