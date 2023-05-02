using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualEnvironmentDbContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
        public VirtualEnvironmentDbContext() : base("name=VirtualEnvironmentDbContext")
        {
        }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VirtualEnvironment> VirtualEnvironments { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VEProfile> VEProfiles { get; set; }

        //public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VirtualMachine> VirtualMachines { get; set; }

        //public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.GuacamoleInstance> GuacamoleInstances { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.LabActivity> LabActivities { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VEType> VETypes { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VirtualMachineLog> VirtualMachineLogs { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VirtualMachineLogStats> VirtualMachineLogStats { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.CloudProvider> CloudProviders { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VirtualEnvironmentImages> VirtualEnvironmentImages { get; set; }
        //public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VirtualMachineMappings> VirtualMachineMappings { get; set; }
        //public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.Tenant> Tenant { get; set; }
        
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.CloudLabUsers> CloudLabUsers { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.CloudLabsSchedule> CloudLabsSchedule { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VMConfig> VMConfig { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VEProfileLabActivities> VEProfileLabActivities { get; set; }


        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.CloudLabsGroups> CloudLabsGroups { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VEProfileLabCreditMappings> VEProfileLabCreditMappings { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.CourseGrade> CourseGrades { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.Regions> Regions { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.ConsoleSchedules> ConsoleSchedules { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.CourseGrants> CourseGrants { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.MachineLabs> MachineLabs { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.MachineLogs> MachineLogs { get; set; }

        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.AspNetRoles> AspNetRoles { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.AspNetUserRoles> AspNetUserRoles { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.LabHourExtension> LabHourExtensions { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.LabHourExtensionType> LabHourExtensionTypes { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.AWSSize> AWSSizes { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.VMSize> VMSize { get; set; }
        //public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.Notification> Notifications { get; set; }
        //  public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.AutoDeletion> AutoDeletions { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.BusinessTypes> BusinessTypes { get; set; }
        public System.Data.Entity.DbSet<CloudSwyft.Web.Api.Models.BusinessGroups> BusinessGroups { get; set; }


    }
}
