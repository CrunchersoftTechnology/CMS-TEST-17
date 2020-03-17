using CMS.Common.Enums;
using System;

namespace CMS.Common.GridModels
{
    public class StudentTimetableGridModel
    {
        public int StudentTimetableId { get; set; }

        public string Description { get; set; }

        public TimetableCategory Category { get; set; }

        public DateTime StudentTimetableDate { get; set; }
        
        public string Action { get; set; }

        public string FileName { get; set; }

        public string SelectedBranches { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
