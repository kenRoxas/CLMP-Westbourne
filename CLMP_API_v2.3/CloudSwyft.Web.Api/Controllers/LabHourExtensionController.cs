using CloudSwyft.Web.Api.DataAccess;
using CloudSwyft.Web.Api.Managers;
using CloudSwyft.Web.Api.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CloudSwyft.Web.Api.Controllers
{
    //[Authorize]
    [RoutePrefix("api/LabHourExtension")]
    public class LabHourExtensionController : ApiController
    {
        private ILabHourExtensionManager _labHourExtensionManager;

        [HttpGet]
        [Route("GetExtensionTypes")]
        public HttpResponseMessage GetExtensionTypes()
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                var response = _labHourExtensionManager.GetLabHourExtensionTypes();
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.InnerException);
            }
        }

        [HttpGet]
        [Route("GetUsersWithLabHourExtensions")]
        public HttpResponseMessage GetUsersWithLabHourExtensions(int veprofileid)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                var response = _labHourExtensionManager.GetUsersWithLabHourExtensions(veprofileid);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("GetUsersWithFixedLabHourExtensions")]
        public HttpResponseMessage GetUsersWithFixedLabHourExtensions([FromBody] GetUsersWithLabHourExtensionsRequest request)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                var response = _labHourExtensionManager.GetUsersWithFixedLabHourExtensions(request);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("GetUsersWithCustomLabHourExtensions")]
        public HttpResponseMessage GetUsersWithCustomLabHourExtensions([FromBody] GetUsersWithLabHourExtensionsRequest request)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                var response = _labHourExtensionManager.GetUsersWithCustomLabHourExtensions(request);
                return Request.CreateResponse(HttpStatusCode.OK, response);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
        }

        [HttpPost]
        [Route("Save")]
        public HttpResponseMessage SaveExtension([FromBody] SaveExtensionRequest request)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                _labHourExtensionManager.SaveLabHourExtension(request);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Saving of lab hour extension failed. ", ex));
            }
        }

        [HttpPost]
        [Route("Delete")]
        public HttpResponseMessage DeleteLabHourExtensionById([FromBody] DeleteLabHourExtensionRequest request)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                _labHourExtensionManager.DeleteLabHourExtensionById(request);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Deletion of lab hour extension failed. ", e));
            }
        }

        [HttpPost]
        [Route("Update")]
        public HttpResponseMessage UpdateLabHourExtension([FromBody] SaveExtensionRequest request)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                _labHourExtensionManager.UpdateLabHourExtension(request);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Updating of lab hour extension failed. ", e));
            }
        }

        [HttpPost]
        [Route("UpdateV2")]
        public HttpResponseMessage UpdateLabHourExtension([FromBody] UpdateExtensionRequest request)
        {
            try
            {
                if (_labHourExtensionManager == null)
                {
                    _labHourExtensionManager = new LabHourExtensionManager(new LabHourExtensionDataAccess());
                }

                _labHourExtensionManager.UpdateLabHourExtensionV2(request);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, new Exception("Updating of lab hour extension failed. ", e));
            }
        }
    }
}
