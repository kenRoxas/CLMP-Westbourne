using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Threading.Tasks;
using LtiLibrary.Lti1;
//using LtiLibrary.Core.Lti1;
//using LtiLibrary.AspNet.Extensions;
using CloudSwyft.CloudLabs.Models;
using CloudSwyft.CloudLabs.Helpers;
using System.Web.Routing;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity.Owin;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Http.Cors;
using System.Data.SqlClient;
using Microsoft.Owin.Host.SystemWeb;
using System.Globalization;
using System.Text;

namespace CloudSwyft.CloudLabs.Controllers
{   
    public class LabSessionController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public string authServer = System.Configuration.ConfigurationManager.AppSettings["CloudSwyftAuthServerUrl"];
        public string cloudLabsServer = System.Configuration.ConfigurationManager.AppSettings["CloudLabsUrl"];
        public string userManagementConnection = System.Configuration.ConfigurationManager.ConnectionStrings["UserManagementConnection"].ConnectionString;
        public string tenantConnection = System.Configuration.ConfigurationManager.ConnectionStrings["TenantConnection"].ConnectionString;
        public string gcpServer = System.Configuration.ConfigurationManager.AppSettings["gcpServer"];


        //public string ltiClientSecret = System.Configuration.ConfigurationManager.AppSettings["LtiClientSecret"];
        //public string ltiClientKey = System.Configuration.ConfigurationManager.AppSettings["LtiClientKey"];

        public LabSessionController()
        {
        }

        public LabSessionController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        public async Task<RoleUser> GetMe(string token)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(authServer);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var requestPath = authServer + "api/account/me";

                HttpResponseMessage response = await client.GetAsync(requestPath);
                RoleUser userDetails = JsonConvert.DeserializeObject<RoleUser>(response.Content.ReadAsStringAsync().Result);

                return userDetails;
            }
        }

        private void WriteUrlCookie(string accessToken, string gcpAccessToken, string gcpRefreshToken)
        {
            HttpCookie apiCookie = new HttpCookie("CloudSwyftToken", accessToken);
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

        [Authorize]
        public ActionResult Index()
        {
            var x = new IdentityServices();
            var isChange = x.AddUpdateClaim();
            if (isChange)
            {
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Login", "Account");
            }

            if (User.IsInRole("Student") || User.IsInRole("Instructor") || User.IsInRole("Admin") || User.IsInRole("Staff"))
                return View();
            else
                return RedirectToAction("Index", "Dashboard"); 
        }

//        public async Task<ActionResult> LtiView()
//        {
//            var lti = new LtiRequest();
//            lti.ParseRequest(Request);
//            var ltiEmail = lti.LisPersonEmailPrimary;

//            var ltiClientKey = lti.ConsumerKey;
//            Console.Write(lti.LisPersonEmailPrimary);
//            Console.Write(lti.ConsumerKey);
//            var ltiCourse = lti.ContextId;
//            string courseId = ltiCourse.Split('+', '+')[1];
            
//            try
//            {
//                Dictionary<string, string> tokenDetails = null;
//                using (var client = new HttpClient())
//                {
//                    var login = new Dictionary<string, string>
//                    {
//                        {"grant_type", "client_credentials"},
//                        {"client_id", ltiClientKey },
//                        {"client_secret", ltiClientSecret },
//                        {"email", ltiEmail }
//                    };

//                    var resp = await client.PostAsync(authServer + "token", new FormUrlEncodedContent(login));

//                    if (resp.IsSuccessStatusCode)
//                    {
//                        tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content.ReadAsStringAsync().Result);

//                        var accessToken = tokenDetails["access_token"].ToString();
//                        var currentUser = await GetMe(accessToken);
//                        {
//                            if (currentUser.Thumbnail == null)
//                                currentUser.Thumbnail = "";

//                            var claims = new List<Claim>
//                    {
//                        new Claim(ClaimTypes.Authentication, accessToken),
//                        new Claim(ClaimTypes.GivenName, currentUser.FirstName+" "+ currentUser.LastName),
//                        new Claim(ClaimTypes.NameIdentifier, currentUser.Id),
//                        new Claim(ClaimTypes.Email, currentUser.Email),
//                        new Claim(ClaimTypes.Role, currentUser.Role),
//                        new Claim(ClaimTypes.Name, currentUser.FirstName),
//                        new Claim(ClaimTypes.Surname, currentUser.LastName),
//                        new Claim("Thumbnail", currentUser.Thumbnail),
//                        new Claim("IsDeleted", currentUser.isDeleted.ToString()),
//                        new Claim("IsDisabled", currentUser.isDisabled.ToString()),
//                        new Claim("UserGroup", currentUser.UserGroup.ToString()),
//                        new Claim("Id", currentUser.Id.ToString()),
//                        new Claim("UserId", currentUser.UserId.ToString()),
//                        new Claim("TenantId", currentUser.TenantId.ToString())
//                    };
//                            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
//                            var properties = new AuthenticationProperties { IsPersistent = false };
//                            SignInManager.AuthenticationManager.SignIn(properties, identity);
//                            WriteUrlCookie(accessToken);
//                        }
                        
//                    }
//                    else
//                    {
//                        dynamic responseDetails = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
//                        string error_desc = responseDetails.error_description;

//                        TempData["CustomError"] = error_desc;
//                        return RedirectToAction("Index", "Dashboard");
//                    }
//                }

//                //if (Request.GenerateOAuthSignature(ltiClientSecret).Equals(lti.Signature) && ltiClientKey.Equals(ltiClientKey))
//                if (ltiClientKey.Equals(ltiClientKey))
//                    return RedirectToAction("Index", "LabSession");
//                else
//                {
//                    Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
//                    return RedirectToAction("Index", "Dashboard");
//                }
//            }
//#pragma warning disable CS0168 // The variable 'e' is declared but never used
//            catch (Exception e)
//#pragma warning restore CS0168 // The variable 'e' is declared but never used
//            {
//                return RedirectToAction("Index", "Dashboard");
//            }
//        }

        public async Task<ActionResult> LTI()
        {
            var lti = new LtiRequest();
            lti.ParseRequest(Request);
            HttpResponseMessage resp = new HttpResponseMessage();
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            GCPToken tokens = new GCPToken();

            using (SqlConnection _db = new SqlConnection(userManagementConnection))
            {
                using (SqlConnection db = new SqlConnection(tenantConnection))
                {
                    _db.Open();
                    db.Open();
                    using (SqlCommand command1 = new SqlCommand("SELECT * FROM AzTenants WHERE ClientKey = '" + lti.ConsumerKey + "'", db))
                    using (SqlDataReader execRead = command1.ExecuteReader())
                    {
                        if (execRead.HasRows)
                        {
                            using (SqlCommand com = new SqlCommand("SELECT * FROM CloudLabUsers WHERE email = '" + lti.LisPersonEmailPrimary + "'" + "or USERIDLTI = '" + lti.UserId + "'", _db))
                            using (SqlDataReader read = com.ExecuteReader())
                            {
                                if (!read.HasRows)
                                {
                                    using (HttpClient client = new HttpClient())
                                    {
                                        //HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

                                        //if (lti.UserId != null)
                                        //    httpResponseMessage = await client.PostAsync(authServer + "api/Account/CreateFromLTI?UseridLTI=" + lti.UserId, null);
                                        //else
                                        //httpResponseMessage = 
                                        await client.PostAsync(authServer + "api/Account/CreateFromLTI?UseridLTI=" + lti.UserId + "&firstname=" + lti.LisPersonNameGiven + "&lastname=" + lti.LisPersonNameFamily + "&email=" + lti.LisPersonEmailPrimary + "&clientKey=" + lti.ConsumerKey, null);
                                        //await client.PostAsync(authServer + "api/Account/CreateFromLTI?UseridLTI=" + lti.UserId + "&firstname=" + lti.LisPersonNameGiven + "&lastname=" + lti.LisPersonNameFamily + "&email=" + lti.LisPersonEmailPrimary + "&clientKey=" + lti.ConsumerKey, null);

                                    }
                                }

                                Dictionary<string, string> tokenDetails = null;
                                using (HttpClient client = new HttpClient())
                                {
                                    var login = new Dictionary<string, string>
                                                {
                                                   {"grant_type", "client_credentials"},
                                                    {"client_id", lti.ConsumerKey },
                                                    {"userIdLTI", lti.UserId},
                                                    //{"client_secret", ltiClientSecret },
                                                    {"email", lti.LisPersonEmailPrimary }
                                                };

                                    resp = await client.PostAsync(authServer + "token", new FormUrlEncodedContent(login));

                                    if (resp.IsSuccessStatusCode)
                                    {
                                        tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content.ReadAsStringAsync().Result);

                                        var accessToken = tokenDetails["access_token"].ToString();
                                        var currentUser = await GetMe(accessToken);
                                        {
                                            if (currentUser.Thumbnail == null)
                                                currentUser.Thumbnail = "";
                                            if (currentUser.UserIdLTI == null)
                                                currentUser.UserIdLTI = "";

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
                                            var properties = new AuthenticationProperties { IsPersistent = false };
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
                        }

                        _db.Close();
                        db.Close();
                    }
                    }
                }

            using (SqlConnection db = new SqlConnection(tenantConnection))
            using (SqlCommand command1 = new SqlCommand("SELECT * FROM AzTenants WHERE ClientKey = '" + lti.ConsumerKey + "'", db))
            {
                db.Open();
            using (SqlDataReader execRead = command1.ExecuteReader())
                {
                    if (execRead.HasRows)
                    {
                        db.Close();
                        return RedirectToAction("Index", "LabSession");
                    }
                    else
                    {
                        db.Close();
                        Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                        return RedirectToAction("Login", "Account");
                    }
                }
            }
        }

    }
}