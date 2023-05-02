using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualMachineLogStats
    {
        [Key]
        public long VirtualMachineLogStatsId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string LastStatus { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public int VeProfileId { get; set; }
        public int MachineInstance { get; set; }
    }
}