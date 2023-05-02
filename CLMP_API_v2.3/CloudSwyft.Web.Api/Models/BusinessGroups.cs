using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class BusinessGroups
    {
        [Key]
        public int BusinessGroupId { get; set; }
        public int BusinessTypeId { get; set; }
        public int UserGroupId { get; set; }
        public int? ModifiedValidity { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class CreateEditBusinessGroup
    {
        public string BusinessGroup { get; set; }
        public string UserGroupName { get; set; }
        public int? ModifiedValidity { get; set; }
        public string CreatedBy { get; set; }
    }
    public class BusinessGroupModel
    {
        public string UserGroupName { get; set; }
        public int? ModifiedValidity { get; set; }
        public string BusinessType { get; set; }
    }
}