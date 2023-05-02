using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Data;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Globalization;
using ExcelDataReader;

namespace CloudSwyft.Web.Api.Controllers
{

    [RoutePrefix("api/CourseGrade")]
    public class CourseGradeController : ApiController
    {
        private string AzureVM = WebConfigurationManager.AppSettings["AzureVM"];
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();
        private VirtualEnvironmentDBTenantContext _dbTenant = new VirtualEnvironmentDBTenantContext();

        [HttpGet]
        [Route("GetUserGrade")]
        public int GetUserGrade(string courseCode = "", string email = "")
        {
            var passed = 0;
            try
            {
                passed = db.CourseGrades.Where(x => x.Email == email && x.CourseCode == courseCode).Select(q => q.isPassed.Value).FirstOrDefault();
                return passed;
            }
            catch
            {
                return passed;
            }
        }

        [HttpGet]
        [Route("EdxGetUserGrade")]
        public int EdxGetUserGrade(string courseCode = "", string email = "", string host = "")
        {
            try

            {
                var tenantId = db.CloudLabsGroups.Where(x => x.EdxUrl == host).FirstOrDefault().TenantId;

                var userId = db.CloudLabUsers.Where(x => x.Email.ToLower() == email.ToLower()).Select(q => q.UserId).FirstOrDefault();

                var userGroup = db.CloudLabUsers.Where(x => x.Email.ToLower() == email.ToLower()).Select(q => q.UserGroup).FirstOrDefault();

                var isCourseExist = db.VEProfiles
                    .Join(db.VEProfileLabCreditMappings,
                    a => a.VEProfileID,
                    b => b.VEProfileID,
                    (a, b) => new { a, b })
                    .Join(db.VirtualEnvironments,
                    c => c.a.VirtualEnvironmentID,
                    d => d.VirtualEnvironmentID,
                    (c, d) => new { c, d })
                    .Any(q => q.c.b.GroupID == userGroup && q.d.Description.ToUpper() == courseCode.ToUpper());

                if (isCourseExist)
                {
                    
                    if (db.CloudLabsSchedule.Where(x => x.UserId == userId).ToList().Count() != 0)
                        if (db.CourseGrades.Where(x => x.UserId == userId).ToList().Count() != 0)
                        {
                            return db.CourseGrades.Where(x => x.CourseCode.ToUpper() == courseCode.ToUpper() && x.UserId == userId).Select(q => q.isPassed.Value).FirstOrDefault();
                        }
                        else return 0; // fail
                    else
                        return 0; // no machine
                }
                else
                    return -1; // no lab profile

            }
            catch (Exception)
            {
                return -1;
            }
        }

        [HttpPost]
        [Route("CreateUserGrade")]
        public HttpResponseMessage CreateUserGrade(int veprofileid, CourseGrade[] CourseGrades)
        {
            try
            {
                foreach (var item in CourseGrades)
                {
                    CourseGrade courseGrade = db.CourseGrades.Where(x => x.Email == item.Email && x.VEProfileId == item.VEProfileId).FirstOrDefault();
                    if(courseGrade != null)
                    {
                        courseGrade.isPassed = item.isPassed;
                        courseGrade.VEProfileId = item.VEProfileId;
                        db.Entry(courseGrade).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        item.VEProfileId = veprofileid;
                        db.CourseGrades.Add(item);
                        db.SaveChanges();
                    }
                }
                
                //foreach (var item in CourseGrades)
                //{
                //    db.Entry(item).State = EntityState.Modified;
                //    db.SaveChanges();
                //}

                return Request.CreateResponse(HttpStatusCode.OK, "OK");
            }
            catch (Exception)
            {

                return Request.CreateResponse(HttpStatusCode.BadRequest, "Not OK");
            }
        }

        [HttpGet]
        [Route("GetGradeUser")]
        public async Task<HttpResponseMessage> GetGradeUser(string role, int veprofileid, int usergroupid, string groupCode, string VEDescription)
        {
            try
            {
                var roleIDs = db.AspNetRoles.Where(anr => role.Contains(anr.Name)).Select(id => id.Id).ToList();
                var virtualEnvironmentId = db.VEProfiles.Where(x => x.VEProfileID == veprofileid).FirstOrDefault().VirtualEnvironmentID;
                var courseCode = db.VirtualEnvironments.Where(x => x.VirtualEnvironmentID == virtualEnvironmentId).FirstOrDefault().Title;
                
                var tenantId = db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == usergroupid).FirstOrDefault().TenantId;
                //var tenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault();
                
                
                var users = db.MachineLabs
                    .Where(x => x.VEProfileId == veprofileid)
                    .Join(db.CloudLabUsers,
                    vm => vm.UserId,
                    clu => clu.UserId,
                    (vm, clu) => new { vm, clu })
                    .Where(s => s.clu.TenantId == tenantId)
                    .Join(db.AspNetUserRoles,
                    p => p.clu.Id,
                    l => l.UserId,
                    (p, l) => new { p, l })
                    .Join(db.CloudLabsSchedule,
                    o => o.p.vm.MachineLabsId,
                    cls => cls.MachineLabsId,
                    (o, cls) => new { o, cls})
                    .Select(f => new gradeCourse()
                    {
                        Id = f.o.p.clu.Id,
                        UserId = f.o.p.clu.UserId,
                        Name = f.o.p.clu.LastName +", " + f.o.p.clu.FirstName,
                        Email = f.o.p.clu.Email,
                        VEProfileId = f.o.p.vm.VEProfileId,
                        isPassed = null,
                        CourseCode = courseCode,
                        GuacamoleSrc = f.o.p.vm.GuacDNS,
                        IsStarted = f.o.p.vm.IsStarted,
                        HoursRemaining = f.cls.InstructorLabHours,
                        MachineStatus = f.o.p.vm.MachineStatus,
                        ResourceId = f.o.p.vm.ResourceId,
                        NameEmail = f.o.p.clu.FirstName + " " + f.o.p.clu.LastName + " " + f.o.p.clu.Email,


                    }).OrderBy(f => f.Name).OrderBy(l => l.Name).ToList();

                ///////////////

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //DateTime dateUtc = DateTime.UtcNow;

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(AzureVM);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
                //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

                //HttpResponseMessage response = null;


                //////////////

                foreach (var user in users)
                {
                    //if user has no grade = notgraded
                    var courseGrade = db.CourseGrades.Where(x => x.VEProfileId == user.VEProfileId && x.UserId == user.UserId).FirstOrDefault();

                    if (courseGrade == null)
                        user.isPassed = -1;
                    else
                        user.isPassed = courseGrade.isPassed;

                    var isInstructorExtensionExist = db.LabHourExtensions.Where(q => q.VEProfileId == user.VEProfileId && q.UserId == user.UserId && !q.IsDeleted && q.ExtensionTypeId == 2).ToList()
                        .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);



                    if (isInstructorExtensionExist)
                        user.isExtend = true;

                    var vetype = db.VEProfiles.Where(q => q.VEProfileID == veprofileid).Join(db.VirtualEnvironments, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).FirstOrDefault().b.VETypeID;

                    //if (vetype <= 4)
                    //{
                    //    response = await client.GetAsync("labs/virtualmachine?resourceId=" + user.ResourceId);

                    //    var dataJson = JsonConvert.DeserializeObject<VMSuccess>(response.Content.ReadAsStringAsync().Result);

                    //    ////////////////////
                    //    ///
                    //    var vm = db.MachineLabs.Where(q => q.ResourceId == user.ResourceId).FirstOrDefault();
                    //    var mls = db.MachineLogs.Where(q => q.ResourceId == user.ResourceId).FirstOrDefault();

                    //    if (dataJson.Status == "running" && (user.MachineStatus == "Starting" || user.MachineStatus == "Virtual machine provisioning success"))
                    //    {
                    //        mls.Logs = '(' + dataJson.Status + ')' + dateUtc + "---" + mls.Logs;
                    //        mls.LastStatus = "Running";
                    //        mls.ModifiedDate = dateUtc;
                    //        vm.MachineStatus = "Running";
                    //        vm.IsStarted = 1;
                    //        db.Entry(vm).State = EntityState.Modified;
                    //        db.Entry(mls).State = EntityState.Modified;

                    //        user.IsStarted = 1;
                    //        user.MachineStatus = "Running";
                    //    }
                    //    if (dataJson.Status == "deallocated" && vm.MachineStatus == "Deallocating")
                    //    {
                    //        mls.Logs = '(' + dataJson.Status + ')' + dateUtc + "---" + mls.Logs;
                    //        mls.LastStatus = "Deallocated";
                    //        mls.ModifiedDate = dateUtc;

                    //        vm.MachineStatus = "Deallocated";
                    //        vm.IsStarted = 0;
                    //        db.Entry(vm).State = EntityState.Modified;
                    //        db.Entry(mls).State = EntityState.Modified;

                    //        user.IsStarted = 0;
                    //        user.MachineStatus = "Deallocated";
                    //    }

                    //    db.SaveChanges();

                    //}

                    db.SaveChanges();

                    /////////////////
                }

                //var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageScreenshots"]);

                //var blobClient = storageAccount.CreateCloudBlobClient();

                //IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();

                //List<CloudBlob> image = new List<CloudBlob>();

                //foreach (var user in users)
                //{
                //    foreach (CloudBlobContainer item in containers)
                //    {
                //        var containerName = item.Name;
                //        if (containerName.Contains(groupCode.ToLower()))
                //        {
                //            foreach (IListBlobItem item1 in item.ListBlobs(null, false))
                //            {
                //                CloudBlockBlob blob = (CloudBlockBlob)item1;

                //                if (blob.Name.Contains(user.Id + '_' + VEDescription + '_' + veprofileid))
                //                    image.Add(blob);
                //            }
                //            if (image.Count() != 0)
                //            {
                //                user.ThumbnailUpload = image.OrderByDescending(x => x.Properties.LastModified.Value).Select(x => x.Uri.AbsoluteUri).ToList();
                //                image = new List<CloudBlob>();
                //            }

                //            else
                //                user.ThumbnailUpload = null;
                //        }
                //    }
                //}




                //if (container.Exists())
                //{
                //    List<CloudBlob> image = new List<CloudBlob>();

                //    if (container.ListBlobs(null, false).Count() != 0)
                //    {
                //        foreach (var item2 in users)
                //        {
                //            foreach (IListBlobItem item in container.ListBlobs(null, false))
                //            {
                //                var blobname = Regex.Replace(item2.Name, @"\s+", "") + '-' + item2.UserId + "-" + item2.VEProfileId;
                //                CloudBlockBlob blob = (CloudBlockBlob)item;

                //                if (blob.Name.Contains(blobname))
                //                    image.Add(blob);
                //            }
                //            if (image.Count() != 0)

                //                item2.ThumbnailUpload = image.OrderByDescending(x => x.Properties.LastModified.Value).Select(x => x.Uri.AbsoluteUri).ToList();
                //            else
                //                item2.ThumbnailUpload = null;
                //        }
                //    }
                //}


                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [HttpPost]
        [Route("ImageUpload")]
        public async Task<IHttpActionResult> ImageUpload(string imageFilename, string id, string groupCode)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageScreenshots"]);

                var blobClient = storageAccount.CreateCloudBlobClient();

                //CloudBlobContainer container = blobClient.GetContainerReference(name.ToLower().Replace(" ",""));
                CloudBlobContainer container = blobClient.GetContainerReference(groupCode.ToLower().Replace(" ", ""));
                if (!container.Exists())
                {
                    container.CreateIfNotExists();
                    var permission = new BlobContainerPermissions();
                    permission.PublicAccess = BlobContainerPublicAccessType.Blob;
                    container.SetPermissions(permission);                    
                }


                //var container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);

                bool isUploaded = false;

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    bool ifExist = true;
                    int i = 0;
                    file.Headers.ContentDisposition.FileName = id + '_' + imageFilename.Replace(" ", "") + '_' + i + ".png";
                    while (ifExist)
                    {
                        var fileStream = File.OpenRead(file.LocalFileName);

                        if (blobClient.GetContainerReference(groupCode.ToLower().Replace(" ", "")).GetBlobReference(file.Headers.ContentDisposition.FileName).Exists())
                        {
                            i++;
                            file.Headers.ContentDisposition.FileName = id + '_' + imageFilename.Replace(" ", "") + '_' + i + ".png";
                        }
                        else
                        {
                            CloudBlockBlob cblob = container.GetBlockBlobReference(file.Headers.ContentDisposition.FileName);
                            cblob.UploadFromStream(fileStream);
                            ifExist = false;
                            isUploaded = true;
                        }
                    }

                }

                //return Ok(uri);
                return Ok(isUploaded);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("ImageUpload1")]
        public async Task<IHttpActionResult> ImageUpload1()
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);

                var blobClient = storageAccount.CreateCloudBlobClient();

                IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();

                List<CloudBlob> image = new List<CloudBlob>();

                foreach (CloudBlobContainer item in containers)
                {
                    var containerName = "cloudsywftstudent2";
                    if (item.Name == containerName)
                    {
                        foreach (IListBlobItem item1 in item.ListBlobs(null, false))
                        {
                            CloudBlockBlob blob = (CloudBlockBlob)item1;

                            if (blob.Name.Contains("CS103"))
                                image.Add(blob);
                        }
                    }


                }
                



                //return Ok(uri);
                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }

        [HttpGet]
        [Route("GetUserImage")]
        public async Task<IHttpActionResult> GetUserImage(string id, string groupCode, string VEDescription, int veprofileid)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageScreenshots"]);

            var blobClient = storageAccount.CreateCloudBlobClient();

            IEnumerable<CloudBlobContainer> containers = blobClient.ListContainers();

            List<CloudBlob> image = new List<CloudBlob>();

            List<string> blobNames = containers.OfType<CloudBlockBlob>().Select(b => b.Name).ToList();
            List<string> thumbNail = new List<string>();

            var veTitle = db.VEProfiles.Where(q => q.VEProfileID == veprofileid)
                .Join(db.VirtualEnvironments, a=>a.VirtualEnvironmentID, b=>b.VirtualEnvironmentID, (a,b) => new { a, b }).FirstOrDefault().b.Title;
            foreach (CloudBlobContainer item in containers)
            {
                

                var containerName = item.Name;
                if (containerName.Contains(groupCode.ToLower()))
                {
                    foreach (IListBlobItem item1 in item.ListBlobs(null, false))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item1;
                            
                        if (blob.Name.Contains(id + '_' + veTitle.Replace(" ", "") + '_' + veprofileid))
                            image.Add(blob);
                    }
                    if (image.Count() != 0)
                    {
                        thumbNail = image.OrderByDescending(x => x.Properties.LastModified.Value).Select(x => x.Uri.AbsoluteUri).ToList();
                        image = new List<CloudBlob>();
                    }

                   // else
                      //  user.ThumbnailUpload = null;
                }
            }

            return Ok(thumbNail);

        }


        [Route("BulkGrade")]
        [HttpGet]
        public async Task<IHttpActionResult> BulkGrade()
        {
            try
            {
                int counter = 0;

                List<string> lisit = new List<string>();
                CourseGrade cgr = new CourseGrade();

                var list = db.MachineLabs.Join(db.CloudLabUsers,
                    y => y.UserId,
                    w => w.UserId,
                    (y, w) => new { y, w }).Join(db.CloudLabsSchedule,
                    a => a.y.MachineLabsId,
                    b => b.MachineLabsId,
                    (a, b) => new { a, b }).Join(db.VEProfiles,
                    c => c.a.y.VEProfileId,
                    d => d.VEProfileID,
                    (c, d) => new { c, d }).Join(db.VirtualEnvironments,
                    e => e.d.VirtualEnvironmentID,
                    f => f.VirtualEnvironmentID,
                    (e, f) => new { e, f }).Select(q => new { q.f.Description, q.e.c.a.y.VEProfileId, q.e.c.a.y.UserId, q.e.c.a.w.Email, q.e.c.a.w.FirstName, q.e.c.a.w.LastName })
                    .Where(b=>b.VEProfileId == 215).ToList();

                var cg = db.CourseGrades.ToList();

                int totalCount = list.Count;
                foreach (var item in list)
                {
                    if (!cg.Any(q => q.UserId == item.UserId && q.VEProfileId == item.VEProfileId))
                    {
                        cgr.CourseCode = item.Description;
                        cgr.UserId = item.UserId;
                        cgr.VEProfileId = item.VEProfileId;
                        cgr.Email = item.Email;
                        cgr.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.LastName) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.FirstName);
                        cgr.isPassed = 1;

                        db.CourseGrades.Add(cgr);
                        db.SaveChanges();
                        counter++;
                    }
                }

                return Ok("Change: " + counter + "    Total Number to be Change: " + totalCount);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }


        [Route("BulkGradeExcel")]
        [HttpGet]
        public async Task<IHttpActionResult> BulkGradeExcel()
        {
            VHDDetails newTemp = new VHDDetails();
            var httpRequest = HttpContext.Current.Request;
            DataSet dsexcelRecords = new DataSet();
            IExcelDataReader reader = null;
            HttpPostedFile Inputfile = null;
            Stream FileStream = null;
            List<BulkGrade> studentList = new List<BulkGrade>();
            CourseGrade cgr = new CourseGrade();
            int count = 0;
            try
            {
                if (httpRequest.Files.Count > 0)
                {
                    Inputfile = httpRequest.Files[0];
                    FileStream = Inputfile.InputStream;

                    if (Inputfile != null && FileStream != null)
                    {
                        if (Inputfile.FileName.EndsWith(".xls"))
                            reader = ExcelReaderFactory.CreateBinaryReader(FileStream);
                        else if (Inputfile.FileName.EndsWith(".xlsx"))
                            reader = ExcelReaderFactory.CreateOpenXmlReader(FileStream);
                        else if (Inputfile.FileName.EndsWith(".csv"))
                            reader = ExcelReaderFactory.CreateCsvReader(FileStream);
                        else
                            return BadRequest();

                        dsexcelRecords = reader.AsDataSet();
                        reader.Close();

                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            DataTable dtStudentRecords = dsexcelRecords.Tables[0];

                            foreach (DataRow item in dtStudentRecords.Rows.Cast<DataRow>().Skip(1))
                            {
                                BulkGrade student = new BulkGrade();
                                student.Email = item.ItemArray[0].ToString();
                                student.VEProfile = Convert.ToInt32(item.ItemArray[1]);
                                studentList.Add(student);
                            }
                        }

                        foreach (var item in studentList)
                        {
                            if(db.CloudLabUsers.Any(q=>q.Email == item.Email))
                            {
                                var user = db.CloudLabUsers.Where(q => q.Email == item.Email).FirstOrDefault();
                                var cg = db.CourseGrades.ToList();
                                var ve = db.VEProfiles.Where(q => q.VEProfileID == item.VEProfile).FirstOrDefault();

                                if (!cg.Any(q => q.UserId == user.UserId && q.VEProfileId == ve.VEProfileID))
                                {
                                    cgr.CourseCode = ve.Description;
                                    cgr.UserId = user.UserId;
                                    cgr.VEProfileId = ve.VEProfileID;
                                    cgr.Email = item.Email;
                                    cgr.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.LastName) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(user.FirstName);
                                    cgr.isPassed = 1;

                                    db.CourseGrades.Add(cgr);
                                    db.SaveChanges();
                                    
                                }
                                count++;
                            }
                            
                        }
                    }
                }
                return Ok(count);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            } }
    }
}