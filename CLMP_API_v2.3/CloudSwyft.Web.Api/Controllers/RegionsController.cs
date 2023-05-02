using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Http;
using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/Regions")]
    public class RegionsController : ApiController
    {
        private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();

        [HttpGet]
        [Route("GetRegions")]
        public List<string> GetRegions()
        {
            return _db.Regions.Select(q => q.RegionName).ToList();
        }

    }

    //[HttpGet]
    //[Route("GetRegions")]
    //public async Task<AzureRegions[]> GetRegions(int tenantId)
    //{
    //    //var url = "/labs/location";

    //    //var tenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).Select(w => new { w.TenantKey, w.SubscriptionKey }).FirstOrDefault();
    //    //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

    //    //HttpClient client = new HttpClient();
    //    //client.BaseAddress = new Uri(AzureVM);
    //    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    //    //client.DefaultRequestHeaders.Add("TenantId", tenant.TenantKey);
    //    //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", tenant.SubscriptionKey);

    //    //HttpResponseMessage response = null;
    //    //response = await client.GetAsync(url);

    //    //var result = await response.Content.ReadAsStringAsync();
    //    //var data = JsonConvert.DeserializeObject<AzureRegions[]>(result);
    //    //return data;

    //}
}
