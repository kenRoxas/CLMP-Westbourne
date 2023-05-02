using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class MachineLabs
    {
        [Key]
        public int MachineLabsId { get; set; }
        public string ResourceId { get; set; }
        public int VEProfileId { get; set; }
        public int UserId { get; set; }
        public string MachineStatus { get; set; }
        public int IsStarted { get; set; }
        public int IsDeleted { get; set; }
        public string ScheduledBy { get; set; }
        public string MachineName { get; set; }
        public string GuacDNS { get; set; }
        public string DateProvision { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FQDN { get; set; }
        public int RunningBy { get; set; }
        public string VMName { get; set; }
        public string IpAddress { get; set; }
    }

    //public class MacStorageAccounts
    //{
    //    [Key]
    //    public int StorageId { get; set; }
    //    public int UserId { get; set; }
    //    public string StorageURL { get; set; }
    //    public string StudentIdentifier { get; set; }
    //    public DateTime ModifiedDate { get; set; }
    //}

    public class MachineLabsContent
    {
        //public List<MachineUserContent> VMUserId;
        public List<int> UserId { get; set; }
        public int VEProfileId { get; set; }
    }

    public class MachineGrants
    {
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public int? IsStarted { get; set; }
        public int? IsCourseGranted { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameEmail { get; set; }
        public string Email { get; set; }
        public Int64 LabHoursRemaining { get; set; }
        public double? LabHoursTotal { get; set; }
        public string hasHours { get; set; }
        public double? CourseHours { get; set; }
    }
    public class MachineGrantsList
    {
        public List<MachineGrants> MachineUsers { get; set; }
        public string totalCount { get; set; }
        public List<MachineGrants> AllUsers { get; set; }
    }
    public class VMSize
    {
        [Key]
        public string Size { get; set; }
        public double MemoryInGb { get; set; }
        public double CPU { get; set; }
        public double TempStorageInGb { get; set; }
    }

    public class VMSuccess
    {
        public string ResourceId { get; set; }
        public string ComputerName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Region { get; set; }
        public string Size { get; set; }
        public string Fqdn { get; set; }
        public string OsType { get; set; }
        public string Status { get; set; }
        public string LastStatus { get; set; }
        public string ProvisioningStatus { get; set; }
        public bool IsReadyForUsage { get; set; }
        public string DeploymentDuration { get; set; }
        public string VirtualMachineName { get; set; }
    }

    public class AWSJson
    {
        public string account_id { get; set; }
        public EC2 ec2_details { get; set; }
        public string region { get; set; }
        public string root { get; set; }


    }
    public class EC2
    {
        public string InstanceType { get; set; }
        public int MaxCount { get; set; }
        public int MinCount { get; set; }
        public string ImageId { get; set; }
        public string KeyName { get; set; }
        public string[] SecurityGroupIds { get; set; }
        public List<TagSpecifications> TagSpecifications { get; set; }

    }

    public class TagSpecifications
    {
        public string ResourceType { get; set; }
        public List<Tags> Tags { get; set; }
    }
    public class Tags
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class AWSSize
    {
        [Key]
        public int Id { get; set; }
        public string Size { get; set; }
    }

    public class MachineGrantsSuperAd
    {
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public int? IsStarted { get; set; }
        public int IsCourseGranted { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullNameEmail { get; set; }
        public string Email { get; set; }
        public Int64 LabHoursRemaining { get; set; }
        public double? LabHoursTotal { get; set; }
        public string hasHours { get; set; }
        public double? TimeRemaining { get; set; }
        public string roleid { get; set; }
        public int MachineStatus { get; set; }
    }

    public class HeartBeatAWS
    {
        public string student_id { get; set; }
        public string instance_id { get; set; }
        public int minutes_rendered { get; set; }
    }

    public class MacHostSuccess
    {
        public string AutoPlacement { get; set; }
        public string AvailabilityZone { get; set; }

        public AvailableCap AvailableCapacity { get; set; }
        public string HostId { get; set; }

        public HostProp HostProperties { get; set; }

        public List<string> Instances { get; set; }
        public string State { get; set; }
        public DateTime AllocationTime { get; set; }
        public List<string> Tags { get; set; }
        public string HostRecovery { get; set; }
        public string AllowsMultipleInstanceTypes { get; set; }
        public string OwnerId { get; set; }
        public string AvailabilityZoneId { get; set; }
        public bool MemberOfServiceLinkedResourceGroup { get; set; }
    }
    public class HostProp
    {
        public int Cores { get; set; }
        public string InstanceType { get; set; }
        public int Sockets { get; set; }
        public int TotalVCpus { get; set; }
    }

    public class AvailableCap
    {
        public List<AvailableInstanceCapacity> AvailableInstanceCapacity { get; set; }
        public int AvailableVCpus { get; set; }
   
    }
    public class AvailableInstanceCapacity
    {
        public int AvailableCapacity  { get; set; }
        public string InstanceType { get; set; }
        public int TotalCapacity { get; set; }
    }

    public class CreateMacInstance
    {
        public string Message { get; set; }
        public string Instance_Id { get; set; }

    }

    public class CreateHost
    {
        public List<string> HostIds { get; set; }
        public ResponseMetaData ResponseMetaDatas { get; set; }
    }

    public class ResponseMetaData
    {
        public string RequestId { get; set; }
        public int HttpStatusCode { get; set; }
        public int RetryAttempts { get; set; }
    }

    public class VMDeleteDetails
    {
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public string SubscriptionId { get; set; }
        public string TenantId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }
        public string DeletedBy { get; set; }
        public string ClientCode { get; set; }
        public string VirtualMachines { get; set; }
        public string NewImageName { get; set; }

    }
    public class BulkProvision
    {
        public string Email { get; set; }
        public string VEName { get; set; }
    }
    public class StartBulk
    {
        public string VMName { get; set; }
        public string ResourceId { get; set; }
        public int RunBy { get; set; }
    }
    public class Bulk
    {
        public string Email { get; set; }
    }
    public class BulkGrade
    {
        public string Email { get; set; }
        public int VEProfile { get; set; }
    }
}