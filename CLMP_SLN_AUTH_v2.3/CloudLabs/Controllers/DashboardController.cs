using System.Web.Mvc;
using System.Security.Claims;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using CloudSwyft.CloudLabs.Models;
using System.Web;
using Microsoft.AspNet.Identity;

namespace CloudSwyft.CloudLabs.Controllers
{
    [Authorize]
    public class DashboardController : Controller
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