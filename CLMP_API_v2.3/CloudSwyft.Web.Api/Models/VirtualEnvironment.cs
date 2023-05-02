using CloudSwyft.Web.Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VirtualEnvironment
    {
        [Key]
        public int VirtualEnvironmentID { get; set; }

        [ForeignKey("VEType")]
        public int VETypeID { get; set; }
        public VEType VEType { get; set; }

        [Required]
        public string Title { get; set; }
        public string Description { get; set; }

    }


}