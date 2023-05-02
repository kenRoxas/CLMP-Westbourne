using System;
using System.Collections.Generic;

namespace CloudSwyft.Web.Api.Models
{
    public class SaveExtensionRequest
    {
        public IEnumerable<int> LabHourExtensionIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int VEProfileId { get; set; }
        public int ExtensionTypeId { get; set; }
        public int? UserId { get; set; }
        public int CreatedByUserId { get; set; }
        public int? EditedByUserId { get; set; }
        public decimal? TotalHours { get; set; }
        public bool IsFixedLabHourExtension { get; set; }
    }
}