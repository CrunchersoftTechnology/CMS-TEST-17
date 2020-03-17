using System;

namespace CMS.Common.GridModels
{
    public class AttendanceGridModel
    {
        public int AttendanceId { get; set; }

        public int ClassId { get; set; }

        public int BatchId { get; set; }

        public DateTime Date { get; set; }

        public string Activity { get; set; }

        public string StudentAttendence { get; set; }

        public string UserId { get; set; }

        public string ClassName { get; set; }

        public string BatchName { get; set; }

        public string TeacherName { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public string SubjectName { get; set; }

        //public int SubjectId { get; set; }

        public DateTime Intime { get; set; }

        public DateTime Outime { get; set; }

        public bool IsManual { get; set; }

        public bool IsSend { get; set; }

        [Exclude] 
        public string Action { get; set; }

        [Exclude]
        public string Select { get; set; }

        public DateTime CreatedOn { get; set; }

        public string status { get; set; }
    }
}
