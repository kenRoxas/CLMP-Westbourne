using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VEProfile
    {
        [Key]
        public int VEProfileID { get; set; }

        [ForeignKey("VirtualEnvironment")]
        public int VirtualEnvironmentID { get; set; }
        public VirtualEnvironment VirtualEnvironment { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailURL { get; set; }
        public DateTime DateCreated { get; set; }
    }

    public class LabProfileModel
    {
        [Key]
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }

        [ForeignKey("VirtualEnvironment")]
        public int VirtualEnvironmentID { get; set; }
        public VirtualEnvironment VirtualEnvironment { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public int ConnectionLimit { get; set; }
        public string ThumbnailURL { get; set; }
        public int IsEnabled { get; set; }
        public DateTime DateProvisionTrigger { get; set; }
        public int Status { get; set; }
        public string Remarks { get; set; }
        public int IsEmailEnabled { get; set; }
        public int PassingRate { get; set; }
        public int ExamPassingRate { get; set; }
        public bool ShowExamPassingRate { get; set; }
    }

    public class VEProfileViewModel2
    {
        public List<VEProfile> VEProfiles { get; set; }
        public int PageSize { get; set; }
        public int ActivePage { get; set; }
        public int TotalPages { get; set; }
        public string SearchFilter { get; set; }

        public int TotalItems { get; set; }
        //public List<ViewList> ViewList { get; set; }
    }

    public class ViewList
    {
        public int CourseID { get; set; }
        public string Description { get; set; }
        public int IsEnabled { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public string ThumbnailURL { get; set; }
        public int VEProfileID { get; set; }
        public List<VirtualEnvironment> VirtualEnvironment { get; set; }
        public int VirtualEnvironmentID { get; set; }
    }

    public class UserVeProfile
    {
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
    }

    public class CourseDetails
    {
        public double? TimeRemaining { get; set; }
        public string ResourceId { get; set; }
        public double? LabHoursTotal { get; set; }
        public double? InstructorLabHours { get; set; }
        public int veprofileid { get; set; }
        public string Name { get; set; }
        public string RoleName { get; set; }
        public string MachineName { get; set; }
        public int UserId { get; set; }
        public int? IsStarted { get; set; }
        public string Thumbnail { get; set; }
        public bool IsCourseGranted { get; set; }
        public int IsProvisioned { get; set; }
        public string CourseCode { get; set; }
        public int VEType { get; set; }
        public string GuacamoleUrl { get; set; }
        public string ConsoleLink { get; set; }
        public string AccountEmail { get; set; }
        public string AccountName { get; set; }
        public long AccountID { get; set; }
        public object Data_transfer_budget_limit { get; set; }
        public object Cost_budget_limit { get; set; }
        public object Actual_data_transfer_spend { get; set; }
        public object Actual_costs_spend { get; set; }
        public bool Is_suspended { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string MachineSize { get; set; }
        public string MachineStatus { get; set; }
        public string FQDN { get; set; }
        public int RunningBy { get; set; }
        public int IsProvisioning { get; set; }

        public bool IsExtend { get; set; }
        public string IpAddress { get; set; }
        public int MachineLabsId { get; set; }
    }
}