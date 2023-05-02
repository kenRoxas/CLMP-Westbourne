using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class LabActivity
    {
        [Key]
        public int LabActivityID { get; set; }

        public string Name { get; set; }

        public string Tasks { get; set; } // Format: ["Task 1", "Task 2", "Task 3"]

        public string TasksHtml { get; set; }

        public string LabAnswerKey { get; set; }

        public string LabAnswerKeyName { get; set; }

        public string CourseCode { get; set; }
    }

    public class LabActivityImage
    {
        public string imageFileName { get; set; }
    }
}