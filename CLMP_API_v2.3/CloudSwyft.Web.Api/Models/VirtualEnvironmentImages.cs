using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CloudSwyft.Web.Api.Models;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualEnvironmentImages
    {
        [Key]
        public int VirtualEnvironmentImagesID { get; set; }
        [ForeignKey("VirtualEnvironment")]
        public int VirtualEnvironmentID { get; set; }
        public VirtualEnvironment VirtualEnvironment{ get; set; }
        [Required]
        public string Name { get; set; }
        public int? GroupId { get; set; }
        public string ProjectFamily { get; set; }
        public string ImageFamily { get; set; }
        public string ImageFamilyMinDiskSize { get; set; }
    }

    public class StorageName
    {
        public string storageAccountName { get; set; }
        public string connectionstring { get; set; }
        public string storageKey { get; set; }
    }

}