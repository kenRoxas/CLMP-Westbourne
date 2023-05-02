using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSwyft.Web.Api.Models
{
    public class VEProfileLabActivities
    {
        [Key]
        public int VEProfileID { get; set; }
        
        public int LabActivityID { get; set; }
    }
}