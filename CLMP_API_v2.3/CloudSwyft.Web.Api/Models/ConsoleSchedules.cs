using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class ConsoleSchedules
    {
        [Key]
        public int ConsoleId { get; set; }
        public Int64 RequestId { get; set; }
        public string ConsoleLink { get; set; }
        public string Email { get; set; }
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public int IsProvisioned { get; set; }
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public string AccountEmail { get; set; }
        public string AccountPassword { get; set; }
        public string AccountName { get; set; }
        public Int64 AccountId { get; set; }
        public string OrgUnit { get; set; }
    }

    public class ConsoleDetailsCSV
    {
        public string TeamName { get; set; }
        public string Platform { get; set; }
        public string Email { get; set; }
        public int LabId { get; set; }
        public long LabGuid { get; set; }
        public bool Suspended { get; set; }
        public string price_ytd { get; set; }
        public string price_by_date_range { get; set; }
        public string Comments { get; set; }
    }
    public class SchedulesPerUserGroup
    {
        public string Email { get; set; }
        public int UserId { get; set; }
        public double? DaysRemaining { get; set; }
        public int VEProfileId { get; set; }
        public double? TimeRemaining { get; set; }
        public double? LabHoursTotal { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string DateProvision { get; set; }
        public string Name { get; set; }
        public string FullNameEmail { get; set; }
    }

    public class UsersData
    {
        public int Count { get; set; }

        public double? DaysRemaining { get; set; }

        public string ConsumedLabHours { get; set; }
        public string RemainingLabHours { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string DateProvision { get; set; }

    }
}