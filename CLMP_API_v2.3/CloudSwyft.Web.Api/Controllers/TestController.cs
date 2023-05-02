using CloudSwyft.Web.Api.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private VirtualEnvironmentDBTenantContext _dbTenant = new VirtualEnvironmentDBTenantContext();
        public static string ApiCall(string method, string url, string data = null)
        {
            try
            {
                var responseDetails = string.Empty;

                string operationsResponse = string.Empty;
                Uri requestUri = new Uri(url);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.Method = method;
                request.Accept = "application/json";
                request.ContentType = "application/x-www-form-urlencoded";

                if (method == "POST")
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    request.ContentLength = byteArray.Length;
                    Stream dataStream = request.GetRequestStream();
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    dataStream.Close();
                }

                WebResponse response = request.GetResponse();

                if (response.ContentLength > 0)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(responseStream);

                    responseDetails = reader.ReadToEnd();

                    responseStream.Close();
                    reader.Close();
                }

                return responseDetails;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [HttpGet]
        [Route("Test")]
        public string GetUserGrade()
        {

            TestController.ApiCall("POST", "http://localhost:7071/api/negotiate/", "message");

            return "Ok";
        }

        [HttpGet]
        [Route("BadReq")]
        public IHttpActionResult BadReq()
        {

          //  TestController.ApiCall("POST", "http://localhost:7071/api/negotiate/", "message");

            return BadRequest();
        }

        [HttpGet]
        [Route("OKay")]
        public IHttpActionResult OKay()
        {

            return Ok();
        }
        [HttpGet]
        [Route("Test2")]
        public string GetUserGrade2()
        {

            TestController.ApiCall("POST", "http://localhost:7071/api/messages/", "message");

            return "Ok";
        }



        [HttpGet]
        [Route("GetStudentImageGrade")]
        public async Task<IHttpActionResult> GetStudentImageGrade(string fullname, int userid, int veprofileid)
        {
            try
            {
                string name = fullname.Replace(" ", "");

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);

                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);
                var uri = string.Empty;

                List<CloudBlob> image = new List<CloudBlob>();

                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    image.Add(blob);
                    var s = blob.Properties.LastModified.Value;
                }

                var modified = image.OrderByDescending(x => x.Properties.LastModified.Value);
                var q = modified.FirstOrDefault().Uri.AbsoluteUri;
                return Ok(uri);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        [HttpGet]
        [Route("Encrypt")]
        public string Encrypt(string clearText)
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
        [Route("Decrypt")]
        public string Decrypt(string cipherText)
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


        //[HttpGet]
        //[Route("TestTest")]
        //public async Task<string> TestTest()
        //{
        //    var tenantId = 2;
        //    var AzureVM = "https://api.cloudswyft.com/";
        //    DateTime dateUtc = DateTime.UtcNow;


        //    try
        //    {
        //        var machineLabs = _db.MachineLabs.Where(q => q.IsStarted == 4)
        //            .Join(_db.VEProfiles,
        //            a => a.VEProfileId,
        //            b => b.VEProfileID,
        //            (a, b) => new { a, b })
        //            .Join(_db.VirtualEnvironments,
        //            c => c.b.VirtualEnvironmentID,
        //            d => d.VirtualEnvironmentID,
        //            (c, d) => new { c, d }).Select(w => new { w.c.a.ResourceId, w.c.a.VEProfileId, w.d.VETypeID, w.c.a.UserId }).ToList();

        //        var tenant = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).Select(q => new { q.TenantKey, q.SubscriptionKey, q.GuacamoleURL, q.GuacConnection, q.EnvironmentCode }).FirstOrDefault();

        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri(AzureVM);
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
        //        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);
        //        HttpResponseMessage response = null;

        //        VMSuccess dataJson = new VMSuccess();

        //        foreach (var item in machineLabs)
        //        {
        //            var vm = _db.MachineLabs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();
        //            var mls = _db.MachineLogs.Where(q => q.ResourceId == item.ResourceId).FirstOrDefault();

        //            response = await client.GetAsync("labs/virtualmachine/?resourceid=" + item.ResourceId);

        //            if (item.VETypeID <= 4) // azure windows, linux, baseline windows, linux
        //            {
        //                if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId))
        //                {
        //                    if (response.Content.ReadAsStringAsync().Result.Contains("virtualMachineName"))
        //                    {
        //                        dataJson = JsonConvert.DeserializeObject<VMSuccess>(response.Content.ReadAsStringAsync().Result);

        //                        vm.MachineName = dataJson.ComputerName == null ? "NO Computer NAME" : dataJson.ComputerName;
        //                        vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;

        //                        _db.Entry(vm).State = EntityState.Modified;
        //                        _db.SaveChanges();

        //                        if (dataJson.ProvisioningStatus == "Aborted" || response.Content.ReadAsStringAsync().Result.Contains("failure") || response.Content.ReadAsStringAsync().Result.Contains("Failed")) //means failed
        //                        {
        //                            vm.MachineStatus = "Failed";
        //                            vm.VMName = dataJson.VirtualMachineName == null ? "NO Computer NAME" : dataJson.VirtualMachineName;
        //                            vm.IsStarted = 3;
        //                            mls.ModifiedDate = dateUtc;
        //                            _db.Entry(mls).State = EntityState.Modified;
        //                            _db.Entry(vm).State = EntityState.Modified;
        //                            _db.SaveChanges();
        //                        }

        //                        else //success 
        //                        {
        //                            if (dataJson.IsReadyForUsage)
        //                            {
        //                                CloudLabsSchedule cls = new CloudLabsSchedule();
        //                                var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == item.VEProfileId && (_db.CloudLabUsers.Where(s => s.UserId == item.UserId).FirstOrDefault().UserGroup) == q.GroupID).FirstOrDefault();

        //                                var ve = item.VETypeID;

        //                                var guacURL = AddMachineToDatabase(vm.MachineName, tenant.GuacamoleURL, tenant.GuacConnection, dataJson.Username, dataJson.Password, item.VETypeID, tenant.EnvironmentCode, dataJson.Fqdn);
        //                                if (guacURL != "")
        //                                {
        //                                    vm.GuacDNS = guacURL;
        //                                    vm.DateProvision = dateUtc.ToString();
        //                                    vm.Username = dataJson.Username;
        //                                    vm.Password = Encrypt(dataJson.Password);
        //                                    vm.FQDN = dataJson.Fqdn;
        //                                    vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;

        //                                    cls.LabHoursTotal = labCredit.CourseHours;
        //                                    cls.InstructorLabHours = 120;

        //                                    _db.Entry(vm).State = EntityState.Modified;
        //                                    _db.SaveChanges();

        //                                    mls.LastStatus = "Provisioned";
        //                                    mls.Logs = '(' + mls.LastStatus + ')' + dateUtc + "---" + mls.Logs;
        //                                    mls.ModifiedDate = dateUtc;

        //                                    cls.VEProfileID = vm.VEProfileId;
        //                                    cls.UserId = vm.UserId;
        //                                    cls.MachineLabsId = vm.MachineLabsId;
        //                                    cls.TimeRemaining = TimeSpan.FromHours(labCredit.CourseHours / 60).TotalSeconds;
        //                                    cls.LabHoursTotal = labCredit.CourseHours;
        //                                    cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;

        //                                    var isCLSExist = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == vm.MachineLabsId);

        //                                    _db.SaveChanges();
        //                                    if (!isCLSExist)
        //                                    {
        //                                        _db.Entry(mls).State = EntityState.Modified;
        //                                        _db.CloudLabsSchedule.Add(cls);
        //                                        _db.SaveChanges();
        //                                    }

        //                                }
        //                                else
        //                                {
        //                                    var guacamoleURL = EditMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, tenant.EnvironmentCode, dataJson.Fqdn, dataJson.Password, dataJson.Username);
        //                                    vm.GuacDNS = guacamoleURL;
        //                                    vm.DateProvision = dateUtc.ToString();
        //                                    vm.Username = dataJson.Username;
        //                                    vm.Password = Encrypt(dataJson.Password);
        //                                    vm.FQDN = dataJson.Fqdn;
        //                                    vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;

        //                                    cls.LabHoursTotal = labCredit.CourseHours;
        //                                    cls.InstructorLabHours = 120;

        //                                    _db.Entry(vm).State = EntityState.Modified;
        //                                    _db.SaveChanges();

        //                                    mls.LastStatus = "Provisioned";
        //                                    mls.Logs = '(' + mls.LastStatus + ')' + dateUtc + "---" + mls.Logs;
        //                                    mls.ModifiedDate = dateUtc;

        //                                    cls.VEProfileID = vm.VEProfileId;
        //                                    cls.UserId = vm.UserId;
        //                                    cls.MachineLabsId = vm.MachineLabsId;
        //                                    cls.TimeRemaining = TimeSpan.FromHours(labCredit.CourseHours / 60).TotalSeconds;
        //                                    cls.LabHoursTotal = labCredit.CourseHours;
        //                                    cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;

        //                                    var isCLSExist = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == vm.MachineLabsId);

        //                                    _db.SaveChanges();
        //                                    if (!isCLSExist)
        //                                    {
        //                                        _db.Entry(mls).State = EntityState.Modified;
        //                                        _db.CloudLabsSchedule.Add(cls);
        //                                        _db.SaveChanges();
        //                                    }
        //                                }

        //                                if (dataJson.Status.ToLower() == "deallocated")
        //                                {
        //                                    mls.Logs = '(' + dataJson.Status + ')' + dateUtc + "---" + mls.Logs;
        //                                    mls.LastStatus = "Deallocated";
        //                                    mls.ModifiedDate = dateUtc;

        //                                    vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;
        //                                    vm.MachineStatus = "Deallocated";
        //                                    vm.IsStarted = 0;
        //                                    _db.Entry(vm).State = EntityState.Modified;
        //                                    _db.Entry(mls).State = EntityState.Modified;
        //                                    _db.SaveChanges();
        //                                }

        //                                if (dataJson.Status.ToLower() == "deallocating")
        //                                {
        //                                    mls.Logs = "(Deallocating)" + dateUtc + "---" + mls.Logs;
        //                                    mls.LastStatus = "Deallocating";
        //                                    mls.ModifiedDate = dateUtc;

        //                                    vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;
        //                                    vm.MachineStatus = "Deallocating";
        //                                    vm.IsStarted = 0;
        //                                    _db.Entry(vm).State = EntityState.Modified;
        //                                    _db.Entry(mls).State = EntityState.Modified;
        //                                    _db.SaveChanges();

        //                                }

        //                                if (dataJson.Status.ToLower() == "running" && (mls.ModifiedDate.Value.AddMinutes(2) < dateUtc))
        //                                {
        //                                    mls.Logs = "(Running)" + dateUtc + "---" + mls.Logs;
        //                                    mls.LastStatus = "Running";
        //                                    mls.ModifiedDate = dateUtc;

        //                                    vm.MachineStatus = "Running";
        //                                    vm.IsStarted = 1;
        //                                    _db.Entry(vm).State = EntityState.Modified;
        //                                    _db.Entry(mls).State = EntityState.Modified;
        //                                    _db.SaveChanges();

        //                                }
        //                            }

        //                        }
        //                    }
        //                    else
        //                    {
        //                        var responseJson = JsonConvert.DeserializeObject<VMStats>(response.Content.ReadAsStringAsync().Result);
        //                        if (responseJson.ProvisioningStatus == "Aborted" || response.Content.ReadAsStringAsync().Result.Contains("failure") || response.Content.ReadAsStringAsync().Result.Contains("Failed"))
        //                        {
        //                            vm.VMName = responseJson.VirtualMachineName == null ? "NO Computer NAME" : responseJson.VirtualMachineName;
        //                            vm.MachineStatus = "Failed";
        //                            vm.IsStarted = 3;
        //                            mls.ModifiedDate = dateUtc;
        //                        }
        //                        else if (response.Content.ReadAsStringAsync().Result.Contains("could not be found"))
        //                        {
        //                            vm.VMName = "Not Found";
        //                            vm.MachineStatus = "Failed";
        //                            vm.IsStarted = 3;
        //                            mls.ModifiedDate = dateUtc;
        //                        }
        //                        _db.Entry(vm).State = EntityState.Modified;
        //                        _db.SaveChanges();
        //                    }

        //                }

        //                if (_db.MachineLabs.Any(q => q.ResourceId == item.ResourceId && q.IsStarted == 3))
        //                {
        //                    if (response.Content.ReadAsStringAsync().Result.Contains("virtualMachineName"))
        //                    {
        //                        dataJson = JsonConvert.DeserializeObject<VMSuccess>(response.Content.ReadAsStringAsync().Result);

        //                        vm.MachineName = dataJson.ComputerName == null ? "NO Computer NAME" : dataJson.ComputerName;
        //                        vm.VMName = dataJson.VirtualMachineName == null ? "NO VM NAME" : dataJson.VirtualMachineName;

        //                        _db.Entry(vm).State = EntityState.Modified;
        //                        _db.SaveChanges();

        //                        if (dataJson.ProvisioningStatus == "Aborted" || response.Content.ReadAsStringAsync().Result.Contains("failure") || response.Content.ReadAsStringAsync().Result.Contains("Failed")) //means failed
        //                        {
        //                            vm.MachineStatus = "Failed";
        //                            vm.IsStarted = 3;
        //                            mls.ModifiedDate = dateUtc;
        //                            _db.Entry(mls).State = EntityState.Modified;
        //                            _db.Entry(vm).State = EntityState.Modified;
        //                            _db.SaveChanges();

        //                        }

        //                        else //success 
        //                        {

        //                            CloudLabsSchedule cls = new CloudLabsSchedule();
        //                            var labCredit = _db.VEProfileLabCreditMappings.Where(q => q.VEProfileID == item.VEProfileId && (_db.CloudLabUsers.Where(s => s.UserId == item.UserId).FirstOrDefault().UserGroup) == q.GroupID).FirstOrDefault();

        //                            var ve = item.VETypeID;

        //                            var guacURL = AddMachineToDatabase(vm.MachineName, tenant.GuacamoleURL, tenant.GuacConnection, dataJson.Username, dataJson.Password, item.VETypeID, tenant.EnvironmentCode, dataJson.Fqdn);
        //                            if (guacURL != "")
        //                            {
        //                                vm.MachineStatus = "Deallocating";
        //                                vm.IsStarted = 2;
        //                                vm.RunningBy = 0;

        //                                vm.GuacDNS = guacURL;
        //                                vm.DateProvision = dateUtc.ToString();
        //                                vm.Username = dataJson.Username;
        //                                vm.Password = Encrypt(dataJson.Password);
        //                                vm.FQDN = dataJson.Fqdn;

        //                                cls.LabHoursTotal = labCredit.CourseHours;
        //                                cls.InstructorLabHours = 120;

        //                                _db.Entry(vm).State = EntityState.Modified;
        //                                _db.SaveChanges();

        //                                mls.LastStatus = "Provisioned";
        //                                mls.Logs = '(' + mls.LastStatus + ')' + dateUtc + "---" + mls.Logs;
        //                                mls.ModifiedDate = dateUtc;

        //                                cls.VEProfileID = vm.VEProfileId;
        //                                cls.UserId = vm.UserId;
        //                                cls.MachineLabsId = vm.MachineLabsId;
        //                                cls.TimeRemaining = TimeSpan.FromHours(labCredit.CourseHours / 60).TotalSeconds;
        //                                cls.LabHoursTotal = labCredit.CourseHours;
        //                                cls.InstructorLabHours = TimeSpan.FromHours(2).TotalSeconds;

        //                                var isCLSExist = _db.CloudLabsSchedule.Any(q => q.MachineLabsId == vm.MachineLabsId);

        //                                _db.SaveChanges();
        //                                if (!isCLSExist)
        //                                {
        //                                    _db.Entry(mls).State = EntityState.Modified;
        //                                    _db.CloudLabsSchedule.Add(cls);
        //                                    _db.SaveChanges();
        //                                }

        //                            }
        //                        }

        //                    }

        //                }

        //            }
        //            else if (item.VETypeID == 9)
        //            {
        //                string Password = "c5w1N4W5c2";

        //                HttpClient clientAWS = new HttpClient();
        //                clientAWS.BaseAddress = new Uri("https://0h01s47ihg.execute-api.ap-southeast-1.amazonaws.com/");
        //                clientAWS.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                var jsonDetails = new
        //                {
        //                    instance_id = item.ResourceId,
        //                    region = "ap-southeast-1"
        //                };
        //                var jsonData = JsonConvert.SerializeObject(jsonDetails);

        //                var responseGetDetails = await clientAWS.PostAsync("dev/get_vm_details", new StringContent(jsonData, Encoding.UTF8, "application/json"));
        //                var getDetails = JObject.Parse(responseGetDetails.Content.ReadAsStringAsync().Result);

        //                if (getDetails.SelectToken("Reservations[0]") != null) {
        //                    var isRunning = getDetails.SelectToken("Reservations[0].Instances[0].State.Name").ToString();

        //                    if (isRunning == "running" && vm.IsStarted != 1)
        //                    {
        //                        var DNS = getDetails.SelectToken("Reservations[0].Instances[0].PublicDnsName").ToString();

        //                        if (vm.GuacDNS == null)
        //                        {
        //                            var guacUrl = AddMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, "cloudswyft", Password, item.VETypeID, tenant.EnvironmentCode, DNS);
        //                            vm.GuacDNS = guacUrl;
        //                            _db.SaveChanges();
        //                        }

        //                        if (vm.FQDN != DNS)
        //                        {
        //                            var guacurl = EditMachineToDatabase(vm.VMName, tenant.GuacamoleURL, tenant.GuacConnection, tenant.EnvironmentCode, DNS, Password, "cloudswyft");

        //                            vm.IsStarted = 1;
        //                            vm.MachineStatus = "Running";
        //                            vm.FQDN = DNS;
        //                            if (guacurl != "")
        //                                vm.GuacDNS = guacurl;
        //                            vm.RunningBy = 1;

        //                            mls.Logs = "(Running)" + dateUtc + "---" + mls.Logs;
        //                            mls.LastStatus = "Running";
        //                            mls.ModifiedDate = dateUtc;
        //                            _db.Entry(mls).State = EntityState.Modified;

        //                            _db.Entry(vm).State = EntityState.Modified;

        //                            _db.SaveChanges();
        //                        }

        //                    }

        //                    if (isRunning == "stopped" && vm.IsStarted != 0)
        //                    {

        //                        var v = _db.MachineLabs.Where(q => q.ResourceId == vm.ResourceId).FirstOrDefault();

        //                        HttpClient clientremove = new HttpClient();

        //                        clientremove.BaseAddress = new Uri("https://0h01s47ihg.execute-api.ap-southeast-1.amazonaws.com/");
        //                        clientremove.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //                        var data = new
        //                        {
        //                            instance_id = v.ResourceId,
        //                            ip_address = v.IpAddress,
        //                            region = "ap-southeast-1",
        //                            action_type = "REMOVE"
        //                        };

        //                        var dataRemove = JsonConvert.SerializeObject(data);
        //                        await clientremove.PostAsync("dev/update_security_group", new StringContent(dataRemove, Encoding.UTF8, "application/json"));

        //                        v.IsStarted = 0;
        //                        v.MachineStatus = "Deallocated";
        //                        v.MachineName = "UI" + item.UserId.ToString() + "VE" + v.VEProfileId;
        //                        v.RunningBy = 0;
        //                        v.IpAddress = null;
        //                        _db.Entry(v).State = EntityState.Modified;
        //                        _db.SaveChanges();
        //                        mls.Logs = "(Deallocated)" + dateUtc + "---" + mls.Logs;
        //                        mls.LastStatus = "Deallocated";
        //                        mls.ModifiedDate = dateUtc;
        //                        //_db.Entry(mls).State = EntityState.Modified;

        //                        //_db.Entry(vm).State = EntityState.Modified;

        //                        _db.SaveChanges();
        //                    }
        //                }
                        

        //            }
        //        }
        //        return "";
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        public static string AddMachineToDatabase(string machineName, string GuacamoleUrl, string guacCon, string vmusername, string vmpassword, int VETypeId, string environment, string fqdn)
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
                    return "";

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

                    guacDatabase.Close();
                    return "";
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

        [HttpGet]
        [Route("grant")]
        public string Grant()
        {
            var user = _db.MachineLabs.Join(_db.CloudLabsSchedule,
                a => a.MachineLabsId,
                b => b.MachineLabsId,
                (a, b) => new { a, b }).Select(q => new { userid = q.a.UserId, veid = q.a.VEProfileId }).ToList();

            var vetype = _db.VEProfiles.Join(_db.VirtualEnvironments,
                a => a.VirtualEnvironmentID,
                b => b.VirtualEnvironmentID,
                (a, b) => new { a, b }).Select(q => new { vetypeid = q.b.VETypeID }).FirstOrDefault();

            foreach (var item in user)
            {
                if (!_db.CourseGrants.Any(q => q.UserID == item.userid && q.VEProfileID == item.veid))
                {
                    CourseGrants cg = new CourseGrants();
                    cg.UserID = item.userid;
                    cg.VEProfileID = item.veid;
                    cg.VEType = vetype.vetypeid;
                    cg.IsCourseGranted = true;
                    _db.CourseGrants.Add(cg);
                    _db.SaveChanges();
                }
            }

            return "";
        }

    }


}
