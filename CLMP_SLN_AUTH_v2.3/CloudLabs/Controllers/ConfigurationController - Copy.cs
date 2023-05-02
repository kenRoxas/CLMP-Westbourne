using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Threading.Tasks;

namespace NewVE.Controllers
{
    [Authorize]
    public class ConfigurationController : Controller
    {

        public ActionResult Index()
        {
            if (User.IsInRole("Student"))
                return RedirectToAction("Index", "Labsession");
            else
                return View();

        }
    }
}