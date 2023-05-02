using System.Web;
using System.Web.Optimization;

namespace CloudSwyft.CloudLabs
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                          "~/Scripts/angular/angular.min.js"
                        , "~/Scripts/angular/angular-slick.min.js"
                        , "~/Scripts/angular-route/angular-route.min.js"
                        , "~/Scripts/angular-ui/ui-bootstrap.min.js"
                        , "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js"
                        , "~/Scripts/ng-infinite-scroll/ng-infinite-scroll.min.js"
                        , "~/Scripts/ng-file-upload/ng-file-upload-shim.min.js"
                        , "~/Scripts/ng-file-upload/ng-file-upload.min.js"
                        , "~/Scripts/angular-animate/angular-animate.js"
                        , "~/Scripts/angular-screenshot/angular-screenshot.min.js"
                        , "~/Scripts/alasql/alasql.min.js"
                        , "~/Scripts/alasql/xlsx.core.min.js"
                        , "~/Scripts/shared/site.js"
                        , "~/Scripts/shared/moment.js"
                        , "~/Scripts/angular-sanitize/textAngular-sanitize.min.js"
                        , "~/Scripts/other/ng-csv.min.js"
                        , "~/Scripts/other/clipboard.js"
                        , "~/Scripts/other/ngclipboard.min.js"
                        , "~/Scripts/other/ng-pattern-restrict.min.js"
                        , "~/Scripts/other/Chart.min.js"
                        , "~/Scripts/other/html2canvas.min.js"
                        , "~/Scripts/other/DateTimePicker.js"
                        //, "~/Scripts/tinymce/tinymce/tinymce.js"
                        //, "~/Scripts/tinymce/tinymce/themes/inlite/themes.js"
                        //, "~/Scripts/tinymce/tinymce/plugins/powerpaste/plugin.js"
                        //, "~/Scripts/tinymce/angular-ui-tinymce/src/tinymce.js"
                        , "~/Scripts/textAngular-sanitize/textAngular-rangy.min.js"
                        , "~/Scripts/textAngular-sanitize/textAngular-sanitize.min.js"
                        , "~/Scripts/textAngular-sanitize/textAngular.min.js"
                        , "~/Scripts/angular-draganddrop/angular-drag-and-drop-lists.min.js"
                        , "~/Scripts/slick/slick.min.js"
                        , "~/Scripts/textAngular/textAngular-rangy.min.js"
                        , "~/Scripts/textAngular/textAngular.min.js"
                        , "~/Scripts/bootstrap/bootstrap-filestyle.js"));

            //bundles.Add(new ScriptBundle("~/bundles/alasql").IncludeDirectory("~/Scripts/alasql", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/angular").IncludeDirectory("~/Scripts/angular", "*.js"));

            //bundles.Add(new ScriptBundle("~/bundles/animate").IncludeDirectory("~/Scripts/angular-animate", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/aria").IncludeDirectory("~/Scripts/angular-aria", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/toggle").IncludeDirectory("~/Scripts/angular-bootstrap-toggle", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/cookies").IncludeDirectory("~/Scripts/angular-cookies", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/draganddrop").IncludeDirectory("~/Scripts/angular-draganddrop", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/material").IncludeDirectory("~/Scripts/angular-material", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/messages").IncludeDirectory("~/Scripts/angular-messages", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/route").IncludeDirectory("~/Scripts/angular-route", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/sanitize").IncludeDirectory("~/Scripts/angular-sanitize", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/screenshot").IncludeDirectory("~/Scripts/angular-screenshot", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/ui").IncludeDirectory("~/Scripts/angular-ui", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").IncludeDirectory("~/Scripts/bootstrap", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/cldr").IncludeDirectory("~/Scripts/cldr", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/globalize").IncludeDirectory("~/Scripts/globalize", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery/jquery-{version}.js", "~/Scripts/jquery/jquery-ui.min.js", "~/Scripts/jquery/jquery.cookie.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include("~/Scripts/jquery/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").IncludeDirectory("~/Scripts/modernizr", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/tinymce").IncludeDirectory("~/Scripts/tinymce", "*.js"));

            //bundles.Add(new ScriptBundle("~/bundles/upload").IncludeDirectory("~/Scripts/ng-file-upload", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/scroll").IncludeDirectory("~/Scripts/ng-infinite-scroll", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/other").IncludeDirectory("~/Scripts/other", "*.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/shared").IncludeDirectory("~/Scripts/shared", "*.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/slick").IncludeDirectory("~/Scripts/slick", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/uibootstrap").IncludeDirectory("~/Scripts/ui-bootstrap", "*.min.js"));


            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/site.less",
                      "~/Content/css/login.less"));

            //bundles.Add(new ScriptBundle("~/app/layoutjs").Include(
            //       "~/app/Common/PasswordChange.js"
            //      ,"~/app/Common/ChangeGroup.js"));

            bundles.Add(new ScriptBundle("~/app/dash").Include(
                "~/app/Dashboard/Main/app-dashboard.js",
                "~/app/Dashboard/Main/DashboardController.js"));
            
            bundles.Add(new ScriptBundle("~/app/labprof").Include(
                "~/app/LabProfiles/Main/app-labprofiles.js",
                "~/app/LabProfiles/Main/LabProfilesController.js",
                "~/app/LabProfiles/Modal/CreateModalController.js",
                "~/app/Common/ConfirmationModalController.js",
                "~/app/LabProfiles/Modal/ActivityViewController.js",
                "~/app/LabProfiles/Modal/GrantLabAccessController.js",
                "~/app/LabProfiles/Modal/LabHoursController.js",
                "~/app/LabProfiles/Modal/GrantConfirmationModalController.js",
                "~/app/LabProfiles/Modal/GrantAdditionalModalController.js",
                "~/app/LabProfiles/Modal/CourseGradeModalController.js",
                "~/app/LabProfiles/Modal/ImageGradeController.js",
                "~/app/LabProfiles/Modal/ViewImageController.js",
                "~/app/LabProfiles/Modal/RemoteLabAccessController.js",
                "~/app/LabProfiles/Modal/LabHoursExtensionController.js"));

            bundles.Add(new ScriptBundle("~/app/config").Include(
                    "~/app/Configuration/Main/app-configuration.js",
                    "~/app/Configuration/Main/ConfigurationController.js",
                    "~/app/Configuration/Modal/configurationModalController.js",
                    "~/app/Configuration/Modal/confirmationModalController.js"));

            bundles.Add(new ScriptBundle("~/app/labact").Include(
                    "~/app/LabActivity/Main/app-labactivity.js",
                    "~/app/LabActivity/Main/LabActivityController.js",
                    "~/app/LabActivity/Modal/labActivityModalController.js",
                    "~/app/LabActivity/Modal/confirmationModalController.js",
                    "~/app/LabActivity/Modal/uploadModalController.js"));

            bundles.Add(new ScriptBundle("~/app/labsess").Include(
                    "~/app/LabSession/Main/app-labsession.js",
                    "~/app/LabSession/Main/LabsessionController.js",
                    "~/app/LabSession/RenderPage/RenderPageController.js",
                    "~/app/LabSession/RenderPage/ViewRenderImageController.js",
                    "~/app/LabSession/RenderPage/UserNotificationController.js",
                    "~/app/LabSession/RenderPage/ConfirmationController.js",
                    "~/app/LabSession/RenderPage/RequestMachineController.js",
                    "~/app/LabSession/RenderPage/RegistrationController.js",
                    "~/app/LabSession/RenderPage/WhiteListController.js"));

            bundles.Add(new ScriptBundle("~/app/user").Include(
                    "~/app/Usermanagement/Main/app-usermanagement.js",
                    "~/app/Usermanagement/Main/UsermanagementController.js",
                    "~/app/Usermanagement/Modal/createUserController.js",
                    "~/app/Usermanagement/Modal/alertModalController.js"));

            bundles.Add(new StyleBundle("~/Content/appconf").Include("~/Content/css/configuration.css", "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/appdash").Include("~/Content/css/dashboard.css", "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/appacti").Include("~/Content/css/labAct.css", "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/appprof").Include("~/Content/css/labprofiles.css", "~/Content/css/render.css", "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/appsess").Include("~/Content/css/labsession.css", "~/Content/css/render.css", "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/appnoti").Include("~/Content/css/notification.css", "~/Content/css/site.css"));
            bundles.Add(new StyleBundle("~/Content/appuser").Include("~/Content/css/usermanagement.css", "~/Content/css/site.css"));

            bundles.Add(new StyleBundle("~/Content/global").Include(
                        "~/Content/css/site.css",
                        "~/Content/css/login.css"));

            bundles.Add(new StyleBundle("~/Content/scriptscss").IncludeDirectory("~/Content/scripts_css", "*.css"));

            BundleTable.EnableOptimizations = true;




        }
    }
}
