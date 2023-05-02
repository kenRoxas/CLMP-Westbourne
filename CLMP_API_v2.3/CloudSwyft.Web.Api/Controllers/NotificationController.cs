//using CloudSwyft.Web.Api.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

//namespace CloudSwyft.Web.Api.Controllers
//{
//    [RoutePrefix("api/Notification")]
//    public class NotificationController : ApiController
//    {
//        private VirtualEnvironmentDbContext _db = new VirtualEnvironmentDbContext();
//        //post

//        [HttpPost]
//        [Route("GetAllNotification/{userGroup}/{roleName}")]
//        public IHttpActionResult GetAllNotification(int userGroup, string roleName)
//        {
//            try
//            {
//                if (roleName == "SuperAdmin")
//                {
//                    var superAdminContext = _db.Notifications.Where(q => q.IsDeleted == false && q.UserGroup == userGroup).ToList();
//                    if (superAdminContext != null)
//                    {
//                        foreach (var item in superAdminContext)
//                        {
                            
//                            if (item.EditedBy != null)
//                            {
//                                item.EditedBy = _db.CloudLabUsers.Where(q => q.Email == item.EditedBy).FirstOrDefault().FirstName + " "
//                                    + _db.CloudLabUsers.Where(q => q.Email == item.EditedBy).FirstOrDefault().LastName;
//                            }
//                            else
//                            {
//                                item.CreatedBy = _db.CloudLabUsers.Where(q => q.Email == item.CreatedBy).FirstOrDefault().FirstName + " "
//                                    + _db.CloudLabUsers.Where(q => q.Email == item.CreatedBy).FirstOrDefault().LastName;
//                            }
//                            var utcStartDate = DateTime.Parse(item.StartDate.ToString());
//                            var localStartDate = utcStartDate.ToLocalTime();
//                            item.StartDate = localStartDate;

//                            var utcEndDate = DateTime.Parse(item.EndDate.ToString());
//                            var localEndDate = utcEndDate.ToLocalTime();
//                            item.EndDate = localEndDate;

     

//                        }

                        
//                        return Ok(superAdminContext);
//                    }
//                }
//                else
//                {
//                    var currentDate = DateTime.Now.ToUniversalTime();
//                    var adminContext = _db.Notifications.Where(w => w.IsDeleted == false && w.UserGroup == userGroup && w.EndDate > currentDate).ToList();
//                    if (adminContext != null)
//                    {
//                        foreach (var item in adminContext)
//                        {

//                            if (item.EditedBy != null)
//                            {
//                                item.EditedBy = _db.CloudLabUsers.Where(q => q.Email == item.EditedBy).FirstOrDefault().FirstName + " "
//                                    + _db.CloudLabUsers.Where(q => q.Email == item.EditedBy).FirstOrDefault().LastName;
//                            }
//                            else
//                            {
//                                item.CreatedBy = _db.CloudLabUsers.Where(q => q.Email == item.CreatedBy).FirstOrDefault().FirstName + " "
//                                    + _db.CloudLabUsers.Where(q => q.Email == item.CreatedBy).FirstOrDefault().LastName;
//                            }
//                            var utcStartDate = DateTime.Parse(item.StartDate.ToString());
//                            var localStartDate = utcStartDate.ToLocalTime();
//                            item.StartDate = localStartDate;

//                            var utcEndDate = DateTime.Parse(item.EndDate.ToString());
//                            var localEndDate = utcEndDate.ToLocalTime();
//                            item.EndDate = localEndDate;



//                        }
//                    }
//                    return Ok(adminContext);
//                }

//            }
//            catch (Exception ex)
//            {
//                return InternalServerError(ex);
//            }


//            return InternalServerError();
//        }

//        [HttpPost]
//        [Route("InsertNotification")]
//        public IHttpActionResult InsertNotification(Notification notification)
//        {
//            try
//            {
//                var localStartDate = DateTime.Parse(notification.StartDate.ToString());
//                notification.StartDate = localStartDate.ToUniversalTime();

//                var localEndDate = DateTime.Parse(notification.EndDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999).ToString());
//                notification.EndDate = localEndDate.ToUniversalTime();

//                notification.DateModified = DateTime.Now;
//                notification.EditedBy = null;
//                _db.Notifications.Add(notification);
//                _db.SaveChanges();
//                return Ok(notification);
//            }
//            catch (Exception ex)
//            {
//                return InternalServerError(ex);
//            }

//        }
            
//        [HttpDelete]
//        [Route("DeleteNotification/{notificationId}")]
//        public IHttpActionResult DeleteNotification (int notificationId)
//        {
//           try
//            {
//                var existingNotification = _db.Notifications.SingleOrDefault(q => q.NotificationId == notificationId);
//                if (existingNotification != null)
//                {
//                    existingNotification.IsDeleted = true;
//                    _db.SaveChanges();
//                    return Ok("Ok");
//                }
//            }
//            catch (Exception ex)
//            {
//                return InternalServerError(ex);
//            }

//            return InternalServerError();

//        }

//        [HttpPost]
//        [Route("UpdateNotification/{notificationId}")]
//        public IHttpActionResult UpdateNotification (int notificationId, Notification notification)
//        {
//            try
//            {
//                var existingNotification = _db.Notifications.Find(notificationId);
//                if (existingNotification != null)
//                {
//                    var localStartDate = DateTime.Parse(notification.StartDate.ToString());
//                    notification.StartDate = localStartDate.ToUniversalTime();

//                    var localEndDate = DateTime.Parse(notification.EndDate.AddHours(23).AddMinutes(59).AddSeconds(59).AddMilliseconds(999).ToString());
//                    notification.EndDate = localEndDate.ToUniversalTime();

//                    existingNotification.StartDate = notification.StartDate;
//                    existingNotification.EndDate = notification.EndDate;
//                    existingNotification.DateModified = DateTime.Now;
//                    existingNotification.Message = notification.Message;
//                    existingNotification.UserGroup = notification.UserGroup;
//                    existingNotification.EditedBy = notification.EditedBy;
                    

//                    _db.SaveChanges();

//                    return Ok(existingNotification);
//                }
//            }
//            catch (Exception ex)
//            {
//                return InternalServerError(ex);
//            }

//            return InternalServerError();
            
//        }
//    }
//}
