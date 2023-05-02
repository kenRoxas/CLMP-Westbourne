using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class GuacamoleInstance
    {
        [Key]
        public int GuacamoleInstanceID { get; set; }
        public string Hostname { get; set; }
        public string Connection_Name { get; set; }
        public string Url { get; set; }
        [DefaultValue(1)]
        public int GuacLinkType { get; set; }

        [ForeignKey("VirtualMachine")]
        public int VirtualMachineID { get; set; }
        public VirtualMachine VirtualMachine { get; set; }
    }
}