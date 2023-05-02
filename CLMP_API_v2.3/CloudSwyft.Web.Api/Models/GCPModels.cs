using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class GCPModels
    {
    }

    public class Project
    {
        public List<Results> results { get; set; }
    }
    public class Results
    {
        public int id { get; set; }
        public string parent { get; set; }
        public string project_id { get; set; }
        public string state { get; set; }
        public string display_name { get; set; }
    }
    public class VCP
    {
        public List<ResultsVCP> results { get; set; }
    }
    public class ResultsVCP
    {
        public int id { get; set; }
        public string name { get; set; }
        public string region { get; set; }
        public string status { get; set; }
        public string project_id { get; set; }
    }
    public class SubNet
    {
        public List<ResultsSubNet> results { get; set; }
        public class ResultsSubNet
        {
            public int id { get; set; }
            public string name { get; set; }
            public string ip_range { get; set; }
            public string network { get; set; }
            public string network_name { get; set; }
            public string region { get; set; }
            public string project_id { get; set; }
        }
    } 
    public class ProjFamily
    {
        public List<ResultsFamily> data { get; set; }
        public class ResultsFamily
        {
            public string name { get; set; }

        }
    }
    public class ProjAMIFamily
    {
        public ImagesAMI data { get; set; }
        public class ImagesAMI
        {
            public List<ImagesFamilyAMI> images { get; set; }
        }
        public class ImagesFamilyAMI
        {
            public string id { get; set; }
            public string kind { get; set; }
            public string name { get; set; }
            public string disk_size_gb { get; set; }
            public string status { get; set; }
            public string description { get; set; }
            public string source_type { get; set; }
            public string deprecated { get; set; }

        }
    }
    public class MachineType
    {
        public List<ResultsMachine> machine_types { get; set; }
        public class ResultsMachine
        {
            public string name { get; set; }
            public string description { get; set; }
            public int guestCpus { get; set; }
            public int memoryMb { get; set; }

        }
    }
    public class VMPayload
    {
        public VMInfoPayload data { get; set; }
    }
    public class VMInfoPayload
    {
        public string instance_id { get; set; }
        public string instance_name { get; set; }
        public string user { get; set; }
        public string vm_pass { get; set; }
        public string status { get; set; }
        public string project_id { get; set; }
        public string zone { get; set; }
        public string image_project { get; set; }
        public string image_os { get; set; }
        public string disk_size_gb { get; set; }
        public string network_i_p { get; set; }
        public string nat_i_p { get; set; }
        public string disk_os { get; set; }
        public string machine_type { get; set; }
        public string network { get; set; }
        public string subnetwork { get; set; }
        public string family { get; set; }
    }


}