//using CloudSwyft.Web.Api.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace CloudSwyft.Web.Api.Controllers
//{
//    [RoutePrefix("api")]
//    public class AutoDeletionController : ApiController

//    {
//        private VirtualEnvironmentDbContext db = new VirtualEnvironmentDbContext();

//        [HttpGet]
//        [Route("AutoDeletions/GetCloudLabsForAutoDeletion")]
//        public IHttpActionResult GetCloudLabsForAutoDeletion()
//        {

//            var typeOfBusinessArray = db.TypeOfBusinesses.ToList();


//            if (typeOfBusinessArray.Count > 0)
//            {
//                var typeOfBusinessId = db.TypeOfBusinesses.Where(q => q.TypeOfBusinessName == "semestral").FirstOrDefault().Id;
//                var filteredContext = db.CloudLabsGroups.Where(x => x.TypeOfBusinessId != typeOfBusinessId).ToList();

//                return Ok(filteredContext);
//            }
//            else
//            {
//                var context = db.CloudLabsGroups.ToList();
//                return Ok(context);
//            }


//        }

//        [HttpPost]
//        [Route("AutoDeletions/AddorEditScheduleForAutoDeletion")]
//        public IHttpActionResult AddorEditScheduleForAutoDeletion(AutoDeletion autoDeletionModel)
//        {
//            var existingDeletionModel = db.AutoDeletions.Where(x => x.UserGroupId == autoDeletionModel.UserGroupId).FirstOrDefault();
//            if (existingDeletionModel != null)
//            {
//                existingDeletionModel.UserGroupId = autoDeletionModel.UserGroupId;
//                existingDeletionModel.CreatedBy = existingDeletionModel.CreatedBy;
//                if (existingDeletionModel.EditedBy == null)
//                {
//                    existingDeletionModel.EditedBy = autoDeletionModel.EditedBy;
//                }
//                else
//                {
//                    existingDeletionModel.EditedBy = autoDeletionModel.EditedBy + ";" + existingDeletionModel.EditedBy;
//                }

//                existingDeletionModel.ModifiedDate = DateTime.Now;
//                existingDeletionModel.NumberOfDays = autoDeletionModel.NumberOfDays;

//                db.SaveChanges();
//                return Ok(existingDeletionModel);
//            }
//            else
//            {
//                autoDeletionModel.ModifiedDate = DateTime.Now;
//                autoDeletionModel.EditedBy = null;
//                db.AutoDeletions.Add(autoDeletionModel);
//                db.SaveChanges();

//                return Ok(autoDeletionModel);
//            }

//        }

//        [HttpPost]
//        [Route("AutoDeletions/CheckIfUserIdHasScheduleForAutoDeletion/{userGroupId}")]
//        public IHttpActionResult CheckIfUserIdHasScheduleForAutoDeletion(int userGroupId)
//        {
//            var context = db.AutoDeletions.Where(x => x.UserGroupId == userGroupId).FirstOrDefault();
//            if (context != null)
//            {
//                return Ok(context);
//            }
//            else
//            {
//                var errorMessage = "Nothing found";
//                return Ok(errorMessage);
//            }
//        }

//        [HttpGet]
//        [Route("TypeOfBusiness/GetAllTypeOfBusiness")]
//        public IHttpActionResult GetAllTypeOfBusiness()
//        {
//            var context = db.TypeOfBusinesses.ToList();
//            if (context != null)
//            {
//                return Ok(context);
//            }

//            return Ok();
//        }
//    }
//}
