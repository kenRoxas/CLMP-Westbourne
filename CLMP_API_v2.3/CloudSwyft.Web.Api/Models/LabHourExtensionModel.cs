using System;
using System.ComponentModel.DataAnnotations;

namespace CloudSwyft.Web.Api.Models
{
    public class LabHourExtension   
    {
        public DateTime DbTimeStamp { get; set; }
        [Key]
        public int Id { get; set; }
        public int CloudLabsScheduleId { get; set; }
        public int UserId { get; set; }
        public int ExtensionTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CreatedByUserId { get; set; }
        public int EditedByUserId { get; set; }
        public int VEProfileId { get; set; }
        public double TimeRemaining { get; set; }
        public bool IsDeleted { get; set; }
        public decimal? TotalHours { get; set; }
        public bool? IsFixedLabHourExtension { get; set; }
    }

    public class LabHourExtensionType
    {

        public DateTime DbTimeStamp { get; set; }
        [Key]
        public int Id { get; set; }
        public string Label { get; set; }
        public string Description{ get; set; }     
    }    
    public class BulkType
    {

        public int[] UserId { get; set; }
        
        public int VEProfileId { get; set; }
     }

    public class LabHourExtensionData
    {
        public string[] UserId { get; set; }   
        public string[] LabHourExtensionId { get; set; }   
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int VEProfileId { get; set; }
        public int ExtensionTypeId { get; set; }
    }
}