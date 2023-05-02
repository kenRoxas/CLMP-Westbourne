using CloudSwyft.Web.Api.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
    [RoutePrefix("api/GCP")]

    public class GCPController : ApiController
    {
        private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private readonly VirtualEnvironmentDBTenantContext _dbTenant = new VirtualEnvironmentDBTenantContext();

        public string gcpServer = System.Configuration.ConfigurationManager.AppSettings["gcpServer"];

        [HttpGet]
        [Route("GetRegions")]
        public async Task<List<string>> GetRegions()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;


            response = await client.GetAsync("api/gcp/images/list/zones");

            var regions = JsonConvert.DeserializeObject<DataRegions>(response.Content.ReadAsStringAsync().Result);

            return regions.zones;

        }

        [HttpGet]
        [Route("GetProject")]
        public async Task<List<Results>> GetProject()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            HttpResponseMessage response = null;

            response = await client.GetAsync("api/gcp/project/");

            var project = JsonConvert.DeserializeObject<Project>(response.Content.ReadAsStringAsync().Result);

            return project.results;

        }

        [HttpGet]
        [Route("GetVPCGCP")]
        public async Task<List<ResultsVCP>> GetVPCGCP()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            HttpResponseMessage response = null;

            response = await client.GetAsync("api/gcp/vcp/network/");

            var project = JsonConvert.DeserializeObject<VCP>(response.Content.ReadAsStringAsync().Result);

            return project.results;

        }

        [HttpGet]
        [Route("GetSubGCP")]
        public async Task<List<SubNet.ResultsSubNet>> GetSubGCP()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            HttpResponseMessage response = null;

            response = await client.GetAsync("api/gcp/vcp/subnetwork/");

            var project = JsonConvert.DeserializeObject<SubNet>(response.Content.ReadAsStringAsync().Result);

            return project.results;

        }

        [HttpGet]
        [Route("GetProjectFamily")]
        public async Task<List<ProjFamily.ResultsFamily>> GetProjectFamily(int tenantId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = null;

            response = await client.GetAsync("api/gcp/images/project/family");

            var project = JsonConvert.DeserializeObject<ProjFamily>(response.Content.ReadAsStringAsync().Result);

            if (_dbTenant.AzTenants.Any(q => q.TenantId == tenantId))
            {
                var projName = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault().ProjectName;

                project.data.Add(new ProjFamily.ResultsFamily()
                {
                    name = projName
                }); 
            }

            return project.data;

        }

        [HttpGet]
        [Route("GetAMI")]
        public async Task<List<ProjAMIFamily.ImagesFamilyAMI>> GetAMI(string family)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string proj = "";

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;
            List<ProjAMIFamily.ImagesFamilyAMI> ami = new List<ProjAMIFamily.ImagesFamilyAMI>();

            //if (_dbTenant.AzTenants.Any(q=>q.TenantId == tenantId))
            //{
            //    proj = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault().ProjectName;
            //    response = await client.GetAsync("api/gcp/images/project?image_project=" + proj);

            //    var addProj = JsonConvert.DeserializeObject<ProjAMIFamily>(response.Content.ReadAsStringAsync().Result);

            //    foreach (var item in addProj.data.images)
            //    {
            //        if (item.deprecated == "")
            //        {
            //            ami.Add(item);
            //        }
            //    }
            //}

            response = await client.GetAsync("api/gcp/images/project?image_project=" + family);

            var project = JsonConvert.DeserializeObject<ProjAMIFamily>(response.Content.ReadAsStringAsync().Result);

            foreach (var item in project.data.images)
            {
                if(item.deprecated == "")
                {
                    ami.Add(item);
                }
            }

          

            return ami;

        }

        [HttpGet]
        [Route("GetMachineType")]
        public async Task<List<MachineType.ResultsMachine>> GetMachineType(string zone)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gcpServer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = null;

            response = await client.GetAsync("api/gcp/images/list/zone/machine-type?zone=" + zone);

            var project = JsonConvert.DeserializeObject<MachineType>(response.Content.ReadAsStringAsync().Result);

            return project.machine_types;

        }
    }
}