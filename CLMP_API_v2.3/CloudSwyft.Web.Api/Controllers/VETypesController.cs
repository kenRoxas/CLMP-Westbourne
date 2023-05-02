using System;
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

namespace CloudSwyft.Web.Api.Controllers
{
    public class VETypesController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();

        // GET: api/VETypes
        public IQueryable<VEType> GetVETypes()
        {
            return db.VETypes;
        }

        // GET: api/VETypes/5
        [ResponseType(typeof(VEType))]
        public IHttpActionResult GetVEType(int id)
        {
            VEType vEType = db.VETypes.Find(id);
            if (vEType == null)
            {
                return NotFound();
            }
            

            return Ok(vEType);
        }

        // PUT: api/VETypes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVEType(int id, VEType vEType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vEType.VETypeID)
            {
                return BadRequest();
            }

            db.Entry(vEType).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VETypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/VETypes
        [ResponseType(typeof(VEType))]
        public IHttpActionResult PostVEType(VEType vEType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.VETypes.Add(vEType);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = vEType.VETypeID }, vEType);
        }

        // DELETE: api/VETypes/5
        [ResponseType(typeof(VEType))]
        public IHttpActionResult DeleteVEType(int id)
        {
            VEType vEType = db.VETypes.Find(id);
            if (vEType == null)
            {
                return NotFound();
            }

            db.VETypes.Remove(vEType);
            db.SaveChanges();

            return Ok(vEType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VETypeExists(int id)
        {
            return db.VETypes.Count(e => e.VETypeID == id) > 0;
        }
    }
}