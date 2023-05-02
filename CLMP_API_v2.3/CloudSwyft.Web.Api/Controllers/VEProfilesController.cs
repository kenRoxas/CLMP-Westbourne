using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
#pragma warning disable CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using CloudSwyft.Web.Api.Models;
#pragma warning restore CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using System.Web.Http.Cors;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Drawing;
using Microsoft.ServiceBus.Messaging;
using System.Web.Configuration;
using System.Web.Http.Hosting;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;
using Microsoft.Ajax.Utilities;
#pragma warning disable CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using CloudSwyft.Web.Api.Models;
#pragma warning restore CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
using Newtonsoft.Json.Linq;
using System.Collections;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MySql.Data.MySqlClient;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/VEProfiles")]
    public class VEProfilesController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private VirtualEnvironmentDBTenantContext _dbTenant = new VirtualEnvironmentDBTenantContext();


        private string OrganizationUnit = WebConfigurationManager.AppSettings["OrganizationUnit"];
        // GET: api/VEProfiles
        public VEProfileViewModel2 GetVEProfiles(string q = "", int pageSize = 0, int activePage = 1, int courseId = 0)
        {
            VEProfileViewModel2 vepVM = new VEProfileViewModel2();

            if (String.IsNullOrWhiteSpace(q) || String.IsNullOrEmpty(q))
            {
                q = "";
            }

            var query = _db.VEProfiles
                .Include(vep => vep.VirtualEnvironment)
                .Where(vep => vep.Name.Contains(q))
                .OrderBy(vep => vep.Name)
                .ToList();

            if (pageSize == 0 && query.Count == 0)
            {
                pageSize = 10;
            }
            else if (pageSize == 0 && query.Count > 0)
            {
                pageSize = query.Count;
            }

            for (int x = 0; x < query.Count; x++)
            {
                //if (!string.IsNullOrEmpty(query[x].ThumbnailURL)) query[x].ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
                //                                        ":" + Url.Request.RequestUri.Port +
                //                                        HttpContext.Current.Request.ApplicationPath +
                //                                        "/" + query[x].ThumbnailURL;

                //if (!String.IsNullOrEmpty(query[x].VirtualEnvironment.ThumbnailURL)
                //    && query[x].VirtualEnvironment.ThumbnailURL.IndexOf(Url.Request.RequestUri.Host) < 0)
                //{
                //    //query[x].VirtualEnvironment.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
                //    //                                    ":" + Url.Request.RequestUri.Port +
                //    //                                    "/" + query[x].VirtualEnvironment.ThumbnailURL;
                //}
            }

            vepVM.SearchFilter = q ?? "";
            vepVM.PageSize = pageSize;
            vepVM.ActivePage = activePage;
            vepVM.VEProfiles = query
                .Skip((activePage * pageSize) - pageSize)
                .Take(pageSize)
                .ToList();
            vepVM.TotalPages = query.Count / pageSize;
            if ((query.Count % pageSize) > 0)
                vepVM.TotalPages++;
            vepVM.TotalItems = query.Count;

            return vepVM;
        }

//        [HttpGet]
//        [Route("GetProvisionedVeprofilesByUser")]
//        [AllowAnonymous]
//        public HttpResponseMessage GetProvisionedVeprofilesByUser(int userId = 0)
//        {
//            var coursesList = new List<CourseDetails>();
//            var getStatusURL = "https://development-api.cloudswyft.com/labs/virtualmachine?resourceid=";
//            try
//            {
//                //var coursesList = _db.CloudLabsSchedule
//                //    .Join(_db.VEProfiles,
//                //    a => new { VEProfileId = a.VEProfileID, UserId = a.UserId },
//                //    b => new { VEProfileId = b.VEProfileID, UserId = userId },
//                //    (a, b) => new { cls = a, vp = b })
//                //    .Join(_db.VirtualMachines,
//                //    c => new { VEProfileId = c.cls.VEProfileID, UserId = userId },
//                //    d => new { VEProfileId = d.VEProfileID, UserId = d.UserID },
//                //    (c, d) => new { clsvp = c, vm = d })
//                //    .Join(_db.VirtualMachineMappings,
//                //    e => new { e.clsvp.cls.VEProfileID, UserId = userId },
//                //    f => new { f.VEProfileID, UserId = f.UserID },
//                //    (e, f) => new { clsvpvmm = e, vmm = f })
//                //    .Join(_db.VirtualEnvironments,
//                //    g => g.clsvpvmm.clsvp.vp.VirtualEnvironmentID,
//                //    h => h.VirtualEnvironmentID,
//                //    (g, h) => new { g, h })
//                //    .Join(_db.GuacamoleInstances,
//                //    i => i.g.clsvpvmm.vm.VirtualMachineID,
//                //    j => j.VirtualMachineID,
//                //    (i, j) => new { i, j })
//                //    .Select(x => new CourseDetails
//                //    {
//                //        LabHoursRemaining = x.i.g.clsvpvmm.clsvp.cls.LabHoursRemaining,
//                //        veprofileid = x.i.g.clsvpvmm.clsvp.cls.VEProfileID,
//                //        Name = x.i.g.clsvpvmm.clsvp.vp.Name,
//                //        RoleName = x.i.g.clsvpvmm.vm.RoleName,
//                //        MachineName = x.i.g.vmm.VMName,
//                //        UserId = userId,
//                //        IsStarted = x.i.g.clsvpvmm.vm.IsStarted,
//                //        Thumbnail = x.i.g.clsvpvmm.clsvp.vp.ThumbnailURL,
//                //        CourseCode = x.i.h.Description,
//                //        VEType = x.i.h.VETypeID,
//                //        GuacamoleUrl = x.j.Url
//                //    }).ToList();

//                var veProfileIds = _db.VirtualEnvironments.Where(q => q.VETypeID == 6 || q.VETypeID == 7) //for consoles
//                    .Join(_db.VEProfiles,
//                    a => a.VirtualEnvironmentID,
//                    b => b.VirtualEnvironmentID,
//                    (a, b) => new { a, b })
//                    .Join(_db.VEProfileLabCreditMappings,
//                    c => c.b.VEProfileID,
//                    d => d.VEProfileID,
//                    (c, d) => new { c, d })
//                    .Where(q => q.d.GroupID == _db.CloudLabUsers.Where(x => x.UserId == userId).FirstOrDefault().UserGroup)
//                    .Select(s => new { s.c.b.VEProfileID, s.c.b.Name, s.c.a.VETypeID, s.c.b.ThumbnailURL }).ToList();

//                var vmUsers = _db.CourseGrants.Where(q => q.VEType != 6 || q.VEType != 7)
//                    .Join(_db.CloudLabUsers,
//                    a => a.UserID,
//                    b => b.UserId,
//                    (a, b) => new { a,b })
//                    .Join(_db.VEProfiles,
//                    c => c.a.VEProfileID,
//                    d => d.VEProfileID,
//                    (c,d) => new { c,d}).Where(x=> x.c.a.UserID == userId)
//                    .Select(s => new { s.d.VEProfileID, s.d.Name, s.c.a.VEType, s.d.ThumbnailURL }).ToList();

//                foreach (var item in veProfileIds)
//                {
//                    if (item.VETypeID == 6)
//                    {
//                        var consoleDGrants = _db.CourseGrants.Where(q => (q.UserID == userId) && (q.VEProfileID == item.VEProfileID)).FirstOrDefault();
//                        var consoleDSchedules = _db.ConsoleSchedules.Where(q => (q.UserId == userId) && (q.VEProfileId == item.VEProfileID)).FirstOrDefault();
//                        var GetStudentConsole = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/";
//                        var url = "dev/get_current_budget_of_student/";

//                        var BLimit = new BudgetLimit
//                        {
//                            Amount = "0",
//                            Unit = "",
//                        };

//                        var ASpend = new DataSpend
//                        {
//                            ActualSpend = new ActualSpend { Amount = "0", Unit = "" }
//                        };

//                        if (consoleDSchedules != null)
//                        {
//                            var response = ApiCall("GET", GetStudentConsole, url + consoleDSchedules.AccountId, null);
//                            var dataJSON = JsonConvert.DeserializeObject<ConsoleDetail>(response.Result);

//                            if (consoleDGrants.IsCourseGranted == true)
//                            {

//                                var result2 = new CourseDetails
//                                {
//                                    UserId = consoleDGrants.UserID,
//                                    Name = item.Name,
//                                    veprofileid = consoleDGrants.VEProfileID,
//                                    VEType = consoleDGrants.VEType,
//                                    IsCourseGranted = consoleDGrants.IsCourseGranted,
//                                    IsProvisioned = consoleDSchedules.IsProvisioned,
//                                    ConsoleLink = consoleDSchedules.ConsoleLink,
//                                    Thumbnail = item.ThumbnailURL,
//                                    Data_transfer_budget_limit = dataJSON.Data_transfer_budget_limit ?? BLimit,
//                                    Cost_budget_limit = dataJSON.Cost_budget_limit ?? BLimit,
//                                    Actual_data_transfer_spend = dataJSON.Actual_data_transfer_spend ?? ASpend,
//                                    Actual_costs_spend = dataJSON.Actual_costs_spend ?? ASpend,
//                                    Is_suspended = dataJSON.Is_suspended
//                                };

//                                coursesList.Add(result2);
//                            }
//                        }
//                        else
//                        {
//                            if (consoleDGrants != null && consoleDGrants.IsCourseGranted == true)
//                            {
//                                var result1 = new CourseDetails
//                                {
//                                    UserId = consoleDGrants.UserID,
//                                    Name = item.Name,
//                                    veprofileid = consoleDGrants.VEProfileID,
//                                    VEType = consoleDGrants.VEType,
//                                    IsCourseGranted = consoleDGrants.IsCourseGranted,
//                                    IsProvisioned = 0,
//                                    ConsoleLink = ""
//                                };

//                                coursesList.Add(result1);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        var consoleCourse = new CourseDetails();

//                        if (_db.ConsoleSchedules.Any(q => q.UserId == userId && q.VEProfileId == item.VEProfileID))
//                        {
//                            var course = _db.ConsoleSchedules.Where(q => q.UserId == userId && q.VEProfileId == item.VEProfileID).FirstOrDefault();
//                            consoleCourse.IsProvisioned = course.IsProvisioned;
//                            consoleCourse.veprofileid = item.VEProfileID;
//                            consoleCourse.UserId = userId;
//                            consoleCourse.Name = item.Name;
//                            consoleCourse.VEType = item.VETypeID; //console
//                            consoleCourse.IsStarted = null;
//                            consoleCourse.LabHoursRemaining = null;
//                            consoleCourse.ConsoleLink = course.ConsoleLink;
//                            consoleCourse.Thumbnail = item.ThumbnailURL;
//                        }
//                        else
//                        {
//                            consoleCourse.Name = item.Name;
//                            consoleCourse.veprofileid = item.VEProfileID;
//                            consoleCourse.UserId = userId;
//                            consoleCourse.Name = item.Name;
//                            consoleCourse.VEType = item.VETypeID; //console
//                            consoleCourse.IsStarted = null;
//                            consoleCourse.LabHoursRemaining = null;
//                            consoleCourse.Thumbnail = item.ThumbnailURL;
//                        }

//                        coursesList.Add(consoleCourse);
//                    }
//                }
//                foreach (var item in vmUsers)
//                {
//                        var consoleCourse = new CourseDetails();

//                        if (_db.ConsoleSchedules.Any(q => q.UserId == userId && q.VEProfileId == item.VEProfileID))
//                        {
//                            var course = _db.ConsoleSchedules.Where(q => q.UserId == userId && q.VEProfileId == item.VEProfileID).FirstOrDefault();
//                            consoleCourse.IsProvisioned = course.IsProvisioned;
//                            consoleCourse.veprofileid = item.VEProfileID;
//                            consoleCourse.UserId = userId;
//                            consoleCourse.Name = item.Name;
//                            consoleCourse.VEType = item.VEType; //console
//                            consoleCourse.IsStarted = null;
//                            consoleCourse.LabHoursRemaining = null;
//                            consoleCourse.ConsoleLink = course.ConsoleLink;
//                            consoleCourse.Thumbnail = item.ThumbnailURL;
//                        }
//                        else
//                        {
//                            consoleCourse.Name = item.Name;
//                            consoleCourse.veprofileid = item.VEProfileID;
//                            consoleCourse.UserId = userId;
//                            consoleCourse.Name = item.Name;
//                            consoleCourse.VEType = item.VEType; //console
//                            consoleCourse.IsStarted = null;
//                            consoleCourse.LabHoursRemaining = null;
//                            consoleCourse.Thumbnail = item.ThumbnailURL;
//                        }

//                        coursesList.Add(consoleCourse);
//                }
////                coursesList[0].
//                foreach (var item in coursesList)
//                {
//                    if(_db.VirtualMachineMappings.Any(q => q.VEProfileID == item.veprofileid && q.UserID == item.UserId && q.IsProvisioned == 0))
//                    {
//                        var resourceId = _db.VirtualMachineMappings.Where(q => q.VEProfileID == item.veprofileid && q.UserID == item.UserId).FirstOrDefault().ResourceID;
//                        var response = ApiCall("GET", getStatusURL, resourceId, null);
//                        VMSuccess dataJson = new VMSuccess();
//                        if (response.Result.Contains("computerName"))
//                        {
//                            //create VirtualMachineMappings
//                            dataJson = JsonConvert.DeserializeObject<VMSuccess>(response.Result);
//                            var vmm = _db.VirtualMachineMappings.Where(q => q.VEProfileID == item.veprofileid && q.UserID == item.UserId && q.IsProvisioned == 0).FirstOrDefault();
//                            vmm.IsProvisioned = 1;
//                            vmm.VMName = dataJson.computerName;
//                            vmm.Status = dataJson.status;
//                        }
//                    }
//                }

//                return Request.CreateResponse(HttpStatusCode.OK, coursesList);
//            }
//            catch (Exception ex)
//            {
//                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
//            }
//            finally
//            {
//                _dbCon.Close();
//            }
//        }

        //[HttpGet]
        //[Route("ByCourse")]
        //public IHttpActionResult ByCourse(int courseID)
        //{
        //    var query = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).ToList();

        //    var resultVEProfile = query.SingleOrDefault(ve => ve.CourseID == courseID);

        //    if (resultVEProfile == null) return BadRequest("VE Profile of course ID " + courseID.ToString() + " not found.");

        //    if (!string.IsNullOrEmpty(resultVEProfile.ThumbnailURL)) resultVEProfile.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
        //                                                ":" + Url.Request.RequestUri.Port +
        //                                                "/" + resultVEProfile.ThumbnailURL;

        //    resultVEProfile.VirtualEnvironment.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
        //                                            ":" + Url.Request.RequestUri.Port +
        //                                            "/" + resultVEProfile.VirtualEnvironment.ThumbnailURL;

        //    return Ok(resultVEProfile);
        //}

        //[HttpGet]
        //[Route("ChangeConnectionLimit")]
        //public IHttpActionResult ChangeConnectionLimit(int veProfileID, int connectionLimit)
        //{
        //    var veProfile = _db.VEProfiles.SingleOrDefault(vp => vp.VEProfileID == veProfileID);

        //    veProfile.ConnectionLimit = connectionLimit;

        //    _db.Entry(veProfile).State = EntityState.Modified;
        //    _db.SaveChanges();

        //    return Ok();
        //}

        [HttpPost]
        [Route("UploadThumbnail")]
        public async Task<IHttpActionResult> UploadThumbnail(int veProfileID)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return InternalServerError(
                    new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
            }

            VEProfile veProfile = _db.VEProfiles.Find(veProfileID);

            if (veProfile == null)
            {
                return NotFound();
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/" + veProfileID.ToString());

                    if (!Directory.Exists(dirPath))
                    {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }

                    var dataStream = await file.ReadAsStreamAsync();

                    Image image = Image.FromStream(dataStream);
                    image = (Image)(new Bitmap(image, new Size(650, 389)));
                    String fileName = veProfileID.ToString() + "-" + DateTime.Now.ToString("MMDDYYhhmmss") + "-Thumbnail.jpg";
                    String fullPath = Path.Combine(dirPath, fileName);

                    if (File.Exists(fullPath)) File.Delete(fullPath);

                    image.Save(fullPath);

                    veProfile.ThumbnailURL = "Content/Images/" + veProfileID.ToString() + "/" + fileName;
                    _db.Entry(veProfile).State = EntityState.Modified;
                    _db.SaveChanges();

                    return Ok("Successful upload");
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Ok();
        }

        [HttpPost]
        [Route("AddLabActivities")]
        public IHttpActionResult AddLabActivities(VEProfileAddLabActivity addLabActivity)
        {
            if (!VEProfileExists(addLabActivity.VEProfileID))
            {
                return NotFound();
            }

            int currentLabActivityID;

            for (int x = 0; x < addLabActivity.LabActivities.Count; x++)
            {
                currentLabActivityID = addLabActivity.LabActivities[x].LabActivityID;

                if (_db.LabActivities.Count(e => e.LabActivityID == currentLabActivityID) < 1)
                {
                    return NotFound();
                }
                else
                {
                    var occurCount = _db.Database.ExecuteSqlCommand("SELECT COUNT(*) FROM dbo.VEProfileLabActivities WHERE VEProfileID = " + addLabActivity.VEProfileID.ToString() + " AND LabActivityID = " + currentLabActivityID.ToString());

                    if (occurCount < 1)
                    {
                        _db.Database.ExecuteSqlCommand(
                            "INSERT INTO dbo.VEProfileLabActivities (VEProfileID, LabActivityID) "
                            + "VALUES (" + addLabActivity.VEProfileID.ToString() + ',' + currentLabActivityID.ToString() + ")");
                    }

                }
            }

            return Ok();
        }

        [HttpPut]
        [Route("UpdateLabActivities")]
        public IHttpActionResult UpdateLabActivities(VEProfileAddLabActivity addLabActivity)
        {
            if (!VEProfileExists(addLabActivity.VEProfileID))
            {
                return NotFound();
            }

            _db.Database.ExecuteSqlCommand(
                        "DELETE FROM dbo.VEProfileLabActivities "
                        + "WHERE VEProfileID =" + addLabActivity.VEProfileID.ToString());

            int currentLabActivityID;

            for (int x = 0; x < addLabActivity.LabActivities.Count; x++)
            {
                currentLabActivityID = addLabActivity.LabActivities[x].LabActivityID;

                if (_db.LabActivities.Count(e => e.LabActivityID == currentLabActivityID) < 1)
                {
                    return NotFound();
                }
                else
                {
                    var occurCount = _db.Database.ExecuteSqlCommand("SELECT COUNT(*) FROM dbo.VEProfileLabActivities WHERE VEProfileID = " + addLabActivity.VEProfileID.ToString() + " AND LabActivityID = " + currentLabActivityID.ToString());

                    if (occurCount < 1)
                    {
                        _db.Database.ExecuteSqlCommand(
                            "INSERT INTO dbo.VEProfileLabActivities (VEProfileID, LabActivityID) "
                            + "VALUES (" + addLabActivity.VEProfileID.ToString() + ',' + currentLabActivityID.ToString() + ")");
                    }

                }
            }

            return Ok();
        }

        [HttpDelete]
        [Route("RemoveLabActivity")]
        public IHttpActionResult RemoveLabActivity(int veProfileID, int labActivityID)
        {
            if (!VEProfileExists(veProfileID))
            {
                return NotFound();
            }

            _db.Database.ExecuteSqlCommand(
                "DELETE FROM dbo.VEProfileLabActivities "
                + "WHERE VEProfileID=" + veProfileID.ToString() + " AND LabActivityID=" + labActivityID.ToString());

            return Ok();
        }




        /*
        [Route("ProvisionWebPlatform")]
        public void ProvisionWebPlatform(VEProfile veProfile, List<User> userList)
        {
            if (veProfile == null || veProfile.CourseID < 1)
                return;

            CloudProvider cloudProvider = db.CloudProviders.SingleOrDefault
                (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

            VirtualEnvironmentImages veImages =
                db.VirtualEnvironmentImages.SingleOrDefault(
                    vi => vi.VirtualEnvironmentID == veProfile.VirtualEnvironment.VirtualEnvironmentID);

            WebPlatform webPlatform = db.WebPlatforms.SingleOrDefault(wp => wp.Name == veImages.Name);
            List<VirtualMachine> veProfileVMs = db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

            // Delete unused VMs
            List<VirtualMachine> deleteVMs = new List<VirtualMachine>();
            List<int> userIDList = new List<int>();

            for (var x = 0; x < userList.Count; x++)
                userIDList.Add(userList[x].UserId);

            deleteVMs = veProfileVMs.Where(vm => !userIDList.Contains(vm.UserID)).ToList();
            DeleteVMs(deleteVMs, cloudProvider);

            // Remove users with VMs already provisioned
            List<User> provisionUsersList = new List<User>();
            foreach (User user in userList)
                if (!veProfileVMs.Exists(vm => vm.UserID == user.UserId))
                    provisionUsersList.Add(user);

            foreach (User userRecord in provisionUsersList)
            {
                VirtualMachine virtualMachine = new VirtualMachine();
                virtualMachine.CourseID = veProfile.CourseID;
                virtualMachine.d = 1;
                virtualMachine.ServiceName = "Web";
                virtualMachine.LastTimeStamp = DateTime.Now;
                virtualMachine.UserID = userRecord.UserId;
                virtualMachine.VEProfileID = veProfile.VEProfileID;
                virtualMachine.RoleName = veProfile.CourseID + "-" + veProfile.VEProfileID + "-" + userRecord.UserId;
                db.VirtualMachines.Add(virtualMachine);
            }
            db.SaveChanges();

            GuacamoleInstance guacInstance = db.GuacamoleInstances.SingleOrDefault(gi => gi.Url == webPlatform.BaseURL);
            if (guacInstance == null)
            {
                guacInstance = new GuacamoleInstance();
                guacInstance.Connection_Name = webPlatform.Name;
                guacInstance.Hostname = webPlatform.Name;
                guacInstance.Url = webPlatform.BaseURL;
                db.GuacamoleInstances.Add(guacInstance);
                db.SaveChanges();
            }
        }
        */

        //[HttpPost]
        //[Route("ProvisionUsers")]
        //public async Task<IHttpActionResult> ProvisionUsers(int veProfileID, List<User> bbUsers)
        //{
        //    string returnMessage = "Virtual machines successfully provisioned. ";
        //    VEProfile veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == veProfileID);
        //    if (veProfile == null)
        //        return Ok("VE Profile does not exist");

        //    List<VirtualEnvironmentImages> veImages = _db.VirtualEnvironmentImages.Where(vi => vi.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).ToList();

        //    CloudProvider cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    UserController userController = new UserController();
        //    userController.Configuration = new HttpConfiguration();
        //    userController.Request = new HttpRequestMessage();

        //    if (veProfile == null || veProfile.CourseID < 1) return BadRequest("VE Profile is not yet attached to any course.");

        //    var userList = new List<User>();
        //    if (bbUsers != null && bbUsers[0].UserId != 0)
        //        bbUsers.CopyItemsTo(userList);
        //    else
        //    {
        //        HttpResponseMessage actionResult = await userController.GetUsersByCourse(veProfile.CourseID, veProfileID)
        //            .ExecuteAsync(new CancellationToken());

        //        string getUsersResponse = await actionResult.Content.ReadAsStringAsync();

        //        userList = getUsersResponse.IndexOf("Data is Null") < 0 ? JsonConvert.DeserializeObject<List<User>>(getUsersResponse) : new List<User>();
        //    }
        //    List<VirtualMachine> veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //    // Delete unused VMs
        //    List<VirtualMachine> deleteVMs = new List<VirtualMachine>();

        //    List<int> userIDList = userList.Select(t => t.UserId).ToList();

        //    deleteVMs = veProfileVMs.Where(vm => !userIDList.Contains(vm.UserID)).ToList();

        //    DeleteVMs(deleteVMs, cloudProvider);

        //    // Remove users with VMs already provisioned
        //    List<User> provisionUsersList = userList.Where(user => !veProfileVMs.Exists(vm => vm.UserID == user.UserId)).ToList();

        //    //Provision Multiple or Single Images
        //    int count = 1;
        //    foreach (VirtualEnvironmentImages veImage in veImages)
        //    {
        //        string suffix = string.Empty;
        //        if (veImages.Count > 1)
        //            suffix = count.ToString();

        //        returnMessage += ProvisionVMs(100, 100, 100, veProfile, provisionUsersList, veImage.Name, veImage.Size, suffix, veImage.Type, veImage.Protocol, 0, false);
        //        count++;
        //    }

        //    //include update time of message sent then check if time is within 5 mins
        //    if (_vmOperationMultiple.Count > 0 && veProfile.DateProvisionTrigger.AddMinutes(5) <= DateTime.Now)
        //    {
        //        SendProvisionMessage(cloudProvider.Name);
        //        //update DateProvisionTrigger in DB after sending provision message
        //        UpdateProvisionDate(veProfile);
        //    }
        //    else
        //    {
        //        returnMessage = "VM Provisioning was not triggered. VM Count: " + _vmOperationMultiple.Count.ToString();
        //    }

        //    return Ok(returnMessage);
        //}

        //[HttpPost]
        //[Route("ProvisionCustomVe")]
        //public IHttpActionResult ProvisionCustomVe(int veProfileId, int userId)
        //{
        //    var returnMessage = "Virtual machines successfully provisioned. ";
        //    var veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == veProfileId);
        //    if (veProfile == null)
        //        return Ok("VE Profile does not exist");

        //    var veImages = _db.VirtualEnvironmentImages.Where(vi => vi.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).ToList();

        //    var cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    var userList = new List<User>()
        //    {
        //        new User() {UserId = userId}
        //    };

        //    var veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //    // Remove users with VMs already provisioned
        //    var provisionUsersList = userList.Where(user => veProfileVMs.Exists(vm => vm.UserID == user.UserId)).ToList();

        //    var tenantId = _db.CloudLabUsers.SingleOrDefault(cl => cl.UserId == userId);

        //    //returnMessage += ProvisionVMs(veProfile, provisionUsersList, veImages[0].Name, veImages[0].Size, "", veImages[0].Type, veImages[0].Protocol, tenantId.TenantId);
        //    returnMessage += ProvisionVMs(100, 100, 100, veProfile, provisionUsersList, veImages[0].Name, veImages[0].Size, "", veImages[0].Type, veImages[0].Protocol, tenantId.TenantId, false);

        //    //include update time of message sent then check if time is within 5 mins
        //    if (_vmOperationMultiple.Count > 0 && veProfile.DateProvisionTrigger.AddMinutes(5) <= DateTime.Now)
        //    {
        //        SendProvisionMessage(cloudProvider.Name);
        //        //update DateProvisionTrigger in DB after sending provision message
        //        UpdateProvisionDate(veProfile);

        //        //CloudLabsSchedule cls = new CloudLabsSchedule();
        //        //cls.VEProfileID = veProfile.VEProfileID; cls.UserId = userId; cls.ScheduledBy = "Admin"; cls.DateCreated = DateTime.Now;

        //        //ScheduleUsers(cls);
        //    }
        //    else
        //    {
        //        returnMessage = string.Format("VM Provisioning was not triggered. VM Count: {0}", _vmOperationMultiple.Count);
        //    }


        //    return Ok(returnMessage);
        //}

        [HttpPost]
        [Route("ScheduleUsers")]
        public IHttpActionResult ScheduleUsers(CloudLabsSchedule data)
        {

            _db.CloudLabsSchedule.Add(data);
            _db.SaveChanges();
            return Ok();
        }

        //[HttpPost]
        //[Route("CaptureCustomVe")]
        //public IHttpActionResult CaptureCustomVe(int veProfileId, string imageName)
        //{
        //    string returnMessage = "Virtual machines successfully captured.";
        //    var veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == veProfileId);
        //    var veType = _db.VETypes.SingleOrDefault(vt => vt.VETypeID == veProfile.VirtualEnvironment.VETypeID);
        //    var prefix = _db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0];

        //    var cloudProvider = _db.CloudProviders.SingleOrDefault(cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    #region for multi-tenancy provisioning
        //    _conn.Open();
        //    var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND GuacamoleUrl is not null", prefix), _conn);
        //    var azurePort = string.Empty;
        //    var guacConnection = string.Empty;
        //    var apiUrl = string.Empty;
        //    var guacUrl = string.Empty;
        //    var region = string.Empty;
        //    using (var sqlReader = sqlCommand.ExecuteReader())
        //    {
        //        while (sqlReader.Read())
        //        {
        //            azurePort = sqlReader["AzurePort"].ToString();
        //            guacConnection = sqlReader["GuacConnection"].ToString();
        //            apiUrl = sqlReader["ApiUrl"].ToString();
        //            guacUrl = sqlReader["GuacamoleUrl"].ToString();
        //            region = sqlReader["Region"].ToString();
        //        }
        //    }
        //    _conn.Close();
        //    #endregion

        //    var mapping = VirtualMachineMappingGenerator(prefix, veProfile.CourseID, 0, veProfile.VEProfileID);

        //    var vmOperation = new VMOperation
        //    {
        //        Operation = "Capture",
        //        ImageName = imageName,
        //        CourseID = 0,
        //        VEProfileID = veProfileId,
        //        UserID = 0,
        //        OSFamily = veType.Name,
        //        Prefix = prefix,
        //        AzurePort = azurePort,
        //        GuacConnection = guacConnection,
        //        WebApiUrl = apiUrl,
        //        GuacamoleUrl = guacUrl,
        //        Region = region,
        //        MachineName = mapping
        //    };

        //    _vmOperationMultiple.Add(vmOperation);

        //    if (_vmOperationMultiple.Count > 0 && veProfile.DateProvisionTrigger.AddMinutes(5) <= DateTime.Now)
        //    {
        //        SendProvisionMessage(cloudProvider.Name);
        //        //update DateProvisionTrigger in DB after sending provision message
        //        UpdateProvisionDate(veProfile);
        //    }
        //    else
        //    {
        //        returnMessage = "VM Provisioning was not triggered. VM Count: " + _vmOperationMultiple.Count.ToString();
        //    }

        //    return Ok(returnMessage);
        //}

        //private string ProvisionVMs(Int64 totalRemainingHours, int groupID, Int64 labHours, VEProfile veProfile, List<User> usersList, string veName, string veSize, string suffix, string imageType, string protocol, int tenantId = 0, bool isLaas = false, string VMSize = "")
        //{
        //    var roleNames = "";

        //    var veType = _db.VETypes.SingleOrDefault(vt => vt.VETypeID == veProfile.VirtualEnvironment.VETypeID);

        //    var sqlTenant = new SqlCommand(string.Format("SELECT Code, TenantName FROM Tenants WHERE TenantId = '{0}'", tenantId), _connTenant);

        //    var sqlCode = string.Empty;
        //    var TenantName = string.Empty;

        //    _connTenant.Open();
        //    using (var sqlReader = sqlTenant.ExecuteReader())
        //    {
        //        while (sqlReader.Read())
        //        {
        //            sqlCode = sqlReader["Code"].ToString();
        //            TenantName = sqlReader["TenantName"].ToString();
        //        }
        //    }

        //    var prefix = sqlCode;

        //    #region for multi-tenancy provisioning
        //    //_conn.Open();
        //    //var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND GuacamoleUrl is not null", prefix), _conn);
        //    var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl FROM tenants WHERE TenantId = '{0}' AND GuacamoleUrl is not null", tenantId), _connTenant);
        //    var azurePort = string.Empty;
        //    var guacConnection = string.Empty;
        //    var apiUrl = string.Empty;
        //    var guacUrl = string.Empty;
        //    var region = string.Empty;
        //    using (var sqlReader = sqlCommand.ExecuteReader())
        //    {
        //        while (sqlReader.Read())
        //        {
        //            azurePort = sqlReader["AzurePort"].ToString();
        //            guacConnection = sqlReader["GuacConnection"].ToString();
        //            apiUrl = sqlReader["ApiUrl"].ToString();
        //            guacUrl = sqlReader["GuacamoleUrl"].ToString();
        //            //region = sqlReader["Region"].ToString();
        //            region = "Southeast Asia";
        //        }
        //    }
        //    //_conn.Close();
        //    _connTenant.Close();
        //    #endregion

        //    #region Update Port  
        //    //_conn.Open();
        //    _connTenant.Open();
        //    if (Convert.ToInt32(azurePort) == 60000)
        //    {
        //        azurePort = "30000";
        //    }
        //    //using (
        //    //    sqlCommand =
        //    //        new SqlCommand(
        //    //            string.Format(
        //    //                "UPDATE TenantCodes SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //    //                prefix, (Convert.ToInt32(azurePort) + 3)), _conn))
        //    using (
        //        sqlCommand =
        //            new SqlCommand(
        //                string.Format(
        //                    "UPDATE tenants SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //                    prefix, (Convert.ToInt32(azurePort) + 3)), _connTenant))
        //    {
        //        sqlCommand.ExecuteNonQuery();
        //    }

        //    //_conn.Close();
        //    _connTenant.Close();
        //    #endregion


        //    foreach (var t in usersList)
        //    {
        //        string mapping;
        //        if (veType != null)
        //        {

        //            if (t.MachineName == null)
        //                mapping = VirtualMachineMappingGenerator(Rolenames(groupID, sqlCode).ToUpper(), veProfile.CourseID, t.UserId, veProfile.VEProfileID, (string.IsNullOrEmpty(suffix) ? 0 : Convert.ToInt32(suffix)), isLaas);
        //            else
        //            {
        //                mapping = t.MachineName.ToUpper();

        //                if (isLaas)
        //                {
        //                    var vmMapping = new VirtualMachineMappings
        //                    {
        //                        RoleName = mapping,
        //                        UserID = t.UserId,
        //                        VEProfileID = veProfile.VEProfileID,
        //                        CourseID = veProfile.CourseID,
        //                        MachineInstance = (string.IsNullOrEmpty(suffix) ? 0 : Convert.ToInt32(suffix)),
        //                        IsProvisioned = 1,
        //                        IsLaasProvisioned = 0
        //                    };
        //                    _db.VirtualMachineMappings.Add(vmMapping);
        //                    _db.SaveChanges();
        //                }
        //                else
        //                {
        //                    var vmMapping = new VirtualMachineMappings
        //                    {
        //                        RoleName = mapping,
        //                        UserID = t.UserId,
        //                        VEProfileID = veProfile.VEProfileID,
        //                        CourseID = veProfile.CourseID,
        //                        MachineInstance = (string.IsNullOrEmpty(suffix) ? 0 : Convert.ToInt32(suffix)),
        //                        IsProvisioned = 0,
        //                        IsLaasProvisioned = 1
        //                    };
        //                    _db.VirtualMachineMappings.Add(vmMapping);
        //                    _db.SaveChanges();
        //                }
        //            }

        //            var vmOperation = new VMOperation
        //            {
        //                Operation = "Provision",
        //                ImageName = veName,
        //                CourseID = veProfile.CourseID,
        //                VEProfileID = veProfile.VEProfileID,
        //                UserID = t.UserId,
        //                OSFamily = veType.Name,
        //                Prefix = prefix,
        //                Suffix = suffix,
        //                ImageSize = veSize,
        //                AzurePort = azurePort,
        //                GuacConnection = guacConnection,
        //                WebApiUrl = apiUrl,
        //                GuacamoleUrl = guacUrl,
        //                Region = region,
        //                ImageType = imageType,
        //                Protocol = protocol,
        //                MachineName = mapping,
        //                //added by Mau
        //                GroupID = groupID,
        //                CourseHours = labHours,
        //                ScheduledBy = "",
        //                ApiUrl = WebConfigurationManager.AppSettings["CSWebApiUrl"],
        //                ResourceGroup = ResourceGroupName(groupID, sqlCode).ToUpper(),
        //                TenantName = TenantName,
        //                IsLaasProvision = isLaas,
        //                VMSize = VMSize
        //            };

        //            _vmOperationMultiple.Add(vmOperation);

        //            azurePort = (Convert.ToInt32(azurePort) + 3).ToString();
        //        }

        //        if (suffix.Length > 0)
        //            roleNames += string.Format(", {0}-{1}-{2}-{3}-{4}", prefix, veProfile.CourseID, veProfile.VEProfileID, t.UserId, suffix);
        //        else
        //            roleNames += string.Format(", {0}-{1}-{2}-{3}", prefix, veProfile.CourseID, veProfile.VEProfileID, t.UserId);
        //    }

        //    #region Update Port
        //    //_conn.Open();
        //    _connTenant.Open();
        //    if (Convert.ToInt32(azurePort) == 60000)
        //    {
        //        azurePort = "30000";
        //    }
        //    //using (
        //    //    sqlCommand =
        //    //        new SqlCommand(
        //    //            string.Format(
        //    //                "UPDATE TenantCodes SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //    //                prefix, (Convert.ToInt32(azurePort) + 3)), _conn))
        //    using (
        //        sqlCommand =
        //            new SqlCommand(
        //                string.Format(
        //                    "UPDATE tenants SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //                    prefix, (Convert.ToInt32(azurePort) + 3)), _connTenant))
        //    {
        //        sqlCommand.ExecuteNonQuery();
        //    }
        //    _connTenant.Close();
        //    // _conn.Close();
        //    #endregion

        //    if (roleNames.Length > 0)
        //        roleNames = roleNames.Substring(2);

        //    return roleNames;
        //}

        private string ResourceGroupName(int GroupId, string TenantPrefix)
        {
            var GroupName = _db.CloudLabsGroups.Where(clg => clg.CloudLabsGroupID == GroupId).FirstOrDefault();

            return ("CS-" + TenantPrefix + '-' + GroupName.CLPrefix);

        }

        private string Rolenames(int GroupId, string TenantPrefix)
        {
            var GroupName = _db.CloudLabsGroups.Where(clg => clg.CloudLabsGroupID == GroupId).FirstOrDefault();

            return (TenantPrefix + '-' + GroupName.CLPrefix);

        }

        //private string VirtualMachineMappingGenerator(string rolename, int courseId, int userId, int veProfileId, int suffix = 0, bool isLaas = false)
        //{
        //    var mapping =
        //        _db.VirtualMachineMappings.SingleOrDefault(
        //            map =>
        //                map.CourseID == courseId && map.VEProfileID == veProfileId &&
        //                map.UserID == userId && map.MachineInstance == suffix);

        //    if (mapping != null)
        //        return mapping.RoleName;

        //    string result;

        //    while (true)
        //    {

        //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //        var random = new Random();
        //        var generatedValues = new string(
        //            Enumerable.Repeat(chars, 7)
        //                .Select(s => s[random.Next(s.Length)])
        //                .ToArray());
        //        result = string.Format("{0}-{1}", rolename, generatedValues);
        //        if (VirtualMachineMappingExists(result))
        //            break;
        //    }
        //    if (isLaas)
        //    {
        //        var vmMapping = new VirtualMachineMappings
        //        {
        //            RoleName = result,
        //            UserID = userId,
        //            VEProfileID = veProfileId,
        //            CourseID = courseId,
        //            MachineInstance = suffix,
        //            IsProvisioned = 1,
        //            IsLaasProvisioned = 0
        //        };

        //        _db.VirtualMachineMappings.Add(vmMapping);
        //        _db.SaveChanges();
        //    }
        //    else
        //    {
        //        var vmMapping = new VirtualMachineMappings
        //        {
        //            RoleName = result,
        //            UserID = userId,
        //            VEProfileID = veProfileId,
        //            CourseID = courseId,
        //            MachineInstance = suffix,
        //            IsProvisioned = 0,
        //            IsLaasProvisioned = 1
        //        };

        //        _db.VirtualMachineMappings.Add(vmMapping);
        //        _db.SaveChanges();
        //    }
        //    return result;
        //}

        //private string ContainerMappingGenerator(string CLPrefix, string title, string Id, int userId, int veProfileId, int courseId, int suffix)
        //{
        //    string result = "cs-" + CLPrefix.ToLower() + '-' + title.ToLower() + "-container-" + Id;

        //    if (!_db.VirtualMachineMappings.Any(q => q.RoleName == result))
        //    {
        //        var vmMapping = new VirtualMachineMappings
        //        {
        //            RoleName = result,
        //            UserID = userId,
        //            VEProfileID = veProfileId,
        //            CourseID = courseId,
        //            MachineInstance = suffix,
        //            IsProvisioned = 0,
        //            IsLaasProvisioned = 1
        //        };

        //        _db.VirtualMachineMappings.Add(vmMapping);
        //        _db.SaveChanges();
        //        return result;
        //    }
        //    else
        //        return result = null;
        //}

        //private bool VirtualMachineMappingExists(string roleName)
        //{
        //    return _db.VirtualMachineMappings.SingleOrDefault(e => e.RoleName == roleName) == null;
        //}

        //private void SendProvisionMessage(string imageName, string prefix, int courseID,
        //    int veProfileID, int userID, string osFamily, string cloudProviderName, string suffix)

        private void SendMessage(object message, string queueName)
        {

            string connectionString = WebConfigurationManager.AppSettings["QueueConnectionString"];
            QueueClient Client =
              QueueClient.CreateFromConnectionString(connectionString, queueName); //SamplQueue is a test Queue
            string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            BrokeredMessage sendMessage = new BrokeredMessage(jsonStr);

            Client.Send(sendMessage);

            //var storageAccount = CloudStorageAccount.Parse(WebConfigurationManager.AppSettings["QueueConnectionString"]);
            //var queueClient = storageAccount.CreateCloudQueueClient();
            //var queue = queueClient.GetQueueReference(queueName);
            //string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);


            //List<VMOperation> objectMessage = JsonConvert.DeserializeObject<List<VMOperation>>(jsonStr);

            //var s = objectMessage.Count();

            //var queueMessage = new CloudQueueMessage(jsonStr);
            //queue.AddMessage(queueMessage);
        }


        // GET: api/VEProfiles/5
        [ResponseType(typeof(VEProfile))]
        public IHttpActionResult GetVEProfile(int id)
        {
            VEProfile vEProfile = _db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault<VEProfile>(vp => vp.VEProfileID == id);

            if (!string.IsNullOrEmpty(vEProfile.ThumbnailURL)) vEProfile.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
                                                        ":" + Url.Request.RequestUri.Port +
                                                        HttpContext.Current.Request.ApplicationPath +
                                                        "/" + vEProfile.ThumbnailURL;
            /*
            vEProfile.VirtualEnvironment.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
                                                    ":" + Url.Request.RequestUri.Port +
                                                    "/" + vEProfile.VirtualEnvironment.ThumbnailURL;
            */

            if (vEProfile == null)
            {
                return NotFound();
            }

            return Ok(vEProfile);
        }

        //// PUT: api/VEProfiles/5
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutVEProfile()
        //{
        //    string root = HttpContext.Current.Server.MapPath("~/");
        //    var provider = new MultipartFormDataStreamProvider(root);

        //    VEProfile veProfile;

        //    if (Request.Content.IsMimeMultipartContent())
        //    {
        //        // Read the form data and return an async task.
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        int veProfileID = Convert.ToInt32(provider.FormData.GetValues("VEProfileID").First());

        //        veProfile = _db.VEProfiles.Find(veProfileID);

        //        veProfile.Name = provider.FormData.GetValues("Name").First();
        //        veProfile.Description = provider.FormData.GetValues("Description").First();
        //        veProfile.ConnectionLimit = Convert.ToInt32(provider.FormData.GetValues("ConnectionLimit").First());
        //        veProfile.VirtualEnvironmentID = Convert.ToInt32(provider.FormData.GetValues("virtualEnvironmentID").First());

        //        _db.Entry(veProfile).State = EntityState.Modified;
        //        _db.SaveChanges();

        //        //Array labActivities = Convert.ToBase64CharArray(provider.FormData.GetValues("virtualEnvironmentID").First());

        //        if (provider.FileData.Count > 0)
        //        {
        //            string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/" + veProfile.VEProfileID.ToString());

        //            if (!Directory.Exists(dirPath))
        //            {
        //                System.IO.Directory.CreateDirectory(dirPath);
        //            }

        //            // This illustrates how to get the file names for uploaded files.
        //            foreach (var file in provider.FileData)
        //            {
        //                FileInfo uploadedFile = new FileInfo(file.LocalFileName);

        //                byte[] data = new byte[uploadedFile.Length];

        //                // Load a filestream and put its content into the byte[]
        //                FileStream dataStream = uploadedFile.OpenRead();
        //                dataStream.Read(data, 0, data.Length);

        //                Image image = Image.FromStream(dataStream);
        //                image = (Image)(new Bitmap(image, new Size(650, 389)));
        //                String fileName = veProfile.VEProfileID.ToString() + "-Thumbnail.jpg";
        //                String fullPath = Path.Combine(dirPath, fileName);

        //                if (File.Exists(fullPath)) File.Delete(fullPath);

        //                image.Save(fullPath);

        //                veProfile.ThumbnailURL = "Content/Images/" + veProfile.VEProfileID.ToString() + "/" + fileName;
        //                _db.Entry(veProfile).State = EntityState.Modified;
        //                _db.SaveChanges();

        //            }
        //        }
        //    }
        //    else
        //    {
        //        var formData = await Request.Content.ReadAsFormDataAsync();

        //        var veProfileId = Convert.ToInt32(formData.GetValues("VEProfileID").First());

        //        veProfile = _db.VEProfiles.Find(veProfileId);

        //        veProfile.Name = formData.GetValues("Name").First();
        //        veProfile.Description = formData.GetValues("Description").First();
        //        veProfile.ConnectionLimit = Convert.ToInt32(formData.GetValues("ConnectionLimit").First());
        //        veProfile.VirtualEnvironmentID = Convert.ToInt32(formData.GetValues("VirtualEnvironmentID").First());

        //        _db.Entry(veProfile).State = EntityState.Modified;
        //        _db.SaveChanges();
        //    }

        //    return Ok(veProfile);
        //}


        //[HttpGet]
        //[ResponseType(typeof(void))]
        //public IHttpActionResult UpdateVeProfileStatus(int id, int status)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var veProfile = _db.VEProfiles.Find(id);

        //    if (id != veProfile.VEProfileID)
        //    {
        //        return BadRequest();
        //    }

        //    veProfile.Status = status;
        //    _db.Entry(veProfile).State = EntityState.Modified;

        //    try
        //    {
        //        _db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!VEProfileExists(id))
        //        {
        //            return NotFound();
        //        }
        //        throw;
        //    }

        //    return StatusCode(HttpStatusCode.OK);
        //}

        [HttpGet]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateVirtualEnvironmentId(int id, int veId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var veProfile = _db.VEProfiles.Find(id);

            if (id != veProfile.VEProfileID)
            {
                return BadRequest();
            }

            veProfile.VirtualEnvironmentID = veId;
            _db.Entry(veProfile).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VEProfileExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.OK);
        }

        // POST: api/VEProfiles
        //[ResponseType(typeof(VEProfile))]
        //public async Task<IHttpActionResult> PostVEProfile()
        //{
        //    string root = HttpContext.Current.Server.MapPath("~/");
        //    var provider = new MultipartFormDataStreamProvider(root);

        //    VEProfile veProfile = new VEProfile();

        //    if (Request.Content.IsMimeMultipartContent())
        //    {
        //        //try
        //        //{
        //        // Read the form data and return an async task.
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        veProfile.Name = provider.FormData.GetValues("Name").First();
        //        veProfile.Description = provider.FormData.GetValues("Description").First();
        //        veProfile.ConnectionLimit = Convert.ToInt32(provider.FormData.GetValues("ConnectionLimit").First());
        //        veProfile.VirtualEnvironmentID = Convert.ToInt32(provider.FormData.GetValues("VirtualEnvironmentID").First());
        //        veProfile.DateProvisionTrigger = DateTime.Parse("2016-04-01 00:00:00");

        //        _db.VEProfiles.Add(veProfile);
        //        _db.SaveChanges();

        //        //Array labActivities = Convert.ToBase64CharArray(provider.FormData.GetValues("virtualEnvironmentID").First());

        //        string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/" + veProfile.VEProfileID.ToString());

        //        if (!Directory.Exists(dirPath))
        //        {
        //            System.IO.Directory.CreateDirectory(dirPath);
        //        }

        //        if (provider.FileData.Count > 0)
        //        {
        //            // This illustrates how to get the file names for uploaded files.
        //            foreach (var file in provider.FileData)
        //            {
        //                if (!string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
        //                {
        //                    FileInfo uploadedFile = new FileInfo(file.LocalFileName);

        //                    byte[] data = new byte[uploadedFile.Length];

        //                    // Load a filestream and put its content into the byte[]
        //                    FileStream dataStream = uploadedFile.OpenRead();
        //                    dataStream.Read(data, 0, data.Length);

        //                    Image image = Image.FromStream(dataStream);
        //                    image = (Image)(new Bitmap(image, new Size(650, 389)));
        //                    String fileName = veProfile.VEProfileID.ToString() + "-Thumbnail.jpg";
        //                    String fullPath = Path.Combine(dirPath, fileName);

        //                    if (File.Exists(fullPath)) File.Delete(fullPath);

        //                    image.Save(fullPath);

        //                    veProfile.ThumbnailURL = "Content/Images/" + veProfile.VEProfileID.ToString() + "/" + fileName;
        //                    _db.Entry(veProfile).State = EntityState.Modified;
        //                    _db.SaveChanges();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            veProfile.ThumbnailURL = null;
        //            _db.Entry(veProfile).State = EntityState.Modified;
        //            _db.SaveChanges();
        //        }
        //        /*
        //        }
        //        catch (Exception e)
        //        {
        //            throw e;
        //        }    
        //        */
        //    }
        //    else
        //    {
        //        var formData = await Request.Content.ReadAsFormDataAsync();

        //        veProfile.Name = formData.GetValues("Name").First();
        //        veProfile.Description = formData.GetValues("Description").First();
        //        veProfile.ConnectionLimit = Convert.ToInt32(formData.GetValues("ConnectionLimit").First());
        //        veProfile.VirtualEnvironmentID = Convert.ToInt32(formData.GetValues("VirtualEnvironmentID").First());
        //        veProfile.DateProvisionTrigger = DateTime.Parse("2016-04-01 00:00:00");

        //        _db.VEProfiles.Add(veProfile);
        //        _db.SaveChanges();
        //    }

        //    return Ok(veProfile);
        //}


        // DELETE: api/VEProfiles/5
        [ResponseType(typeof(VEProfile))]
        public IHttpActionResult DeleteVEProfile(int id)
        {
            VEProfile vEProfile = _db.VEProfiles.Find(id);
            if (vEProfile == null)
            {
                return NotFound();
            }

            _db.Database.ExecuteSqlCommand(
                "DELETE FROM dbo.VEProfileLabActivities "
                + "WHERE VEProfileID=" + id.ToString());

            _db.VEProfiles.Remove(vEProfile);
            _db.SaveChanges();

            return Ok(vEProfile);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VEProfileExists(int id)
        {
            return _db.VEProfiles.Count(e => e.VEProfileID == id) > 0;
        }

        private async Task<string> ApiCall(string method, string baseAddress, string url, string data = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(baseAddress);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;

            if (method == "POST")
            {
                response = await client.PostAsync(client.BaseAddress + url, new StringContent(data));
            }
            else if (method == "GET")
            {
                response = await client.GetAsync(client.BaseAddress + url).ConfigureAwait(false);
            }
            else if (method == "DELETE")
            {
                response = await client.DeleteAsync(url);
            }

            return await response.Content.ReadAsStringAsync();

        }

        //private void UpdateProvisionDate(VEProfile veProfile)
        //{
        //    veProfile.DateProvisionTrigger = DateTime.UtcNow;
        //    _db.Entry(veProfile).State = EntityState.Modified;
        //    _db.SaveChanges();
        //    //return false;
        //}

        //[HttpPost]
        //[Route("ProvisionUsersBB")]
        //public async Task<IHttpActionResult> ProvisionUsersBB(UserVEViewModel data)
        //{
        //    return await ProvisionUsers(data.VeProfile.VEProfileID, data.Users);
        //}

        [HttpPost]
        [Route("CreateLabProfile")]
        public async Task<IHttpActionResult> CreateLabProfile(VEProfile model, string Type, int GroupID)
        {
            model.DateCreated = DateTime.Today;
            try
            {
                if (Type == "create")
                {
                    _db.VEProfiles.Add(model);

                    await _db.SaveChangesAsync();

                    int id = model.VEProfileID;

                    VEProfileLabCreditMappings ve = new VEProfileLabCreditMappings
                    {
                        VEProfileID = id,
                        GroupID = GroupID,
                        CourseHours = 0,
                        NumberOfUsers = 0
                    };

                    _db.VEProfileLabCreditMappings.Add(ve);

                    await _db.SaveChangesAsync();
                }

                else if (Type == "edit")
                {
                    _db.Entry(model).State = EntityState.Modified;
                    _db.SaveChanges();
                }


                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest("Error Creating Lab Profile" + ex);
            }
            finally
            {

            }

        }

        // DELETE: api/VEProfiles/5
        [HttpDelete]
        [Route("DeleteLabProfile")]
        public async Task<IHttpActionResult> DeleteLabProfile(int VEProfileID, int GroupID)
        {
            try
            {
                var profile = _db.VEProfileLabCreditMappings.FirstOrDefault(x => x.VEProfileID == VEProfileID && x.GroupID == GroupID);
                _db.VEProfileLabCreditMappings.Remove(profile);
                await _db.SaveChangesAsync();
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest("Error Deleting Lab Profile" + ex);
            }

            finally
            {

            }
        }

        [HttpGet]
        [Route("GetLabSchedules")]
        public IHttpActionResult GetLabSchedules(int veProfileId, int userId)
        {
            try
            {
                var Schedule = _db.CloudLabsSchedule.Where(p => p.VEProfileID == veProfileId && p.UserId == userId).FirstOrDefault();

                var result = new CloudLabsSchedule
                {
                    CloudLabsScheduleId = Schedule.CloudLabsScheduleId,
                    VEProfileID = Schedule.VEProfileID,
                    UserId = Schedule.UserId,
                    TimeRemaining = Schedule.TimeRemaining,
                    LabHoursTotal = Schedule.LabHoursTotal,
                    StartLabTriggerTime = Schedule.StartLabTriggerTime,
                    RenderPageTriggerTime = Schedule.RenderPageTriggerTime
                };

                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }
            finally
            {

            }
        }

        //[HttpPost]
        //[Route("LogTime")]
        //public async Task<IHttpActionResult> LogTime(int veProfileId, int userId, bool isInstructor)
        //{
        //    try
        //    {
        //        CloudLabsSchedule Schedule = _db.CloudLabsSchedule.Where(p => p.VEProfileID == veProfileId && p.UserId == userId).FirstOrDefault();
        //        MachineLabs vm = _db.MachineLabs.Where(q => q.VEProfileId == veProfileId && q.UserId == userId).FirstOrDefault();

        //        var isExtensionStudentExist = _db.LabHourExtensions.Where(q => q.VEProfileId == veProfileId && q.UserId == userId)
        //            .Join(_db.LabHourExtensionTypes,
        //            a => a.ExtensionTypeId,
        //            b => b.Id,
        //            (a, b) => new { a, b }).Where(w => w.b.Label == "Student")
        //            .ToList().Any(q => q.a.StartDate.ToLocalTime() <= DateTime.Now && q.a.EndDate.ToLocalTime() > DateTime.Now);

        //        var isExtensionInstuctorExist = _db.LabHourExtensions.Where(q => q.VEProfileId == veProfileId && q.UserId == userId)
        //            .Join(_db.LabHourExtensionTypes,
        //            a => a.ExtensionTypeId,
        //            b => b.Id,
        //            (a, b) => new { a, b }).Where(w => w.b.Label == "Instructor")
        //            .ToList().Any(q => q.a.StartDate.ToLocalTime() <= DateTime.Now && q.a.EndDate.ToLocalTime() > DateTime.Now);


        //        if (isInstructor)
        //        {
        //            if (!isExtensionInstuctorExist)
        //            {
        //                if (Schedule.InstructorLabHours != 0 && vm.IsStarted == 1)
        //                {
        //                    Schedule.InstructorLabHours -= 1;
        //                    Schedule.InstructorLastAccess = DateTime.UtcNow;
        //                    await _db.SaveChangesAsync();
        //                }
        //                return Ok(Schedule.InstructorLabHours);
        //            }
        //        }
        //        else
        //        {

        //            if (!isExtensionStudentExist)
        //            {
        //                if (Schedule.LabHoursRemaining != 0 && vm.IsStarted == 1)
        //                {
        //                    Schedule.LabHoursRemaining -= 1;
        //                    Schedule.RenderPageTriggerTime = DateTime.UtcNow;
        //                    Schedule.StartLabTriggerTime = DateTime.UtcNow;
        //                    await _db.SaveChangesAsync();
        //                }
        //                return Ok(Schedule.LabHoursRemaining);
        //            }
        //        }
        //        return Ok(Schedule.LabHoursRemaining);
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Error" + ex);
        //    }

        //    finally
        //    {

        //    }
        //}

        [HttpGet]
        [Route("GetTotalLabCredits")]
        public IHttpActionResult GetLabCredits(int GroupID, int VEProfileID)
        {
            try
            {
                var query = _db.VEProfileLabCreditMappings
                          .Join(_db.CloudLabsGroups,
                           a => a.GroupID,
                           b => b.CloudLabsGroupID,
                           (a, b) => new { VEProfileLabCreditMappings = a, CloudLabsGroups = b })
                           .Select(x => new
                           {
                               VEProfileID = x.VEProfileLabCreditMappings.VEProfileID,
                               GroupID = x.CloudLabsGroups.CloudLabsGroupID,
                               CourseHours = x.VEProfileLabCreditMappings.CourseHours,
                               NumberOfUsers = x.VEProfileLabCreditMappings.NumberOfUsers,
                               SubscriptionCredits = x.CloudLabsGroups.SubscriptionHourCredits,
                               TotalCourseHours = x.VEProfileLabCreditMappings.TotalCourseHours,
                               SubscriptionRemainingCredits = x.CloudLabsGroups.SubscriptionRemainingHourCredits,
                               TotalRemainingCourseHours = x.VEProfileLabCreditMappings.TotalRemainingCourseHours,
                               TotalRemainingContainers = x.VEProfileLabCreditMappings.TotalRemainingContainers,
                               MachineSize = x.VEProfileLabCreditMappings.MachineSize
                           }).Where(q => q.VEProfileID == VEProfileID).FirstOrDefault();

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }
            finally
            {

            }
        }

        [HttpGet]
        [Route("GetRemainingLabCredits")]
        public IHttpActionResult GetRemainingLabCredits(int veProfileId)
        {
            try
            {
                var Schedule = _db.CloudLabsSchedule.Where(p => p.VEProfileID == veProfileId);
                var TotalHours = Schedule.Sum(q => q.LabHoursTotal);

                return Ok(TotalHours);

            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }
            finally
            {

            }
        }

        [HttpGet]
        [Route("GetLabHoursByUserId")]
        public IHttpActionResult GetLabHoursByUserId(int userId, int VEProfileID)
        {
            try
            {
                var Schedule = _db.CloudLabsSchedule.Where(p => p.UserId == userId && p.VEProfileID == VEProfileID);

                return Ok(Schedule);

            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }
            finally
            {

            }
        }


        [HttpGet]
        [Route("GetProfile")]
        public async Task<IHttpActionResult> GetLabProfile(int VEProfileID)
        {
            try
            {
                //x => x.VEProfileId == Id
                VEProfile a = _db.VEProfiles.FirstOrDefault(x => x.VEProfileID == VEProfileID);
                await _db.SaveChangesAsync();
                return Ok(a);
            }

            catch (Exception ex)
            {
                return BadRequest("Error Profile Not Found" + ex);
            }

            finally
            {

            }
        }

        //[HttpPost]
        //[Route("ProvisionCloudLabs")]
        //public IHttpActionResult ProvisionCloudLabs(int veProfileId, List<User> CLUsers)
        //{
        //    var returnMessage = "Virtual machines successfully provisioned. ";
        //    var veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == veProfileId);
        //    if (veProfile == null)
        //        return Ok("VE Profile does not exist");

        //    var veImages = _db.VirtualEnvironmentImages.Where(vi => vi.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).ToList();

        //    var cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);
        //    var veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();


        //    var provisionUserList = CLUsers.Where(clu => !veProfileVMs.Exists(ve => ve.UserID != clu.UserId)).ToList();

        //    var existingUserSchedule = CLUsers.Where(clu => veProfileVMs.Exists(ve => ve.UserID == clu.UserId)).ToList();

        //    var tenantId = _db.CloudLabUsers.Select(cl => cl.TenantId).FirstOrDefault();


        //    returnMessage += ProvisionVMs(100, 100, 200, veProfile, provisionUserList, veImages[0].Name, veImages[0].Size, "", veImages[0].Type, veImages[0].Protocol, tenantId, false);



        //    if (_vmOperationMultiple.Count > 0)
        //    {
        //        SendProvisionMessage(cloudProvider.Name);
        //        //update DateProvisionTrigger in DB after sending provision message
        //        UpdateProvisionDate(veProfile);
        //    }
        //    else
        //    {
        //        returnMessage = string.Format("VM Provisioning was not triggered. VM Count: {0}", _vmOperationMultiple.Count);
        //    }

        //    return Ok(returnMessage);
        //}

        [HttpPost]
        [Route("EditLabSchedule")]
        public IHttpActionResult EditLabSchedule(List<CloudLabsSchedule> CLUsers)
        {
            _db.Entry(CLUsers).State = EntityState.Modified;
            return Ok();
        }

        [HttpGet]
        [Route("GetVEProfiles")]
        public IHttpActionResult GetVEProfiles(int UserGroupID, bool isSuperAdmin)
        {
            try
            {
                if (isSuperAdmin)
                {
                    var query = _db.VEProfiles
                        .GroupJoin(_db.CloudLabsSchedule,
                         a => a.VEProfileID,
                         b => b.VEProfileID,
                         (a, b) => new { VEProfiles = a, CloudLabsSchedule = b })
                         .Select(x => new
                         {
                             vep = x.VEProfiles,
                             cls = x.CloudLabsSchedule
                         }).ToList()
                         .Join(_db.VirtualEnvironments,
                        clu => clu.vep.VirtualEnvironmentID,
                        anur => anur.VirtualEnvironmentID,
                        (clu, anur) => new { CloudLabsSchedule = clu, VirtualEnvironment = anur })
                        .Select(x => new
                        {
                            CSSchedule = x.CloudLabsSchedule,
                            VEnviron = x.VirtualEnvironment
                        })
                        .Join(_db.VEProfileLabCreditMappings,
                        x => x.CSSchedule.vep.VEProfileID,
                        y => y.VEProfileID,
                        (x, y) => new { CloudLabsSchedule = x, VEProfileLabCreditMappings = y })
                        .Select(x => new
                        {
                            DateProvisionTrigger = x.CloudLabsSchedule.CSSchedule.vep.DateCreated,
                            Description = x.CloudLabsSchedule.CSSchedule.vep.Description,
                            Name = x.CloudLabsSchedule.CSSchedule.vep.Name,
                            ThumbnailURL = x.CloudLabsSchedule.CSSchedule.vep.ThumbnailURL,
                            VEProfileID = x.CloudLabsSchedule.CSSchedule.vep.VEProfileID,
                            VirtualEnvironment = x.CloudLabsSchedule.CSSchedule.vep.VirtualEnvironment,
                            VEDescription = x.CloudLabsSchedule.CSSchedule.vep.VirtualEnvironment.Description,
                            VirtualEnvironmentID = x.CloudLabsSchedule.CSSchedule.vep.VirtualEnvironmentID,
                            CloudLabsSchedule = x.CloudLabsSchedule.CSSchedule.cls.Where(q => q.VEProfileID == x.CloudLabsSchedule.CSSchedule.vep.VEProfileID),
                            GroupId = x.VEProfileLabCreditMappings.GroupID,
                            VETypeId = x.CloudLabsSchedule.VEnviron.VETypeID
                        })
                        .Where(h => h.GroupId == UserGroupID)
                        .Distinct()
                        .OrderBy(y => y.Name).ToList();

                    return Ok(query);
                }
                else
                {
                    var query = _db.VEProfiles
                       .GroupJoin(_db.CloudLabsSchedule,
                        a => a.VEProfileID,
                        b => b.VEProfileID,
                        (a, b) => new { VEProfiles = a, CloudLabsSchedule = b })
                        .Select(x => new
                        {
                            vep = x.VEProfiles,
                            cls = x.CloudLabsSchedule
                        }).ToList()
                        .Join(_db.VirtualEnvironments,
                       clu => clu.vep.VirtualEnvironmentID,
                       anur => anur.VirtualEnvironmentID,
                       (clu, anur) => new { CloudLabsSchedule = clu, VirtualEnvironment = anur })
                       .Select(x => new
                       {
                           CSSchedule = x.CloudLabsSchedule,
                           VEnviron = x.VirtualEnvironment
                       })
                       .Join(_db.VEProfileLabCreditMappings,
                       x => x.CSSchedule.vep.VEProfileID,
                       y => y.VEProfileID,
                       (x, y) => new { CloudLabsSchedule = x, VEProfileLabCreditMappings = y })
                       .Select(x => new
                       {
                           DateProvisionTrigger = x.CloudLabsSchedule.CSSchedule.vep.DateCreated,
                           Description = x.CloudLabsSchedule.CSSchedule.vep.Description,
                           Name = x.CloudLabsSchedule.CSSchedule.vep.Name,
                           ThumbnailURL = x.CloudLabsSchedule.CSSchedule.vep.ThumbnailURL,
                           VEProfileID = x.CloudLabsSchedule.CSSchedule.vep.VEProfileID,
                           VirtualEnvironment = x.CloudLabsSchedule.CSSchedule.vep.VirtualEnvironment,
                           VEDescription = x.CloudLabsSchedule.CSSchedule.vep.VirtualEnvironment.Description,
                           VirtualEnvironmentID = x.CloudLabsSchedule.CSSchedule.vep.VirtualEnvironmentID,
                           CloudLabsSchedule = x.CloudLabsSchedule.CSSchedule.cls.Where(q => q.VEProfileID == x.CloudLabsSchedule.CSSchedule.vep.VEProfileID),
                           GroupId = x.VEProfileLabCreditMappings.GroupID,
                           VETypeId = x.CloudLabsSchedule.VEnviron.VETypeID
                       })
                       .Where(h => h.GroupId == UserGroupID && h.VETypeId != 3 && h.VETypeId != 4)
                       .Distinct()
                       .OrderBy(y => y.Name).ToList();

                    return Ok(query);
                }
               

            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }

        //[HttpPost]
        //[Route("ProvisionMachines")]
        //public async Task<IHttpActionResult> ProvisionMachines(ProvisionDetails contents, bool isLaas)
        //{
        //    try
        //    {
        //        VEDetails labCreditMapping = contents.labCreditMapping;
        //        CloudLabsGroups cloudLabsGroups = new CloudLabsGroups();

        //        List<User> CLUsers = contents.CLUsers;

        //        var returnMessage = "Virtual machines successfully provisioned. ";
        //        var veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == labCreditMapping.VEProfileID);

        //        if (veProfile == null)
        //            return Ok("VE Profile does not exist");

        //        var veImages = _db.VirtualEnvironmentImages.Where(vi => vi.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).ToList();

        //        var cloudProvider = _db.CloudProviders.SingleOrDefault
        //            (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //        var veTypeId = _db.VirtualEnvironments.SingleOrDefault(q => q.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).VETypeID;

        //        var veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //        var provisionUsersList = CLUsers.Where(user => !veProfileVMs.Exists(vm => vm.UserID == user.UserId)).ToList();

        //        int LabHoursPerCourse = Convert.ToInt32(CLUsers[0].labHoursTotal);

        //        var existingUserSchedule = CLUsers.Where(clu => veProfileVMs.Exists(ve => ve.UserID == clu.UserId)).ToList();

        //        var existingCloudLabSchedules = _db.CloudLabsSchedule.Where(cls => cls.VEProfileID == labCreditMapping.VEProfileID).ToList();

        //        foreach (var schedule in existingUserSchedule)
        //        {
        //            var result = existingCloudLabSchedules.Where(cls => cls.UserId == schedule.UserId).ToList();

        //            result[0].LabHoursRemaining = Convert.ToDouble(schedule.labHoursRemaining);
        //            result[0].LabHoursTotal = Convert.ToDouble(schedule.labHoursTotal);
        //            labCreditMapping.TotalRemainingCourseHours -= labCreditMapping.CourseHours;
        //            //_db.Entry(result[0]).State = EntityState.Modified;
        //            //_db.SaveChanges();

        //            cloudLabsGroups.SubscriptionHourCredits -= labCreditMapping.CourseHours;
        //            _db.Entry(labCreditMapping).State = EntityState.Modified;
        //            _db.SaveChanges();
        //        }

        //        //var tenantId = _db.CloudLabUsers.Select(cl => cl.TenantId).FirstOrDefault();
        //        var tenantId = _db.CloudLabUsers.Where(cl => cl.UserGroup == labCreditMapping.GroupID).Select(q => q.TenantId).FirstOrDefault();

        //        returnMessage += ProvisionVMs(cloudLabsGroups.SubscriptionRemainingHourCredits, labCreditMapping.GroupID, labCreditMapping.CourseHours, veProfile, provisionUsersList, veImages[0].Name, veImages[0].Size, "", veImages[0].Type, veImages[0].Protocol, tenantId, isLaas, labCreditMapping.Size);
        //        var dataMessage = JsonConvert.SerializeObject(_vmOperationMultiple);
        //        ApiCall("POST", FAApiUrl, ProvisionMachine, dataMessage);





        //        return Ok(string.Format("Successfully Provision"));
        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(string.Format("Errroorrrr! {0}", e.Message));

        //    }
        //}
        //[HttpPost]
        //[Route("CreateLabCreditRecord")]
        //public HttpResponseMessage CreateLabCreditRecord(VEProfileLabCreditMappings model)
        //{
        //    var veprofilemappings = _db.VEProfileLabCreditMappings.Where(p => p.VEProfileID == model.VEProfileID && p.GroupID == model.GroupID).Count();
        //    if(veprofilemappings == 0)
        //        _db.VEProfileLabCreditMappings.Add(model);
        //    else
        //    {
        //        VEProfileLabCreditMappings record = _db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == model.VEProfileID && x.GroupID == model.GroupID).FirstOrDefault();
        //        record.TotalLabHours = model.TotalLabHours;
        //        record.TotalRemainingHours = model.TotalRemainingHours;
        //        record.LabHoursPerCourse = model.LabHoursPerCourse;
        //    }
        //    _db.SaveChanges();
        //    return Request.CreateResponse(HttpStatusCode.OK, model);

        //}

        [HttpPost]
        [Route("EditLabCreditRecord")]
        public HttpResponseMessage EditLabCreditRecord(VEProfileLabCreditMappings model)
        {
            //_db.VEProfileLabCreditMappings.Add(model);
            VEProfileLabCreditMappings record = _db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == model.VEProfileID && x.GroupID == model.GroupID).FirstOrDefault();
            //record.TotalLabHours = model.TotalLabHours;
            //record.TotalRemainingHours = model.TotalRemainingHours;
            //record.LabHoursPerCourse = model.LabHoursPerCourse;
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [Route("GetMappingLabProfileUserGroup")]
        public IHttpActionResult GetMappingLabProfileUserGroup(int GroupID, int VEProfileID)
        {
            var vmMappings = _db.VEProfileLabCreditMappings.Where(x => x.GroupID == GroupID && x.VEProfileID == VEProfileID).FirstOrDefault();
            return Ok(vmMappings);
        }

        [HttpPost]
        [Route("DeductLabHours")]
        public HttpResponseMessage DeductLabHours(IDs IDs)
        {
            VEProfileLabCreditMappings record = _db.VEProfileLabCreditMappings.Where(x => x.VEProfileID == IDs.VEProfileID && x.GroupID == IDs.GroupID).FirstOrDefault();
            record.TotalRemainingCourseHours -= record.CourseHours;
            _db.Entry(record).State = EntityState.Modified;
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("GetAllVEProfiles")]
        public IHttpActionResult GetAllVEProfiles()
        {
            try
            {
                var query = _db.VEProfiles
                         .GroupJoin(_db.CloudLabsSchedule,
                          a => a.VEProfileID,
                          b => b.VEProfileID,
                          (a, b) => new { VEProfiles = a, CloudLabsSchedule = b })
                          .Select(x => new
                          {
                              vep = x.VEProfiles,
                              cls = x.CloudLabsSchedule
                          }).ToList()
                          .Join(_db.VirtualEnvironments,
                         clu => clu.vep.VirtualEnvironmentID,
                         anur => anur.VirtualEnvironmentID,
                         (clu, anur) => new { CloudLabsSchedule = clu, VirtualEnvironment = anur })
                         .Select(x => new
                         {
                             //ConnectionLimit = x.CloudLabsSchedule.vep.ConnectionLimit,
                             //CourseID = x.CloudLabsSchedule.vep.CourseID,
                             DateProvisionTrigger = x.CloudLabsSchedule.vep.DateCreated,
                             Description = x.CloudLabsSchedule.vep.Description,
                             //ExamPassingRate = x.CloudLabsSchedule.vep.ExamPassingRate,
                             //IsEmailEnabled = x.CloudLabsSchedule.vep.IsEmailEnabled,
                             //IsEnabled = x.CloudLabsSchedule.vep.IsEnabled,
                             Name = x.CloudLabsSchedule.vep.Name,
                             //PassingRate = x.CloudLabsSchedule.vep.PassingRate,
                             //Remairks = x.CloudLabsSchedule.vep.Remarks,
                             //Status = x.CloudLabsSchedule.vep.Status,
                             ThumbnailURL = x.CloudLabsSchedule.vep.ThumbnailURL,
                             VEProfileID = x.CloudLabsSchedule.vep.VEProfileID,
                             VirtualEnvironment = x.VirtualEnvironment,
                             VirtualEnvironmentID = x.CloudLabsSchedule.vep.VirtualEnvironmentID,
                             CloudLabsSchedule = x.CloudLabsSchedule.cls
                         })
                         .OrderBy(y => y.Name).ToList();

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }

        //[HttpGet]
        //[Route("IdleMachines")]
        //public HttpResponseMessage IdleMachines()
        //{
        //    var tenantId = _db.CloudLabUsers.Select(cl => cl.TenantId).FirstOrDefault();

        //    var sqlTenant = new SqlCommand(string.Format("SELECT Code, TenantName FROM Tenants WHERE TenantId = '{0}'", tenantId), _connTenant);

        //    var sqlCode = string.Empty;

        //    _connTenant.Open();
        //    using (var sqlReader = sqlTenant.ExecuteReader())
        //    {
        //        while (sqlReader.Read())
        //        {
        //            sqlCode = sqlReader["Code"].ToString();
        //        }
        //    }


        //    var IdleMachines = _db.VirtualMachines.Where(v => v.IsStarted == 1).ToList()
        //        .Join(_db.CloudLabsSchedule.ToList(),
        //        vm => new { Id = vm.UserID, Type = vm.VEProfileID },
        //        cls => new { Id = cls.UserId, Type = cls.VEProfileID },
        //        (vm, cls) => new { vm = vm, cls = cls })
        //        .Join(_db.VirtualMachineMappings,
        //        q => new { Id = q.cls.UserId, Type = q.cls.VEProfileID },
        //        vmm => new { Id = vmm.UserID, Type = vmm.VEProfileID },
        //        (q, vmm) => new { query = q, vmm = vmm })
        //        .Join(_db.CloudLabUsers,
        //        que => que.query.vm.UserID,
        //        clu => clu.UserId,
        //        (f, d) => new { s = f, cls = d })
        //        .Join(_db.CloudLabsGroups,
        //        k => k.cls.UserGroup,
        //        u => u.CloudLabsGroupID,
        //        (p, l) => new { i = p, j = l })
        //        .Select(x => new
        //        {
        //            MachineName = x.i.s.vmm.RoleName,
        //            Rolename = x.i.s.query.vm.RoleName,
        //            UserId = x.i.s.vmm.UserID,
        //            VEProfileId = x.i.s.vmm.VEProfileID,
        //            IsStarted = x.i.s.query.vm.IsStarted,
        //            StartLabTriggerTime = x.i.s.query.cls.StartLabTriggerTime.Value,
        //            RenderPageTriggerTime = x.i.s.query.cls.RenderPageTriggerTime.Value,
        //            InstructorLastAccess = x.i.s.query.cls.InstructorLastAccess,
        //            ResourceGroup = ResourceGroupName(x.j.CloudLabsGroupID, sqlCode)
        //        })
        //        .ToList();

        //    return Request.CreateResponse(HttpStatusCode.OK, IdleMachines);
        //    //var IdleMachines = _db.VirtualMachines.Where(v => v.IsStarted == 1).ToList()
        //    //    .Join(_db.CloudLabsSchedule.ToList(),
        //    //    vm => new { Id = vm.UserID, Type = vm.VEProfileID },
        //    //    cls => new { Id = cls.UserId, Type = cls.VEProfileID },
        //    //    (vm, cls) => new { vm = vm, cls = cls })
        //    //    .Join(_db.VirtualMachineMappings,
        //    //    q => new { Id = q.cls.UserId, Type = q.cls.VEProfileID },
        //    //    vmm => new { Id = vmm.UserID, Type = vmm.VEProfileID },
        //    //    (q, vmm) => new { query = q, vmm = vmm })
        //    //    .Select(x => new
        //    //    {
        //    //        MachineName = x.vmm.RoleName,
        //    //        Rolename = x.query.vm.RoleName,
        //    //        UserId = x.vmm.UserID,
        //    //        VEProfileId = x.vmm.VEProfileID,
        //    //        IsStarted = x.query.vm.IsStarted,
        //    //        StartLabTriggerTime = x.query.cls.StartLabTriggerTime.Value,
        //    //        RenderPageTriggerTime = x.query.cls.RenderPageTriggerTime.Value
        //    //    })
        //    //    .ToList();

        //    //return Request.CreateResponse(HttpStatusCode.OK, IdleMachines);

        //}

        [HttpGet]
        [Route("IsCourseByGroupProvisioned")]
        public HttpResponseMessage IsCourseByGroupProvisioned(int GroupID, int VEProfileID)
        {
            var bar = _db.VEProfileLabCreditMappings
                .Join(_db.CloudLabUsers,
                a => a.GroupID,
                b => b.UserGroup,   
                (a, b) => new { VEProfileLabCreditMappings = a, CloudLabUsers = b })
                .Join(_db.CloudLabsSchedule,
                c => new { c.VEProfileLabCreditMappings.VEProfileID, c.CloudLabUsers.UserId },
                d => new { d.VEProfileID, d.UserId },
                (c, d) => new { c = c, d = d })
                .ToList().Exists(q => q.c.CloudLabUsers.UserGroup == GroupID && q.d.VEProfileID == VEProfileID);


            return Request.CreateResponse(HttpStatusCode.OK, bar);
        }

        [HttpPost]
        [Route("AddSubsciptionCredit")]
        public HttpResponseMessage AddSubsciptionCredit(CloudLabsGroups model)
        {
            if(_db.CloudLabsGroups.Any(x => x.CloudLabsGroupID == model.CloudLabsGroupID && x.GroupName == model.GroupName))
            {
                CloudLabsGroups record = _db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == model.CloudLabsGroupID && x.GroupName == model.GroupName).FirstOrDefault();
                model.SubscriptionHourCredits = model.SubscriptionHourCredits;
                //model.SubscriptionHourCredits = model.SubscriptionHourCredits * 60;
                model.SubscriptionRemainingHourCredits = model.SubscriptionRemainingHourCredits;
                //model.SubscriptionRemainingHourCredits = model.SubscriptionRemainingHourCredits * 60;
                record.SubscriptionHourCredits += model.SubscriptionHourCredits;
                record.SubscriptionRemainingHourCredits = model.SubscriptionHourCredits;
                _db.SaveChanges();
            }

            return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpPost]
        [Route("EditSubsciptionCredit")]
        public HttpResponseMessage EditSubsciptionCredit(CloudLabsGroups model)
        {
            try
            {
                //_db.VEProfileLabCreditMappings.Add(model);
                CloudLabsGroups record = _db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == model.CloudLabsGroupID && x.GroupName == model.GroupName).FirstOrDefault();
                model.SubscriptionHourCredits = model.SubscriptionHourCredits;
                //model.SubscriptionHourCredits = model.SubscriptionHourCredits * 60;
                model.SubscriptionRemainingHourCredits = model.SubscriptionRemainingHourCredits;
                //model.SubscriptionRemainingHourCredits = model.SubscriptionRemainingHourCredits * 60;
                record.SubscriptionHourCredits += model.SubscriptionHourCredits;
                record.SubscriptionRemainingHourCredits += model.SubscriptionHourCredits;

                _db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, model);

            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }

        //[HttpGet]
        //[Route("IdleMachines1")]
        //public HttpResponseMessage IdleMachines1()
        //{
        //    var tenantId = _db.CloudLabUsers.Select(cl => cl.TenantId).FirstOrDefault();

        //    var sqlTenant = new SqlCommand(string.Format("SELECT Code, TenantName FROM Tenants WHERE TenantId = '{0}'", tenantId), _connTenant);

        //    var sqlCode = string.Empty;

        //    _connTenant.Open();
        //    using (var sqlReader = sqlTenant.ExecuteReader())
        //    {
        //        while (sqlReader.Read())
        //        {
        //            sqlCode = sqlReader["Code"].ToString();
        //        }
        //    }


        //    var IdleMachines = _db.VirtualMachines.Where(v => v.IsStarted == 1).ToList()
        //        .Join(_db.CloudLabsSchedule.ToList(),
        //        vm => new { Id = vm.UserID, Type = vm.VEProfileID },
        //        cls => new { Id = cls.UserId, Type = cls.VEProfileID },
        //        (vm, cls) => new { vm = vm, cls = cls })
        //        .Join(_db.VirtualMachineMappings,
        //        q => new { Id = q.cls.UserId, Type = q.cls.VEProfileID },
        //        vmm => new { Id = vmm.UserID, Type = vmm.VEProfileID },
        //        (q, vmm) => new { query = q, vmm = vmm })
        //        .Join(_db.CloudLabUsers,
        //        que => que.query.vm.UserID,
        //        clu => clu.UserId,
        //        (f, d) => new { s = f, cls = d })
        //        .Join(_db.CloudLabsGroups,
        //        k => k.cls.UserGroup,
        //        u => u.CloudLabsGroupID,
        //        (p, l) => new { i = p, j = l })
        //        .Select(x => new
        //        {
        //            MachineName = x.i.s.vmm.RoleName,
        //            Rolename = x.i.s.query.vm.RoleName,
        //            UserId = x.i.s.vmm.UserID,
        //            VEProfileId = x.i.s.vmm.VEProfileID,
        //            IsStarted = x.i.s.query.vm.IsStarted,
        //            StartLabTriggerTime = x.i.s.query.cls.StartLabTriggerTime.Value,
        //            RenderPageTriggerTime = x.i.s.query.cls.RenderPageTriggerTime.Value,
        //            ResourceGroup = ResourceGroupName(x.j.CloudLabsGroupID, sqlCode)
        //        })
        //        .ToList();

        //    return Request.CreateResponse(HttpStatusCode.OK, IdleMachines);
        //}

        [HttpGet]
        [Route("Dashboard")]
        public IHttpActionResult Dashboard(int UserGroupID)
        {
            try
            {
                var query = _db.VEProfiles
                    .Join(_db.MachineLabs,
                    a => a.VEProfileID,
                    b => b.VEProfileId,
                    (a, b) => new { a, b })
                    .Join(_db.VirtualEnvironments,
                    c => c.a.VirtualEnvironmentID,
                    d => d.VirtualEnvironmentID,
                    (c, d) => new { c, d })
                    .Join(_db.VEProfileLabCreditMappings,
                    e => e.c.a.VEProfileID,
                    f => f.VEProfileID,
                    (e, f) => new { e, f })
                    .Join(_db.CloudLabsGroups,
                    g => g.f.GroupID,
                    h => h.CloudLabsGroupID,
                    (g, h) => new { g, h })
                    .Select(p => new
                    {
                        coursecode = p.g.e.d.Title,
                        thumbnail = p.g.e.c.a.ThumbnailURL,
                        description = p.g.e.c.a.Description,
                        totalcoursehours = p.g.f.TotalCourseHours,
                        numberofuser = p.g.f.NumberOfUsers,
                        coursehours = p.g.f.CourseHours,
                        remainingcoursehours = p.g.f.TotalRemainingCourseHours,
                        groupid = p.h.CloudLabsGroupID
                    }).Where(x => x.groupid == UserGroupID)
                    .OrderBy(y => y.coursecode).ToList();

                return Ok(query);
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }


        //[HttpPost]
        //[Route("insertschedules")]
        //public IHttpActionResult insertschedules(int veprofileid, int[] userid)
        //{
        //    try
        //    {
        //        CloudLabsSchedule sched = new CloudLabsSchedule();



        //        foreach (var item in userid)
        //        {
        //            var dateCreated = _db.VirtualMachines.Where(q => q.UserID == item && q.VEProfileID == veprofileid).FirstOrDefault().DateCreated;


        //            sched.DateCreated = dateCreated;
        //            sched.InstructorLabHours = 120;
        //            sched.InstructorLastAccess = Convert.ToDateTime("1753-01-01 00:00:00.000");
        //            sched.LabHoursRemaining = 600;
        //            sched.LabHoursTotal = 600;
        //            sched.RenderPageTriggerTime = Convert.ToDateTime("1753-01-01 00:00:00.000");
        //            sched.ScheduledBy = "";
        //            sched.StartLabTriggerTime = Convert.ToDateTime("1753-01-01 00:00:00.000");
        //            sched.UserId = item;

        //            sched.VEProfileID = veprofileid;
        //            _db.CloudLabsSchedule.Add(sched);
        //            _db.SaveChanges();

        //        }


        //        return Ok("OK");
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Error" + ex);
        //    }

        //    finally
        //    {

        //    }
        //}

        [HttpPost]
        [Route("updatehours")]
        public IHttpActionResult updatehours(int[] veprofileid)
        {
            try
            {
                CloudLabsSchedule sched = new CloudLabsSchedule();

                foreach (var item in veprofileid)
                {
                    var labhours = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == item).FirstOrDefault().CourseHours;
                    if (labhours != 600)
                    {
                        var sched1 = _db.CloudLabsSchedule.Where(s => s.VEProfileID == item).ToList();


                        foreach (var item1 in sched1)
                        {
                            item1.TimeRemaining = TimeSpan.FromHours(labhours).TotalSeconds;
                            item1.LabHoursTotal = labhours;

                            _db.Entry(item1).State = EntityState.Modified;
                            _db.SaveChanges();

                        }
                    }

                }


                return Ok("OK");
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }

        //[HttpPost]
        //[Route("MachineProvision")]
        //public async Task<IHttpActionResult> MachineProvision(ProvisionDetails contents, bool isLaas, string schedBy)
        //{
        //    try
        //    {
        //        VEDetails labCreditMapping = contents.labCreditMapping;
        //        //CloudLabsGroups cloudLabsGroups = new CloudLabsGroups();

        //        List<User> CLUsers = contents.CLUsers;

        //        List<string> userId = new List<string>();
        //        string parameters = "";

        //        var veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == labCreditMapping.VEProfileID);

        //        var userTenantGroupId = _db.CloudLabUsers.Where(cl => cl.UserGroup == labCreditMapping.GroupID).Select(q => new { q.TenantId, q.UserGroup }).FirstOrDefault();

        //        var VMvhdUrl = _db.VirtualEnvironmentImages.Where(q => q.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).FirstOrDefault().Name;

        //        var VETypeIdTitle = _db.VirtualEnvironments.Where(q => q.VirtualEnvironmentID == veProfile.VirtualEnvironmentID).Select(x => new { x.VETypeID, x.Title }).FirstOrDefault();

        //        var tenantDetails = _dbTenant.Tenant.Where(q => q.TenantId == userTenantGroupId.TenantId).FirstOrDefault();

        //        var courseHours = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == veProfile.VEProfileID && q.GroupID == userTenantGroupId.UserGroup).FirstOrDefault().CourseHours;

        //        var groupDetails = _db.CloudLabsGroups.Where(q => q.TenantId == userTenantGroupId.TenantId && q.CloudLabsGroupID == userTenantGroupId.UserGroup).FirstOrDefault();

        //        if (veProfile == null)
        //            return Ok("VE Profile does not exist");

        //        List<User> user = new List<User>();
        //        List<VMOperation> _vmOperationMultiple = new List<VMOperation>();

        //        //Check if the user has no schedules
        //        foreach (var item in CLUsers)
        //        {
        //            if (!_db.VirtualMachineMappings.Any(q => q.UserID == item.UserId && q.VEProfileID == veProfile.VEProfileID) &&
        //                !_db.CloudLabsSchedule.Any(q => q.UserId == item.UserId && q.VEProfileID == veProfile.VEProfileID) &&
        //                !_db.VirtualMachines.Any(q => q.UserID == item.UserId && q.VEProfileID == veProfile.VEProfileID))
        //                user.Add(item);
        //            else
        //            {
        //                var sched = _db.CloudLabsSchedule.Where(q => q.UserId == item.UserId && q.VEProfileID == veProfile.VEProfileID).FirstOrDefault();
        //                sched.LabHoursRemaining = courseHours;
        //                sched.LabHoursTotal = sched.LabHoursTotal + courseHours;
        //                sched.DateCreated = DateTime.UtcNow;
        //                _db.Entry(sched).State = EntityState.Modified;
        //                _db.SaveChanges();

        //                var labcredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == veProfile.VEProfileID && q.GroupID == groupDetails.CloudLabsGroupID).FirstOrDefault();
        //                labcredit.TotalRemainingCourseHours -= courseHours;
        //                _db.Entry(labcredit).State = EntityState.Modified;
        //                _db.SaveChanges();
        //            }
        //        }


        //        //Add virtualmachineMappings to the user
        //        foreach (var item in user)
        //        {
        //            string Id = _db.CloudLabUsers.Where(q => q.UserId == item.UserId).FirstOrDefault().Id;

        //            if (VETypeIdTitle.VETypeID == 1 || VETypeIdTitle.VETypeID == 2 || VETypeIdTitle.VETypeID == 3 || VETypeIdTitle.VETypeID == 4)
        //            {
        //                //Save to VirtualMachineMappings
        //                string VMName = VirtualMachineMappingGenerator(Rolenames(userTenantGroupId.UserGroup, tenantDetails.Code).ToUpper(), 1, item.UserId, veProfile.VEProfileID, 0, false);

        //                var vmOperation = new VMOperation
        //                {
        //                    ImageName = VMvhdUrl,
        //                    CourseID = veProfile.CourseID,
        //                    VEProfileID = veProfile.VEProfileID,
        //                    VETypeId = VETypeIdTitle.VETypeID,
        //                    UserID = item.UserId,
        //                    GuacConnection = tenantDetails.GuacConnection,
        //                    GuacamoleUrl = tenantDetails.GuacamoleURL,
        //                    MachineName = VMName,
        //                    GroupID = userTenantGroupId.UserGroup,
        //                    CourseHours = courseHours,
        //                    ScheduledBy = schedBy,
        //                    ApiUrl = tenantDetails.ApiUrl,
        //                    ResourceGroup = ResourceGroupName(userTenantGroupId.UserGroup, tenantDetails.Code).ToUpper(),
        //                    TenantName = tenantDetails.TenantName,
        //                    IsLaasProvision = isLaas,
        //                    Region = labCreditMapping.Region,
        //                    VMSize = labCreditMapping.Size,
        //                    RoleName = tenantDetails.Code + '-' + veProfile.VEProfileID + '-' + item.UserId,

        //                    //Operation = "Provision",
        //                    //ImageType = imageType,
        //                    //WebApiUrl = tenantDetails.ApiUrl,
        //                    //OSFamily = veType.Name,
        //                    //Prefix = prefix,
        //                    //Suffix = suffix,
        //                    //ImageSize = veSize,
        //                    //AzurePort = azurePort,
        //                    Protocol = "RDP"
        //                };

        //                _vmOperationMultiple.Add(vmOperation);
        //            }
        //            else
        //            {
        //                string VMName = ContainerMappingGenerator(groupDetails.CLPrefix, VETypeIdTitle.Title, Id, item.UserId, veProfile.VEProfileID, veProfile.CourseID, 0);
        //                parameters = "&client_code=" + groupDetails.CLPrefix.ToLower() + "&course_code=" + VETypeIdTitle.Title.ToLower() + "&location=" + labCreditMapping.Region.ToLower() + "&image_name=" + VETypeIdTitle.Title.ToLower() + "&scheduledBy=" + schedBy; //veProfile.Name;
        //                if (VMName != null)
        //                {
        //                    userId.Add(Id);
        //                }
        //            }
        //        }

        //        if (user.Count() >= 1)
        //        {
        //            if (VETypeIdTitle.VETypeID == 1 || VETypeIdTitle.VETypeID == 2 || VETypeIdTitle.VETypeID == 3 || VETypeIdTitle.VETypeID == 4)
        //            {
        //                var dataMessage = JsonConvert.SerializeObject(_vmOperationMultiple);
        //                ApiCall("POST", FAApiUrl, ProvisionMachine, dataMessage);
        //            }
        //            else
        //            {
        //                var dataMessage = JsonConvert.SerializeObject(userId);
        //                ApiCall("POST", FAContainerUrl, ContainerProvision + parameters, dataMessage);
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    return Ok(string.Format("Successfully Provision"));
        //}

        [HttpGet]
        [Route("ProvisionAWSConsole")]
        public async Task<IHttpActionResult> ProvisionAWSConsole(string email, int userId, int veProfileId)
        {
            try
            {
                string baseAddress = "http://csaws.southeastasia.cloudapp.azure.com/api/createAccountwithStudent";
                string baseAddressTwo = "http://csaws.southeastasia.cloudapp.azure.com/api/createStudent";
                string baseAddressCheck = "http://csaws.southeastasia.cloudapp.azure.com/api/available_accounts?organization_unit_id="+ OrganizationUnit + "&quick=true";

                //string url = "createAccountwithStudent";
                var accountDetails = createAccountNameEmail("AWS");
                string password = createPassword();
                string accountEmail = accountDetails.AccountEmail;
                string accountName = accountDetails.AccountName;
                string requestId = "";
                          
                var data = new
                {
                    student_username = email,
                    student_password = password,
                    organization_unit_id = OrganizationUnit,
                    account_email = accountEmail,
                    account_name = accountName
                };

                var dataTwo = new
                {
                    student_username = email,
                    student_password = password,
                    organization_unit_id = OrganizationUnit
                };

                var dataMessage = JsonConvert.SerializeObject(data);
                var dataMessageTwo = JsonConvert.SerializeObject(dataTwo);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpClient clientCheck = new HttpClient();
                clientCheck.BaseAddress = new Uri(baseAddressCheck);
                clientCheck.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpClient clientTwo = new HttpClient();
                clientTwo.BaseAddress = new Uri(baseAddressTwo);
                clientTwo.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var responseCheck = await client.GetAsync(clientCheck.BaseAddress);
                AccountCheck requestCheck = JsonConvert.DeserializeObject<AccountCheck>(responseCheck.Content.ReadAsStringAsync().Result);

                HttpResponseMessage response = new HttpResponseMessage();

                //if(requestCheck.output == true)
                //{
                //    response = await client.PostAsync(clientTwo.BaseAddress, new StringContent(dataMessageTwo, Encoding.UTF8, "application/json"));

                //    requestId = response.Content.ReadAsStringAsync().Result.Substring(response.Content.ReadAsStringAsync().Result.IndexOf("REQUEST_ID : ") + 13);
                //}
                //else
                //{
                response = await client.PostAsync(client.BaseAddress, new StringContent(dataMessage, Encoding.UTF8, "application/json"));

                    requestId = response.Content.ReadAsStringAsync().Result.Substring(response.Content.ReadAsStringAsync().Result.IndexOf("REQUEST_ID : ") + 13);

                //}

                //dynamic result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);


                ConsoleSchedules CS = new ConsoleSchedules
                {
                    RequestId = Int32.Parse(requestId),                    
                    Email = email,
                    UserId = userId,
                    VEProfileId = veProfileId,
                    IsProvisioned = 2,
                    Status = "Provisioning",
                    DateCreated = DateTime.Today,
                    AccountEmail = accountEmail,
                    AccountName = accountName,
                    OrgUnit = OrganizationUnit
                };

                
                _db.ConsoleSchedules.Add(CS);
                _db.SaveChanges();

                return Ok(response.Content.ReadAsStringAsync());

            }
            catch (Exception e)
            {
                return Ok(string.Format(e.Message));

            }

        }
        public class AccountCheck
        {
            public string[] available_accounts { get; set; }
            public bool output { get; set; }
        }

        public class AccountDetails
        {
            public string AccountName { get; set; }
            public string AccountEmail { get; set; }
        }
        public static string createPassword()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var generatedValues = new string(
                Enumerable.Repeat(chars, 9)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            return generatedValues;
        }
        public AccountDetails createAccountNameEmail(string console)
        {
            var result = new AccountDetails();

            while (true)
            {
                const string chars = "12345678910";
                var random = new Random();
                var generatedName = new string(
                    Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
                if (console == "AWS")
                {
                    result = new AccountDetails
                    {

                        AccountEmail = string.Format("awsadmin{0}@cloudswyft.com", generatedName),
                        AccountName = string.Format("AWSadmin-{0}", generatedName)

                    };
                }
                else if (console == "Alibaba")
                {
                    result = new AccountDetails
                    {

                        AccountEmail = string.Format("aliadmin{0}@cloudswyft.com", generatedName),
                        AccountName = string.Format("Aliadmin-{0}", generatedName)

                    };
                }

                if (!isAccountEmailExist(result.AccountEmail))
                    break;

           }
            return result;
        }

        public bool isAccountEmailExist(string AccountEmail)
        {
            return _db.ConsoleSchedules.Any(e => e.AccountEmail == AccountEmail);
        } 
        public bool isAccountNameExist(string AccountName)
        {
            return _db.ConsoleSchedules.Any(e => e.AccountName == AccountName);
        }

        [HttpPost]
        [Route("CheckConsoleUser")]
        public HttpResponseMessage CheckConsoleUser(ConsoleUserDetails consoleUsers)
        {
            try
            {
                List<ConsoleResultChecker> ConUsers = consoleUsers.ConsoleUserID;
                List<ConsoleResultChecker> ConResult = new List<ConsoleResultChecker>();

                foreach (var user in ConUsers)
                {

                    var consoleDGrants = _db.CourseGrants.Where(q => (q.UserID == user.UserId) && (q.VEProfileID == consoleUsers.VEProfileID)).FirstOrDefault();
                    var consoleDSchedules = _db.ConsoleSchedules.Where(q => (q.UserId == user.UserId) && (q.VEProfileId == consoleUsers.VEProfileID)).FirstOrDefault();

                    if (consoleDGrants == null)
                    {
                        var result = new ConsoleResultChecker
                        {
                            UserId = user.UserId,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Email = user.Email,
                            FullNameEmail = user.FullNameEmail,
                            VEProfileId = consoleUsers.VEProfileID,
                            VEType = consoleUsers.VEType,
                            IsCourseGranted = 0,
                            IsProvisioned = 0
                        };

                        ConResult.Add(result);
                    }
                    else
                    {
                        if (consoleDSchedules == null)
                        {
                            var result1 = new ConsoleResultChecker
                            {
                                UserId = consoleDGrants.UserID,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                FullNameEmail = user.FullNameEmail,
                                VEProfileId = consoleDGrants.VEProfileID,
                                VEType = consoleDGrants.VEType,
                                IsCourseGranted = Convert.ToInt32(consoleDGrants.IsCourseGranted),
                                IsProvisioned = 0,
                            };
                            ConResult.Add(result1);
                        }
                        else
                        {
                            var result2 = new ConsoleResultChecker
                            {
                                UserId = consoleDGrants.UserID,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                FullNameEmail = user.FullNameEmail,
                                VEProfileId = consoleDGrants.VEProfileID,
                                VEType = consoleDGrants.VEType,
                                IsCourseGranted = Convert.ToInt32(consoleDGrants.IsCourseGranted),
                                IsProvisioned = consoleDSchedules.IsProvisioned,
                            };
                            ConResult.Add(result2);
                        }




                    }
                };

                return Request.CreateResponse(HttpStatusCode.OK, ConResult);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpPost]
        [Route("SaveGrantUser")]
        public async Task<IHttpActionResult> SaveConsoleUser(GrantConsoleDetails contents, int GrantedBy)
        {
            try
            {
                List<ConsoleAccessDetail> CLUsers = contents.ConsoleUsers;
                //i need to do a to list to check if there 

                foreach (var user in CLUsers)
                {
                    var consoleDetails = _db.CourseGrants.Where(q => (q.UserID == user.UserId) && (q.VEProfileID == user.VEProfileId)).FirstOrDefault();
                    var veprofileMappings = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == user.VEProfileId && q.GroupID == user.UserGroup).FirstOrDefault();
                    var iSGranted = _db.CourseGrants.Any(q => q.UserID == user.UserId && q.VEProfileID == user.VEProfileId);
                    
                    if (consoleDetails == null)
                    {
                        var result = new CourseGrants
                        {
                            UserID = user.UserId,
                            VEProfileID = user.VEProfileId,
                            VEType = user.VEType,
                            IsCourseGranted = !user.IsCourseGranted,
                           GrantedBy = GrantedBy

                        };
                        //_db.Entry(result).State = EntityState.Added;
                        _db.CourseGrants.Add(result);
                        _db.SaveChanges();

                        veprofileMappings.TotalRemainingCourseHours -= veprofileMappings.CourseHours;
                        _db.Entry(veprofileMappings).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        if (iSGranted)
                        {
                            if(_db.CourseGrants.Where(q => q.UserID == user.UserId && q.VEProfileID == user.VEProfileId).FirstOrDefault().IsCourseGranted)
                            {
                                veprofileMappings.TotalRemainingCourseHours += veprofileMappings.CourseHours;
                                consoleDetails.IsCourseGranted = !_db.CourseGrants.Where(q => q.UserID == user.UserId && q.VEProfileID == user.VEProfileId).FirstOrDefault().IsCourseGranted;
                                consoleDetails.GrantedBy = GrantedBy;
                            }
                            else
                            {
                                veprofileMappings.TotalRemainingCourseHours -= veprofileMappings.CourseHours;
                                consoleDetails.IsCourseGranted = !_db.CourseGrants.Where(q => q.UserID == user.UserId && q.VEProfileID == user.VEProfileId).FirstOrDefault().IsCourseGranted;
                                consoleDetails.GrantedBy = GrantedBy;
                            }
                        }
                        else
                        {
                            veprofileMappings.TotalRemainingCourseHours -= veprofileMappings.CourseHours;
                            consoleDetails.IsCourseGranted = true;
                            consoleDetails.GrantedBy = GrantedBy;
                        }

                        _db.Entry(consoleDetails).State = EntityState.Modified;
                        _db.SaveChanges();
                        _db.Entry(veprofileMappings).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }

            }
            catch (Exception e)
            {

                return Ok(e.Message);

            }
            return Ok(string.Format("Successfully Granted Console Access"));
        }

        [HttpPost]
        [Route("SaveVMUser")]
        public async Task<IHttpActionResult> SaveVMUser(GrantConsoleDetails contents)
        {
            try
            {
                List<ConsoleAccessDetail> CLUsers = contents.ConsoleUsers;
                //i need to do a to list to check if there 

                foreach (var user in CLUsers)
                {
                    var consoleDetails = _db.CourseGrants.Where(q => (q.UserID == user.UserId) && (q.VEProfileID == user.VEProfileId)).FirstOrDefault();

                    if (consoleDetails == null)
                    {
                        var result = new CourseGrants
                        {
                            UserID = user.UserId,
                            VEProfileID = user.VEProfileId,
                            VEType = user.VEType,
                            IsCourseGranted = user.IsCourseGranted
                        };
                        //_db.Entry(result).State = EntityState.Added;
                        _db.CourseGrants.Add(result);
                        _db.SaveChanges();
                    }
                    else
                    {
                        consoleDetails.IsCourseGranted = user.IsCourseGranted;
                        _db.SaveChanges();
                    }
                }

            }
            catch (Exception )
            {


            }
            return Ok(string.Format("Successfully Granted Course Access"));
        }

        [HttpGet]
        [Route("GetConsoleLab")]
        public HttpResponseMessage GetConsoleLab()
        {
            try
            {
                var consoleLab = _db.VEProfiles
                    .Join(_db.VirtualEnvironments,
                    a => a.VirtualEnvironmentID,
                    b => b.VirtualEnvironmentID,
                    (a, b) => new { a, b })
                    .Join(_db.VETypes,
                    c => c.b.VETypeID,
                    d => d.VETypeID,
                    (c, d) => new { c, d })
                    .Where(q => q.d.VETypeID == 6)
                    .Select(x => new {
                        Name = x.c.a.Name,
                        veprofileid = x.c.a.VEProfileID,
                        VEType = x.d.VETypeID,
                    })
                    .ToList();

                return Request.CreateResponse(HttpStatusCode.OK, consoleLab);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);

            }
        }

        [HttpGet]
        [Route("ProvisionAlibabaConsole")]
        public async Task<IHttpActionResult> ProvisionAlibabaConsole(string email, int userId, int veProfileId)
        {
            try
            {
                string baseAddress = "http://csaws.southeastasia.cloudapp.azure.com/ali/";

                var userGroupName = _db.CloudLabUsers
                    .Join(_db.CloudLabsGroups,
                    a => a.UserGroup,
                    b => b.CloudLabsGroupID,
                    (a, b) => new { a, b })
                    .Where(q => q.a.UserId == userId)
                    .FirstOrDefault().b.CLPrefix;

                string url = "addstudent";
                var accountDetails = createAccountNameEmail("Alibaba");
                string password = createPassword();
                string accountEmail = accountDetails.AccountEmail;
                string accountName = accountDetails.AccountName;
                
                var data = new
                {
                    student_username = email.Substring(0, email.LastIndexOf('@')),
                    student_password = password,
                    parent_foldername = userGroupName.ToUpper()
                };

                var dataMessage = JsonConvert.SerializeObject(data);

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //HttpResponseMessage response = null;

                var response = await client.PostAsync(client.BaseAddress + url, new StringContent(dataMessage, Encoding.UTF8, "application/json"));

                //dynamic result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

                var requestId = response.Content.ReadAsStringAsync().Result.Substring(response.Content.ReadAsStringAsync().Result.IndexOf("REQUEST_ID : ") + 30);

                ConsoleSchedules CS = new ConsoleSchedules
                {
                    RequestId = Int32.Parse(requestId),
                    Email = email,
                    UserId = userId,
                    VEProfileId = veProfileId,
                    IsProvisioned = 2,
                    Status = "Provisioning",
                    DateCreated = DateTime.Today,
                    AccountEmail = accountEmail,
                    AccountName = accountName,
                    OrgUnit = data.parent_foldername
                };


                _db.ConsoleSchedules.Add(CS);
                _db.SaveChanges();

                return Ok(response.Content.ReadAsStringAsync());

            }
            catch (Exception e)
            {
                return Ok(string.Format(e.Message));

            }

        }

        //[HttpGet]
        //[Route("ProvisionAWS1Console")]
        //public async Task<IHttpActionResult> ProvisionAWS1Console(string email, int userId, int veProfileId)
        //{
        //    try
        //    {
        //        string baseAddressToCheck = "http://csaws.southeastasia.cloudapp.azure.com/api/available_accounts?organization_unit_id=ou-c9j5-dy5anb4w";
        //        string baseAddressToProvisionExisting = "http://csaws.southeastasia.cloudapp.azure.com/api/createStudent";
        //        string baseAddressToProvisionNew = "http://csaws.southeastasia.cloudapp.azure.com/api/createAccountwithStudent";



        //        //call baseAddressToCheck

        //        //string url = "createAccountwithStudent";
        //        var accountDetails = createAccountNameEmail("AWS");
        //        string password = createPassword();
        //        string accountEmail = accountDetails.AccountEmail;
        //        string accountName = accountDetails.AccountName;

        //        var data = new
        //        {
        //            student_username = email,
        //            student_password = password,
        //            organization_unit_id = "ou-c9j5-dy5anb4w",
        //            account_email = accountEmail,
        //            account_name = accountName
        //        };

        //        var dataMessage = JsonConvert.SerializeObject(data);

        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri(baseAddress);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //HttpResponseMessage response = null;

        //        var response = await client.PostAsync(client.BaseAddress, new StringContent(dataMessage, Encoding.UTF8, "application/json"));

        //        //dynamic result = JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);

        //        var requestId = response.Content.ReadAsStringAsync().Result.Substring(response.Content.ReadAsStringAsync().Result.IndexOf("REQUEST_ID : ") + 13);

        //        ConsoleSchedules CS = new ConsoleSchedules
        //        {
        //            RequestId = Int32.Parse(requestId),
        //            Email = email,
        //            UserId = userId,
        //            VEProfileId = veProfileId,
        //            IsProvisioned = 2,
        //            Status = "Provisioning",
        //            DateCreated = DateTime.Today,
        //            AccountEmail = accountEmail,
        //            AccountName = accountName,
        //            OrgUnit = OrganizationUnit
        //        };


        //        _db.ConsoleSchedules.Add(CS);
        //        _db.SaveChanges();

        //        return Ok(response.Content.ReadAsStringAsync());

        //    }
        //    catch (Exception e)
        //    {
        //        return Ok(string.Format(e.Message));

        //    }

        //}
        
        [HttpGet]
        [Route("CourseAvailable")]
        public IHttpActionResult CourseAvailable(int groupId)
        {
            var courseName = _db.VEProfiles
                .Join(_db.VEProfileLabCreditMappings,
                a => a.VEProfileID,
                b => b.VEProfileID,
                (a, b) => new { a, b }).Where(f => f.b.GroupID == groupId)
                .Join(_db.VirtualEnvironments, c=>c.a.VirtualEnvironmentID, d=>d.VirtualEnvironmentID, (c,d) => new { c,d})
                .Where(q=>q.d.VETypeID != 3 && q.d.VETypeID != 4)
                .Select(x => x.c.a.Name).ToList();
            return Ok(courseName);

        }

        [HttpGet]
        [Route("SendRequestCourse")]
        public HttpResponseMessage SendRequestCourse(string email, string course, string groupName, string note)
        {
            MailModel mailInfo = new MailModel();
            //MailAddressCollection receiver = new MailAddressCollection();
            //mailInfo.sendTo = "valerie@cloudswyft.com";
            //mailInfo.CC = "support@cloudswyft.com";

            string htmlBody = string.Empty;

            if (note != null)
            {
                htmlBody = "Email: <b>" + email + "</b></br>"
                    + "Course Name: <b>" + course + "</b></br>"
                    + "Group Name: <b>" + groupName + "</b></br>"
                    + "Date of Request: <b>" + DateTime.UtcNow + "</b></br></br>"
                    + "<b>Note: </b>" + note;
            }
            else
            {
                htmlBody = "Email: <b>" + email + "</b></br>"
                    + "Course Name: <b>" + course + "</b></br>"
                    + "Group Name: <b>" + groupName + "</b></br>"
                    + "Date of Request: <b>" + DateTime.UtcNow + "</b>";


            }

            mailInfo.subject = "Request for Hands on Labs";

            mailInfo.htmlBody = htmlBody;

            SendRequestLabMail(mailInfo);

            return Request.CreateResponse(HttpStatusCode.OK);

        }
        public static void SendRequestLabMail(MailModel model)
        {
            try
            {

                MailMessage mailMsg = new MailMessage();
                //mailMsg.To.Add(new MailAddress("kenneth@cloudswyft.com"));
                //mailMsg.To.Add(new MailAddress("valerie@cloudswyft.com"));
                //mailMsg.To.Add(new MailAddress("pearl@cloudswyft.com"));
                mailMsg.CC.Add(new MailAddress("support@cloudswyft.com"));
                //mailMsg.To.Add(new MailAddress("kenneth@cloudswyft.com"));
                mailMsg.From = new MailAddress(WebConfigurationManager.AppSettings["smtpSender"], "CloudSwyft Global Systems Inc");
                mailMsg.Subject = model.subject;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(model.htmlBody, null, MediaTypeNames.Text.Html));
                SmtpClient smtpClient = new SmtpClient(WebConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(WebConfigurationManager.AppSettings["smtpUser"], WebConfigurationManager.AppSettings["smtpPass"]);
                smtpClient.Credentials = credentials;

                smtpClient.Send(mailMsg);
            }
            catch (Exception)
            {
            }
        }


    }

}

