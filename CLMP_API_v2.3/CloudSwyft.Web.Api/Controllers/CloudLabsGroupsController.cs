using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CloudSwyft.Web.Api.Models;

using System.Web.Configuration;
using System.Globalization;
using System.Data.Entity.SqlServer;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.ModelConfiguration.Configuration;
using Newtonsoft.Json.Linq;
using Microsoft.Owin;
#pragma warning disable CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace
#pragma warning restore CS0105 // The using directive for 'CloudSwyft.Web.Api.Models' appeared previously in this namespace



namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/CloudLabsGroups")]
    public class CloudLabsGroupsController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();
        private TenantDBContext _dbTenant = new TenantDBContext();

        [HttpPost]
        [Route("CreateCloudLabsGroup")]
        public HttpResponseMessage CreateCloudLabsGroup(CloudLabsGroups model)
        {
            try
            {

                var tenantId = _dbTenant.AzTenants.Where(q => q.ClientCode.ToUpper() == model.CLPrefix.ToUpper()).FirstOrDefault().TenantId;

                model.TenantId = tenantId;
                db.CloudLabsGroups.Add(model);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, model);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e);
            }
            //var sqlTenant = new SqlCommand(string.Format("SELECT TenantId FROM Tenants WHERE EdxUrl = '{0}'", model.EdxUrl), _connTenant);

            //string tenantId = string.Empty;

            //_connTenant.Open();
            //using (var sqlReader = sqlTenant.ExecuteReader())
            //{
            //    while (sqlReader.Read())
            //    {
            //        tenantId = sqlReader["TenantId"].ToString();
            //    }
            //}

            //if (tenantId == "")
            //    model.TenantId = 0;
            //else
            //    model.TenantId = Int32.Parse(tenantId);

            //db.CloudLabsGroups.Add(model);
            //db.SaveChanges();
            //return Request.CreateResponse(HttpStatusCode.OK, model);
        }

        [HttpGet]
        [Route("GetCloudLabsGroup")]
        public HttpResponseMessage GetCloudLabsGroup()
        {
            //var groups = db.CloudLabsGroups.GroupJoin(db.TypeOfBusinesses, a => a.TypeOfBusinessId, b => b.Id, (a, b) => new { a, b }).ToList().Select(q => new
            //{
            //    Groups = q.a,
            //    TypeOfBusinessName = q.b.Any(w => w.Id == q.a.TypeOfBusinessId) ? q.b.Where(w=>w.Id == q.a.TypeOfBusinessId).FirstOrDefault().TypeOfBusinessName : null
            //});

            var groups = db.CloudLabsGroups.ToList();

            List<UserGroups> userGroups = new List<UserGroups>();

            foreach (var item in groups)
            {
                var oldGroup = new UserGroups();
                if (_dbTenant.AzTenants.Any(q => q.TenantId == item.TenantId))
                {
                    var eCode = _dbTenant.AzTenants.Where(q => q.TenantId == item.TenantId).FirstOrDefault().EnvironmentCode;

                    //if (typeOfBusinessArray.Count > 0)
                    //{
                    //    typeOfBusinessName = db.TypeOfBusinesses.Find(item.TypeOfBusinessId).TypeOfBusinessName;
                    //}
                    var Environment = eCode.Trim() == "D" ? "Staging" : eCode.Trim() == "Q" ? "QA" : eCode.Trim() == "U" ? "DMO" : "Prod";
                    //var Environment = eCode.Trim();
                    oldGroup.Environment = Environment;
                    oldGroup.TenantId = item.TenantId;
                    oldGroup.CLPrefix = item.CLPrefix;
                    oldGroup.CLUrl = item.CLUrl;
                    oldGroup.EdxUrl = item.EdxUrl;
                    oldGroup.GroupName = item.GroupName;
                    oldGroup.CloudLabsGroupID = item.CloudLabsGroupID;
                    oldGroup.CLPrefix = item.CLPrefix;
                    oldGroup.ApiPrefix = item.ApiPrefix;
                    //oldGroup.TypeOfBusinessName = item.TypeOfBusinessName;
                    userGroups.Add(oldGroup);
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, userGroups);
        }

        [HttpPost]
        [Route("EditCloudLabsGroup")]
        public HttpResponseMessage EditCloudLabsGroup(CloudLabsGroups model)
        {
            CloudLabsGroups VE = db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == model.CloudLabsGroupID).FirstOrDefault();
            VE.GroupName = model.GroupName;
            VE.EdxUrl = model.EdxUrl;
            VE.CLUrl = model.CLUrl;
            //only prefix changed not the tenantID para mailapat ng RG pero sama sama ang mga users.
            VE.CLPrefix = model.CLPrefix;
            VE.ApiPrefix = model.ApiPrefix;
            //VE.TypeOfBusinessId = model.TypeOfBusinessId;
            db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, VE.CloudLabsGroupID);
        }

        [HttpDelete]
        [Route("DeleteCloudLabsGroup")]
        public HttpResponseMessage DeleteCloudLabsGroup(int CloudlabsGroupID, string GroupName)
        {
            var virtualEnvironment = db.CloudLabsGroups.Find(CloudlabsGroupID);
            db.CloudLabsGroups.Remove(virtualEnvironment);

            var data = db.Database.ExecuteSqlCommand("UPDATE Cloudlabusers SET UserGroup = 0 WHERE UserGroup = '" + CloudlabsGroupID + "'");

            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, virtualEnvironment.CloudLabsGroupID);
        }

        //[HttpGet]
        //[Route("GetUserSchedulePerUserGroup")]
        //public HttpResponseMessage GetUserSchedulePerUserGroup(int CloudlabsGroupID, int VEProfileID)
        //{
        //    var users = db.CloudLabUsers
        //                   .Where(clu => clu.UserGroup == CloudlabsGroupID)
        //                   .Join(db.VirtualMachines,
        //                   x => x.UserId,
        //                   y => y.UserID,
        //                   (x, y) => new { CloudLabUsers = x, Virtualmachines = y })
        //                   .Select(s => new
        //                   {
        //                       s.CloudLabUsers,
        //                       s.Virtualmachines
        //                   })
        //                   .Join(db.CloudLabsSchedule,
        //                   q => new { q.Virtualmachines.VEProfileID, q.CloudLabUsers.UserId },
        //                   w => new { w.VEProfileID, w.UserId },
        //                   (q, w) => new { Query = q, CloudLabsSchedule = w })
        //                   .Select(d => new
        //                   {
        //                       DaysRemaining = d.Query.Virtualmachines.DateCreated,
        //                       Email = d.Query.CloudLabUsers.Email,
        //                       UserId = d.Query.CloudLabUsers.UserId,
        //                       VEProfileId = d.CloudLabsSchedule.VEProfileID,
        //                       LabHoursRemaining = d.CloudLabsSchedule.LabHoursRemaining,
        //                       LabHoursTotal = d.CloudLabsSchedule.LabHoursTotal,
        //                       Firstname = d.Query.CloudLabUsers.FirstName,
        //                       Lastname = d.Query.CloudLabUsers.LastName,
        //                       Name = d.Query.CloudLabUsers.FirstName + " " + d.Query.CloudLabUsers.LastName
        //                   })
        //                   .Where(cls => cls.VEProfileId == VEProfileID).ToList();

        //    return Request.CreateResponse(HttpStatusCode.OK, users);

        //}

        [HttpGet]
        [Route("GetCurrentUserGroup")]
        public HttpResponseMessage GetCurrentUserGroup(int userGroupId)
        {
            try
            {
                var groupCount = db.CloudLabsGroups.ToList().Count();
                object userGroup;

                var currentUserGroup = db.CloudLabsGroups.Where(cg => cg.CloudLabsGroupID == userGroupId).ToList();
                var userGroups = db.CloudLabsGroups.Where(cg => cg.CloudLabsGroupID != userGroupId).ToList();

                if (groupCount <= 2)
                {
                    if (groupCount == 1)
                    {
                        userGroup = currentUserGroup.Select(x => new
                        {
                            UserCurrentGroup = currentUserGroup.Select(q => q.GroupName).FirstOrDefault(),
                            UserGroupId = "",
                            UserGroupName = currentUserGroup.Select(q => q.GroupName).FirstOrDefault()
                        });
                    }
                    else
                    {
                        userGroup = userGroups.Select(x => new
                        {
                            UserCurrentGroup = currentUserGroup.Select(q => q.GroupName).FirstOrDefault(),
                            UserGroupId = x.CloudLabsGroupID,
                            UserGroupName = x.GroupName
                        });
                    }
                }

                else
                {
                    userGroup = userGroups.Select(x => new
                    {
                        UserCurrentGroup = currentUserGroup.Where(r => r.CloudLabsGroupID == userGroupId).Select(y => y.GroupName).FirstOrDefault(),
                        UserGroupId = x.CloudLabsGroupID,
                        UserGroupName = x.GroupName
                    }).ToList();
                }
                return Request.CreateResponse(HttpStatusCode.OK, userGroup);

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);

            }
        }

        [HttpPost]
        [Route("ChangeUserGroup")]
        public HttpResponseMessage ChangeUserGroup(string userGroup, string userid)
        {
            var userGroupId = db.CloudLabsGroups.Where(x => x.GroupName == userGroup).FirstOrDefault();

            CloudLabUsers clu = db.CloudLabUsers.Where(x => x.Id == userid).FirstOrDefault();
            clu.UserGroup = userGroupId.CloudLabsGroupID;
            clu.TenantId = userGroupId.TenantId;
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, clu);

        }

        [HttpGet]
        [Route("GetVEProfilesBasedOnUserGroup")]
        public HttpResponseMessage GetVEProfilesBasedOnUserGroup(int userGroupId)
        {
            var profiles = db.VEProfileLabCreditMappings
                .Where(VEPLCM => VEPLCM.GroupID == userGroupId)
                .Join(db.VEProfiles,
                x => x.VEProfileID,
                y => y.VEProfileID,
                (x, y) => new { VEProfileLabCreditMappings = x, VEProfiles = y })
                .Select(s => new
                {
                    VEProfileID = s.VEProfileLabCreditMappings.VEProfileID,
                    GroupID = s.VEProfileLabCreditMappings.GroupID
                }).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, profiles);
        }

        [HttpGet]
        [Route("CheckTotalSubscriptionCredit")]
        public HttpResponseMessage CheckTotalSubscriptionCredit(int CloudLabsGroupID)
        {
            CloudLabsGroups record = db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == CloudLabsGroupID).FirstOrDefault();

            CloudLabsGroups credits = new CloudLabsGroups
            {
                //SubscriptionHourCredits = record.SubscriptionHourCredits / 60,
                SubscriptionHourCredits = record.SubscriptionHourCredits,
                SubscriptionRemainingHourCredits = record.SubscriptionRemainingHourCredits
                //SubscriptionRemainingHourCredits = record.SubscriptionRemainingHourCredits / 60
            };

            return Request.CreateResponse(HttpStatusCode.OK, credits);
        }

        [HttpGet]
        [Route("GetUserGroupName")]
        public HttpResponseMessage GetUserGroupName(int CloudLabsGroupID)
        {
            var groupName = db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == CloudLabsGroupID).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, groupName.GroupName);
        }

        [HttpGet]
        [Route("GetUserGroupCode")]
        public HttpResponseMessage GetUserGroupCode(int CloudLabsGroupID)
        {
            var groupName = db.CloudLabsGroups.Where(x => x.CloudLabsGroupID == CloudLabsGroupID).FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, groupName.CLPrefix);
        }

        [HttpGet]
        [Route("GetUserGroupAzCode")]
        public HttpResponseMessage GetUserGroupAzCode(string environment, string currentCode = null)
        {
            var prefix = _dbTenant.AzTenants.Where(q => q.EnvironmentCode.ToUpper() == environment.ToUpper()).Select(x => x.ClientCode.ToUpper()).ToList();

            List<string> code = new List<string>();

            foreach (var item in prefix)
            {
                if (!db.CloudLabsGroups.Any(q => q.CLPrefix.ToUpper() == item))
                    code.Add(item);
            }

            if (currentCode != null && currentCode != "undefined")
                code.Add(currentCode);

            return Request.CreateResponse(HttpStatusCode.OK, code);
        }

        [HttpGet]
        [Route("GetUserSchedulePerUserGroup")]
        public HttpResponseMessage GetUserSchedulePerUserGroup(int CloudlabsGroupID, int VEProfileID)
        {
            try
            {
                
                var tenantId = db.CloudLabsGroups.Where(q => q.CloudLabsGroupID == CloudlabsGroupID).FirstOrDefault().TenantId;
                var isExtendAble = db.BusinessGroups.Any(q => q.UserGroupId == CloudlabsGroupID);
                var BusinessId = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault().BusinessId;

                int? validity = 0;

                if (isExtendAble)
                {
                    if (db.BusinessGroups.Where(q => q.UserGroupId == CloudlabsGroupID).FirstOrDefault().ModifiedValidity != null)
                        validity = db.BusinessGroups.Where(q => q.UserGroupId == CloudlabsGroupID).FirstOrDefault().ModifiedValidity;
                    else
                        validity = db.BusinessTypes.Where(q => q.BusinessId == BusinessId).FirstOrDefault().Validity;
                }
                else
                    validity = db.BusinessTypes.Where(q => q.BusinessId == BusinessId).FirstOrDefault().Validity;


                var users = db.CloudLabUsers
                               .Where(clu => clu.UserGroup == CloudlabsGroupID)
                               .Join(db.MachineLabs,
                               x => x.UserId,
                               y => y.UserId,
                               (x, y) => new { CloudLabUsers = x, MachineLabs = y })
                               .Select(s => new
                               {
                                   s.CloudLabUsers,
                                   s.MachineLabs
                               })
                               .Join(db.CloudLabsSchedule,
                               q => q.MachineLabs.MachineLabsId,
                               w => w.MachineLabsId,
                               (q, w) => new { Query = q, CloudLabsSchedule = w })
                               .Where(cls => cls.CloudLabsSchedule.VEProfileID == VEProfileID)
                               .Select(d => new SchedulesPerUserGroup
                               {
                                   //DaysRemaining = d.Query.MachineLabs.DateProvision,
                                   //DaysRemaining = (validity - DbFunctions.DiffDays(DateTime.Parse(d.Query.MachineLabs.DateProvision), DateTime.UtcNow)) <= 0 ? 0 : validity - DbFunctions.DiffDays(DateTime.Parse(d.Query.MachineLabs.DateProvision), DateTime.UtcNow),
                                   Email = d.Query.CloudLabUsers.Email,
                                   UserId = d.Query.CloudLabUsers.UserId,
                                   VEProfileId = d.CloudLabsSchedule.VEProfileID,
                                   TimeRemaining = d.CloudLabsSchedule.TimeRemaining,
                                   LabHoursTotal = d.CloudLabsSchedule.LabHoursTotal,
                                   Firstname = d.Query.CloudLabUsers.FirstName,
                                   Lastname = d.Query.CloudLabUsers.LastName,
                                   DateProvision =d.Query.MachineLabs.DateProvision,
                                   Name = d.Query.CloudLabUsers.FirstName + " " + d.Query.CloudLabUsers.LastName, 
                                   FullNameEmail = d.Query.CloudLabUsers.FirstName + " " + d.Query.CloudLabUsers.LastName + " " + d.Query.CloudLabUsers.Email
                               }).ToList();

                foreach (var item in users)
                {
                    var days = DateTime.ParseExact(item.DateProvision, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                    var DaysRemaining = (validity - Math.Round(DateTime.UtcNow.Subtract(days).TotalDays) <= 0 ? 0 : validity - Math.Round(DateTime.UtcNow.Subtract(days).TotalDays));

                    item.DaysRemaining = DaysRemaining;
                }

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Issue");
            }
        }

        [HttpGet]
        [Route("GroupLabs")]
        public IHttpActionResult GroupLabs(int CloudLabsGroupId)
        {
            var query = db.VEProfiles
                          .GroupJoin(db.MachineLabs,
                           a => a.VEProfileID,
                           b => b.VEProfileId,
                           (a, b) => new { VEProfiles = a, VirtualMachineMappings = b })
                           .Select(x => new
                           {
                               CourseDetails = x
                           })
                           .Join(db.VirtualEnvironments,
                           e => e.CourseDetails.VEProfiles.VirtualEnvironmentID,
                           f => f.VirtualEnvironmentID,
                           (e, f) => new { e, f })
                            .Join(db.VEProfileLabCreditMappings,
                           q => q.e.CourseDetails.VEProfiles.VEProfileID,
                           w => w.VEProfileID,
                           (q, w) => new { CourseDetails = q, VEProfileLabCreditMappings = w })
                           .Select(z => new
                           {
                               z.CourseDetails,
                               z.VEProfileLabCreditMappings

                           })
                           .Join(db.CloudLabsGroups,
                           g => g.VEProfileLabCreditMappings.GroupID,
                           h => h.CloudLabsGroupID,
                           (g, h) => new { VEProfileCourseDetails = g, CloudLabsGroups = h })
                           .Select(k => new
                           {
                               k
                           })
                           .Join(db.VirtualEnvironmentImages,
                           i => i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VirtualEnvironmentID,
                           j => j.VirtualEnvironmentID, (i,j) => new { i, j}).Where(pp=>pp.j.GroupId == CloudLabsGroupId)
                           .Where(f => f.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID == CloudLabsGroupId).ToList()
                          .OrderBy(y => y.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.Name)
                          .Select((x, index) => new
                          {
                              Count = index + 1,
                              CourseName = x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.Name,
                              VEProfileId = x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID,
                              VirtualEnvironmentId = x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VirtualEnvironmentID,
                              ProvisionCount = x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VirtualMachineMappings.Where(w => w.VEProfileId == x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID & w.IsStarted != 5).Count(),
                              //CourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours / 60,
                              CourseHours = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours,
                              NumberOfUsers = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.NumberOfUsers,
                              //TotalCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours / 60,
                              TotalCourseHours = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours,
                              TotalRemainingCourseHours = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingCourseHours,
                              TotalRemainingContainers = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingContainers,
                              GroupId = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID,
                              SubscriptionHourCredits = x.i.k.CloudLabsGroups.SubscriptionHourCredits,
                              SubscriptionRemainingHourCredits = x.i.k.CloudLabsGroups.SubscriptionRemainingHourCredits,
                              DisabledCourse = x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VirtualMachineMappings.Where(w => w.VEProfileId == x.i.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID & w.IsStarted != 5).Count() > 0
                                                ? true : false,
                              DisabledUsers = false,
                              VETypeID = x.i.k.VEProfileCourseDetails.CourseDetails.f.VETypeID,
                              MachineSize = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.MachineSize,
                              DiskSize = x.i.k.VEProfileCourseDetails.VEProfileLabCreditMappings.DiskSize,
                              DiskSizeMin = x.j.ImageFamilyMinDiskSize
                          }).ToList();

            var queryConsole = db.VEProfiles
                          .GroupJoin(db.MachineLabs,
                           a => a.VEProfileID,
                           b => b.VEProfileId,
                           (a, b) => new { VEProfiles = a, VirtualMachineMappings = b })
                           .Select(x => new
                           {
                               CourseDetails = x
                           })
                           .Join(db.VirtualEnvironments,
                           e => e.CourseDetails.VEProfiles.VirtualEnvironmentID,
                           f => f.VirtualEnvironmentID,
                           (e, f) => new { e, f })
                            .Join(db.VEProfileLabCreditMappings,
                           q => q.e.CourseDetails.VEProfiles.VEProfileID,
                           w => w.VEProfileID,
                           (q, w) => new { CourseDetails = q, VEProfileLabCreditMappings = w })
                           .Select(z => new
                           {
                               z.CourseDetails,
                               z.VEProfileLabCreditMappings

                           })
                           .Join(db.CloudLabsGroups,
                           g => g.VEProfileLabCreditMappings.GroupID,
                           h => h.CloudLabsGroupID,
                           (g, h) => new { VEProfileCourseDetails = g, CloudLabsGroups = h })
                           .Select(k => new
                           {
                               k
                           }).Where(f => f.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID == CloudLabsGroupId).ToList()
                          .OrderBy(y => y.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.Name)
                          .Where(p=>p.k.VEProfileCourseDetails.CourseDetails.f.VETypeID == 11)
                          .Select((x, index) => new
                          {
                              Count = index + 1,
                              CourseName = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.Name,
                              VEProfileId = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID,
                              VirtualEnvironmentId = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VirtualEnvironmentID,
                              ProvisionCount = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VirtualMachineMappings.Where(w => w.VEProfileId == x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID & w.IsStarted != 5).Count(),
                              CourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours,
                              NumberOfUsers = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.NumberOfUsers,
                              TotalCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours,
                              TotalRemainingCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingCourseHours,
                              TotalRemainingContainers = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingContainers,
                              GroupId = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID,
                              SubscriptionHourCredits = x.k.CloudLabsGroups.SubscriptionHourCredits,
                              SubscriptionRemainingHourCredits = x.k.CloudLabsGroups.SubscriptionRemainingHourCredits,
                              DisabledCourse = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VirtualMachineMappings.Where(w => w.VEProfileId == x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID & w.IsStarted != 5).Count() > 0
                                                ? true : false,
                              DisabledUsers = false,
                              VETypeID = x.k.VEProfileCourseDetails.CourseDetails.f.VETypeID,
                              MachineSize = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.MachineSize,
                              DiskSize = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.DiskSize,
                              DiskSizeMin = ""
                          }).ToList();

            if(queryConsole != null)
            {
                query.AddRange(queryConsole);
            }
            return Ok(query);

        }

        [HttpPost]
        [Route("SaveGroupLabs")]
        public IHttpActionResult SaveGroupLabs(int subscriptionRemaining, List<VEProfileCreditGroups> model)
        {
            foreach (var t in model)
            {
                CloudLabsGroups CLGmodel = db.CloudLabsGroups.Where(q => q.CloudLabsGroupID == t.GroupId).FirstOrDefault();
                VEProfileLabCreditMappings VEmodel = db.VEProfileLabCreditMappings.Where(x => x.GroupID == t.GroupId && x.VEProfileID == t.VEProfileId).FirstOrDefault();
                VEmodel.CourseHours = t.CourseHours;
                //VEmodel.CourseHours = t.CourseHours * 60;
                VEmodel.NumberOfUsers = t.NumberOfUsers;
                if (t.MachineSize == null)
                    VEmodel.DiskSize = null;
                else
                    VEmodel.DiskSize = t.DiskSize;
                VEmodel.TotalCourseHours = t.TotalCourseHours;
                //VEmodel.TotalCourseHours = t.TotalCourseHours * 60;
                VEmodel.TotalRemainingContainers = t.ProvisionCount + VEmodel.TotalRemainingContainers;
                VEmodel.MachineSize = t.MachineSize;
                if (VEmodel.TotalCourseHours == t.TotalCourseHours)
                    VEmodel.TotalRemainingCourseHours = t.TotalRemainingCourseHours;
                else
                    VEmodel.TotalRemainingCourseHours += t.TotalRemainingCourseHours;
                //CLGmodel.SubscriptionHourCredits = t.SubscriptionHourCredits * 60;
                //CLGmodel.SubscriptionRemainingHourCredits = subscriptionRemaining * 60;
                CLGmodel.SubscriptionRemainingHourCredits = subscriptionRemaining;

                db.SaveChanges();

            }

            return Ok();
        }

        [HttpGet]
        [Route("DataUsers")]
        public IHttpActionResult DataUsers(int CloudlabsGroupID, int VEProfileID)
        {
            TextInfo info = CultureInfo.CurrentCulture.TextInfo;
            try
            {
                var tenantId = db.CloudLabsGroups.Where(q => q.CloudLabsGroupID == CloudlabsGroupID).FirstOrDefault().TenantId;
                var isExtendAble = db.BusinessGroups.Any(q => q.UserGroupId == CloudlabsGroupID); 
                var BusinessId = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault().BusinessId;

                int? validity = 0;

                if (isExtendAble)
                {
                    if (db.BusinessGroups.Where(q => q.UserGroupId == CloudlabsGroupID).FirstOrDefault().ModifiedValidity != null)
                        validity = db.BusinessGroups.Where(q => q.UserGroupId == CloudlabsGroupID).FirstOrDefault().ModifiedValidity;
                    else
                        validity = db.BusinessTypes.Where(q => q.BusinessId == BusinessId).FirstOrDefault().Validity;
                }
                else
                    validity = db.BusinessTypes.Where(q => q.BusinessId == BusinessId).FirstOrDefault().Validity;


                var users = db.CloudLabUsers
                    .Join(db.MachineLabs,
                    x => x.UserId,
                    y => y.UserId,
                    (x, y) => new { CloudLabUsers = x, machinelabs = y })
                    .Select(s => new
                    {
                        s.CloudLabUsers,
                        s.machinelabs
                    })
                    .Join(db.CloudLabsSchedule,
                    g => new { g.machinelabs.MachineLabsId },
                    w => new { w.MachineLabsId },
                    (g, w) => new { Query = g, CloudLabsSchedule = w })
                    .Select(d => new
                    {
                        d
                    })
                    .Where(cls => cls.d.CloudLabsSchedule.VEProfileID == VEProfileID && cls.d.Query.CloudLabUsers.UserGroup == CloudlabsGroupID)
                    .ToList()
                    .OrderBy(w => w.d.Query.CloudLabUsers.FirstName + " " + w.d.Query.CloudLabUsers.LastName)
                    .Select((k, index) => new UsersData
                    {
                        Count = index + 1,
                        //DaysRemaining = 180 - Math.Ceiling((DateTime.Today - k.d.Query.Virtualmachines.DateCreated).TotalDays) + " Days",
                        //DaysRemaining = k.d.Query.machinelabs.DateProvision,
                        //ConsumedLabHours = Math.Floor((double)(k.d.CloudLabsSchedule.LabHoursTotal * 60 - k.d.CloudLabsSchedule.TimeRemaining) / 60) + " h " + Math.Floor((double)(k.d.CloudLabsSchedule.LabHoursTotal * 60 - k.d.CloudLabsSchedule.TimeRemaining) % 3600 / 60) + " m ",
                        ConsumedLabHours = (TimeSpan.FromHours((double)k.d.CloudLabsSchedule.LabHoursTotal) - TimeSpan.FromSeconds((double)k.d.CloudLabsSchedule.TimeRemaining)).Hours + " h " + (TimeSpan.FromHours((double)k.d.CloudLabsSchedule.LabHoursTotal) - TimeSpan.FromSeconds((double)k.d.CloudLabsSchedule.TimeRemaining)).Minutes + " m ",//Math.Floor((double)((k.d.CloudLabsSchedule.LabHoursTotal / 3600) - (k.d.CloudLabsSchedule.TimeRemaining) / 3600)) + " h " + Math.Floor((double)(60 - (k.d.CloudLabsSchedule.TimeRemaining) % 3600 / 60)) + " m ",
                        //RemainingLabHours = TimeSpan.FromSeconds((double)k.d.CloudLabsSchedule.TimeRemaining).Hours + " h " + TimeSpan.FromSeconds((double)k.d.CloudLabsSchedule.TimeRemaining).Minutes + " m ", //k.d.CloudLabsSchedule.TimeRemaining > 0 ? Math.Floor((double)(k.d.CloudLabsSchedule.TimeRemaining / 3600)) + " h " + Math.Floor((double)k.d.CloudLabsSchedule.TimeRemaining % 3600 / 60) + " m " : 0 + " h " + 0 + " m ",
                        RemainingLabHours = TimeSpan.FromSeconds((double)k.d.CloudLabsSchedule.TimeRemaining).Hours + " h " + TimeSpan.FromSeconds((double)k.d.CloudLabsSchedule.TimeRemaining).Minutes + " m ", //k.d.CloudLabsSchedule.TimeRemaining > 0 ? Math.Floor((double)(k.d.CloudLabsSchedule.TimeRemaining / 3600)) + " h " + Math.Floor((double)k.d.CloudLabsSchedule.TimeRemaining % 3600 / 60) + " m " : 0 + " h " + 0 + " m ",
                        Name = info.ToTitleCase(k.d.Query.CloudLabUsers.LastName) + ", " + info.ToTitleCase(k.d.Query.CloudLabUsers.FirstName),
                        Email = k.d.Query.CloudLabUsers.Email.ToLower(),
                        DateProvision = k.d.Query.machinelabs.DateProvision
                    }).ToList();

                foreach (var item in users)
                {
                    //var DaysRemaining = (validity - Math.Round(DateTime.UtcNow.Subtract(DateTime.Parse(item.DateProvision)).TotalDays) <= 0 ? 0 : validity - Math.Round(DateTime.UtcNow.Subtract(DateTime.Parse(item.DateProvision)).TotalDays));
                    var days = DateTime.ParseExact(item.DateProvision, "dd/MM/yyyy HH:mm:ss tt", CultureInfo.InvariantCulture);
                    var Days = (validity - Math.Round(DateTime.UtcNow.Subtract(days).TotalDays) <= 0 ? 0 : validity - Math.Round(DateTime.UtcNow.Subtract(days).TotalDays));

                    //var Days = (validity - Math.Round(DateTime.UtcNow.Subtract(DateTime.Parse(item.DateProvision)).TotalDays) <= 0 ? 0 : validity - Math.Round(DateTime.UtcNow.Subtract(DateTime.Parse(item.DateProvision)).TotalDays));

                    item.DaysRemaining = Days;

                }

                return Ok(users);
            }
            catch(Exception e)
            {
                return Ok("Issue");

            }


        }

        [HttpGet]
        [Route("DataCourse")]
        public IHttpActionResult DataCourse(int CloudLabsGroupId, bool isSuperAdmin)
        {
            List<object> que = new List<object>();

            var query = db.VEProfiles
                          .GroupJoin(db.MachineLabs,
                           a => a.VEProfileID,
                           b => b.VEProfileId,
                           (a, b) => new { VEProfiles = a, VirtualMachineMappings = b })
                           .Select(x => new
                           {
                               CourseDetails = x
                           })
                           .Join(db.VEProfileLabCreditMappings,
                           q => q.CourseDetails.VEProfiles.VEProfileID,
                           w => w.VEProfileID,
                           (q, w) => new { CourseDetails = q, VEProfileLabCreditMappings = w })
                           .Select(z => new
                           {
                               z.CourseDetails,
                               z.VEProfileLabCreditMappings
                           })
                           .Join(db.CloudLabsGroups,
                           g => g.VEProfileLabCreditMappings.GroupID,
                           h => h.CloudLabsGroupID,
                           (g, h) => new { VEProfileCourseDetails = g, CloudLabsGroups = h })
                           .Select(k => new
                           {
                               k
                           })
                           .Join(db.VirtualEnvironments, l=>l.k.VEProfileCourseDetails.CourseDetails.CourseDetails.VEProfiles.VirtualEnvironmentID, m=> m.VirtualEnvironmentID, (l,m) => new { l,m})
                           .Where(f => f.l.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID == CloudLabsGroupId).ToList()
                          .OrderBy(y => y.l.k.VEProfileCourseDetails.CourseDetails.CourseDetails.VEProfiles.Name)
                          .Select((x, index) => new
                          {
                              Count = index + 1,
                              CourseName = x.l.k.VEProfileCourseDetails.CourseDetails.CourseDetails.VEProfiles.Name,
                              //CourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours / 60 + " Hrs",
                              CourseHours = x.l.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours + " Hrs",
                              NumberOfUsers = x.l.k.VEProfileCourseDetails.VEProfileLabCreditMappings.NumberOfUsers,
                              TotalCourseHours = x.l.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours + " Hrs",
                             // TotalCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours / 60 + " Hrs",
                              TotalRemainingCourseHours = x.l.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingCourseHours + " Hrs",
                              VEType = x.m.VETypeID
                              //TotalRemainingCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingCourseHours / 60 + " Hrs",
                          }).ToList();

            if (!isSuperAdmin)
            {
                foreach (var item in query)
                {
                    if (item.VEType != 3 && item.VEType != 4)
                        que.Add(item);
                }
                return Ok(que);
            }
            else
                return Ok(query);

        }

        [HttpGet]
        [Route("GetUserByUserGroupId")]
        public HttpResponseMessage GetUserByUserGroupId(int CloudlabsGroupID)
        {
            try
            {

                var users = db.CloudLabUsers.Join(db.CloudLabsGroups, a => a.UserGroup, b => b.CloudLabsGroupID, (a, b) => new { a, b }).Where(q => q.b.CloudLabsGroupID == CloudlabsGroupID)
                    .Join(db.AspNetUserRoles, c => c.a.Id, d => d.UserId, (c, d) => new { c, d }).Join(db.AspNetRoles, e => e.d.RoleId, f => f.Id, (e, f) => new { e, f })
                    .Select(w => new UsersByUserGroup
                    {
                        Email = w.e.c.a.Email,
                        Firstname = w.e.c.a.FirstName,
                        Lastname = w.e.c.a.LastName,
                        GroupName = w.e.c.b.GroupName,
                        Role = w.f.Name,
                        DateCreated = w.e.c.a.DateCreated
                    }).Where(q=>q.Role != "SuperAdmin").ToList();

                return Request.CreateResponse(HttpStatusCode.OK, users);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, e.Message);
            }
        }
    }
}