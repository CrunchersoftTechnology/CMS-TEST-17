using System;

namespace CMS.Domain.Storage.Projections
{
    public class AttendanceProjection
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


        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public string SubjectName { get; set; }

        public string InTime { get; set; }

        public string OutTime { get; set; }
    }
}
