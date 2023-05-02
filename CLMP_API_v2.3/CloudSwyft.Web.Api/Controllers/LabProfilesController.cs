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
using CloudSwyft.Web.Api.Models;
using System.Web.Http.Cors;
using System.Text;
using System.IO;
using CloudSwyft.Web.Models;
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
using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json.Linq;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/LabProfiles")]
    public class LabProfilesController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private List<VMOperation> _vmOperationMultiple = new List<VMOperation>();
        private SqlConnection _conn = new SqlConnection(WebConfigurationManager.AppSettings["AuthConnectionString"]);

        // GET: api/VEProfiles
        //public VEProfileViewModel2 GetVEProfiles(string q = "", int pageSize = 0, int activePage = 1, int courseId = 0)
        //{
        //    VEProfileViewModel2 vepVM = new VEProfileViewModel2();

        //    if (String.IsNullOrWhiteSpace(q) || String.IsNullOrEmpty(q))
        //    {
        //        q = "";
        //    }

        //    var query = _db.VEProfiles
        //        .Include(vep => vep.VirtualEnvironment)
        //        .Where(vep => vep.Name.Contains(q))
        //        .OrderBy(vep => vep.Name)
        //        .ToList();

        //    if (courseId > 0)
        //    {
        //        query.RemoveAll(vep => vep.CourseID > 0);
        //    }

        //    if (pageSize == 0 && query.Count == 0)
        //    {
        //        pageSize = 10;
        //    }
        //    else if (pageSize == 0 && query.Count > 0)
        //    {
        //        pageSize = query.Count;
        //    }

        //    for (int x = 0; x < query.Count; x++)
        //    {
        //        if (!string.IsNullOrEmpty(query[x].ThumbnailURL)) query[x].ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
        //                                                ":" + Url.Request.RequestUri.Port +
        //                                                HttpContext.Current.Request.ApplicationPath +
        //                                                "/" + query[x].ThumbnailURL;

        //        if (!String.IsNullOrEmpty(query[x].VirtualEnvironment.ThumbnailURL)
        //            && query[x].VirtualEnvironment.ThumbnailURL.IndexOf(Url.Request.RequestUri.Host) < 0)
        //        {
        //            query[x].VirtualEnvironment.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
        //                                                ":" + Url.Request.RequestUri.Port +
        //                                                "/" + query[x].VirtualEnvironment.ThumbnailURL;
        //        }
        //    }

        //    vepVM.SearchFilter = q ?? "";
        //    vepVM.PageSize = pageSize;
        //    vepVM.ActivePage = activePage;
        //    vepVM.VEProfiles = query
        //        .Skip((activePage * pageSize) - pageSize)
        //        .Take(pageSize)
        //        .ToList();
        //    vepVM.TotalPages = query.Count / pageSize;
        //    if ((query.Count % pageSize) > 0)
        //        vepVM.TotalPages++;
        //    vepVM.TotalItems = query.Count;

        //    return vepVM;
        //}

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

        //[HttpPost]
        //[Route("UploadThumbnail")]
        //public async Task<IHttpActionResult> UploadThumbnail(int veProfileID)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        return InternalServerError(
        //            new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
        //    }

        //    VEProfile veProfile = _db.VEProfiles.Find(veProfileID);

        //    if (veProfile == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        var provider = new MultipartMemoryStreamProvider();
        //        await Request.Content.ReadAsMultipartAsync(provider);
        //        foreach (var file in provider.Contents)
        //        {
        //            string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/" + veProfileID.ToString());

        //            if (!Directory.Exists(dirPath))
        //            {
        //                System.IO.Directory.CreateDirectory(dirPath);
        //            }

        //            var dataStream = await file.ReadAsStreamAsync();

        //            Image image = Image.FromStream(dataStream);
        //            image = (Image)(new Bitmap(image, new Size(650, 389)));
        //            String fileName = veProfileID.ToString() + "-" + DateTime.Now.ToString("MMDDYYhhmmss") + "-Thumbnail.jpg";
        //            String fullPath = Path.Combine(dirPath, fileName);

        //            if (File.Exists(fullPath)) File.Delete(fullPath);

        //            image.Save(fullPath);

        //            veProfile.ThumbnailURL = "Content/Images/" + veProfileID.ToString() + "/" + fileName;
        //            _db.Entry(veProfile).State = EntityState.Modified;
        //            _db.SaveChanges();

        //            return Ok("Successful upload");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        return InternalServerError(e);
        //    }

        //    return Ok();
        //}


        //[HttpPost]
        //[Route("AddLabActivities")]
        //public IHttpActionResult AddLabActivities(VEProfileAddLabActivity addLabActivity)
        //{
        //    if (!VEProfileExists(addLabActivity.VEProfileID))
        //    {
        //        return NotFound();
        //    }

        //    int currentLabActivityID;

        //    for (int x = 0; x < addLabActivity.LabActivities.Count; x++)
        //    {
        //        currentLabActivityID = addLabActivity.LabActivities[x].LabActivityID;

        //        if (_db.LabActivities.Count(e => e.LabActivityID == currentLabActivityID) < 1)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            var occurCount = _db.Database.ExecuteSqlCommand("SELECT COUNT(*) FROM dbo.VEProfileLabActivity WHERE VEProfileID = " + addLabActivity.VEProfileID.ToString() + " AND LabActivityID = " + currentLabActivityID.ToString());

        //            if (occurCount < 1)
        //            {
        //                _db.Database.ExecuteSqlCommand(
        //                    "INSERT INTO dbo.VEProfileLabActivity (VEProfileID, LabActivityID) "
        //                    + "VALUES (" + addLabActivity.VEProfileID.ToString() + ',' + currentLabActivityID.ToString() + ")");
        //            }

        //        }
        //    }

        //    return Ok();
        //}

        //[HttpPut]
        //[Route("UpdateLabActivities")]
        //public IHttpActionResult UpdateLabActivities(VEProfileAddLabActivity addLabActivity)
        //{
        //    if (!VEProfileExists(addLabActivity.VEProfileID))
        //    {
        //        return NotFound();
        //    }

        //    _db.Database.ExecuteSqlCommand(
        //                "DELETE FROM dbo.VEProfileLabActivity "
        //                + "WHERE VEProfileID =" + addLabActivity.VEProfileID.ToString());

        //    int currentLabActivityID;

        //    for (int x = 0; x < addLabActivity.LabActivities.Count; x++)
        //    {
        //        currentLabActivityID = addLabActivity.LabActivities[x].LabActivityID;

        //        if (_db.LabActivities.Count(e => e.LabActivityID == currentLabActivityID) < 1)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            var occurCount = _db.Database.ExecuteSqlCommand("SELECT COUNT(*) FROM dbo.VEProfileLabActivity WHERE VEProfileID = " + addLabActivity.VEProfileID.ToString() + " AND LabActivityID = " + currentLabActivityID.ToString());

        //            if (occurCount < 1)
        //            {
        //                _db.Database.ExecuteSqlCommand(
        //                    "INSERT INTO dbo.VEProfileLabActivity (VEProfileID, LabActivityID) "
        //                    + "VALUES (" + addLabActivity.VEProfileID.ToString() + ',' + currentLabActivityID.ToString() + ")");
        //            }

        //        }
        //    }

        //    return Ok();
        //}

        //[HttpDelete]
        //[Route("RemoveLabActivity")]
        //public IHttpActionResult RemoveLabActivity(int veProfileID, int labActivityID)
        //{
        //    if (!VEProfileExists(veProfileID))
        //    {
        //        return NotFound();
        //    }

        //    _db.Database.ExecuteSqlCommand(
        //        "DELETE FROM dbo.VEProfileLabActivity "
        //        + "WHERE VEProfileID=" + veProfileID.ToString() + " AND LabActivityID=" + labActivityID.ToString());

        //    return Ok();
        //}


        //[HttpPost]
        //[Route("AttachToCourse")]
        //public async Task<IHttpActionResult> AttachToCourse(int veProfileID, int courseID)
        //{
        //    VEProfile veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == veProfileID);

        //    CloudProvider cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    if (veProfile.CourseID != 0)
        //    {
        //        List<VirtualMachine> veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //        DeleteVMs(veProfileVMs, cloudProvider);
        //    }

        //    veProfile.CourseID = courseID;

        //    _db.Entry(veProfile).State = EntityState.Modified;

        //    _db.SaveChanges();

        //    return Ok();
        //}

        //[HttpPost]
        //[Route("DetachFromCourse")]
        //public async Task<IHttpActionResult> DetachFromCourse(int veProfileID)
        //{
        //    VEProfile veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault<VEProfile>(ve => ve.VEProfileID == veProfileID);

        //    CloudProvider cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    if (veProfile.CourseID != 0)
        //    {
        //        veProfile.CourseID = 0;

        //        _db.Entry(veProfile).State = EntityState.Modified;

        //        _db.SaveChanges();

        //        List<VirtualMachine> veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //        DeleteVMs(veProfileVMs, cloudProvider);

        //        return Ok("Course successfully detached. All Virtual Machines provisioned under the course has been queued for deletion as well.");
        //    }
        //    else
        //    {
        //        return BadRequest("VE Profile not attached to any course.");
        //    }
        //}

        //[HttpPost]
        //[Route("DeleteCustomVe")]
        //public IHttpActionResult DeleteCustomVe(int veProfileId)
        //{
        //    var veProfile = _db.VEProfiles.Include(ve => ve.VirtualEnvironment).SingleOrDefault(ve => ve.VEProfileID == veProfileId);

        //    var cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    if (veProfile != null )
        //    {
        //        var veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //        DeleteVMs(veProfileVMs, cloudProvider);

        //        return Ok("Custom VM is queued for deletion");
        //    }
        //    else
        //    {
        //        return BadRequest("Custom VE not found");
        //    }
        //}

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
                virtualMachine.IsStarted = 1;
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

        //        returnMessage += ProvisionVMs(veProfile, provisionUsersList, veImage.Name, veImage.Size, suffix, veImage.Type, veImage.Protocol);
        //        count++;
        //    }

        //    //include update time of message sent then check if time is within 5 mins
        //    if (_vmOperationMultiple.Count > 0 && veProfile.DateProvisionTrigger.AddMinutes(5)<=DateTime.Now)
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
        //public IHttpActionResult ProvisionCustomVe(int veProfileId)
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
        //        new User() {UserId = 0}
        //    };

        //    var veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //    // Remove users with VMs already provisioned
        //    var provisionUsersList = userList.Where(user => !veProfileVMs.Exists(vm => vm.UserID == user.UserId)).ToList();

        //    returnMessage += ProvisionVMs(veProfile, provisionUsersList, veImages[0].Name, veImages[0].Size, "",veImages[0].Type, veImages[0].Protocol);

        //    //include update time of message sent then check if time is within 5 mins
        //    if (_vmOperationMultiple.Count > 0 && veProfile.DateProvisionTrigger.AddMinutes(5) <= DateTime.Now)
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

        //    var mapping = VirtualMachineMappingGenerator(prefix,veProfile.CourseID, 0, veProfile.VEProfileID);

        //    var vmOperation = new VMOperation
        //    {
        //        Operation = Operation.Capture,
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
        //[HttpGet]
        //[Route("EmailUsers")]
        //public HttpResponseMessage EmailUsers(int courseId, int userId)
        //{
        //    try
        //    {
        //        var userController = new UserController();
        //        var userEntry = userController.GetUserByIDInternal(userId);

        //        var courseController = new CourseMSSQL();
        //        var courseEntry = courseController.GetCourses(courseId);

        //        var mailMsg = new MailMessage();

        //        mailMsg.To.Add(new MailAddress(userEntry.Email));

        //        mailMsg.From = new MailAddress("no-reply@cloudswyft.com", "CloudSwyft Global Systems Inc");

        //        mailMsg.Subject = string.Format("Machine assigned for {0}, {1} on Course {2}", userEntry.LastName,
        //            userEntry.FirstName, courseEntry[0].FullName);
        //        mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(GenerateEmailMessage(userEntry, courseEntry), null, MediaTypeNames.Text.Html));

        //        var smtpClient = new SmtpClient(WebConfigurationManager.AppSettings["smtpHost"], Convert.ToInt32(587));
        //        var credentials = new NetworkCredential(WebConfigurationManager.AppSettings["smtpUser"], WebConfigurationManager.AppSettings["smtpPass"]);
        //        smtpClient.Credentials = credentials;

        //        smtpClient.Send(mailMsg);

        //        return Request.CreateResponse(HttpStatusCode.OK, "Success");
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
        //    }            
        //}

        //private string GenerateEmailMessage(User userEntry, List<Courses> startDateTime)
        //{
        //    var emailMsg = "";

        //    emailMsg += "<p>Hello " + userEntry.LastName + ", " + userEntry.FirstName + "</p>";
        //    emailMsg += "<p>Machine provisioned for you is ready to be accessed.</p>";
        //    emailMsg += "<p>Kindly inform your course creator/trainer if you encounter problems on this machine</p>";
        //    emailMsg += "<p>Good Luck!</p>";

        //    return emailMsg;
        //}

        //private string ProvisionVMs(VEProfile veProfile, List<User> usersList, string veName, string veSize, string suffix, string imageType, string protocol)
        //{
        //    var roleNames = "";

        //    var veType = _db.VETypes.SingleOrDefault(vt => vt.VETypeID == veProfile.VirtualEnvironment.VETypeID);
        //    //CloudProvider cloudProvider = db.CloudProviders.SingleOrDefault
        //    //    (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    var prefix = _db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0];

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

        //    #region Update Port  
        //    _conn.Open();

        //    if (Convert.ToInt32(azurePort) == 60000)
        //    {
        //        azurePort = "30000";
        //    }
        //    using (
        //        sqlCommand =
        //            new SqlCommand(
        //                string.Format(
        //                    "UPDATE TenantCodes SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //                    prefix, (Convert.ToInt32(azurePort) + 3)), _conn))
        //    {
        //        sqlCommand.ExecuteNonQuery();
        //    }

        //    _conn.Close();
        //    #endregion

        //    #region Update Port  
        //    _conn.Open();

        //    if (Convert.ToInt32(azurePort) == 60000)
        //    {
        //        azurePort = "30000";
        //    }
        //    using (
        //        sqlCommand =
        //            new SqlCommand(
        //                string.Format(
        //                    "UPDATE TenantCodes SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //                    prefix, (Convert.ToInt32(azurePort) + 3)), _conn))
        //    {
        //        sqlCommand.ExecuteNonQuery();
        //    }

        //    _conn.Close();
        //    #endregion

        //    foreach (var t in usersList)
        //    {
        //        //SendProvisionMessage(veName, prefix, veProfile.CourseID, veProfile.VEProfileID,
        //        //    usersList[x].UserId, veType.Name, cloudProvider.Name, Suffix);\


        //        if (veType != null)
        //        {

        //            var mapping = VirtualMachineMappingGenerator(prefix,veProfile.CourseID, t.UserId, veProfile.VEProfileID, (string.IsNullOrEmpty(suffix) ? 0 : Convert.ToInt32(suffix)));

        //            var vmOperation = new VMOperation
        //            {
        //                Operation = Operation.Provision,
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
        //                MachineName = mapping
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
        //    _conn.Open();

        //    if (Convert.ToInt32(azurePort) == 60000)
        //    {
        //        azurePort = "30000";
        //    }
        //    using (
        //        sqlCommand =
        //            new SqlCommand(
        //                string.Format(
        //                    "UPDATE TenantCodes SET AzurePort = {1} WHERE Code = '{0}' AND GuacamoleUrl is not null",
        //                    prefix, (Convert.ToInt32(azurePort) + 3)), _conn))
        //    {
        //        sqlCommand.ExecuteNonQuery();
        //    }

        //    _conn.Close();
        //    #endregion

        //    if (roleNames.Length > 0)
        //        roleNames = roleNames.Substring(2);

        //    return roleNames;
        //}

        //private string VirtualMachineMappingGenerator(string prefix,int courseId, int userId, int veProfileId, int suffix = 0)
        //{
        //    var mapping =
        //        _db.VirtualMachineMappings.SingleOrDefault(
        //            map =>
        //                map.CourseID == courseId && map.VEProfileID == veProfileId &&
        //                map.UserID == userId && map.MachineInstance == suffix);

        //    if (mapping != null) return mapping.RoleName;

        //    string result;

        //    while (true)
        //    {

        //        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //        var random = new Random();
        //        var generatedValues = new string(
        //            Enumerable.Repeat(chars, 11)
        //                .Select(s => s[random.Next(s.Length)])
        //                .ToArray());
        //        result = string.Format("{0}-{1}", prefix, generatedValues);
        //        if (VirtualMachineMappingExists(result))
        //        break;
        //    }           

        //    var vmMapping = new VirtualMachineMappings
        //    {
        //        RoleName = result,
        //        UserID = userId,
        //        VEProfileID = veProfileId,
        //        CourseID = courseId,
        //        MachineInstance = suffix                
        //    };



        //    _db.VirtualMachineMappings.Add(vmMapping);
        //    _db.SaveChanges();
        //    return result;
        //}

        //private bool VirtualMachineMappingExists(string roleName)
        //{
        //    return  _db.VirtualMachineMappings.SingleOrDefault(e => e.RoleName == roleName) == null;
        //}

        ////private void SendProvisionMessage(string imageName, string prefix, int courseID,
        ////    int veProfileID, int userID, string osFamily, string cloudProviderName, string suffix)
        //private void SendProvisionMessage(string cloudProviderName)
        //{
        //    string queueName = string.Empty;

        //    switch (cloudProviderName)
        //    {
        //        case "WindowsAzure":
        //            queueName = WebConfigurationManager.AppSettings["AzureProvisionQueueName"];
        //            break;
        //        case "AmazonWebServices":
        //            queueName = WebConfigurationManager.AppSettings["AmazonProvisionQueueName"];
        //            break;
        //    }

        //    SendMessage(_vmOperationMultiple, queueName);
        //    _vmOperationMultiple.Clear();
        //}

        //private void SendMessage(object message, string queueName)
        //{
        //    string connectionString = WebConfigurationManager.AppSettings["QueueConnectionString"];
        //    QueueClient Client =
        //      QueueClient.CreateFromConnectionString(connectionString, queueName); //SamplQueue is a test Queue
        //    string jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(message);

        //    BrokeredMessage sendMessage = new BrokeredMessage(jsonStr);

        //    Client.Send(sendMessage);

        //}

        //[HttpDelete]
        //[Route("DeleteUserVMs")]
        //public IHttpActionResult DeleteUserVMs(int veProfileID)
        //{
        //    VEProfile veProfile = _db.VEProfiles.Include(vp => vp.VirtualEnvironment).SingleOrDefault<VEProfile>(vp => vp.VEProfileID == veProfileID);

        //    CloudProvider cloudProvider = _db.CloudProviders.SingleOrDefault
        //        (cp => cp.CloudProviderID == veProfile.VirtualEnvironment.CloudProviderID);

        //    if (veProfile.CourseID < 1) return BadRequest("VE Profile is not yet attached to any course therefore has no VMs.");

        //    List<VirtualMachine> veProfileVMs = _db.VirtualMachines.Where(vm => vm.VEProfileID == veProfile.VEProfileID).ToList();

        //    // Deleting proper
        //    DeleteVMs(veProfileVMs, cloudProvider);

        //    return Ok("VE Profile virtual machines successfully deleted.");
        //}

        //private void DeleteVMs(List<VirtualMachine> vmList, CloudProvider cloudProvider)
        //{
        //    char[] dashSplit = { '-' };

        //    foreach (var t in vmList)
        //    {
        //        var prefix = t.RoleName.Split(dashSplit).Length > 3 ?
        //            _db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0] :
        //            string.Empty;

        //        var suffix = t.RoleName.Split(dashSplit).Length > 4 ?
        //            t.RoleName.Split(dashSplit)[4] :
        //            string.Empty;

        //        #region for multi-tenancy provisioning
        //        _conn.Open();
        //        var sqlCommand = new SqlCommand(string.Format("SELECT DISTINCT ApiUrl, AzurePort, GuacConnection, GuacamoleUrl, Region FROM TenantCodes WHERE Code = '{0}' AND guacConnection is not null", prefix), _conn);
        //        var guacConnection = string.Empty;
        //        var apiUrl = string.Empty;
        //        using (var sqlReader = sqlCommand.ExecuteReader())
        //        {
        //            while (sqlReader.Read())
        //            {
        //                guacConnection = sqlReader["GuacConnection"].ToString();
        //                apiUrl = sqlReader["ApiUrl"].ToString();
        //            }
        //        }
        //        _conn.Close();
        //        #endregion

        //        //SendDeleteMessage(prefix, vmList[x].CourseID, vmList[x].VEProfileID, vmList[x].UserID, cloudProvider.Name, suffix);
        //        var vmOperation = new VMOperation
        //        {
        //            Operation = Operation.Delete,
        //            CourseID = t.CourseID,
        //            VEProfileID = t.VEProfileID,
        //            UserID = t.UserID,
        //            Prefix = prefix,
        //            Suffix = suffix,
        //            GuacConnection = guacConnection,
        //            WebApiUrl = apiUrl
        //        };


        //        _vmOperationMultiple.Add(vmOperation);
        //    }

        //    if (_vmOperationMultiple.Count > 0)
        //    {
        //        SendDeleteMessage(cloudProvider.Name);
        //    }
        //}
        //private void SendDeleteMessage(string cloudProviderName)
        //{
        //    string queueName = string.Empty;

        //    if (cloudProviderName == "WindowsAzure")
        //    {
        //        queueName = WebConfigurationManager.AppSettings["AzureProvisionQueueName"];
        //    }
        //    else if (cloudProviderName == "AmazonWebServices")
        //    {
        //        queueName = WebConfigurationManager.AppSettings["AmazonProvisionQueueName"];
        //    }

        //    SendMessage(_vmOperationMultiple, queueName);
        //}

        [HttpPost]
        [Route("CreateLabProfile")]
        public async Task<IHttpActionResult> CreateLabProfile(VEProfile model)
        {
            try
            {
                
                //if (model.VEProfileID != 0)
                //    veProfile = _db.VEProfiles.Find(model.VEProfileID);

                //veProfile.VirtualEnvironmentID = model.VirtualEnvironmentID;
                //veProfile.Name = model.Name;
                //veProfile.Description = model.Description;
                //veProfile.ConnectionLimit = model.ConnectionLimit;
                //veProfile.CourseID = model.CourseID;
                //veProfile.ThumbnailURL = model.ThumbnailURL;
                //veProfile.IsEnabled = model.IsEnabled;
                //veProfile.DateProvisionTrigger = DateTime.Parse("2016-04-01 00:00:00");
                //veProfile.Status = model.Status;
                //veProfile.Remarks = model.Remarks;
                //veProfile.IsEmailEnabled = model.IsEmailEnabled;
                //veProfile.PassingRate = model.PassingRate;
                //veProfile.ExamPassingRate = model.ExamPassingRate;
                //veProfile.ShowExamPassingRate = model.ShowExamPassingRate;
                
                //if (model.VEProfileID != 0)
                //{
                //    _db.Entry(veProfile).State = EntityState.Modified;
                //}
                //else
                //{
                    _db.VEProfiles.Add(model);
                //}

                await _db.SaveChangesAsync();

                //if (result > 0) // add job profiles and smart checkpoints
                //{
                //    int veprofileid = veProfile.VEProfileID;
                //    addAssessmentJobProfile(model.JobProfiles, veprofileid);
                //    addSmartCheckpointAssessment(model.SmartCheckpoints, veprofileid);
                //    addExamAssessment(model.Exams, veprofileid);
                //}
                //else
                //{
                //    return BadRequest("Error Creating Skill Test");
                //}
                //Must return veprofile ID
                return Created("Created", model);
            }
            catch (Exception ex)
            {

                return BadRequest("Error Creating Skill Test" + ex);
            }
            finally
            {

            }

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

        // PUT: api/VEProfiles/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutVEProfile()
        {
            string root = HttpContext.Current.Server.MapPath("~/");
            var provider = new MultipartFormDataStreamProvider(root);

            VEProfile veProfile;

            if (Request.Content.IsMimeMultipartContent())
            {
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                int veProfileID = Convert.ToInt32(provider.FormData.GetValues("VEProfileID").First());

                veProfile = _db.VEProfiles.Find(veProfileID);

                veProfile.Name = provider.FormData.GetValues("Name").First();
                veProfile.Description = provider.FormData.GetValues("Description").First();
                veProfile.ConnectionLimit = Convert.ToInt32(provider.FormData.GetValues("ConnectionLimit").First());
                veProfile.VirtualEnvironmentID = Convert.ToInt32(provider.FormData.GetValues("virtualEnvironmentID").First());

                _db.Entry(veProfile).State = EntityState.Modified;
                _db.SaveChanges();

                //Array labActivities = Convert.ToBase64CharArray(provider.FormData.GetValues("virtualEnvironmentID").First());

                if (provider.FileData.Count > 0)
                {
                    string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/" + veProfile.VEProfileID.ToString());

                    if (!Directory.Exists(dirPath))
                    {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }

                    // This illustrates how to get the file names for uploaded files.
                    foreach (var file in provider.FileData)
                    {
                        FileInfo uploadedFile = new FileInfo(file.LocalFileName);

                        byte[] data = new byte[uploadedFile.Length];

                        // Load a filestream and put its content into the byte[]
                        FileStream dataStream = uploadedFile.OpenRead();
                        dataStream.Read(data, 0, data.Length);

                        Image image = Image.FromStream(dataStream);
                        image = (Image)(new Bitmap(image, new Size(650, 389)));
                        String fileName = veProfile.VEProfileID.ToString() + "-Thumbnail.jpg";
                        String fullPath = Path.Combine(dirPath, fileName);

                        if (File.Exists(fullPath)) File.Delete(fullPath);

                        image.Save(fullPath);

                        veProfile.ThumbnailURL = "Content/Images/" + veProfile.VEProfileID.ToString() + "/" + fileName;
                        _db.Entry(veProfile).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                }
            }
            else
            {
                var formData = await Request.Content.ReadAsFormDataAsync();

                var veProfileId = Convert.ToInt32(formData.GetValues("VEProfileID").First());

                veProfile = _db.VEProfiles.Find(veProfileId);
                
                veProfile.Name = formData.GetValues("Name").First();
                veProfile.Description = formData.GetValues("Description").First();
                veProfile.ConnectionLimit = Convert.ToInt32(formData.GetValues("ConnectionLimit").First());
                veProfile.VirtualEnvironmentID = Convert.ToInt32(formData.GetValues("VirtualEnvironmentID").First());

                _db.Entry(veProfile).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Ok(veProfile);
        }


        [HttpGet]
        [ResponseType(typeof(void))]
        public IHttpActionResult UpdateVeProfileStatus(int id, int status)
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
            
            veProfile.Status = status;
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
        [ResponseType(typeof(VEProfile))]
        public async Task<IHttpActionResult> PostVEProfile()
        {
            string root = HttpContext.Current.Server.MapPath("~/");
            var provider = new MultipartFormDataStreamProvider(root);

            VEProfile veProfile = new VEProfile();

            if (Request.Content.IsMimeMultipartContent())
            {
                //try
                //{
                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                veProfile.Name = provider.FormData.GetValues("Name").First();
                veProfile.Description = provider.FormData.GetValues("Description").First();
                veProfile.ConnectionLimit = Convert.ToInt32(provider.FormData.GetValues("ConnectionLimit").First());
                veProfile.VirtualEnvironmentID = Convert.ToInt32(provider.FormData.GetValues("VirtualEnvironmentID").First());
                veProfile.DateProvisionTrigger = DateTime.Parse("2016-04-01 00:00:00");

                _db.VEProfiles.Add(veProfile);
                _db.SaveChanges();

                //Array labActivities = Convert.ToBase64CharArray(provider.FormData.GetValues("virtualEnvironmentID").First());

                string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/" + veProfile.VEProfileID.ToString());

                if (!Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }

                if (provider.FileData.Count > 0)
                {
                    // This illustrates how to get the file names for uploaded files.
                    foreach (var file in provider.FileData)
                    {
                        if (!string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                        {
                            FileInfo uploadedFile = new FileInfo(file.LocalFileName);

                            byte[] data = new byte[uploadedFile.Length];

                            // Load a filestream and put its content into the byte[]
                            FileStream dataStream = uploadedFile.OpenRead();
                            dataStream.Read(data, 0, data.Length);

                            Image image = Image.FromStream(dataStream);
                            image = (Image)(new Bitmap(image, new Size(650, 389)));
                            String fileName = veProfile.VEProfileID.ToString() + "-Thumbnail.jpg";
                            String fullPath = Path.Combine(dirPath, fileName);

                            if (File.Exists(fullPath)) File.Delete(fullPath);

                            image.Save(fullPath);

                            veProfile.ThumbnailURL = "Content/Images/" + veProfile.VEProfileID.ToString() + "/" + fileName;
                            _db.Entry(veProfile).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    veProfile.ThumbnailURL = null;
                    _db.Entry(veProfile).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                /*
                }
                catch (Exception e)
                {
                    throw e;
                }    
                */
            }
            else
            {
                var formData = await Request.Content.ReadAsFormDataAsync();

                veProfile.Name = formData.GetValues("Name").First();
                veProfile.Description = formData.GetValues("Description").First();
                veProfile.ConnectionLimit = Convert.ToInt32(formData.GetValues("ConnectionLimit").First());
                veProfile.VirtualEnvironmentID = Convert.ToInt32(formData.GetValues("VirtualEnvironmentID").First());
                veProfile.DateProvisionTrigger = DateTime.Parse("2016-04-01 00:00:00");

                _db.VEProfiles.Add(veProfile);
                _db.SaveChanges();
            }

            return Ok(veProfile);
        }


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
                "DELETE FROM dbo.VEProfileLabActivity "
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

        private void UpdateProvisionDate(VEProfile veProfile)
        {
            veProfile.DateProvisionTrigger = DateTime.Now;
            _db.Entry(veProfile).State = EntityState.Modified;
            _db.SaveChanges();
            //return false;
        }

        //[HttpPost]
        //[Route("ProvisionUsersBB")]
        //public async Task<IHttpActionResult> ProvisionUsersBB(UserVEViewModel data)
        //{
        //    return await ProvisionUsers(data.VeProfile.VEProfileID, data.Users);
        //}

       

    }

}