using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Threading.Tasks;
using CloudSwyft.CloudLabs.Models;
using Microsoft.AspNet.Identity;

namespace CloudSwyft.CloudLabs.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        public ActionResult Index()
        {
            var x = new IdentityServices();
            var isChange = x.AddUpdateClaim();

            if (isChange)
            {
                Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("Login", "Account");
            }

            if (User.IsInRole("Student"))
                return RedirectToAction("Index", "Labsession");
            else
                return View();
        }
    }
}