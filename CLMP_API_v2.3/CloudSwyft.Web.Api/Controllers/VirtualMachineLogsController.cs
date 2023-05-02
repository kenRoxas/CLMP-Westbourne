using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
using System.Web.Configuration;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/VirtualMachineLogs")]

    public class VirtualMachineLogsController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();

        // GET: api/VirtualMachineLogs
        public IQueryable<VirtualMachineLog> GetVirtualMachineLogs()
        {
            return db.VirtualMachineLogs;
        }


        //[HttpGet]
        //[Route("Logs")]
        //public HttpResponseMessage Logs(int UserId, int VEProfileId, string comment)
        //{

        //    var date = DateTime.Now;
        //    //List<string> listStamp = new List<string>();
        //    string stampList = "";

        //    var virtualMachineMappings = db.VirtualMachineMappings.Where(vmm => vmm.UserID == UserId && vmm.VEProfileID == VEProfileId).FirstOrDefault();
        //    var virtualMachineLogs = db.VirtualMachineLogs.Where(x => x.VEProfileID == VEProfileId && x.UserID == UserId).FirstOrDefault();
        //    var virtualMachine = db.VirtualMachines.Where(v => v.UserID == UserId && v.VEProfileID == VEProfileId).FirstOrDefault();

        //    if (virtualMachineLogs != null)
        //    {
        //        //listStamp.Add(virtualMachineLogs.TimeStamp + ';' + date + "-" + comment);

        //        var vmlength = virtualMachineLogs.TimeStamp.Length;
        //        stampList = virtualMachineLogs.TimeStamp + date + "----" + comment.ToUpper() + ';';

        //        virtualMachineLogs.TimeStamp = stampList;

               
        //    }
        //    else
        //    {
        //        VirtualMachineLog vml = new VirtualMachineLog();
        //        vml.RoleName = virtualMachineMappings.VMName;
        //        vml.VirtualMachineID = virtualMachine.VirtualMachineID;
        //        vml.Comment = comment;
        //        vml.UserID = UserId;
        //        vml.VEProfileID = VEProfileId;
        //        stampList = date + "----" + comment.ToUpper()+ ';';
        //        vml.TimeStamp = stampList;
        //        db.VirtualMachineLogs.Add(vml);

        //    }
        //    db.SaveChanges();

        //    return Request.CreateResponse(HttpStatusCode.OK, virtualMachineLogs.TimeStamp);
        //}

        //[AllowAnonymous]
        //// POST: api/VirtualMachineLogs
        //[ResponseType(typeof(VirtualMachineLog))]
        //public IHttpActionResult PostVirtualMachineLog(VirtualMachineLog virtualMachineLog)
        //{
        //    //string[] Status = { "Created", "Started", "Running", "Stop" };

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    //VirtualMachine virtualMachine = db.VirtualMachines.SingleOrDefault(vm => vm.CourseID == virtualMachineLog.CourseID
        //    //    && vm.UserID == virtualMachineLog.UserID && vm.VEProfileID == virtualMachineLog.VEProfileID);
        //    var virtualMachine = db.VirtualMachines.FirstOrDefault(vm => vm.RoleName == virtualMachineLog.RoleName);
        //    if (virtualMachine == null)
        //    {
        //        return BadRequest("Virtual machine not found.");
        //    }

        //    virtualMachineLog.VirtualMachineID = virtualMachine.VirtualMachineID;

        //    //db.VirtualMachineLogs.Add(virtualMachineLog);

        //    var virtualMachineLogs = db.VirtualMachineLogs.Where(vml => vml.RoleName == virtualMachineLog.RoleName);

        //    var ifExist = virtualMachineLogs.ToList().Exists(x => x.Comment == virtualMachineLog.Comment);

        //    if (ifExist)
        //    {
        //        var s = virtualMachineLogs.Where(x => x.Comment == virtualMachineLog.Comment).FirstOrDefault();
        //        s.TimeStamp = virtualMachineLog.TimeStamp;

        //        if(s.Comment == "Running")
        //            s.ConsumedMinutes += 1;
        //    }
        //    else
        //    {
        //        db.VirtualMachineLogs.Add(virtualMachineLog);
        //    }

        //    db.SaveChanges();

        //    if (virtualMachineLog.Comment == "Stopped" && virtualMachine.IsStarted== 1)
        //    {
        //        VirtualMachinesController vmController = new VirtualMachinesController();

        //        vmController.ShutdownVM(virtualMachine.CourseID, virtualMachine.VEProfileID, virtualMachine.UserID);

        //        virtualMachine.IsStarted = 0;
        //    }

        //    // Set Timestamp
        //    virtualMachine.LastTimeStamp = virtualMachineLog.TimeStamp;


        //    db.Entry(virtualMachine).State = EntityState.Modified;

        //    db.SaveChanges();

        //    //db.Database.ExecuteSqlCommand("UPDATE TenantData SET ConsumedMinutes = ConsumedMinutes + 1");

        //    return CreatedAtRoute("DefaultApi", new { id = virtualMachineLog.VirtualMachineLogID }, virtualMachineLog);
        //}

        // DELETE: api/VirtualMachineLogs/5
        [ResponseType(typeof(VirtualMachineLog))]
        public IHttpActionResult DeleteVirtualMachineLog(int id)
        {
            VirtualMachineLog virtualMachineLog = db.VirtualMachineLogs.Find(id);
            if (virtualMachineLog == null)
            {
                return NotFound();
            }

            db.VirtualMachineLogs.Remove(virtualMachineLog);
            db.SaveChanges();

            return Ok(virtualMachineLog);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VirtualMachineLogExists(int id)
        {
            return db.VirtualMachineLogs.Count(e => e.VirtualMachineLogID == id) > 0;
        }
    }
}