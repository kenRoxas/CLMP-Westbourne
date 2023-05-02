using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/BusinessType")]
    public class BusinessTypeController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();

        // GET: BusinessType
        [HttpGet]
        [Route("GetBusinessType")]
        public IHttpActionResult GetBusinessType()
        {
            return Ok(_db.BusinessTypes.ToList());
        }

        [HttpGet]
        [Route("GetBusinessIdById")]
        public IHttpActionResult getBusinessIdById(int businessId)
        {
            return Ok(_db.BusinessTypes.Where(q=>q.BusinessId == businessId).FirstOrDefault().BusinessType);
        }
    }
}