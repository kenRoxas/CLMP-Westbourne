using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CloudSwyft.CloudLabs.Models;
using System.Collections.Generic;
using System.Net.Http;
using System;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Net.Http.Headers;
using Microsoft.Owin.Security;
using CloudSwyft.OAuthServer.Helpers;
using System.Globalization;
using System.Text;
using Saml;
using System.Diagnostics;
using System.Web.Security;
using Antlr.Runtime;
using System.Data.SqlClient;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace CloudSwyft.CloudLabs.Controllers
{
    public class AccountController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public string authServer = System.Configuration.ConfigurationManager.AppSettings["CloudSwyftAuthServerUrl"];
        public string gcpServer = System.Configuration.ConfigurationManager.AppSettings["gcpServer"];
        public string cloudLabsServer = System.Configuration.ConfigurationManager.AppSettings["CloudLabsUrl"]; 
        public string userManagementConnection = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
        public string tenantConnection = System.Configuration.ConfigurationManager.ConnectionStrings["TenantConnection"].ConnectionString;


        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [HttpGet]
        [Route("Register")]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [Route("Privacy")]
        public ActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        [Route("TermsOfService")]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [HttpGet]
        [Route("Login")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (TempData["CustomError"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["CustomError"].ToString());
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                GCPToken tokens = new GCPToken();
                if (!ModelState.IsValid)
                {
                    TempData["CustomError"] = "Invalid username or password";
                    return RedirectToAction("Login", "Account");
                }
                Dictionary<string, string> tokenDetails = null;
                using (var client = new HttpClient())
                {
                    var login = new Dictionary<string, string>
                    {
                        {"grant_type", "password"},
                        {"username", model.Username },
                        {"password", model.Password }
                    };

                    var resp = await client.PostAsync(authServer + "token", new FormUrlEncodedContent(login));

                    if (resp.IsSuccessStatusCode)
                    {
                        tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content.ReadAsStringAsync().Result);

                        var accessToken = tokenDetails["access_token"].ToString();
                        RoleUser currentUser = GetMe(accessToken);
                        {
                            if (currentUser.Thumbnail == null)
                                currentUser.Thumbnail = "";

                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Authentication, accessToken),
                                new Claim(ClaimTypes.GivenName,  info.ToTitleCase(currentUser.LastName) +", " + info.ToTitleCase(currentUser.FirstName)),
                                new Claim(ClaimTypes.NameIdentifier, currentUser.Id),
                                new Claim(ClaimTypes.Email, currentUser.Email),
                                new Claim(ClaimTypes.Role, currentUser.Role),
                                new Claim(ClaimTypes.Name, currentUser.FirstName),
                                new Claim(ClaimTypes.Surname, currentUser.LastName),
                                new Claim("Thumbnail", currentUser.Thumbnail),
                                new Claim("IsDeleted", currentUser.isDeleted.ToString()),
                                new Claim("IsDisabled", currentUser.isDisabled.ToString()),
                                new Claim("UserGroup", currentUser.UserGroup.ToString()),
                                new Claim("Id", currentUser.Id.ToString()),
                                new Claim("UserId", currentUser.UserId.ToString()),
                                new Claim("TenantId", currentUser.TenantId.ToString()),
                                new Claim("UserIdLTI", currentUser.UserIdLTI)
                            };
                            
                            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                            var properties = new AuthenticationProperties { IsPersistent = model.RememberMe };
                            SignInManager.AuthenticationManager.SignIn(properties, identity);

                            using (var clientGCP = new HttpClient())
                            {
                                var loginGCP = new 
                                {
                                     username = "root-admin",
                                     password = "root-admin"
                                };

                                var dataGCP = JsonConvert.SerializeObject(loginGCP);
                                
                                var respGCP = await clientGCP.PostAsync(gcpServer + "api/users/token/", new StringContent(dataGCP, Encoding.UTF8, "application/json"));

                                tokens = JsonConvert.DeserializeObject<GCPToken>(respGCP.Content.ReadAsStringAsync().Result);
                                
                            }

                            WriteUrlCookie(accessToken, tokens.access, tokens.refresh);
                        }
                    }
                    else
                    {
                        dynamic responseDetails = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                        string error_desc = responseDetails.error_description;

                        TempData["CustomError"] = error_desc;
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
            }
            
            return RedirectToLocal(returnUrl);
        }
        public RedirectResult LogoutDUO()
        {
            var samlEndpoint = "https://sso-dbb138d3.sso.duosecurity.com/saml2/sp/DIZHI4B26A2HSEX739T2/slo";


            return Redirect(samlEndpoint);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("ConfirmEmail")]
        public ActionResult ConfirmEmail(string userId)
        {            
            var user = UserManager.FindById(userId);
            //if (password != "")
            //    user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
            if(user == null)
            {
                TempData["CustomError"] = "Invalid Email. Please contact your administrator.";
                return RedirectToAction("Login", "Account");
            }

            user.EmailConfirmed = true;
            UserManager.Update(user);

            return View();
        }

        //        [HttpGet]
        //        [AllowAnonymous]
        //        [Route("ConfirmEmailWithPass")]
        //        public async Task<ActionResult> ConfirmEmailWithPass(ForgotChangePasswordViewModel model)
        //        {
        //            try
        //            {
        //                if (!ModelState.IsValid)
        //                {
        //                    if (model.Id == null)
        //                        TempData["CustomError"] = "Invalid user";
        //                    else if (model.Password.Length < 6)
        //                        TempData["CustomError"] = "The Password must be at least 6 characters long.";
        //                    else
        //                        TempData["CustomError"] = "The password and confirmation password do not match.";
        //                    return RedirectToAction("ForChangePassword", "Account", new { @id = model.Id });
        //                }

        //                var user = await UserManager.FindByIdAsync(model.Id);
        //                if (user == null)
        //                {
        //                    ModelState.AddModelError(string.Empty, "Invalid user");
        //                    return RedirectToAction("ForChangePassword", "Account", new { @id = model.Id });
        //                }
        //                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
        //                user.EmailConfirmed = true;
        //                UserManager.Update(user);

        //                //SendClientUpdateMail(user.Id, user.Email, model.Password, true);

        //                return View();
        //            }
        //#pragma warning disable CS0168 // The variable 'e' is declared but never used
        //            catch (Exception e)
        //#pragma warning restore CS0168 // The variable 'e' is declared but never used
        //            {

        //            }

        //            return View();
        //        }

        [AllowAnonymous]
        public ActionResult ConfirmEmailWithPass(string id)
        {
            ViewBag.Id = id;

            if (TempData["CustomError"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["CustomError"].ToString());
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmailWithPass(ForgotChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (model.Id == null)
                        TempData["CustomError"] = "Invalid user";
                    else if (model.Password.Length < 6)
                        TempData["CustomError"] = "The Password must be at least 6 characters long.";
                    else
                        TempData["CustomError"] = "The password and confirmation password do not match.";
                    return RedirectToAction("ForChangePassword", "Account", new { @id = model.Id });
                }

                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid user");
                    return RedirectToAction("ConfirmEmailWithPass", "Account", new { @id = model.Id });
                }
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                user.EmailConfirmed = true;
                UserManager.Update(user);

                //SendClientUpdateMail(user.Id, user.Email, model.Password, true);

                return View();
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {

            }

            return View();
        }

        [HttpGet]
        [Route("Logout")]
        public ActionResult Logout()
        {
            var samlEndpoint = "https://sso-dbb138d3.sso.duosecurity.com/saml2/sp/DIZHI4B26A2HSEX739T2/slo";


            IAuthenticationManager AuthenticationManager = HttpContext.GetOwinContext().Authentication;

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);


            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            RedirectToAction("LogoutDUO", "Account");

            return RedirectToAction("Login", "Account");
        }

        public ActionResult SignOut()
        {
            Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        public RoleUser GetMe(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(authServer);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var requestPath = authServer + "api/account/me";

                HttpResponseMessage response = client.GetAsync(requestPath).Result;

                RoleUser userDetails = JsonConvert.DeserializeObject<RoleUser>(response.Content.ReadAsStringAsync().Result);

                return userDetails;
            }
        }

        private void WriteUrlCookie(string accessToken, string gcpAccessToken, string gcpRefreshToken)
        {
            HttpCookie apiCookie = new HttpCookie ("CloudSwyftToken", accessToken);
            HttpCookie apiGCPAccess = new HttpCookie("CloudSwyftGCPAccessToken", gcpAccessToken);
            HttpCookie apiGCPRefresh = new HttpCookie("CloudSwyftGCPRefreshToken", gcpRefreshToken);

            
            //apiCookie.Value = accessToken;
            Response.Cookies.Add(apiCookie);
            Response.Cookies.Add(apiGCPAccess);
            Response.Cookies.Add(apiGCPRefresh);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)

            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }
            base.Dispose(disposing);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            if (TempData["CustomError"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["CustomError"].ToString());
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["CustomError"] = "Invalid Email Address";
                    return RedirectToAction("ForgotPassword", "Account");
                }

                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid user");
                    return View("ForgotPassword");
                }

                user.EmailConfirmed = false;
                UserManager.Update(user);

                SendForgotPasswordMail(user.Id, model.Email);

                return View();
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {

            }
            
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForChangePassword(string id)
        {
            ViewBag.Id = id;

            if (TempData["CustomError"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["CustomError"].ToString());
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ForChangePassword(ForgotChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (model.Id == null)
                        TempData["CustomError"] = "Invalid user";
                    else if (model.Password.Length < 6)
                        TempData["CustomError"] = "The Password must be at least 6 characters long.";
                    else
                        TempData["CustomError"] = "The password and confirmation password do not match.";
                    return RedirectToAction("ForChangePassword", "Account", new { @id=model.Id});
                }

                var user = await UserManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid user");
                    return RedirectToAction("ForChangePassword", "Account", new { @id = model.Id });
                }
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password);
                user.EmailConfirmed = true;
                UserManager.Update(user);

                //SendClientUpdateMail(user.Id, user.Email, model.Password, true);

                return View();
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {

            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<string> ChangePassword(ChangePasswordModel model)
        {
            try
            {
                string errorMessage = "";

                var user = await UserManager.FindByIdAsync(model.Id);
                

                if (!ModelState.IsValid)
                {

                    if (model.Id == null)
                        errorMessage = "Invalid user";
                    if(model.NewPassword == null || model.ConfirmPassword == null)
                        errorMessage = "The Password must be at least 6 characters long";
                    else if (model.NewPassword.Length < 6 || model.ConfirmPassword.Length < 6)
                        errorMessage = "The Password must be at least 6 characters long";
                    else if (model.CurrentPassword == null)
                        errorMessage = "Incorrect Password";
                    else
                        errorMessage = "The password and confirm password do not match";
                    return errorMessage;
                }


                PasswordVerificationResult isSuccess = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.CurrentPassword);
                if (isSuccess.ToString() != "Success")
                {
                    errorMessage = "Incorrect Password";
                    return errorMessage;
                }


                if (user == null)
                {
                    errorMessage = "Invalid user";
                    return errorMessage;
                }
                user.PasswordHash = UserManager.PasswordHasher.HashPassword(model.NewPassword);
                user.EmailConfirmed = true;
                UserManager.Update(user);

                return errorMessage;

            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        public void SendForgotPasswordMail(string userId, string email)
        {
            string returnUrl = cloudLabsServer + "/Account/ForChangePassword?id=" + userId;
             
            MailInfo mailInfo = new MailInfo();
            mailInfo.SendTo = email;
            string htmlBody = string.Empty;

            htmlBody = "<head>"
                   + "<meta charset='utf-8'>"
                   + "<meta name='viewport' content='width=device-width, initial-scale=1.0'/>"
                   + "<title> Welcome to CloudSwyft!</title>"
                   + "<link rel='stylesheet' href=''>"
                   + "</head>"
                   + "<body style='margin: 0; padding: 0; font-family:'Open Sans', sans;'>"

                   + "<table border='0' cellpadding='0' cellspacing='0' width='100%'>"
                   + "<tr><td><table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' width = '600' style = 'border-collapse: collapse; border:1px solid #1058a0;'></tr></td>"
                   + "</table>"

                   + "<table align='center' width='600' style='background-color: #00bff6;padding-top:2%;border-top:2px solid blue;'>"
                   + "<tr align='center'>"
                   + "<td style='padding-top:10px; font-size: 18px; font-family: Helvetica'>Welcome to</td></tr>"
                   + "<tr align='center'><td style='font-family: Trebuchet MS;color: white;font-weight: bold; font-size: 60px;'>CloudSwyft</td ></tr >"
                   + "<tr align='center'><td style='font-family: Verdana; font-size: 23px;color: #005bab;'>CLOUD LABS</td></tr >"
                   + "</table>"

                   + "<table align = 'center' style='background-color: white; width: 600px;padding:40px 40px 40px 40px; border: 1px solid #00bff6; text-align: center;border-bottom: none;'>"
                   + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; text-align: center;'> Click the button below to create new password</td></tr >"
                   + "</table>"

                   + "<table align = 'center' style='background-color: white; width: 600px;padding:0px 40px 20px 40px; border: 1px solid #00bff6; border-top:none;'>"
                   + "<tr align = 'center' ><td>"
                   + "<a href = " + returnUrl + " style ='text-decoration: none; color: #fff;'>"
                   + " <div style='background-color: #8FD034; font-size: 24px; color: white; padding: 8px;'> CREATE NEW PASSWORD </div> </a></td ></tr > "
                   + "</table>"

                   + "</table>";

            mailInfo.Subject = "Welcome to CloudSwyft";

            mailInfo.HtmlBody = htmlBody;

            MailHelper.SendMail(mailInfo);
        }

        [HttpPost]
        public ActionResult DUOLogin()
        {
            //TODO: specify the SAML provider url here, aka "Endpoint"
            var samlEndpoint = "https://sso-dbb138d3.sso.duosecurity.com/saml2/sp/DIZHI4B26A2HSEX739T2/sso";

            var request = new AuthRequest(
                "https://sso-dbb138d3.sso.duosecurity.com/saml2/sp/DIZHI4B26A2HSEX739T2/metadata", //TODO: put your app's "entity ID" here
                "https://labs-westbourne.cloudswyft.com/Account/SamlConsume" //TODO: put Assertion Consumer URL (where the provider should redirect users after authenticating)
            );
            
            //now send the user to the SAML provider
            return Redirect(request.GetRedirectUrl(samlEndpoint));
        }

        public async Task<ActionResult> SamlConsume()
        {
            // 1. TODO: specify the certificate that your SAML provider gave you
            string samlCertificate = @"-----BEGIN CERTIFICATE-----
MIIDDTCCAfWgAwIBAgIUCfpk7O0v8A9uOyYYx602ujGr+RUwDQYJKoZIhvcNAQEL
BQAwNjEVMBMGA1UECgwMRHVvIFNlY3VyaXR5MR0wGwYDVQQDDBRESVpISTRCMjZB
MkhTRVg3MzlUMjAeFw0yMzAzMjcyMjM4MDFaFw0zODAxMTkwMzE0MDdaMDYxFTAT
BgNVBAoMDER1byBTZWN1cml0eTEdMBsGA1UEAwwURElaSEk0QjI2QTJIU0VYNzM5
VDIwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC32G20P1K+T8q4fsNc
+lxpP+pvm6LbDZB3WyMmn8abT3pBFcmq/UB5Uut/1jHurkGgvFkJvm5KiF2JsfPg
SsBPnjmA6qwScF0XwKXKjQaqf6w9SSbqYOmUSdBv1KdYfCpOoeRt9cdhdXXWjVVi
qSauE0Rc/zzOcgvfk6WetEJv4mqD0LW6RSbz4eZCeqofX8eYI+CG0PdqX7V2o6Ur
VRPFVtN2zfA2LrPAbhIUBl+FjRWD4VByYhCOgZgHxJCo/rdrdxT7Jy1en0gRQu7P
j+YNOER4f7sEwbGGzk+6Eceylgb7lP9Je5O0PVxY11EKhMHxOIdtIFpvLcTX4siP
V9tFAgMBAAGjEzARMA8GA1UdEwEB/wQFMAMBAf8wDQYJKoZIhvcNAQELBQADggEB
AHLBy+shhiIWnDix0QmQPojs5o9y0LQ/Bl30a2rOU1gwCnDNB96WBz8WuROTHUOy
amIv/8UDA1yj8rdgJIL8UMpFuqJf0CGEwq8x41mzEmKfSySVQkiA/vcRtEWUmSEw
iATgs/HJ2dllkaIea1m7GQv3LnwFuE93tWMDbZBQG+ANX0LOZ7hHEZ58PmHP/rUC
RlA3P0WI92015XlHcRssZ9vR7v4Ywg2lLREuyT6h8F3UjsScyF85DLH5YOEci+xY
8i30737KkgZ7DM4tWGqVy5QvE7elQtCPyafRi2KHtSYWgXd55hJQ9q5rPDYhN/Xh
LasHKy1j9KhcMVs5uUDLhEk=
-----END CERTIFICATE-----";


            var samlResponse = new Response(samlCertificate, Request.Form["SAMLResponse"]);
            Debug.WriteLine("samlResponse: " + samlResponse);

            // 3. DONE!
            if (samlResponse.IsValid()) //all good?
            {
                //WOOHOO!!! the user is logged in
                var username = samlResponse.GetNameID(); //let's get the username
                var email1 = samlResponse.GetCustomAttribute("emailAddress");
                var email = samlResponse.GetEmail(); //let's get the username
                var lastname = samlResponse.GetLastName(); //let's get the username
                var firstname = samlResponse.GetFirstName(); //let's get the username
                var DisplayName = samlResponse.GetCustomAttribute("DisplayName"); //let's get the username
                var Email = samlResponse.GetCustomAttribute("Email"); //let's get the username
                var UPN = samlResponse.GetCustomAttribute("upn"); //let's get the username
                var UPN1 = samlResponse.GetCustomAttribute("Upn"); //let's get the username
                var upn = samlResponse.GetUpn();

                //the user has been authenticated
                //now set a cookie using FormsAuthentication.SetAuthCookie()
                FormsAuthentication.SetAuthCookie(Email, false);

                ApplicationUser user = new ApplicationUser();

                user.Email = Email;
                user.FirstName = DisplayName;
                user.LastName = DisplayName;
                user.UserGroup = 1;
                user.Thumbnail = null;
                user.TenantId = 1;
                user.UserName = Email;
                user.DateCreated = DateTime.UtcNow;
                user.CreatedBy = "Duo";
                user.isDeleted = false;
                user.isDisabled = false;
                user.UserIdLTI = "Duo";

                var dataUser = JsonConvert.SerializeObject(user);

                using (var client = new HttpClient())
                {
                    var resp = await client.PostAsync(authServer + "/api/account/CreateDuo", new StringContent(dataUser, Encoding.UTF8, "application/json"));
                }

                TextInfo info = CultureInfo.CurrentCulture.TextInfo;

                GCPToken tokens = new GCPToken();
                Dictionary<string, string> tokenDetails = null;
                using (var client = new HttpClient())
                {
                    var login = new Dictionary<string, string>
                    {
                        {"grant_type", "password"},
                        {"username", user.Email },
                        {"password", "Password1!" }
                    };

                    var resp = await client.PostAsync(authServer + "token", new FormUrlEncodedContent(login));

                    if (resp.IsSuccessStatusCode)
                    {
                        tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content.ReadAsStringAsync().Result);

                        var accessToken = tokenDetails["access_token"].ToString();
                        RoleUser currentUser = GetMe(accessToken);
                        {
                            if (currentUser.Thumbnail == null)
                                currentUser.Thumbnail = "";

                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Authentication, accessToken),
                                new Claim(ClaimTypes.GivenName,  info.ToTitleCase(currentUser.LastName) +", " + info.ToTitleCase(currentUser.FirstName)),
                                new Claim(ClaimTypes.NameIdentifier, currentUser.Id),
                                new Claim(ClaimTypes.Email, currentUser.Email),
                                new Claim(ClaimTypes.Role, currentUser.Role),
                                new Claim(ClaimTypes.Name, currentUser.FirstName),
                                new Claim(ClaimTypes.Surname, currentUser.LastName),
                                new Claim("Thumbnail", currentUser.Thumbnail),
                                new Claim("IsDeleted", currentUser.isDeleted.ToString()),
                                new Claim("IsDisabled", currentUser.isDisabled.ToString()),
                                new Claim("UserGroup", currentUser.UserGroup.ToString()),
                                new Claim("Id", currentUser.Id.ToString()),
                                new Claim("UserId", currentUser.UserId.ToString()),
                                new Claim("TenantId", currentUser.TenantId.ToString()),
                                new Claim("UserIdLTI", currentUser.UserIdLTI)
                            };

                            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                            var properties = new AuthenticationProperties { IsPersistent = true };
                            SignInManager.AuthenticationManager.SignIn(properties, identity);

                            using (var clientGCP = new HttpClient())
                            {
                                var loginGCP = new
                                {
                                    username = "root-admin",
                                    password = "root-admin"
                                };

                                var dataGCP = JsonConvert.SerializeObject(loginGCP);

                                var respGCP = await clientGCP.PostAsync(gcpServer + "api/users/token/", new StringContent(dataGCP, Encoding.UTF8, "application/json"));

                                tokens = JsonConvert.DeserializeObject<GCPToken>(respGCP.Content.ReadAsStringAsync().Result);
                            }

                            WriteUrlCookie(accessToken, tokens.access, tokens.refresh);
                        }
                    }
                    else
                    {
                        dynamic responseDetails = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                        string error_desc = responseDetails.error_description;

                        TempData["CustomError"] = error_desc;
                        return RedirectToAction("Login", "Account");
                    }
                }


               // return "username=" + username + "-email1=" + email1 + "-lastname="+ lastname +"-firstname="+ firstname + "-DisplayName="+ DisplayName + "-Email="+ Email + "-UPN=" + UPN + "-upn=" + upn + "-UPN1=" + UPN1;

                return RedirectToAction("Index", "Labsession");
            }
            //return "";
            return RedirectToAction("Login", "Account");
        }
    }
}
