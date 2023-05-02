using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace CloudSwyft.CloudLabs
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
           name: "Render",
           url: "{controller}/{action}/{veprofileid}/{userid}",
           defaults: new { controller = "VirtualEnvironment", action = "Render", veprofileid = UrlParameter.Optional, userid = UrlParameter.Optional }
           );

            routes.MapRoute(
                name: "RenderEx",
                url: "{controller}/{action}/{courseid}/{userid}/{id}",
                defaults: new { controller = "VirtualEnvironment", action = "Render", courseid = UrlParameter.Optional, userid = UrlParameter.Optional }
            );
        }
    }
}
