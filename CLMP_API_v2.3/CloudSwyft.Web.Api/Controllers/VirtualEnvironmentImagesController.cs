using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using CloudSwyft.Web.Api.Models;
using System.Net.Http;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Web.Configuration;
using Microsoft.WindowsAzure.Management.Storage;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Configuration;
using Microsoft.Azure;
using Azure.Storage.Blobs;
using Microsoft.WindowsAzure.Storage.Auth;

namespace CloudSwyft.Web.Api.Controllers
{
    [RoutePrefix("api/VirtualEnvironmentImages")]
    public class VirtualEnvironmentImagesController : ApiController
    {
        private readonly VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
        private TenantDBContext _dbTenant = new TenantDBContext();
        
        string StorageContainerName = System.Configuration.ConfigurationManager.AppSettings["StorageContainerName"];
        private string AzureVM = WebConfigurationManager.AppSettings["AzureVM"];
        private string StorageAccount = WebConfigurationManager.AppSettings["StorageAccount"];

        public IQueryable<VirtualEnvironmentImages> GetVirtualEnvironmentImages()
        {
            return _db.VirtualEnvironmentImages.Include(ve => ve.VirtualEnvironment);
        }

        [HttpGet]
        [Route("ByVeImagesId")]
        public IHttpActionResult ByVeImagesId(int veImagesId)
        {
            var veQuery = _db.VirtualEnvironmentImages.Include(ve => ve.VirtualEnvironment).Where(ve => ve.VirtualEnvironmentImagesID == veImagesId).ToList();

            var veList = veQuery.ToList();

            return Ok(veList);
        }

        [HttpGet]
        [Route("ByVeId")]
        public IHttpActionResult ByVeId(int veId)
        {
            var veQuery = _db.VirtualEnvironmentImages.Include(ve => ve.VirtualEnvironment).Where(ve => ve.VirtualEnvironmentID == veId).ToList();

            var veList = veQuery.ToList();

            return Ok(veList);
        }

        // POST: api/VirtualEnvironments
        [ResponseType(typeof(VirtualEnvironmentImages))]
        public IHttpActionResult PostVirtualEnvironment(VirtualEnvironmentImages virtualEnvironmentImages)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _db.VirtualEnvironmentImages.Add(virtualEnvironmentImages);
            _db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = virtualEnvironmentImages.VirtualEnvironmentImagesID }, virtualEnvironmentImages);
        }

        // PUT: api/VirtualEnvironmentImages/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVirtualEnvironment(int id, VirtualEnvironmentImages virtualEnvironment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != virtualEnvironment.VirtualEnvironmentImagesID)
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
                if (!VirtualEnvironmentImagesExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(VirtualEnvironmentImages))]
        public IHttpActionResult DeleteVirtualEnvironmentImages(int id)
        {
            var virtualEnvironmentImages = _db.VirtualEnvironmentImages.Find(id);
            if (virtualEnvironmentImages == null)
            {
                return NotFound();
            }

            _db.VirtualEnvironmentImages.Remove(virtualEnvironmentImages);
            _db.SaveChanges();

            return Ok(virtualEnvironmentImages);
        }

        private bool VirtualEnvironmentImagesExists(int id)
        {
            return _db.VirtualEnvironmentImages.Count(e => e.VirtualEnvironmentImagesID == id) > 0;
        }


        [HttpGet]
        [Route("GetFilteredImagesFromAzure")]
        public async Task<IHttpActionResult> GetFilteredImagesFromAzure(string Title, int userGroup)
        {
            //try
            //{
            //    var url =  "/labs/virtualmachine/image";

            //    var tenantId = _db.CloudLabsGroups.Where(q => q.CloudLabsGroupID == userGroup).FirstOrDefault().TenantId;

            //    var tenantKey = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).FirstOrDefault().TenantKey;
            //    var subscriptionKey = _dbTenant.AzTenants.Where(x => x.TenantId == tenantId).FirstOrDefault().SubscriptionKey;

            //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //    HttpClient client = new HttpClient();
            //    client.BaseAddress = new Uri(AzureVM);
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //    client.DefaultRequestHeaders.Add("TenantId", tenantKey);
            //    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            //    HttpResponseMessage response = null;

            //    response = await client.GetAsync(url).ConfigureAwait(false);

            //    var result = JsonConvert.DeserializeObject<string[]>(response.Content.ReadAsStringAsync().Result);
            //    List<string> image = new List<string>();


            //    foreach (var item in result)
            //    {
            //        if (item.ToUpper().Contains(Title.ToUpper()))
            //            image.Add(item);
            //    }

            //    return Ok(image);
            //}
            //catch (Exception)
            //{
            //    return Ok("No tenantKey");
            //}
            
            //var storageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageAccountNameAndKey"]);
            ////var storageAccountContainer = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageAccountNameAndKeyContainer"]);
            string[] blobss = new string[] { };

            try
            {
                var veTypeId = _db.VirtualEnvironments.Where(q => q.Title == Title).FirstOrDefault().VETypeID;

                if (veTypeId == 1 || veTypeId == 2 || veTypeId == 3 || veTypeId == 4)
                {
                    List<string> image = new List<string>();
                    //int counter = 0;
                    CloudStorageAccount backupStorageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageAccountNameAndKey"]);

                    var backupBlobClient = backupStorageAccount.CreateCloudBlobClient();
                    var backupContainer = backupBlobClient.GetContainerReference(StorageContainerName);

                    // useFlatBlobListing is true to ensure loading all files in
                    // virtual blob sub-folders as a plain list
                    var list = backupContainer.ListBlobs(useFlatBlobListing: true);
                    var listOfFileNames = new List<string>();

                    foreach (var blob in list)
                    {
                        string temp = blob.Uri.Segments.Last().ToString().ToLower();
                        if (temp.Contains(Title.ToLower()))
                        {
                            image.Add(blob.Uri.ToString());
                            //counter++; 
                        }
//                        listOfFileNames.Add(blobFileName);
                    }

                    //var blobClient = storageAccount.CreateCloudBlobClient();
                    //var container = blobClient.GetContainerReference(StorageContainerName);

                    //var blobs = container.ListBlobs();
                    //int counter = 0;
                    ////for initialization of the size of the 2d array
                    //foreach (var blob in blobs)
                    //{
                    //    string temp = blob.Uri.Segments.Last().ToString().ToLower();
                    //    if (temp.Contains(Title.ToLower()))
                    //    { counter++; }
                    //}

                    //string[,] blobList = new string[counter, 2];
                    //int i = 0;
                    //foreach (var blob in list)
                    //{
                    //    string temp = blob.Uri.Segments.Last().ToString().ToLower();
                    //    if (temp.Contains(Title.ToLower()))
                    //    {
                    //        blobList[i, 0] = blob.Uri.ToString();
                    //        blobList[i, 1] = blob.Uri.Segments.Last();
                    //        i++;
                    //    }
                    //}

                    return Ok(image);

                }
                else
                    return Ok(blobss);
            }
            catch (Exception e)
            {
                return Ok(blobss);
            }



        }


        //[HttpPost]
        //[Route("AddImage")]
        //public HttpResponseMessage AddImage(VirtualEnvironmentImages model)
        //{
        //    _db.VirtualEnvironmentImages.Add(model);
        //    _db.SaveChanges();
        //    return Request.CreateResponse(HttpStatusCode.OK, model);
        //}

        [HttpPost]
        [Route("EditImage")]
        public HttpResponseMessage EditImage(VirtualEnvironmentImages newModel)
        {
            try
            {
                VirtualEnvironmentImages oldModel = _db.VirtualEnvironmentImages.Where(x => x.VirtualEnvironmentID == newModel.VirtualEnvironmentID && x.GroupId == newModel.GroupId).FirstOrDefault();
                var isVEImagesExist = _db.VirtualEnvironmentImages.Any(x => x.VirtualEnvironmentID == newModel.VirtualEnvironmentID && x.GroupId == newModel.GroupId);
                if (isVEImagesExist)
                {
                    oldModel.Name = newModel.Name;
                    oldModel.ImageFamily = newModel.ImageFamily;
                    oldModel.ImageFamilyMinDiskSize = newModel.ImageFamilyMinDiskSize;
                    oldModel.ProjectFamily = newModel.ProjectFamily;
                    oldModel.GroupId = newModel.GroupId;
                    _db.SaveChanges();
                }
                else
                {
                    _db.VirtualEnvironmentImages.Add(newModel);
                    _db.SaveChanges();
                }
                return Request.CreateResponse(HttpStatusCode.OK, oldModel);
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpDelete]
        [Route("DeleteImage")]
        public HttpResponseMessage DeleteImage(int VirtualEnvironmentID)
        {
            VirtualEnvironmentImages virtualEnvironment = _db.VirtualEnvironmentImages.Where(x => x.VirtualEnvironmentID == VirtualEnvironmentID).FirstOrDefault();
            _db.VirtualEnvironmentImages.Remove(virtualEnvironment);
            _db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, virtualEnvironment.Name);
        }

        [HttpGet]
        [Route("GetVirtualEnvironmentImagesID")]
        public HttpResponseMessage GetVirtualEnvironmentImagesID(int VirtualEnvironmentID)
        {
            VirtualEnvironmentImages sample = _db.VirtualEnvironmentImages.FirstOrDefault(x => x.VirtualEnvironmentID == VirtualEnvironmentID);
            return Request.CreateResponse(HttpStatusCode.OK, sample);
        }

        [HttpGet]
        [Route("GetStorageAccountName")]
        public async Task<HttpResponseMessage> GetStorageAccountName(int tenantId)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var tenant = _dbTenant.AzTenants.Where(q => q.TenantId == tenantId).FirstOrDefault();

                HttpClient clientFA = new HttpClient();
                // clientFA.BaseAddress = new Uri(FunctionAppUrl);
                clientFA.BaseAddress = new Uri(StorageAccount);
                clientFA.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var data = new
                {
                    //SubscriptionId= "19376908-9c8d-4fa9-815f-13f80d86e944",
                    //TenantId= "72162010-f489-4a94-b485-638b150f4873",
                    //ApplicationKey= "ZOP7Q~vzt1sxt-Gr.4QfIZ26FuNnkrksc4f1O",
                    //ApplicationId="db7555ab-64fc-42ff-85c0-37f77ff97f17"
                    SubscriptionId = tenant.SubscriptionId,
                    TenantId = tenant.ApplicationTenantId,
                    ApplicationKey = tenant.ApplicationSecretKey,
                    ApplicationId = tenant.ApplicationId
                };

                var dataMsg = JsonConvert.SerializeObject(data);

                var client = await clientFA.PostAsync("", new StringContent(dataMsg, Encoding.UTF8, "application/json"));
                List<StorageName> res = new List<StorageName>();
                List<StorageName> result = new List<StorageName>();

                if (client.StatusCode.ToString() != "BadRequest")
                {
                    var r = JsonConvert.DeserializeObject(client.Content.ReadAsStringAsync().Result);
                    res = JsonConvert.DeserializeObject<List<StorageName>>(r.ToString());

                    foreach (var item in res)
                    {
                        if (item.storageAccountName.Contains("disk"))
                            result.Add(item);
                    }
                }

                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);

            }

        }

        [HttpGet]
        [Route("GetImageTemplate")]
        public IHttpActionResult GetImageTemplate(string Description, string storageConnectionString, string storageKey, string storageAccountName)
        {
            string[] blobss = new string[] { };

            try
            {
                var veTypeId = _db.VirtualEnvironments.Where(q => q.Description == Description).FirstOrDefault().VETypeID;
                var title = _db.VirtualEnvironments.Where(q => q.Description == Description).FirstOrDefault().Title;
                storageConnectionString = storageConnectionString.Replace(" ", "+");
                storageKey = storageKey.Replace(" ", "+");
                if (veTypeId == 1 || veTypeId == 2 || veTypeId == 3 || veTypeId == 4)
                {
                    //List<string> image = new List<string>();
                    ////int counter = 0;
                    //CloudStorageAccount backupStorageAccount = CloudStorageAccount.Parse(System.Configuration.ConfigurationManager.AppSettings["StorageAccountNameAndKey"]);
                    ////CloudStorageAccount backupStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
                    //var backupBlobClient = backupStorageAccount.CreateCloudBlobClient();
                    //var backupContainer = backupBlobClient.GetContainerReference(StorageContainerName);

                    //// useFlatBlobListing is true to ensure loading all files in
                    //// virtual blob sub-folders as a plain list
                    //var list = backupContainer.ListBlobs(useFlatBlobListing: true);
                    //var listOfFileNames = new List<string>();

                    //foreach (var blob in list)
                    //{
                    //    string temp = blob.Uri.Segments.Last().ToString().ToLower();
                    //    if (temp.Contains(Description.ToLower()))
                    //    {
                    //        image.Add(blob.Uri.ToString());
                    //        //counter++; 
                    //    }
                    //    //                        listOfFileNames.Add(blobFileName);
                    //}


                    List<string> image = new List<string>();

                    string accountName = storageAccountName;
                    string keyValue = storageKey;

                    var useHttps = true;
                    var connValid = true;

                    var storageCredentials = new StorageCredentials(accountName, keyValue);
                    var storageAccount = new CloudStorageAccount(storageCredentials, useHttps);
                    var conString = storageAccount.ToString(connValid);

                    CloudStorageAccount sa = CloudStorageAccount.Parse(conString);

                    var ss = sa.CreateCloudBlobClient();
                    var sss = ss.GetContainerReference(StorageContainerName);

                    var list = sss.ListBlobs(useFlatBlobListing: true);
                    var listOfFileNames = new List<string>();

                    foreach (var blob in list)
                    {
                        string temp = blob.Uri.Segments.Last().ToString().ToLower();
                        if (temp.Contains(title.ToLower()))
                        {
                            image.Add(blob.Uri.ToString());
                        }
                    }
                    return Ok(image);

                }
                else
                    return Ok(blobss);
            }
            catch (Exception e)
            {
                return Ok(blobss);
            }

        }
        //[HttpGet]
        //[Route("CheckDiskSize")]
        //public HttpResponseMessage CheckDiskSize(int VirtualEnvironmentID)
        //{
        //    try
        //    {
        //        if(_db.VirtualEnvironmentImages.Any(x => x.VirtualEnvironmentID == VirtualEnvironmentID))
        //        {
        //            string size = _db.VirtualEnvironments.Join(_db.VirtualEnvironmentImages, a => a.VirtualEnvironmentID, b => b.VirtualEnvironmentID, (a, b) => new { a, b }).Where(x => x.b.VirtualEnvironmentID == VirtualEnvironmentID).FirstOrDefault().b.ImageFamilyMinDiskSize;
        //            return Request.CreateResponse(HttpStatusCode.OK, size);
        //        }
        //        else
        //            return Request.CreateResponse(HttpStatusCode.OK, 0);

        //    }
        //    catch (Exception e)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, 0);
        //    }
        //}

    }
}