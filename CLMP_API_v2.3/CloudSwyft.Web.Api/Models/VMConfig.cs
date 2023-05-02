using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VMConfig
    {
        [Key]
        public int HeartbeatInterval { get; set; }
        public int MaxIdleTime { get; set; }
        public int MaxJobIdleTime { get; set; }
    }
}