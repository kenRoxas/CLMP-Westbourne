using CloudSwyft.Auth.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace CloudSwyft.Auth
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;
        AuthContext _db = new AuthContext();


        private UserManager<ApplicationUser> _userManager;

        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }

        public async Task<IdentityResult> RegisterUser(RegisterViewModel userModel)
        {

            IdentityResult result = null;
            try
            {
                var user = new ApplicationUser
                {
                    FirstName = userModel.FirstName,
                    LastName = userModel.LastName,
                    Email = userModel.Email,
                    UserName = userModel.Email,
                    DateCreated = DateTime.Now,
                    CreatedBy = "Self-Registration"
                };
                
                //var roleName = _db.Roles.Where(r => r.Name == "Trainee").FirstOrDefault().Id;
                var roleName = "Student";
                result = await _userManager.CreateAsync(user, userModel.Password);

                //if (result.Succeeded)
                //    using (HttpClient client = new HttpClient())
                //    {
                //        await client.GetAsync(CloudSwyftAssessmentUrl + "api/users/SendClientUpdateMail?userId=" + user.Id + "&email=" + user.Email + "&password=" + userModel.Password + "&isEdit=" + false);
                //    }

                await _userManager.AddToRoleAsync(user.Id, roleName);
                user.CredentialsSent = true;
                await _userManager.UpdateAsync(user);
            }
            catch (Exception e)
            {
                string f = e.Message;
            }
            return result;
        }

        //public async Task<ApplicationUser> FindUser(string userName, string password)
        //{
        //    ApplicationUser user = await _userManager.FindAsync(userName, password);
        //    return user;
        //}
        public ApplicationUser FindUser(string userName, string password)
        {
            ApplicationUser user = _userManager.Find(userName, password);
            return user;
        }
        //public async Task<ApplicationUser> FindUser(string email)
        //{
        //    ApplicationUser user = await _userManager.FindByEmailAsync(email);
        //    return user;
        //}
        public ApplicationUser FindUser(string email)
        {
            ApplicationUser user = _userManager.FindByEmail(email);
            return user;
        }
        //public async Task<ApplicationUser> FindUserById(string id)
        //{
        //    ApplicationUser user = await _userManager.FindByIdAsync(id);
        //    return user;
        //}
        public ApplicationUser FindUserById(string id)
        {
            ApplicationUser user = _userManager.FindById(id);
            return user;
        }
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}