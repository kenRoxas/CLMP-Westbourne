using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/HeartBeat")]
    public class HeartBeatController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();
        private TenantDBContext _dbTenant = new TenantDBContext();


        [HttpGet]
        [Route("GetVMInfo")]
        public IHttpActionResult GetVMInfo(string MachineName)
        {
            double userExtensionStudentHours = 0;
            double userExtensionInstructorHours = 0;
            try
            {
                var machineLabs = db.MachineLabs.Where(q => q.MachineName == MachineName)
                    .Join(db.CloudLabUsers,
                    a => a.UserId,
                    b => b.UserId,
                    (a, b) => new { a, b })
                    .Join(db.CloudLabsGroups,
                    c => c.b.UserGroup,
                    d => d.CloudLabsGroupID,
                    (c, d) => new { c, d })
                    .Select(w => new {
                        ResourceId = w.c.a.ResourceId,
                        MachineName = w.c.a.MachineName,
                        TenantId = w.d.TenantId,
                        RunningBy = w.c.a.RunningBy,
                        VEProfileId = w.c.a.VEProfileId,
                        UserId = w.c.a.UserId
                    }).FirstOrDefault();

                var sched = db.CloudLabsSchedule.Where(q => q.UserId == machineLabs.UserId && q.VEProfileID == machineLabs.VEProfileId).FirstOrDefault();

                var tenantInfo = _dbTenant.AzTenants.Where(q => q.TenantId == machineLabs.TenantId)
                    .Join(_dbTenant.EnvironmentAPIs,
                    c => c.EnvironmentCode,
                    d => d.EnvironmentCode,
                    (c, d) => new { c, d })
                    .Select(info => new
                    {
                        TenantKey = info.c.ApplicationTenantId,
                        SubscriptionKey = info.c.SubscriptionId,
                        VMUrl = info.d.EnvironmentVMURL,
                        TenantId = info.c.TenantId
                    }).FirstOrDefault();

                var labExtension = db.LabHourExtensions.Where(q => q.UserId == machineLabs.UserId && q.VEProfileId == machineLabs.VEProfileId).ToList();

                var isUserStudentHasExtension = db.LabHourExtensions.Where(q => q.VEProfileId == machineLabs.VEProfileId && q.UserId == machineLabs.UserId && q.ExtensionTypeId == 1 && q.IsDeleted == false).ToList()
                        .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);
                var isUserInstructorHasExtension = db.LabHourExtensions.Where(q => q.VEProfileId == machineLabs.VEProfileId && q.UserId == machineLabs.UserId && q.ExtensionTypeId == 2 && q.IsDeleted == false).ToList()
                        .Any(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now);
                if (isUserStudentHasExtension)
                {
                    userExtensionStudentHours = db.LabHourExtensions.Where(q => q.VEProfileId == machineLabs.VEProfileId && q.UserId == machineLabs.UserId && q.ExtensionTypeId == 1 && q.IsDeleted == false).ToList()
                        .Where(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now).FirstOrDefault().TimeRemaining;
                }
                if (isUserInstructorHasExtension) {
                    userExtensionInstructorHours = db.LabHourExtensions.Where(q => q.VEProfileId == machineLabs.VEProfileId && q.UserId == machineLabs.UserId && q.ExtensionTypeId == 2 && q.IsDeleted == false).ToList()
                        .Where(q => q.StartDate.ToLocalTime() <= DateTime.Now && q.EndDate.ToLocalTime() > DateTime.Now).FirstOrDefault().TimeRemaining;
                }

                var vmInfo = new VMInfo
                {
                    TenantKey = tenantInfo.TenantKey,
                    SubscriptionKey = tenantInfo.SubscriptionKey,
                    ResourceId = machineLabs.ResourceId,
                    EnvironmentVMURL = tenantInfo.VMUrl,
                    RunningBy = machineLabs.RunningBy,
                    LabHourExtension = labExtension,
                    //LabHoursRemaining = sched.LabHoursRemaining
                    TimeRemaining = isUserStudentHasExtension ? userExtensionStudentHours : sched.TimeRemaining,
                    TimeRemainingInstructor = isUserInstructorHasExtension ? userExtensionInstructorHours : sched.InstructorLabHours,
                    TenantId = tenantInfo.TenantId
                };

                return Ok(vmInfo);
            }
            catch (Exception ex)
            {
                return BadRequest("Error" + ex);
            }

            finally
            {

            }
        }
    }
}