using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Security.Principal;
using System.ComponentModel.DataAnnotations;
using Microsoft.Owin.Security.OAuth;
using System.Web;
using System.Net.Http;
using System;
using CloudSwyft.CloudLabs.Models;
using CloudSwyft.CloudLabs.Controllers;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace CloudSwyft.CloudLabs.Models
{
    public class OpenEdxToken
    {
        public string access_token { get; set; }

        public string token_type { get; set; }

        public string expires_in { get; set; }
    }
    public class Constants
    {
        public static string HOSTNAME = "http://localhost:56057/Account/Login?ReturnUrl=%2F";
        public static string KEY = "lpoxa";
        public static string SECRET = "123456";
        public static string AUTH_PATH = "/learn/api/public/v1/oauth2/token";

        public static string HOSTNAME_CS = "http://localhost:56057/Account/Login?ReturnUrl=%2F";
        // public static string KEY_CS = WebConfigurationManager.AppSettings["BBClient"];
        public static string SECRET_CS = "123456";
        //public static string AUTH_PATH_CS = "/learn/api/public/v1/oauth2/token";


    }

    public class OpenEdxAuthorizer
    {
        //HttpClient client;
        //OpenEdxToken token { get; set; }

        //public async Task<OpenEdxToken> Authorize(bool cs = false, string csClientId = "")
        //{
            //    string hostname = Constants.HOSTNAME, auth_path = Constants.AUTH_PATH, key = Constants.KEY, secret = Constants.SECRET;

            //    if (cs)
            //    {
            //        hostname = Constants.HOSTNAME_CS;
            //        auth_path = "";
            //        key = csClientId;
            //        secret = Constants.SECRET_CS;
            //    }

            //    var authData = string.Format("{0}:{1}", key, secret);
            //    Console.WriteLine("authData: {0}", authData);
            //    var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));
            //    Console.WriteLine("authHeaderValue: {0}", authHeaderValue);

            //    client = new HttpClient();

            //    var endpoint = new Uri(hostname + auth_path);

            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

            //    var postData = new List<KeyValuePair<string, string>>();
            //    postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));


            //    HttpContent body = new FormUrlEncodedContent(postData);

            //    Console.WriteLine("body: {0}", body.ToString());

            //    HttpResponseMessage response;
            //    try
            //    {
            //        response = await client.PostAsync(endpoint, body).ConfigureAwait(false);
            //        string token1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            //        return token1;
            //        if (response.IsSuccessStatusCode)
            //        {
            //            var content = await response.Content.ReadAsStringAsync();
            //            token = JsonConvert.DeserializeObject<OpenEdxToken>(content);
            //            Console.WriteLine("Authorize() Token: " + token.ToString());
            //        }
            //        else
            //        {
            //            Console.WriteLine(response.ToString());
            //            response.EnsureSuccessStatusCode();
            //        }


            //    }
            //    catch (AggregateException agex)
            //    {
            //        Console.WriteLine(@"ERROR {0}\n{1}", agex.Message, agex.InnerException.Message);
            //        throw new AggregateException(agex.Message);
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine(@"ERROR {0}", ex.Message);
            //        throw new Exception(ex.Message);
            //    }

            //    return token;
            
        //}
    }

}