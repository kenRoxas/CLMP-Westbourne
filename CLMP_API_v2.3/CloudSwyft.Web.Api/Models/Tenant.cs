using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class Tenant
    {
        [Key]
        public int TenantId { get; set; }
        [Required]
        public string ApiUrl { get; set; }
        public string AuthConnectionString { get; set; }
        [Required]
        [MaxLength(3)]
        public string Code { get; set; }
        public string EdxUrl { get; set; }
        public string TenantName { get; set; }
        public string GuacConnection { get; set; }
        public string GuacamoleURL { get; set; }
        public int SubscriptionMinutes { get; set; }
        public string AzurePort { get; set; }
        public string SubscriptionId { get; set; }

    }

    public class AzTenant
    {
        [Key]
        public int TenantId { get; set; }
        // public string TenantKey { get; set; }
        //public string Location { get; set; }
        public string ClientCode { get; set; }
        public string ProjectName { get; set; }
        public string EnvironmentCode { get; set; }
        //public string SubscriptionKey { get; set; }
        public string GuacConnection { get; set; }
        public string Regions { get; set; }
        public string RegionGCP { get; set; }
        public string GuacamoleURL { get; set; }
        public string CreatedBy { get; set; }
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public DateTime DateCreated { get; set; }
        public string SubscriptionId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationTenantId { get; set; }
        public string ApplicationSecretKey { get; set; }
        public int BusinessId { get; set; }
        public string VPCNetworkGCP { get; set; }
        public string VPCSubNetworkGCP { get; set; }
    }

    public class EnvironmentAPI
    {
        [Key]
        public string EnvironmentId { get; set; }
        public string Name { get; set; }
        public string EnvironmentVMURL { get; set; }
        public string ClmpURL { get; set; }
        public string EnvironmentCode { get; set; }
    }

    public class VMInfo
    {
        public string TenantKey { get; set; }
        public string SubscriptionKey { get; set; }
        public string EnvironmentVMURL { get; set; }
        public string ResourceId { get; set; }
        public int RunningBy { get; set; }
        public List<LabHourExtension> LabHourExtension { get; set; }
        public double? TimeRemaining { get; set; }
        public double TimeRemainingInstructor { get; set; }
        public double DeductTime { get; set; }
        public int TenantId { get; set; }
    }
    public class CustomerTenants
    {
        [Key]
        public int TenantId { get; set; }
        public string ClientCode { get; set; }
        public string EnvironmentCode { get; set; }
        //public string SubscriptionKey { get; set; }
        public string GuacConnection { get; set; }
        public string Regions { get; set; }
        public string GuacamoleURL { get; set; }
        public string CreatedBy { get; set; }
        public string ClientKey { get; set; }
        public string ClientSecret { get; set; }
        public DateTime DateCreated { get; set; }
        public string SubscriptionId { get; set; }
        public string ApplicationId { get; set; }
        public string ApplicationTenantId { get; set; }
        public string ApplicationSecretKey { get; set; }
        public BusinessTypes BusinessTypes { get; set; }
        public string VPCNetworkGCP { get; set; }
        public string VPCSubNetworkGCP { get; set; }
        public string RegionGCP { get; set; }
        public string ProjectName { get; set; }
    }


}