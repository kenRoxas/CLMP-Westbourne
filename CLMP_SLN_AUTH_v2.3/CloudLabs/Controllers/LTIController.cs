using System.Web.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using CloudSwyft.CloudLabs.Models;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using System.Net.Http;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace CloudSwyft.CloudLabs.Controllers
{
    [Authorize]
    public class LTIController : Controller
    {
        public string authServer = System.Configuration.ConfigurationManager.AppSettings["CloudSwyftAuthServerUrl"];
        public string ltiClientSecret = System.Configuration.ConfigurationManager.AppSettings["LtiClientSecret"];
        public string ltiClientKey = System.Configuration.ConfigurationManager.AppSettings["LtiClientKey"];

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public LTIController()
        {
        }

        public LTIController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        [HttpPost]
        [AllowAnonymous]
        public async Task LoginLTI(string email, string ltiClientKey, string ltiSignature)
        {
            try
            {
                Dictionary<string, string> tokenDetails = null;
                using (var client = new HttpClient())
                {
                    var login = new Dictionary<string, string>
                    {
                        {"grant_type", "client_credentials"},
                        {"client_id", ltiClientKey },
                        {"client_secret", ltiSignature },
                        {"email", email }
                    };

                    var resp = await client.PostAsync(authServer + "token", new FormUrlEncodedContent(login));

                    if (resp.IsSuccessStatusCode)
                    {
                        tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content.ReadAsStringAsync().Result);

                        var accessToken = tokenDetails["access_token"].ToString();

                        var currentUser = await GetMe(accessToken);
                        {
                            if (currentUser.Thumbnail == null)
                                currentUser.Thumbnail = "";

                            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Authentication, accessToken),
                        new Claim(ClaimTypes.GivenName, currentUser.FirstName+" "+ currentUser.LastName),
                        new Claim(ClaimTypes.NameIdentifier, currentUser.Id),
                        new Claim(ClaimTypes.Email, currentUser.Email),
                        new Claim(ClaimTypes.Role, currentUser.Role),
                        new Claim(ClaimTypes.Name, currentUser.FirstName),
                        new Claim(ClaimTypes.Surname, currentUser.LastName),
                        new Claim("Thumbnail", currentUser.Thumbnail),
                        new Claim("IsDeleted", currentUser.isDeleted.ToString()),
                        new Claim("IsDisabled", currentUser.isDisabled.ToString()),
                        new Claim("UserGroup", currentUser.UserGroup.ToString()),
                        new Claim("Id", currentUser.Id.ToString())
                    };
                            var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                            var properties = new AuthenticationProperties { IsPersistent = false };
                            SignInManager.AuthenticationManager.SignIn(properties, identity);
                            WriteUrlCookie(accessToken);
                        }
                    }
                    else
                    {
                        dynamic responseDetails = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                        string error_desc = responseDetails.error_description;
                        TempData["CustomError"] = error_desc;
                    }

                    //if (resp.IsSuccessStatusCode)
                    //{
                    //    tokenDetails = JsonConvert.DeserializeObject<Dictionary<string, string>>(resp.Content.ReadAsStringAsync().Result);

                    //    var accessToken = tokenDetails["access_token"].ToString();

                    //    var requestBody = CsApi.ApiCall(authServer, "/api/Account/FindByEmail?email="+ "stud4@cloudswyft.com", accessToken);

                    //    dynamic currentUser = JsonConvert.DeserializeObject<Users>(requestBody as string);

                    //    if (currentUser.Thumbnail == null)
                    //        currentUser.Thumbnail = "";

                    //    IPrincipal currentPrincipal = Thread.CurrentPrincipal;
                    //    var identity = currentPrincipal.Identity as ClaimsIdentity;

                    //    identity.AddClaim(new Claim(ClaimTypes.Authentication, accessToken));
                    //    identity.AddClaim(new Claim(ClaimTypes.GivenName, currentUser.FirstName + " " + currentUser.LastName));
                    //    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, currentUser.Id));
                    //    identity.AddClaim(new Claim(ClaimTypes.Email, currentUser.Email));
                    //    identity.AddClaim(new Claim(ClaimTypes.Name, currentUser.FirstName));
                    //    identity.AddClaim(new Claim(ClaimTypes.Surname, currentUser.LastName));
                    //    identity.AddClaim(new Claim("Thumbnail", currentUser.Thumbnail));
                    //    identity.AddClaim(new Claim("IsDeleted", currentUser.isDeleted.ToString()));
                    //    identity.AddClaim(new Claim("IsDisabled", currentUser.isDisabled.ToString()));
                    //    identity.AddClaim(new Claim("UserGroup", currentUser.UserGroup.ToString()));
                    //    identity.AddClaim(new Claim("Id", currentUser.Id.ToString()));

                    //    WriteUrlCookie(accessToken);
                    //}
                    //else
                    //{
                    //    dynamic responseDetails = JsonConvert.DeserializeObject(resp.Content.ReadAsStringAsync().Result);
                    //    string error_desc = responseDetails.error_description;
                    //}
                }
            }
            catch (Exception e)
            {

            }
        }


        private void WriteUrlCookie(string accessToken)
        {
            HttpCookie apiCookie = new HttpCookie("CloudSwyftToken");

            apiCookie.Value = accessToken;
            Response.Cookies.Add(apiCookie);
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
    }
}