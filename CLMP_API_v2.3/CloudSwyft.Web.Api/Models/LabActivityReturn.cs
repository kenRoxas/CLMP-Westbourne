using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudSwyft.Web.Api.Models
{
    public class LabActivityReturn
    {
        public int LabActivityID { get; set; }

        public string Name { get; set; }

        public List<string> Tasks { get; set; }

        public int UseCount { get; set; }

        public string TasksHtml { get; set; }

        public string CourseCode { get; set; }


        public string LabAnswerKey { get; set; }
        public string LabAnswerKeyName { get; set; }

    }

    public class LabActivityReturnPart
    {
        
        public int LabActivityID { get; set; }

        public string Name { get; set; }

        public List<string> Tasks { get; set; } // Format: ["Task 1", "Task 2", "Task 3"]

        public int UseCount { get; set; }

        public string TasksHtml { get; set; }

        public string LabAnswerKey { get; set; }
        public string LabAnswerKeyName { get; set; }

        public string CourseCode { get; set; }

        public LabActivityReturnPart(int id, string name, List<string> tasks, string taskshtml, int usestring, string labAnswerKey, string labAnswerKeyName, string courseCode)
        {
            LabActivityID = id;
            Name = name;
            Tasks = tasks;
            TasksHtml = taskshtml;
            UseCount = usestring;
            LabAnswerKey = labAnswerKey;
            LabAnswerKeyName = labAnswerKeyName != null ? labAnswerKeyName.Replace("\"", "") : null;
            CourseCode = courseCode;
        }
    }

    public class LabActivityViewModelPart
    {
        public List<LabActivityReturnPart> LabActivities { get; set; }
    }

    public class LabActivityViewModel
    {
        public List<LabActivityReturn> LabActivities { get; set; }
        public int PageSize { get; set; }
        public int ActivePage { get; set; }
        public int TotalPages { get; set; }
        public string SearchFilter { get; set; }
        public int TotalItems { get; set; }
    }

    public class LabActivityUploadReturn
    {
        public string location { get; set; }
    }
}