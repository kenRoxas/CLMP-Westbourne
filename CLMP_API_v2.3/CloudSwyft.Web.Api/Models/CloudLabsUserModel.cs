using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class CloudLabUsers
    {
        public string Id { get; set; }        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }  
        public bool EmailConfirmed { get; set; }

        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public int TenantId { get; set; }
        public bool CredentialsSent { get; set; }
        public string ThumbNail { get; set; }

        public bool isDeleted { get; set; }
        public string CreatedBy { get; set; }
        public bool isDisabled { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserId { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }

        public int UserGroup { get; set; }
        public string UserIdLTI { get; set; }

    }

    public class UserProvCL
    {
        public int UserId { get; set; }
        public string Email { get; set; }
    }
    
    public class AllUsers
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsDeleted { get; set; }
        public string Name { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string CreatedBy { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Thumbnail { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
    }

}