using System.Linq;
using System.Web.Http;
using CloudSwyft.Web.Api.Models;
using System.Net.Http;
using System.Data.Entity;
using System.Collections.Generic;
using System;

namespace CloudSwyft.Web.Api.Controllers
{

    //[RoutePrefix("api/VirtualMachineMappings")]
    //public class VirtualMachineMappingsController : ApiController
    //{
    //    private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();

    //    //[HttpDelete]
    //    //[Route("DeleteVmMapping")]
    //    //public IHttpActionResult DeleteVirtualMachineMappings(int courseId, int userId, int veProfileId, int machineInstance)
    //    //{
    //    //    var vmMappings = _db.VirtualMachineMappings.SingleOrDefault(vm => vm.CourseID == courseId && vm.UserID == userId && vm.VEProfileID == veProfileId && vm.MachineInstance == machineInstance);
    //    //    if (vmMappings == null)
    //    //    {
    //    //        return NotFound();
    //    //    }

    //    //    _db.VirtualMachineMappings.Remove(vmMappings);
    //    //    _db.SaveChanges();

    //    //    return Ok("Virtual machine mapping deleted.");
    //    //}

    //    //[HttpGet]
    //    //[Route("GetVmMapping")]
    //    //public IHttpActionResult GetVirtualMachineMappings(int courseId, int userId, int veProfileId, int machineInstance)
    //    //{
    //    //    var vmMappings = _db.VirtualMachineMappings.SingleOrDefault(vm => vm.CourseID == courseId && vm.UserID == userId && vm.VEProfileID == veProfileId && vm.MachineInstance == machineInstance);
    //    //    if (vmMappings == null)
    //    //    {
    //    //        return NotFound();
    //    //    }

    //    //    return Ok(vmMappings);
    //    //}

    //    [HttpGet]
    //    [Route("GetVmMapping")]
    //    public IHttpActionResult GetVirtualMachineMappings(string roleName)
    //    {
    //        var vmMappings = _db.VirtualMachineMappings.SingleOrDefault(vm => vm.VMName == roleName);
    //        if (vmMappings == null)
    //        {
    //            return NotFound();
    //        }

    //        return Ok(vmMappings);
    //    }

    //    [HttpPost]
    //    [Route("FinishProvisioning")]
    //    public IHttpActionResult FinishProvisioning(CloudLabsSchedule model)
    //    {
    //        VirtualMachineMappings data = _db.VirtualMachineMappings.Where(x => x.UserID == model.UserId && x.VEProfileID == model.VEProfileID).FirstOrDefault();
    //        data.IsProvisioned = 1;
    //        _db.SaveChanges();
    //        return Ok();
    //    }

    //    //[HttpPost]
    //    //[Route("FinishProvisioningLaaS")]
    //    //public IHttpActionResult FinishProvisioningLaaS(CloudLabsSchedule model)
    //    //{
    //    //    VirtualMachineMappings data = _db.VirtualMachineMappings.Where(x => x.UserID == model.UserId && x.VEProfileID == model.VEProfileID).FirstOrDefault();
    //    //    data.IsLaasProvisioned = 1;
    //    //    _db.SaveChanges();
    //    //    return Ok();
    //    //}

    //    [HttpGet]
    //    [Route("GetVirtualMachineMappingsEntry")]
    //    public IHttpActionResult GetVirtualMachineMappingsEntry()
    //    {
    //        try
    //        {

    //            var vmMappings = _db.VirtualMachineMappings.Where(x => x.IsProvisioned == 0).ToList();
    //            return Ok(vmMappings.Count);
    //        }
    //        catch(Exception e)
    //        {
    //            return BadRequest(e.Message);
    //        }
    //    }

    //    [HttpGet]
    //    [Route("GroupLabs")]
    //    public IHttpActionResult GroupLabs(int CloudLabsGroupId)
    //    {
    //        var i = 0;
    //        var query = _db.VEProfiles
    //                      .GroupJoin(_db.VirtualMachineMappings,
    //                       a => a.VEProfileID,
    //                       b => b.VEProfileID,
    //                       (a, b) => new { VEProfiles = a, VirtualMachineMappings = b })
    //                       .Select(x => new
    //                       {
    //                           CourseDetails = x
    //                       })
    //                       .Join(_db.VirtualEnvironments,
    //                       e => e.CourseDetails.VEProfiles.VirtualEnvironmentID,
    //                       f => f.VirtualEnvironmentID,
    //                       (e, f) => new { e, f })
    //                        .Join(_db.VEProfileLabCreditMappings,
    //                       q => q.e.CourseDetails.VEProfiles.VEProfileID,
    //                       w => w.VEProfileID,
    //                       (q, w) => new { CourseDetails = q, VEProfileLabCreditMappings = w })
    //                       .Select(z => new
    //                       {
    //                           z.CourseDetails,
    //                           z.VEProfileLabCreditMappings

    //                       })
    //                       .Join(_db.CloudLabsGroups,
    //                       g => g.VEProfileLabCreditMappings.GroupID,
    //                       h => h.CloudLabsGroupID,
    //                       (g, h) => new { VEProfileCourseDetails = g, CloudLabsGroups = h })
    //                       .Select(k => new
    //                       {
    //                           k
    //                       })
    //                       .Where(f => f.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID == CloudLabsGroupId).ToList()

    //                      .OrderBy(y => y.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.Name)
    //                      .Select((x, index) => new {
    //                          Count = index + 1,
    //                          CourseName = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.Name,
    //                          VEProfileId = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID,
    //                          VirtualEnvironmentId = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VirtualEnvironmentID,
    //                          ProvisionCount = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VirtualMachineMappings.Where(w => w.VEProfileID == x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID & w.IsProvisioned == 1).Count(),
    //                          CourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours / 60,
    //                          NumberOfUsers = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.NumberOfUsers,
    //                          TotalCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours / 60,
    //                          TotalRemainingCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingCourseHours,
    //                          TotalRemainingContainers = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingContainers,
    //                          GroupId = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID,
    //                          SubscriptionHourCredits = x.k.CloudLabsGroups.SubscriptionHourCredits,
    //                          SubscriptionRemainingHourCredits = x.k.CloudLabsGroups.SubscriptionRemainingHourCredits,
    //                          DisabledCourse = x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VirtualMachineMappings.Where(w => w.VEProfileID == x.k.VEProfileCourseDetails.CourseDetails.e.CourseDetails.VEProfiles.VEProfileID & w.IsProvisioned == 1).Count() > 0
    //                                            ? true : false,
    //                          DisabledUsers = false,
    //                          VETypeID = x.k.VEProfileCourseDetails.CourseDetails.f.VETypeID

    //                      }).ToList();

    //        return Ok(query);

    //    }

    //    [HttpPost]
    //    [Route("SaveGroupLabs")]
    //    public IHttpActionResult SaveGroupLabs(int subscriptionRemaining, List<VEProfileCreditGroups> model)
    //    {
    //        foreach (var t in model)
    //        {
    //            CloudLabsGroups CLGmodel = _db.CloudLabsGroups.Where(q => q.CloudLabsGroupID == t.GroupId).FirstOrDefault();
    //            VEProfileLabCreditMappings VEmodel = _db.VEProfileLabCreditMappings.Where(x => x.GroupID == t.GroupId && x.VEProfileID == t.VEProfileId).FirstOrDefault();
    //            VEmodel.CourseHours = t.CourseHours * 60;
    //            VEmodel.NumberOfUsers = t.NumberOfUsers;
    //            VEmodel.TotalCourseHours = t.TotalCourseHours * 60;
    //            VEmodel.TotalRemainingContainers = t.ProvisionCount + VEmodel.TotalRemainingContainers;
    //            if (VEmodel.TotalCourseHours == t.TotalCourseHours * 60)
    //                VEmodel.TotalRemainingCourseHours = t.TotalRemainingCourseHours;
    //            else
    //                VEmodel.TotalRemainingCourseHours += t.TotalRemainingCourseHours;
    //            //CLGmodel.SubscriptionHourCredits = t.SubscriptionHourCredits * 60;
    //            CLGmodel.SubscriptionRemainingHourCredits = subscriptionRemaining * 60;
                
    //            _db.SaveChanges();

    //        }

    //        return Ok();
    //    }

    //    [HttpGet]
    //    [Route("DataCourse")]
    //    public IHttpActionResult DataCourse(int CloudLabsGroupId)
    //    {
    //        var i = 0;
    //        var query = _db.VEProfiles
    //                      .GroupJoin(_db.VirtualMachineMappings,
    //                       a => a.VEProfileID,
    //                       b => b.VEProfileID,
    //                       (a, b) => new { VEProfiles = a, VirtualMachineMappings = b })
    //                       .Select(x => new
    //                       {
    //                           CourseDetails = x
    //                       })
    //                       .Join(_db.VEProfileLabCreditMappings,
    //                       q => q.CourseDetails.VEProfiles.VEProfileID,
    //                       w => w.VEProfileID,
    //                       (q, w) => new { CourseDetails = q, VEProfileLabCreditMappings = w })
    //                       .Select(z => new
    //                       {
    //                           z.CourseDetails,
    //                           z.VEProfileLabCreditMappings

    //                       })
    //                       .Join(_db.CloudLabsGroups,
    //                       g => g.VEProfileLabCreditMappings.GroupID,
    //                       h => h.CloudLabsGroupID,
    //                       (g, h) => new { VEProfileCourseDetails = g, CloudLabsGroups = h })
    //                       .Select(k => new
    //                       {
    //                           k
    //                       })
    //                       .Where(f => f.k.VEProfileCourseDetails.VEProfileLabCreditMappings.GroupID == CloudLabsGroupId).ToList()
    //                      .OrderBy(y => y.k.VEProfileCourseDetails.CourseDetails.CourseDetails.VEProfiles.Name)
    //                      .Select((x, index) => new {
    //                          Count = index + 1,
    //                          CourseName = x.k.VEProfileCourseDetails.CourseDetails.CourseDetails.VEProfiles.Name,
    //                          CourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.CourseHours / 60 + " Hrs",
    //                          NumberOfUsers = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.NumberOfUsers,
    //                          TotalCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalCourseHours / 60 + " Hrs",
    //                          TotalRemainingCourseHours = x.k.VEProfileCourseDetails.VEProfileLabCreditMappings.TotalRemainingCourseHours / 60+ " Hrs",
    //                      }).ToList();

    //        return Ok(query);
    //    }

    //    [HttpGet]
    //    [Route("DataUsers")]
    //    public IHttpActionResult DataUsers(int CloudlabsGroupID, int VEProfileID)
    //    {
    //        var users = _db.CloudLabUsers
    //            .Join(_db.VirtualMachines,
    //            x => x.UserId,
    //            y => y.UserID,
    //            (x, y) => new { CloudLabUsers = x, Virtualmachines = y })
    //            .Select(s => new
    //            {
    //                s.CloudLabUsers,
    //                s.Virtualmachines
    //            })
    //            .Join(_db.CloudLabsSchedule,
    //            g => new { g.Virtualmachines.VEProfileID, g.CloudLabUsers.UserId },
    //            w => new { w.VEProfileID, w.UserId },
    //            (g, w) => new { Query = g, CloudLabsSchedule = w })
    //            .Select(d => new
    //            {
    //                d
    //            })
    //            .Where(cls => cls.d.CloudLabsSchedule.VEProfileID == VEProfileID && cls.d.Query.CloudLabUsers.UserGroup == CloudlabsGroupID)
    //            .ToList()
    //            .OrderBy(w=>w.d.Query.CloudLabUsers.FirstName + " " + w.d.Query.CloudLabUsers.LastName)
    //            .Select((k, index) => new {
    //                Count = index + 1,
    //                //DaysRemaining = 180 - Math.Ceiling((DateTime.Today - k.d.Query.Virtualmachines.DateCreated).TotalDays) + " Days",
    //                DaysRemaining = k.d.Query.Virtualmachines.DateCreated,
    //                ConsumedLabHours = Math.Floor((k.d.CloudLabsSchedule.LabHoursTotal - k.d.CloudLabsSchedule.LabHoursRemaining) / 60) + " h " + Math.Floor((k.d.CloudLabsSchedule.LabHoursTotal - k.d.CloudLabsSchedule.LabHoursRemaining) % 60) + " m ",
    //                RemainingLabHours = Math.Floor(k.d.CloudLabsSchedule.LabHoursRemaining / 60) + " h " + Math.Floor(k.d.CloudLabsSchedule.LabHoursRemaining % 60) + " m ",
    //                Name = k.d.Query.CloudLabUsers.FirstName + " " + k.d.Query.CloudLabUsers.LastName,
    //            })                 
    //            .ToList();


    //        return Ok(users);

    //    }



    //}
}