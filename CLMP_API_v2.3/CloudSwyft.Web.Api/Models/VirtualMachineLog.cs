using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualMachineLog
    {
        [Key]
        public int VirtualMachineLogID { get; set; }
        [Required]
        public string RoleName { get; set; } 
        [Required]
        public int UserID { get; set; }
        [Required]
        public int VEProfileID { get; set; }
        public int VirtualMachineID { get; set; }
        [Required]
        public string TimeStamp { get; set; }
        public string Comment { get; set; } 
    }

    public class VirtualMachineLogVM
    {
        [Key]
        public int VirtualMachineLogID { get; set; }
        [Required]
        public string RoleName { get; set; }
        [Required]
        public int CourseID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int VEProfileID { get; set; }
        public int VirtualMachineID { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }
        public string Comment { get; set; }
        public int MachineInstance { get; set; }
        public string MachineName { get; set; }
    }
}