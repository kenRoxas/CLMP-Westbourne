using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace CloudSwyft.CloudLabs.Helpers
{
    public class Globals
    {
        static string tokenCookieName = WebConfigurationManager.AppSettings["TokenCookieName"];

        public static string TokenCookieName
        {
            get { return Globals.tokenCookieName; }
        }
    }
}
