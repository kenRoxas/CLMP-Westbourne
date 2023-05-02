using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class CloudService
    {
        public string ServiceName { get; set; }
        public string Url { get; set; }
        public Deployments Deployments { get; set; }
    }

    public class Deployment
    {
        public string Name { get; set; }
        public string PrivateID { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        public RoleInstanceList RoleInstanceList { get; set; }
    }

    public class Deployments
    {
        public List<Deployment> Deployment { get; set; }
    }

    public class RoleInstance
    {
        public string RoleName { get; set; }
        public string InstanceName { get; set; }
        public string IpAddress { get; set; }
        public string PowerState { get; set; }
        public string HostName { get; set; }
        public InstanceEndpoints InstanceEndpoints { get; set; }
    }

    public class RoleInstanceList
    {
        public List<RoleInstance> RoleInstance { get; set; }
    }

    public class InstanceEndpoint
    {
        public string Name { get; set; }
        public string Vip { get; set; }
        public string PublicPort { get; set; }
        public string LocalPort { get; set; }
        public string Protocol { get; set; }
    }

    public class InstanceEndpoints
    {
        public List<InstanceEndpoint> InstanceEndpoint { get; set; }
    }
}