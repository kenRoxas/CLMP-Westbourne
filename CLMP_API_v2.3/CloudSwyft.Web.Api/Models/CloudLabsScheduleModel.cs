using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CloudSwyft.Web.Api.Models
{
    public class CloudLabsSchedule
    {
        [Key]
        public int CloudLabsScheduleId { get; set; }  
        
        public int VEProfileID { get; set; }
        public int UserId { get; set; }
        //public string ScheduledBy { get; set; }  
        //public DateTime DateCreated { get; set; }
        
        public double? TimeRemaining { get; set; }
        public double? LabHoursTotal { get; set; }
        public DateTime ? StartLabTriggerTime { get; set; }
        public DateTime ? RenderPageTriggerTime { get; set; }

        public double  InstructorLabHours { get; set; }
        public DateTime ? InstructorLastAccess { get; set; }

//[ForeignKey("MachineLabs")]
        public int MachineLabsId { get; set; }
        public MachineLabs MachineLabs { get; set; }

    }

    public class ModelList
    {
        public List<CloudLabsSchedule> SelectedUsers { get; set; }
    }

}