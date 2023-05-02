using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using MySql.Data.Entity;
using System.Data;
using System.Data.Entity;

namespace CloudSwyft.Web.Api.Models
{

    public class User
    {
        public virtual int UserId { get; set; }
        public virtual string Username { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Token { get; set; }
        public virtual string Role { get; set; }
        public virtual string SessionId { get; set; }
        public virtual DateTime LoginDateTime { get; set; }
        public virtual string Email { get; set; }
        public virtual DateTime LastAccess { get; set; }
        public virtual DateTime LastLogin { get; set; }
        public virtual string Id { get; set; } // to accomodate calls from oauth
        public virtual string CreatedBy { get; set; }
        // by Mau
        public virtual int ? labHoursTotal { get; set; }
        public virtual int ? labHoursRemaining { get; set; }
        public string MachineName { get; set; }
    }


    public class UsersRolesMachineBindingModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }
        public int IsStarted { get; set; }
    }

    public interface IUser
    {
        User GetUserInfo(User user);
        List<User> GetUsersByCourse(int id);
        User GetUserById(int id);
    }

    public class AdminInstModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public int UserGroup { get; set; }
        public string Email { get; set; }
        public string UserRole { get; set; }
    }

}