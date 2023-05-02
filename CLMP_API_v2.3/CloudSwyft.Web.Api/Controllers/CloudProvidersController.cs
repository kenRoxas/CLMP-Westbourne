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
    public class CloudProvidersController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();

        // GET: api/CloudProviders
        public IQueryable<CloudProvider> GetCloudProviders()
        {
            return db.CloudProviders.Where(q=>q.IsDisabled == 0);
        }

        // GET: api/CloudProviders/5
        [ResponseType(typeof(CloudProvider))]
        public IHttpActionResult GetCloudProvider(int id)
        {
            CloudProvider cloudProvider = db.CloudProviders.Find(id);
            if (cloudProvider == null)
            {
                return NotFound();
            }

            return Ok(cloudProvider);
        }

        // PUT: api/CloudProviders/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCloudProvider(int id, CloudProvider cloudProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cloudProvider.CloudProviderID)
            {
                return BadRequest();
            }

            db.Entry(cloudProvider).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CloudProviderExists(id))
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

        // POST: api/CloudProviders
        [ResponseType(typeof(CloudProvider))]
        public IHttpActionResult PostCloudProvider(CloudProvider cloudProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CloudProviders.Add(cloudProvider);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = cloudProvider.CloudProviderID }, cloudProvider);
        }

        // DELETE: api/CloudProviders/5
        [ResponseType(typeof(CloudProvider))]
        public IHttpActionResult DeleteCloudProvider(int id)
        {
            CloudProvider cloudProvider = db.CloudProviders.Find(id);
            if (cloudProvider == null)
            {
                return NotFound();
            }

            db.CloudProviders.Remove(cloudProvider);
            db.SaveChanges();

            return Ok(cloudProvider);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CloudProviderExists(int id)
        {
            return db.CloudProviders.Count(e => e.CloudProviderID == id) > 0;
        }
    }
}