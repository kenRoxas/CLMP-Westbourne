using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Auth.Models
{
    public class RegisterViewModel
    {
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Roles { get; set; }

        public string UserName { get; set; }

        public string CreatedBy { get; set; }
        public string Thumbnail { get; set; }

        public int UserId { get; }

        public int UserGroup { get; set; }
    }

    public class MeViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsDisabled { get; set; }
        public string Thumbnail { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
        public int TenantId { get; set; }
        public int UserGroup { get; set; }
        public string UserIdLTI { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Roles { get; set; }
        [Required]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Thumbnail { get; set; }

        public int UserGroup { get; set; }
    }

    public class EditViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Roles { get; set; }
        public string Thumbnail { get; set; }

        public int UserGroup { get; set; }
        public int TenantId { get; set; }
    }
    public class EditViewModelLTI
    {
        public string Id { get; set; }
        public string UserIdLTI { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

    }
}