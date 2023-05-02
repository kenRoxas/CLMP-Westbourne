
(function (angular) {
    'use strict';
    angular.module('app-labsession')
        .controller('RenderPageController', RenderPageController).factory('Timer', Timer);

    Timer.$inject = ['$interval'];
    function Timer($interval) {
        return function (delay) {
            var initialMs = (new Date()).getTime();
            var result = { totalMilliseconds: 0, counts: 0 };
            $interval(function () {
                result.totalMilliseconds = (new Date()).getTime() - initialMs;
                result.counts++;
            }, delay);
            return result;
        };
    }

    RenderPageController.$inject = ['$scope', '$rootScope', '$injector', '$location', 'svc', 'item', '$sce', '$uibModalInstance', '$uibModal', 'content', 'userId', 'profileId', '$anchorScroll', '$timeout', '$document', 'Timer', '$window', 'hoursRemaining', '$route', '$uibModalStack', 'labIds', 'Upload', 'coursecode', '$interval', 'coursename', 'resourceId'];

    function RenderPageController($scope, $rootScope, $injector, $location, svc, item, $sce, $uibModalInstance, $uibModal, content, userId, profileId, $anchorScroll, $timeout, $document, Timer, $window, hoursRemaining, $route, $uibModalStack, labIds, Upload, coursecode, $interval, coursename, resourceId) {
        $scope.imgPreview = true;
        $scope.showUpload = false;
        $scope.imgLarge = false;
        $scope.showRemove = false;
        $scope.isActive = false;
        $scope.showError = false;
        $scope.hasImage = false;
        $scope.notAllowedBack = true;
        $scope.notAllowedNext = true;
        $scope.btnBackRender = 'nav-previous-disabled';
        $scope.btnNextRender = 'nav-next-disabled';
        $scope.isBasicOpen = false;
        $scope.isValid = true;
        $scope.uploadActive = false;
        $scope.courseName = coursename;
        $scope.isRenderFocus = true;
        $scope.isFullScreen = false;
       // $scope.remainingHours = 0;
       // $scope.remainingMins = 0;
        var hrs = 0;
        $scope.isLabClose = function () {
            $scope.showLabActivities = !$scope.showLabactivities;
            $scope.isRenderFocus = false;
            $scope.showRemove = false;
            $scope.isActive = false;
            $scope.showUpload = false;
            $scope.imgPreview = false;
            $scope.picFile = null;
            $scope.f = null;
            $scope.hasImage = false;
        };

        $scope.update = function () {
            svc.getLabHours(resourceId)
            .then(function (response) {
                hrs = response;
                var d = Number(hrs);
                $scope.remainingHours = Math.floor(d / 3600);
                $scope.remainingMins = Math.floor(d % 3600 / 60);

                if ($scope.remainingHours <= 0)
                    $scope.remainingHours = 0;
                if ($scope.remainingMins <= 0)
                    $scope.remainingMins = 0;

                var s = Math.floor(d % 3600 % 60);
            });
          
        }
        $scope.update();
        $interval($scope.update, 30000);

        var self = this;
        self.advanceApi;
        self.cancel = cancel;
        self.download = download;
        function cancel() {
            if (self.advanceApi) self.advanceApi.cancel();
        }
        function download() {
            if (self.advanceApi) self.advanceApi.download();
        }

        //$scope.checkValidity = function (file, fillle) {
        //    $scope.showRemove = false;
        //    $scope.showUpload = false;
        //    if (file > 3145728) {
        //        $scope.imgPreview = false;
        //        $scope.imgLarge = true;
        //    }
        //    else if (file === undefined)
        //        $scope.imgLarge = true;

        //    else {
        //        $scope.imgPreview = true;
        //        $scope.imgLarge = false;
        //        $scope.showRemove = true;
        //        $scope.showUpload = true;
        //    }
        //};

        $scope.isRemove = function () {
            $scope.isRenderFocus = false;
            $scope.uploadActive = false;
            $scope.isValid = true;
            $scope.isActive = true;
            $scope.showRemove = false;
            $scope.showUpload = false;
            $scope.imgPreview = false;
            $scope.picFile = null;
            $scope.f = null;
            $scope.hasImage = false;
        };

        $rootScope.$on('isRemove', function () {
            $scope.isRenderFocus = false;
            $scope.uploadActive = false;
            $scope.isValid = true;
            $scope.isActive = true;
            $scope.showRemove = false;
            $scope.showUpload = false;
            $scope.imgPreview = false;
            $scope.picFile = null;
            $scope.f = null;
            $scope.hasImage = false;
        });

        var imageInfo;
        $scope.uploadButton = function () {
            $scope.isRenderFocus = false;
            $uibModal.open({
                templateUrl: '/app/LabSession/RenderPage/UserNotificationView.html',
                controller: "UserNotificationController",
                size: 'xs',
                backdrop: 'static',
                keyboard: false,
                windowClass: 'notification-modal',
                resolve: {
                    imageInfo: function () {
                        return imageInfo;
                    },
                    message: function () {
                        return "Are you sure you want to upload?";
                    },
                    type: function () {
                        return null;
                    },
                    UserId: function () {
                        return null;
                    },
                    ProfileId: function () {
                        return null;
                    },
                    resourceId: function () {
                        return resourceId;
                    }
                }
            });
        };

        $scope.uploadFiles = function (file, errFiles) {
            $scope.isRenderFocus = false;
            if (file !== null) {
                $scope.imgPreview = true;
                $scope.showUpload = false;
                $scope.uploadActive = true;

                $scope.showRemove = false;
                $scope.showError = false;
                $scope.hasImage = true;
                $scope.isValid = true;
                $scope.f = file;
                $scope.errFile = errFiles && errFiles[0];
                if (file.type === "image/jpg" || file.type === "image/jpeg" || file.type === "image/png" || file.type === "image/JPG") {
                    //var imageFilename = name.split(' ').join('') + '-' + userId + '-' + profileId + '-' + Date.now();
                    var imageFilename = coursecode + "_" + profileId + "_" + "labActivity" + parseInt($scope.index + 1);
                    imageInfo = {
                        type: "image",
                        file: file,
                        imageFilename: imageFilename
                    };

                    //var imageFilename = userId + '-' + profileId + '-' + Date.now();
                    $scope.showRemove = true;
                    $scope.showUpload = true;
                }
                else {
                    $scope.isValid = false;

                    $scope.uploadActive = true;
                    //$scope.showError = true;
                    //$scope.showUpload = false;
                    $scope.errorMsg = "Please select .jpg or .png file to upload";
                }
            }
            else {
                $scope.isValid = false;

                $scope.uploadActive = true;
                $scope.errorMsg = "Please select .jpg or .png file to upload";
            }
        };

        $scope.isRemove2 = function () {
            $scope.isRenderFocus = false;
            $scope.uploadActive = false;
            $scope.isValid = true;
            $scope.isActive = true;
            $scope.showRemove = false;
            $scope.showUpload = false;
            $scope.imgPreview = false;
            $scope.picFile = "";
            $scope.f = "";
            $scope.hasImage = false;

        };
        $scope.isLoading = true;
        $scope.showLabActivities = true;
        $scope.isRenderFocus = false;
        $scope.index = 0;
        $scope.tasks = [];
        $scope.copytask = [];
        $scope.test = [];
        $scope.hoursRemaining = hoursRemaining;
        $scope.frameIsOpen = true;
        $scope.stopLog = false;
        $scope.noCredits = false;
        $scope.inactive = false;
        $scope.elapsed = Timer(1000);

        var fifteennotif = false;
        var zeronotif = false;
        var rex = /src\=[\"\']?([a-zA-Z0-9 \:\-\#\(\)\.\_\/\;\'\,]+)\;?[\"\']?/ig;
        var arr = [];
        var htmlRef = angular.element($document[0].html);

        //svc.getLabSchedule(profileId, userId)
        //    .then(function (response) {
        //        $scope.hoursRemaining = response.LabHoursRemaining;
        //    });

        //$scope.logTime = function () {
        //    if ($scope.frameIsOpen === true && $scope.stopLog === false) {
        //        svc.logTime(profileId, userId, false)
        //            .then(function (response) {
        //                hoursRemaining = response;
        //            });
        //    }
        //    $timeout(function () {
        //        $scope.logTime();
        //    }, 60000);
        //};

        $timeout(function () {
            angular.element('#iRender').focus();
        }, 5000);

        angular.forEach(content[0], function (key, value) {
            $scope.tasks.push({ "TaskDetails": key["TasksHtml"] });
            if ($scope.tasks.length > 1) {
                $scope.notAllowedNext = false;
                $scope.btnNextRender = 'nav-next';
            }
        });

        angular.forEach($scope.tasks, function (key, value) {
            arr = [];
            var rex = /src\=[\"\']?([a-zA-Z0-9 \:\-\#\(\)\.\_\/\;\'\,]+)\;?[\"\']?/ig;
            while ((arr = rex.exec($scope.tasks[value]["TaskDetails"])) !== null) {
                if (($scope.tasks[value]["TaskDetails"]).indexOf("src") >= 0) {
                    var imgr = ($scope.tasks[value]["TaskDetails"]).split(" ");
                    angular.forEach(imgr, function (key1, value1) {
                        if (imgr[value1].indexOf("src") >= 0) {
                            var blobSrc = key1.split('=');
                            imgr[value1] += " onclick=\"viewImage('" + blobSrc[1].replace(/"/g, "") + "')\"";
                        }
                    });
                    $scope.tasks[value]["TaskDetails"] = imgr.join(" ");
                    if ($scope.tasks.length > 1) {
                        $scope.notAllowedNext = false;
                        $scope.btnNextRender = 'nav-next';
                    }
                    break;
                }
            }
        });


        $scope.shutdownVM = function () {
            if ($scope.stopLog === false) {
                //svc.logTime(profileId, userId, false)
                //    .then(function (response) {
                //    });
            }

            if ($scope.inactive === false) {
                svc.machineLogs(resourceId, "", "No Hours").then(function (response) {f

                });
                svc.vmOperation(resourceId, "Stop", "Student")
                    .then(function (response) {
                        //$route.reload();
                    });
                //svc.shutdownVm(1, profileId, userId)
                //    .then(function (response) {
                //    });
                $scope.frameIsOpen = false;
            }
        };

        $scope.viewImage = function (src) {
            $uibModal.open({
                animation: true,
                templateUrl: '/app/LabSession/RenderPage/ViewRenderImageView.html',
                controller: 'ViewRenderImageController',
                size: 'xl',
                backdrop: 'static',
                windowClass: 'image-view',
                keyboard: false,
                resolve: {
                    source: function () {
                        return src;
                    }
                }
            });
        };

        $scope.capture = function () {
            $scope.isBasicOpen = !$scope.isBasicOpen;
            var iRender = document.getElementById("iRender");
            var renderFinal = iRender.contentDocument.documentElement;
            html2canvas(renderFinal).then(function (canvas) {
                // Export the canvas to its data URI representation
                var base64image = canvas.toDataURL("image/png");
            });

            //shutter shot
            $('#shutter').addClass('on');
            setTimeout(function () {
                $('#shutter').removeClass('on');
            }, 500);/* Shutter speed (double & add 45) */
        };

        //check if the machine has credits
        //$scope.checkLabCredits = function () {
        //    var message;
        //    var messagetype;
        //    $scope.isRenderFocus = false;
        //    if ($scope.frameIsOpen) {
        //        if (((hoursRemaining <= 15)) && fifteennotif === false) {
        //            if (hoursRemaining === 1)
        //                message = "You only have " + hoursRemaining + " minute left. Please save your work";
        //            else
        //                message = "You only have " + hoursRemaining + " minutes left. Please save your work";
        //            messagetype = "fifteen";
        //            fifteennotif = true;
        //            $scope.userNotification(message, messagetype);
        //        }
        //        else if ((hoursRemaining <= 0) && zeronotif === false && $scope.inactive === false) {
        //            $scope.shutdownVM();
        //            $scope.stopLog = true;
        //            $scope.noCredits = true;
        //            message = "Your lab credits have ran out. Session is now closing.";
        //            messagetype = "nohours";
        //            zeronotif = true;
        //            $scope.userNotification(message, messagetype);
        //        }

        //    }
        //    else if (hoursRemaining >= $scope.elapsed) {
        //        message = "Your lab credits have ran out. Session is now closing.";
        //        messagetype = "close";
        //        $scope.userNotification(message, messagetype);
        //    }
        //    $timeout(function () {
        //        $scope.checkLabCredits();
        //    }, 55000);
        //};

        $scope.userNotification = function (message, messagetype) {
            $("html").css({ "overflow": "hidden" });
            htmlRef.addClass('ovh');
            $uibModal.open({
                templateUrl: '/app/LabSession/RenderPage/UserNotificationView.html',
                controller: "UserNotificationController",
                size: 'md',
                backdrop: 'static',
                keyboard: false,
                windowClass: 'notification-modal',
                resolve: {
                    message: function () {
                        return message;
                    },
                    type: function () {
                        return messagetype;
                    },
                    imageInfo: function () {
                        return null;
                    },
                    UserId: function () {
                        return userId;
                    },
                    ProfileId: function () {
                        return profileId;
                    },
                    resourceId: function () {
                        return resourceId;
                    }
                }
            });
        };

        $scope.focusMove = function () {
            angular.element('#iRender').focus();
            $scope.isRenderFocus = true;
        };

        $scope.notRenderFocus = function ($event) {
            var keyCode = $event.which || $event.keyCode;
            if (keyCode === 9) {
                $scope.isRenderFocus = false;
            }

        }

        $scope.removeFocus = function () {
            $scope.isRenderFocus = false;
        }


        $scope.checkKeyPressed = function () {

            return KeyboardEvent.call(44);

        };

        $scope.guacamoleUrl = $sce.trustAsResourceUrl(item);

        $scope.clickedLab = function () {
            $scope.isRenderFocus = false;
            $scope.showLabActivities = !$scope.showLabActivities;
            $scope.isActive = true;
            $scope.labHeaderLeft = 'active-header-left';
            if ($scope.labHeaderRight === 'active-header-right') {
                $scope.labHeaderLeft = '';
            }
        };

        $scope.requestFullScreen = function () {
            $scope.isRenderFocus = false;
            $scope.element = document.body;
            $scope.requestMethod = $scope.element.requestFullscreen || $scope.element.webkitRequestFullscreen || $scope.element.mozRequestFullscreen || $scope.element.msRequestFullscreen;
            if ($scope.isFullScreen == true) {
                document.exitFullscreen();
                $scope.isFullScreen = false;
            } else if ($scope.requestMethod) { // Native full screen.
                $scope.requestMethod.call($scope.element);
                $scope.isFullScreen = true;
            } else if (typeof window.ActiveXObject !== "undefined") { // Older IE.
                var wscript = new ActiveXObject("WScript.Shell");
                if (wscript !== null) {
                    wscript.SendKeys("{F11}");
                }
            }
        }

        $scope.addLabAct = function () {
            $scope.isRenderFocus = false;
            if (!$scope.isActive) {
                $scope.isActive = true;
                $('#shutter').addClass('on');
                $('#shutter').removeClass('off');
            } else {
                $scope.isActive = false;
                $('#shutter').addClass('off');
                $('#shutter').removeClass('on');
            }

            var uploadBtn = document.querySelector(".upload-screenshot-btn");
            var element = document.querySelector(".labAct-container");
            uploadBtn.classList.toggle('appear');
            element.classList.toggle('is-visible');
        }

        $scope.close = function () {
            var message = "Closing this will end your session and shutdown the machine. Do you wish to continue?";
            var messagetype = "shutdown";
            $scope.isRenderFocus = false;
            $scope.userNotification(message, messagetype);
            //$scope.tasks.length = 0;
            //content.length = 0;                
        };

        $rootScope.$on('upload-finish', function () {
            $scope.isRenderFocus = false;
            $scope.uploadActive = false;
            $scope.isValid = true;
            $scope.isActive = true;
            $scope.showRemove = false;
            $scope.showUpload = false;
            $scope.imgPreview = false;
            $scope.picFile = null;
            $scope.f = null;
            $scope.hasImage = false;
        });

        $scope.btnNext = function () {
            $scope.isRenderFocus = false;
            $scope.index = $scope.index + 1;
            if ($scope.index === $scope.tasks.length) {
                $scope.isActive = true;
                $scope.notAllowedNext = true;
                $scope.btnNextRender = 'nav-next-disabled';
            }
            else if ($scope.index > $scope.tasks.length) {
                $scope.isActive = false;
                $scope.notAllowedBack = false;
            }
            else {
                $scope.isActive = true;
                $scope.notAllowedBack = false;
                if ($scope.index + 1 === $scope.tasks.length) {
                    $scope.btnNextRender = 'nav-next-disabled';
                    $scope.notAllowedNext = true;
                }
                $scope.btnBackRender = 'nav-previous';

                var ele = angular.element(document.querySelector('#divID'));
                $scope.resetScroll(ele);
            }
        };

        $scope.btnPrev = function () {
            $scope.isRenderFocus = false;
            if ($scope.index + 1 > 0) {
                $scope.index = $scope.index - 1;
                $scope.notAllowedBack = false;
                $scope.notAllowedNext = false;
                $scope.isActive = true;
                $scope.btnNextRender = 'nav-next';
                if ($scope.index === 0) {
                    $scope.notAllowedBack = true;
                    $scope.btnBackRender = 'nav-previous-disabled';
                }
                var ele = angular.element(document.querySelector('#divID'));
                $scope.resetScroll(ele);
            }
            else {
                $scope.isActive = false;
                $scope.notAllowedBack = false;
            }
        };

        //$timeout(function () {
        //    //$scope.logTime();
        //    $scope.checkLabCredits();
        //}, 180000);

        $scope.resetScroll = function link(ele) {
            angular.element(ele)[0].scrollTop = 0;
        };

        $rootScope.$on('shutdown', function () {
            $scope.frameIsOpen = false;
            //$scope.shutdownVM();
            $rootScope.$emit('reload');
            $uibModalStack.dismissAll();
        });
    }
})(window.angular);
