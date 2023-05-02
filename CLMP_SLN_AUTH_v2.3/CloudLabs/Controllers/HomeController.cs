using CloudSwyft.CloudLabs.Models;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace CloudSwyft.CloudLabs.Controllers
{
    //[Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var x = new IdentityServices();
            x.AddUpdateClaim();

            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Index", "Labsession");
            }
            else
            {
                return RedirectToAction("Index", "Dashboard");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    }
}