using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSwyft.CloudLabs.Models
{
    public class TenantCode
    {
        [Key]
        public int TenantCodeId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ApiUrl { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        [MaxLength(3)]
        public string Code { get; set; }

        public string DbName { get; set; }
        public string DbHost { get; set; }
        public string DbUser { get; set; }
        public string DbPass { get; set; }

        public string FTPHost { get; set; }
        public string FTPUser { get; set; }
        public string FTPPass { get; set; }
        public string AzurePort { get; set; }
        public string GuacConnection { get; set; }
        public string GuacamoleUrl { get; set; }
        public string Region { get; set; }
    }
}