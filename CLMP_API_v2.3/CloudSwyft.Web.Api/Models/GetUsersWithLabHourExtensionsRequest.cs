using System;

namespace CloudSwyft.Web.Api.Models
{
    public class GetUsersWithLabHourExtensionsRequest
    {
        public string SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortDirection { get; set; }
        public string SortField { get; set; }
        public int VEProfileId { get; set; }
        public bool ShowAllRecords { get; set; }
    }
}