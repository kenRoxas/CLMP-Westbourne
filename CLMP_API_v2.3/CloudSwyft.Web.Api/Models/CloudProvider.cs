using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSwyft.Web.Api.Models
{
    public class CloudProvider
    {
        [Key]
        public int CloudProviderID { get; set; }
        [Required]
        public string Name { get; set; }
        public int  IsDisabled { get; set; }
    }


}
