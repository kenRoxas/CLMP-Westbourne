using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CloudSwyft.Web.Api.Models;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web.Configuration;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/ConsoleSchedules")]
    public class ConsoleSchedulesController : ApiController
    {
        private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private string AWSConsoleURL = WebConfigurationManager.AppSettings["AWSConsoleURL"];


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


        [HttpGet]
        [Route("SpecificConsoleUser")]
        public HttpResponseMessage SpecificConsoleUser(int userId, int veprofileId)
        {
            try
            {
                var GetStudentConsole = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/";
                var url = "dev/get_current_budget_of_student/";

                var consoleDSchedules = _db.ConsoleSchedules.Where(q => (q.UserId == userId) && (q.VEProfileId == veprofileId)).FirstOrDefault();

                var response = ApiCall("GET", GetStudentConsole, url + consoleDSchedules.AccountId, null);

                var dataJSON = JsonConvert.DeserializeObject<ConsoleDetail>(response.Result);


                var BLimit = new BudgetLimit
                {
                    Amount = "0",
                    Unit = "",
                };

                var ASpend = new DataSpend
                {
                    ActualSpend = new ActualSpend { Amount = "0", Unit = "" }
                };

                var ConsoleDetails = new ConsoleDetail
                {
                    AccountID = consoleDSchedules.AccountId,
                    AccountName = consoleDSchedules.AccountName,
                    AccountEmail = consoleDSchedules.AccountEmail,
                    VEProfileID = consoleDSchedules.VEProfileId,
                    Data_transfer_budget_limit = dataJSON.Data_transfer_budget_limit ?? BLimit,
                    Cost_budget_limit = dataJSON.Cost_budget_limit ?? BLimit,
                    Actual_data_transfer_spend = dataJSON.Actual_data_transfer_spend ?? ASpend,
                    Actual_costs_spend = dataJSON.Actual_costs_spend ?? ASpend,
                    Is_suspended = dataJSON.Is_suspended
                };

         

                return Request.CreateResponse(HttpStatusCode.OK, ConsoleDetails);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpPost]
        [Route("IsSuspendedTriggered")]
        public async Task<IHttpActionResult> IsSuspendedTriggered(SuspendedDetails contents)
        {
            try
            {

                var GetStudentConsole = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/dev/suspend_account";
                //var url = "/dev/suspend_account";
                var dataMessage = JsonConvert.SerializeObject(contents);
                var response = await ApiCall("POST", GetStudentConsole, "", dataMessage);
                return Ok(response);
            }
            catch (Exception e)
            {

                return Ok(e);
            }
        }


        [HttpGet]
        [Route("GetConsoleLab")]
        public HttpResponseMessage GetConsoleLab(int usergroupId)
        {
            try
            {
                var url = "dev/get_current_budget_of_student/";
                var result = new List<ConsoleDetail>();

                var consoleLab = _db.ConsoleSchedules
                                   .Join(_db.VEProfiles,
                                   a => a.VEProfileId,
                                   b => b.VEProfileID,
                                   (a, b) => new { a, b })
                                   .Join(_db.VirtualEnvironments,
                                   c => c.b.VirtualEnvironmentID,
                                   d => d.VirtualEnvironmentID,
                                   (c, d) => new { c, d })
                                   .Join(_db.VETypes,
                                   e => e.d.VETypeID,
                                   f => f.VETypeID,
                                   (e, f) => new { e, f })
                                   .Join(_db.CloudLabUsers,
                                   g => g.e.c.a.UserId,
                                   h => h.UserId,
                                   (g, h) => new { g, h })
                                   .Where(q => (q.g.f.VETypeID == 6 || q.g.f.VETypeID == 7) && q.h.UserGroup == usergroupId)
                                   .Select(x => new
                                   {
                                       VEProfileID = x.g.e.c.b.VEProfileID,
                                       AccountEmail = x.g.e.c.a.AccountEmail,
                                       //AccountName = x.g.e.c.a.AccountName,,
                                       AccountName = x.h.FirstName + " " +x.h.LastName,
                                       AccountID = x.g.e.c.a.AccountId,
                                       AccountPassword = x.g.e.c.a.AccountPassword
                                   }).ToList();

                foreach (var item in consoleLab)
                {

                    var response = ApiCall("GET", AWSConsoleURL, url + item.AccountID, null);

                    var dataJSON = JsonConvert.DeserializeObject<ConsoleDetail>(response.Result);
             
                    var BLimit = new BudgetLimit
                    {
                        Amount = "0",
                        Unit = "",
                    };

                    var ASpend = new DataSpend
                    {
                        ActualSpend = new ActualSpend { Amount="0", Unit=""}
                    };
                   
                    string password = Decrypt(item.AccountPassword);

                    var ConsoleDetails = new ConsoleDetail
                    {                         
                        AccountID = item.AccountID,
                        AccountName = item.AccountName,
                        AccountEmail = item.AccountEmail,
                        VEProfileID = item.VEProfileID,
                        Data_transfer_budget_limit = dataJSON.Data_transfer_budget_limit ?? BLimit,
                        Cost_budget_limit = dataJSON.Cost_budget_limit ?? BLimit,
                        Actual_data_transfer_spend = dataJSON.Actual_data_transfer_spend ?? ASpend,
                        Actual_costs_spend = dataJSON.Actual_costs_spend ?? ASpend,
                        Is_suspended = dataJSON.Is_suspended,
                        SuspendProgress = false,
                        AccountPassword = password
                    };

                    result.Add(ConsoleDetails);


                }
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);

            }
        }

        [HttpGet]
        [Route("GetAWSConsoleDetails")]
        public async Task<IHttpActionResult> GetAWSConsoleDetails(string accountId, string startDate, string endDate)
        {
            try
            {
                var s = JsonConvert.DeserializeObject<string[]>(accountId);

                var url = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/dev/get_budget_range";
                var urlSuspend = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/dev/get_current_budget_of_student/";
                List<ConsoleDetailsCSV> details = new List<ConsoleDetailsCSV>();

                foreach (var item in s)
                {
                    var data = new
                    {
                        account_id = item,
                        start_date = "2021-02-01",
                        end_date = "2021-02-28"
                    };

                    var userData = _db.ConsoleSchedules.Where(q => q.AccountId.ToString() == item).FirstOrDefault();
                    var courseName = _db.VEProfiles.Where(q => q.VEProfileID == userData.VEProfileId).FirstOrDefault().Name;

                    var veTypeName = _db.VEProfiles.Where(q => q.VEProfileID == userData.VEProfileId)
                        .Join(_db.VirtualEnvironments,
                        a => a.VirtualEnvironmentID,
                        b => b.VirtualEnvironmentID,
                        (a, b) => new { a, b })
                        .Join(_db.VETypes,
                        c => c.b.VETypeID,
                        d => d.VETypeID,
                        (c, d) => new { c, d }).FirstOrDefault().d.Name;

                    var dataContent = JsonConvert.SerializeObject(data);

                    var result = await ApiCall("POST", url, "", dataContent); 

                    var resultSuspend = await ApiCall("GET", urlSuspend + item, "");

                    var AWSdata = JsonConvert.DeserializeObject<ConsoleDetailsCSV>(result);
                    var suspendData = JsonConvert.DeserializeObject<ConsoleDetail>(resultSuspend);

                    AWSdata.TeamName = courseName;
                    AWSdata.Platform = veTypeName;
                    AWSdata.Email = userData.Email;
                    AWSdata.LabId = userData.VEProfileId;
                    //AWSdata.LabGuid = userData.RequestId;
                    AWSdata.Suspended = suspendData.Is_suspended;
                    AWSdata.Comments = "";
                    AWSdata.Comments = "";

                    details.Add(AWSdata);
                }
                return Ok(details);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("UpdateBudget")]
        public async Task<Boolean> UpdateBudget(string accountId, string budgetLimit, string limitType)
        {
            try
            {
                var url = "https://pclduv263j.execute-api.us-east-1.amazonaws.com/dev/update_budget";

                var data = new
                {
                    new_budget_limit= budgetLimit,
                    account_id = accountId,
                    limit_type = limitType
                };

                string dataString = JsonConvert.SerializeObject(data);

                bool isTrue = true;

                while (isTrue)
                {
                    var response = await ApiCall("POST", url, "", dataString);
                    if (response == "\"Done\"")
                        isTrue = false;
                }

                return true;

            }
            catch (Exception)
            {
                return false;
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

    }
}
