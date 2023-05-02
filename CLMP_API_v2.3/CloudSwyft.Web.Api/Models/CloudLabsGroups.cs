using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class CloudLabsGroups
    {

        //public string OldGroupName;
        //public string OldEdxUrl;
        [Key]
        public int CloudLabsGroupID { get; set; }
        public string GroupName { get; set; }
        public string EdxUrl { get; set; }
        public int TenantId { get; set; }
        public string CLUrl { get; set; }
        public Int64 SubscriptionHourCredits { get; set; }
        public Int64 SubscriptionRemainingHourCredits { get; set; }
        public string CLPrefix { get; set; }
        public string CreatedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public string ApiPrefix { get; set; }
        //public int? TypeOfBusinessId { get; set; }

        //public int TotalLabHours { get; set; }
        //public int LabHoursPerCourse { get; set; }
        //public int TotalRemainingLabHours { get; set; }
    }
    public class UserGroups
    {

        //public string OldGroupName;
        //public string OldEdxUrl;
        [Key]
        public int CloudLabsGroupID { get; set; }
        public string GroupName { get; set; }
        public string EdxUrl { get; set; }
        public int TenantId { get; set; }
        public string CLUrl { get; set; }
        public Int64 SubscriptionHourCredits { get; set; }
        public Int64 SubscriptionRemainingHourCredits { get; set; }
        public string CLPrefix { get; set; }
        public string Environment { get; set; }
        public string ApiPrefix { get; set; }
        public string TypeOfBusinessName { get; set; }
    }
    public class UsersByUserGroup
    {
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string GroupName { get; set; }
        public string Role { get; set; }
        public DateTime DateCreated { get; set; }
    }
}