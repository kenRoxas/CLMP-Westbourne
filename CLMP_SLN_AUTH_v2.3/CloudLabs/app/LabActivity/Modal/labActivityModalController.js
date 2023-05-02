(function (angular) {
    'use strict';
    angular.module('app-labactivity')
        .controller('labActivityModalController', labActivityModalController).directive('unique', unique);

    unique.$inject = ['$q', '$timeout', 'svc'];
    function unique($q, $timeout, svc) {
        return {
            restrict: "A",
            scope: {},
            replace: false,
            link: function (scope, element, attrs, ctrl) {
                element.bind('keypress', function (event) {
                    var regex = new RegExp('(^[a-zA-Z0-9.-]$)');
                    //var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
                    if (!regex.test(event.key, "g")) {
                        event.preventDefault();
                        return false;
                    }
                });
            }
        };
    }   
    labActivityModalController.$inject = ['$scope', '$uibModalInstance', '$http', '$uibModal', 'svc', 'data', '$window', '$sce'];
    function labActivityModalController($scope, $uibModalInstance, $http, $uibModal, svc, data, $window, $sce) {
            $window.addEventListener("dragover", function (e) {
                e = e || event;
                e.preventDefault();
            }, false);
            $window.addEventListener("drop", function (e) {
                e = e || event;
                e.preventDefault();
            }, false);
            var id = data.LabActivityID();
        $scope.status = data.status();
        $scope.courseCode = data.courseCode();
        $scope.LabAnswerKey = data.LabAnswerKey();
        $scope.LabAnswerKeyName = data.LabAnswerKeyName();
            $scope.activityName = data.Name();
            $scope.tinymceModel = data.TasksHtml();
            var files = [];
            var imageFileName = [];
            $scope.tinymceOptions = {
                plugins: 'link image code imagetools lists advlist table nonbreaking powerpaste',
                advlist_bullet_styles: 'circle,disc,square',
                object_resizing: false,
                force_p_newlines: false,
                force_br_newlines: true,
                resize: false,
                powerpaste_word_import: 'merge',
                forced_root_block: '',
                height: '500px',
                convert_newlines_to_brs: false,
                remove_linebreaks: true,
                nonbreaking_force_tab: true,
                toolbar: 'undo redo | formatselect bold italic fontsizeselect | alignjustify alignleft aligncenter alignright  | bullist numlist | code image | indent outdent',
                content_style: 'img{max-width:50%;max-height:300vh;}',
                imagetools_toolbar: "rotateleft rotateright | flipv fliph | imageoptions",
                file_picker_callback: function (cb, value, meta) {
                    var input = document.createElement('input');
                    input.setAttribute('type', 'file');
                    input.setAttribute('accept', 'image/png');
                    if (meta.filetype === 'image') {

                        input.onchange = function ()
                        {
                            
                            var file = this.files[0];
                            var fileExtension = file.name.replace(/^.*\./, '');
                            if (fileExtension === 'png' || fileExtension === 'PNG') {
                                if (file.size < 3000000) {
                                    
                                    var reader = new FileReader();
                                    reader.readAsDataURL(file);
                                    reader.onload = function () {

                                        var id = 'blobid' + (new Date()).getTime();
                                        
                                        imageFileName.push(file.name.replace("." + fileExtension, "-" + (new Date()).getTime() + Math.floor(Math.random() * 11) + "." + fileExtension));

                                        files.push(file);
                                        
                                        var blobCache = tinymce.activeEditor.editorUpload.blobCache;
                                        var blobFile = blobStorage + imageFileName[imageFileName.length - 1];
                                        var blobInfo = blobCache.create(id, file, blobFile);
                                        blobCache.add(blobInfo);
                                        cb(blobInfo.blobUri(), file.name );
                                      
                                    };
                                }
                                else {  
                                    
                                    $("#mce-modal-block").removeAttr('style');
                                    $('#mce-modal-block').css('display', 'none');
                                    $(".mce-container.mce-panel.mce-floatpanel.mce-window.mce-in").css({ "opacity": "0" });
                                    $(".mce-container.mce-panel.mce-floatpanel.mce-window.mce-in").css({ "z-index": "0" });
                                    $(".mce-reset.mce-fade.mce-in").css({"z-index":"0"});
                                    $(".mce-widget.mce-tooltip.mce-tooltip-n").css({"z-index":"0"});
                                    (function (angular) {
                                        var modal = $uibModal.open({
                                            animation: true,
                                            templateUrl: '/app/LabActivity/Modal/confirmationModal.html',
                                            controller: 'confirmationModalController',
                                            controllerAs: '$confmodal',
                                            size: 'sm',
                                            backdrop: 'static',
                                            keyboard: false,
                                            resolve: {
                                                items: function () {
                                                    return {

                                                        message: function () {
                                                            return "File is too large!";
                                                        },
                                                        title: function () {
                                                            return "Error";
                                                        },
                                                        labActivity: function () {
                                                            return "";
                                                        },
                                                        labActID: function () {
                                                            return "";
                                                        },
                                                        status: function () {
                                                            return "size";
                                                        }
                                                    };
                                                }


                                            }
                                        });
                                    }(window.angular));
                                }
                               
                            }
                        };
                    }
                    input.click();
                },  
               // automatic_uploads: true,
                //images_upload_url: apiUrl + 'LabActivities/UploadThumbnailLabActivities',
                image_dimensions: false,
                image_class_list: [
                    { title: 'Responsive', value: 'img-responsive' }
                ]

                
                
            };
          
         

            (function (angular) {


                $scope.getContent = function (name, task, courseCode, LabAnswerKey, LabAnswerKeyName) {
                   
                    var s = JSON.stringify(imageFileName);

                    $scope.index = 0;
                    if (files.length > 0) {
                        svc.uploadImage1(files, s ).then(
                            function (response) {
                            }
                        );
                    }
                    //var newstr = $scope.tinymceModel.replace(/<[a-zA-Z0-9/]*>/g, "");
                    var newstr1 = $scope.tinymceModel.replace(/data:image\/png;+base64,/gi, "");
                    
                    var labActValue = {
                        LabActivityID: id,
                        Name: name,
                        Tasks: newstr1,
                        TasksHtml: newstr1,
                        CourseCode: courseCode,
                        LabAnswerKey: LabAnswerKey,
                        LabAnswerKeyName: LabAnswerKeyName
                    };

                    if ($scope.status === 'create') {
                        var modal = $uibModal.open({
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
                                            return "Do you wish to create the lab activity?";
                                        },
                                        title: function () {
                                            return "Create Lab Activity";
                                        },
                                        labActivity: function () {
                                            return labActValue;
                                        },
                                        labActID: function () {
                                            return id;
                                        },
                                        status: function () {
                                            return "create";
                                        }
                                    };
                                }


                            }
                        });
                    }
                    else if ($scope.status === 'edit') {
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
                                            return "Do you wish to save changes?";
                                        },
                                        title: function () {
                                            return "Edit Lab Activity";
                                        },
                                        labActivity: function () {
                                            return labActValue;
                                        },
                                        labActID: function () {
                                            return id;
                                        },
                                        status: function () {
                                            return "edit";
                                        }
                                    };
                                }


                            }
                        });
                    }
                };
                $scope.close = function () {
                    $("html").removeAttr('style');

                    $uibModalInstance.close();
                    $scope.index = 0;

                };
                $scope.cancel = function () {
                    $("html").removeAttr('style');
                    $scope.status = true;
                    $uibModalInstance.close();
                    $scope.index = 0;

                };


            }(window.angular));

        }
})(window.angular);