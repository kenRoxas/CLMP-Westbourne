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
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http.Headers;
using System.Web;
using System.Drawing;
using System.Web.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using System.Threading;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/LabActivities")]
    public class LabActivitiesController : ApiController
    {
        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();

        private LabActivityReturn ReformatLabActivity(LabActivity labActivity)
        {
            LabActivityReturn returnLabActivity = new LabActivityReturn();

            returnLabActivity.LabActivityID = labActivity.LabActivityID;
            returnLabActivity.Name = labActivity.Name;
            try
            {
                returnLabActivity.Tasks = JsonConvert.DeserializeObject<List<string>>(labActivity.Tasks);
            }
            catch
            {
                returnLabActivity.Tasks = new List<string>();
            }
            returnLabActivity.TasksHtml = labActivity.TasksHtml.Replace("<p>&nbsp;</p>", "");
            returnLabActivity.UseCount = GetVEProfileUseCount(labActivity.LabActivityID);
            returnLabActivity.CourseCode = labActivity.CourseCode;

            returnLabActivity.LabAnswerKey = labActivity.LabAnswerKey;
            returnLabActivity.LabAnswerKeyName = labActivity.LabAnswerKeyName;
            return returnLabActivity;
        }

        // GET: api/LabActivities
        public LabActivityViewModelPart GetLabActivities(string q = "", int pageSize = 0, int activePage = 1)
        {

            LabActivityViewModelPart laVm = new LabActivityViewModelPart();
            if (String.IsNullOrEmpty(q))
            {
                q = "";
            }

            var query2 = db.LabActivities.Select
                (la => new 
                {
                    Id = la.LabActivityID,
                    Name = la.Name,
                    LabAnswerKey = la.LabAnswerKey
                }).ToList();

            var query = db.LabActivities
                .Where(la => la.Name.Contains(q))
                .Select(la => new { la.LabActivityID, la.Name, la.LabAnswerKey, la.LabAnswerKeyName, la.CourseCode })
                .OrderBy(la => la.Name)
                .ToList();

            List<LabActivityReturnPart> returnQuery = new List<LabActivityReturnPart>();
            //LabActivity asd = new LabActivity();

            List<string> empty = new List<string>();

            for (int x = 0; x < query.Count; x++)
            {
                var use = GetVEProfileUseCount(query[x].LabActivityID);

                //returnQuery.Add(new LabActivityReturnPart(query[x].LabActivityID, query[x].Name));
                returnQuery.Add(new LabActivityReturnPart(query[x].LabActivityID, query[x].Name, empty, "", use, query[x].LabAnswerKey, query[x].LabAnswerKeyName, query[x].CourseCode));
            }

            laVm.LabActivities = returnQuery;
            

            return laVm;
        }

        // GET: api/LabActivities/5
        [ResponseType(typeof(LabActivityReturn))]
        public IHttpActionResult GetLabActivity(int id)
        {
            LabActivity labActivity = db.LabActivities.Find(id);
            if (labActivity == null)
            {
                return NotFound();
            }

            return Ok(ReformatLabActivity(labActivity));
        }

            [HttpPost]
            [Route("UploadCSV")]
            public async Task<IHttpActionResult> UploadCSV()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return InternalServerError(
                    new UnsupportedMediaTypeException("The request doesn't contain valid content!", 
                        new MediaTypeHeaderValue("multipart/form-data")));
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
                await Request.Content.ReadAsMultipartAsync(provider).ConfigureAwait(false);

                HttpContent content = provider.Contents.FirstOrDefault();

                if (content != null)
                {
                    Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false);

                    StreamReader reader = new StreamReader(stream);

                    List<LabActivityReturn> labActivityList = new List<LabActivityReturn>();

                    int lineIndex = 0;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        string[] values = line.Split(',');

                        if (lineIndex == 0)
                        {
                            LabActivityReturn newLabActivity;

                            for (int x = 0; x < values.Length; x++)
                            {
                                newLabActivity = new LabActivityReturn();
                                newLabActivity.Name = values[x];
                                newLabActivity.Tasks = new List<string>();

                                labActivityList.Add(newLabActivity);
                            }

                        }
                        else
                        {
                            for (int y = 0; y < values.Length; y++)
                            {
                                if (!string.IsNullOrEmpty(values[y])) labActivityList[y].Tasks.Add(values[y]);
                            }
                        }

                        lineIndex++;
                    }

                    for (int z = 0; z < labActivityList.Count; z++)
                    {
                        db.LabActivities.Add(ReformatLabActivityReturn(labActivityList[z]));
                        db.SaveChanges();
                    }

                }

                return Ok("Lab Activities extracted and saved to database.");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
            
        }

        [HttpPost]
        [Route("UploadCSVPreview")]
        public async Task<IHttpActionResult> UploadCSVPreview()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return InternalServerError(
                    new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();

                await Request.Content.LoadIntoBufferAsync().ConfigureAwait(false);
                await Request.Content.ReadAsMultipartAsync(provider).ConfigureAwait(false);

                HttpContent content = provider.Contents.FirstOrDefault();

                List<LabActivityReturn> labActivityList = new List<LabActivityReturn>();

                if (content != null)
                {
                    Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false);

                    StreamReader reader = new StreamReader(stream);

                    int lineIndex = 0;

                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();

                        string[] values = line.Split(',');

                        if (lineIndex == 0)
                        {
                            LabActivityReturn newLabActivity;

                            for (int x = 0; x < values.Length; x++)
                            {
                                newLabActivity = new LabActivityReturn();
                                newLabActivity.Name = values[x];
                                newLabActivity.Tasks = new List<string>();

                                labActivityList.Add(newLabActivity);
                            }

                        }
                        else
                        {
                            for (int y = 0; y < values.Length; y++)
                            {
                                if (!string.IsNullOrEmpty(values[y])) labActivityList[y].Tasks.Add(values[y]);
                            }
                        }

                        lineIndex++;
                    }

                }

                return Ok(labActivityList);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

        }

        [HttpPost]
        [Route("ParseString")]
        public IHttpActionResult ParseObject(object contentObj)
        {
            string contentString = (string)contentObj;
            return Ok(ParseString(contentString));
        }

        [HttpPost]
        [Route("ParseString")]
        public IHttpActionResult ParseString(string contentString)
        {
            char[] invalidHtml = { '<', '>' };
            if (contentString.IndexOfAny(invalidHtml) > -1)
            {
                return BadRequest("Content cannot contain HTML elements.");
            }

            string[] separator = { "^|" };

            List<string> contentList = contentString.Split(separator, StringSplitOptions.None).ToList();

            contentList.Add(String.Empty);

            LabActivityReturn newLabActivityReturn = new LabActivityReturn();

            newLabActivityReturn.Name = contentList[0];
            newLabActivityReturn.Tasks = new List<string>();

            for (int x = 1; x < contentList.Count; x++ )
            {
                if (string.IsNullOrEmpty(contentList[x]))
                {
                    db.LabActivities.Add(ReformatLabActivityReturn(newLabActivityReturn));
                    db.SaveChanges();
                }
                else if (string.IsNullOrEmpty(contentList[x - 1]))
                {
                    newLabActivityReturn = new LabActivityReturn();

                    newLabActivityReturn.Name = contentList[x];
                    newLabActivityReturn.Tasks = new List<string>();
                } else
                {
                    newLabActivityReturn.Tasks.Add(contentList[x]);
                }
            }
            return Ok(contentString);
        }

        [HttpPost]
        [ResponseType(typeof(List<LabActivityReturn>))]
        [Route("ParseStringPreview")]
        public IHttpActionResult PreviewParseString(string contentString)
        {
            char[] invalidHtml = { '<', '>' };

            if (contentString.IndexOfAny(invalidHtml) > -1)
            {
                return BadRequest("Content cannot contain HTML elements.");
            }

            string[] separator = { "^|" };

            List<LabActivityReturn> previewList = new List<LabActivityReturn>();

            List<string> contentList = contentString.Split(separator, StringSplitOptions.None).ToList();

            contentList.Add(String.Empty);

            LabActivityReturn newLabActivityReturn = new LabActivityReturn();

            newLabActivityReturn.Name = contentList[0];
            newLabActivityReturn.Tasks = new List<string>();

            for (int x = 1; x < contentList.Count; x++)
            {
                if (string.IsNullOrEmpty(contentList[x]))
                {
                    previewList.Add(newLabActivityReturn);
                }
                else if (string.IsNullOrEmpty(contentList[x - 1]))
                {
                    newLabActivityReturn = new LabActivityReturn();

                    newLabActivityReturn.Name = contentList[x];
                    newLabActivityReturn.Tasks = new List<string>();
                }
                else
                {
                    newLabActivityReturn.Tasks.Add(contentList[x]);
                }
            }

            return Ok(previewList);
        }

        [HttpPost]
        [Route("UploadThumbnail")]
        public async Task<IHttpActionResult> UploadThumbnail()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return InternalServerError(
                    new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                foreach (var file in provider.Contents)
                {
                    string ext = Path.GetExtension(file.Headers.ContentDisposition.FileName.Replace("\\\"", "").Replace("\"", ""));
                    string dirPath = HttpContext.Current.Server.MapPath("~/Content/Images/LabActivity/");

                    if (!Directory.Exists(dirPath))
                    {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                    
                    var dataStream = await file.ReadAsStreamAsync();

                    Image image = Image.FromStream(dataStream);
                    string fileName = DateTime.Now.ToString("MMddyyhhmmss") + ext;
                    string fullPath = Path.Combine(dirPath, fileName);

                    if (File.Exists(fullPath)) File.Delete(fullPath);
                    image.Save(fullPath);

                    LabActivityUploadReturn labReturn = new LabActivityUploadReturn();
                    labReturn.location = WebConfigurationManager.AppSettings["CSWebApiUrl"] + "Content/Images/LabActivity/" + fileName;

                    return Ok(labReturn);
                }
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Ok();
        }

        [HttpGet]
        [Route("ByVEProfile")]
        public IHttpActionResult ByVEProfile(int veProfileID)
        {
            //var labActivities = db.LabActivities
            //                    .Join(db.VEProfileLabActivities,
            //                    la => la.LabActivityID,
            //                    vela => vela.LabActivityID,
            //                    (la, vela) => new { LabActivity = la, VEProfileLabActivity = vela })
            //                    .Where(laids => laids.VEProfileLabActivity.VEProfileID == veProfileID)
            //                    .Select(x => new
            //                    {
            //                        LabActivityId = x.LabActivity.LabActivityID,
            //                        Name = x.LabActivity.Name,

            //                     }).ToList();


            List<int> labActivities = db.Database.SqlQuery<int>(
            "SELECT LabActivityID FROM dbo.VEProfileLabActivities WHERE VEProfileID =" + veProfileID.ToString()).ToList();

            //var laList = db.LabActivities.Where(la => labActivities.Contains(la.LabActivityID)).ToList();

            var laList = new List<LabActivity>();
            List<LabActivity> labActList = db.LabActivities.ToList<LabActivity>();

            for (int y = 0; y < labActivities.Count; y++)
            {
                for (int z = 0; z < labActList.Count; z++)
                {
                    if (labActivities[y] == labActList[z].LabActivityID)
                    {
                        laList.Add(labActList[z]);
                        break;
                    }
                }
            }
            //laList.Add(null);

            List<LabActivityReturn> returnList = new List<LabActivityReturn>();

            for (int x = 0; x < laList.Count; x++)
            {
                returnList.Add(ReformatLabActivity(laList[x]));
            }

            return Ok(returnList);
            //return Ok(labActivities);
        }

        int GetVEProfileUseCount (int labActivityID)
        {
            List<int> veProfiles = db.Database.SqlQuery<int>(
                "SELECT VEProfileID FROM dbo.VEProfileLabActivities WHERE LabActivityID =" + labActivityID.ToString()).ToList();

            return veProfiles.Count;
        }

        // PUT: api/LabActivities/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLabActivity(int id, LabActivityReturn labActivityReturn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != labActivityReturn.LabActivityID)
            {
                return BadRequest();
            }

            LabActivity labActivity = db.LabActivities.Find(id);
            labActivity.Name = labActivityReturn.Name;
            labActivity.Tasks = JsonConvert.SerializeObject(labActivityReturn.Tasks);
            labActivity.TasksHtml = labActivityReturn.TasksHtml;
            db.Entry(labActivity).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LabActivityExists(id))
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

        private LabActivity ReformatLabActivityReturn(LabActivityReturn labActivityReturn)
        {
            LabActivity labActivity = new LabActivity();

            labActivity.LabActivityID = labActivityReturn.LabActivityID;
            labActivity.Name = labActivityReturn.Name;
            labActivity.TasksHtml = labActivityReturn.TasksHtml;
            labActivity.Tasks = JsonConvert.SerializeObject(labActivityReturn.Tasks);

            labActivity.LabAnswerKey = labActivityReturn.LabAnswerKey;

            labActivity.LabAnswerKeyName = labActivityReturn.LabAnswerKeyName;

            /*
            string tasksString = "[";

            for (int x = 0; x < labActivityReturn.Tasks.Count; x++)
            {
                tasksString = tasksString + "'" + labActivityReturn.Tasks[x].Replace("'", "\\'") + "',";
            }

            tasksString = tasksString.Substring(0, tasksString.Length - 1) + "]";

            labActivity.Tasks = tasksString;
            */
            return labActivity;
        }

        // POST: api/LabActivities
        [ResponseType(typeof(LabActivityReturn))]
        public IHttpActionResult PostLabActivity(LabActivityReturn labActivityReturn)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            LabActivity labActivity = ReformatLabActivityReturn(labActivityReturn);
            db.LabActivities.Add(labActivity);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = labActivity.LabActivityID }, labActivityReturn);
        }
       [HttpPost]
       [Route("CreateLabActivities")]
       public IHttpActionResult CreateLabActivities(LabActivity model)
       {
           try
            {
                string pattern = "src=\"";
                int Start, End;

                var indexString = Enumerable.Range(0, model.TasksHtml.Length)
                       .Select(index => { return new { Index = index, Length = (index + pattern.Length) > model.TasksHtml.Length ? model.TasksHtml.Length - index : pattern.Length }; })
                       .Where(searchbit => searchbit.Length == pattern.Length && pattern.Equals(model.TasksHtml.Substring(searchbit.Index, searchbit.Length), StringComparison.OrdinalIgnoreCase))
                       .Select(searchbit => searchbit.Index);

                foreach (var item in indexString)
                {
                    if (model.TasksHtml.Contains("src=\"") && model.TasksHtml.Contains("\""))
                    {
                        Start = model.TasksHtml.IndexOf("src=\"", item) + "src=\"".Length;
                        End = model.TasksHtml.IndexOf("\"", Start);

                        model.TasksHtml = model.TasksHtml.Replace(model.TasksHtml.Substring(Start, End - Start), model.TasksHtml.Substring(Start, End - Start).Replace(" ", ""));
                    }

                    if (model.Tasks.Contains("src=\"") && model.Tasks.Contains("\""))
                    {
                        Start = model.Tasks.IndexOf("src=\"", item) + "src=\"".Length;
                        End = model.Tasks.IndexOf("\"", Start);

                        model.Tasks = model.Tasks.Replace(model.Tasks.Substring(Start, End - Start), model.Tasks.Substring(Start, End - Start).Replace(" ", ""));
                    }
                }
                if (model.LabActivityID == 0)
                {
                    db.LabActivities.Add(model);
                    db.SaveChanges();
                    return Created("Created", model);
                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error Creating Lab Activities" + ex);
            }
            finally
            {
            }
       }

        // DELETE: api/LabActivities/5
        [HttpDelete]
        [Route("DeleteLabActivities")]
        public IHttpActionResult DeleteLabActivities(int id)
        {
            LabActivity labActivity = db.LabActivities.Find(id);
            if (labActivity == null)
            {
                return NotFound();
            }
            
            db.LabActivities.Remove(labActivity);
            db.SaveChanges();

            return Ok(labActivity);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LabActivityExists(int id)
        {
            return db.LabActivities.Count(e => e.LabActivityID == id) > 0;
        }

        [HttpPost]
        [Route("UploadThumbnailLabActivities")]
        public async Task<IHttpActionResult> UploadThumbnailLabActivities(string imageFilename)
        {
            List<string> imagefile = JsonConvert.DeserializeObject<List<string>>(imageFilename);
          //if (!Request.Content.IsMimeMultipartContent())
          //  {
          //      return InternalServerError(
          //          new UnsupportedMediaTypeException("The request doesn't contain valid content!", new MediaTypeHeaderValue("multipart/form-data")));
          //  }
            try
            {
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);

                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);
                var uri = string.Empty;

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);
                int i = 0;
                foreach (var file in provider.FileData)
                {
                    bool readable = false;
                    int retries = 0;
                    
                    
                    file.Headers.ContentDisposition.FileName = imagefile[i].Replace(" ", "");

                    i++;
                    
                    var blockBlob = container.GetBlockBlobReference(file.Headers.ContentDisposition.FileName.Replace("\"", ""));
                    //new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read); File.OpenRead(file.LocalFileName)
                    while (!readable)
                    {
                        try
                        {
                            using (var fileStream = File.OpenRead(file.LocalFileName))// new FileStream(file.LocalFileName,FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                            {
                                blockBlob.UploadFromStream(fileStream);
                                readable = true;
                            }
                        }
                        catch (IOException ioex)
                        {
                            retries++;
                            Console.WriteLine(ioex.Message);
                            Thread.Sleep(10000);
                        }
                    }

                    uri = blockBlob.Uri.AbsoluteUri;
                    
                }

                return Ok(uri);

            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            
        }


        [HttpPost]
        [Route("UploadLabAnswerKey")]
        public async Task<IHttpActionResult> UploadLabAnswerKey(string PDFFilename, int labactivityid)
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageLabAnswerKeys"]);

                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerLabKeysName"]);
                var uri = string.Empty;

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (var file in provider.FileData)
                {
                    bool readable = false;
                    int retries = 0;

                    file.Headers.ContentDisposition.FileName = PDFFilename;
                    var blockBlob = container.GetBlockBlobReference(file.Headers.ContentDisposition.FileName.Replace("\"", ""));
                    //new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read); File.OpenRead(file.LocalFileName)
                    
                    while (!readable)
                    {
                        try
                        {
                            using (var fileStream = File.OpenRead(file.LocalFileName))// new FileStream(file.LocalFileName,FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
                            {
                                blockBlob.UploadFromStream(fileStream);
                                readable = true;
                                var labactivity = db.LabActivities.Where(x => x.LabActivityID == labactivityid).FirstOrDefault();
                                labactivity.LabAnswerKey = blockBlob.Uri.AbsoluteUri;
                                labactivity.LabAnswerKeyName = file.Headers.ContentDisposition.FileName;
                                db.Entry(labactivity).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        catch (IOException ioex)
                        {
                            retries++;
                            Console.WriteLine(ioex.Message);
                            Thread.Sleep(10000);
                        }
                    }
                 

                    uri = blockBlob.Uri.AbsoluteUri;
                }
                return Created("Upload Successful", uri);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}