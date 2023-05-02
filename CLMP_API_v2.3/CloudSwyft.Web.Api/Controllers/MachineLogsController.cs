using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/MachineLogs")]
    public class MachineLogsController : ApiController
    {
        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        
        [HttpPost]
        [Route("Logs")]
        public void Logs(string resourceID = "", string requestId= "", string status = "")
        {

            try
            {
                DateTime dateUtc = DateTime.UtcNow;

                var logs = _db.MachineLogs.Where(q => q.ResourceId == resourceID).FirstOrDefault();
                //logs.Logs = '(' + status + ')' + dateUtc + "---" + logs.Logs;
                logs.Logs = '(' + status + ')' + dateUtc + "---" + logs.Logs;
                logs.LastStatus = status;
                logs.ModifiedDate = dateUtc;

                //_db.Entry(logs).State = EntityState.Modified;
                _db.SaveChanges();

            }
            catch (Exception)
            {
            }
        }



    }
}