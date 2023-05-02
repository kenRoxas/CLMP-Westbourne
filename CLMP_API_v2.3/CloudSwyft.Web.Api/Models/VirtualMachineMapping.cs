using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualMachineMappings
    {
        [Key]
        public string ResourceID { get; set; }
        public string VMName { get; set; }
        public int UserID { get; set; }
        public int VEProfileID { get; set; }
        public int IsProvisioned { get; set; }
        public int isDeleted { get; set; }
        public string Status { get; set; }
        public string ScheduledBy { get; set; }
    }
}