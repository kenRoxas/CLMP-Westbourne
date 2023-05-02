using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;
using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Data.OData.Query.SemanticAst;
using System.Data.Entity;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.IO;
using System.Globalization;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/UserManagement")]
    public class UsermanagementController : ApiController
    {

        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private readonly SqlConnection _dbCon = new SqlConnection(WebConfigurationManager.AppSettings["AssConnectionString"]);

        [HttpGet]
        [Route("GetUserManagementUsers")]
        public HttpResponseMessage GetUserManagementUsers(string userId, string role)
        {
            var users = new ArrayList();
            try
            {
                SqlCommand dbCommand = new SqlCommand("spGetUserManagementUsers", _dbCon);
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.Parameters.Add(new SqlParameter("userId", userId));
                dbCommand.Parameters.Add(new SqlParameter("role", role));
                _dbCon.Open();

                var dbReader = dbCommand.ExecuteReader();

                while (dbReader.Read())
                {
                    var dataRow = new Hashtable();
                    for (var i = 0; i < dbReader.FieldCount; i++)
                    {
                        dataRow.Add(dbReader.GetName(i), dbReader[dbReader.GetName(i)]);
                    }
                    users.Add(dataRow);
                }
                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception e)
            {
                //throw new Exception(e.Message);
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }


        [HttpGet]
        [Route("GetCurrentUserRole")]
        public HttpResponseMessage GetCurrentUserRole(string role)
        {
            var roles = new ArrayList();
            if (role == "SuperAdmin")
            {
                roles.Add("Admin");
                roles.Add("Candidate");
                roles.Add("Recruiter");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            if (role == "Admin")
            {
                roles.Add("Candidate");
                roles.Add("Recruiter");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            if (role == "Recruiter")
            {
                roles.Add("Candidate");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            else
            {
                roles.Add("");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }

        }

        [HttpGet]
        [Route("GetUserManagementRecord")]
        public HttpResponseMessage GetUserManagementRecord(string id, string userRole, string name = "", string email = "", string role = "", string status = "", string createdBy = "", string startDate = "", string endDate = "")
        {
            ArrayList CandidateRecordByUser = new ArrayList();

            try
            {
                SqlCommand dbCommand = new SqlCommand("spGetUserManagementRecord", _dbCon);
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.Parameters.Add(new SqlParameter("id", id));
                dbCommand.Parameters.Add(new SqlParameter("userRole", userRole));

                if (!string.IsNullOrEmpty(name))
                    dbCommand.Parameters.Add(new SqlParameter("name", name));
                if (!string.IsNullOrEmpty(email))
                    dbCommand.Parameters.Add(new SqlParameter("email", email));
                if (!string.IsNullOrEmpty(role))
                    dbCommand.Parameters.Add(new SqlParameter("role", role));
                if (!string.IsNullOrEmpty(status))
                    dbCommand.Parameters.Add(new SqlParameter("status", status));
                if (!string.IsNullOrEmpty(createdBy))
                    dbCommand.Parameters.Add(new SqlParameter("createdBy", createdBy));
                if (!string.IsNullOrEmpty(startDate))
                    dbCommand.Parameters.Add(new SqlParameter("startdate", startDate));
                //dbCommand.Parameters.Add(new SqlParameter("startdate", Convert.ToDateTime(startDate)));
                if (!string.IsNullOrEmpty(endDate))
                    dbCommand.Parameters.Add(new SqlParameter("enddate", endDate));
                //dbCommand.Parameters.Add(new SqlParameter("enddate", Convert.ToDateTime(endDate)));

                _dbCon.Open();

                SqlDataReader dbReader = dbCommand.ExecuteReader();

                while (dbReader.Read())
                {
                    Hashtable dataRow = new Hashtable();
                    for (int i = 0; i < dbReader.FieldCount; i++)
                    {
                        dataRow.Add(dbReader.GetName(i), dbReader[dbReader.GetName(i)]);
                    }
                    CandidateRecordByUser.Add(dataRow);
                }
                return Request.CreateResponse(HttpStatusCode.OK, CandidateRecordByUser);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }

        [HttpGet]
        [Route("GetEmailAddressExist")]
        public HttpResponseMessage GetEmailAddressExist(string email)
        {
            try
            {
                var isEmailExist = _db.CloudLabUsers.Any(x => x.Email == email);

                return Request.CreateResponse(HttpStatusCode.OK, isEmailExist);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }

        [HttpGet]
        [Route("GetCreatedBy")]
        public HttpResponseMessage GetCreatedBy()
        {
            try
            {
                var createdBy = _db.CloudLabUsers.Select(x => x.CreatedBy).Distinct();

                return Request.CreateResponse(HttpStatusCode.OK, createdBy);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }

        [HttpGet]
        [Route("GetUserRoleCloudLabs")]
        public HttpResponseMessage GetUserRoleCloudLabs(string role)
        {
            var roles = new ArrayList();
            if (role == "SuperAdmin")
            {
                roles.Add("Admin");
                roles.Add("Student");
                roles.Add("Instructor");
                roles.Add("Staff");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            if (role == "Admin")
            {
                roles.Add("Student");
                roles.Add("Instructor");
                roles.Add("Staff");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            if (role == "Instructor")
            {
                roles.Add("Student");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }
            else
            {
                roles.Add("");
                return Request.CreateResponse(HttpStatusCode.OK, roles);
            }

        }

        [HttpGet]
        [Route("GetUsersCloudLabs")]
        public HttpResponseMessage GetUsersCloudLabs(string id, string role, int groupId)
        {
            string role2 = "Admin", role3 = "Instructor";

            if (role == "Admin")
            {
                role2 = "SuperAdmin"; role3 = "SuperAdmin";
            }
            else if (role == "Instructor")
            {
                role2 = "SuperAdmin"; role3 = "Admin";
            }
            else
            {
                role2 = "SuperAdmin"; role3 = "SuperAdmin";
            }
            var users = _db.AspNetUserRoles
                .Where(aur => aur.UserId != id)
                .Join(_db.CloudLabUsers,
                x => x.UserId,
                y => y.Id,
                (x, y) => new { AspNetUserRoles = x, CloudLabUsers = y })
                .Select(s => new
                {
                    RoleId = s.AspNetUserRoles.RoleId,
                    s.CloudLabUsers
                })
                .Join(_db.AspNetRoles,
                a => a.RoleId,
                b => b.Id,
                (a, b) => new { CloudLabUsers = a, AspNetRoles = b })
                .Where(ar => ar.AspNetRoles.Name != role)
                .Where(ar => ar.AspNetRoles.Name != role2)
                .Where(ar => ar.AspNetRoles.Name != role3)
                .Select(q => new
                {
                    CloudlabUser = q.CloudLabUsers,
                    Rolename = q.AspNetRoles.Name
                })
                .Join(_db.CloudLabsGroups,
                a => a.CloudlabUser.CloudLabUsers.UserGroup,
                b => b.CloudLabsGroupID,
                (a, b) => new { CloudLabUsers = a, CloudLabsGroups = b })
                .Select(w => new AllUsers
                {
                    Id = w.CloudLabUsers.CloudlabUser.CloudLabUsers.Id,
                    RoleName = w.CloudLabUsers.Rolename,
                    Email = w.CloudLabUsers.CloudlabUser.CloudLabUsers.Email,
                    IsDisabled = w.CloudLabUsers.CloudlabUser.CloudLabUsers.isDisabled,
                    IsDeleted = w.CloudLabUsers.CloudlabUser.CloudLabUsers.isDeleted,
                    Name = w.CloudLabUsers.CloudlabUser.CloudLabUsers.FirstName + " " + w.CloudLabUsers.CloudlabUser.CloudLabUsers.LastName,
                    Firstname = w.CloudLabUsers.CloudlabUser.CloudLabUsers.FirstName,
                    Lastname = w.CloudLabUsers.CloudlabUser.CloudLabUsers.LastName,
                    CreatedBy = w.CloudLabUsers.CloudlabUser.CloudLabUsers.CreatedBy,
                    EmailConfirmed = w.CloudLabUsers.CloudlabUser.CloudLabUsers.EmailConfirmed,
                    Thumbnail = w.CloudLabUsers.CloudlabUser.CloudLabUsers.ThumbNail,
                    UserId = w.CloudLabUsers.CloudlabUser.CloudLabUsers.UserId,
                    DateCreated = DbFunctions.TruncateTime(w.CloudLabUsers.CloudlabUser.CloudLabUsers.DateCreated).Value,
                    GroupName = w.CloudLabsGroups.GroupName,
                    GroupId = w.CloudLabsGroups.CloudLabsGroupID
                });

            if (role != "SuperAdmin")
                users = users.Where(x => x.GroupId == groupId);
          

            return Request.CreateResponse(HttpStatusCode.OK, users);

        }


        [HttpGet]
        [Route("GetRoleIdByName")]
        public HttpResponseMessage GetRoleIdByName(string roleName)
        {
            var roleIDs = _db.AspNetRoles.Where(anr => roleName.Contains(anr.Name)).Select(id => id.Id).ToList();
            //      var roleId = _db.AspNetRoles.Where(anr => anr.Name == roleNames).FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, roleIDs);
        }

        [HttpGet]
        [Route("GetUsersByRoleId")]
        public HttpResponseMessage GetUsersByRole(string roleId, int VEProfileID, int GroupID)
        {
            try
            {
                var userMachine = _db.CloudLabUsers
                          .Where(clu => clu.EmailConfirmed && !clu.isDeleted && !clu.isDisabled && clu.UserGroup == GroupID)
                          .Join(_db.MachineLabs,
                          a => a.UserId,
                          b => b.UserId,
                          (a, b) => new { a, b })
                          .Select(w=> new 
                          {
                              IsStarted = w.b.IsStarted,
                              UserId = w.b.UserId,
                              VEProfileId = w.b.VEProfileId
                          })
                          .Where(s=>s.VEProfileId == VEProfileID).ToList();

                var users = _db.CloudLabUsers
                         .Where(clu => clu.EmailConfirmed && !clu.isDeleted && !clu.isDisabled && clu.UserGroup == GroupID)
                         .GroupJoin(_db.CloudLabsSchedule,
                          a => a.UserId,
                          b => b.UserId,
                          (a, b) => new { CloudLabUsers = a, CloudLabsSchedule = b })
                          .Select(x => new
                          {
                              CloudLabsSchedule = x.CloudLabsSchedule.Where(s => s.VEProfileID == VEProfileID),
                              CloudLabUsers = x.CloudLabUsers
                          }).ToList()
                          .Join(_db.AspNetUserRoles,
                         clu => clu.CloudLabUsers.Id,
                         anur => anur.UserId,
                         (clu, anur) => new { CloudLabUsers = clu, AspNetUserRoles = anur })     
                         .Select(q => new MachineGrantsSuperAd
                         {
                             FirstName = q.CloudLabUsers.CloudLabUsers.FirstName,
                             LastName = q.CloudLabUsers.CloudLabUsers.LastName,
                             Email = q.CloudLabUsers.CloudLabUsers.Email,
                             UserId = q.CloudLabUsers.CloudLabUsers.UserId,
                             LabHoursTotal = q.CloudLabUsers.CloudLabsSchedule.Select(s => s.LabHoursTotal).SingleOrDefault(),
                             TimeRemaining = q.CloudLabUsers.CloudLabsSchedule.Select(s => s.TimeRemaining).SingleOrDefault(),
                             roleid = q.AspNetUserRoles.RoleId,
                             FullNameEmail = q.CloudLabUsers.CloudLabUsers.FirstName + " " + q.CloudLabUsers.CloudLabUsers.LastName + " " + q.CloudLabUsers.CloudLabUsers.Email,
                             hasHours = userMachine.Any(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID) ?
                             (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted >= 2) || (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted == 5) ? "Available" :
                             (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted == 3) ? "Failed" :
                             (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted == 4) ? "Provisioning" : "Available" : "dsadas",
                             //hasHours = 
                             //((q.CloudLabUsers.CloudLabsSchedule.Select(d => d.LabHoursTotal).SingleOrDefault() > 0) && (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted == 4)) ? "Provisioning" :
                             //((q.CloudLabUsers.CloudLabsSchedule.Select(d => d.LabHoursTotal).SingleOrDefault() > 0) && (userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted == 3)) ? "Failed" :
                             //(q.CloudLabUsers.CloudLabsSchedule.Select(d => d.LabHoursTotal).SingleOrDefault() > 0) ? "Granted" : "Not Granted",
                             MachineStatus = userMachine.Any(x=>x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID) ? 
                             userMachine.Where(x => x.UserId == q.CloudLabUsers.CloudLabUsers.UserId && x.VEProfileId == VEProfileID).FirstOrDefault().IsStarted : 0
                             //userMachine.Where(p=> p.UserId == q.CloudLabUsers.CloudLabUsers.UserId && p.VEProfileId == VEProfileID).FirstOrDefault().IsStarted
                         })
                         .OrderBy(q => q.FirstName)
                         .Where(g => roleId.Contains(g.roleid)).ToList();

                //foreach (var item in users)
                //{
                //    var userGrant = _db.CourseGrants.Where(q => q.VEProfileID == VEProfileID && q.UserID == item.UserId).FirstOrDefault();
                //    var userProv = _db.MachineLabs.Where(q => q.VEProfileId == VEProfileID && q.UserId == item.UserId).FirstOrDefault();

                //    if (userProv == null && userGrant == null)
                //        item.hasHours = "Available";
                //    else
                //    {
                //        if (userProv != null)
                //        {
                //            if (userProv.IsStarted == 3)
                //                item.hasHours = "Failed";
                //            else if (userProv.IsStarted == 4)
                //                item.hasHours = "Provisioning";
                //            else if (userProv.IsStarted == 0 || userProv.IsStarted == 2)
                //                item.hasHours = "Provisioned";
                //        }
                //        else if (userGrant.IsCourseGranted && userGrant != null)
                //            item.hasHours = "Granted";
                //        else
                //            item.hasHours = "Available";
                //    }

                //}
                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }

        [HttpGet]
        [Route("GetUserGroupExist")]
        public HttpResponseMessage GetUserGroupExist(int id)
        {
            try
            {
                var isUserGroupExist = _db.CloudLabUsers.Any(x => x.UserGroup == id);

                return Request.CreateResponse(HttpStatusCode.OK, isUserGroupExist);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }

        [HttpGet]
        [Route("GetAdminInstructorEmailByUserGroup")]
        public HttpResponseMessage GetAdminInstructorEmailByUserGroup()
        {
               var user =_db.AspNetRoles
                .Where(x => x.Name == "Admin" || x.Name == "Instructor")
                .Join(_db.AspNetUserRoles,
                anr => anr.Id,
                anur => anur.RoleId,
                (anr, anur) => new { AspNetRoles = anr, AspNetUserRoles = anur })
                .Join(_db.CloudLabUsers,
                idrole => idrole.AspNetUserRoles.UserId,
                iduser => iduser.Id,
                (idrole, iduser) => new { user = iduser, role = idrole })
                .Select(q => new
                {
                    Name = q.user.FirstName + " "+ q.user.LastName,
                    UserGroup = q.user.UserGroup,
                    Email = q.user.Email,
                    UserRole = q.role.AspNetRoles.Name
                }).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, user);
        }

        //[HttpGet]
        //[Route("BulkUserCreation")]
        //public HttpResponseMessage BulkUserCreation(int tenantId, string userCreator)
        //{
        //    var userGroup = _db.CloudLabsGroups.Where(x => x.TenantId == tenantId).FirstOrDefault();

        //    List<string> listA = new List<string>();
        //    using (var reader = new StreamReader(@"D:\BOOK.csv"))
        //    {
        //        var line = reader.ReadLine();
        //        while (!reader.EndOfStream)
        //        {
        //            line = reader.ReadLine();
        //            listA.Add(line);
        //        }
        //    }

        //    foreach (var item in listA)
        //    {
        //        var user = item.Split(',');
        //        CloudLabUsers userAdd = new CloudLabUsers();
        //        var roleId = _db.AspNetRoles.Where(x => x.Name == user[3]).FirstOrDefault().Id;

        //        userAdd.FirstName = user[0];
        //        userAdd.LastName = user[0];
        //        userAdd.Email = user[0];
        //        userAdd.UserName = user[0];
        //        userAdd.CreatedBy = userCreator;
        //        userAdd.DateCreated = DateTime.UtcNow;
        //        userAdd.TenantId = userGroup.TenantId;
        //        userAdd.EmailConfirmed = true;
        //        userAdd.UserGroup = userGroup.CloudLabsGroupID;
        //        _db.CloudLabUsers.Add(user);
        //    }


        //    return Request.CreateResponse(HttpStatusCode.OK, "");
        //}
        [HttpGet]
        [Route("GetAspNetRole")]
        public HttpResponseMessage GetUser()
        {
            var roles = _db.AspNetRoles.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, roles);
        }
        [HttpPost]
        [Route("UpdateAspNetRole")]
        public HttpResponseMessage UpdateUser(AspNetRoles model)
        {
            AspNetRoles role = _db.AspNetRoles.Where(x => x.Id == model.Id).FirstOrDefault();
            role.Name = model.Name;
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, role.Name);
        }

        [HttpPost]
        [Route("ConfirmEmail")]
        public bool ConfirmEmail(int userid)
        {
            try
            {
                CloudLabUsers user = _db.CloudLabUsers.Where(q => q.UserId == userid).FirstOrDefault();
                user.EmailConfirmed = !user.EmailConfirmed;
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }


        [HttpGet]
        [Route("GetUserIfUserIsValid")]
        public HttpResponseMessage GetUserIfUserIsValid(string userIdLTI)
        {
            try
            {
                var isUserActive = _db.CloudLabUsers.Where(x => x.UserIdLTI == userIdLTI).Select(q => new
                {
                    Firstname = q.FirstName,
                    LastName = q.LastName,
                    Email = q.Email,
                    Username = q.UserName,
                    EmailConfirmed = q.EmailConfirmed
                }).FirstOrDefault();

                if (isUserActive.Email.Contains("temporaryemail"))
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                else
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                //if (isUserActive.Firstname == "" && isUserActive.LastName == "")
                //    return Request.CreateResponse(HttpStatusCode.OK, true);
                //else
                //    return Request.CreateResponse(HttpStatusCode.OK, false);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
            finally
            {
                _dbCon.Close();
            }
        }
    }
}

