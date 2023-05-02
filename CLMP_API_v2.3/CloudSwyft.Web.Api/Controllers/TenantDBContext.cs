using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Controllers
{

    public class TenantDBContext : DbContext
    {
        public TenantDBContext() : base("name=TenantURL")
        {
        }

        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<AzTenant> AzTenants { get; set; }
        public DbSet<EnvironmentAPI> EnvironmentAPIs { get; set; }
    }
}