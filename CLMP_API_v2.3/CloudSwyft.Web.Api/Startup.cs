using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(CloudSwyft.Web.Api.Startup))]

namespace CloudSwyft.Web.Api
{
    public partial class Startup
    {
        //public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        //public void Configuration(IAppBuilder app)
        //{
        //    HttpConfiguration config = new HttpConfiguration();

        //    ConfigureOAuth(app);

        //    WebApiConfig.Register(config);
        //    app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
        //    app.UseWebApi(config);

        //}

        //private void ConfigureOAuth(IAppBuilder app)
        //{
        //    OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        //    //Token Consumption
        //    app.UseOAuthBearerAuthentication(OAuthBearerOptions);
        //}
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
