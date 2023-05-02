using CloudSwyft.Web.Api;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Web.Http;
using CloudSwyft.CloudLabs;

[assembly: OwinStartup(typeof(CloudSwyft.CloudLabs.Startup))]
namespace CloudSwyft.CloudLabs
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            ConfigureAuth(app);
            WebApiConfig.Register(config);
        }
    }
}
