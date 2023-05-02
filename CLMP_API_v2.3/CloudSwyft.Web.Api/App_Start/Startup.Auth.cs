using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace CloudSwyft.Web.Api
{
    public partial class Startup
    {

        public void ConfigureAuth(IAppBuilder app)
        {
           // app.UseCors(CorsOptions.AllowAll);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
            });
            //http://www.c-sharpcorner.com/UploadFile/736ca4/token-based-authentication-in-web-api-2/
            //http://stackoverflow.com/questions/38661090/token-based-authentication-in-web-api-without-any-user-interface
            //https://thompsonhomero.wordpress.com/2015/01/21/creating-a-clean-web-api-2-project-with-external-authentication-part-2/ !!
        }

    }
}
