using CloudSwyft.Auth.Models;
using CloudSwyft.OAuthServer.Helpers;
using ExcelDataReader;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace CloudSwyft.Auth.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        AuthContext _db = new AuthContext();
        AuthRepository auth = new AuthRepository();
        private UserManager<ApplicationUser> _userManager;

        private AuthContext _ctx;
        private AuthRepository _repo = null;
        public string cloudLabsServer = System.Configuration.ConfigurationManager.AppSettings["CloudLabsUrl"];
        public string authContext = System.Configuration.ConfigurationManager.ConnectionStrings["AuthContext"].ConnectionString;

        public AccountController()
        {
            _repo = new AuthRepository();
            _ctx = new AuthContext();

            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _repo.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }
            return null;
        }

        [Authorize]
        [HttpGet]
        [Route("Me")]
        public HttpResponseMessage Me()
        {
            try
            {
                string userId = User.Identity.GetUserId().ToString();
                //ApplicationUser user = await auth.FindUserById(userId);
                ApplicationUser user = auth.FindUserById(userId);
                string rid = user.Roles.Select(r => r.RoleId).FirstOrDefault();
                var roleName = _db.Roles.Where(r => r.Id == rid).FirstOrDefault().Name;
                var pass = user.PasswordHash;
                MeViewModel userViewModel = new MeViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    Role = roleName,
                    IsDeleted = user.isDeleted,
                    IsDisabled = user.isDisabled,
                    Thumbnail = user.Thumbnail,
                    Password = user.PasswordHash,
                    UserId = user.UserId,
                    UserGroup = user.UserGroup,
                    TenantId = user.TenantId,
                    UserIdLTI = user.UserIdLTI
                };
                return Request.CreateResponse(HttpStatusCode.OK, userViewModel);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Create")]
        public async Task<IHttpActionResult> CreateUser(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
                    string userId = User.Identity.GetUserId().ToString();
                    //ApplicationUser adminUser = await auth.FindUserById(userId);
                    ApplicationUser adminUser = auth.FindUserById(userId);
                    IdentityResult result;
                    var user = new ApplicationUser
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.Email,
                        Email = model.Email,
                        CreatedBy = adminUser.FirstName + " " + adminUser.LastName,
                        DateCreated = DateTime.UtcNow,
                        TenantId = adminUser.TenantId,
                        UserGroup = model.UserGroup,
                        UserIdLTI = "CreatedInUserManagement"
                    };
                    result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {

                        user.EmailConfirmed = false;
                        if (model.Roles != null)
                        {
                            await _userManager.AddToRolesAsync(user.Id, model.Roles);
                        }

                        using (HttpClient client = new HttpClient())
                        {
                            SendNewUserMail(user.Id, user.Email, model.Password);
                            // await client.GetAsync(cloudLabsServer + "api/users/SendClientUpdateMail?userId=" + user.Id + "&email=" + user.Email + "&password=" + model.Password + "&isEdit=" + false);
                        }

                        user.CredentialsSent = true;
                        await _userManager.UpdateAsync(user);
                        return Ok(user);
                    }
                    else
                        AddErrors(result);
                }
                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

        }


        [AllowAnonymous]
        [HttpPost]
        [Route("CreateDuo")]
        public async Task<IHttpActionResult> CreateDuo(ApplicationUser model)
        {
            try
            {
                _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
                var result = _userManager.CreateAsync(model, "Password1!").Result;
               
                if (result.Succeeded)
                {
                    model.EmailConfirmed = true;
                    if (model.Roles != null)
                    {
                        await _userManager.AddToRolesAsync(model.Id, "Student");
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        await SendNewUserMailWithoutUsePass(cloudLabsServer, model.Email, "Password1!");
                    }

                    model.CredentialsSent = true;
                    await _userManager.UpdateAsync(model);
                    return Ok(model);
                }
                else if(((string[])result.Errors)[0] == "Name " + model.Email + " is already taken.")
                    return Ok(model);
                else
                    AddErrors(result);

                return BadRequest(ModelState);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        [HttpPost]
        [Route("EditProfile")]
        public async Task<IHttpActionResult> EditProfile(EditViewModel editModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(editModel.Id);

                    IdentityResult result;

                    user.FirstName = editModel.FirstName;
                    user.UserName = editModel.Email;
                    user.LastName = editModel.LastName;
                    user.Email = editModel.Email;
                    user.UserGroup = editModel.UserGroup;
                    user.TenantId = editModel.TenantId;

                    result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        string[] allUserRoles = _userManager.GetRoles(editModel.Id).ToArray();
                        _userManager.RemoveFromRoles(editModel.Id, allUserRoles);

                        await _userManager.AddToRolesAsync(user.Id, editModel.Roles);

                        return Ok(user);
                    }
                    else
                        AddErrors(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            }
            return BadRequest(ModelState);
        }

        [HttpDelete]
        [Route("Disable")]
        public async Task<IHttpActionResult> DisableUser(string userId, bool isDisable)
        {
            ApplicationUser user = _userManager.FindById(userId);
            try
            {
                if (user != null && isDisable)
                {
                    user.isDisabled = true;
                    await _userManager.UpdateAsync(user);
                    return Ok(user);
                }
                else if (user != null && !isDisable)
                {
                    user.isDisabled = false;
                    await _userManager.UpdateAsync(user);
                    return Ok(user);
                }
                else
                    return NotFound();
            }
            catch (Exception)
            {
                return NotFound();
            }

        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IHttpActionResult> DeleteUser(string userId)
        {
            ApplicationUser user = _userManager.FindById(userId);

            if (user != null)
            {
                user.isDeleted = true;
                await _userManager.UpdateAsync(user);
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateFromEdx")]
        public async Task<IHttpActionResult> CreateFromEdx(string email, string password, string firstname, string lastname, string edxUrl)
        {
            try
            {
                var cloudlabsGroupId = 0;
                var tenantId = 0;
                var cloudLabsUrl = "";
                using (SqlConnection db = new SqlConnection(WebConfigurationManager.AppSettings["AuthConnectionString"]))
                {
                    db.Open();
                    using (SqlCommand command = new SqlCommand("SELECT CloudLabsGroupId, TenantID, CLUrl FROM CloudLabsGroups WHERE EdxUrl = '" + edxUrl + "'", db))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                cloudlabsGroupId = Int32.Parse(reader["CloudLabsGroupId"].ToString());
                                tenantId = Int32.Parse(reader["TenantID"].ToString());
                                cloudLabsUrl = reader["CLUrl"].ToString();
                            }

                        }
                        else
                            return BadRequest("EdxURL is not available");

                    }

                    using (SqlCommand com = new SqlCommand("SELECT Email FROM Cloudlabusers WHERE Email = '" + email + "'", db))
                    using (SqlDataReader read = com.ExecuteReader())
                    {
                        if (read.HasRows)
                            return BadRequest("Email already in use");
                        else {
                            var user = new ApplicationUser
                            {
                                UserName = email,
                                FirstName = firstname,
                                LastName = lastname,
                                Email = email,
                                CreatedBy = "EDX",
                                DateCreated = DateTime.Today,
                                UserGroup = cloudlabsGroupId,
                                TenantId = tenantId,
                                EmailConfirmed = true,
                                UserIdLTI = "EDx"
                            };

                            var roleName = "Student";
                            await _userManager.CreateAsync(user, password);

                            await _userManager.AddToRolesAsync(user.Id, roleName);
                            await SendNewUserMailWithoutUsePass(cloudLabsUrl, user.Email, password);
                            user.CredentialsSent = true;
                            await _userManager.UpdateAsync(user);
                            db.Close();
                        }
                    }


                }

                return Ok("created");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

        }

        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(string email, string newPassword)
        {
            try
            {
                ApplicationUser user = _userManager.FindByEmail(email);

                user.PasswordHash = _userManager.PasswordHasher.HashPassword(newPassword);

                await _userManager.UpdateAsync(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        public void SendNewUserMail(string userId, string email, string password)
        {
            string returnUrl = cloudLabsServer + "/Account/ConfirmEmailWithPass?";
            string paramUrl = "id=" + userId;
            MailInfo mailInfo = new MailInfo();
            mailInfo.SendTo = email;
            string htmlBody = string.Empty;

            htmlBody = "<head>"
                    + "<meta charset='utf-8'>"
                    + "<meta name='viewport' content='width=device-width, initial-scale=1.0'/>"
                    + "<title> Welcome to CloudSwyft!</title>"
                    + "<link rel='stylesheet' href=''>"
                    + "</head>"
                    + "<body style='margin: 0; padding: 0; font-family:'Open Sans', sans;'>"

                    + "<table border='0' cellpadding='0' cellspacing='0' width='100%'>"
                    + "<tr><td><table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' width = '600' style = 'border-collapse: collapse; border:1px solid #1058a0;'></tr></td>"
                    + "</table>"

                    + "<table align='center' width='600' style='background-color: #00bff6;padding-top:2%;border-top:2px solid blue;'>"
                    + "<tr align='center'>"
                    + "<td style='padding-top:10px; font-size: 18px; font-family: Helvetica'>Welcome to</td></tr>"
                    + "<tr align='center'><td style='font-family: Trebuchet MS;color: white;font-weight: bold; font-size: 60px;'>CloudSwyft</td ></tr >"
                    + "<tr align='center'><td style='font-family: Verdana; font-size: 23px;color: #005bab;'>CLOUD LABS</td></tr >"
                    + "</table>"

                    + "<table align = 'center' style='background-color: white; width: 600px;padding:15px 40px 0 40px; border: 1px solid #00bff6; text-align: center;border-bottom: none;'>"
                    + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; text-align: center;'> Thank you for signing up for the CloudSwyft Global Systems Inc.</td></tr >"
                    + "<td align='center' style='font-family: Verdana; font-size: 15px; text-align: center; font-weight: bold; padding-top:15px;'> Email Address: " + email + "</td>"
                    + "</table>"

                    + "<table align = 'center' style='background-color: white; width: 600px;padding:0px 40px 20px 40px; border: 1px solid #00bff6;border-bottom:none;'>"
                    + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; padding-top: 20px; text-align: justify;'> To get started with the Cloud Labs, click on the button below to verify your email address.</tr >"
                    + "</table>"

                    + "<table align = 'center' style='background-color: white; width: 600px;padding:0px 40px 20px 40px; border: 1px solid #00bff6; border-top:none;'>"
                    + "<tr align = 'center' ><td>"
                    + "<a href = " + returnUrl + paramUrl + " style ='text-decoration: none; color: #fff; background-color: #8FD034; font-size: 24px; color: white; padding: 8px;'>"
                    + "GET STARTED </a></td ></tr > "
                    + "</table>"

                    + "</table>";

            mailInfo.Subject = "Welcome to CloudSwyft";

            mailInfo.HtmlBody = htmlBody;

            MailHelper.SendMail(mailInfo);
        }

        public async Task<string> SendNewUserMailWithoutUsePass(string CloudLabsUrl, string email, string password)
        {
            try
            {
                string returnUrl = CloudLabsUrl;
                MailInfo mailInfo = new MailInfo();
                mailInfo.SendTo = email;
                string htmlBody = string.Empty;

                htmlBody = "<head>"
                        + "<meta charset='utf-8'>"
                        + "<meta name='viewport' content='width=device-width, initial-scale=1.0'/>"
                        + "<title> Welcome to CloudSwyft!</title>"
                        + "<link rel='stylesheet' href=''>"
                        + "</head>"
                        + "<body style='margin: 0; padding: 0; font-family:'Open Sans', sans;'>"

                        + "<table border='0' cellpadding='0' cellspacing='0' width='100%'>"
                        + "<tr><td><table align = 'center' border = '0' cellpadding = '0' cellspacing = '0' width = '600' style = 'border-collapse: collapse; border:1px solid #1058a0;'></tr></td>"
                        + "</table>"

                        + "<table align='center' width='600' style='background-color: #00bff6;padding-top:2%;border-top:2px solid blue;'>"
                        + "<tr align='center'>"
                        + "<td style='padding-top:10px; font-size: 18px; font-family: Helvetica'>Welcome to</td></tr>"
                        + "<tr align='center'><td style='font-family: Trebuchet MS;color: white;font-weight: bold; font-size: 60px;'>CloudSwyft</td ></tr >"
                        + "<tr align='center'><td style='font-family: Verdana; font-size: 23px;color: #005bab;'>CLOUD LABS</td></tr >"
                        + "</table>"

                        + "<table align = 'center' style='background-color: white; width: 600px;padding:15px 40px 0 40px; border: 1px solid #00bff6; text-align: center;border-bottom: none;'>"
                        + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; text-align: center;'> Thank you for signing up for the CloudSwyft Global Systems Inc.</td></tr >"
                        + "<td align='center' style='font-family: Verdana; font-size: 15px; text-align: center; font-weight: bold; padding-top:15px;'> Email Address: " + email + "</td>"
                        + "</table>"

                        //+ "<table align = 'center' style='background-color: white; width: 600px;padding:15px 40px 15px 40px; text-align: center; border: 1px solid #00bff6;border-top: none;'>"
                        //+ "<td align='center' style='font-family: Verdana; font-size: 15px; text-align: center; font-weight: bold;'> Password: " + password + "</td>"
                        //+ "</table>"

                        + "<table align = 'center' style='background-color: white; width: 600px;padding:0px 40px 20px 40px; border: 1px solid #00bff6;border-bottom:none;'>"
                        + "<tr align = 'center' ><td align='center' style='font-family: Verdana; font-size: 15px; padding-top: 20px; text-align: justify;'> To get started with the Cloud Labs, click on the button below to sign in.</tr >"
                        + "</table>"

                        + "<table align = 'center' style='background-color: white; width: 600px;padding:0px 40px 20px 40px; border: 1px solid #00bff6; border-top:none;'>"
                        + "<tr align = 'center' ><td>"
                        + "<a href = " + returnUrl + " style ='text-decoration: none; color: #fff;'>"
                        + " <div style='background-color: #8FD034; font-size: 24px; color: white; padding: 8px;'> GET STARTED </div> </a></td ></tr > "
                        + "</table>"

                        + "</table>";

                mailInfo.Subject = "Welcome to CloudSwyft";

                mailInfo.HtmlBody = htmlBody;

                MailHelper.SendMail(mailInfo);

                return "OK";
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateUserFromEdx")]
        public async Task<IHttpActionResult> CreateUserFromEdx(string email, string password, string firstname, string lastname, string edxUrl)

        {
            try
            {
                var cloudlabsGroupId = 0;
                var tenantId = 0;
                var cloudLabsUrl = "";
                var AuthConnectionString = "";
                _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };

                using (SqlConnection db = new SqlConnection(WebConfigurationManager.AppSettings["TenantConnectionString"]))
                {
                    db.Open();
                    using (SqlCommand command = new SqlCommand("SELECT TenantId, AuthConnectionString FROM AzTenants WHERE EdxUrl = '" + edxUrl + "' ", db))
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            AuthConnectionString = reader["AuthConnectionString"].ToString();
                            tenantId = Int32.Parse(reader["TenantID"].ToString());
                        }
                    }

                    using (SqlConnection _db = new SqlConnection(AuthConnectionString))
                    {
                        _db.Open();
                        using (SqlCommand com = new SqlCommand("SELECT CloudLabsGroupId, CLUrl FROM CloudLabsGroups WHERE TenantId = '" + tenantId + "' and EdxUrl = '" + edxUrl + "'", _db))
                        using (SqlDataReader read = com.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                cloudlabsGroupId = Int32.Parse(read["CloudLabsGroupId"].ToString());
                                cloudLabsUrl = read["CLUrl"].ToString();
                            }
                        }

                        var user = new ApplicationUser
                        {
                            UserName = email,
                            FirstName = firstname,
                            LastName = lastname,
                            Email = email,
                            CreatedBy = "EDX",
                            DateCreated = DateTime.Today,
                            UserGroup = cloudlabsGroupId,
                            TenantId = tenantId,
                            EmailConfirmed = true,
                            UserIdLTI = "NO USER ID GIVEN"
                        };

                        var roleName = "Student";
                        var result = await _userManager.CreateAsync(user, password);
                        if (result.Succeeded)
                            await _userManager.AddToRoleAsync(user.Id, roleName);
                        //await _userManager.UpdateAsync(user);
                        //await _userManager.AddToRoleAsync(user.Id, roleName);
                        await SendNewUserMailWithoutUsePass(cloudLabsUrl, user.Email, password);
                        user.CredentialsSent = true;
                        await _userManager.UpdateAsync(user);
                        db.Close();

                    }

                }


                return Ok("created");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

        }


        [AllowAnonymous]
        [HttpPost]
        [Route("CreateFromEdxDiffDB")]
        public async Task<IHttpActionResult> CreateFromEdxDiffDB(string email, string password, string firstname, string lastname, string edxUrl)
        {
            try
            {
                var authConnectionString = "";
                var tenantId = 0;
                var apiUrl = "";
                var cloudLabsGroupId = 0;
                var cloudLabsUrl = "";

                using (SqlConnection db = new SqlConnection(WebConfigurationManager.AppSettings["TenantConnectionString"]))
                {
                    db.Open();
                    using (SqlCommand tenantCommand = new SqlCommand("SELECT TenantID, ApiUrl, AuthConnectionString FROM Tenants WHERE EdxUrl = '" + edxUrl + "'", db))
                    using (SqlDataReader reader = tenantCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tenantId = Int32.Parse(reader["TenantID"].ToString());
                            apiUrl = reader["ApiUrl"].ToString();
                            authConnectionString = reader["AuthConnectionString"].ToString();
                        }
                        reader.Close();
                    }

                    db.Close();

                }

                using (SqlConnection _db = new SqlConnection(authConnectionString))
                {
                    _db.Open();
                    using (SqlCommand authCommand = new SqlCommand("SELECT CloudLabsGroupId, CLUrl FROM CloudLabsGroups WHERE TenantId = '" + tenantId + "'", _db))
                    using (SqlDataReader authReader = authCommand.ExecuteReader())
                    {
                        while (authReader.Read())
                        {
                            cloudLabsGroupId = Int32.Parse(authReader["CloudLabsGroupId"].ToString());
                            cloudLabsUrl = authReader["CLUrl"].ToString();
                        }

                        var user = new ApplicationUser
                        {
                            UserName = email,
                            FirstName = firstname,
                            LastName = lastname,
                            Email = email,
                            CreatedBy = "EDX",
                            DateCreated = DateTime.Today,
                            UserGroup = cloudLabsGroupId,
                            TenantId = tenantId,
                            EmailConfirmed = true
                        };

                        var roleName = "Student";
                        await _userManager.CreateAsync(user, password);

                        await _userManager.AddToRolesAsync(user.Id, roleName);
                        await SendNewUserMailWithoutUsePass(cloudLabsUrl, user.Email, password);
                        user.CredentialsSent = true;
                        await _userManager.UpdateAsync(user);
                    }


                }


                return Ok("created");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

        }

        [Route("BulkCreate")]
        [HttpPost]
        public async Task<IHttpActionResult> BulkCreate(string createdBy, int userGroupId, int tenantId)
        {
            try
            {
                HttpResponseMessage ResponseMessage = null;
                var httpRequest = HttpContext.Current.Request;
                DataSet dsexcelRecords = new DataSet();
                IExcelDataReader reader = null;
                HttpPostedFile Inputfile = null;
                Stream FileStream = null;
                _userManager.UserValidator = new UserValidator<ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };

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
                        List<Student> studentList = new List<Student>();

                        if (dsexcelRecords != null && dsexcelRecords.Tables.Count > 0)
                        {
                            await Task.Run(() =>
                            {
                                DataTable dtStudentRecords = dsexcelRecords.Tables[0];

                                foreach (DataRow item in dtStudentRecords.Rows.Cast<DataRow>().Skip(1))
                                {
                                    Student student = new Student();

                                    student.Email = item.ItemArray[0].ToString();
                                    student.Password = item.ItemArray[1].ToString();
                                    student.Firstname = item.ItemArray[2].ToString();
                                    student.Lastname = item.ItemArray[3].ToString();
                                    student.Role = "Student";
                                    student.UserGroup = userGroupId;
                                    student.TenantId = tenantId;
                                    student.UserIdLTI = "Bulk Create";

                                    //student.Role = item.ItemArray[4].ToString();
                                    //student.UserGroup = Convert.ToInt32(item.ItemArray[5]);
                                    //student.UserGroup = Convert.ToInt32(item.ItemArray[5]);
                                    //student.TenantId = Convert.ToInt32(item.ItemArray[6]);
                                    //student.UserIdLTI = "Bulk Create";

                                    studentList.Add(student);
                                }

                                IdentityResult result;

                                foreach (var stud in studentList)
                                {
                                    try
                                    {
                                        var user = new ApplicationUser
                                        {
                                            FirstName = stud.Firstname,
                                            LastName = stud.Lastname,
                                            UserName = stud.Email,
                                            Email = stud.Email,
                                            CreatedBy = createdBy,
                                            DateCreated = DateTime.UtcNow,
                                            TenantId = stud.TenantId,
                                            UserGroup = stud.UserGroup,
                                            UserIdLTI = stud.UserIdLTI
                                        };

                                        Thread.Sleep(10000);

                                        result = CreateUser(user, stud.Password);
                                        //result = _userManager.CreateAsync(user, stud.Password);

                                        if (result.Succeeded)
                                        {
                                            user.EmailConfirmed = false;
                                            if (stud.Role != null)
                                            {
                                                _userManager.AddToRolesAsync(user.Id, stud.Role);
                                            }

                                            using (HttpClient client = new HttpClient())
                                            {
                                                SendNewUserMail(user.Id, user.Email, stud.Password);
                                            }
                                            user.EmailConfirmed = true;
                                            user.CredentialsSent = true;
                                            _userManager.UpdateAsync(user);
                                        }
                                    }
                                    catch(Exception e)
                                    {
                                    }
                                   
                                }
                            });
                        }
                        else
                            return BadRequest();
                    }
                    else
                        return BadRequest();
                }
                else
                    ResponseMessage = Request.CreateResponse(HttpStatusCode.BadRequest);

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public IdentityResult CreateUser(ApplicationUser user, string Password)
        {
            var result = _userManager.CreateAsync(user, Password).Result;

            return result;
        }

        [Route("BulkUserCreation")]
        [HttpGet]
        public async Task<IHttpActionResult> BulkUserCreation()
        {
            //if (!Request.Content.IsMimeMultipartContent())
            //{
            //    return InternalServerError(
            //        new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
            //}
            var counter = 0;
            var notCounted = 0;
            try
            {
                List<Student> studentList = new List<Student>();
                //using (StreamReader sr = new StreamReader("C:\\Users\\Kenneth\\Desktop\\StudentList.csv"))
                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/StudentList.csv"))
                {
                    try {
                        string[] headers = sr.ReadLine().Split(',');
                        while (!sr.EndOfStream)
                        {
                            string[] rows = sr.ReadLine().Split(',');
                            var student = new Student
                            {
                                Email = rows[0],
                                Password = rows[1],
                                Firstname = rows[2],
                                Lastname = rows[3],
                                Role = rows[4],
                                UserGroup = Int32.Parse(rows[5]),
                                TenantId = Int32.Parse(rows[6]),
                                UserIdLTI = rows[7]
                            };
                            studentList.Add(student);
                        }
                    }
                    catch(Exception e)
                    {
                        return Ok(e.Message);

                    }

                }
                _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
//                string userId = User.Identity.GetUserId().ToString();
                //ApplicationUser adminUser = await auth.FindUserById(userId);
                //ApplicationUser adminUser = auth.FindUserById(userId);
                IdentityResult result;

                foreach (var stud in studentList)
                {
                    var user = new ApplicationUser
                    {
                        FirstName = stud.Firstname,
                        LastName = stud.Lastname,
                        UserName = stud.Email,
                        Email = stud.Email,
                        CreatedBy = "Super Admin",
                        DateCreated = DateTime.UtcNow,
                        TenantId = stud.TenantId,
                        UserGroup = stud.UserGroup,
                        UserIdLTI = stud.UserIdLTI
                    };
                    result = await _userManager.CreateAsync(user, stud.Password);

                    if (result.Succeeded)
                    {
                        counter++;

                        user.EmailConfirmed = false;
                        if (stud.Role != null)
                        {
                            await _userManager.AddToRolesAsync(user.Id, stud.Role);
                        }

                        using (HttpClient client = new HttpClient())
                        {
                            //SendNewUserMail(user.Id, user.Email, stud.Password);

                            //  SendNewUserMail(user.Id, user.Email, stud.Password);
                            // await client.GetAsync(cloudLabsServer + "api/users/SendClientUpdateMail?userId=" + user.Id + "&email=" + user.Email + "&password=" + model.Password + "&isEdit=" + false);
                        }
                        user.EmailConfirmed = true;
                        user.CredentialsSent = true;
                        await _userManager.UpdateAsync(user);
                        //return Ok(user);
                    }
                    else
                        notCounted++;
                    //else
                    //    AddErrors(result);
                }


                return Ok("Total NotCounted: " + notCounted + "    Total created: " + counter + "      Total Users:" + studentList.Count()) ;
            }
           catch (DbEntityValidationException e)
            {
                return BadRequest(e.Message);
            }


        }

        [Route("ResendEmail")]
        [HttpGet]
        public async Task<IHttpActionResult> ResendEmail()
        {
            var counter = 0;
            var notCounted = 0;
            try
            {
                List<ResendEmailStudent> res = new List<ResendEmailStudent>();
                //using (StreamReader sr = new StreamReader("C:\\Users\\Kenneth\\Desktop\\StudentList.csv"))
                using (StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/StudentList1.csv"))
                {
                    try
                    {
                        string[] headers = sr.ReadLine().Split(',');
                        while (!sr.EndOfStream)
                        {
                            string[] rows = sr.ReadLine().Split(',');
                            var student = new ResendEmailStudent
                            {
                                Email = rows[0],
                                Id = rows[1],
                                
                            };
                            res.Add(student);
                        }
                    }
                    catch (Exception e)
                    {
                        return Ok(e.Message);

                    }

                }
                _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
              
                IdentityResult result;

                foreach (var stud in res)
                {


                        using (HttpClient client = new HttpClient())
                        {
                            SendNewUserMail(stud.Id, stud.Email, "");

                          
                        }
                }

                return Ok("Total NotCounted: " + notCounted + "    Total created: " + counter + "      Total Users:" + res.Count());
            }
            catch (DbEntityValidationException e)
            {
                return BadRequest(e.Message);
            }


        }

        public class Student
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Role { get; set; }
            public int UserGroup { get; set; }
            public int TenantId { get; set; }
            public string UserIdLTI { get; set; }
        }
        public class ResendEmailStudent
        {
            public string Email { get; set; }
            public string Id { get; set; }
        }

        public class StudentEmail
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string CloudLabsUrl { get; set; }

        }


        [Route("BulkSendUserEmailLMS")]
        [HttpGet]
        public async Task<IHttpActionResult> BulkSendUserEmailLMS()
        {
            try
            {
                List<StudentEmail> studentList = new List<StudentEmail>();
                using (StreamReader sr = new StreamReader("C:\\Users\\Kenneth\\Desktop\\SendBulkEMailLMS.csv"))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        var studentEmail = new StudentEmail
                        {
                            Email = rows[0],
                            CloudLabsUrl = rows[1],
                            Password = rows[2],
                            
                        };
                        studentList.Add(studentEmail);
                    }
                }
                foreach (var stud in studentList)
                {
                    //SendNewUserMail("1234556-7123", stud.Email, stud.Password);

                    SendNewUserMailWithoutUsePass(stud.CloudLabsUrl, stud.Email, stud.Password);

                }
                return Ok(studentList);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("BulkUserCreation")]
        //public async Task<IHttpActionResult> BulkUserCreation(int tenantId, string userCreator)
        //{
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        return InternalServerError(
        //            new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
        //    }

        //    try
        //    {
        //        var provider = new MultipartMemoryStreamProvider();
        //        await Request.Content.ReadAsMultipartAsync(provider);
        //        foreach (var file in provider.Contents)
        //        {
        //            string ext = Path.GetExtension(file.Headers.ContentDisposition.FileName.Replace("\\\"", "").Replace("\"", ""));
        //            string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/LabActivity/");

        //            using (var reader = new StreamReader(@"D:\BOOK.csv"))
        //            {
        //                var line = reader.ReadLine();
        //                while (!reader.EndOfStream)
        //                {
        //                    line = reader.ReadLine();
        //                    listA.Add(line);
        //                }
        //            }
        //            if (!Directory.Exists(dirPath))
        //            {
        //                System.IO.Directory.CreateDirectory(dirPath);
        //            }

        //            var dataStream = await file.ReadAsStreamAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return Ok();
        //            //try
        //            //{
        //            //    if (ModelState.IsValid)
        //            //    {
        //            //        _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
        //            //        string userId = User.Identity.GetUserId().ToString();
        //            //        ApplicationUser adminUser = auth.FindUserById(userId);
        //            //        IdentityResult result;
        //            //        var user = new ApplicationUser
        //            //        {
        //            //            FirstName = model.FirstName,
        //            //            LastName = model.LastName,
        //            //            UserName = model.Email,
        //            //            Email = model.Email,
        //            //            CreatedBy = adminUser.FirstName + " " + adminUser.LastName,
        //            //            DateCreated = DateTime.UtcNow,
        //            //            TenantId = adminUser.TenantId,
        //            //            UserGroup = model.UserGroup
        //            //        };
        //            //        result = await _userManager.CreateAsync(user, model.Password);

        //            //        if (result.Succeeded)
        //            //        {

        //            //            user.EmailConfirmed = false;
        //            //            if (model.Roles != null)
        //            //            {
        //            //                await _userManager.AddToRolesAsync(user.Id, model.Roles);
        //            //            }

        //            //            using (HttpClient client = new HttpClient())
        //            //            {
        //            //                SendNewUserMail(user.Id, user.Email, model.Password);
        //            //                // await client.GetAsync(cloudLabsServer + "api/users/SendClientUpdateMail?userId=" + user.Id + "&email=" + user.Email + "&password=" + model.Password + "&isEdit=" + false);
        //            //            }

        //            //            user.CredentialsSent = true;
        //            //            await _userManager.UpdateAsync(user);
        //            //            return Ok(user);
        //            //        }
        //            //        else
        //            //            AddErrors(result);
        //            //    }
        //            //    return BadRequest(ModelState);
        //            //}
        //            //catch (Exception e)
        //            //{
        //            //    return BadRequest(ModelState);

        //            //}

        //        }

        [AllowAnonymous]
        [HttpPost]
        [Route("CreateFromLTI")]
        public async Task<IHttpActionResult> CreateFromLTI(string firstname, string lastname, string email, string userIdLTI, string clientKey)
        {
            try
            {
                _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };

                SqlConnection db = new SqlConnection(WebConfigurationManager.AppSettings["AuthConnectionString"]);
                SqlConnection _db = new SqlConnection(WebConfigurationManager.AppSettings["TenantConnectionString"]);
                db.Open();
                _db.Open();
                SqlCommand command = new SqlCommand("Select TenantId FROM AzTenants WHERE ClientKey = '" + clientKey + "'", _db);
                Int32 tenantId = (Int32)command.ExecuteScalar();

                SqlCommand command2 = new SqlCommand("Select CloudLabsGroupId FROM CloudLabsGroups WHERE TenantId = " + tenantId, db);
                Int32 userGroup = (Int32)command2.ExecuteScalar();

                SqlCommand command3 = new SqlCommand("Select CLPREFIX FROM CloudLabsGroups WHERE TenantId = " + tenantId, db);
                string clPrefix = (string)command3.ExecuteScalar();


                IdentityResult result;


                if(email == null)
                {
                    IdentityResult result2;
                    var user = new ApplicationUser
                    {
                        FirstName = "",
                        LastName = "",
                        Email = EmailGenerator(clPrefix),
                        UserName = EmailGenerator(clPrefix),
                        CreatedBy = "LTI Client",
                        DateCreated = DateTime.Today,
                        UserGroup = userGroup,
                        TenantId = tenantId,
                        EmailConfirmed = true,
                        UserIdLTI = userIdLTI
                    };

                    var roleName = "Student";
                    result2 = await _userManager.CreateAsync(user, "Password1!");

                    if (result2.Succeeded)
                    {
                        await _userManager.AddToRolesAsync(user.Id, roleName);

                        user.CredentialsSent = true;
                        await _userManager.UpdateAsync(user);
                        return Ok(user);
                    }
                }
                else
                {
                    var user = new ApplicationUser
                    {
                        FirstName = firstname != null ? firstname : "",
                        LastName = lastname != null ? lastname : "",
                        Email = email,
                        UserName = email,
                        CreatedBy = "LTI Client",
                        DateCreated = DateTime.Today,
                        UserGroup = userGroup,
                        TenantId = tenantId,
                        EmailConfirmed = true,
                        UserIdLTI = userIdLTI == null ? "NO USER ID GIVEN" : userIdLTI
                    };

                    var roleName = "Student";
                    result = await _userManager.CreateAsync(user, "P@$Sword1!");

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRolesAsync(user.Id, roleName);

                        user.CredentialsSent = true;

                        await _userManager.UpdateAsync(user);
                        return Ok(user);
                    }
                }

                db.Close();
                _db.Close();
               
                //else
                //    return BadRequest();
                //}
                return Ok("OK");
                // }

                //}
                //}

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);

            }

        }

        public static string EmailGenerator(string clPrefix)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            const string numbers = "0123456789";
            var random = new Random();
            string result = string.Empty;

            var generatedValues = new string(
                Enumerable.Repeat(chars, 7)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            var iterator = new string(
                Enumerable.Repeat(numbers, 4)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());

            result = "temporaryemail" + iterator + generatedValues + "@" + clPrefix + ".com";

            return result;
        }

        [HttpPost]
        [Route("EditFromLTI")]
        public async Task<IHttpActionResult> EditFromLTI(EditViewModelLTI editModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = await _userManager.FindByIdAsync(editModel.Id);

                    IdentityResult result;

                    user.FirstName = editModel.FirstName;
                    user.UserName = editModel.Email;
                    user.LastName = editModel.LastName;
                    user.Email = editModel.Email;
                    user.PasswordHash = _userManager.PasswordHasher.HashPassword(editModel.Password);

                    result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            SendNewUserMail(user.Id, user.Email, editModel.Password);
                        }

                        return Ok(user);
                    }
                    else
                        AddErrors(result);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);

                }

            }
            return BadRequest(ModelState);
        }

        //[AllowAnonymous]
        //[HttpPost]
        //[Route("CreateDuoUser")]
        //public async Task<IHttpActionResult> CreateDuoUser(string email, string firstname, string lastname)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _userManager.UserValidator = new UserValidator<Models.ApplicationUser>(_userManager) { AllowOnlyAlphanumericUserNames = false };
                  
        //            IdentityResult result;
        //            var user = new ApplicationUser
        //            {
        //                FirstName = firstname,
        //                LastName = lastname,
        //                UserName = email,
        //                Email = email,
        //                CreatedBy = "DUO",
        //                DateCreated = DateTime.UtcNow,
        //                TenantId = 1,
        //                UserGroup = 1,
        //                UserIdLTI = "DUO"
        //            };
        //            result = await _userManager.CreateAsync(user, "Password1!");

        //            if (result.Succeeded)
        //            {

        //                user.EmailConfirmed = false;

        //                await _userManager.AddToRolesAsync(user.Id, "Student");
                       

        //                using (HttpClient client = new HttpClient())
        //                {
        //                    SendNewUserMail(user.Id, user.Email, model.Password);
        //                    // await client.GetAsync(cloudLabsServer + "api/users/SendClientUpdateMail?userId=" + user.Id + "&email=" + user.Email + "&password=" + model.Password + "&isEdit=" + false);
        //                }

        //                user.CredentialsSent = true;
        //                await _userManager.UpdateAsync(user);
        //                return Ok(user);
        //            }
        //            else
        //                AddErrors(result);
        //        }
        //        return BadRequest(ModelState);
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message);

        //    }

        //}
    }
}