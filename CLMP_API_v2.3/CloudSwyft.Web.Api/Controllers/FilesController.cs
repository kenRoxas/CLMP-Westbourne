using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using System.Web;
using System.Configuration;
using System.Threading;

namespace CloudSwyft.Web.Api.Controllers
{
    [Route("api/File")]
    public class FilesController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                if (!Request.Content.IsMimeMultipartContent())
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);

                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);
                var uri = string.Empty;

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await  Request.Content.ReadAsMultipartAsync(provider);
                
                foreach (var file in provider.FileData)
                {
                    bool readable = false;
                    int retries = 0;
                    var blockBlob = container.GetBlockBlobReference(file.Headers.ContentDisposition.FileName.Replace("\"", ""));
                    //new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read); File.OpenRead(file.LocalFileName)
                    while (!readable) {
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
                   // blockBlob.UploadFromStream(fileStream);
                    
                    uri = blockBlob.Uri.AbsoluteUri;
                }
                return Created("Upload Successful",uri);
            }
            catch (Exception ex)
            {
                
                return BadRequest(ex.Message);
            }
        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task<IHttpActionResult> Delete(string filename)
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AzureStorageConnectionString"]);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["ContainerName"]);

            var blockBlob = container.GetBlockBlobReference(filename);
            blockBlob.Delete();
            
            
            return Ok("Success: Image Deleted");

        }
    }
}
