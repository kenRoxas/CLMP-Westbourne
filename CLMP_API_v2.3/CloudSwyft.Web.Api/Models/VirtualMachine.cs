using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualMachine
    {
        [Key]
        public int VirtualMachineID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int VEProfileID { get; set; }
        [Required]
        public int CourseID { get; set; }
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public string Port { get; set; }
        public int IsStarted { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public string NetworkID { get; set; }
        public DateTime DateStartedTrigger { get; set; }
        public string VirtualMachineType { get; set; }
        public int MachineInstance { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class VirtualMachineBindingModel
    {
        public int VirtualMachineID { get; set; }
        public int UserID { get; set; }
        public User UserEntry { get; set; }
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }
        public string ServiceName { get; set; }
        public string RoleName { get; set; }
        public int GuacamoleInstanceID { get; set; }
        public GuacamoleInstance GuacamoleInstance { get; set; }
        public string Port { get; set; }
        public int IsStarted { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public string NetworkID { get; set; }
        public DateTime DateStartedTrigger { get; set; }
        public string VirtualMachineType { get; set; }
        public int MachineInstance { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class VirtualMachinePostModel
    {
        public int UserID { get; set; }
        
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }
        public string ServiceName { get; set; }
        public string RoleName { get; set; }        
        public List<GuacamoleInstance> GuacamoleInstances { get; set; }
        public int IsStarted { get; set; }
        public DateTime LastTimeStamp { get; set; }
        public string NetworkID { get; set; }
        public DateTime DateStartedTrigger { get; set; }
        public string VirtualMachineType { get; set; }
        public int MachineInstance { get; set; }
        public DateTime DateCreated { get; set; }
        public int LabHoursPerCourse { get; set; }
    }

    public class VMSendMessage
    {
        public string Rolename { get; set; }
        public string Machinename { get; set; }
        public string ApiUrl { get; set; }
        public string ResourceGroup { get; set; }
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
    }
    public class VMStats
    {
        public string ResourceId { get; set; }
        public string LastStatusDescription { get; set; }
        public bool IsDeleted { get; set; }
        public string ProvisioningStatus { get; set; }
        public string DeploymentDuration { get; set; }
        public string VirtualMachineName { get; set; }
        public bool IsReadyForUsage { get; set; }
    }

}