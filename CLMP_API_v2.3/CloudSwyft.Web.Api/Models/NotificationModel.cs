using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public DateTime DateModified { get; set; }
        public string Message { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserGroup { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public string EditedBy { get; set; }

    }
}