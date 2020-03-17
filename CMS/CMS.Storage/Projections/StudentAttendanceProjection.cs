using System;

namespace CMS.Domain.Storage.Projections
{
    public class StudentAttendanceProjection
    {
        public int BatchId { get; set; }

        public int ClassId { get; set; }

        public int BranchId { get; set; }

        public int SId { get; set; }

        public int PunchId { get; set; }

        public bool IsActive { get; set; }

        public string ClassName { get; set; }

        public string BranchName { get; set; }

        public string BatchName { get; set; }

        public DateTime InTime { get; set; }

        public DateTime OutTime { get; set; }

        public string parentAppPlayerId { get; set; }

        public string studentAppPlayerId { get; set; }

        public string StudentName { get; set; }
    }
}
