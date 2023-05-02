
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
using System.Net.Http;
using System.Collections;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Data;
using System;

namespace CloudSwyft.Web.Api.Controllers
{
    [System.Web.Mvc.RoutePrefix("api/VirtualMachineLogStats")]
    public class VirtualMachineLogStatsController : ApiController
    {

        private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        // POST: api/VirtualMachineLogStats
        [HttpPost]
        [ResponseType(typeof(VirtualMachineLogStats))]
        public IHttpActionResult PostVirtualMachineLogStats(VirtualMachineLogStats virtualMachineLogStats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.VirtualMachineLogStats.Add(virtualMachineLogStats);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = virtualMachineLogStats.VirtualMachineLogStatsId }, virtualMachineLogStats);
        }

        // PUT: api/VirtualMachineLogStats

        [HttpPut]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVirtualMachineLogStats(long id, VirtualMachineLogStats virtualMachineLogStats)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != virtualMachineLogStats.VirtualMachineLogStatsId)
            {
                return BadRequest();
            }

            _db.Entry(virtualMachineLogStats).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VirtualMachineLogStatsExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpGet]
        // GET: api/VirtualMachineLogStats/4
        [ResponseType(typeof(VirtualMachineLogStats))]
        public IHttpActionResult GetVirtualMachineLogStats(long id)
        {
            var logs = _db.VirtualMachineLogStats.SingleOrDefault(vm => vm.VirtualMachineLogStatsId == id);
            if (logs == null)
            {
                return NotFound();
            }

            return Ok(logs);
        }

        private bool VirtualMachineLogStatsExists(long id)
        {
            return _db.VirtualMachineLogStats.Count(e => e.VirtualMachineLogStatsId == id) > 0;
        }

       
    }
}