using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;
using Microsoft.Owin.Security.OAuth;
using System.Web;
using System.Net.Http;
using System;
using CloudSwyft.CloudLabs.Models;
using CloudSwyft.CloudLabs.Controllers;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel.DataAnnotations.Schema;

//using System.Web.Mvc;

namespace CloudSwyft.CloudLabs.Models
{
    [RoutePrefix("api/ApplicationUser")]
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool isDeleted { get; set; }
        public bool isDisabled { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public bool CredentialsSent { get; set; }
        public string Thumbnail { get; set; }
        public int UserGroup { get; set; }
        public int TenantId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserIdLTI { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]{3,15}")]
        public string UserName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]{3,15}")]
        public string Email { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        //public bool isDeleted { get; set; }
        //public bool isDisabled { get; set; }
        //public string CreatedBy { get; set; }

        //public int UserGroup { get; set; }
        //public bool CredentialsSent { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            var user = manager.Users.Where(x => x.Email == userIdentity.Name).FirstOrDefault();
            return userIdentity;
        }
    }

    public class RoleUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool isDeleted { get; set; }
        public bool isDisabled { get; set; }
        public string CreatedBy { get; set; }
        public bool CredentialsSent { get; set; }
        public string Role { get; set; }
        public string Thumbnail { get; set; }
        public string UserGroup { get; set; }
#pragma warning disable CS0114 // 'RoleUser.Id' hides inherited member 'IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>.Id'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public string Id { get; set; }
#pragma warning restore CS0114 // 'RoleUser.Id' hides inherited member 'IdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>.Id'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.

        public string UserId { get; set; }
        public string TenantId { get; set; }
        public string Rolename { get; set; }
        public string UserIdLTI { get; set; }
    }

    public class UserClaim
    {
        public string Id { get; set; }
        public string Fullname { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public string Thumbnail { get; set; }
        public string UserGroup { get; set; }

        public string UserId { get; set; }
        public string TenantId { get; set; }
        public string Rolename { get; set; }
        public string UserIdLTI { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("UserManagementConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().ToTable("CloudLabUsers");
        }
    }

    public static class changeRole
    {
        public static bool RoleChange = false;
    }

    public static class CustomUserIdentity
    {
        public static UserClaim UserDetails()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var firstname = identity.Claims.Where(r => r.Type == ClaimTypes.Name).Select(r => r.Value).FirstOrDefault();
            var fullname = identity.Claims.Where(r => r.Type == ClaimTypes.GivenName).Select(r => r.Value).FirstOrDefault();
            var role = identity.Claims.Where(r => r.Type == ClaimTypes.Role).Select(r => r.Value).FirstOrDefault();
            var email = identity.Claims.Where(r => r.Type == ClaimTypes.Email).Select(r => r.Value).FirstOrDefault();
            var thumbnail = identity.Claims.Where(r => r.Type == "Thumbnail").Select(r => r.Value).FirstOrDefault();
            var lastname = identity.Claims.Where(r => r.Type == ClaimTypes.Surname).Select(r => r.Value).FirstOrDefault();
            var usergroup = identity.Claims.Where(r => r.Type == "UserGroup").Select(r => r.Value).FirstOrDefault();
            var id = identity.Claims.Where(r => r.Type == "Id").Select(r => r.Value).FirstOrDefault();
            var userId = identity.Claims.Where(r => r.Type == "UserId").Select(r => r.Value).FirstOrDefault();
            var tenantid = identity.Claims.Where(r => r.Type == "TenantId").Select(r => r.Value).FirstOrDefault();
            var userIdLTI = identity.Claims.Where(r => r.Type == "UserIdLTI").Select(r => r.Value).FirstOrDefault();
            var username = identity.Claims.Where(r => r.Type == "Username").Select(r => r.Value).FirstOrDefault();

            var UserDetails = new UserClaim
            {
                Firstname = firstname,
                Lastname = lastname,
                Fullname = fullname,
                Role = role,
                Email = email,
                Thumbnail = thumbnail,
                UserGroup = usergroup,
                Id = id,
                UserId = userId,
                UserIdLTI = userIdLTI,
                TenantId = tenantid,
            };
            return UserDetails;
        }

        public static bool UserChangeDetails()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var isDeleted = identity.Claims.Where(r => r.Type == "IsDeleted").Select(r => r.Value).FirstOrDefault();
            var isDisabled = identity.Claims.Where(r => r.Type == "IsDisabled").Select(r => r.Value).FirstOrDefault();
            var roleChange = identity.Claims.Where(r => r.Type == "RoleChange").Select(r => r.Value).FirstOrDefault();
            bool userDeleteDisable = false;

            if (identity.FindFirst("RoleChange") != null)
            {
                if (isDisabled == "True" || isDeleted == "True" || roleChange == "True")
                    userDeleteDisable = true;
                else
                    userDeleteDisable = false;
            }
            else
            {
                if (isDisabled == "True" || isDeleted == "True")
                    userDeleteDisable = true;
                else
                    userDeleteDisable = false;
            }
            return userDeleteDisable;
        }
        public static string cloudlabsAPI = System.Configuration.ConfigurationManager.AppSettings["Cloudswyft.Api"];

        public static string GroupName()
        {
            var groupId = UserDetails().UserGroup;
            var client = new HttpClient { BaseAddress = new Uri(cloudlabsAPI) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = client.GetAsync(cloudlabsAPI+ "CloudLabsGroups/GetUserGroupName?CloudLabsGroupID="+ groupId).Result;

            if (!response.IsSuccessStatusCode) return response.ReasonPhrase;
            var responseContent = response.Content;
            var responseString = responseContent.ReadAsStringAsync().Result;
            responseString = responseString.Replace("\"", "");
            return responseString;
        }

        public static string GroupCode()
        {
            var groupId = UserDetails().UserGroup;
            var client = new HttpClient { BaseAddress = new Uri(cloudlabsAPI) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = client.GetAsync(cloudlabsAPI + "CloudLabsGroups/GetUserGroupCode?CloudLabsGroupID=" + groupId).Result;

            if (!response.IsSuccessStatusCode) return response.ReasonPhrase;
            var responseContent = response.Content;
            var responseString = responseContent.ReadAsStringAsync().Result;
            responseString = responseString.Replace("\"", "");
            return responseString;
        }
    }

    public class GroupName
    {
        public string GroupName1 { get; set; }       
    }
    public class IdentityServices
    {
        public bool AddUpdateClaim()
        {
            try
            {
                IPrincipal currentPrincipal = Thread.CurrentPrincipal;
                var identity = currentPrincipal.Identity as ClaimsIdentity;

                var token = identity.Claims.Where(r => r.Type == ClaimTypes.Authentication).SingleOrDefault().Value;

                var userNew = new
                {
                    firstname = identity.Claims.Where(r => r.Type == ClaimTypes.Name).SingleOrDefault().Value,
                    lastname = identity.Claims.Where(r => r.Type == ClaimTypes.Surname).SingleOrDefault().Value,
                    role = identity.Claims.Where(r => r.Type == ClaimTypes.Role).SingleOrDefault().Value,
                    email = identity.Claims.Where(r => r.Type == ClaimTypes.Email).SingleOrDefault().Value,
                    //thumbnail = identity.Claims.Where(r => r.Type == "Thumbnail").SingleOrDefault().Value,
                    //isDeleted = identity.Claims.Where(r => r.Type == "IsDeleted").SingleOrDefault().Value,
                    //isDisabled = identity.Claims.Where(r => r.Type == "IsDisabled").SingleOrDefault().Value,
                    usergroup = identity.Claims.Where(r => r.Type == "UserGroup").SingleOrDefault().Value,
                    tenantid = identity.Claims.Where(r => r.Type == "TenantId").SingleOrDefault().Value,
                };
                AccountController x = new AccountController();
                TextInfo info = CultureInfo.CurrentCulture.TextInfo;
                var isChange = false;

                var userDetails =  x.GetMe(identity.Claims.Where(r => r.Type == ClaimTypes.Authentication).Select(r => r.Value).FirstOrDefault());

                var userOld = new
                {
                    firstname = userDetails.FirstName,
                    lastname = userDetails.LastName,
                    role = userDetails.Role,
                    email = userDetails.Email,
                    //thumbnail = userDetails.Thumbnail,
                    //isDeleted = userDetails.isDeleted.ToString(),
                    //isDisabled = userDetails.isDisabled.ToString(),
                    usergroup = userDetails.UserGroup,
                    tenantid = userDetails.TenantId
                };

                if (userOld.email != userNew.email)
                    isChange = true;
                
                //if (userOld.role != userNew.role)
                //    identity.AddClaim(new Claim("RoleChange", "True"));
                //else
                //{
                //    if (identity.FindFirst("RoleChange") != null)
                //        identity.RemoveClaim(identity.FindFirst("RoleChange"));
                //}                

                if (!userOld.Equals(userNew))
                {
                    //identity.RemoveClaim(identity.FindFirst(ClaimTypes.Name));
                    //identity.RemoveClaim(identity.FindFirst(ClaimTypes.Surname));
                    identity.RemoveClaim(identity.FindFirst(ClaimTypes.Role));
                    identity.RemoveClaim(identity.FindFirst(ClaimTypes.Email));
                    identity.RemoveClaim(identity.FindFirst(ClaimTypes.GivenName));
                    //identity.RemoveClaim(identity.FindFirst("Thumbnail"));
                    //identity.RemoveClaim(identity.FindFirst("IsDeleted"));
                    //identity.RemoveClaim(identity.FindFirst("IsDisabled"));
                    identity.RemoveClaim(identity.FindFirst("UserGroup"));
                    identity.RemoveClaim(identity.FindFirst("TenantId"));

                    //identity.AddClaim(new Claim(ClaimTypes.Name, userDetails.FirstName));
                    //identity.AddClaim(new Claim(ClaimTypes.Surname, userDetails.LastName));
                    identity.AddClaim(new Claim(ClaimTypes.Role, userDetails.Role));
                    identity.AddClaim(new Claim(ClaimTypes.Email, userDetails.Email));
                    identity.AddClaim(new Claim(ClaimTypes.GivenName, info.ToTitleCase(userDetails.LastName) + ", " + info.ToTitleCase(userDetails.FirstName)));
                    //identity.AddClaim(new Claim("Thumbnail", userDetails.Thumbnail));
                    //identity.AddClaim(new Claim("IsDeleted", userDetails.isDeleted.ToString()));
                    //identity.AddClaim(new Claim("IsDisabled", userDetails.isDisabled.ToString()));
                    identity.AddClaim(new Claim("UserGroup", userDetails.UserGroup.ToString()));
                    identity.AddClaim(new Claim("TenantId", userDetails.TenantId.ToString()));
                }

                return isChange;

            }

#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (System.Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
                return false;
            }
        }
    }
}