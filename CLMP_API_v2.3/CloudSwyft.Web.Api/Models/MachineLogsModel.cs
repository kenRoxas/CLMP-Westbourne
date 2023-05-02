using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class MachineLogs
    {
        [Key]
        public int MachineLogsId { get; set; }
        public string ResourceId { get; set; }
        public string RequestId { get; set; }
        public string LastStatus { get; set; }
        public string Logs { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}