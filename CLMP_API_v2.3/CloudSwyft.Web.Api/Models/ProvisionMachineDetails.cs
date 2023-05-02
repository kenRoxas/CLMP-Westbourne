using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class ProvisionMachineDetails

    {
        public VEProfileLabCreditMappings labCreditMapping;
        public List<User> CLUsers;
    }
    //public class ProvisionDetails

    //{
    //    public VEDetails labCreditMapping;
    //    public List<User> CLUsers;
    //}
    public class ProvisionDetails
    {
        private string rg;

        private string machineName;
        private string userName;
        private string password;
        private string resourceId;
        public string CLPrefix;
        public string ResourceId
        {
            get { return resourceId; }
            set { resourceId = Guid.NewGuid().ToString(); }
        }
        public string FQDN
        {
            get { return MachineName; }
        }
             
        public string ResourceGroup
        {
            get { return "CS-" + rg.ToUpper() + "-" + CLPrefix.ToUpper(); }
            set { rg = value; }
        }
        public string MachineName
        {
            get { return machineName.ToUpper(); }
            set { machineName = CLPrefix.ToUpper() + "-" + GenerateRandomName(CLPrefix.Count()); }

        }
        public string Username
        {
            get { return userName; }
            set { userName = GenerateUserNameRandomName(); }
        }
        public string Password
        {
            get { return password; }
            set { password = GeneratePasswordRandomName(); }
        }
        public string ImageName { get; set; }
        public string ScheduledBy { get; set; }
        public int VETypeID { get; set; }
        public int UserID { get; set; }
        public int VEProfileID { get; set; }
        public int TenantID { get; set; }
        public string Size { get; set; }

        public static string GenerateRandomName(int count)
        {
            var diff = 14 - count; // 16 kasama un -
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, diff)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GenerateUserNameRandomName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 7)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GeneratePasswordRandomName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 12)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result+"!"}";
        }
    }
    public class ProvisionDetailsCustom
    {
        private string resourceId;
        private string rg;
        public string CLPrefix;
        public string ResourceId
        {
            get { return resourceId; }
            set { resourceId = Guid.NewGuid().ToString(); }
        }
        public string FQDN
        {
            get { return MachineName; }
        }

        public string ResourceGroup
        {
            get { return "CS-"+ rg.ToUpper() + "-" + CLPrefix.ToUpper(); }
            set { rg = value; }

        }
        public string MachineName { get; set; }
        public string Username { get; set; }
        
        public string Password { get; set; }        
        public string ImageName { get; set; }
        public string ScheduledBy { get; set; }
        public int VETypeID { get; set; }
        public int UserID { get; set; }
        public int VEProfileID { get; set; }
        public int TenantID { get; set; }
        public string Size { get; set; }

        public static string GenerateRandomName(int count)
        {
            var diff = 14 - count; // 16 kasama un -
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, diff)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GenerateUserNameRandomName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 7)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GeneratePasswordRandomName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 12)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result + "!"}";
        }
       
    }
    
    public class VHDDetails
    {
        public string ImageName { get; set; }
        public string Size { get; set; }
        public string NewImageName { get; set; }
    }

    public class DataProvision
    {
        public int[] UserId { get; set; }
        public int VEProfileID { get; set; }
        public Int64 CourseHours { get; set; }
        public Int64 NumberOfUsers { get; set; }
        public Int64 TotalCourseHours { get; set; }
        public Int64 TotalRemainingCourseHours { get; set; }
        public string MachineSize { get; set; }
        public VMEmptyData[] VMEmptyData { get; set; }
    }

    public class VMEmptyData
    {
        public string MachineName { get; set; }
        public int UserId { get; set; }
    }

    public class ProvisionData
    {
        private string machineName;
        private string userName;
        private string password;
        private string resourceId;
        public string CLPrefix;
        public string Environment;
        public string ResourceId
        {
            get { return resourceId; }
            set { resourceId = Guid.NewGuid().ToString(); }
        }     
        public string ResourceGroup { get; set; }
        public string VirtualMachineName
        { //CS-KENTJ-D-VM-12345-12456-12345-12345
            get { return machineName.ToUpper(); }
            set { machineName = "CS-" + CLPrefix.ToUpper() + Environment  + "-VM-"  + Guid.NewGuid(); }

        }
        public string Username
        {
            get { return userName; }
            set { userName = GenerateUserNameRandomName(); }
        }
        public string Password
        {
            get { return password; }
            set { password = GeneratePasswordRandomName(); }
        }
        public string ImageName { get; set; }
        public string ScheduledBy { get; set; }
        public int VETypeID { get; set; }
        public int UserID { get; set; }
        public int VEProfileID { get; set; }
        public int TenantID { get; set; }
        public string Size { get; set; }
        public string Location { get; set; }

        public static string GenerateRandomName(int count)
        {
            var diff = 14 - count; // 16 kasama un -
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, diff)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GenerateUserNameRandomName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 7)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result}";
        }
        public static string GeneratePasswordRandomName()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 12)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            return $"{result + "!"}";
        }
    }

}