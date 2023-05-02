using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MySql.Data.Entity;
using System.Data;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace CloudSwyft.Web.Api.Models
{

    public class ConsoleDetail
    {
        public int VEProfileID { get; set; }
        public string AccountEmail { get; set; }
        public string AccountName { get; set; }
        public long AccountID { get; set; }
        public object Data_transfer_budget_limit { get; set; }
        public object Cost_budget_limit { get; set; }
        public object Actual_data_transfer_spend { get; set; }
        public object Actual_costs_spend { get; set; }
        public bool Is_suspended { get; set; }
        public bool SuspendProgress { get; set; }
        public string AccountPassword { get; set; }
    }

    public class BudgetLimit
    {
        public string Amount { get; set; }
        public string Unit { get; set; }
    }

    public class DataSpend
    {
        public ActualSpend ActualSpend;
    }

    public class ActualSpend
    {

        public string Amount { get; set; }
        public string Unit { get; set; }
    }

    public class CourseGrants
    {
        [Key]
        public int AccessID { get; set; }
        public int UserID { get; set; }
        public int VEProfileID { get; set; }
        public int VEType { get; set; }
        public bool IsCourseGranted { get; set; }
        public int? GrantedBy { get; set; }
    }

    public class ConsoleAccessDetail
    {
        [Key]
        public int AccessId { get; set; }
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public int VEType { get; set; }
        public int UserGroup { get; set; }
        public bool IsCourseGranted { get; set; }
    }

    public class GrantConsoleDetails
    {
        public List<ConsoleAccessDetail> ConsoleUsers;
    }

    public class ConsoleUserDetails
    {
        public List<ConsoleResultChecker> ConsoleUserID;
        public int VEType;
        public int VEProfileID;
    }

    public class ConsoleResultChecker
    {
        public int UserId { get; set; }
        public int VEProfileId { get; set; }
        public int VEType { get; set; }
        public int IsCourseGranted { get; set; }
        public int? IsProvisioned { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullNameEmail { get; set; }
    }

    public class SuspendedDetails
    {
        public string account_id { get; set; }
        public string suspend { get; set; }
    }
}
