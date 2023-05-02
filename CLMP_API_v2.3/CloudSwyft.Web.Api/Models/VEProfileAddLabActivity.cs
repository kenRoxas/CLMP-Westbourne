using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VEProfileAddLabActivity
    {
        public int VEProfileID { get; set; }
        public List<LabActivityReturn> LabActivities { get; set; }
    }
}