using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualEnvironmentDBCustomerVMContext : DbContext
    {
        public VirtualEnvironmentDBCustomerVMContext() : base("name=CustomerVM")
        {
        }
        public DbSet<VirtualMachineDetails> VirtualMachineDetails { get; set; }
        public DbSet<VMStatus> VMStatus { get; set; }

    }
}