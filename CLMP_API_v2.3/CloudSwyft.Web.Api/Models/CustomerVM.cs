using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualMachineDetails
    {
        [Key]
        public int VirtualMachineId { get; set; }
        public string ResourceId { get; set; }
        public int Status { get; set; }
        public string VMName { get; set; }
        public string FQDN { get; set; }
        public string OperationId { get; set; }
        public DateTime DateLastModified { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class VMStatus
    {
        [Key]
        public int VMStatusId { get; set; }
        public int Id { get; set; }
        public string Status { get; set; }
    }
}