using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using CloudSwyft.Web.Api.Models;
using MySql.Data.MySqlClient;
using System.Net.Http.Headers;
#pragma warning disable CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using CloudSwyft.Web.Api.Models;
#pragma warning restore CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Web.Configuration;
using System.Collections;
using System.Data.SqlClient;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/Course")]
    public class CourseController : ApiController
    {

        // Get api/Course/Courses



     
        //[HttpGet]
        //[Route("ProvisionVMs")]
        //public async Task<IHttpActionResult> ProvisionVMs(int courseID)
        //{
        //    string debugMessage = "";
        //    VEProfile veProfile = db.VEProfiles.Include(vp => vp.VirtualEnvironment).FirstOrDefault(ve => ve.CourseID == courseID);
        //    debugMessage += "\nVE Profile ID: " + veProfile.VEProfileID.ToString();
        //    UserController userController = new UserController();
        //    userController.Configuration = new HttpConfiguration();
        //    userController.Request = new HttpRequestMessage();
        //    VEProfilesController veProfilesController = new VEProfilesController();

        //    if (veProfile == null)
        //        return NotFound();

        //    VEType veType = db.VETypes.SingleOrDefault(vt => vt.VETypeID == veProfile.VirtualEnvironment.VETypeID);

        //    HttpResponseMessage actionResult = await userController.GetUsersByCourse(veProfile.CourseID, veProfile.VEProfileID)
        //        .ExecuteAsync(new CancellationToken());

        //    string getUsersResponse = await actionResult.Content.ReadAsStringAsync();
            
        //    List<User> userList = JsonConvert.DeserializeObject<List<User>>(getUsersResponse);
        //    debugMessage += "\nVE Type ID: " + veType.VETypeID.ToString();

        //    /*
        //    if (veType.VETypeID == 3)
        //        veProfilesController.ProvisionWebPlatform(veProfile, userList);
        //    else
        //        await veProfilesController.ProvisionUsers(veProfile.VEProfileID);
        //        */

        //    if (veType.VETypeID != 3)
        //        await veProfilesController.ProvisionUsers(veProfile.VEProfileID,null);

        //    return Ok("Virtual machines queued for provisioning." + debugMessage);
        //}


        private async Task<string> ApiCall(string method, string baseAddress, string url, string data = "")
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;

            if (method == "POST")
            {
                response = await client.PostAsync(url, new StringContent(data));
            }
            else if (method == "GET")
            {
                response = await client.GetAsync(url);
            }
            else if (method == "DELETE")
            {
                response = await client.DeleteAsync(url);
            }

            return await response.Content.ReadAsStringAsync();

        }


        //[HttpGet]
        //[Route("GetCoursesByUserId")]
        ////[AllowAnonymous] 
        //public HttpResponseMessage GetCoursesByUserId(int userId = 0)
        //{
        //    var coursesList = new ArrayList();

        //    try
        //    {
        //        var dbCommand = new SqlCommand("spGetCoursesByUserId", _dbCon)
        //        {
        //            CommandType = CommandType.StoredProcedure
        //        };

        //        dbCommand.Parameters.Add(new SqlParameter("userId", userId));

        //        _dbCon.Open();

        //        var dbReader = dbCommand.ExecuteReader();

        //        while (dbReader.Read())
        //        {
        //            var dataRow = new Hashtable();
        //            for (var i = 0; i < dbReader.FieldCount; i++)
        //            {
        //                dataRow.Add(dbReader.GetName(i), dbReader[dbReader.GetName(i)]);
        //            }
        //            coursesList.Add(dataRow);
        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, coursesList);
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
        //    }
        //    finally
        //    {
        //        _dbCon.Close();
        //    }
        //}

    }
}
