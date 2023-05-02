using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudSwyft.Web.Api.Models
{
    public class CourseGrade
    {
        [Key]
        public int CourseGradeId { get; set; }
        public int UserId { get; set; }

        public string Email { get; set; }

        public string CourseCode { get; set; }

        public int? isPassed { get; set; }

        public int VEProfileId { get; set; }

        public string Name { get; set; }

    }


    public partial class gradeCourse
    {
        public string Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email  { get; set; }
        public int VEProfileId  { get; set; }
        public int? isPassed  { get; set; }
        public string CourseCode  { get; set; }
        public List<string> ThumbnailUpload  { get; set; }
        public string GuacamoleSrc { get; set; }
        public double ? HoursRemaining { get; set; }
        public string MachineStatus { get; set; }
        public string ResourceId { get; set; }
        public int IsStarted { get; set; }
        public string NameEmail { get; set; }
        public bool isExtend { get; set; }

    }

}