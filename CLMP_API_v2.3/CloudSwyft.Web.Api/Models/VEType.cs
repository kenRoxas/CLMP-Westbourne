using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class VEType
    {
        [Key]
        public int VETypeID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public string ThumbnailUrl { get; set; }
    }
}