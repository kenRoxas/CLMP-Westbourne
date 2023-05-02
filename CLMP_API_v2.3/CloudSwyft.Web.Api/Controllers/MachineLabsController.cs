using CloudSwyft.Web.Api.Models;
using ExcelDataReader;
using Microsoft.Owin.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/MachineLabs")]
    public class MachineLabsController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private VirtualEnvironmentDBTenantContext _dbTenant = new VirtualEnvironmentDBTenantContext();
        private VirtualEnvironmentDBCustomerVMContext _dbCustomer = new VirtualEnvironmentDBCustomerVMContext();
        private string AzureVM = WebConfigurationManager.AppSettings["AzureVM"];
        private string FunctionAppUrl = WebConfigurationManager.AppSettings["FunctionAppUrl"];
        private string GCPServer = WebConfigurationManager.AppSettings["GCPServer"];
        private string ProvisionMachine = WebConfigurationManager.AppSettings["ProvisionMachine"];
        private string ProvisionVMAzure = WebConfigurationManager.AppSettings["ProvisionVMAzure"];
        private string StartMachine = WebConfigurationManager.AppSettings["StartMachine"];
        private string ShutdownMachine = WebConfigurationManager.AppSettings["ShutdownMachine"];
        private string ReProvisionMachine = WebConfigurationManager.AppSettings["ReProvisionMachine"];
        private string AWSProvision = WebConfigurationManager.AppSettings["AWSProvision"];
        private string AWSVM = WebConfigurationManager.AppSettings["AWSVM"];
        private string MacOS = WebConfigurationManager.AppSettings["MacOS"];
        private string Password = "c5w1N4W5c2";

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

        //[HttpPost]
        //[Route("ProvisionVM")]
        //public async Task<IHttpActionResult> ProvisionVM(DataProvision dataContents, int tenantId, string schedBy, bool selfProv)
        //{
        //    try
        //    {
        //        VHDDetails newTemp = new VHDDetails();
        //        string dataMsg;
        //        var VMvhdUrl = _db.VEProfiles.Where(q => q.VEProfileID == dataContents.VEProfileID).Select(w => new { w.VirtualEnvironmentID, w.VEProfileID }).Join(_db.VirtualEnvironmentImages,
        //            a => a.VirtualEnvironmentID,
        //            b => b.VirtualEnvironmentID,
        //            (a, b) => new { a, b }).FirstOrDefault().b.Name;

        //        var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.ApplicationTenantId, q.SubscriptionId, q.EnvironmentCode }).FirstOrDefault();
        //        var userGroup = _db.CloudLabsGroups.Where(q => q.TenantId == tenantId).FirstOrDefault().CloudLabsGroupID;
        //        var groupPrefix = _db.CloudLabsGroups.Where(q => q.TenantId == tenantId).FirstOrDefault().CLPrefix;
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        var Environment = tenant.EnvironmentCode.Trim() == "D" ? "DEV" : tenant.EnvironmentCode.Trim() == "Q" ? "QA" : tenant.EnvironmentCode.Trim() == "U" ? "DMO" : "PRD";

        //        //HttpClient client = new HttpClient();
        //        //client.BaseAddress = new Uri(AzureVM);
        //        //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
        //        //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

        //        //client.DefaultRequestHeaders.Add("Content-Type", "application/json");
        //        var veTypeId = _db.VEProfiles.Where(q => q.VEProfileID == dataContents.VEProfileID).Join(_db.VirtualEnvironments,
        //            a => a.VirtualEnvironmentID,
        //            b => b.VirtualEnvironmentID,
        //            (a, b) => new { a, b }).FirstOrDefault().b.VETypeID;

        //        HttpResponseMessage response = null;
        //        HttpResponseMessage responseProv = null;

        //        VMStats result = null;
        //        // dynamic aqs = JsonConvert.DeserializeObject<dynamic>(dataContents.UserId);

        //        if (dataContents.MachineSize == null)
        //        {
        //            dataContents.MachineSize = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID && q.GroupID == userGroup).FirstOrDefault().MachineSize;
        //        }

        //        var vhd = new
        //        {
        //            imageName = VMvhdUrl,
        //            size = dataContents.MachineSize
        //        };

        //        string dataTemp = "";
        //        var data = JsonConvert.SerializeObject(vhd);

        //        foreach (var item in dataContents.UserId)
        //        {
        //            if (dataContents.VMEmptyData != null)
        //            {
        //                foreach (var dataVM in dataContents.VMEmptyData)
        //                {
        //                    if (dataVM.UserId == item)
        //                    {
        //                        if (veTypeId == 4)
        //                        {
        //                            newTemp = new VHDDetails
        //                            {
        //                                ImageName = VMvhdUrl,
        //                                Size = dataContents.MachineSize,
        //                                NewImageName = dataVM.MachineName.ToUpper() + "-LINUX"
        //                            };
        //                            dataTemp = JsonConvert.SerializeObject(newTemp);
        //                        }
        //                        else
        //                        {
        //                            newTemp = new VHDDetails
        //                            {
        //                                ImageName = VMvhdUrl,
        //                                Size = dataContents.MachineSize,
        //                                NewImageName = dataVM.MachineName
        //                            };
        //                            dataTemp = JsonConvert.SerializeObject(newTemp);
        //                        }

        //                    }

        //                }

        //            }

        //            MachineLabs vmm = new MachineLabs();
        //            CourseGrants cg = new CourseGrants();

        //            var isVMExist = _db.MachineLabs.Any(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID);
        //            MachineLogs mls = new MachineLogs();
        //            DateTime dateUtc = DateTime.UtcNow;

        //            var isCourseGrantExist = _db.CourseGrants.Any(q => q.UserID == item & q.VEProfileID == dataContents.VEProfileID);
        //            var courseGrant = _db.CourseGrants.Where(q => q.UserID == item & q.VEProfileID == dataContents.VEProfileID).FirstOrDefault();
        //            var veprofileMappings = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID && q.GroupID == userGroup).FirstOrDefault();

        //            var veType = _db.VEProfiles.Where(q => q.VEProfileID == dataContents.VEProfileID).Join(_db.VirtualEnvironments,
        //                a => a.VirtualEnvironmentID,
        //                b => b.VirtualEnvironmentID,
        //                (a, b) => new { a, b }).FirstOrDefault().b.VETypeID;

        //            if (!isVMExist)
        //            {
        //                if (veTypeId <= 4)
        //                {
        //                    HttpClient clientFA = new HttpClient();
        //                    clientFA.BaseAddress = new Uri(FunctionAppUrl);
        //                   // clientFA.BaseAddress = new Uri("http://localhost:7071/");
        //                    clientFA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                    if (veTypeId <= 2)
        //                    {
        //                        var q = new ProvisionDetails
        //                        {
        //                            ResourceGroup = Environment,
        //                            CLPrefix = groupPrefix,
        //                            ResourceId = "",
        //                            MachineName = "",
        //                            Password = "",
        //                            Username = "",
        //                            ImageName = VMvhdUrl,
        //                            ScheduledBy = schedBy,
        //                            VETypeID = veTypeId,
        //                            UserID = item,
        //                            VEProfileID = dataContents.VEProfileID,
        //                            TenantID = tenantId,
        //                            Size = dataContents.MachineSize
        //                        };
        //                        dataMsg = JsonConvert.SerializeObject(q);

        //                        await Task.Run(() =>
        //                        {
        //                           // clientFA.PostAsync("api/VMProvisionVer2", new StringContent(dataMsg, Encoding.UTF8, "application/json"));
        //                            clientFA.PostAsync(ProvisionMachine, new StringContent(dataMsg, Encoding.UTF8, "application/json"));
        //                        });

        //                        vmm.DateProvision = dateUtc.ToString();

        //                        vmm.VMName = q.MachineName;
        //                        vmm.MachineName = q.MachineName;
        //                        vmm.ResourceId = q.ResourceId;
        //                        vmm.UserId = item;
        //                        vmm.Username = q.Username;
        //                        vmm.Password = Encrypt(q.Password);
        //                        vmm.VEProfileId = dataContents.VEProfileID;
        //                        vmm.IsStarted = 4; // provisioning 
        //                        vmm.IsDeleted = 0;
        //                        vmm.MachineStatus = "Provisioning";
        //                        vmm.ScheduledBy = schedBy;

        //                        mls.ResourceId = q.ResourceId;
        //                        mls.LastStatus = "Provisioning";
        //                        mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
        //                        mls.ModifiedDate = dateUtc;

        //                        _db.MachineLogs.Add(mls);
        //                        _db.MachineLabs.Add(vmm);
        //                        _db.SaveChanges();

        //                    }
        //                    else
        //                    {
        //                        var qCustom = new ProvisionDetailsCustom
        //                        {
        //                            ResourceGroup = Environment,
        //                            CLPrefix = groupPrefix,
        //                            Username = "cloudswyft",
        //                            Password = "Password1!",
        //                            ResourceId = "",
        //                            MachineName = newTemp.NewImageName,
        //                            ImageName = VMvhdUrl,
        //                            ScheduledBy = schedBy,
        //                            VETypeID = veTypeId,
        //                            UserID = item,
        //                            VEProfileID = dataContents.VEProfileID,
        //                            TenantID = tenantId,
        //                            Size = dataContents.MachineSize
        //                        };
        //                        dataMsg = JsonConvert.SerializeObject(qCustom);

        //                        await Task.Run(() =>
        //                        {
        //                            //clientFA.PostAsync("api/VMProvision", new StringContent(dataMsg, Encoding.UTF8, "application/json"));
        //                            clientFA.PostAsync(ProvisionMachine, new StringContent(dataMsg, Encoding.UTF8, "application/json"));
        //                        });

        //                        vmm.DateProvision = dateUtc.ToString();

        //                        vmm.VMName = qCustom.MachineName;
        //                        vmm.MachineName = qCustom.MachineName;
        //                        vmm.ResourceId = qCustom.ResourceId;
        //                        vmm.UserId = item;
        //                        vmm.Username = qCustom.Username;
        //                        vmm.Password = Encrypt(qCustom.Password);
        //                        vmm.VEProfileId = dataContents.VEProfileID;
        //                        vmm.IsStarted = 4; // provisioning 
        //                        vmm.IsDeleted = 0;
        //                        vmm.MachineStatus = "Provisioning";
        //                        vmm.ScheduledBy = schedBy;

        //                        mls.ResourceId = qCustom.ResourceId;
        //                        mls.LastStatus = "Provisioning";
        //                        mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
        //                        mls.ModifiedDate = dateUtc;

        //                        _db.MachineLogs.Add(mls);
        //                        _db.MachineLabs.Add(vmm);
        //                        _db.SaveChanges();
        //                    }

        //                    //response = await client.PostAsync("labs/virtualmachine/", new StringContent(data, Encoding.UTF8, "application/json"));
        //                    //result = JsonConvert.DeserializeObject<VMStats>(response.Content.ReadAsStringAsync().Result);


        //                } //windows or linux azure 
        //                //else if (veType == 3 || veTypeId == 4)
        //                //{
        //                //    response = await client.PostAsync("labs/virtualmachine/template", new StringContent(dataTemp, Encoding.UTF8, "application/json"));
        //                //    result = JsonConvert.DeserializeObject<VMStats>(response.Content.ReadAsStringAsync().Result);

        //                //    vmm.DateProvision = dateUtc.ToString();

        //                //    vmm.VMName = "NO VM NAME";
        //                //    vmm.MachineName = "NO Computer NAME";
        //                //    vmm.ResourceId = result.ResourceId;
        //                //    vmm.UserId = item;
        //                //    vmm.VEProfileId = dataContents.VEProfileID;
        //                //    vmm.IsStarted = 4; // provisioning 
        //                //    vmm.IsDeleted = Convert.ToInt32(result.IsDeleted);
        //                //    vmm.MachineStatus = result.LastStatusDescription;
        //                //    vmm.ScheduledBy = schedBy;

        //                //    mls.ResourceId = result.ResourceId;
        //                //    mls.LastStatus = "Provisioning";
        //                //    mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
        //                //    mls.ModifiedDate = dateUtc;

        //                //    _db.MachineLogs.Add(mls);
        //                //    _db.MachineLabs.Add(vmm);
        //                //    _db.SaveChanges();

        //                //} // windows or linux empty azure 
        //                else if (veType == 9)
        //                {
        //                    MachineLabs ml = new MachineLabs();

        //                    ml.DateProvision = DateTime.UtcNow.ToString();
        //                    ml.ResourceId = "No Instance";
        //                    ml.VEProfileId = dataContents.VEProfileID;
        //                    ml.UserId = item;
        //                    ml.MachineStatus = "Provisioning";
        //                    ml.MachineName = "UI" + item.ToString() + "VE" + dataContents.VEProfileID;
        //                    ml.IsStarted = 4;
        //                    ml.ScheduledBy = schedBy;
        //                    ml.RunningBy = 0;
        //                    ml.FQDN = null;
        //                    ml.GuacDNS = null;
        //                    ml.Username = "cloudswyft";
        //                    ml.Password = Encrypt(Password);
        //                    ml.IsDeleted = 0;

        //                    _db.MachineLabs.Add(ml);
        //                    _db.SaveChanges();
        //                    await ProvisionAWSWindows(dataContents.VEProfileID, item, dataContents.MachineSize, schedBy, tenantId, veTypeId);

        //                } //aws windows
        //                else if (veType == 8)
        //                {
        //                    MachineLabs ml = new MachineLabs();

        //                    ml.DateProvision = DateTime.UtcNow.ToString();
        //                    ml.ResourceId = "No Instance";
        //                    ml.VEProfileId = dataContents.VEProfileID;
        //                    ml.UserId = item;
        //                    ml.MachineStatus = "Provisioning";
        //                    ml.MachineName = "UI" + item.ToString() + "VE" + dataContents.VEProfileID;
        //                    ml.IsStarted = 4;
        //                    ml.ScheduledBy = schedBy;
        //                    ml.RunningBy = 0;
        //                    ml.FQDN = null;
        //                    ml.GuacDNS = null;
        //                    ml.Username = "cloudswyft";
        //                    ml.Password = Encrypt(Password);
        //                    ml.IsDeleted = 0;

        //                    _db.MachineLabs.Add(ml);
        //                    _db.SaveChanges();
        //                    await ProvisionMacOS(dataContents.VEProfileID, item, dataContents.MachineSize, schedBy, tenantId, veTypeId);
        //                }

        //                if (!isCourseGrantExist)
        //                {
        //                    var grantedBy = _db.CloudLabUsers.Where(q => q.Email == schedBy).FirstOrDefault().UserId;

        //                    cg.UserID = item;
        //                    cg.VEProfileID = dataContents.VEProfileID;
        //                    cg.IsCourseGranted = true;
        //                    cg.VEType = veType;
        //                    cg.GrantedBy = grantedBy;
        //                    _db.CourseGrants.Add(cg);
        //                }
        //                else
        //                {
        //                    courseGrant.IsCourseGranted = true;
        //                    _db.Entry(courseGrant).State = EntityState.Modified;
        //                    _db.SaveChanges();
        //                }

        //                if (!selfProv)
        //                {
        //                    veprofileMappings.TotalRemainingCourseHours -= veprofileMappings.CourseHours;
        //                    _db.Entry(veprofileMappings).State = EntityState.Modified;
        //                    _db.SaveChanges();
        //                }


        //            } // provision
        //            else
        //            {
        //                var isVMFailed = _db.MachineLabs.Any(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID && q.IsStarted == 3);

        //                if (isVMFailed) //re-provision
        //                {
        //                    //HttpClient httpclient = new HttpClient();

        //                    //var resourceId = _db.MachineLabs.Where(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID && q.IsStarted == 3).FirstOrDefault().ResourceId;
        //                    //var resourceIds = _db.MachineLabs.Where(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID && q.IsStarted == 3).FirstOrDefault();
        //                    //var logs = _db.MachineLogs.Where(q => q.ResourceId == resourceId).FirstOrDefault();

        //                    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //                    //var operation = new
        //                    //{
        //                    //    ResourceId = resourceId
        //                    //};

        //                    //var url = AzureVM + "labs/virtualmachine";
        //                    //var payload = new StringContent(JsonConvert.SerializeObject(operation), Encoding.UTF8, "application/json");
        //                    //var clientDelete = new HttpClient();
        //                    //var request = new HttpRequestMessage
        //                    //{
        //                    //    Method = HttpMethod.Delete,
        //                    //    RequestUri = new Uri(url),
        //                    //    Content = payload
        //                    //};

        //                    //clientDelete.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);
        //                    //clientDelete.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
        //                    //var responseDelete = await client.SendAsync(request);
        //                    //responseDelete.EnsureSuccessStatusCode();
        //                    //string resultDelete = responseDelete.Content.ReadAsStringAsync().Result;


        //                    //_db.MachineLabs.Remove(resourceIds);
        //                    //_db.MachineLogs.Remove(logs);
        //                    //_db.SaveChanges();


        //                    //re-provision
        //                    //responseProv = await client.PostAsync("labs/virtualmachine/", new StringContent(data, Encoding.UTF8, "application/json"));
        //                    //result = JsonConvert.DeserializeObject<VMStats>(responseProv.Content.ReadAsStringAsync().Result);

        //                    vmm.DateProvision = dateUtc.ToString();

        //                    vmm.VMName = "NO VM NAME";
        //                    vmm.MachineName = "NO Computer NAME";
        //                    vmm.ResourceId = result.ResourceId;
        //                    vmm.UserId = item;
        //                    vmm.VEProfileId = dataContents.VEProfileID;
        //                    vmm.IsStarted = 4; // provisioning 
        //                    vmm.IsDeleted = Convert.ToInt32(result.IsDeleted);
        //                    vmm.MachineStatus = result.LastStatusDescription;
        //                    vmm.ScheduledBy = schedBy;

        //                    mls.ResourceId = result.ResourceId;
        //                    mls.LastStatus = "Provisioning";
        //                    mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
        //                    mls.ModifiedDate = dateUtc;

        //                    _db.MachineLogs.Add(mls);
        //                    _db.MachineLabs.Add(vmm);
        //                    _db.SaveChanges();
        //                }
        //                else //extend hours
        //                {
        //                    var cls = _db.CloudLabsSchedule.Where(q => q.UserId == item && q.VEProfileID == dataContents.VEProfileID).FirstOrDefault();
        //                    var ml = _db.MachineLabs.Where(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID).FirstOrDefault();
        //                    var vemap = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID).FirstOrDefault();

        //                    vemap.TotalRemainingCourseHours -= dataContents.CourseHours;
        //                    cls.TimeRemaining += TimeSpan.FromHours(dataContents.CourseHours / 60).TotalSeconds;
        //                    cls.LabHoursTotal += dataContents.CourseHours;
        //                    ml.DateProvision = DateTime.UtcNow.ToString();

        //                    _db.Entry(ml).State = EntityState.Modified;
        //                    _db.Entry(cls).State = EntityState.Modified;
        //                    _db.Entry(vemap).State = EntityState.Modified;
        //                    _db.SaveChanges();
        //                }

        //            }
        //        }
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return Ok();
        //    }
        //}

        [HttpGet]
        [Route("GetCourseVMs")]
        public async Task<HttpResponseMessage> GetCourseVMs(int userId = 0, int tenantId = 0)
        {
            var coursesList = new List<CourseDetails>();
            var GetStudentConsole = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/";
            var url = "dev/get_current_budget_of_student/";

            var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.TenantId, q.ClientCode, q.GuacamoleURL, q.GuacConnection, q.EnvironmentCode }).FirstOrDefault();
            var envi = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "DEV" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "Q" ? "QA" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "U" ? "DMO" : "PRD";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(AzureVM);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

            HttpResponseMessage response = null;

            GuacamoleInstancesController guac = new GuacamoleInstancesController();

            try
            {
                var veProfileIds = _db.VirtualEnvironments.Where(q => q.VETypeID == 6 || q.VETypeID == 7) //for consoles
                    .Join(_db.VEProfiles,
                    a => a.VirtualEnvironmentID,
                    b => b.VirtualEnvironmentID,
                    (a, b) => new { a, b })
                    .Join(_db.VEProfileLabCreditMappings,
                    c => c.b.VEProfileID,
                    d => d.VEProfileID,
                    (c, d) => new { c, d })
                    .Where(q => q.d.GroupID == _db.CloudLabUsers.Where(x => x.UserId == userId).FirstOrDefault().UserGroup)
                    .Select(s => new { s.c.b.VEProfileID, s.c.b.Name, s.c.a.VETypeID, s.c.b.ThumbnailURL }).ToList();

                var vmUsers = _db.CourseGrants
                    .Join(_db.CloudLabUsers,
                    a => a.UserID,
                    b => b.UserId,
                    (a, b) => new { a, b })
                    .Join(_db.VEProfiles,
                    c => c.a.VEProfileID,
                    d => d.VEProfileID,
                    (c, d) => new { c, d }).Where(x => x.c.a.UserID == userId)
                    .Join(_db.VEProfileLabCreditMappings,
                    e => new { ab = e.c.b.UserGroup, aa = e.d.VEProfileID },
                    f => new { ab = f.GroupID, aa = f.VEProfileID },
                    (e, f) => new { e, f })
                    .Join(_db.VirtualEnvironments,
                    g => g.e.d.VirtualEnvironmentID,
                    h => h.VirtualEnvironmentID,
                    (g, h) => new { g, h })
                    .Where(w => w.g.e.c.a.IsCourseGranted)
                    .Select(s => new { s.g.e.d.VEProfileID, s.g.e.d.Name, s.h.VETypeID, s.g.e.d.ThumbnailURL, s.g.e.c.a.IsCourseGranted, s.g.f.MachineSize, s.h.Title }).Where(q => q.VETypeID != 6 && q.VETypeID != 7).ToList();  // not for consoles

                foreach (var item in veProfileIds)
                {
                    if (item.VETypeID == 6)
                    {
                        var consoleDGrants = _db.CourseGrants.Where(q => (q.UserID == userId) && (q.VEProfileID == item.VEProfileID)).FirstOrDefault();
                        var consoleDSchedules = _db.ConsoleSchedules.Where(q => (q.UserId == userId) && (q.VEProfileId == item.VEProfileID)).FirstOrDefault();

                        var BLimit = new BudgetLimit
                        {
                            Amount = "0",
                            Unit = "",
                        };

                        var ASpend = new DataSpend
                        {
                            ActualSpend = new ActualSpend { Amount = "0", Unit = "" }
                        };

                        if (consoleDSchedules != null)
                        {
                            var result = ApiCall("GET", GetStudentConsole, url + consoleDSchedules.AccountId, null);
                            var dataJSON = JsonConvert.DeserializeObject<ConsoleDetail>(result.Result);

                            if (consoleDGrants.IsCourseGranted == true)
                            {

                                var result2 = new CourseDetails
                                {
                                    UserId = consoleDGrants.UserID,
                                    Name = item.Name,
                                    veprofileid = consoleDGrants.VEProfileID,
                                    VEType = consoleDGrants.VEType,
                                    IsCourseGranted = consoleDGrants.IsCourseGranted,
                                    IsProvisioned = consoleDSchedules.IsProvisioned,
                                    ConsoleLink = consoleDSchedules.ConsoleLink,
                                    Thumbnail = item.ThumbnailURL,
                                    Data_transfer_budget_limit = dataJSON.Data_transfer_budget_limit ?? BLimit,
                                    Cost_budget_limit = dataJSON.Cost_budget_limit ?? BLimit,
                                    Actual_data_transfer_spend = dataJSON.Actual_data_transfer_spend ?? ASpend,
                                    Actual_costs_spend = dataJSON.Actual_costs_spend ?? ASpend,
                                    Is_suspended = dataJSON.Is_suspended
                                };

                                coursesList.Add(result2);
                            }
                        }
                        else
                        {
                            if (consoleDGrants != null && consoleDGrants.IsCourseGranted == true)
                            {
                                var result1 = new CourseDetails
                                {
                                    UserId = consoleDGrants.UserID,
                                    Name = item.Name,
                                    veprofileid = consoleDGrants.VEProfileID,
                                    VEType = consoleDGrants.VEType,
                                    IsCourseGranted = consoleDGrants.IsCourseGranted,
                                    IsProvisioned = 0,
                                    ConsoleLink = ""
                                };

                                coursesList.Add(result1);
                            }
                        }
                    }
                    else
                    {
                        var consoleCourse = new CourseDetails();

                        if (_db.ConsoleSchedules.Any(q => q.UserId == userId && q.VEProfileId == item.VEProfileID))
                        {
                            var course = _db.ConsoleSchedules.Where(q => q.UserId == userId && q.VEProfileId == item.VEProfileID).FirstOrDefault();
                            consoleCourse.IsProvisioned = course.IsProvisioned;
                            consoleCourse.veprofileid = item.VEProfileID;
                            consoleCourse.UserId = userId;
                            consoleCourse.Name = item.Name;
                            consoleCourse.VEType = item.VETypeID; //console
                            consoleCourse.IsStarted = null;
                            //consoleCourse.LabHoursRemaining = null;
                            consoleCourse.ConsoleLink = course.ConsoleLink;
                            consoleCourse.Thumbnail = item.ThumbnailURL;
                        }
                        else
                        {
                            consoleCourse.Name = item.Name;
                            consoleCourse.veprofileid = item.VEProfileID;
                            consoleCourse.UserId = userId;
                            consoleCourse.Name = item.Name;
                            consoleCourse.VEType = item.VETypeID; //console
                            consoleCourse.IsStarted = null;
                            //consoleCourse.LabHoursRemaining = null;
                            consoleCourse.Thumbnail = item.ThumbnailURL;
                        }

                        coursesList.Add(consoleCourse);
                    }
                } //for consoles

                foreach (var item in vmUsers)
                {
                    var courseDetail = new CourseDetails();

                    if (_db.MachineLabs.Any(q => q.UserId == userId && q.VEProfileId == item.VEProfileID))
                    {
                        var course = _db.MachineLabs.Where(q => q.UserId == userId && q.VEProfileId == item.VEProfileID).FirstOrDefault();

                        var sched = _db.CloudLabsSchedule.Any(e => e.MachineLabsId == course.MachineLabsId) ?
                            _db.CloudLabsSchedule.Where(f => f.MachineLabsId == course.MachineLabsId).FirstOrDefault() : null;

                        var isUserHasExtension = _db.LabHourExtensions.Where(g => g.VEProfileId == item.VEProfileID && g.UserId == userId && g.IsDeleted == false && g.ExtensionTypeId == 1).ToList()
                            .Any(h => h.StartDate.ToLocalTime() <= DateTime.Now && h.EndDate.ToLocalTime() > DateTime.Now);

                        courseDetail.veprofileid = item.VEProfileID;
                        courseDetail.UserId = userId;
                        courseDetail.Name = item.Name;
                        courseDetail.VEType = item.VETypeID;

                        courseDetail.TimeRemaining = sched != null ? sched.TimeRemaining : 0;

                        courseDetail.RunningBy = course.RunningBy;
                        courseDetail.MachineStatus = course.MachineStatus;
                        courseDetail.IsStarted = course.IsStarted;
                        courseDetail.LabHoursTotal = sched != null ? sched.LabHoursTotal : 0;
                        courseDetail.InstructorLabHours = sched != null ? sched.InstructorLabHours : 0;
                        courseDetail.GuacamoleUrl = course.GuacDNS;
                        courseDetail.Thumbnail = item.ThumbnailURL;
                        courseDetail.ResourceId = course.ResourceId;
                        courseDetail.IsCourseGranted = item.IsCourseGranted;
                        courseDetail.IsProvisioned = 1;
                        courseDetail.Username = course.Username;
                        courseDetail.Password = course.Password != null ? Decrypt(course.Password) : null;
                        courseDetail.MachineStatus = course.MachineStatus;
                        courseDetail.FQDN = course.FQDN;
                        courseDetail.CourseCode = item.Title;
                        courseDetail.IsExtend = isUserHasExtension;
                        courseDetail.MachineLabsId = course.MachineLabsId;
                        courseDetail.IpAddress = course.IpAddress;

                    }
                    else
                    {
                        courseDetail.Name = item.Name;
                        courseDetail.veprofileid = item.VEProfileID;
                        courseDetail.UserId = userId;
                        courseDetail.Name = item.Name;
                        courseDetail.VEType = item.VETypeID;
                        courseDetail.IsStarted = null;
                        //courseDetail.LabHoursRemaining = null;
                        courseDetail.Thumbnail = item.ThumbnailURL;
                        courseDetail.IsCourseGranted = item.IsCourseGranted;
                        courseDetail.MachineSize = item.MachineSize;
                        courseDetail.CourseCode = item.Title;

                    }

                    coursesList.Add(courseDetail);
                }

                foreach (var item in coursesList)
                {
                    VMSuccess dataJson = new VMSuccess();

                    var vm = _db.MachineLabs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                    var mls = _db.MachineLogs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                    DateTime dateUtc = DateTime.UtcNow;

                    #region
                    // MacStorageAccounts ms = _db.MacStorageAccounts.Where(q => q.UserId == item.UserId).FirstOrDefault();


                    //if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId) && (item.VEType == 1 || item.VEType == 2)) //q.MachineStatus == "Starting" || q.MachineStatus == "Shutting Down"))
                    //{
                    //    response = await client.GetAsync("labs/virtualmachine/?resourceid=" + item.ResourceId);

                    //    if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId && q.MachineStatus == "Virtual machine provisioning started"))
                    //    {
                    //        response = await client.GetAsync("labs/virtualmachine/?resourceid=" + item.ResourceId);

                    //        dataJson = JsonConvert.DeserializeObject<VMSuccess>(response.Content.ReadAsStringAsync().Result);

                    //        if (dataJson.VirtualMachineName != null)
                    //        {
                    //            vm.MachineName = dataJson.ComputerName == null ? "NO Computer NAME" : dataJson.ComputerName;
                    //            vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;

                    //            if (dataJson.Status != null && dataJson.Status.ToLower() == "deallocating")
                    //            {
                    //                vm.MachineStatus = "Deallocating";
                    //                vm.IsStarted = 2;
                    //                vm.RunningBy = 0;
                    //            }

                    //            _db.Entry(vm).State = EntityState.Modified;
                    //            _db.SaveChanges();


                    //            if (dataJson.ProvisioningStatus == "Aborted") //means failed
                    //            {
                    //                vm.MachineStatus = "Failed";
                    //                vm.IsStarted = 3;
                    //                _db.Entry(vm).State = EntityState.Modified;
                    //                _db.SaveChanges();
                    //            }

                    //            else //success 
                    //            {

                    //                CloudLabsSchedule cls = new CloudLabsSchedule();
                    //                var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == item.veprofileid && (_db.CloudLabUsers.Where(s => s.UserId == userId).FirstOrDefault().UserGroup) == q.GroupID).FirstOrDefault();

                    //                if (vm.GuacDNS == null && vm.MachineName != null) {
                    //                    var guacURL = guac.AddMachineToDatabase(vm.MachineName, tenant.GuacamoleURL, tenant.GuacConnection, dataJson.Username, dataJson.Password, item.VEType, tenant.EnvironmentCode, dataJson.Region, dataJson.Fqdn);
                    //                    if (guacURL != "")
                    //                    {
                    //                        vm.GuacDNS = guacURL;
                    //                        vm.DateProvision = dateUtc.ToString(); ;
                    //                        vm.Username = dataJson.Username;
                    //                        vm.Password = Encrypt(dataJson.Password);
                    //                        vm.FQDN = dataJson.Fqdn;

                    //                        cls.LabHoursTotal = item.LabHoursTotal;
                    //                        cls.InstructorLabHours = 120;

                    //                        _db.Entry(vm).State = EntityState.Modified;
                    //                        _db.SaveChanges();

                    //                        mls.LastStatus = "Provisioned";
                    //                        mls.Logs = '(' + mls.LastStatus + ')' + dateUtc + "---" + mls.Logs;
                    //                        mls.ModifiedDate = dateUtc;

                    //                        cls.VEProfileID = vm.VEProfileId;
                    //                        cls.UserId = vm.UserId;
                    //                        cls.MachineLabsId = vm.MachineLabsId;
                    //                        cls.TimeRemaining = TimeSpan.FromHours(labCredit.CourseHours / 60).TotalSeconds;
                    //                        cls.LabHoursTotal = labCredit.CourseHours;
                    //                        cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;

                    //                        var isCLSExist = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == vm.MachineLabsId);

                    //                        if (!isCLSExist)
                    //                        {
                    //                            _db.Entry(mls).State = EntityState.Modified;
                    //                            _db.CloudLabsSchedule.Add(cls);
                    //                            _db.SaveChanges();
                    //                        }
                    //                    }
                    //                }

                    //            }

                    //        }
                    //        else
                    //        {
                    //            var responseJson = JsonConvert.DeserializeObject<VMStats>(response.Content.ReadAsStringAsync().Result);

                    //            if (responseJson.ProvisioningStatus == "Aborted")
                    //            {
                    //                vm.VMName = responseJson.VirtualMachineName == null ? "NO Computer NAME" : responseJson.VirtualMachineName;
                    //                vm.MachineStatus = "Failed";
                    //                vm.IsStarted = 3;
                    //            }
                    //            _db.Entry(vm).State = EntityState.Modified;
                    //            _db.SaveChanges();
                    //        }

                    //    }  // provisioning
                    //    if (response.Content.ReadAsStringAsync().Result.Contains("lastStatus"))
                    //    {
                    //        dataJson = JsonConvert.DeserializeObject<VMSuccess>(response.Content.ReadAsStringAsync().Result);
                    //        vm.MachineName = dataJson.ComputerName;

                    //        if (vm.GuacDNS == null && vm.MachineName != null)
                    //        {
                    //            CloudLabsSchedule cls = new CloudLabsSchedule();
                    //            var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == item.veprofileid && (_db.CloudLabUsers.Where(s => s.UserId == userId).FirstOrDefault().UserGroup) == q.GroupID).FirstOrDefault();

                    //            if (dataJson.Status != null && dataJson.Status.ToLower() == "deallocating")
                    //            {
                    //                vm.MachineStatus = "Deallocating";
                    //                vm.IsStarted = 2;
                    //                vm.RunningBy = 0;
                    //                _db.Entry(vm).State = EntityState.Modified;
                    //                _db.SaveChanges();
                    //            }

                    //            var guacURL = guac.AddMachineToDatabase(vm.MachineName, tenant.GuacamoleURL, tenant.GuacConnection, dataJson.Username, dataJson.Password, item.VEType, tenant.EnvironmentCode, dataJson.Region, dataJson.Fqdn);
                    //            if (guacURL != "")
                    //            {
                    //                vm.GuacDNS = guacURL;
                    //                vm.DateProvision = dateUtc.ToString();
                    //                vm.Username = dataJson.Username;
                    //                vm.Password = Encrypt(dataJson.Password);
                    //                vm.FQDN = dataJson.Fqdn;

                    //                _db.Entry(vm).State = EntityState.Modified;
                    //                _db.SaveChanges();

                    //                cls.LabHoursTotal = item.LabHoursTotal;
                    //                cls.InstructorLabHours = 120;


                    //                mls.LastStatus = "Provisioned";
                    //                mls.Logs = '(' + mls.LastStatus + ')' + dateUtc + "---" + mls.Logs;
                    //                mls.ModifiedDate = dateUtc;

                    //                cls.VEProfileID = vm.VEProfileId;
                    //                cls.UserId = vm.UserId;
                    //                cls.MachineLabsId = vm.MachineLabsId;
                    //                cls.TimeRemaining = TimeSpan.FromHours(labCredit.CourseHours / 60).TotalSeconds;
                    //                cls.LabHoursTotal = labCredit.CourseHours;
                    //                cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;

                    //                var isCLSExist = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == vm.MachineLabsId);

                    //                if (!isCLSExist)
                    //                {
                    //                    _db.Entry(mls).State = EntityState.Modified;
                    //                    _db.CloudLabsSchedule.Add(cls);
                    //                    _db.SaveChanges();
                    //                }

                    //            }
                    //        }

                    //        if (mls.ModifiedDate == null)
                    //        {
                    //            mls.ModifiedDate = dateUtc;
                    //            _db.Entry(mls).State = EntityState.Modified;
                    //            _db.SaveChanges();
                    //        }

                    //        if (dataJson.Status == "running" && dataJson.IsReadyForUsage == true && (vm.MachineStatus == "Starting" || vm.MachineStatus == "Virtual machine provisioning started"))
                    //        {
                    //            mls.Logs = "(Running-Waiting to Render)" + dateUtc + "---" + mls.Logs;
                    //            mls.LastStatus = "Running-Waiting to Render";
                    //            mls.ModifiedDate = dateUtc;

                    //            vm.MachineStatus = "Running-Waiting to Render";
                    //            //vm.IsStarted = 1;
                    //            _db.Entry(vm).State = EntityState.Modified;
                    //            _db.Entry(mls).State = EntityState.Modified;
                    //            _db.SaveChanges();
                    //        }

                    //        if (dataJson.Status == "running" && dataJson.IsReadyForUsage == true && (vm.MachineStatus == "Running-Waiting to Render" || vm.MachineStatus == "Failed") && (mls.ModifiedDate.Value.AddMinutes(2) < dateUtc)) // scenario na nagfailed yung machine upon starting
                    //        {
                    //            mls.Logs = "(Running)" + dateUtc + "---" + mls.Logs;
                    //            mls.LastStatus = "Running";
                    //            mls.ModifiedDate = dateUtc;

                    //            vm.MachineStatus = "Running";
                    //            vm.IsStarted = 1;
                    //            _db.Entry(vm).State = EntityState.Modified;
                    //            _db.Entry(mls).State = EntityState.Modified;
                    //            _db.SaveChanges();

                    //        }

                    //        if (dataJson.Status == "deallocated" && (vm.MachineStatus == "Deallocating" || vm.MachineStatus == "Failed" || vm.MachineStatus == "Virtual machine provisioning started") && dataJson.IsReadyForUsage == true) // 1 scenario na nagfailed yung machine upon deallocating
                    //        {
                    //            mls.Logs = '(' + dataJson.Status + ')' + dateUtc + "---" + mls.Logs;
                    //            mls.LastStatus = "Deallocated";
                    //            mls.ModifiedDate = dateUtc;

                    //            vm.MachineStatus = "Deallocated";
                    //            vm.IsStarted = 0;
                    //            _db.Entry(vm).State = EntityState.Modified;
                    //            _db.Entry(mls).State = EntityState.Modified;
                    //            _db.SaveChanges();
                    //        }

                    //    } // provisioning completed 
                    //    if (response.Content.ReadAsStringAsync().Result.Contains("Failed") || response.Content.ReadAsStringAsync().Result.Contains("failure"))
                    //    {
                    //        if (vm.VMName != null && vm.VMName != null && vm.FQDN != null)
                    //        {
                    //            // no changes (scenario if failed status pero success naman ang provision)
                    //        }
                    //        else
                    //        {
                    //            vm.MachineStatus = "Failed";
                    //            vm.IsStarted = 3;
                    //            _db.Entry(vm).State = EntityState.Modified;
                    //            _db.SaveChanges();
                    //        }
                    //    } //failed Status
                    //} //for Azure windows and linux
                    #endregion
                    #region WORKAROUND WILL DELETE AFTER

                    if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId) && (item.VEType <= 4)) // for AWS windows
                    {


                    } //for AZURE WINDOWS

                    #endregion

                    if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId) && (item.VEType == 9 || item.VEType == 8)) // for AWS windows
                    {
                        try
                        {
                            HttpClient clientAWS = new HttpClient();
                            HttpResponseMessage responseGetDetails = null;
                            JObject getDetails = null;

                            clientAWS.BaseAddress = new Uri(AWSVM);
                            clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            HttpClient clientAWSHB = new HttpClient();
                            HttpResponseMessage responseGetDetailsHB = null;

                            clientAWSHB.BaseAddress = new Uri(AWSVM);
                            clientAWSHB.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            HttpClient clientMac = new HttpClient();

                            clientMac.BaseAddress = new Uri(MacOS);
                            clientMac.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                            if (vm.ResourceId != "No Instance")
                            {
                                var jsonDetailsHB = new
                                {
                                    instance_id = vm.ResourceId,
                                    region = "ap-southeast-1"
                                };

                                var jsonDataHB = JsonConvert.SerializeObject(jsonDetailsHB);

                                //var jsonDetails = new
                                //{
                                //    instance_id = vm.ResourceId,
                                //    region = "ap-southeast-1"
                                //};

                                //var jsonData = JsonConvert.SerializeObject(jsonDetails);

                                responseGetDetailsHB = await clientAWSHB.PostAsync("dev/minutes_rendered", new StringContent(jsonDataHB, Encoding.UTF8, "application/json"));

                                if (responseGetDetailsHB.Content.ReadAsStringAsync().Result != "[]" && vm.IsStarted == 1)
                                {
                                    var obj = JsonConvert.DeserializeObject<dynamic>(responseGetDetailsHB.Content.ReadAsStringAsync().Result);
                                    HeartBeatAWS info = ((JArray)obj)[0].ToObject<HeartBeatAWS>();

                                    var cs = _db.CloudLabsSchedule.Where(q => q.MachineLabsId == item.MachineLabsId).FirstOrDefault();

                                    var minRendered = info.minutes_rendered;

                                    var difference = (cs.LabHoursTotal * 3600) - (Convert.ToInt64(minRendered) *60);

                                    cs.TimeRemaining = difference;

                                    _db.SaveChanges();
                                    string dataParseStop = "";

                                    if (cs.TimeRemaining <= 0)
                                    {
                                        if (item.VEType == 9)
                                        {
                                            var dataJsonStop = new
                                            {
                                                ec2_id = vm.ResourceId,
                                                region = "ap-southeast-1"
                                            };
                                            dataParseStop = JsonConvert.SerializeObject(dataJsonStop);
                                        }
                                        else if (item.VEType == 8)
                                        {
                                            var dataJsonStop = new
                                            {
                                                // student_identifier = ms.StudentIdentifier,
                                                ec2_id = vm.ResourceId,
                                                region = "ap-southeast-1"
                                            };
                                            dataParseStop = JsonConvert.SerializeObject(dataJsonStop);
                                        }

                                        var responseStop = await clientMac.PostAsync("dev/mac_instance/stop_mac_instance", new StringContent(dataParseStop, Encoding.UTF8, "application/json"));
                                        //var getDetailsStop = JObject.Parse(responseStop.Content.ReadAsStringAsync().Result);
                                    }

                                }

                                //responseGetDetails = await clientAWS.PostAsync("dev/get_vm_details", new StringContent(jsonDataHB, Encoding.UTF8, "application/json"));

                                //getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

                                //var isRunning = getDetails.SelectToken("Reservations[0].Instances[0].State.Name").ToString();

                                //if (isRunning == "running" && vm.IsStarted != 1)
                                //{
                                //    var DNS = getDetails.SelectToken("Reservations[0].Instances[0].PublicDnsName").ToString();

                                //    if (item.VEType == 8)
                                //    {
                                //        var storagePayload = new
                                //        {
                                //            // student_identifier = ms.StudentIdentifier,
                                //            ec2_id = item.ResourceId,
                                //            region = "ap-southeast-1"
                                //        };
                                //        var jsonData = JsonConvert.SerializeObject(storagePayload);
                                //        responseGetDetailsHB = await clientMac.PostAsync("dev/mac_instance/mount_storage_account", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                                //    }

                                //    if (vm.GuacDNS == null)
                                //    {
                                //        //var guacUrl = AddMachineToDatabase(envi + '-' + tenant.ClientCode + '-' + vm.ResourceId, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VEType, tenant.EnvironmentCode, DNS);
                                //        var guacUrl = AddMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VEType, tenant.EnvironmentCode, DNS);
                                //        vm.GuacDNS = guacUrl;
                                //        _db.SaveChanges();
                                //    }

                                //    if (vm.FQDN != DNS)
                                //    {
                                //        var guacurl = EditMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, tenant.EnvironmentCode, DNS);

                                //        vm.IsStarted = 1;
                                //        vm.MachineStatus = "Running";
                                //        vm.FQDN = DNS;
                                //        if (guacurl != "")
                                //            vm.GuacDNS = guacurl;
                                //        vm.RunningBy = 1;

                                //        mls.Logs = "(Running)" + dateUtc + "---" + mls.Logs;
                                //        mls.LastStatus = "Running";
                                //        mls.ModifiedDate = dateUtc;
                                //        _db.Entry(mls).State = EntityState.Modified;

                                //        _db.Entry(vm).State = EntityState.Modified;

                                //        _db.SaveChanges();
                                //    }

                                //}

                                //if (isRunning == "stopped" && vm.IsStarted != 0)
                                //{

                                //    var v = _db.MachineLabs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();

                                //    HttpClient clientremove = new HttpClient();

                                //    clientremove.BaseAddress = new Uri(AWSVM);
                                //    clientremove.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                //    var data = new
                                //    {
                                //        instance_id = v.ResourceId,
                                //        ip_address = v.IpAddress,
                                //        region = "ap-southeast-1",
                                //        action_type = "REMOVE"
                                //    };

                                //    var dataRemove = JsonConvert.SerializeObject(data);
                                //    await clientremove.PostAsync("dev/update_security_group", new StringContent(dataRemove, Encoding.UTF8, "application/json"));

                                //    v.IsStarted = 0;
                                //    v.MachineStatus = "Deallocated";
                                //    //v.VMName = envi + '-' + tenant.ClientCode + '-' + v.ResourceId;
                                //    v.MachineName = "UI" + userId.ToString() + "VE" + v.VEProfileId;
                                //    //v.FQDN = "UI30VE89";
                                //    //v.GuacDNS = "UI30VE89";
                                //    //v.MachineName = "UI30VE89";
                                //    v.RunningBy = 0;
                                //    v.IpAddress = null;
                                //    _db.Entry(v).State = EntityState.Modified;
                                //    _db.SaveChanges();
                                //    mls.Logs = "(Deallocated)" + dateUtc + "---" + mls.Logs;
                                //    mls.LastStatus = "Deallocated";
                                //    mls.ModifiedDate = dateUtc;
                                //    //_db.Entry(mls).State = EntityState.Modified;

                                //    //_db.Entry(vm).State = EntityState.Modified;

                                //    _db.SaveChanges();
                                //}
                            }
                        }
                        catch (Exception e)
                        {
                            //vm.IsStarted = 6;
                            //vm.MachineStatus = "Deleted";
                            //_db.Entry(vm).State = EntityState.Modified;

                            //_db.SaveChanges();
                        }

                    } //for AWS Windows

                    if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId) && item.VEType == 8) // for MacOS
                    {
                        HttpClient clientMac = new HttpClient();
                        clientMac.BaseAddress = new Uri(AWSVM);
                        clientMac.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var jsonDetails = new
                        {
                            instance_id = vm.ResourceId,
                            region = "ap-southeast-1"
                        };

                        var jsonData = JsonConvert.SerializeObject(jsonDetails);

                        var getMacDetails = await clientMac.PostAsync("dev/minutes_rendered", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                        if (getMacDetails.Content.ReadAsStringAsync().Result != "[]" && vm.MachineStatus == "Virtual machine provisioning started")
                        {

                        }

                    } // for MacOS

                }

                return Request.CreateResponse(HttpStatusCode.OK, coursesList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            finally
            {
                // _dbCon.Close();
            }
        }

        //[HttpGet]
        //[Route("GetCourseNoStatus")]
        //public async Task<HttpResponseMessage> GetCourseNoStatus(int userId = 0, int tenantId = 0)
        //{
        //    var coursesList = new List<CourseDetails>();
        //    var GetStudentConsole = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/";
        //    var url = "dev/get_current_budget_of_student/";

        //    var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.TenantKey, q.SubscriptionKey, q.GuacamoleURL, q.GuacConnection, q.EnvironmentCode }).FirstOrDefault();

        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    HttpClient client = new HttpClient();
        //    client.BaseAddress = new Uri(AzureVM);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //    client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
        //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

        //    HttpResponseMessage response = null;

        //    GuacamoleInstancesController guac = new GuacamoleInstancesController();

        //    try
        //    {
        //        var veProfileIds = _db.VirtualEnvironments.Where(q => q.VETypeID == 6 || q.VETypeID == 7) //for consoles
        //            .Join(_db.VEProfiles,
        //            a => a.VirtualEnvironmentID,
        //            b => b.VirtualEnvironmentID,
        //            (a, b) => new { a, b })
        //            .Join(_db.VEProfileLabCreditMappings,
        //            c => c.b.VEProfileID,
        //            d => d.VEProfileID,
        //            (c, d) => new { c, d })
        //            .Where(q => q.d.GroupID == _db.CloudLabUsers.Where(x => x.UserId == userId).FirstOrDefault().UserGroup)
        //            .Select(s => new { s.c.b.VEProfileID, s.c.b.Name, s.c.a.VETypeID, s.c.b.ThumbnailURL }).ToList();

        //        var vmUsers = _db.CourseGrants
        //            .Join(_db.CloudLabUsers,
        //            a => a.UserID,
        //            b => b.UserId,
        //            (a, b) => new { a, b })
        //            .Join(_db.VEProfiles,
        //            c => c.a.VEProfileID,
        //            d => d.VEProfileID,
        //            (c, d) => new { c, d }).Where(x => x.c.a.UserID == userId)
        //            .Join(_db.VEProfileLabCreditMappings,
        //            e => new { ab = e.c.b.UserGroup, aa = e.d.VEProfileID },
        //            f => new { ab = f.GroupID, aa = f.VEProfileID },
        //            (e, f) => new { e, f })
        //            .Join(_db.VirtualEnvironments,
        //            g => g.e.d.VirtualEnvironmentID,
        //            h => h.VirtualEnvironmentID,
        //            (g, h) => new { g, h })
        //            .Where(w => w.g.e.c.a.IsCourseGranted)
        //            .Select(s => new { s.g.e.d.VEProfileID, s.g.e.d.Name, s.g.e.c.a.VEType, s.g.e.d.ThumbnailURL, s.g.e.c.a.IsCourseGranted, s.g.f.MachineSize, s.h.Title }).Where(q => q.VEType != 6 && q.VEType != 7).ToList();  // not for consoles

        //        foreach (var item in veProfileIds)
        //        {
        //            if (item.VETypeID == 6)
        //            {
        //                var consoleDGrants = _db.CourseGrants.Where(q => (q.UserID == userId) && (q.VEProfileID == item.VEProfileID)).FirstOrDefault();
        //                var consoleDSchedules = _db.ConsoleSchedules.Where(q => (q.UserId == userId) && (q.VEProfileId == item.VEProfileID)).FirstOrDefault();

        //                var BLimit = new BudgetLimit
        //                {
        //                    Amount = "0",
        //                    Unit = "",
        //                };

        //                var ASpend = new DataSpend
        //                {
        //                    ActualSpend = new ActualSpend { Amount = "0", Unit = "" }
        //                };

        //                if (consoleDSchedules != null)
        //                {
        //                    var result = ApiCall("GET", GetStudentConsole, url + consoleDSchedules.AccountId, null);
        //                    var dataJSON = JsonConvert.DeserializeObject<ConsoleDetail>(result.Result);

        //                    if (consoleDGrants.IsCourseGranted == true)
        //                    {

        //                        var result2 = new CourseDetails
        //                        {
        //                            UserId = consoleDGrants.UserID,
        //                            Name = item.Name,
        //                            veprofileid = consoleDGrants.VEProfileID,
        //                            VEType = consoleDGrants.VEType,
        //                            IsCourseGranted = consoleDGrants.IsCourseGranted,
        //                            IsProvisioned = consoleDSchedules.IsProvisioned,
        //                            ConsoleLink = consoleDSchedules.ConsoleLink,
        //                            Thumbnail = item.ThumbnailURL,
        //                            Data_transfer_budget_limit = dataJSON.Data_transfer_budget_limit ?? BLimit,
        //                            Cost_budget_limit = dataJSON.Cost_budget_limit ?? BLimit,
        //                            Actual_data_transfer_spend = dataJSON.Actual_data_transfer_spend ?? ASpend,
        //                            Actual_costs_spend = dataJSON.Actual_costs_spend ?? ASpend,
        //                            Is_suspended = dataJSON.Is_suspended
        //                        };

        //                        coursesList.Add(result2);
        //                    }
        //                }
        //                else
        //                {
        //                    if (consoleDGrants != null && consoleDGrants.IsCourseGranted == true)
        //                    {
        //                        var result1 = new CourseDetails
        //                        {
        //                            UserId = consoleDGrants.UserID,
        //                            Name = item.Name,
        //                            veprofileid = consoleDGrants.VEProfileID,
        //                            VEType = consoleDGrants.VEType,
        //                            IsCourseGranted = consoleDGrants.IsCourseGranted,
        //                            IsProvisioned = 0,
        //                            ConsoleLink = ""
        //                        };

        //                        coursesList.Add(result1);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var consoleCourse = new CourseDetails();

        //                if (_db.ConsoleSchedules.Any(q => q.UserId == userId && q.VEProfileId == item.VEProfileID))
        //                {
        //                    var course = _db.ConsoleSchedules.Where(q => q.UserId == userId && q.VEProfileId == item.VEProfileID).FirstOrDefault();
        //                    consoleCourse.IsProvisioned = course.IsProvisioned;
        //                    consoleCourse.veprofileid = item.VEProfileID;
        //                    consoleCourse.UserId = userId;
        //                    consoleCourse.Name = item.Name;
        //                    consoleCourse.VEType = item.VETypeID; //console
        //                    consoleCourse.IsStarted = null;
        //                    //consoleCourse.LabHoursRemaining = null;
        //                    consoleCourse.ConsoleLink = course.ConsoleLink;
        //                    consoleCourse.Thumbnail = item.ThumbnailURL;
        //                }
        //                else
        //                {
        //                    consoleCourse.Name = item.Name;
        //                    consoleCourse.veprofileid = item.VEProfileID;
        //                    consoleCourse.UserId = userId;
        //                    consoleCourse.Name = item.Name;
        //                    consoleCourse.VEType = item.VETypeID; //console
        //                    consoleCourse.IsStarted = null;
        //                    //consoleCourse.LabHoursRemaining = null;
        //                    consoleCourse.Thumbnail = item.ThumbnailURL;
        //                }

        //                coursesList.Add(consoleCourse);
        //            }
        //        } //for consoles

        //        foreach (var item in vmUsers)
        //        {
        //            var courseDetail = new CourseDetails();

        //            if (_db.MachineLabs.Any(q => q.UserId == userId && q.VEProfileId == item.VEProfileID))
        //            {
        //                var course = _db.MachineLabs.Where(q => q.UserId == userId && q.VEProfileId == item.VEProfileID).FirstOrDefault();

        //                var sched = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == course.MachineLabsId) ?
        //                    _db.CloudLabsSchedule.Where(q => q.MachineLabsId == course.MachineLabsId).FirstOrDefault() : null;

        //                var isUserHasExtension = _db.LabHourExtensions.Where(q => q.VEProfileId == item.VEProfileID && q.UserId == userId && q.IsDeleted == false && q.ExtensionTypeId == 1).ToList()
        //                    .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);

        //                courseDetail.veprofileid = item.VEProfileID;
        //                courseDetail.UserId = userId;
        //                courseDetail.Name = item.Name;
        //                courseDetail.VEType = item.VEType;

        //                courseDetail.TimeRemaining = sched != null ? sched.TimeRemaining : 0;

        //                courseDetail.RunningBy = course.RunningBy;
        //                courseDetail.MachineStatus = course.MachineStatus;
        //                courseDetail.IsStarted = course.IsStarted;
        //                courseDetail.LabHoursTotal = sched != null ? sched.LabHoursTotal : 0;
        //                courseDetail.InstructorLabHours = sched != null ? sched.InstructorLabHours : 0;
        //                courseDetail.GuacamoleUrl = course.GuacDNS;
        //                courseDetail.Thumbnail = item.ThumbnailURL;
        //                courseDetail.ResourceId = course.ResourceId;
        //                courseDetail.IsCourseGranted = item.IsCourseGranted;
        //                courseDetail.IsProvisioned = 1;
        //                courseDetail.Username = course.Username;
        //                courseDetail.Password = course.Password != null ? Decrypt(course.Password) : null;
        //                courseDetail.MachineStatus = course.MachineStatus;
        //                courseDetail.FQDN = course.FQDN;
        //                courseDetail.CourseCode = item.Title;
        //                courseDetail.IsExtend = isUserHasExtension;

        //            }
        //            else
        //            {
        //                courseDetail.Name = item.Name;
        //                courseDetail.veprofileid = item.VEProfileID;
        //                courseDetail.UserId = userId;
        //                courseDetail.Name = item.Name;
        //                courseDetail.VEType = item.VEType;
        //                courseDetail.IsStarted = null;
        //                //courseDetail.LabHoursRemaining = null;
        //                courseDetail.Thumbnail = item.ThumbnailURL;
        //                courseDetail.IsCourseGranted = item.IsCourseGranted;
        //                courseDetail.MachineSize = item.MachineSize;
        //                courseDetail.CourseCode = item.Title;
        //            }

        //            coursesList.Add(courseDetail);
        //        }
        //        return Request.CreateResponse(HttpStatusCode.OK, coursesList);

        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, e.Message);

        //    }
        //}

        [HttpGet]
        [HttpPost]
        [Route("VMOperation")]
        public async Task<IHttpActionResult> VMOperation(string resourceID, int tenantId, string operation, string role, bool isClick = false, bool isIdle = false)
        {
            try
            {
                var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).FirstOrDefault();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                DateTime dateUtc = DateTime.UtcNow;
                double timeRemainingStudent = 1;
                double timeRemainingInstructor = 1;

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(AzureVM);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
                //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

                //HttpResponseMessage response = null;

                var ml = _db.MachineLabs.Where(q => q.ResourceId == resourceID).FirstOrDefault();
                var logs = _db.MachineLogs.Where(q => q.ResourceId == resourceID).FirstOrDefault();

                //var ops = new { resourceId = resourceID, operation = operation };
                //var data = JsonConvert.SerializeObject(ops);


                //check if labhourextension exist to prevent from shuttingdown
                var isUserStudentHasExtension = _db.LabHourExtensions.Where(q => q.VEProfileId == ml.VEProfileId && q.UserId == ml.UserId && q.ExtensionTypeId == 1 && q.IsDeleted == false).ToList()
                        .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);
                var isUserInstructorHasExtension = _db.LabHourExtensions.Where(q => q.VEProfileId == ml.VEProfileId && q.UserId == ml.UserId && q.ExtensionTypeId == 2 && q.IsDeleted == false).ToList()
                        .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);

                if (isUserInstructorHasExtension)
                {
                    timeRemainingInstructor = _db.LabHourExtensions.Where(q => q.VEProfileId == ml.VEProfileId && q.UserId == ml.UserId && q.ExtensionTypeId == 2 && q.IsDeleted == false).ToList()
                        .Where(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now).FirstOrDefault().TimeRemaining;
                }
                if (isUserStudentHasExtension)
                {
                    timeRemainingStudent = _db.LabHourExtensions.Where(q => q.VEProfileId == ml.VEProfileId && q.UserId == ml.UserId && q.ExtensionTypeId == 1 && q.IsDeleted == false).ToList()
                        .Where(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now).FirstOrDefault().TimeRemaining;
                }
                var timeRemaining = _db.CloudLabsSchedule.Where(q => q.MachineLabsId == ml.MachineLabsId).FirstOrDefault().TimeRemaining;

                //if ((role == "Student") && (timeRemainingStudent <= 0) && !isClick)
                //    return Ok();
                //if ((role == "Instructor") && (timeRemainingInstructor <= 0) && !isClick)
                //    return Ok();

                if (operation.ToLower() == "stop" && ml.IsStarted != 0)
                {
                    ml.IsStarted = 2;
                    ml.MachineStatus = "Deallocating";
                    ml.RunningBy = 0;
                    _db.Entry(ml).State = EntityState.Modified;
                    _db.SaveChanges();

                    logs.Logs = "(Deallocating)" + dateUtc + "---" + logs.Logs;
                    logs.LastStatus = "Deallocating";
                    logs.ModifiedDate = dateUtc;
                    _db.Entry(logs).State = EntityState.Modified;
                    _db.SaveChanges();

                }
                else if (operation.ToLower() == "start" && ml.IsStarted != 1)
                {
                    ml.IsStarted = 5;
                    ml.MachineStatus = "Starting";
                    ml.RunningBy = 0;
                    _db.Entry(ml).State = EntityState.Modified;

                    _db.SaveChanges();

                    logs.Logs = "(Starting)" + dateUtc + "---" + logs.Logs;
                    logs.LastStatus = "Starting";
                    logs.ModifiedDate = dateUtc;
                    _db.Entry(logs).State = EntityState.Modified;
                    _db.SaveChanges();

                }

                if (isClick)
                {
                    await Operation(resourceID, tenant.TenantId, operation, role);                   
                }
                else if (!isClick)
                {
                    if (isIdle)
                        await Operation(resourceID, tenant.TenantId, operation, "");
                    else if (timeRemaining <= 0 && !isIdle)
                        await Operation(resourceID, tenant.TenantId, operation, "");

                }
                else
                    return Ok();
                ///
                return Ok();

            }
            catch (Exception e)
            {
                return Ok();
            }
        }

        //[HttpPost]
        //[Route("CheckVMUser")]
        //public HttpResponseMessage CheckVMUser(MachineLabsContent consoleUsers, int groupId)
        //{
        [HttpPost]
        [Route("CheckVMUser")]
        public HttpResponseMessage CheckVMUser(int VEProfileID, int groupId, int pageIndex, int pageSize, string VMStatus, string Search)
        {
            try
            {
                var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == VEProfileID && q.GroupID == groupId).FirstOrDefault();
                var vetypeID = _db.VEProfiles.Join(_db.VirtualEnvironments, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).Where(q => q.a.VEProfileID == VEProfileID).FirstOrDefault().b.VETypeID;

                MachineGrantsList machineLabUser = new MachineGrantsList();

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["VirtualEnvironmentDbContext"].ConnectionString))
                {
                    if (VMStatus == "undefined" && Search == "undefined" || (VMStatus == "null" && Search == null) || (VMStatus == null && Search == "null") || (VMStatus == null && Search == null))
                    {
                        if (vetypeID == 3 || vetypeID == 4)
                        {
                            try
                            {
                                SqlCommand cmd = new SqlCommand("GetAllUsersGrantLabAccessBaseline", connection);
                                cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                                cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                                cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                                cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                                cmd.CommandType = CommandType.StoredProcedure;

                                connection.Open();
                                SqlDataReader dr = cmd.ExecuteReader();
                                List<MachineGrants> listEmp = new List<MachineGrants>();

                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);

                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);

                                    listEmp.Add(mg);
                                }

                                dr.NextResult();

                                while (dr.Read())
                                {
                                    machineLabUser.totalCount = dr["totalCount"].ToString();
                                }

                                machineLabUser.MachineUsers = listEmp;

                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("GetAllUsersGrantLabAccess", connection);
                            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                            cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                            cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                            cmd.CommandType = CommandType.StoredProcedure;
                            try
                            {
                                connection.Open();
                                SqlDataReader dr = cmd.ExecuteReader();
                                List<MachineGrants> listEmp = new List<MachineGrants>();
                                List<MachineGrants> listEmp2 = new List<MachineGrants>();

                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);
                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId && q.VEProfileID == VEProfileID) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId && q.VEProfileID == VEProfileID).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    //mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.CourseHours = (Int32?)labCredit.CourseHours;

                                    listEmp.Add(mg);
                                }

                                machineLabUser.MachineUsers = listEmp;

                                dr.NextResult();

                                while (dr.Read())
                                {
                                    machineLabUser.totalCount = dr["totalCount"].ToString();
                                }

                                dr.NextResult();
                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);
                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId && q.VEProfileID == VEProfileID) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId && q.VEProfileID == VEProfileID).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    //mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.CourseHours = (Int32?)labCredit.CourseHours;

                                    listEmp2.Add(mg);
                                }

                                machineLabUser.AllUsers = listEmp2;

                            }
                            catch (Exception ex)
                            {
                                throw;
                            }

                        }

                    }
                    else if (VMStatus != "undefined" && (Search == "null" || Search == null))
                    {
                        if (vetypeID == 3 || vetypeID == 4)
                        {
                            SqlCommand cmd = new SqlCommand("GetAllUsersGrantLabAccessWithStatusBaseline", connection);
                            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                            cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                            cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                            cmd.Parameters.Add("@VMStatus", SqlDbType.NVarChar).Value = VMStatus;
                            cmd.CommandType = CommandType.StoredProcedure;

                            try
                            {
                                connection.Open();
                                SqlDataReader dr = cmd.ExecuteReader();
                                List<MachineGrants> listEmp = new List<MachineGrants>();

                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);

                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);

                                    listEmp.Add(mg);
                                }

                                dr.NextResult();

                                while (dr.Read())
                                {
                                    machineLabUser.totalCount = dr["totalCount"].ToString();
                                }

                                machineLabUser.MachineUsers = listEmp;


                            }
                            catch (Exception ex)
                            {
                                throw;
                            }

                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("GetAllUsersGrantLabAccessWithStatus", connection);
                            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                            cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                            cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                            cmd.Parameters.Add("@VMStatus", SqlDbType.NVarChar).Value = VMStatus;
                            cmd.CommandType = CommandType.StoredProcedure;
                            try
                            {
                                connection.Open();
                                SqlDataReader dr = cmd.ExecuteReader();
                                List<MachineGrants> listEmp = new List<MachineGrants>();

                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);

                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);

                                    listEmp.Add(mg);
                                }

                                dr.NextResult();

                                while (dr.Read())
                                {
                                    machineLabUser.totalCount = dr["totalCount"].ToString();
                                }

                                machineLabUser.MachineUsers = listEmp;


                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }

                    }
                    else if (Search != "undefined" && VMStatus == "null")
                    {
                        if (vetypeID == 3 || vetypeID == 4)
                        {
                            SqlCommand cmd = new SqlCommand("GetAllUsersGrantLabAccessWithSearchBaseline", connection);
                            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                            cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                            cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                            cmd.Parameters.Add("@Search", SqlDbType.NVarChar).Value = Search;
                            cmd.CommandType = CommandType.StoredProcedure;
                            try
                            {
                                connection.Open();
                                SqlDataReader dr = cmd.ExecuteReader();
                                List<MachineGrants> listEmp = new List<MachineGrants>();

                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);

                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);

                                    listEmp.Add(mg);
                                }

                                dr.NextResult();

                                while (dr.Read())
                                {
                                    machineLabUser.totalCount = dr["totalCount"].ToString();
                                }

                                machineLabUser.MachineUsers = listEmp;


                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                        else
                        {
                            SqlCommand cmd = new SqlCommand("GetAllUsersGrantLabAccessWithSearch", connection);
                            cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                            cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                            cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                            cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                            cmd.Parameters.Add("@Search", SqlDbType.NVarChar).Value = Search;
                            cmd.CommandType = CommandType.StoredProcedure;
                            try
                            {
                                connection.Open();
                                SqlDataReader dr = cmd.ExecuteReader();
                                List<MachineGrants> listEmp = new List<MachineGrants>();

                                while (dr.Read())
                                {
                                    int userId = Convert.ToInt32(dr["UserId"]);

                                    MachineGrants mg = new MachineGrants();
                                    mg.Email = dr["Email"].ToString();
                                    mg.FirstName = dr["FirstName"].ToString();
                                    mg.LastName = dr["LastName"].ToString();
                                    mg.FullNameEmail = dr["FullNameEmail"].ToString();
                                    mg.hasHours = dr["hasHours"].ToString();
                                    mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                                    mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                                    mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                                    //mg.LabHoursTotal = labCredit.TotalCourseHours;
                                    mg.LabHoursTotal = _db.CloudLabsSchedule.Any(q => q.UserId == userId) ? _db.CloudLabsSchedule.Where(q => q.UserId == userId).FirstOrDefault().LabHoursTotal : 0;
                                    mg.UserId = Convert.ToInt32(dr["UserId"]);
                                    mg.VEProfileId = VEProfileID;
                                    mg.CourseHours = dr["LabHoursTotal"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);

                                    listEmp.Add(mg);
                                }

                                dr.NextResult();

                                while (dr.Read())
                                {
                                    machineLabUser.totalCount = dr["totalCount"].ToString();
                                }

                                machineLabUser.MachineUsers = listEmp;


                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                           

                    }
                }
                //List<MachineGrants> machineLabUser = new List<MachineGrants>();

                //foreach (var userId in consoleUsers.UserId)
                //{
                //    var addUser = new MachineGrants();

                //    var user = _db.CloudLabUsers.Where(q => q.UserId == userId).FirstOrDefault();
                //    var userGrant = _db.CourseGrants.Where(q => q.VEProfileID == consoleUsers.VEProfileId && q.UserID == userId).FirstOrDefault();
                //    var userProv = _db.MachineLabs.Where(q => q.VEProfileId == consoleUsers.VEProfileId && q.UserId == userId).FirstOrDefault();
                //    var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == consoleUsers.VEProfileId && q.GroupID == groupId).FirstOrDefault();

                //    if (userGrant != null)
                //        addUser.IsCourseGranted = Convert.ToInt32(userGrant.IsCourseGranted);

                //    if (userProv != null)
                //        addUser.IsStarted = userProv.IsStarted;
                //    else
                //        addUser.IsStarted = null;

                //    addUser.UserId = userId;
                //    addUser.VEProfileId = consoleUsers.VEProfileId;
                //    addUser.FirstName = user.FirstName;
                //    addUser.LastName = user.LastName;
                //    addUser.Email = user.Email;
                //    addUser.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                //    addUser.LabHoursTotal = labCredit.TotalCourseHours;
                //    addUser.FullNameEmail = user.FirstName + " " + user.LastName + " " + user.Email;
                //    if (userProv == null && userGrant == null)
                //        addUser.hasHours = "Available";
                //    else
                //    {
                //        if (userProv != null)
                //        {
                //            if (userProv.IsStarted == 3)
                //                addUser.hasHours = "Failed";
                //            else if (userProv.IsStarted == 4)
                //                addUser.hasHours = "Provisioning";
                //            else if (userProv.IsStarted == 0 || userProv.IsStarted == 2)
                //                addUser.hasHours = "Provisioned";
                //        }
                //        else if (userGrant.IsCourseGranted && userGrant != null)
                //            addUser.hasHours = "Granted";
                //        else
                //            addUser.hasHours = "Available";
                //    }
                //    machineLabUser.Add(addUser);

                //}

                //    var userMachine = _db.CloudLabUsers
                //            .Where(clu => clu.EmailConfirmed && !clu.isDeleted && !clu.isDisabled && clu.UserGroup == groupId)
                //            .Join(_db.MachineLabs,
                //            a => a.UserId,
                //            b => b.UserId,
                //            (a, b) => new { a, b })
                //            .Select(w => new
                //            {
                //                IsStarted = w.b.IsStarted,
                //                UserId = w.b.UserId,
                //                VEProfileId = w.b.VEProfileId
                //            })
                //            .Where(s => s.VEProfileId == consoleUsers.VEProfileId).ToList();

                //    var courseGrant = _db.CourseGrants.ToList();

                //    var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == consoleUsers.VEProfileId && q.GroupID == groupId).FirstOrDefault();

                //    var users = _db.CloudLabUsers
                //             .Where(clu => clu.EmailConfirmed && !clu.isDeleted && !clu.isDisabled && clu.UserGroup == groupId)
                //             .GroupJoin(_db.CloudLabsSchedule,
                //              a => a.UserId,
                //              b => b.UserId,
                //              (a, b) => new { CloudLabUsers = a, CloudLabsSchedule = b })
                //              .Select(x => new
                //              {
                //                  CloudLabsSchedule = x.CloudLabsSchedule.Where(s => s.VEProfileID == consoleUsers.VEProfileId),
                //                  CloudLabUsers = x.CloudLabUsers
                //              }).ToList()
                //              .Join(_db.AspNetUserRoles,
                //             clu => clu.CloudLabUsers.Id,
                //             anur => anur.UserId,
                //             (clu, anur) => new { CloudLabUsers = clu, AspNetUserRoles = anur })
                //             .Select(q => new MachineGrants
                //             {
                //                 FirstName = q.CloudLabUsers.CloudLabUsers.FirstName,
                //                 LastName = q.CloudLabUsers.CloudLabUsers.LastName,
                //                 Email = q.CloudLabUsers.CloudLabUsers.Email,
                //                 UserId = q.CloudLabUsers.CloudLabUsers.UserId,
                //                 LabHoursTotal = labCredit.TotalCourseHours,
                //                 FullNameEmail = q.CloudLabUsers.CloudLabUsers.FirstName + " " + q.CloudLabUsers.CloudLabUsers.LastName + " " + q.CloudLabUsers.CloudLabUsers.Email,
                //                 hasHours = userMachine.Any(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == consoleUsers.VEProfileId) ?
                //                 (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId ==  consoleUsers.VEProfileId).FirstOrDefault().IsStarted >= 2) || (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == consoleUsers.VEProfileId).FirstOrDefault().IsStarted == 5) ? "Available" :
                //                 (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == consoleUsers.VEProfileId).FirstOrDefault().IsStarted == 3) ? "Failed" :
                //                 (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == consoleUsers.VEProfileId).FirstOrDefault().IsStarted == 4) ? "Provisioning" : "Granted" : "Available",
                //                 IsStarted = userMachine.Any(w => w.UserId == q.CloudLabUsers.CloudLabUsers.UserId && w.VEProfileId == consoleUsers.VEProfileId) ? userMachine.Where(w => w.UserId == q.CloudLabUsers.CloudLabUsers.UserId && w.VEProfileId == consoleUsers.VEProfileId).FirstOrDefault().IsStarted : (int?)null,
                //                 IsCourseGranted = courseGrant.Any(w=>w.UserID == q.CloudLabUsers.CloudLabUsers.UserId && w.VEProfileID == consoleUsers.VEProfileId) ? Convert.ToInt32(courseGrant.Where(w => w.UserID == q.CloudLabUsers.CloudLabUsers.UserId && w.VEProfileID == consoleUsers.VEProfileId).FirstOrDefault().IsCourseGranted) : (int?)null,
                //                 VEProfileId = consoleUsers.VEProfileId,
                //                 LabHoursRemaining = labCredit.TotalRemainingCourseHours
                //})
                //             .OrderBy(q => q.FirstName).Skip(12).Take(12).ToList();







                return Request.CreateResponse(HttpStatusCode.OK, machineLabUser);


            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);

            }
        }

        [HttpPost]
        [Route("CheckVMAdminStaff")]
        public HttpResponseMessage CheckVMAdminStaff(int VEProfileID, int groupId, int pageIndex, int pageSize)
        {
            try
            {
                var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == VEProfileID && q.GroupID == groupId).FirstOrDefault();
                MachineGrantsList machineLabUser = new MachineGrantsList();

                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["VirtualEnvironmentDbContext"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand("GetAllAdminStaffGrantLabAccess", connection);
                    cmd.Parameters.Add("@PageIndex", SqlDbType.Int).Value = pageIndex;
                    cmd.Parameters.Add("@pageSize", SqlDbType.Int).Value = pageSize;
                    cmd.Parameters.Add("@UserGroup", SqlDbType.Int).Value = groupId;
                    cmd.Parameters.Add("@VEProfileId", SqlDbType.Int).Value = VEProfileID;
                    cmd.CommandType = CommandType.StoredProcedure;
                    try
                    {
                        connection.Open();
                        SqlDataReader dr = cmd.ExecuteReader();
                        List<MachineGrants> listEmp = new List<MachineGrants>();


                        while (dr.Read())
                        {
                            MachineGrants mg = new MachineGrants();
                            mg.Email = dr["Email"].ToString();
                            mg.FirstName = dr["FirstName"].ToString();
                            mg.LastName = dr["LastName"].ToString();
                            mg.FullNameEmail = dr["FullNameEmail"].ToString();
                            mg.hasHours = dr["hasHours"].ToString();
                            mg.IsCourseGranted = dr["IsCourseGranted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsCourseGranted"]);
                            mg.IsStarted = dr["IsStarted"].ToString() == "" ? (Int32?)null : Convert.ToInt32(dr["IsStarted"]);
                            mg.LabHoursRemaining = labCredit.TotalRemainingCourseHours;
                            //mg.LabHoursTotal = labCredit.TotalCourseHours;
                            mg.LabHoursTotal = dr["LabHoursTotal"].ToString() == "" ? (Int64?)null : Convert.ToInt64(dr["LabHoursTotal"]); 
                            mg.UserId = Convert.ToInt32(dr["UserId"]);
                            mg.VEProfileId = VEProfileID;

                            listEmp.Add(mg);
                        }

                        dr.NextResult();

                        while (dr.Read())
                        {
                            machineLabUser.totalCount = dr["totalCount"].ToString();
                        }
                        machineLabUser.MachineUsers = listEmp;


                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
                return Request.CreateResponse(HttpStatusCode.OK, machineLabUser);


            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);

            }
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "abc123";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;


        }
        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "abc123";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        [HttpGet]
        [Route("Bong")]
        public async Task<HttpResponseMessage> Bong(string Password)
        {
            return Request.CreateResponse(HttpStatusCode.OK, Encrypt(Password)); ;
        }
        [HttpGet]
        [Route("Ken")]
        public async Task<HttpResponseMessage> Ken(string Password)
        {
            return Request.CreateResponse(HttpStatusCode.OK, Decrypt(Password)); ;
        }

        [HttpGet]
        [Route("GetVMSize")]
        public async Task<HttpResponseMessage> GetVMSize(int tenantId)
        {
            try
            {
                //var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.TenantKey, q.SubscriptionKey }).FirstOrDefault();
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //DateTime dateUtc = DateTime.UtcNow;

                //HttpClient client = new HttpClient();
                //client.BaseAddress = new Uri(AzureVM);
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
                //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

                //HttpResponseMessage response = null;

                //response = await client.GetAsync("labs/virtualmachine/size");
                //var result = JsonConvert.DeserializeObject<List<VMSize>>(response.Content.ReadAsStringAsync().Result);

                var result = _db.VMSize.ToList();
                //var result1 = response.Content.ReadAsStringAsync().Result;

                return Request.CreateResponse(HttpStatusCode.OK, result);
                //return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }


        }

        [HttpGet]
        [Route("GetAWSVMSize")]
        public HttpResponseMessage GetAWSVMSize()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _db.AWSSizes.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }


        }

        [HttpGet]
        [Route("UpdateConsumedHours")]
        public async Task<IHttpActionResult> UpdateConsumedHours(string info)
        {
            try
            {
                VMInfo vmInfo = JsonConvert.DeserializeObject<VMInfo>(info);

                MachineLabs vm = _db.MachineLabs.Where(q => q.ResourceId == vmInfo.ResourceId).FirstOrDefault();
                CloudLabsSchedule Schedule = _db.CloudLabsSchedule.Where(p => p.VEProfileID == vm.VEProfileId && p.UserId == vm.UserId).FirstOrDefault();

                var isExtensionStudentExist = _db.LabHourExtensions.Where(q => q.VEProfileId == vm.VEProfileId && q.UserId == vm.UserId && !q.IsDeleted)
                    .Join(_db.LabHourExtensionTypes,
                    a => a.ExtensionTypeId,
                    b => b.Id,
                    (a, b) => new { a, b }).Where(w => w.b.Label == "Student")
                    .ToList().Any(q => q.a.StartDate.ToLocalTime() <= DateTime.Now && q.a.EndDate.ToLocalTime() > DateTime.Now);

                var isExtensionInstuctorExist = _db.LabHourExtensions.Where(q => q.VEProfileId == vm.VEProfileId && q.UserId == vm.UserId && !q.IsDeleted)
                    .Join(_db.LabHourExtensionTypes,
                    a => a.ExtensionTypeId,
                    b => b.Id,
                    (a, b) => new { a, b }).Where(w => w.b.Label == "Instructor")
                    .ToList().Any(q => q.a.StartDate.ToLocalTime() <= DateTime.Now && q.a.EndDate.ToLocalTime() > DateTime.Now);

                if (isExtensionInstuctorExist)
                {
                    var timeRemainingInstructor = _db.LabHourExtensions.Where(q => q.VEProfileId == vm.VEProfileId && q.UserId == vm.UserId && !q.IsDeleted)
                        .Join(_db.LabHourExtensionTypes,
                        a => a.ExtensionTypeId,
                        b => b.Id,
                        (a, b) => new { a, b }).Where(w => w.b.Label == "Instructor")
                        .ToList().Where(q => q.a.StartDate.ToLocalTime() <= DateTime.Now && q.a.EndDate.ToLocalTime() > DateTime.Now).Select(q => q.a.TimeRemaining).First();
                }

                if (isExtensionStudentExist)
                {
                    var timeRemainingStudent = _db.LabHourExtensions.Where(q => q.VEProfileId == vm.VEProfileId && q.UserId == vm.UserId && !q.IsDeleted)
                         .Join(_db.LabHourExtensionTypes,
                         a => a.ExtensionTypeId,
                         b => b.Id,
                         (a, b) => new { a, b }).Where(w => w.b.Label == "Student")
                         .ToList().Where(q => q.a.StartDate.ToLocalTime() <= DateTime.Now && q.a.EndDate.ToLocalTime() > DateTime.Now).Select(q => q.a.TimeRemaining).First();
                }

                if (vmInfo.DeductTime <= 0)
                    vmInfo.DeductTime = 60;

                if (vm.RunningBy == 2)//if (vmInfo.RunningBy == 2)
                {
                    if (!isExtensionInstuctorExist)
                    {
                        if (Schedule.InstructorLabHours >= 0 && vm.IsStarted == 1)
                        {
                            Schedule.InstructorLabHours -= vmInfo.DeductTime;
                            //Schedule.InstructorLabHours -= 1;
                            Schedule.InstructorLastAccess = DateTime.UtcNow;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            await Operation(vmInfo.ResourceId, vmInfo.TenantId, "Stop", "Instructor");
                        }
                        //Update Machinelogs
                        var mLogs = _db.MachineLogs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();
                        mLogs.LastStatus = "Run HeartBeat";
                        mLogs.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                        return Ok(Schedule.InstructorLabHours);
                    }
                    else
                    {
                        var labHourInstructor = _db.LabHourExtensions.Where(q => q.VEProfileId == vm.VEProfileId && q.UserId == vm.UserId)
                            .Where(q => q.StartDate <= DateTime.Now && q.EndDate > DateTime.Now && !q.IsDeleted && q.ExtensionTypeId == 2).First();

                        labHourInstructor.TimeRemaining -= vmInfo.DeductTime;

                        await _db.SaveChangesAsync();
                        //Update Machinelogs
                        var mLogs = _db.MachineLogs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();
                        mLogs.LastStatus = "Run HeartBeat";
                        mLogs.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                    }
                }
                if (vm.RunningBy == 1)
                {
                    if (!isExtensionStudentExist)
                    {
                        if (Schedule.TimeRemaining >= 0 && vm.IsStarted == 1)
                        {
                            //Schedule.TimeRemaining -= 60;
                            Schedule.TimeRemaining -= vmInfo.DeductTime;
                            Schedule.RenderPageTriggerTime = DateTime.UtcNow;
                            Schedule.StartLabTriggerTime = DateTime.UtcNow;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            await Operation(vmInfo.ResourceId, vmInfo.TenantId, "Stop", "Student");
                        }

                        //Update Machinelogs
                        var mLogs = _db.MachineLogs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();
                        mLogs.LastStatus = "Run HeartBeat";
                        mLogs.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                        return Ok(Schedule.TimeRemaining);
                    }
                    else
                    {
                        var labHourStudent = _db.LabHourExtensions.Where(q => q.VEProfileId == vm.VEProfileId && q.UserId == vm.UserId && !q.IsDeleted)
                            .Where(q => q.StartDate <= DateTime.Now && q.EndDate > DateTime.Now && !q.IsDeleted && q.ExtensionTypeId == 1).First();

                        labHourStudent.TimeRemaining -= vmInfo.DeductTime;

                        await _db.SaveChangesAsync();
                        //Update Machinelogs
                        var mLogs = _db.MachineLogs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();
                        mLogs.LastStatus = "Run HeartBeat";
                        mLogs.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                    }
                }


                return Ok();
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
        [Route("GetLabHours")]
        public IHttpActionResult GetLabHours(string resourceId)
        {
            try
            {
                var ml = _db.MachineLabs.Where(q => q.ResourceId == resourceId).FirstOrDefault();
                var cls = _db.CloudLabsSchedule.Where(q => q.VEProfileID == ml.VEProfileId && q.UserId == ml.UserId).FirstOrDefault();

                return Ok(cls.TimeRemaining);
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }

        [HttpPost]
        [Route("SetRunningBy")]
        public IHttpActionResult SetRunningBy(string resourceId, int runningBy)
        {
            try
            {
                var ml = _db.MachineLabs.Where(q => q.ResourceId == resourceId).FirstOrDefault();

                ml.RunningBy = runningBy;
                _db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }

        public async Task Operation(string resourceId, int tenantId, string operation, string role = "")
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                DateTime dateUtc = DateTime.UtcNow;

                HttpClient clientGCP = new HttpClient();
                clientGCP.BaseAddress = new Uri(GCPServer);
                clientGCP.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var ml = _db.MachineLabs.Where(q => q.ResourceId == resourceId).FirstOrDefault();
                var logs = _db.MachineLogs.Where(q => q.ResourceId == resourceId).FirstOrDefault();
                var cs = _db.CloudLabsSchedule.Where(q => q.MachineLabsId == ml.MachineLabsId).FirstOrDefault();

                var veType = _db.MachineLabs.Join(_db.VEProfiles, a => a.VEProfileId, b => b.VEProfileID, (a, b) => new { a, b }).Join(_db.VirtualEnvironments, c => c.b.VirtualEnvironmentID, d => d.VirtualEnvironmentID, (c, d) => new { c, d }).Where(q => q.c.a.ResourceId == resourceId).FirstOrDefault().d.VETypeID;
                var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.GuacamoleURL, q.GuacConnection, q.EnvironmentCode, q.ClientCode }).FirstOrDefault();
                var envi = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "DEV" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "Q" ? "QA" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "U" ? "DMO" : "PRD";

                var ops = new { resourceId = resourceId, operation = operation };

                var data = JsonConvert.SerializeObject(ops);

                //    MacStorageAccounts ms = _db.MacStorageAccounts.Where(q => q.UserId == ml.UserId).FirstOrDefault();

                HttpResponseMessage response = null;

                if (veType == 8 || veType == 9) //aws windows
                {
                    if (operation.ToLower() == "stop")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        HttpClient clientStop = new HttpClient();
                        clientStop.BaseAddress = new Uri(MacOS);
                        clientStop.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        string dataParse = "";

                        if (veType == 9)
                        {
                            var dataJson = new
                            {
                                ec2_id = resourceId,
                                region = "ap-southeast-1"
                            };
                            dataParse = JsonConvert.SerializeObject(dataJson);
                        }
                        else if (veType == 8)
                        {
                            var dataJson = new
                            {
                                //       student_identifier = ms.StudentIdentifier,
                                ec2_id = resourceId,
                                region = "ap-southeast-1"
                            };
                            dataParse = JsonConvert.SerializeObject(dataJson);
                        }

                        var isTrue = true;

                        while (isTrue)
                        {
                            var responseStop = await clientStop.PostAsync("dev/mac_instance/stop_mac_instance", new StringContent(dataParse, Encoding.UTF8, "application/json"));
                            var getDetails = JObject.Parse(responseStop.Content.ReadAsStringAsync().Result);
                            var stop = getDetails.SelectToken("message").ToString();
                            if (stop == "Instance stopped")
                            {
                                isTrue = false;
                                ml.IsStarted = 0;
                                ml.MachineStatus = "Deallocated";
                                logs.Logs = "(Deallocated)" + dateUtc + "---" + logs.Logs;
                                ml.IsStarted = 2;
                                ml.MachineStatus = "Deallocating";
                                logs.Logs = "(Deallocating)" + dateUtc + "---" + logs.Logs;

                                logs.LastStatus = "Deallocating";
                                logs.ModifiedDate = dateUtc;
                                _db.Entry(logs).State = EntityState.Modified;

                                _db.Entry(ml).State = EntityState.Modified;

                                _db.SaveChanges();

                                if (ml.IpAddress != null)
                                {
                                    HttpClient clientremove = new HttpClient();

                                    clientremove.BaseAddress = new Uri(AWSVM);
                                    clientremove.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                    var dataRe = new
                                    {
                                        instance_id = ml.ResourceId,
                                        ip_address = ml.IpAddress,
                                        region = "ap-southeast-1",
                                        action_type = "REMOVE"
                                    };

                                    var dataRemove = JsonConvert.SerializeObject(dataRe);
                                    await clientremove.PostAsync("dev/update_security_group", new StringContent(dataRemove, Encoding.UTF8, "application/json"));

                                    ml.IpAddress = null;
                                    _db.SaveChanges();
                                }
                            }
                        }

                    }
                    if (operation.ToLower() == "start")
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        HttpClient clientStart = new HttpClient();
                        clientStart.BaseAddress = new Uri(MacOS);
                        clientStart.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        HttpClient client1 = new HttpClient();
                        client1.BaseAddress = new Uri(AWSVM);
                        client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var jsonDetails = new
                        {
                            instance_id = resourceId,
                            region = "ap-southeast-1"
                        };

                        string jsonD = "";

                        if (veType == 9)
                        {
                            var details = new
                            {
                                ec2_id = resourceId,
                                region = "ap-southeast-1"
                            };
                            jsonD = JsonConvert.SerializeObject(details);
                        }
                        else if (veType == 8)
                        {
                            var details = new
                            {
                                //       student_identifier = ms.StudentIdentifier,
                                ec2_id = resourceId,
                                region = "ap-southeast-1"
                            };
                            jsonD = JsonConvert.SerializeObject(details);
                        }

                        var jsonData = JsonConvert.SerializeObject(jsonDetails);


                        var isTrue = true;
                        JObject getDetails;

                        while (isTrue)
                        {
                            var responseStart = await clientStart.PostAsync("dev/mac_instance/start_mac_instance", new StringContent(jsonD, Encoding.UTF8, "application/json"));
                            getDetails = JObject.Parse(responseStart.Content.ReadAsStringAsync().Result);
                            var start = getDetails.SelectToken("message").ToString();

                            if (start == "Instance started")
                                isTrue = false;
                        }

                        //while (!isTrue)
                        //{
                        //    var responseGetDetails = await client1.PostAsync("dev/get_vm_details", new StringContent(jsonData, Encoding.UTF8, "application/json"));
                        //    getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

                        //    var isRunning = getDetails.SelectToken("Reservations[0].Instances[0].State.Name").ToString();

                        //    if (isRunning == "running")
                        //    {
                        //        var DNS = getDetails.SelectToken("Reservations[0].Instances[0].PublicDnsName").ToString();

                        //        if (ml.GuacDNS == null)
                        //        {
                        //            ml.GuacDNS = AddMachineToDatabase(envi + '-' + tenant.ClientCode + '-' + ml.ResourceId, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, veType, tenant.EnvironmentCode, DNS);
                        //            ml.FQDN = DNS;
                        //            _db.SaveChanges();
                        //        }

                        //        var guacURL = EditMachineToDatabase(ml.VMName, tenant.GuacamoleURL, tenant.GuacConnection, tenant.EnvironmentCode, DNS);

                        //        if (ml.GuacDNS == null || ml.GuacDNS == "")
                        //            ml.GuacDNS = guacURL;
                        //        ml.IsStarted = 5;
                        //        ml.MachineStatus = "Starting";
                        //        //ml.FQDN = DNS;
                        //        if (role == "Student")
                        //        {
                        //            ml.RunningBy = 1;
                        //            cs.StartLabTriggerTime = dateUtc;
                        //            cs.RenderPageTriggerTime = dateUtc;
                        //        }
                        //        if (role == "Instructor")
                        //        {
                        //            ml.RunningBy = 2;
                        //            cs.InstructorLastAccess = dateUtc;
                        //        }

                        //        logs.Logs = "(Starting)" + dateUtc + "---" + logs.Logs;
                        //        logs.LastStatus = "Starting";
                        //        logs.ModifiedDate = dateUtc;
                        //        _db.Entry(logs).State = EntityState.Modified;

                        //        _db.Entry(ml).State = EntityState.Modified;

                        //        _db.SaveChanges();

                        //        isTrue = true;
                        //    }
                        //}


                    }

                    //if (operation.ToLower() == "rdp")
                    //{
                    //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    //    HttpClient clientStart = new HttpClient();
                    //    clientStart.BaseAddress = new Uri(MacOS);
                    //    clientStart.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    //    HttpClient client1 = new HttpClient();
                    //    client1.BaseAddress = new Uri(AWSVM);
                    //    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    //    var jsonDetails = new
                    //    {
                    //        instance_id = resourceId,
                    //        region = "ap-southeast-1"
                    //    };

                    //    var details = new
                    //    {
                    //        ec2_id = resourceId,
                    //        region = "ap-southeast-1"
                    //    };

                    //    var jsonD = JsonConvert.SerializeObject(details);
                    //    var jsonData = JsonConvert.SerializeObject(jsonDetails);


                    //    //var isTrue = true;

                    //    logs.Logs = "(Starting)" + dateUtc + "---" + logs.Logs;
                    //    logs.LastStatus = "Starting";
                    //    _db.Entry(logs).State = EntityState.Modified;

                    //    _db.SaveChanges();

                    //    var responseStart = await clientStart.PostAsync("dev/mac_instance/start_mac_instance", new StringContent(jsonD, Encoding.UTF8, "application/json"));

                    //    ml.IsStarted = 5;
                    //    ml.MachineStatus = "Starting RDP";
                    //    if (role == "Student")
                    //        ml.RunningBy = 1;
                    //    if (role == "Instructor")
                    //        ml.RunningBy = 2;

                    //    logs.Logs = "(Running RDP)" + dateUtc + "---" + logs.Logs;
                    //    logs.LastStatus = "Running RDP";
                    //    _db.Entry(logs).State = EntityState.Modified;

                    //    _db.Entry(ml).State = EntityState.Modified;

                    //    _db.SaveChanges();
                    //}
                }
                else if (veType == 10)
                {
                    if (operation.ToLower() == "stop")
                    {
                        await Task.Run(() =>
                        {
                            clientGCP.GetAsync("api/gcp/virtual-machine/"+ ml.VMName.ToLower() + "/stop/");
                        });
                    }

                    if (operation.ToLower() == "start") 
                    {                       
                        if (role == "Student")           
                            ml.RunningBy = 1;

                        if (role == "Instructor")
                            ml.RunningBy = 2;

                        logs.Logs = "(Starting)" + dateUtc + "---" + logs.Logs;
                        logs.LastStatus = "Starting";
                        logs.ModifiedDate = dateUtc;


                        _db.Entry(logs).State = EntityState.Modified;

                        _db.Entry(ml).State = EntityState.Modified;
                        _db.SaveChanges();
                                              

                        await Task.Run(() =>
                        {
                            clientGCP.GetAsync("api/gcp/virtual-machine/" + ml.VMName.ToLower() + "/start/");
                        });

                    }
                }
                else
                {
                    if (operation.ToLower() == "stop")
                    {
                        var dataV = new
                        {
                            VMName = ml.VMName,
                            ResourceId = ml.ResourceId,
                            RunBy = 0
                        };

                        var dataMsg = JsonConvert.SerializeObject(dataV);

                        HttpClient clientStartFA = new HttpClient();
                        clientStartFA.BaseAddress = new Uri(FunctionAppUrl);
                        clientStartFA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        await Task.Run(() =>
                        {
                            clientStartFA.PostAsync(ShutdownMachine, new StringContent(dataMsg, Encoding.UTF8, "application/json"));
                        });

                    }

                    if (operation.ToLower() == "start")
                    {
                        ml.MachineStatus = "Starting";
                        ml.IsStarted = 5;
                        if (role == "Student")
                        {
                            ml.RunningBy = 1;
                            cs.StartLabTriggerTime = dateUtc;
                            cs.RenderPageTriggerTime = dateUtc;
                        }

                        if (role == "Instructor")
                        {
                            ml.RunningBy = 2;
                            cs.InstructorLastAccess = dateUtc;
                        }

                        logs.Logs = "(Starting)" + dateUtc + "---" + logs.Logs;
                        logs.LastStatus = "Starting";
                        logs.ModifiedDate = dateUtc;


                        _db.Entry(logs).State = EntityState.Modified;

                        _db.Entry(ml).State = EntityState.Modified;
                        _db.SaveChanges();

                        var dataV = new
                        {
                            VMName = ml.VMName,
                            ResourceId = ml.ResourceId,
                            RunBy = ml.RunningBy
                        };

                        var dataMsg = JsonConvert.SerializeObject(dataV);

                        HttpClient clientStopFA = new HttpClient();
                        clientStopFA.BaseAddress = new Uri(FunctionAppUrl);
                        clientStopFA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        await Task.Run(() =>
                        {
                            clientStopFA.PostAsync(StartMachine, new StringContent(dataMsg, Encoding.UTF8, "application/json"));
                        });

                    }

                    //response = await client.PutAsync("labs/virtualmachine/", new StringContent(data, Encoding.UTF8, "application/json"));
                    //var result = JsonConvert.DeserializeObject<VMStats>(response.Content.ReadAsStringAsync().Result);

                    //if (result.LastStatusDescription == "Virtual machine stopped")
                    //{
                    //    ml.MachineStatus = "Deallocating";
                    //    ml.IsStarted = 2;
                    //    ml.RunningBy = 0;
                    //    logs.Logs = "(Deallocating)" + dateUtc + "---" + logs.Logs;
                    //    logs.LastStatus = "Deallocating";
                    //    logs.ModifiedDate = dateUtc;

                    //    _db.Entry(logs).State = EntityState.Modified;

                    //    _db.Entry(ml).State = EntityState.Modified;
                    //    _db.SaveChanges();
                    //}
                    //if (result.LastStatusDescription == "Virtual machine started")
                    //{
                    //    ml.MachineStatus = "Starting";
                    //    ml.IsStarted = 5;
                    //    if (role == "Student")
                    //    {
                    //        ml.RunningBy = 1;
                    //        cs.StartLabTriggerTime = dateUtc;
                    //        cs.RenderPageTriggerTime = dateUtc;
                    //    }

                    //    if (role == "Instructor")
                    //    {
                    //        ml.RunningBy = 2;
                    //        cs.InstructorLastAccess = dateUtc;
                    //    }

                    //    logs.Logs = "(Starting)" + dateUtc + "---" + logs.Logs;
                    //    logs.LastStatus = "Starting";
                    //    logs.ModifiedDate = dateUtc;


                    //    _db.Entry(logs).State = EntityState.Modified;

                    //    _db.Entry(ml).State = EntityState.Modified;
                    //    _db.SaveChanges();
                    //}
                }
            }
            catch (Exception e)
            {
            }

        }


        [HttpGet]
        [Route("VMListWithCreds")]
        public IHttpActionResult VMListWithCreds()
        {
            try
            {
                var ml = _db.MachineLabs.Join(_db.CloudLabUsers, a=>a.UserId, b=>b.UserId, (a,b) => new { a,b})
                    .Join(_db.CloudLabsSchedule, f=>f.a.MachineLabsId, g=>g.MachineLabsId, (f,g) => new { f,g})
                    .Join(_db.VEProfiles, c=> c.f.a.VEProfileId, d=> d.VEProfileID, (c,d) => new { c,d})
                    .Select(q=> new { 
                    vename = q.d.Name,
                    ml = q.c.f.a,
                    email = q.c.f.b.Email
                    }).ToList();
                var vm = new List<vmlist>();
                //var vmdata = new vmlist();

                foreach (var item in ml)
                {
                    var vmdata = new vmlist();

                    vmdata.email = item.email;
                    vmdata.course = item.vename; 
                        vmdata.vmname = item.ml.VMName;
                        vmdata.fqdn = item.ml.FQDN;
                        vmdata.username = item.ml.Username;
                        vmdata.password = Decrypt(item.ml.Password);

                        vm.Add(vmdata);

                }
                return Ok(vm);
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }

        public class vmlist
        {
            public string vmname { get; set; }
            public string email { get; set; }
            public string course { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string fqdn { get; set; }
        }

        private async Task<string> ProvisionAWSWindows(int veprofileId, int userId, string machineSize, string schedBy, int tenantId, int veTypeId)
        {
            try
            {
                HttpResponseMessage responseAWS = null;
                HttpResponseMessage responseGetDetails = null;
                JObject getDetails = null;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpClient clientAWS = new HttpClient();
                clientAWS.BaseAddress = new Uri(AWSVM);
                clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var imageId = _db.VEProfiles.Join(_db.VirtualEnvironmentImages, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).Where(q => q.a.VEProfileID == veprofileId).FirstOrDefault().b.Name;
                var groupId = _db.CloudLabUsers.Where(q => q.UserId == userId).FirstOrDefault().UserGroup;
                var tenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).Select(w => new { w.GuacConnection, w.GuacamoleURL, w.EnvironmentCode, w.ClientCode }).FirstOrDefault();
                var environment = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "Staging" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "Q" ? "QA" : tenant.EnvironmentCode == "U" ? "Demo" : "Prod";
                var envi = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "DEV" : tenant.EnvironmentCode == "Q" ? "QA" : tenant.EnvironmentCode == "U" ? "DMO" : "PRD";

                var hours = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == veprofileId && q.GroupID == groupId).Select(w => new { w.CourseHours, w.TotalCourseHours }).FirstOrDefault();

                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();

                var instanceName = new string(
                       Enumerable.Repeat(chars, 6)
                           .Select(s => s[random.Next(s.Length)])
                           .ToArray());

                string[] sg = { "sg-0663e3fcbc92db0af" };

                var TagSpec = new List<TagSpecifications>();
                var tagSpec = new TagSpecifications();

                tagSpec.ResourceType = "instance";

                tagSpec.Tags = new List<Tags>
            {
                new Tags{Key = "Name", Value = envi + '-' + tenant.ClientCode +'-' + instanceName},
                new Tags{Key = "RESOURCE_GROUP", Value = envi + '-' + tenant.ClientCode + '-'},
                new Tags{Key = "STUDENT_ID", Value = "UI" + userId.ToString() + "VE" + veprofileId},
            };

                TagSpec.Add(tagSpec);

                var ec2 = new EC2
                {
                    InstanceType = machineSize,
                    MaxCount = 1,
                    MinCount = 1,
                    ImageId = imageId,
                    KeyName = "cloudswyft-windows-instances",
                    SecurityGroupIds = sg,
                    TagSpecifications = TagSpec
                };

                var message = new AWSJson
                {
                    account_id = "827347782581",
                    ec2_details = ec2,
                    region = "ap-southeast-1",
                    root = "true"
                };


                var data = JsonConvert.SerializeObject(message);

                responseAWS = await clientAWS.PostAsync("dev/provision_vm", new StringContent(data, Encoding.UTF8, "application/json"));

                var details = JObject.Parse(responseAWS.Content.ReadAsStringAsync().Result);

                var InstanceId = details.SelectToken("Instances[0].InstanceId").ToString();

                CloudLabsSchedule cls = new CloudLabsSchedule();
                MachineLabs ml = _db.MachineLabs.Where(q => q.UserId == userId && q.VEProfileId == veprofileId).FirstOrDefault();
                CourseGrants cg = new CourseGrants();
                MachineLogs mlog = new MachineLogs();

                ml.ResourceId = InstanceId;
                ml.VMName = envi + '-' + tenant.ClientCode + '-' + instanceName;


                mlog.ResourceId = InstanceId;
                //mlog.LastStatus = "Provisioned";
                mlog.LastStatus = "Provisioning";
                mlog.Logs = '(' + mlog.LastStatus + ')' + DateTime.UtcNow;
                mlog.ModifiedDate = DateTime.UtcNow;

                //_db.MachineLabs.Add(ml);
                _db.MachineLogs.Add(mlog);
                _db.SaveChanges();

                //mlog.LastStatus = "Deallocated";
                //mlog.LastStatus = "Deallocated";
                //mlog.Logs = '(' + mlog.LastStatus + ')' + DateTime.UtcNow;
                //_db.SaveChanges();

                cls.VEProfileID = veprofileId;
                cls.UserId = userId;
                cls.TimeRemaining = TimeSpan.FromHours(hours.CourseHours).TotalSeconds;
                cls.LabHoursTotal = hours.CourseHours;
                cls.MachineLabsId = ml.MachineLabsId;
                cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;


                _db.CloudLabsSchedule.Add(cls);
                _db.SaveChanges();

                var jsonDetails = new
                {
                    instance_id = InstanceId,
                    region = "ap-southeast-1"
                };
                var jsonData = JsonConvert.SerializeObject(jsonDetails);

                //var isTrue = true;

                // while (isTrue)
                // {
                responseGetDetails = await clientAWS.PostAsync("dev/get_vm_details", new StringContent(jsonData, Encoding.UTF8, "application/json"));
                getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

                var isRunning = getDetails.SelectToken("Reservations[0].Instances[0].State.Name").ToString();

                // if (isRunning == "stopped")
                //     isTrue = false;
                // }
                // var DNS = getDetails.SelectToken("Reservations[0].Instances[0].PublicDnsName").ToString();


                // var guacUrl = AddMachineToDatabase(envi + '-' + tenant.ClientCode + '-' + instanceName, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", password, veTypeId, environment, "southeastasia", DNS);

                return "";
            }
            catch (Exception e)
            {
                return e.Message.ToString();
            }

        }

        private async Task<string> ProvisionMacOS(int veprofileId, int userId, string machineSize, string schedBy, int tenantId, int veTypeId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MacOS);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var imageId = _db.VEProfiles.Join(_db.VirtualEnvironmentImages, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).Where(q => q.a.VEProfileID == veprofileId).FirstOrDefault().b.Name;
                var groupId = _db.CloudLabUsers.Where(q => q.UserId == userId).FirstOrDefault().UserGroup;
                var tenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).Select(w => new { w.GuacConnection, w.GuacamoleURL, w.EnvironmentCode, w.ClientCode }).FirstOrDefault();
                var environment = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "Staging" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "Q" ? "QA" : tenant.EnvironmentCode == "U" ? "Demo" : "Prod";
                var envi = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "DEV" : tenant.EnvironmentCode == "Q" ? "QA" : tenant.EnvironmentCode == "U" ? "DMO" : "PRD";
                var email = _db.CloudLabUsers.Where(q => q.UserId == userId).FirstOrDefault().Email;
                var hours = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == veprofileId && q.GroupID == groupId).Select(w => new { w.CourseHours, w.TotalCourseHours }).FirstOrDefault();
                var identifier = email.Substring(0, email.LastIndexOf('@')).Replace(".", "").Replace("-", "").Replace("_", "") + userId;
                string fileshare_smb_url = "";
                MachineLabs ml = new MachineLabs();
                MachineLogs mls = new MachineLogs();
                CloudLabsSchedule cls = new CloudLabsSchedule();

                string HostIds = "";

                var image = _db.VEProfiles.Join(_db.VirtualEnvironmentImages,
                    a => a.VirtualEnvironmentID,
                    b => b.VirtualEnvironmentID,
                    (a, b) => new { a, b }).Where(q => q.a.VEProfileID == veprofileId).FirstOrDefault().b.Name;
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();

                var instanceName = new string(
                       Enumerable.Repeat(chars, 6)
                           .Select(s => s[random.Next(s.Length)])
                           .ToArray());

                //if (!_db.MacStorageAccounts.Any(q => q.UserId == userId && q.StudentIdentifier == identifier)) //if mac storage is not exist create -- 1 user 1 storage
                //{
                //   // MacStorageAccounts mc = new MacStorageAccounts();

                //    var storagePayload = new
                //    {
                //        student_identifier = identifier,
                //        fileshare_name = identifier
                //    };

                //    var storageData = JsonConvert.SerializeObject(storagePayload);

                //    var responseStorage = await client.PostAsync("dev/mac_instance/create_storage_account", new StringContent(storageData, Encoding.UTF8, "application/json"));

                //    var obj = JsonConvert.DeserializeObject<dynamic>(responseStorage.Content.ReadAsStringAsync().Result);
                //    fileshare_smb_url = ((Newtonsoft.Json.Linq.JContainer)(obj)).First.Parent.SelectToken("fileshare_smb_url").ToString();
                //    //obj.SelectToken("fileshare_smb_url").ToString();

                //    //what if failed, pwede taasan ung limit (250), 1 user 1 storage account

                //    mc.ModifiedDate = DateTime.UtcNow;
                //    mc.StorageURL = fileshare_smb_url;
                //    mc.StudentIdentifier = email.Substring(0, email.LastIndexOf('@')).Replace(".", "").Replace("-", "").Replace("_", "") + userId;
                //    mc.UserId = userId;

                //    _db.MacStorageAccounts.Add(mc);
                //    _db.SaveChanges();
                //}
                //else
                //{
                //    fileshare_smb_url = _db.MacStorageAccounts.Where(q => q.UserId == userId && q.StudentIdentifier == identifier).FirstOrDefault().StorageURL;
                //}

                var responseGetHost = await client.GetAsync("dev/mac_instance/check_available_mac_hosts"); // check if there's available Mac Host

                if (responseGetHost.Content.ReadAsStringAsync().Result.ToString() == "[]") //no available Mac Host, need to create
                {
                    var macPayload = new
                    {
                        availability_zone = "ap-southeast-1a",
                        region = "ap-southeast-1"
                    };
                    var macString = JsonConvert.SerializeObject(macPayload);

                    var responseStorage = await client.PostAsync("dev/mac_instance/allocate_mac_dedicated_host", new StringContent(macString, Encoding.UTF8, "application/json"));

                    var dataParse = JsonConvert.DeserializeObject<CreateHost>(responseStorage.Content.ReadAsStringAsync().Result);

                    HostIds = dataParse.HostIds[0];
                }

                else //get the available host
                {
                    var dataParse = JsonConvert.DeserializeObject<List<MacHostSuccess>>(responseGetHost.Content.ReadAsStringAsync().Result);
                    //var ddd = (((JObject)(JsonConvert.DeserializeObject<dynamic>(responseGetHost.Content.ReadAsStringAsync().Result)[0])).SelectToken("Hostid"));

                    //var data = JObject.Parse(responseGetHost.Content.ReadAsStringAsync().ToString());
                    //HostIds = data.SelectToken("HostIds[0]").ToString();

                    HostIds = dataParse[0].HostId;
                }

                var createMacPayload = new
                {
                    availability_zone = "ap-southeast-1a",
                    region = "ap-southeast-1",
                    student_identifier = identifier,
                    smb_url = fileshare_smb_url,
                    host_id = HostIds,
                    dry_run = "false",
                    image_id = image,
                    instance_name = envi + '-' + tenant.ClientCode + '-' + instanceName,
                    resource_group = envi + '-' + tenant.ClientCode + '-' + instanceName
                };

                var createMacString = JsonConvert.SerializeObject(createMacPayload);

                var responseCreate = await client.PostAsync("dev/mac_instance/create_mac_instance", new StringContent(createMacString, Encoding.UTF8, "application/json"));

                var objCreate = JsonConvert.DeserializeObject<CreateMacInstance>(responseCreate.Content.ReadAsStringAsync().Result);

                if (objCreate.Message.ToLower() == "instance created") //successfully create MacHost
                {
                    ml.ResourceId = objCreate.Instance_Id;
                    ml.VEProfileId = veprofileId;
                    ml.UserId = userId;
                    ml.MachineStatus = "Virtual machine provisioning started";
                    ml.VMName = envi + '-' + tenant.ClientCode + '-' + instanceName;
                    ml.MachineName = "UI" + userId + "VE" + veprofileId;
                    ml.IsStarted = 4;
                    ml.DateProvision = DateTime.UtcNow.ToString();
                    ml.RunningBy = 0;
                    ml.ScheduledBy = schedBy;
                    ml.GuacDNS = null;
                    ml.Username = "ec2-user";
                    ml.Password = Encrypt("P@ssw0rd1!");
                    ml.FQDN = null;

                    mls.ResourceId = objCreate.Instance_Id;
                    mls.LastStatus = "Provisioning";
                    mls.Logs = '(' + mls.LastStatus + ')' + DateTime.UtcNow;
                    mls.ModifiedDate = DateTime.UtcNow;

                    _db.MachineLabs.Add(ml);
                    _db.MachineLogs.Add(mls);
                    _db.SaveChanges();

                    cls.VEProfileID = veprofileId;
                    cls.UserId = userId;
                    cls.TimeRemaining = hours.CourseHours * 60;
                    cls.LabHoursTotal = hours.CourseHours;
                    cls.StartLabTriggerTime = null;
                    cls.RenderPageTriggerTime = null;
                    cls.InstructorLabHours = 120 * 60;
                    cls.MachineLabsId = ml.MachineLabsId;

                }

            }
            catch (Exception e)
            {

            }









            //////////////
            //const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            //var random = new Random();

            //var instanceName = new string(
            //       Enumerable.Repeat(chars, 6)
            //           .Select(s => s[random.Next(s.Length)])
            //           .ToArray());

            //CloudLabsSchedule cls = new CloudLabsSchedule();
            //MachineLabs ml = _db.MachineLabs.Where(q => q.UserId == userId && q.VEProfileId == veprofileId).FirstOrDefault();
            //CourseGrants cg = new CourseGrants();
            //MachineLogs mlog = new MachineLogs();

            //ml.ResourceId = InstanceId;
            //ml.VMName = envi + '-' + tenant.ClientCode + '-' + instanceName;


            //mlog.ResourceId = InstanceId;
            //mlog.LastStatus = "Provisioning";
            //mlog.Logs = '(' + mlog.LastStatus + ')' + DateTime.UtcNow;
            //mlog.ModifiedDate = DateTime.UtcNow;

            ////_db.MachineLabs.Add(ml);
            //_db.MachineLogs.Add(mlog);
            //_db.SaveChanges();

            //cls.VEProfileID = veprofileId;
            //cls.UserId = userId;
            //cls.TimeRemaining = TimeSpan.FromHours(hours.CourseHours / 60).TotalSeconds;
            //cls.LabHoursTotal = hours.CourseHours;
            //cls.MachineLabsId = ml.MachineLabsId;
            //cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;


            //_db.CloudLabsSchedule.Add(cls);
            //_db.SaveChanges();

            return "";
        }

        public string AddMachineToDatabase(string machineName, string GuacamoleUrl, string guacCon, string vmusername, string vmpassword, int VETypeId, string environment, string fqdn)
        {
            try
            {
                var guacDatabase = new MySqlConnection(guacCon);
                var Environment = environment.Trim() == "D" ? "Staging" : environment.Trim() == "Q" ? "QA" : environment.Trim() == "U" ? "Demo" : "Prod";

                guacDatabase.Open();
                string selectQuery = "";
                string protocol = "rdp";

                selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '%{machineName}%'";
                var MySqlCommandConn = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader = MySqlCommandConn.ExecuteReader();

                dataReader.Read();
                //var guacamoleInstance = new List<GuacamoleInstance>();
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i));

                    var hostName = machineName;

                    var insertQuery = "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                        $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'4\')";

                    var insertCommand = new MySqlCommand(insertQuery, guacDatabase);


                    insertCommand.ExecuteNonQuery();

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var insertParamsQuery = string.Empty;


                    insertParamsQuery =
                        "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn.ToLower()}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', 'nla'), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    //windows
                    if (VETypeId == 1 || VETypeId == 3 || VETypeId == 9)
                    {
                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }
                    //linux
                    else if (VETypeId == 2 || VETypeId == 4)
                    {
                        insertParamsQuery = "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn.ToLower()}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', ''), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }
                    else if (VETypeId == 8)
                    {
                        insertParamsQuery =
                      "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                      $"VALUES ({connectionId}, 'hostname', '{fqdn.ToLower()}'), " +
                      $"({connectionId}, 'ignore-cert', 'true'), " +
                      $"({connectionId}, 'password', 'P@ssw0rd1!'), " +
                      $"({connectionId}, 'security', 'vnc'), " +
                      $"({connectionId}, 'port', '5900'), " +
                      $"({connectionId}, 'enable-wallpaper', 'true'), " +
                      $"({connectionId}, 'username', 'ec2-user')";

                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }


                    insertParamsCommand.ExecuteNonQuery();
                    selectQuery = $"SELECT entity_id FROM guacamole_entity WHERE name = '{Environment}'";
                    MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                    var dataReader2 = MySqlCommand.ExecuteReader();
                    dataReader2.Read();
                    var userId = Convert.ToInt32(dataReader2["entity_id"]);
                    dataReader2.Close();

                    var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(entity_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                    var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                    insertPermissionCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;
                }
                else
                {
                    dataReader.Close();
                    return "";

                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
                return "";
            }

        }
        public string EditMachineToDatabase(string vmName, string GuacamoleUrl, string guacCon, string environment, string fqdn)
        {
            try
            {
                var guacDatabase = new MySqlConnection(guacCon);
                var Environment = environment.Trim() == "D" ? "Dev" : environment.Trim() == "Q" ? "QA" : environment.Trim() == "U" ? "Demo" : "Prod";

                guacDatabase.Open();
                string selectQuery = "";
                string protocol = "rdp";

                selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '%{vmName}%'";
                var MySqlCommandConn = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader = MySqlCommandConn.ExecuteReader();

                dataReader.Read();
                //var guacamoleInstance = new List<GuacamoleInstance>();
                if (dataReader.HasRows)
                {
                    dataReader.Close();
                    //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i));

                    var hostName = vmName;

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var updateParamsQuery = string.Empty;

                    updateParamsQuery = "UPDATE guacamole_connection_parameter SET parameter_value = '" + fqdn + "' WHERE connection_Id = " + connectionId + " and parameter_name = 'hostname'";

                    MySqlCommand updateParamsCommand = new MySqlCommand();

                    updateParamsCommand = new MySqlCommand(updateParamsQuery, guacDatabase);

                    updateParamsCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;
                }
                else
                {
                    dataReader.Close();

                    var hostName = vmName;

                    var insertQuery = "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                        $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'4\')";

                    var insertCommand = new MySqlCommand(insertQuery, guacDatabase);


                    insertCommand.ExecuteNonQuery();

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var insertParamsQuery = string.Empty;


                    insertParamsQuery =
                        "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn.ToLower()}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{Password}'), " +
                        $"({connectionId}, 'security', 'nla'), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', 'cloudswyft')";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);

                    insertParamsCommand.ExecuteNonQuery();
                    selectQuery = $"SELECT entity_id FROM guacamole_entity WHERE name = '{Environment}'";
                    MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                    var dataReader2 = MySqlCommand.ExecuteReader();
                    dataReader2.Read();
                    var userId = Convert.ToInt32(dataReader2["entity_id"]);
                    dataReader2.Close();

                    var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(entity_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                    var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                    insertPermissionCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;

                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
                return "";
            }

        }
        public static string EditMachineToDatabase(string machineName, string GuacamoleUrl, string guacCon, string environment, string fqdn, string vmpassword, string vmusername)
        {
            try
            {
                var guacDatabase = new MySqlConnection(guacCon);
                var Environment = environment.Trim() == "D" ? "Staging" : environment.Trim() == "Q" ? "QA" : environment.Trim() == "U" ? "Demo" : "Prod";

                guacDatabase.Open();
                string selectQuery = "";
                string protocol = "rdp";

                selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '%{machineName}%'";
                var MySqlCommandConn = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader = MySqlCommandConn.ExecuteReader();

                dataReader.Read();
                //var guacamoleInstance = new List<GuacamoleInstance>();
                if (dataReader.HasRows)
                {
                    dataReader.Close();
                    //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i));

                    var hostName = machineName;

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var updateParamsQuery = string.Empty;

                    updateParamsQuery = "UPDATE guacamole_connection_parameter SET parameter_value = '" + fqdn + "' WHERE connection_Id = " + connectionId + " and parameter_name = 'hostname'";

                    MySqlCommand updateParamsCommand = new MySqlCommand();

                    updateParamsCommand = new MySqlCommand(updateParamsQuery, guacDatabase);

                    updateParamsCommand.ExecuteNonQuery();
                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;
                }
                else
                {
                    dataReader.Close();

                    var hostName = machineName;

                    var insertQuery = "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                        $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'4\')";

                    var insertCommand = new MySqlCommand(insertQuery, guacDatabase);


                    insertCommand.ExecuteNonQuery();

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var insertParamsQuery = string.Empty;


                    insertParamsQuery =
                        "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn.ToLower()}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', 'nla'), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);

                    insertParamsCommand.ExecuteNonQuery();
                    selectQuery = $"SELECT entity_id FROM guacamole_entity WHERE name = '{Environment}'";
                    MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                    var dataReader2 = MySqlCommand.ExecuteReader();
                    dataReader2.Read();
                    var userId = Convert.ToInt32(dataReader2["entity_id"]);
                    dataReader2.Close();

                    var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(entity_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                    var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                    insertPermissionCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;

                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
                return "";
            }

        }

        [Route("CourseGrantCSV")]
        [HttpGet]
        public async Task<IHttpActionResult> CourseGrantCSV()
        {
            try
            {
                List<CourseGrants> grant = new List<CourseGrants>();

                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/CourseGrant.csv"))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');

                        var name = rows[1];
                        var email = rows[0];

                        var ve = _db.VEProfiles.Where(q => q.Name == name).Select(w => new { w.VEProfileID, w.VirtualEnvironmentID }).FirstOrDefault();
                        var type = _db.VirtualEnvironments.Where(q => q.VirtualEnvironmentID == ve.VirtualEnvironmentID).FirstOrDefault().VETypeID;
                        var userId = _db.CloudLabUsers.Where(q => q.Email == email).FirstOrDefault().UserId;

                        var student = new CourseGrants
                        {
                            IsCourseGranted = true,
                            UserID = userId,
                            VEProfileID = ve.VEProfileID,
                            VEType = type
                        };

                        if (!_db.CourseGrants.Any(q => q.VEProfileID == student.VEProfileID && q.UserID == student.UserID))
                        {
                            _db.CourseGrants.Add(student);
                            _db.SaveChanges();
                        }
                    }
                }


                return Ok(grant);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }


        [Route("AWSWhiteList")]
        [HttpPost]
        public async Task<IHttpActionResult> AWSWhiteList(string IP, string operation)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpClient clientAWS = new HttpClient();
                clientAWS.BaseAddress = new Uri(AWSVM);
                clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = null;

                var dataJson = new
                {
                    instance_id = "i-04e6d634d426bc792",
                    ip_address = IP,
                    region = "ap-southeast-1",
                    action_type = operation
                };

                var data = JsonConvert.SerializeObject(dataJson);

                response = await clientAWS.PostAsync("dev/provision_vm", new StringContent(data, Encoding.UTF8, "application/json"));

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }


        [HttpGet]
        [Route("GetGuacDNS")]
        public string GetGuacDNS(string ResourceId)
        {
            return _db.MachineLabs.Where(q => q.ResourceId == ResourceId).FirstOrDefault().GuacDNS;
        }

        [HttpPost]
        [Route("SetIpAddress")]
        public async Task<string> SetIpAddress(string ResourceId, string IPAddress)
        {
            HttpClient clientAWS = new HttpClient();
            HttpResponseMessage responseGetDetails = null;
            // JObject getDetails = null;

            clientAWS.BaseAddress = new Uri(AWSVM);
            clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var dataJson = new
            {
                instance_id = ResourceId,
                ip_address = IPAddress,
                region = "ap-southeast-1",
                action_type = "ADD"
            };

            var data = JsonConvert.SerializeObject(dataJson);
            responseGetDetails = await clientAWS.PostAsync("dev/update_security_group", new StringContent(data, Encoding.UTF8, "application/json"));
            // getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

            var ml = _db.MachineLabs.Where(q => q.ResourceId == ResourceId).FirstOrDefault();

            ml.IpAddress = IPAddress;

            _db.SaveChanges();


            return "";
        }

        [HttpGet]
        [Route("IsIpExist")]
        public bool IsIpExist(string ResourceId)
        {
            var ml = _db.MachineLabs.Where(q => q.ResourceId == ResourceId).FirstOrDefault();
            return (ml.IpAddress == null || ml.IpAddress == "") ? false : true;
        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IHttpActionResult> Delete()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var operation = new
                {
                    ResourceId = "ada058bd-b66f-4266-9dc7-b67222120e89"
                };

                var url = AzureVM + "/labs/virtualmachine";
                var payload = new StringContent(JsonConvert.SerializeObject(operation), Encoding.UTF8, "application/json");
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = payload
                };

                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "NmI2YzJiNzQtMTI4Ni00MTQ1LTljMDctMzdlN2FlYjBmNGFl");
                client.DefaultRequestHeaders.Add("TenantId", "498017a4-4d1b-414d-9bf5-5f7daa8e9367");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                string result = response.Content.ReadAsStringAsync().Result;

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet]
        [Route("DeleteAWS")]
        public async Task<IHttpActionResult> DeleteAWS()
        {
            try
            {
                List<string> listA = new List<string>();
                using (var reader = new StreamReader(@"C:\Users\Kenneth\Desktop\InstanceId.csv"))
                {
                    //var line = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        listA.Add(line);
                    }
                }

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var operation = new
                {
                    account_id = "827347782581",
                    instance_id = "i-0989fa381a3218718",
                    region = "ap-southeast-1",
                    root = "true"
                };

                var url = "https://0h01s47ihg.execute-api.ap-southeast-1.amazonaws.com/";

                HttpClient clientAWS = new HttpClient();
                HttpResponseMessage responseGetDetails = null;
                // JObject getDetails = null;

                clientAWS.BaseAddress = new Uri(url);
                clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                foreach (var item in listA)
                {
                    var dataJson = new
                    {
                        account_id = "827347782581",
                        instance_id = item,
                        region = "ap-southeast-1",
                        root = "true"
                    };

                    var data = JsonConvert.SerializeObject(dataJson);
                    responseGetDetails = await clientAWS.PostAsync("dev/delete_vm", new StringContent(data, Encoding.UTF8, "application/json"));
                    var s = responseGetDetails.Content.ReadAsStringAsync().Result;
                }


                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [Route("ReadFile")]
        [HttpPost]
        public string ReadFile()
        {
            try
            {
                string message = "";
                HttpResponseMessage ResponseMessage = null;
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;

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
                        else
                            message = "The file format is not supported.";

                        dsexcelRecords = reader.AsDataSet();
                        reader.Close();
                        List<Regions> objStudent1 = new List<Regions>();

                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            DataTable dtStudentRecords = dsexcelRecords.Tables[0];
                            for (int i = 1; i < dtStudentRecords.Rows.Count; i++)
                            {
                                Regions objStudent = new Regions();
                                objStudent.RegionId = Convert.ToInt32(dtStudentRecords.Rows[i][0]);
                                objStudent.RegionName = Convert.ToString(dtStudentRecords.Rows[i][1]);

                                objStudent1.Add(objStudent);
                            }

                            message = "Success";
                        }
                        else
                            message = "Selected file is empty.";
                    }
                    else
                        message = "Invalid File.";
                }
                else
                    ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);

                return message;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        [HttpGet]
        [Route("DuplicateList")]
        public async Task<IHttpActionResult> DuplicateList()
        {
            try
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                List<string> listC = new List<string>();

                using (var reader = new StreamReader(@"C:\Users\Kenneth\Desktop\DuplicateList.csv"))
                {
                    //var line = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        listA.Add(line);
                    }
                }
                using (var reader = new StreamReader(@"C:\Users\Kenneth\Desktop\DuplicateOriginal.csv"))
                {
                    //var line = reader.ReadLine();
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        listB.Add(line);
                    }
                }

                foreach (var item in listA)
                {
                    if (!listB.Contains(item))
                        listC.Add(item);

                }

                return Ok(listC);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        public static string AddMachineToDatabaseUle(string machineName, string GuacamoleUrl, string guacCon, string vmusername, string vmpassword, int VETypeId, string environment, string region, string fqdn)
        {
            try
            {
                var guacDatabase = new MySqlConnection(guacCon);
                var Environment = environment.Trim() == "D" ? "Staging" : environment.Trim() == "Q" ? "QA" : environment.Trim() == "U" ? "Demo" : "Prod";

                guacDatabase.Open();
                string selectQuery = "";
                string protocol = "rdp";

                selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '{machineName}%'";
                var MySqlCommandConn = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader = MySqlCommandConn.ExecuteReader();

                dataReader.Read();
                //var guacamoleInstance = new List<GuacamoleInstance>();
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i));

                    var hostName = machineName;

                    var insertQuery = "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                        $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'4\')";

                    var insertCommand = new MySqlCommand(insertQuery, guacDatabase);


                    insertCommand.ExecuteNonQuery();

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var insertParamsQuery = string.Empty;


                    insertParamsQuery =
                        "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', 'nla'), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    //windows
                    if (VETypeId == 1 || VETypeId == 3)
                    {
                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }
                    //linux
                    else if (VETypeId == 2 || VETypeId == 4)
                    {
                        insertParamsQuery = "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        $"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', ''), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }


                    insertParamsCommand.ExecuteNonQuery();
                    selectQuery = $"SELECT entity_id FROM guacamole_entity WHERE name = '{Environment}'";
                    MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                    var dataReader2 = MySqlCommand.ExecuteReader();
                    dataReader2.Read();
                    var userId = Convert.ToInt32(dataReader2["entity_id"]);
                    dataReader2.Close();

                    var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(entity_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                    var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                    insertPermissionCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;
                }
                else
                {
                    dataReader.Close();
                    return "";

                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
                return "";
            }

        }

        [HttpGet]
        [Route("UpdateAWS")]
        public async Task<IHttpActionResult> UpdateAWS()
        {
            var machineLabs = _db.MachineLabs.Where(q => q.IsStarted == 4)
                   .Join(_db.VEProfiles,
                   a => a.VEProfileId,
                   b => b.VEProfileID,
                   (a, b) => new { a, b })
                   .Join(_db.VirtualEnvironments,
                   c => c.b.VirtualEnvironmentID,
                   d => d.VirtualEnvironmentID,
                   (c, d) => new { c, d })
                   .Join(_db.CloudLabUsers,
                   e => e.c.a.UserId,
                   f => f.UserId,
                   (e, f) => new { e, f }).Select(w => new { w.e.c.a.ResourceId, w.e.c.a.VEProfileId, w.e.d.VETypeID, w.e.c.a.UserId, w.f.TenantId }).Where(s => s.VETypeID == 9).ToList();

            int counter = 0;

            DateTime dateUtc = DateTime.UtcNow;
            var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == 2).Select(q => new { q.TenantId, q.ClientCode, q.GuacamoleURL, q.GuacConnection, q.EnvironmentCode }).FirstOrDefault();
            var envi = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "DEV" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "Q" ? "QA" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "U" ? "DMO" : "PRD";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient clientAWS = new HttpClient();
            HttpResponseMessage responseGetDetails = null;
            JObject getDetails = null;

            clientAWS.BaseAddress = new Uri(AWSVM);
            clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpClient clientAWSHB = new HttpClient();
            HttpResponseMessage responseGetDetailsHB = null;

            clientAWSHB.BaseAddress = new Uri(AWSVM);
            clientAWSHB.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpClient clientMac = new HttpClient();

            clientMac.BaseAddress = new Uri(MacOS);
            clientMac.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            foreach (var item in machineLabs)
            {
                var vm = _db.MachineLabs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                var mls = _db.MachineLogs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();

                if (vm.ResourceId != "No Instance")
                {
                    var jsonDetailsHB = new
                    {
                        instance_id = vm.ResourceId,
                        region = "ap-southeast-1"
                    };

                    var jsonDataHB = JsonConvert.SerializeObject(jsonDetailsHB);


                    responseGetDetailsHB = await clientAWSHB.PostAsync("dev/minutes_rendered", new StringContent(jsonDataHB, Encoding.UTF8, "application/json"));

                    if (responseGetDetailsHB.Content.ReadAsStringAsync().Result != "[]" && vm.IsStarted == 1)
                    {
                        var obj = JsonConvert.DeserializeObject<dynamic>(responseGetDetailsHB.Content.ReadAsStringAsync().Result);
                        HeartBeatAWS info = ((JArray)obj)[0].ToObject<HeartBeatAWS>();

                        var cs = _db.CloudLabsSchedule.Where(q => q.MachineLabsId == vm.MachineLabsId).FirstOrDefault();

                        var minRendered = info.minutes_rendered;

                        var difference = (cs.LabHoursTotal * 3600) - (Convert.ToInt64(minRendered) * 60);

                        cs.TimeRemaining = difference;

                        _db.SaveChanges();
                        string dataParseStop = "";

                        if (cs.TimeRemaining <= 0)
                        {
                            if (item.VETypeID == 9)
                            {
                                var dataJsonStop = new
                                {
                                    ec2_id = vm.ResourceId,
                                    region = "ap-southeast-1"
                                };
                                dataParseStop = JsonConvert.SerializeObject(dataJsonStop);
                            }
                            else if (item.VETypeID == 8)
                            {
                                var dataJsonStop = new
                                {
                                    // student_identifier = ms.StudentIdentifier,
                                    ec2_id = vm.ResourceId,
                                    region = "ap-southeast-1"
                                };
                                dataParseStop = JsonConvert.SerializeObject(dataJsonStop);
                            }

                            var responseStop = await clientMac.PostAsync("dev/mac_instance/stop_mac_instance", new StringContent(dataParseStop, Encoding.UTF8, "application/json"));
                            //var getDetailsStop = JObject.Parse(responseStop.Content.ReadAsStringAsync().Result);
                        }

                    }

                    responseGetDetails = await clientAWS.PostAsync("dev/get_vm_details", new StringContent(jsonDataHB, Encoding.UTF8, "application/json"));

                    getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

                    try
                    {
                        var isRunning = getDetails.SelectToken("Reservations[0].Instances[0].State.Name").ToString();

                        if (isRunning == "running" && vm.IsStarted != 1)
                        {
                            var DNS = getDetails.SelectToken("Reservations[0].Instances[0].PublicDnsName").ToString();

                            if (item.VETypeID == 8)
                            {
                                var storagePayload = new
                                {
                                    // student_identifier = ms.StudentIdentifier,
                                    ec2_id = item.ResourceId,
                                    region = "ap-southeast-1"
                                };
                                var jsonData = JsonConvert.SerializeObject(storagePayload);
                                responseGetDetailsHB = await clientMac.PostAsync("dev/mac_instance/mount_storage_account", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                            }

                            if (vm.GuacDNS == null)
                            {
                                //var guacUrl = AddMachineToDatabase(envi + '-' + tenant.ClientCode + '-' + vm.ResourceId, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VEType, tenant.EnvironmentCode, DNS);
                                var guacUrl = AddMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VETypeID, tenant.EnvironmentCode, DNS);
                                vm.GuacDNS = guacUrl;
                                _db.SaveChanges();
                            }

                            if (vm.FQDN != DNS)
                            {
                                var guacurl = EditMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, tenant.EnvironmentCode, DNS);

                                vm.IsStarted = 1;
                                vm.MachineStatus = "Running";
                                vm.FQDN = DNS;
                                if (guacurl != "")
                                    vm.GuacDNS = guacurl;
                                vm.RunningBy = 1;

                                mls.Logs = "(Running)" + dateUtc + "---" + mls.Logs;
                                mls.LastStatus = "Running";
                                mls.ModifiedDate = dateUtc;
                                _db.Entry(mls).State = EntityState.Modified;

                                _db.Entry(vm).State = EntityState.Modified;

                                _db.SaveChanges();
                            }

                        }

                        if (isRunning == "stopped" && vm.IsStarted != 0)
                        {
                            var v = _db.MachineLabs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();

                            HttpClient clientremove = new HttpClient();

                            clientremove.BaseAddress = new Uri(AWSVM);
                            clientremove.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            var data = new
                            {
                                instance_id = v.ResourceId,
                                ip_address = v.IpAddress,
                                region = "ap-southeast-1",
                                action_type = "REMOVE"
                            };

                            var dataRemove = JsonConvert.SerializeObject(data);
                            await clientremove.PostAsync("dev/update_security_group", new StringContent(dataRemove, Encoding.UTF8, "application/json"));

                            v.IsStarted = 0;
                            v.MachineStatus = "Deallocated";
                            //v.VMName = envi + '-' + tenant.ClientCode + '-' + v.ResourceId;
                            v.MachineName = "UI" + v.UserId + "VE" + v.VEProfileId;
                            //v.FQDN = "UI30VE89";
                            //v.GuacDNS = "UI30VE89";
                            //v.MachineName = "UI30VE89";
                            v.RunningBy = 0;
                            v.IpAddress = null;
                            _db.Entry(v).State = EntityState.Modified;
                            _db.SaveChanges();
                            mls.Logs = "(Deallocated)" + dateUtc + "---" + mls.Logs;
                            mls.LastStatus = "Deallocated";
                            mls.ModifiedDate = dateUtc;
                            //_db.Entry(mls).State = EntityState.Modified;

                            //_db.Entry(vm).State = EntityState.Modified;

                            _db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {

                    }

                }

                counter++;
            }

            return Ok("Total: " + counter);
        }


        [HttpGet]
        [Route("Guac")]
        public async Task<IHttpActionResult> guac()
        {
            DateTime dateUtc = DateTime.UtcNow;

            var machineLabs = _db.MachineLabs
                         .Join(_db.CloudLabUsers,
                         e => e.UserId,
                         f => f.UserId,
                         (e, f) => new { e, f }).
                         Select(w => new
                         {
                             ResourceId = w.e.ResourceId,
                             VEProfileId = w.e.VEProfileId,
                             UserId = w.f.UserId,
                             TenantId = w.f.TenantId,
                             VMName = w.e.VMName,
                             MachineName = w.e.MachineName,
                             FQDN = w.e.FQDN,
                             Username = w.e.Username,
                             Password = w.e.Password,
                             GuacDNS = w.e.GuacDNS,
                             IsStarted = w.e.IsStarted,
                             ScheduledBy = w.e.ScheduledBy
                         }).ToList();


            foreach (var item in machineLabs)
            {
                try
                {
                    var vm = _db.MachineLabs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                    var guacURL = AddMachineToDatabase(item.MachineName, "https://cswstbgrndrappprd.cloudswyft.com/", "user id=guacamole_user;password=CloudSwyft2020!;server=cswstbgrndrappprd.australiaeast.cloudapp.azure.com;port=3306;database=guacamole_db;connectiontimeout=3000;defaultcommandtimeout=3000;protocol=Socket",
                        item.Username, Decrypt(item.Password), 1, "P", "australiaeast", item.FQDN);

                    if (guacURL != "")
                    {
                        var _ml = _db.MachineLabs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                        var _mLogs = _db.MachineLogs.Where(q => q.ResourceId == _ml.ResourceId).FirstOrDefault();
                        var courseHours = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == _ml.VEProfileId && q.GroupID == _db.CloudLabUsers.Where(w => w.UserId == _ml.UserId).FirstOrDefault().UserGroup).FirstOrDefault().CourseHours;

                        CloudLabsSchedule _cs = new CloudLabsSchedule();

                        _ml.IsStarted = 0;
                        _ml.MachineStatus = "Deallocated";
                        _ml.RunningBy = 0;
                        _ml.GuacDNS = guacURL;
                        _ml.FQDN = _ml.VMName + ".australiaeast.cloudapp.azure.com";
                        _ml.DateProvision = DateTime.UtcNow.ToString();
                        _db.Entry(_ml).State = EntityState.Modified;
                        _db.SaveChanges();

                        _mLogs.ModifiedDate = DateTime.UtcNow;
                        _mLogs.LastStatus = "Deallocated";
                        _mLogs.Logs = "(Deallocated)" + DateTime.UtcNow + "---" + _mLogs.Logs;
                        _db.Entry(_mLogs).State = EntityState.Modified;
                        _db.SaveChanges();

                        if (_dbCustomer.VirtualMachineDetails.Any(q => q.ResourceId == _ml.ResourceId))
                        {
                            var _vmCustomer = _dbCustomer.VirtualMachineDetails.Where(q => q.ResourceId == _ml.ResourceId).FirstOrDefault();
                            _vmCustomer.DateLastModified = DateTime.UtcNow;
                            _vmCustomer.Status = 0;
                            _dbCustomer.Entry(_vmCustomer).State = EntityState.Modified;
                            _dbCustomer.SaveChanges();
                        }
                        else
                        {
                            VirtualMachineDetails _vmCustomer = new VirtualMachineDetails();
                            _vmCustomer.ResourceId = _ml.ResourceId;
                            _vmCustomer.DateLastModified = DateTime.UtcNow;
                            _vmCustomer.DateCreated = DateTime.UtcNow;
                            _vmCustomer.Status = 0;
                            _vmCustomer.VMName = _ml.VMName;
                            _vmCustomer.FQDN = _ml.VMName + ".australiaeast.cloudapp.azure.com";
                            _vmCustomer.OperationId = "";
                            _dbCustomer.VirtualMachineDetails.Add(_vmCustomer);
                            _dbCustomer.SaveChanges();
                        }



                        vm.GuacDNS = guacURL;

                        _db.Entry(vm).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }
                catch (Exception e)
                {

                }

            }


            return Ok();

        }
        
        public static string AddMachineToDatabase(string machineName, string GuacamoleUrl, string guacCon, string vmusername, string vmpassword, int VETypeId, string environment, string region, string fqdn)
        {
            try
            {
                var guacDatabase = new MySqlConnection(guacCon);
                var Environment = environment.Trim() == "D" ? "Staging" : environment.Trim() == "Q" ? "QA" : environment.Trim() == "U" ? "Demo" : "Prod";

                guacDatabase.Open();
                string selectQuery = "";
                string protocol = "rdp";

                selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name LIKE '{machineName}%'";
                var MySqlCommandConn = new MySqlCommand(selectQuery, guacDatabase);
                var dataReader = MySqlCommandConn.ExecuteReader();

                dataReader.Read();
                //var guacamoleInstance = new List<GuacamoleInstance>();
                if (!dataReader.HasRows)
                {
                    dataReader.Close();
                    //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i));

                    var hostName = machineName;

                    var insertQuery = "INSERT INTO guacamole_connection (connection_name, protocol, max_connections, max_connections_per_user) " +
                        $"VALUES (\'{hostName}-{protocol}\', \'{protocol}\', \'5\', \'4\')";

                    var insertCommand = new MySqlCommand(insertQuery, guacDatabase);


                    insertCommand.ExecuteNonQuery();

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var insertParamsQuery = string.Empty;


                    insertParamsQuery =
                        "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', 'nla'), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    //windows
                    if (VETypeId == 1 || VETypeId == 3)
                    {
                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }
                    //linux
                    else if (VETypeId == 2 || VETypeId == 4)
                    {
                        insertParamsQuery = "INSERT INTO guacamole_connection_parameter (connection_id, parameter_name, parameter_value) " +
                        //$"VALUES ({connectionId}, 'hostname', '{machineName.ToLower()}.{region}.cloudapp.azure.com'), " +
                        $"VALUES ({connectionId}, 'hostname', '{fqdn}'), " +
                        $"({connectionId}, 'ignore-cert', 'true'), " +
                        $"({connectionId}, 'password', '{vmpassword}'), " +
                        $"({connectionId}, 'security', ''), " +
                        $"({connectionId}, 'port', '3389'), " +
                        $"({connectionId}, 'enable-wallpaper', 'true'), " +
                        $"({connectionId}, 'username', '{vmusername}')";

                        insertParamsCommand = new MySqlCommand(insertParamsQuery, guacDatabase);
                    }


                    insertParamsCommand.ExecuteNonQuery();
                    selectQuery = $"SELECT entity_id FROM guacamole_entity WHERE name = '{Environment}'";
                    MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);
                    var dataReader2 = MySqlCommand.ExecuteReader();
                    dataReader2.Read();
                    var userId = Convert.ToInt32(dataReader2["entity_id"]);
                    dataReader2.Close();

                    var insertPermissionQuery = string.Format("INSERT INTO guacamole_connection_permission(entity_id, connection_id, permission) VALUES ({1},{0}, 'READ')", connectionId, userId);

                    var insertPermissionCommand = new MySqlCommand(insertPermissionQuery, guacDatabase);

                    insertPermissionCommand.ExecuteNonQuery();

                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;
                }
                else
                {
                    dataReader.Close();
                    //guacamoleInstance.Add(GenerateGuacamoleInstance(staticIp, machineName, guacDatabase, "rdp", 1, i));

                    var hostName = machineName;

                    selectQuery = $"SELECT connection_id FROM guacamole_connection WHERE connection_name = \'{hostName}-{protocol}\'";

                    var MySqlCommand = new MySqlCommand(selectQuery, guacDatabase);

                    var dataReaderIns = MySqlCommand.ExecuteReader();

                    dataReaderIns.Read();
                    var connectionId = Convert.ToInt32(dataReaderIns["connection_id"]);

                    dataReaderIns.Close();

                    var guacUrlHostName = GuacamoleUrl;
                    guacUrlHostName = guacUrlHostName.Replace("http://", string.Empty);

                    var updateParamsQuery = string.Empty;

                    updateParamsQuery = "UPDATE guacamole_connection_parameter SET parameter_value = '" + fqdn + "' WHERE connection_Id = " + connectionId + " and parameter_name = 'hostname'";

                    MySqlCommand insertParamsCommand = new MySqlCommand();
                    //windows
                    if (VETypeId == 1 || VETypeId == 3)
                    {
                        insertParamsCommand = new MySqlCommand(updateParamsQuery, guacDatabase);
                        MySqlCommand updateParamsCommand = new MySqlCommand();

                        updateParamsCommand = new MySqlCommand(updateParamsQuery, guacDatabase);
                        updateParamsCommand.ExecuteNonQuery();
                    }
                    //linux
                    else if (VETypeId == 2 || VETypeId == 4)
                    {
                        updateParamsQuery = "UPDATE guacamole_connection_parameter SET parameter_value = '" + fqdn + "' WHERE connection_Id = "
                            + connectionId + " and parameter_name = 'hostname'";

                        MySqlCommand updateParamsCommand = new MySqlCommand();

                        updateParamsCommand = new MySqlCommand(updateParamsQuery, guacDatabase);
                        updateParamsCommand.ExecuteNonQuery();

                        var updateParamsQuery1 = "UPDATE guacamole_connection_parameter SET parameter_value = '" + "" + "' WHERE connection_Id = "
                            + connectionId + " and parameter_name = 'security'";

                        MySqlCommand updateParamsCommand1 = new MySqlCommand();

                        updateParamsCommand1 = new MySqlCommand(updateParamsQuery1, guacDatabase);
                        updateParamsCommand1.ExecuteNonQuery();

                    }




                    var clientId = new string[3] { connectionId.ToString(), "c", "mysql" };

                    var bytes = Encoding.UTF8.GetBytes(string.Join("\0", clientId));
                    var connectionString = Convert.ToBase64String(bytes);

                    var guacUrl =
                        $"{GuacamoleUrl}/guacamole/#/client/{connectionString}?username={Environment}&password=pr0v3byd01n6!";


                    var guacamoleInstance = new GuacamoleInstance()
                    {
                        Connection_Name = hostName,
                        Hostname = guacUrlHostName,
                        Url = guacUrl
                    };

                    guacDatabase.Close();

                    return guacamoleInstance.Url;


                }
            }
            catch (Exception ex)
            {
                var x = ex.InnerException;
                return "";
            }

        }

        [HttpGet]
        [Route("AWSUpdate")]
        public async Task<HttpResponseMessage> AWSUpdate(int tenantId = 13)
        {
            var coursesList = new List<CourseDetails>();
            var GetStudentConsole = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/";
            var url = "dev/get_current_budget_of_student/";

            var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.TenantId, q.ClientCode, q.GuacamoleURL, q.GuacConnection, q.EnvironmentCode }).FirstOrDefault();
            var envi = tenant.EnvironmentCode.Replace(" ", String.Empty) == "D" ? "DEV" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "Q" ? "QA" : tenant.EnvironmentCode.Replace(" ", String.Empty) == "U" ? "DMO" : "PRD";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri(AzureVM);
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

            HttpResponseMessage response = null;

            GuacamoleInstancesController guac = new GuacamoleInstancesController();

            try
            {
                var vmUsers = _db.CourseGrants
                    .Join(_db.CloudLabUsers,
                    a => a.UserID,
                    b => b.UserId,
                    (a, b) => new { a, b })
                    .Join(_db.VEProfiles,
                    c => c.a.VEProfileID,
                    d => d.VEProfileID,
                    (c, d) => new { c, d })
                    .Join(_db.VEProfileLabCreditMappings,
                    e => new { ab = e.c.b.UserGroup, aa = e.d.VEProfileID },
                    f => new { ab = f.GroupID, aa = f.VEProfileID },
                    (e, f) => new { e, f })
                    .Join(_db.VirtualEnvironments,
                    g => g.e.d.VirtualEnvironmentID,
                    h => h.VirtualEnvironmentID,
                    (g, h) => new { g, h })
                    .Where(w => w.g.e.c.a.IsCourseGranted)
                    .Select(s => new { s.g.e.d.VEProfileID, s.g.e.c.b.UserGroup, s.g.e.d.Name, s.h.VETypeID, s.g.e.c.b.UserId, s.g.e.d.ThumbnailURL, s.g.e.c.a.IsCourseGranted, s.g.f.MachineSize, s.h.Title }).Where(q => q.VETypeID != 6 && q.VETypeID != 7 && q.UserGroup == 14).ToList();  // not for consoles


                foreach (var item in vmUsers)
                {
                    var courseDetail = new CourseDetails();

                    if (_db.MachineLabs.Any(q => q.UserId == item.UserId && q.VEProfileId == item.VEProfileID))
                    {
                        var course = _db.MachineLabs.Where(q => q.UserId == item.UserId && q.VEProfileId == item.VEProfileID).FirstOrDefault();

                        var sched = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == course.MachineLabsId) ?
                            _db.CloudLabsSchedule.Where(q => q.MachineLabsId == course.MachineLabsId).FirstOrDefault() : null;

                        var isUserHasExtension = _db.LabHourExtensions.Where(q => q.VEProfileId == item.VEProfileID && q.UserId == item.UserId && q.IsDeleted == false && q.ExtensionTypeId == 1).ToList()
                            .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);

                        courseDetail.veprofileid = item.VEProfileID;
                        courseDetail.UserId = item.UserId;
                        courseDetail.Name = item.Name;
                        courseDetail.VEType = item.VETypeID;

                        courseDetail.TimeRemaining = sched != null ? sched.TimeRemaining : 0;

                        courseDetail.RunningBy = course.RunningBy;
                        courseDetail.MachineStatus = course.MachineStatus;
                        courseDetail.IsStarted = course.IsStarted;
                        courseDetail.LabHoursTotal = sched != null ? sched.LabHoursTotal : 0;
                        courseDetail.InstructorLabHours = sched != null ? sched.InstructorLabHours : 0;
                        courseDetail.GuacamoleUrl = course.GuacDNS;
                        courseDetail.Thumbnail = item.ThumbnailURL;
                        courseDetail.ResourceId = course.ResourceId;
                        courseDetail.IsCourseGranted = item.IsCourseGranted;
                        courseDetail.IsProvisioned = 1;
                        courseDetail.Username = course.Username;
                        courseDetail.Password = course.Password != null ? Decrypt(course.Password) : null;
                        courseDetail.MachineStatus = course.MachineStatus;
                        courseDetail.FQDN = course.FQDN;
                        courseDetail.CourseCode = item.Title;
                        courseDetail.IsExtend = isUserHasExtension;
                        courseDetail.MachineLabsId = course.MachineLabsId;
                        courseDetail.IpAddress = course.IpAddress;

                    }
                    else
                    {
                        courseDetail.Name = item.Name;
                        courseDetail.veprofileid = item.VEProfileID;
                        courseDetail.UserId = item.UserId;
                        courseDetail.Name = item.Name;
                        courseDetail.VEType = item.VETypeID;
                        courseDetail.IsStarted = null;
                        //courseDetail.LabHoursRemaining = null;
                        courseDetail.Thumbnail = item.ThumbnailURL;
                        courseDetail.IsCourseGranted = item.IsCourseGranted;
                        courseDetail.MachineSize = item.MachineSize;
                        courseDetail.CourseCode = item.Title;

                    }

                    coursesList.Add(courseDetail);
                }

                foreach (var item in coursesList)
                {
                    VMSuccess dataJson = new VMSuccess();

                    var vm = _db.MachineLabs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                    var mls = _db.MachineLogs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
                    DateTime dateUtc = DateTime.UtcNow;

                    if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId) && (item.VEType == 9 || item.VEType == 8)) // for AWS windows
                    {
                        HttpClient clientAWS = new HttpClient();
                        HttpResponseMessage responseGetDetails = null;
                        JObject getDetails = null;

                        clientAWS.BaseAddress = new Uri(AWSVM);
                        clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpClient clientAWSHB = new HttpClient();
                        HttpResponseMessage responseGetDetailsHB = null;

                        clientAWSHB.BaseAddress = new Uri(AWSVM);
                        clientAWSHB.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        HttpClient clientMac = new HttpClient();

                        clientMac.BaseAddress = new Uri(MacOS);
                        clientMac.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                        if (vm.ResourceId != "No Instance")
                        {
                            var jsonDetailsHB = new
                            {
                                instance_id = vm.ResourceId,
                                region = "ap-southeast-1"
                            };

                            var jsonDataHB = JsonConvert.SerializeObject(jsonDetailsHB);

                            //var jsonDetails = new
                            //{
                            //    instance_id = vm.ResourceId,
                            //    region = "ap-southeast-1"
                            //};

                            //var jsonData = JsonConvert.SerializeObject(jsonDetails);

                            responseGetDetailsHB = await clientAWSHB.PostAsync("dev/minutes_rendered", new StringContent(jsonDataHB, Encoding.UTF8, "application/json"));

                            if (responseGetDetailsHB.Content.ReadAsStringAsync().Result != "[]" && vm.IsStarted == 1)
                            {
                                var obj = JsonConvert.DeserializeObject<dynamic>(responseGetDetailsHB.Content.ReadAsStringAsync().Result);
                                HeartBeatAWS info = ((JArray)obj)[0].ToObject<HeartBeatAWS>();

                                var cs = _db.CloudLabsSchedule.Where(q => q.MachineLabsId == item.MachineLabsId).FirstOrDefault();

                                var minRendered = info.minutes_rendered;

                                var difference = (cs.LabHoursTotal * 3600) - (Convert.ToInt64(minRendered) * 60);

                                cs.TimeRemaining = difference;

                                mls.LastStatus = "Running";
                                mls.ModifiedDate = dateUtc;
                                _db.Entry(mls).State = EntityState.Modified;

                                _db.SaveChanges();
                                string dataParseStop = "";

                                if (cs.TimeRemaining <= 0)
                                {
                                    if (item.VEType == 9)
                                    {
                                        var dataJsonStop = new
                                        {
                                            ec2_id = vm.ResourceId,
                                            region = "ap-southeast-1"
                                        };
                                        dataParseStop = JsonConvert.SerializeObject(dataJsonStop);
                                    }
                                    else if (item.VEType == 8)
                                    {
                                        var dataJsonStop = new
                                        {
                                            // student_identifier = ms.StudentIdentifier,
                                            ec2_id = vm.ResourceId,
                                            region = "ap-southeast-1"
                                        };
                                        dataParseStop = JsonConvert.SerializeObject(dataJsonStop);
                                    }

                                    var responseStop = await clientMac.PostAsync("dev/mac_instance/stop_mac_instance", new StringContent(dataParseStop, Encoding.UTF8, "application/json"));
                                    //var getDetailsStop = JObject.Parse(responseStop.Content.ReadAsStringAsync().Result);
                                }

                            }

                            responseGetDetails = await clientAWS.PostAsync("dev/get_vm_details", new StringContent(jsonDataHB, Encoding.UTF8, "application/json"));

                            getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

                            var isRunning = getDetails.SelectToken("Reservations[0].Instances[0].State.Name").ToString();

                            if (isRunning == "running" && vm.IsStarted != 1)
                            {
                                var DNS = getDetails.SelectToken("Reservations[0].Instances[0].PublicDnsName").ToString();

                                if (item.VEType == 8)
                                {
                                    var storagePayload = new
                                    {
                                        // student_identifier = ms.StudentIdentifier,
                                        ec2_id = item.ResourceId,
                                        region = "ap-southeast-1"
                                    };
                                    var jsonData = JsonConvert.SerializeObject(storagePayload);
                                    responseGetDetailsHB = await clientMac.PostAsync("dev/mac_instance/mount_storage_account", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                                }

                                if (vm.GuacDNS == null)
                                {
                                    //var guacUrl = AddMachineToDatabase(envi + '-' + tenant.ClientCode + '-' + vm.ResourceId, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VEType, tenant.EnvironmentCode, DNS);
                                    var guacUrl = AddMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VEType, tenant.EnvironmentCode, DNS);
                                    vm.GuacDNS = guacUrl;
                                    _db.SaveChanges();
                                }

                                if (vm.FQDN != DNS)
                                {
                                    var guacurl = EditMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, tenant.EnvironmentCode, DNS);

                                    vm.IsStarted = 1;
                                    vm.MachineStatus = "Running";
                                    vm.FQDN = DNS;
                                    if (guacurl != "")
                                        vm.GuacDNS = guacurl;
                                    vm.RunningBy = 1;

                                    mls.Logs = "(Running)" + dateUtc + "---" + mls.Logs;
                                    mls.LastStatus = "Running";
                                    mls.ModifiedDate = dateUtc;
                                    _db.Entry(mls).State = EntityState.Modified;

                                    _db.Entry(vm).State = EntityState.Modified;

                                    _db.SaveChanges();
                                }

                            }

                            if (isRunning == "stopped" && vm.IsStarted != 0)
                            {

                                var v = _db.MachineLabs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();

                                HttpClient clientremove = new HttpClient();

                                clientremove.BaseAddress = new Uri(AWSVM);
                                clientremove.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                                var data = new
                                {
                                    instance_id = v.ResourceId,
                                    ip_address = v.IpAddress,
                                    region = "ap-southeast-1",
                                    action_type = "REMOVE"
                                };

                                var dataRemove = JsonConvert.SerializeObject(data);
                                await clientremove.PostAsync("dev/update_security_group", new StringContent(dataRemove, Encoding.UTF8, "application/json"));

                                v.IsStarted = 0;
                                v.MachineStatus = "Deallocated";
                                //v.VMName = envi + '-' + tenant.ClientCode + '-' + v.ResourceId;
                                v.MachineName = "UI" + item.UserId.ToString() + "VE" + v.VEProfileId;
                                //v.FQDN = "UI30VE89";
                                //v.GuacDNS = "UI30VE89";
                                //v.MachineName = "UI30VE89";
                                v.RunningBy = 0;
                                v.IpAddress = null;
                                _db.Entry(v).State = EntityState.Modified;
                                _db.SaveChanges();
                                mls.Logs = "(Deallocated)" + dateUtc + "---" + mls.Logs;
                                mls.LastStatus = "Deallocated";
                                mls.ModifiedDate = dateUtc;
                                //_db.Entry(mls).State = EntityState.Modified;

                                //_db.Entry(vm).State = EntityState.Modified;

                                _db.SaveChanges();
                            }
                        }

                    } //for AWS Windows

                    if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId) && item.VEType == 8) // for MacOS
                    {
                        HttpClient clientMac = new HttpClient();
                        clientMac.BaseAddress = new Uri(AWSVM);
                        clientMac.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var jsonDetails = new
                        {
                            instance_id = vm.ResourceId,
                            region = "ap-southeast-1"
                        };

                        var jsonData = JsonConvert.SerializeObject(jsonDetails);

                        var getMacDetails = await clientMac.PostAsync("dev/minutes_rendered", new StringContent(jsonData, Encoding.UTF8, "application/json"));

                        if (getMacDetails.Content.ReadAsStringAsync().Result != "[]" && vm.MachineStatus == "Virtual machine provisioning started")
                        {

                        }

                    } // for MacOS

                }

                return Request.CreateResponse(HttpStatusCode.OK, coursesList);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            finally
            {
                // _dbCon.Close();
            }
        }


        [HttpPost]
        [Route("ProvisionVM")]
        public async Task<IHttpActionResult> ProvisionVM(DataProvision dataContents, int tenantId, string schedBy, bool selfProv, string token)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ProvisionVMAzure);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpClient clientFA = new HttpClient();
                clientFA.BaseAddress = new Uri(FunctionAppUrl);
                clientFA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpClient clientGCP = new HttpClient();
                clientGCP.BaseAddress = new Uri(GCPServer);
                clientGCP.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                clientGCP.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                
                VHDDetails newTemp = new VHDDetails();
                var userGroup = _db.CloudLabsGroups.Where(q => q.TenantId == tenantId).FirstOrDefault();

                var VMvhdUrl = _db.VEProfiles.Where(q => q.VEProfileID == dataContents.VEProfileID).Select(w => new { w.VirtualEnvironmentID, w.VEProfileID }).Join(_db.VirtualEnvironmentImages,
                    a => a.VirtualEnvironmentID,
                    b => b.VirtualEnvironmentID,
                    (a, b) => new { a, b })
                    .Where(w => w.b.GroupId == userGroup.CloudLabsGroupID).FirstOrDefault().b.Name;

                var veImages = _db.VEProfiles.Where(q => q.VEProfileID == dataContents.VEProfileID).Select(w => new { w.VirtualEnvironmentID, w.VEProfileID }).Join(_db.VirtualEnvironmentImages,
                    a => a.VirtualEnvironmentID,
                    b => b.VirtualEnvironmentID,
                    (a, b) => new { a, b })
                    .Where(w=>w.b.GroupId == userGroup.CloudLabsGroupID)
                    .FirstOrDefault().b;

                var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).FirstOrDefault();
                var groupPrefix = _db.CloudLabsGroups.Where(q => q.TenantId == tenantId).FirstOrDefault().CLPrefix;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var Environment = tenant.EnvironmentCode.Trim() == "D" ? "DEV" : tenant.EnvironmentCode.Trim() == "Q" ? "QA" : tenant.EnvironmentCode.Trim() == "U" ? "DMO" : "PRD";

                var veType = _db.VEProfiles.Where(q => q.VEProfileID == dataContents.VEProfileID).Join(_db.VirtualEnvironments,
                    a => a.VirtualEnvironmentID,
                    b => b.VirtualEnvironmentID,
                    (a, b) => new { a, b }).Join(_db.VETypes, c => c.b.VETypeID, d => d.VETypeID, (c, d) => new { c, d }).Select(q => q.d).FirstOrDefault();

                var diskSize = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID && q.GroupID == userGroup.CloudLabsGroupID).FirstOrDefault().DiskSize;

                if (dataContents.MachineSize == null)
                {
                    dataContents.MachineSize = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID && q.GroupID == userGroup.CloudLabsGroupID).FirstOrDefault().MachineSize;
                }

                foreach (var item in dataContents.UserId)
                {
                    if (dataContents.VMEmptyData != null)
                    {
                        foreach (var dataVM in dataContents.VMEmptyData)
                        {
                            if (dataVM.UserId == item)
                            {
                                if (veType.VETypeID == 3 || veType.VETypeID == 4)
                                {
                                    newTemp = new VHDDetails
                                    {
                                        ImageName = VMvhdUrl,
                                        Size = dataContents.MachineSize,
                                        NewImageName = dataVM.MachineName.ToUpper()
                                    };
                                }

                            }

                        }
                    }

                    MachineLabs vmm = new MachineLabs();
                    CourseGrants cg = new CourseGrants();

                    string computerName = "U" + item + "V" + dataContents.VEProfileID;
                    string ResourceId = Guid.NewGuid().ToString();
                    string username = GenerateUserNameRandomName();
                    string password = GeneratePasswordRandomName();
                    string VMName = "CS-" + userGroup.CLPrefix + "-" + tenant.EnvironmentCode.Trim() + "-VM-" + "U" + item + "V" + dataContents.VEProfileID + "-" + ResourceId;


                    var isVMExist = _db.MachineLabs.Any(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID);
                    MachineLogs mls = new MachineLogs();
                    DateTime dateUtc = DateTime.UtcNow;

                    var isCourseGrantExist = _db.CourseGrants.Any(q => q.UserID == item & q.VEProfileID == dataContents.VEProfileID);
                    var courseGrant = _db.CourseGrants.Where(q => q.UserID == item & q.VEProfileID == dataContents.VEProfileID).FirstOrDefault();
                    var veprofileMappings = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID && q.GroupID == userGroup.CloudLabsGroupID).FirstOrDefault();

                    if (!isVMExist)
                    {
                        Thread.Sleep(4000);
                        if (veType.VETypeID <= 4) //windows or linux azure 
                        {

                            var storageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https://") + 8, VMvhdUrl.IndexOf(".") - VMvhdUrl.IndexOf("https://") - 8);

                            object dataV = new object();
                            object qCustom = new object();

                            if (veType.VETypeID <= 2)
                            {
                                if (userGroup.CLPrefix.Length == 5)// version 2.2
                                {
                                    dataV = new
                                    {
                                        SubscriptionId = tenant.SubscriptionId,
                                        TenantId = tenant.ApplicationTenantId,
                                        ApplicationKey = tenant.ApplicationSecretKey,
                                        ApplicationId = tenant.ApplicationId,
                                        Location = tenant.Regions,
                                        Environment = tenant.EnvironmentCode.Trim(),
                                        ClientCode = userGroup.CLPrefix,
                                        VirtualMachineName = VMName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                        Size = dataContents.MachineSize,
                                        IsCustomTemplate = false,
                                        TempStorageSizeInGb = 127,
                                        ImageUri = VMvhdUrl,
                                        ContactPerson = schedBy,
                                        StorageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https", 0) + 8, VMvhdUrl.IndexOf(".blob", (VMvhdUrl.IndexOf("https", 0) + 8)) - (VMvhdUrl.IndexOf("https", 0) + 8)),
                                        OsType = veType.Description,
                                        ComputerName = computerName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                        Username = username,
                                        Password = password,
                                        Fqdn = userGroup.CLUrl.Substring(8, userGroup.CLUrl.Length - 8),// CLMP URL WITHOUT HTTPS userGroup.CLUrl.Split(("https://")[1].ToLower(),
                                        apiprefix = userGroup.ApiPrefix,
                                        uniqueId = ResourceId
                                    };
                                }
                                else if (userGroup.CLPrefix.Length == 3)
                                {
                                    dataV = new
                                    {
                                        SubscriptionId = tenant.SubscriptionId,
                                        TenantId = tenant.ApplicationTenantId,
                                        ApplicationKey = tenant.ApplicationSecretKey,
                                        ApplicationId = tenant.ApplicationId,
                                        Location = tenant.Regions,
                                        Environment = Environment,
                                        ClientCode = userGroup.CLPrefix,
                                        VirtualMachineName = VMName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                        Size = dataContents.MachineSize,
                                        IsCustomTemplate = false,
                                        TempStorageSizeInGb = 127,
                                        ImageUri = VMvhdUrl,
                                        ContactPerson = schedBy,
                                        StorageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https", 0) + 8, VMvhdUrl.IndexOf(".blob", (VMvhdUrl.IndexOf("https", 0) + 8)) - (VMvhdUrl.IndexOf("https", 0) + 8)),
                                        OsType = veType.Description,
                                        ComputerName = computerName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                        Username = username,
                                        Password = password,
                                        Fqdn = userGroup.CLUrl.Substring(8, userGroup.CLUrl.Length - 8),// CLMP URL WITHOUT HTTPS userGroup.CLUrl.Split(("https://")[1].ToLower(),
                                        apiprefix = userGroup.ApiPrefix,
                                        uniqueId = ResourceId,
                                        ResourceGroupName = "CS-" + Environment + "-" + userGroup.CLPrefix
                                    };
                                }


                                var dataMessage = JsonConvert.SerializeObject(dataV);

                                //  await clientRunbook.PostAsync("", new StringContent(dataMessage, Encoding.UTF8, "application/json"));

                                await Task.Run(() =>
                                {
                                    client.PostAsync("", new StringContent(dataMessage, Encoding.UTF8, "application/json"));
                                });
                            }
                            else
                            {
                                if (userGroup.CLPrefix.Length == 5)// version 2.2
                                {
                                    qCustom = new
                                    {
                                        SubscriptionId = tenant.SubscriptionId,
                                        TenantId = tenant.ApplicationTenantId,
                                        ApplicationKey = tenant.ApplicationSecretKey,
                                        ApplicationId = tenant.ApplicationId,
                                        Location = tenant.Regions,
                                        Environment = tenant.EnvironmentCode.Trim(),
                                        ClientCode = userGroup.CLPrefix,
                                        VirtualMachineName = newTemp.NewImageName,
                                        Size = dataContents.MachineSize,
                                        IsCustomTemplate = false,
                                        TempStorageSizeInGb = 127,
                                        ImageUri = VMvhdUrl,
                                        ContactPerson = schedBy,
                                        StorageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https", 0) + 8, VMvhdUrl.IndexOf(".blob", (VMvhdUrl.IndexOf("https", 0) + 8)) - (VMvhdUrl.IndexOf("https", 0) + 8)),
                                        OsType = veType.Description,
                                        ComputerName = computerName,
                                        Username = "cloudswyft",
                                        Password = "CustomPassword1!",
                                        Fqdn = userGroup.CLUrl.Substring(8, userGroup.CLUrl.Length - 8),// CLMP URL WITHOUT HTTPS userGroup.CLUrl.Split(("https://")[1].ToLower(),
                                        apiprefix = userGroup.ApiPrefix,
                                        uniqueId = ResourceId
                                    };
                                }
                                else if (userGroup.CLPrefix.Length == 3)
                                {
                                    qCustom = new
                                    {
                                        SubscriptionId = tenant.SubscriptionId,
                                        TenantId = tenant.ApplicationTenantId,
                                        ApplicationKey = tenant.ApplicationSecretKey,
                                        ApplicationId = tenant.ApplicationId,
                                        Location = tenant.Regions,
                                        Environment = Environment,
                                        ClientCode = userGroup.CLPrefix,
                                        VirtualMachineName = newTemp.NewImageName,
                                        Size = dataContents.MachineSize,
                                        IsCustomTemplate = false,
                                        TempStorageSizeInGb = 127,
                                        ImageUri = VMvhdUrl,
                                        ContactPerson = schedBy,
                                        StorageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https", 0) + 8, VMvhdUrl.IndexOf(".blob", (VMvhdUrl.IndexOf("https", 0) + 8)) - (VMvhdUrl.IndexOf("https", 0) + 8)),
                                        OsType = veType.Description,
                                        ComputerName = computerName,
                                        Username = "cloudswyft",
                                        Password = "CustomPassword1!",
                                        Fqdn = userGroup.CLUrl.Substring(8, userGroup.CLUrl.Length - 8),// CLMP URL WITHOUT HTTPS userGroup.CLUrl.Split(("https://")[1].ToLower(),
                                        apiprefix = userGroup.ApiPrefix,
                                        uniqueId = ResourceId,
                                        ResourceGroupName = "CS-" + Environment + "-" + userGroup.CLPrefix

                                    };
                                }

                                var dataMessage = JsonConvert.SerializeObject(qCustom);

                                await Task.Run(() =>
                                {
                                    client.PostAsync("", new StringContent(dataMessage, Encoding.UTF8, "application/json"));
                                });
                            }

                            vmm.DateProvision = dateUtc.ToString();
                            if (veType.VETypeID <= 2)
                            {
                                vmm.VMName = VMName;
                                vmm.Username = username;
                                vmm.Password = Encrypt(password);
                            }
                            else
                            {
                                vmm.VMName = newTemp.NewImageName;
                                vmm.Username = "cloudswyft";
                                vmm.Password = Encrypt("CustomPassword1!");
                            }

                            vmm.MachineName = computerName;
                            vmm.ResourceId = ResourceId;
                            vmm.UserId = item;
                            vmm.VEProfileId = dataContents.VEProfileID;
                            vmm.IsStarted = 4; // provisioning 
                            vmm.IsDeleted = 0;
                            vmm.MachineStatus = "Provisioning";
                            vmm.ScheduledBy = schedBy;

                            mls.ResourceId = ResourceId;
                            mls.LastStatus = "Provisioning";
                            mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
                            mls.ModifiedDate = dateUtc;

                            _db.MachineLogs.Add(mls);
                            _db.MachineLabs.Add(vmm);
                            _db.SaveChanges();

                            VirtualMachineDetails vmDetails = new VirtualMachineDetails();
                            vmDetails.ResourceId = vmm.ResourceId;
                            vmDetails.Status = vmm.IsStarted;
                            vmDetails.VMName = VMName;
                            vmDetails.FQDN = VMName + "." + tenant.Regions + ".cloudapp.azure.com";
                            vmDetails.DateLastModified = DateTime.UtcNow;
                            vmDetails.DateCreated = DateTime.UtcNow;
                            vmDetails.OperationId = $"virtual-machine-{vmm.ResourceId}";

                            _dbCustomer.VirtualMachineDetails.Add(vmDetails);
                            _dbCustomer.SaveChanges();


                        }
                        else if (veType.VETypeID == 9) //aws windows
                        {
                            MachineLabs ml = new MachineLabs();

                            ml.DateProvision = DateTime.UtcNow.ToString();
                            ml.ResourceId = "No Instance Yet";
                            ml.VEProfileId = dataContents.VEProfileID;
                            ml.UserId = item;
                            ml.MachineStatus = "Provisioning";
                            ml.MachineName = "UI" + item.ToString() + "VE" + dataContents.VEProfileID;
                            ml.IsStarted = 4;
                            ml.ScheduledBy = schedBy;
                            ml.RunningBy = 0;
                            ml.FQDN = null;
                            ml.GuacDNS = null;
                            ml.Username = "cloudswyft";
                            ml.Password = Encrypt(Password);
                            ml.IsDeleted = 0;

                            _db.MachineLabs.Add(ml);
                            _db.SaveChanges();


                            var data = new
                            {
                                VEProfileId = dataContents.VEProfileID,
                                UserId = item,
                                MachineSize = dataContents.MachineSize,
                                SchedBy = schedBy,
                                TenantId = tenantId,
                                VETypeID = veType.VETypeID
                            };

                            var dataMessage = JsonConvert.SerializeObject(data);

                            await Task.Run(() =>
                            {
                                clientFA.PostAsync(AWSProvision, new StringContent(dataMessage, Encoding.UTF8, "application/json"));

                                //ProvisionAWSWindows(dataContents.VEProfileID, item, dataContents.MachineSize, schedBy, tenantId, veType.VETypeID).ConfigureAwait(false);
                            });

                        }
                        else if (veType.VETypeID == 8) //AWS MAC OS
                        {
                            MachineLabs ml = new MachineLabs();

                            ml.DateProvision = DateTime.UtcNow.ToString();
                            ml.ResourceId = "No Instance YET";
                            ml.VEProfileId = dataContents.VEProfileID;
                            ml.UserId = item;
                            ml.MachineStatus = "Provisioning";
                            ml.MachineName = "UI" + item.ToString() + "VE" + dataContents.VEProfileID;
                            ml.IsStarted = 4;
                            ml.ScheduledBy = schedBy;
                            ml.RunningBy = 0;
                            ml.FQDN = null;
                            ml.GuacDNS = null;
                            ml.Username = "cloudswyft";
                            ml.Password = Encrypt(Password);
                            ml.IsDeleted = 0;

                            _db.MachineLabs.Add(ml);
                            _db.SaveChanges();

                            await Task.Run(() =>
                            {
                                ProvisionMacOS(dataContents.VEProfileID, item, dataContents.MachineSize, schedBy, tenantId, veType.VETypeID).ConfigureAwait(true);
                            });

                        }
                        else if (veType.VETypeID == 10) // GCP
                        {
                            MachineLabs ml = new MachineLabs();

                            ml.DateProvision = DateTime.UtcNow.ToString();
                            ml.ResourceId = "No GCP ID YET";
                            ml.VEProfileId = dataContents.VEProfileID;
                            ml.UserId = item;
                            ml.MachineStatus = "Provisioning";
                            ml.MachineName = "UI" + item.ToString() + "VE" + dataContents.VEProfileID;
                            ml.IsStarted = 4;
                            ml.ScheduledBy = schedBy;
                            ml.RunningBy = 0;
                            ml.FQDN = null;
                            ml.GuacDNS = null;
                            ml.IsDeleted = 0;
                            ml.VMName = groupPrefix.ToUpper() + "-" +  GenerateVMName().ToUpper();


                            _db.MachineLabs.Add(ml);
                            _db.SaveChanges();

                            var message = new
                            {
                                instance_name = ml.VMName.ToLower(), // vmname
                                zone = tenant.RegionGCP, //region
                                region = tenant.RegionGCP.Substring(0, tenant.RegionGCP.Length - 2), 
                                image_project = veImages.ProjectFamily, // project family
                                image_os = veImages.ImageFamily, //image name
                                disk_size_gb = diskSize.ToString(), //data size
                                machine_type = dataContents.MachineSize, // size
                                project_id = tenant.ProjectName.ToLower(),
                                network = tenant.VPCNetworkGCP.ToLower(),
                                subnet = tenant.VPCSubNetworkGCP.ToLower(),
                                username = GenerateUserNameRandomName()
                            };

                            var dataPayload = JsonConvert.SerializeObject(message);

                            await Task.Run(() =>
                            {
                                var dataGCP = clientGCP.PostAsync("api/gcp/virtual-machine/", new StringContent(dataPayload, Encoding.UTF8, "application/json"));
                        
                                var dataMessageGCP = JsonConvert.DeserializeObject<VMPayload>(dataGCP.Result.Content.ReadAsStringAsync().Result);

                                var mlData = _db.MachineLabs.Where(q => q.VMName.ToLower() == dataMessageGCP.data.instance_name.ToLower()).FirstOrDefault();
                                mlData.ResourceId = dataMessageGCP.data.instance_id;
                                mlData.Username = dataMessageGCP.data.user;
                                mlData.FQDN = dataMessageGCP.data.nat_i_p;

                                _db.Entry(mlData).State = EntityState.Modified;

                                mls.ResourceId = dataMessageGCP.data.instance_id;
                                mls.LastStatus = "Provisioning";
                                mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
                                mls.ModifiedDate = dateUtc;

                                _db.MachineLogs.Add(mls);
                                _db.SaveChanges();
                            });
                        }
                        else if (veType.VETypeID == 11) //AWS Console
                        {
                            MachineLabs ml = new MachineLabs();

                            ml.DateProvision = DateTime.UtcNow.ToString();
                            ml.ResourceId = "No Instance YET";
                            ml.VEProfileId = dataContents.VEProfileID;
                            ml.UserId = item;
                            ml.MachineStatus = "Provisioning";
                            ml.MachineName = "UI" + item.ToString() + "VE" + dataContents.VEProfileID;
                            ml.IsStarted = 4;
                            ml.ScheduledBy = schedBy;
                            ml.RunningBy = 0;
                            ml.FQDN = null;
                            ml.GuacDNS = null;
                            ml.Username = "null";
                            ml.Password = "null";
                            ml.IsDeleted = 0;

                            _db.MachineLabs.Add(ml);
                            _db.SaveChanges();

                            await Task.Run(() =>
                            {
                                ProvisionAWSConsole(dataContents.VEProfileID, item, schedBy, tenantId, veType.VETypeID).ConfigureAwait(true);
                            });
                        }
                        if (!isCourseGrantExist)
                        {
                            var grantedBy = _db.CloudLabUsers.Where(q => q.Email == schedBy).FirstOrDefault().UserId;

                            cg.UserID = item;
                            cg.VEProfileID = dataContents.VEProfileID;
                            cg.IsCourseGranted = true;
                            cg.VEType = veType.VETypeID;
                            cg.GrantedBy = grantedBy;
                            _db.CourseGrants.Add(cg);
                        }
                        else
                        {
                            courseGrant.IsCourseGranted = true;
                            _db.Entry(courseGrant).State = EntityState.Modified;
                            _db.SaveChanges();
                        }

                        if (!selfProv)
                        {
                            veprofileMappings.TotalRemainingCourseHours -= veprofileMappings.CourseHours;
                            _db.Entry(veprofileMappings).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    } // provision
                    else 
                    {
                        var isVMFailed = _db.MachineLabs.Any(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID && q.IsStarted == 3);

                        if (isVMFailed) //re-provision
                        {
                            if (veType.VETypeID == 3 || veType.VETypeID == 4)
                            {
                                var ml = _db.MachineLabs.Where(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID).FirstOrDefault();
                                var mlogs = _db.MachineLogs.Where(q => q.ResourceId == ml.ResourceId).FirstOrDefault();

                                ml.IsStarted = 6;
                                ml.MachineStatus = "Deleting";
                                _db.Entry(ml).State = EntityState.Modified;
                                _db.SaveChanges();

                                mlogs.LastStatus = "Deleting";
                                mlogs.ModifiedDate = DateTime.UtcNow;
                                mlogs.Logs = "(Deleting)" + DateTime.UtcNow + "---" + mlogs.Logs;
                                _db.Entry(mlogs).State = EntityState.Modified;
                                _db.SaveChanges();

                                var dataV = new VMDeleteDetails
                                {
                                    UserId = item,
                                    VEProfileId = dataContents.VEProfileID,
                                    SubscriptionId = tenant.SubscriptionId,
                                    ApplicationSecret = tenant.ApplicationSecretKey,
                                    ApplicationId = tenant.ApplicationId,
                                    ClientCode = tenant.ClientCode,
                                    TenantId = tenant.ApplicationTenantId,
                                    VirtualMachines = ml.VMName,
                                    DeletedBy = schedBy,
                                    NewImageName = newTemp.NewImageName
                                };

                                var dataMessage = JsonConvert.SerializeObject(dataV);

                                await Task.Run(() =>
                                {
                                    clientFA.PostAsync(ReProvisionMachine, new StringContent(dataMessage, Encoding.UTF8, "application/json"));
                                });
                            }
                            else if(veType.VETypeID == 1 || veType.VETypeID == 2)
                            {
                                var ml = _db.MachineLabs.Where(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID).FirstOrDefault();
                                var mlogs = _db.MachineLogs.Where(q => q.ResourceId == ml.ResourceId).FirstOrDefault();

                                ml.IsStarted = 6;
                                ml.MachineStatus = "Deleting";
                                _db.Entry(ml).State = EntityState.Modified;
                                _db.SaveChanges();

                                mlogs.LastStatus = "Deleting";
                                mlogs.ModifiedDate = DateTime.UtcNow;
                                mlogs.Logs = "(Deleting)" + DateTime.UtcNow + "---" + mlogs.Logs;
                                _db.Entry(mlogs).State = EntityState.Modified;
                                _db.SaveChanges();

                                var dataV = new VMDeleteDetails
                                {
                                    UserId = item,
                                    VEProfileId = dataContents.VEProfileID,
                                    SubscriptionId = tenant.SubscriptionId,
                                    ApplicationSecret = tenant.ApplicationSecretKey,
                                    ApplicationId = tenant.ApplicationId,
                                    ClientCode = tenant.ClientCode,
                                    TenantId = tenant.ApplicationTenantId,
                                    VirtualMachines = ml.VMName,
                                    DeletedBy = schedBy,
                                };

                                var dataMessage = JsonConvert.SerializeObject(dataV);

                                await Task.Run(() =>
                                {
                                    clientFA.PostAsync(ReProvisionMachine, new StringContent(dataMessage, Encoding.UTF8, "application/json"));
                                });
                            }
                            
                        }
                        else //extend hours
                        {
                            var ml = _db.MachineLabs.Where(q => q.UserId == item && q.VEProfileId == dataContents.VEProfileID).FirstOrDefault();
                            var cls = _db.CloudLabsSchedule.Where(q =>q.MachineLabsId == ml.MachineLabsId).FirstOrDefault();
                            var vemap = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == dataContents.VEProfileID).FirstOrDefault();

                            vemap.TotalRemainingCourseHours -= dataContents.CourseHours;
                            cls.TimeRemaining += TimeSpan.FromHours(dataContents.CourseHours).TotalSeconds;
                            cls.LabHoursTotal += dataContents.CourseHours;
                            ml.DateProvision = DateTime.UtcNow.ToString();

                            _db.Entry(ml).State = EntityState.Modified;
                            _db.Entry(cls).State = EntityState.Modified;
                            _db.Entry(vemap).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    } //reprovision or extend
                }
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("ReProvisionVM")]
        public async Task<IHttpActionResult> ReProvisionVM(int userId, int veProfileId, int tenantId)
        {
            try
            {
                var tenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault();
                var ml = _db.MachineLabs.Where(q => q.UserId == userId && q.VEProfileId == veProfileId).FirstOrDefault();
                var userEmail = _db.CloudLabUsers.Where(q => q.UserId == userId).FirstOrDefault().Email;
                var mlogs = _db.MachineLogs.Where(q => q.ResourceId == ml.ResourceId).FirstOrDefault();

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(FunctionAppUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;


                ml.IsStarted = 6;
                ml.MachineStatus = "Deleting";
                _db.Entry(ml).State = EntityState.Modified;
                _db.SaveChanges();

                mlogs.LastStatus = "Deleting";
                mlogs.ModifiedDate = DateTime.UtcNow;
                mlogs.Logs = "(Deleting)" + DateTime.UtcNow + "---" + mlogs.Logs;
                _db.Entry(mlogs).State = EntityState.Modified;
                _db.SaveChanges();

                var dataV = new VMDeleteDetails
                {
                    UserId = userId,
                    VEProfileId = veProfileId,
                    SubscriptionId = tenant.SubscriptionId,
                    ApplicationSecret = tenant.ApplicationSecretKey,
                    ApplicationId = tenant.ApplicationId,
                    ClientCode = tenant.ClientCode,
                    TenantId = tenant.ApplicationTenantId,
                    VirtualMachines = ml.VMName,
                    DeletedBy = userEmail,
                };

                var dataMessage = JsonConvert.SerializeObject(dataV);

                await Task.Run(() =>
                {
                    client.PostAsync(ReProvisionMachine, new StringContent(dataMessage, Encoding.UTF8, "application/json"));
                });

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        [Route("BulkProvisionVM")]
        public async Task<IHttpActionResult> BulkProvisionVM(int veprofileId, string schedBy)
        {
            try
            {
                HttpClient clientAWS = new HttpClient();
                clientAWS.BaseAddress = new Uri(FunctionAppUrl);
                clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpClient clientFA = new HttpClient();
                clientFA.BaseAddress = new Uri(ProvisionVMAzure);
                clientFA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                VHDDetails newTemp = new VHDDetails();
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                List<Bulk> studentList = new List<Bulk>();
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
                                Bulk student = new Bulk();
                                student.Email = item.ItemArray[0].ToString();
                                studentList.Add(student);
                            }

                            foreach (var stud in studentList)
                            {
                                var veDetails = _db.VEProfiles.Where(q => q.VEProfileID == veprofileId).Join(_db.VEProfileLabCreditMappings, a => a.VEProfileID, b => b.VEProfileID, (a, b) => new { a, b })
                                    .Select(p => new { VEProfileId = p.a.VEProfileID, MachineSize = p.b.MachineSize }).FirstOrDefault();

                                var userDetails = _db.CloudLabUsers.Where(q => q.Email == stud.Email).FirstOrDefault();

                                var isSameTenant = _db.CloudLabUsers.Where(q => q.Email == schedBy).Any(w => w.TenantId == userDetails.TenantId);

                                if (isSameTenant)
                                {
                                    var VMvhdUrl = _db.VEProfiles.Where(q => q.VEProfileID == veDetails.VEProfileId).Select(w => new { w.VirtualEnvironmentID, w.VEProfileID }).Join(_db.VirtualEnvironmentImages,
                                        a => a.VirtualEnvironmentID,
                                        b => b.VirtualEnvironmentID,
                                        (a, b) => new { a, b }).FirstOrDefault().b.Name;

                                    var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == userDetails.TenantId).FirstOrDefault();
                                    var userGroup = _db.CloudLabsGroups.Where(q => q.TenantId == userDetails.TenantId).FirstOrDefault();
                                    var groupPrefix = _db.CloudLabsGroups.Where(q => q.TenantId == userDetails.TenantId).FirstOrDefault().CLPrefix;

                                    var Environment = tenant.EnvironmentCode.Trim() == "D" ? "DEV" : tenant.EnvironmentCode.Trim() == "Q" ? "QA" : tenant.EnvironmentCode.Trim() == "U" ? "DMO" : "PRD";

                                    var veType = _db.VEProfiles.Where(q => q.VEProfileID == veDetails.VEProfileId).Join(_db.VirtualEnvironments,
                                        a => a.VirtualEnvironmentID,
                                        b => b.VirtualEnvironmentID,
                                        (a, b) => new { a, b }).Join(_db.VETypes, c => c.b.VETypeID, d => d.VETypeID, (c, d) => new { c, d }).Select(q => q.d).FirstOrDefault();

                                    MachineLabs vmm = new MachineLabs();
                                    CourseGrants cg = new CourseGrants();

                                    string computerName = "U" + userDetails.UserId + "V" + veDetails.VEProfileId;
                                    string ResourceId = Guid.NewGuid().ToString();
                                    string username = GenerateUserNameRandomName();
                                    string password = GeneratePasswordRandomName();
                                    string VMName = "CS-" + userGroup.CLPrefix + "-" + tenant.EnvironmentCode.Trim() + "-VM-" + "U" + userDetails.UserId + "V" + veDetails.VEProfileId + "-" + ResourceId;


                                    var isVMExist = _db.MachineLabs.Any(q => q.UserId == userDetails.UserId && q.VEProfileId == veDetails.VEProfileId);
                                    MachineLogs mls = new MachineLogs();
                                    DateTime dateUtc = DateTime.UtcNow;

                                    var isCourseGrantExist = _db.CourseGrants.Any(q => q.UserID == userDetails.UserId & q.VEProfileID == veDetails.VEProfileId);
                                    var courseGrant = _db.CourseGrants.Where(q => q.UserID == userDetails.UserId & q.VEProfileID == veDetails.VEProfileId).FirstOrDefault();
                                    var veprofileMappings = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == veDetails.VEProfileId && q.GroupID == userGroup.CloudLabsGroupID).FirstOrDefault();

                                    if (!isVMExist)
                                    {
                                        Thread.Sleep(4000);
                                        if (veType.VETypeID <= 2) //windows or linux azure 
                                        {
                                            var storageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https://") + 8, VMvhdUrl.IndexOf(".") - VMvhdUrl.IndexOf("https://") - 8);

                                            object dataV = new object();
                                            object qCustom = new object();

                                            if (userGroup.CLPrefix.Length == 5)// version 2.2
                                            {
                                                dataV = new
                                                {
                                                    SubscriptionId = tenant.SubscriptionId,
                                                    TenantId = tenant.ApplicationTenantId,
                                                    ApplicationKey = tenant.ApplicationSecretKey,
                                                    ApplicationId = tenant.ApplicationId,
                                                    Location = tenant.Regions,
                                                    Environment = tenant.EnvironmentCode.Trim(),
                                                    ClientCode = userGroup.CLPrefix,
                                                    VirtualMachineName = VMName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                                    Size = veDetails.MachineSize,
                                                    IsCustomTemplate = false,
                                                    TempStorageSizeInGb = 127,
                                                    ImageUri = VMvhdUrl,
                                                    ContactPerson = schedBy,
                                                    StorageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https", 0) + 8, VMvhdUrl.IndexOf(".blob", (VMvhdUrl.IndexOf("https", 0) + 8)) - (VMvhdUrl.IndexOf("https", 0) + 8)),
                                                    OsType = veType.Description,
                                                    ComputerName = computerName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                                    Username = username,
                                                    Password = password,
                                                    Fqdn = userGroup.CLUrl.Substring(8, userGroup.CLUrl.Length - 8),// CLMP URL WITHOUT HTTPS userGroup.CLUrl.Split(("https://")[1].ToLower(),
                                                    apiprefix = userGroup.ApiPrefix,
                                                    uniqueId = ResourceId
                                                };
                                            }
                                            else if (userGroup.CLPrefix.Length == 3)
                                            {
                                                dataV = new
                                                {
                                                    SubscriptionId = tenant.SubscriptionId,
                                                    TenantId = tenant.ApplicationTenantId,
                                                    ApplicationKey = tenant.ApplicationSecretKey,
                                                    ApplicationId = tenant.ApplicationId,
                                                    Location = tenant.Regions,
                                                    Environment = Environment,
                                                    ClientCode = userGroup.CLPrefix,
                                                    VirtualMachineName = VMName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                                    Size = veDetails.MachineSize,
                                                    IsCustomTemplate = false,
                                                    TempStorageSizeInGb = 127,
                                                    ImageUri = VMvhdUrl,
                                                    ContactPerson = schedBy,
                                                    StorageAccountName = VMvhdUrl.Substring(VMvhdUrl.IndexOf("https", 0) + 8, VMvhdUrl.IndexOf(".blob", (VMvhdUrl.IndexOf("https", 0) + 8)) - (VMvhdUrl.IndexOf("https", 0) + 8)),
                                                    OsType = veType.Description,
                                                    ComputerName = computerName,//"cs-PATTN-d-vm-U3V2-1254c6667524480ead0e5491dd6a17a8",
                                                    Username = username,
                                                    Password = password,
                                                    Fqdn = userGroup.CLUrl.Substring(8, userGroup.CLUrl.Length - 8),// CLMP URL WITHOUT HTTPS userGroup.CLUrl.Split(("https://")[1].ToLower(),
                                                    apiprefix = userGroup.ApiPrefix,
                                                    uniqueId = ResourceId,
                                                    ResourceGroupName = "CS-" + Environment + "-" + userGroup.CLPrefix
                                                };
                                            }


                                            var dataMessage = JsonConvert.SerializeObject(dataV);

                                            await Task.Run(() =>
                                            {
                                                clientFA.PostAsync("", new StringContent(dataMessage, Encoding.UTF8, "application/json"));
                                            });

                                            vmm.DateProvision = dateUtc.ToString();

                                            vmm.VMName = VMName;
                                            vmm.MachineName = computerName;
                                            vmm.ResourceId = ResourceId;
                                            vmm.UserId = userDetails.UserId;
                                            vmm.Username = username;
                                            vmm.Password = Encrypt(password);
                                            vmm.VEProfileId = veDetails.VEProfileId;
                                            vmm.IsStarted = 4; // provisioning 
                                            vmm.IsDeleted = 0;
                                            vmm.MachineStatus = "Provisioning";
                                            vmm.ScheduledBy = schedBy;

                                            mls.ResourceId = ResourceId;
                                            mls.LastStatus = "Provisioning";
                                            mls.Logs = '(' + mls.LastStatus + ')' + dateUtc;
                                            mls.ModifiedDate = dateUtc;

                                            _db.MachineLogs.Add(mls);
                                            _db.MachineLabs.Add(vmm);
                                            _db.SaveChanges();

                                            VirtualMachineDetails vmDetails = new VirtualMachineDetails();
                                            vmDetails.ResourceId = vmm.ResourceId;
                                            vmDetails.Status = vmm.IsStarted;
                                            vmDetails.VMName = VMName;
                                            vmDetails.FQDN = VMName + "." + tenant.Regions + ".cloudapp.azure.com";
                                            vmDetails.DateLastModified = DateTime.UtcNow;
                                            vmDetails.DateCreated = DateTime.UtcNow;
                                            vmDetails.OperationId = $"virtual-machine-{vmm.ResourceId}";

                                            _dbCustomer.VirtualMachineDetails.Add(vmDetails);
                                            _dbCustomer.SaveChanges();
                                        }
                                        else if (veType.VETypeID == 9)
                                        {
                                            MachineLabs ml = new MachineLabs();

                                            ml.DateProvision = DateTime.UtcNow.ToString();
                                            ml.ResourceId = "No Instance";
                                            ml.VEProfileId = veDetails.VEProfileId;
                                            ml.UserId = userDetails.UserId;
                                            ml.MachineStatus = "Provisioning";
                                            ml.MachineName = "UI" + userDetails.UserId.ToString() + "VE" + veDetails.VEProfileId;
                                            ml.IsStarted = 4;
                                            ml.ScheduledBy = schedBy;
                                            ml.RunningBy = 0;
                                            ml.FQDN = null;
                                            ml.GuacDNS = null;
                                            ml.Username = "cloudswyft";
                                            ml.Password = Encrypt(Password);
                                            ml.IsDeleted = 0;

                                            _db.MachineLabs.Add(ml);
                                            _db.SaveChanges();

                                            var data = new
                                            {
                                                VEProfileId = veDetails.VEProfileId,
                                                UserId = userDetails.UserId,
                                                MachineSize = veprofileMappings.MachineSize,
                                                SchedBy = schedBy,
                                                TenantId = tenant.TenantId,
                                                VETypeID = veType.VETypeID
                                            };

                                            var dataMessage = JsonConvert.SerializeObject(data);

                                            await Task.Run(() =>
                                            {
                                                clientAWS.PostAsync(AWSProvision, new StringContent(dataMessage, Encoding.UTF8, "application/json"));

                                            });
                                        } //aws windows

                                        if (!isCourseGrantExist)
                                        {
                                            var grantedBy = _db.CloudLabUsers.Where(q => q.Email == schedBy).FirstOrDefault().UserId;

                                            cg.UserID = userDetails.UserId;
                                            cg.VEProfileID = veDetails.VEProfileId;
                                            cg.IsCourseGranted = true;
                                            cg.VEType = veType.VETypeID;
                                            cg.GrantedBy = grantedBy;
                                            _db.CourseGrants.Add(cg);
                                        }
                                        else
                                        {
                                            courseGrant.IsCourseGranted = true;
                                            _db.Entry(courseGrant).State = EntityState.Modified;
                                            _db.SaveChanges();
                                        }

                                        veprofileMappings.TotalRemainingCourseHours -= veprofileMappings.CourseHours;
                                        _db.Entry(veprofileMappings).State = EntityState.Modified;
                                        _db.SaveChanges();


                                    } // provision
                                }

                            }

                        }
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
        public static string GenerateUserNameRandomName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GeneratePasswordRandomName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz!@";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 13)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result + "3!1"}";
        }
        public static string GenerateVMName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 15)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }

        [HttpGet]
        [Route("StartVMBulk")]
        public async Task<IHttpActionResult> StartVMBulk()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://csfavmprovisionwhi.azurewebsites.net/api/VMStart?code=TAjSyjH/JgdcDGjxYn1BAvaKd4TjnafVxPLKX8l3IfhfhV7sK8PkLA==");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            List<StartBulk> studentList = new List<StartBulk>();
            List<BulkProvision> finished = new List<BulkProvision>();

            using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/StartVM.csv"))
            {
                string[] headers = sr.ReadLine().Split(',');
                while (!sr.EndOfStream)
                {
                    Thread.Sleep(5000);
                    string[] rows = sr.ReadLine().Split(',');
                   
                    var dataV = new StartBulk
                    {
                        VMName = rows[0],
                        ResourceId = rows[1],
                        RunBy = 1
                    };
                    //studentList.Add(dataV);
                    var dataMsg = JsonConvert.SerializeObject(dataV);
                    await Task.Run(() =>
                    {
                        try
                        {
                            client.PostAsync("", new StringContent(dataMsg, Encoding.UTF8, "application/json"));

                        }
                        catch(Exception e)
                        {

                        }
                    });
                }
            }
          
            return Ok();

        }

        private async Task<string> ProvisionAWSConsole(int veprofileId, int userId, string schedBy, int tenantId, int veTypeId)
        {
            try
            {
                var email = _db.CloudLabUsers.Where(q => q.UserId == userId).FirstOrDefault().Email;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MacOS);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                MachineLabs ml = new MachineLabs();
                MachineLogs mls = new MachineLogs();
                CloudLabsSchedule cls = new CloudLabsSchedule();

                var createAccount = new
                {
                    organization_unit_id = "ou-c9j5-5jbdvx9h",
                    username = email,
                    email = email
                };


                var initializeAccount = new
                {
                    account_id = "662090352497",
                    student_username = "ken-test@cloudswyft.com",
                    student_email = "ken-test@cloudswyft.com",
                    stack_number = "12223123",
                    student_password = "F5VQeUi0",
                    budget_limit = "30",
                    data_transfer_out_limit = "30.0"
                };

                var createAccntString = JsonConvert.SerializeObject(createAccount);

                var responseCreate = await client.PostAsync("/api/users/aws/", new StringContent(createAccntString, Encoding.UTF8, "application/json"));

                var objCreate = JsonConvert.DeserializeObject<CreateMacInstance>(responseCreate.Content.ReadAsStringAsync().Result);

            }
            catch (Exception e)
            {

            }

            return "";
        }



      


    }
}