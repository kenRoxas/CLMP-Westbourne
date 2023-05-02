using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;


namespace CloudSwyft.Web.Api.Models
{
    public class Regions
    {
        [Key]
        public int RegionId { get; set; }
        public string RegionName { get; set; }
    }
    public class AzureRegions
    {
        public string Location { get; set; }
        public string Region { get; set; }
    }
    public class DataRegions
    {
        public List<string> zones { get; set; }
    }
}