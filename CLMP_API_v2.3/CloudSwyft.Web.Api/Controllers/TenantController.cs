using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data.Entity;
using System.Text;
using System.Configuration;
using System.Threading;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/Tenant")]
    public class TenantController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();
        private TenantDBContext _db = new TenantDBContext();
        private SqlConnection _connTenant = new SqlConnection(WebConfigurationManager.AppSettings["TenantURL"]);
        private string RunbookTenant = WebConfigurationManager.AppSettings["RunbookTenant"];
        private string FSDS = WebConfigurationManager.AppSettings["FSDS"];
        private string senderNoReply = WebConfigurationManager.AppSettings["senderNoReply"];
        private string senderDaily = WebConfigurationManager.AppSettings["senderDaily"];
        private string smtpUser = WebConfigurationManager.AppSettings["smtpUser"];
        private string smtpPass = WebConfigurationManager.AppSettings["smtpPass"];
        private string appServicePlan = WebConfigurationManager.AppSettings["appServicePlan"];
        private string appServicePlanRG = WebConfigurationManager.AppSettings["appServicePlanRG"];
        private string[] CC = WebConfigurationManager.AppSettings["CC"].Split(';');
        private string[] TO = WebConfigurationManager.AppSettings["TO"].Split(';');
        private string strcon = ConfigurationManager.ConnectionStrings["VirtualEnvironmentDbContext"].ConnectionString;

        [HttpGet]
        [Route("SubscriptionMinutes")]
        public HttpResponseMessage SubscriptionMinutes()
        {
            var sss = strcon.Split('=')[1].Split(';')[0];
            return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<int>("SELECT TOP 1 SubscriptionMinutes FROM dbo.TenantData").ToList()[0]);
        }

        [HttpGet]
        [Route("Name")]
        public HttpResponseMessage Name()
        {
            return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<string>("SELECT TOP 1 Name FROM dbo.TenantData").ToList()[0]);
        }

        [HttpGet]
        [Route("VMPrefix")]
        public HttpResponseMessage VMPrefix()
        {
            return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<string>("SELECT TOP 1 VMPrefix FROM dbo.TenantData").ToList()[0]);
        }

        [HttpGet]
        [Route("ConsumedMinutes")]
        public HttpResponseMessage ConsumedMinutes()
        {
            return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<int>("SELECT TOP 1 ConsumedMinutes FROM dbo.TenantData").ToList()[0]);
        }

        [HttpGet]
        [Route("SubscriptionUsers")]
        public HttpResponseMessage SubscriptionUsers()
        {
            return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<int>("SELECT TOP 1 SubscriptionUsers FROM dbo.TenantData").ToList()[0]);
        }

        [HttpGet]
        [Route("GetArcheTypes")]
        public HttpResponseMessage GetArcheTypes()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, db.Database.SqlQuery<string>("SELECT TOP 1 RoleArcheType FROM dbo.TenantData").ToList()[0]);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(e);
            }
              }

        //[HttpGet]
        //[Route("ConcurrentUsers")]
        //public async Task<HttpResponseMessage> ConcurrentUsers()
        //{
        //    List<VirtualMachine> virtualMachines = db.VirtualMachines.Where(vm => vm.IsStarted == 1).ToList();

        //    List<ConcurrentUser> concurrentUsers = new List<ConcurrentUser>();

        //    ConcurrentUser newConcurrentUser;
        //    VirtualMachineLog startLog;
        //    VirtualMachineLog latestLog;

        //    try
        //    {
        //        for (int x = 0; x < virtualMachines.Count; x++)
        //        {

        //            newConcurrentUser = new ConcurrentUser();

        //            UserController userController = new UserController();
        //            CourseController courseController = new CourseController();

        //            string getUserResponse = await ApiCall("GET", GenericStringsCollection.CSWebApiUrl,
        //                "api/User/UserByID?id=" + virtualMachines[x].UserID.ToString());

        //            User user = JsonConvert.DeserializeObject<User>(getUserResponse);

        //            newConcurrentUser.User = user;

        //            string getCourseResponse = await ApiCall("GET", GenericStringsCollection.CSWebApiUrl,
        //                "api/Course/Courses?id=" + virtualMachines[x].CourseID.ToString());

        //            List<Courses> courseList = JsonConvert.DeserializeObject<List<Courses>>(getCourseResponse);

        //            newConcurrentUser.Course = courseList.First();

        //            startLog = db.VirtualMachineLogs.ToList().Where(vl => vl.RoleName == virtualMachines[x].RoleName &&
        //                vl.Comment == "Starting").OrderBy(vl => vl.TimeStamp).First();
        //            latestLog = db.VirtualMachineLogs.ToList().Where(vl => vl.RoleName == virtualMachines[x].RoleName &&
        //                vl.TimeStamp > startLog.TimeStamp).Last();

        //            /*
        //             * startLog = db.VirtualMachineLogs.Where(vl => vl.RoleName.Equals(virtualMachines[x].RoleName) &&
        //                vl.Comment.Equals("Starting")).OrderBy(vl => vl.TimeStamp).ToList().First();
        //            latestLog = db.VirtualMachineLogs.Where(vl => vl.RoleName.Equals(virtualMachines[x].RoleName) &&
        //                vl.TimeStamp > startLog.TimeStamp).ToList().Last();
        //             */

        //            if ( startLog != null && latestLog != null)
        //            {
        //                TimeSpan differenceTimeSpan = latestLog.TimeStamp - startLog.TimeStamp;

        //                newConcurrentUser.SessionMinutes = differenceTimeSpan.Minutes;
        //            } else
        //            {
        //                newConcurrentUser.SessionMinutes = 0;
        //            }

        //            concurrentUsers.Add(newConcurrentUser);

        //        }

        //        return Request.CreateResponse(HttpStatusCode.OK, concurrentUsers);
        //    } catch
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, concurrentUsers);
        //    }
        //}

        private async Task<string> ApiCall(string method, string baseAddress, string url, string data = null)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseAddress);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpContent c = new StringContent(data, Encoding.UTF8, "application/json");
                HttpResponseMessage response = null;

                if (method == "POST")
                {
                    response = await client.PostAsync(client.BaseAddress + url, c);
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
            catch (Exception e)
            {
                return e.Message;
            }
        }

        [HttpGet]
        [Route("url")]
        public HttpResponseMessage GetApiUrl(string prefix)
        {
            string url = "";
            try
            {
            SqlConnection conn = new SqlConnection();
            
            conn.ConnectionString = WebConfigurationManager.AppSettings["TenantURL"];
            conn.Open();
            
            SqlCommand command = new SqlCommand("Select ApiUrl from Tenants where Code='" + prefix + "'", conn);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                        url = reader.GetValue(0).ToString();
                }
            }
                //Tenant tenant = db.Tenant.FirstOrDefault(tc => tc.Code == prefix);

                //string apiUrl = tenant.ApiUrl;

                return Request.CreateResponse(HttpStatusCode.OK, url);
            }
            catch(Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
        }

        [HttpPost]
        [Route("AddTenant")]
        public HttpResponseMessage AddTenant(Tenant TenantInfo)
        {
            var tenantData = _db.Tenants.Where(x => x.Code == TenantInfo.Code).ToList();

            Tenant tenant = new Tenant();
            _db.Tenants.Add(TenantInfo);
            _db.SaveChanges();

            //var sqlCommand = new SqlCommand(string.Format("INSERT INTO Tenants (ApiURL, Code, DbHost, DbName, DbUser, DbPass, SubscriptionMinutes, GuacConnection, GuacamoleURL, TenantName, AzurePort, SubscriptionId) VALUES('"
            //    + TenantInfo.ApiUrl + "','" + TenantInfo.Code + "','" + TenantInfo.DbHost + "','" + TenantInfo.DbName + "','" + TenantInfo.DbUser + "','" + TenantInfo.DbPass + "','" + TenantInfo.SubscriptionMinutes + "','" + TenantInfo.GuacConnection + "','" + TenantInfo.GuacamoleURL + "','" + TenantInfo.TenantName + "','" + TenantInfo.AzurePort + "','" + TenantInfo.SubscriptionId + "')"), _connTenant);
            //_connTenant.Open();
            //var command = sqlCommand.ExecuteReader();
            //_connTenant.Close();
            return Request.CreateResponse(HttpStatusCode.OK, "Naysu!!");
        }

        [HttpGet]
        [Route("GetTenant")]
        public HttpResponseMessage GetTenant(string code)
        {
            try
            {
                var hasRows = _db.Tenants.Any(x => x.Code == code);
                return Request.CreateResponse(HttpStatusCode.OK, hasRows);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            //List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            //Dictionary<string, string> column;

            //SqlCommand sqlCommand = new SqlCommand(string.Format("SELECT * from Tenants"), _connTenant);
            //_connTenant.Open();
            //SqlDataReader reader = sqlCommand.ExecuteReader();
            //while (reader.Read())
            //{
            //    column = new Dictionary<string, string>();
            //    column["TenantId"] = reader["TenantId"].ToString();
            //    column["ApiUrl"] = reader["ApiUrl"].ToString();
            //    column["Code"] = reader["Code"].ToString();
            //    //column["DbHost"] = reader["DbHost"].ToString();
            //    //column["DbName"] = reader["DbName"].ToString();
            //    //column["DbUser"] = reader["DbUser"].ToString();
            //    //column["DbPass"] = reader["DbPass"].ToString();
            //    column["SubscriptionMinutes"] = reader["SubscriptionMinutes"].ToString();
            //    column["GuacConnection"] = reader["GuacConnection"].ToString();
            //    column["GuacamoleURL"] = reader["GuacamoleURL"].ToString();
            //    column["TenantName"] = reader["TenantName"].ToString();
            //    column["AzurePort"] = reader["AzurePort"].ToString();
            //    column["SubscriptionId"] = reader["SubscriptionId"].ToString();
            //    rows.Add(column);
            //}
            //reader.Close();
            //_connTenant.Close();
        }

        [HttpPost]
        [Route("EditTenant")]
        public HttpResponseMessage EditTenant(Tenant TenantInfo)
        {
            //var sqlCommand = new SqlCommand(string.Format("UPDATE Tenants SET ApiUrl='" + TenantInfo.ApiUrl + "', Code='" + TenantInfo.Code + "', DbHost='" + TenantInfo.DbHost + "', DbName='" + TenantInfo.DbName +
            //    "',DbUser='" + TenantInfo.DbUser + "', DbPass='" + TenantInfo.DbPass + "', SubscriptionMinutes='" + TenantInfo.SubscriptionMinutes + "', GuacConnection='" + TenantInfo.GuacConnection + "', GuacamoleURL='" + TenantInfo.GuacamoleURL +
            //    "',AzurePort='" + TenantInfo.AzurePort + "', SubscriptionId='" + TenantInfo.SubscriptionId + "' WHERE TenantId='" + TenantInfo.TenantId + "'"), _connTenant);

            //_connTenant.Open();
            //var command = sqlCommand.ExecuteReader();
            //_connTenant.Close();
            return Request.CreateResponse(HttpStatusCode.OK, "Pasok Yehey!!");
        }

        [HttpPost]
        [Route("CreateTenant")]
        public async Task<HttpResponseMessage> CreateTenant(AzTenant TenantInfo)
        {
            try
            {
                string tenantInfo = JsonConvert.SerializeObject(TenantInfo);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                //var data = new TenantData
                //{
                //    ClientCode = TenantInfo.ClientCode,
                //    CreatedBy = TenantInfo.CreatedBy,
                //    //Location = TenantInfo.Location,
                //    Environment = TenantInfo.Environment
                //};
                // string data1 = JsonConvert.SerializeObject(data);

                // var s = await ApiCall("POST", "https://development-api.cloudswyft.com/", "internal/tenant", data1);
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();

                var clientKey = new string(
                    Enumerable.Repeat(chars, 16)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray());

                var clientSecret = new string(
                    Enumerable.Repeat(chars, 16)
                        .Select(s => s[random.Next(s.Length)])
                        .ToArray());

                var tenantData = new AzTenant
                {
                    ClientCode = TenantInfo.ClientCode.ToUpper(),
                    EnvironmentCode = TenantInfo.EnvironmentCode,
                    GuacConnection = TenantInfo.GuacConnection,
                    GuacamoleURL = TenantInfo.GuacamoleURL,
                    CreatedBy = TenantInfo.CreatedBy,
                    DateCreated = DateTime.UtcNow,
                    Regions = TenantInfo.Regions,
                    SubscriptionId = TenantInfo.SubscriptionId,
                    ApplicationId = TenantInfo.ApplicationId,
                    ApplicationSecretKey = TenantInfo.ApplicationSecretKey,
                    ApplicationTenantId = TenantInfo.ApplicationTenantId,
                    ClientKey = clientKey,
                    ClientSecret = clientSecret,
                    BusinessId = TenantInfo.BusinessId,
                    RegionGCP = TenantInfo.RegionGCP,
                    ProjectName = TenantInfo.ProjectName,
                    VPCNetworkGCP = TenantInfo.VPCNetworkGCP,
                    VPCSubNetworkGCP = TenantInfo.VPCSubNetworkGCP
                };
                

                if(TenantInfo.SubscriptionId != null)
                {
                    var data = new
                    {
                        clientCode = TenantInfo.ClientCode,
                        environment = TenantInfo.EnvironmentCode,
                        contactEmail = TenantInfo.CreatedBy,
                        location = TenantInfo.Regions,
                        subscriptionId = TenantInfo.SubscriptionId,
                        applicationId = TenantInfo.ApplicationId,
                        applicationKey = TenantInfo.ApplicationSecretKey,
                        tenantId = TenantInfo.ApplicationTenantId
                    };

                    var data2 = new
                    {
                        ApplicationSubscriptionId = TenantInfo.SubscriptionId,
                        ApplicationTenantId = TenantInfo.ApplicationTenantId,
                        ApplicationId = TenantInfo.ApplicationId,
                        ApplicationSecret = TenantInfo.ApplicationSecretKey,
                        ClientCode = TenantInfo.ClientCode,
                        ClientName = TenantInfo.ClientCode,
                        environment = TenantInfo.EnvironmentCode,
                        location = TenantInfo.Regions,
                        appServicePlanName = appServicePlan,
                        appServicePlanResourceGroupName = appServicePlanRG,
                        timezone = "Any",
                        CC = CC,
                        TO = TO,
                        FROM = senderNoReply,
                        FROM_DAILYREPORT_EMAIL = senderDaily,
                        LabsDataBase = strcon.Split('=')[2].Split(';')[0],
                        LabsDataServer = strcon.Split('=')[1].Split(';')[0],
                        LabsPassword = strcon.Split('=')[5].Split(';')[0],
                        LabsUserId = strcon.Split('=')[4].Split(';')[0],
                        SendGridKey = smtpPass,
                        SendGridName = smtpUser
                    };
                    var dataMsg = JsonConvert.SerializeObject(data);
                    var dataMsgFSDR = JsonConvert.SerializeObject(data2);

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(RunbookTenant);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpClient clientFSDR = new HttpClient();
                    clientFSDR.BaseAddress = new Uri(FSDS);
                    clientFSDR.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    await Task.Run(() => {
                        client.PostAsync("", new StringContent(dataMsg, Encoding.UTF8, "application/json"));

                        _db.AzTenants.Add(tenantData);
                        _db.SaveChanges();
                        Thread.Sleep(20000);

                    });

                    await Task.Run(() => {
                        clientFSDR.PostAsync("", new StringContent(dataMsgFSDR, Encoding.UTF8, "application/json"));
                    });
                }
                else
                {
                    _db.AzTenants.Add(tenantData);
                    _db.SaveChanges();
                }
                

                return Request.CreateResponse(HttpStatusCode.OK, "Ok!");

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
        }
        [HttpPost]
        [Route("UpdateTenant")]
        public HttpResponseMessage UpdateTenant(AzTenant tenants)
        {
            try
            {

                #region to access Azure Container Table
                //CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=csapacdevopsassetsdstg;AccountKey=IvcbX80exwAe3dVP86EzwOeEE4o+C1RRbyy2ifJFFpY2ni8j7QkQPlDRIRk2GAM28Gv8A/UYtpO61rA5lLVS5Q==;EndpointSuffix=core.windows.net");

                //CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

                //CloudTable table = tableClient.GetTableReference("Tenant");

                //TableQuery<AuthorEntity> rangeQuery = new TableQuery<AuthorEntity>().Where(
                //TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.GreaterThanOrEqual, envName.ToUpper()));

                //// Loop through the results, displaying information about the entity.
                //foreach (AuthorEntity entity in table.ExecuteQuery(rangeQuery))
                //{
                //    if (entity.ClientCode.ToUpper() == (clientCode + envName).ToUpper())
                //        return true;
                //}

                //return false;

                #endregion

                var tenant = _db.AzTenants.Where(q => q.ClientCode == tenants.ClientCode).FirstOrDefault();

                tenant.ApplicationTenantId = tenants.ApplicationTenantId;
                tenant.SubscriptionId = tenants.SubscriptionId;
                tenant.GuacamoleURL = tenants.GuacamoleURL;
                tenant.GuacConnection = tenants.GuacConnection;
                tenant.ApplicationId = tenants.ApplicationId;
                tenant.BusinessId = tenants.BusinessId;
                tenant.Regions = tenants.Regions;
                tenant.ApplicationSecretKey = tenants.ApplicationSecretKey;
                tenant.ProjectName = tenants.ProjectName;
                tenant.VPCSubNetworkGCP = tenants.VPCSubNetworkGCP;
                tenant.VPCNetworkGCP = tenants.VPCNetworkGCP;
                tenant.RegionGCP = tenants.RegionGCP;

                _db.Entry(tenant).State = EntityState.Modified;
                _db.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, "Ok!");
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);

            }

        }
        [HttpDelete]
        [Route("DeleteTenant")]
        public HttpResponseMessage DeleteTenant(int TenantId)
        {
            var sqlCommand = new SqlCommand(string.Format("DELETE FROM Tenants WHERE TenantId = '" + TenantId + "'"), _connTenant);

            _connTenant.Open();
            var command = sqlCommand.ExecuteReader();
            _connTenant.Close();
            return Request.CreateResponse(HttpStatusCode.OK, "Pasok Yehey!!");
        }

        [HttpGet]
        [Route("GetTenantID")]
        public HttpResponseMessage GetTenantID(string ApiUrl)
        {

            SqlCommand sqlCommand = new SqlCommand(string.Format("SELECT TenantID from Tenants where ApiUrl='" + ApiUrl + "'"), _connTenant);
            _connTenant.Open();
            SqlDataReader reader = sqlCommand.ExecuteReader();
            int id = 0;
            if (reader.Read())
            {
                id = Convert.ToInt32(reader["TenantId"].ToString());
            }
            reader.Close();
            _connTenant.Close();
            return Request.CreateResponse(HttpStatusCode.OK, id);
        }

        [HttpGet]
        [Route("CheckTenant")]
        public bool CheckTenant(string clientCode, string projectName)
        {
            if(clientCode != null)
                return _db.AzTenants.Any(q => q.ClientCode.ToUpper() == clientCode.ToUpper());
            else
                return _db.AzTenants.Any(q => q.ProjectName.ToUpper() == clientCode.ToUpper());
        }

        [HttpGet]
        [Route("GetTenantCodes")]
        public List<string> GetTenantCodes()
        {
            return _db.AzTenants.Select(q => q.ClientCode).ToList();
        }

        [HttpGet]
        [Route("GetTenantValues")]
        public List<CustomerTenants> GetTenantValues(string clientCode)
        {
            var tenants = _db.AzTenants.Where(q => q.ClientCode.ToUpper() == clientCode.ToUpper()).FirstOrDefault();

            var businessName = db.BusinessTypes.Where(w => w.BusinessId == tenants.BusinessId).ToArray();

            List<CustomerTenants> customerAz = new List<CustomerTenants>();
            CustomerTenants customer = new CustomerTenants();


            customer.ApplicationId = tenants.ApplicationId;
            customer.ApplicationSecretKey = tenants.ApplicationSecretKey;
            customer.ApplicationTenantId = tenants.ApplicationTenantId;
            customer.BusinessTypes = businessName[0];
            customer.ClientCode = tenants.ClientCode;
            customer.ClientKey = tenants.ClientKey;
            customer.ClientSecret = tenants.ClientSecret;
            customer.CreatedBy = tenants.CreatedBy;
            customer.DateCreated = tenants.DateCreated;
            customer.EnvironmentCode = tenants.EnvironmentCode;
            customer.GuacamoleURL = tenants.GuacamoleURL;
            customer.GuacConnection = tenants.GuacConnection;
            customer.Regions = tenants.Regions;
            customer.SubscriptionId = tenants.SubscriptionId;
            customer.TenantId = tenants.TenantId;
            customer.ProjectName = tenants.ProjectName;
            customer.RegionGCP = tenants.RegionGCP;
            customer.VPCNetworkGCP = tenants.VPCNetworkGCP;
            customer.VPCSubNetworkGCP = tenants.VPCSubNetworkGCP;

            customerAz.Add(customer);

            return customerAz;
        }

        [HttpGet]
        [Route("GetTenantGCPRegion")]
        public string GetTenantGCPRegion(string TenantId)
        {
            var tenant = Convert.ToInt32(TenantId);
            return _db.AzTenants.Where(q => q.TenantId == tenant).FirstOrDefault().RegionGCP;
        }


    }
}