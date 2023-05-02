using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VEProfileLabCreditMappings
    {
        [Key]
        [Column(Order = 1)]
        public int VEProfileID { get; set; }
        [Key]
        [Column(Order = 2)]
        public int GroupID { get; set; }

        //public Int64 TotalLabHours { get; set; }
        //public Int64 TotalRemainingHours { get; set; }
        //public int LabHoursPerCourse { get; set; }
        public Int64 CourseHours { get; set; }
        public Int64 NumberOfUsers { get; set; }
        public Int64 TotalCourseHours { get; set; }
        public Int64 TotalRemainingCourseHours { get; set; }

        public Nullable<int> TotalRemainingContainers { get; set; }
        public string MachineSize { get; set; }
        public int? DiskSize { get; set; }
    }

    public class IDs
    {
        public int VEProfileID { get; set; }
        public int GroupID { get; set; }
    }

    public class VEProfileCreditGroups
    {
        public string CourseName { get; set; }
        public int VEProfileId { get; set; }
        public int VirtualEnvironmentId { get; set; }
        public int ProvisionCount { get; set; }
        public Int64 CourseHours { get; set; }
        public int NumberOfUsers { get; set; }
        public Int64 TotalCourseHours { get; set; }
        public Int64 TotalRemainingCourseHours { get; set; }
        public int GroupId { get; set; }
        public Int64 SubscriptionHourCredits { get; set; }
        public Int64 SubscriptionRemainingHourCredits { get; set; }
        public Nullable<int> TotalRemainingContainers { get; set; }
        public string MachineSize { get; set; }
        public int? DiskSize { get; set; }
    }

    public class VEDetails
    {
      
        public int VEProfileID { get; set; }
        public int GroupID { get; set; }

        public Int64 CourseHours { get; set; }
        public Int64 NumberOfUsers { get; set; }
        public Int64 TotalCourseHours { get; set; }
        public Int64 TotalRemainingCourseHours { get; set; }
        public string Region { get; set; }
        public string Size { get; set; }
    }
}