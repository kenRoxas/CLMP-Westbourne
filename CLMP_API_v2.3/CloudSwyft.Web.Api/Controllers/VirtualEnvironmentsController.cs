using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
using System.Net.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/VirtualEnvironments")]
    public class VirtualEnvironmentsController : ApiController
    {
        private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();

        // GET: api/VirtualEnvironments
        public IQueryable<VirtualEnvironment> GetVirtualEnvironments()
        {
            var veList = _db.VirtualEnvironments.ToList();

            foreach (var virtualEnvironment in veList)
            {
                //if (!string.IsNullOrEmpty(virtualEnvironment.ThumbnailURL))
                //{
                //    virtualEnvironment.ThumbnailURL = string.Format("http://{0}:{1}/{2}", Url.Request.RequestUri.Host, Url.Request.RequestUri.Port, virtualEnvironment.ThumbnailURL);
                //}
            }

            return _db.VirtualEnvironments.Include(ve => ve.VEType);
        }

        [HttpGet]
        [Route("ByVEType")]
        public IHttpActionResult ByVeType(int veTypeId, int userGroup)
        {

            var veQuery1 = _db.VirtualEnvironments.Include(ve => ve.VEType).Where(ve => ve.VETypeID == veTypeId).ToList();

            var veQuery = _db.VirtualEnvironments
                  .Join(_db.VirtualEnvironmentImages,
                  a => a.VirtualEnvironmentID,
                  b => b.VirtualEnvironmentID,
                  (a, b) => new { a, b }).Where(ve => ve.a.VETypeID == veTypeId && ve.b.GroupId == userGroup).Select(w=>w.a).ToList();

 

            return Ok(veQuery);
        }

        // GET: api/VirtualEnvironments/5
        [ResponseType(typeof(VirtualEnvironment))]
        public IHttpActionResult GetVirtualEnvironment(int id)
        {
            var virtualEnvironment = _db.VirtualEnvironments.Include(ve => ve.VEType).Single(ve => ve.VirtualEnvironmentID == id);

            //if (!string.IsNullOrEmpty(virtualEnvironment.ThumbnailURL))
            //{
            //    virtualEnvironment.ThumbnailURL = "http://" + Url.Request.RequestUri.Host +
            //                                    ":" + Url.Request.RequestUri.Port +
            //                                    "/" + virtualEnvironment.ThumbnailURL;
            //}

            return Ok(virtualEnvironment);
        }

        // PUT: api/VirtualEnvironments/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVirtualEnvironment(int id, VirtualEnvironment virtualEnvironment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != virtualEnvironment.VirtualEnvironmentID)
            {
                return BadRequest();
            }

            _db.Entry(virtualEnvironment).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VirtualEnvironmentExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/VirtualEnvironments
        [ResponseType(typeof(VirtualEnvironment))]
        public IHttpActionResult PostVirtualEnvironment(VirtualEnvironment virtualEnvironment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.VirtualEnvironments.Add(virtualEnvironment);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = virtualEnvironment.VirtualEnvironmentID }, virtualEnvironment);
        }

        // DELETE: api/VirtualEnvironments/5
        [ResponseType(typeof(VirtualEnvironment))]
        public IHttpActionResult DeleteVirtualEnvironment(int id)
        {
            var virtualEnvironment = _db.VirtualEnvironments.Find(id);
            if (virtualEnvironment == null)
            {
                return NotFound();
            }

            _db.VirtualEnvironments.Remove(virtualEnvironment);
            _db.SaveChanges();

            return Ok(virtualEnvironment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool VirtualEnvironmentExists(int id)
        {
            return _db.VirtualEnvironments.Count(e => e.VirtualEnvironmentID == id) > 0;
        }



        [HttpPost]
        [Route("CreateVirtualEnvironments")]
        public HttpResponseMessage CreateVirtualEnvironments(VirtualEnvironment model)
        {
            _db.VirtualEnvironments.Add(model);
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, model);

        }

        [HttpGet]
        [Route("ReadVirtualEnvironments")]
        public HttpResponseMessage ReadVirtualEnvironments()
        {

            var environments = _db.VirtualEnvironments.ToList();
            return Request.CreateResponse(HttpStatusCode.OK, environments);
        }

        [HttpPost]
        [Route("UpdateVirtualEnvironments")]
        public HttpResponseMessage UpdateVirtualEnvironments(VirtualEnvironment model)
        {
            VirtualEnvironment VE = _db.VirtualEnvironments.Where(x => x.VirtualEnvironmentID == model.VirtualEnvironmentID).FirstOrDefault();
            //VE.ThumbnailURL = model.ThumbnailURL;
            VE.VETypeID = model.VETypeID;
            VE.Title = model.Title;
            VE.Description = model.Description;
            //VE.Name = model.Name;
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, VE);
        }

        [HttpDelete]
        [Route("DeleteVirtualEnvironments")]
        public HttpResponseMessage DeleteVirtualEnvironments(int id)
        {
            var virtualEnvironment = _db.VirtualEnvironments.Find(id);
            _db.VirtualEnvironments.Remove(virtualEnvironment);
            _db.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, virtualEnvironment);
        }

        [HttpGet]
        [Route("GetVirtualEnvironmentsByVeId")]
        public HttpResponseMessage GetVirtualEnvironmentsByVeId(int vetypeId)
        {

            var environments = _db.VirtualEnvironments.Where(q=>q.VETypeID == vetypeId).ToList();
            return Request.CreateResponse(HttpStatusCode.OK, environments);
        }
    }
}