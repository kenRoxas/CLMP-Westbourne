using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class BusinessTypes
    {
        [Key]
        public int BusinessId { get; set; }
        public string BusinessType { get; set; }
        public int Validity { get; set; }
        public bool IsCustomizable { get; set; }
    }
    public class BusinessTypesVMExists
    {
        [Key]
        public int BusinessId { get; set; }
        public string BusinessType { get; set; }
        public int Validity { get; set; }
        public bool IsCustomizable { get; set; }
        public bool IsVMExists { get; set; }
    }
}