using System;

namespace CMS.Domain.Storage.Projections
{
    public class StudentAttendanceDetails
    {
        public int ClassId { get; set; }
        public int BatchId { get; set; }
        public int PunchId { get; set; }
        public int BranchId { get; set; }
        public int SId { get; set; }
        public DateTime PunchDateTime { get; set; }
        public DateTime BatchInTime { get; set; }
        public DateTime BatchOutTime { get; set; }
        public string SelectedAttendance { get; set; }
        public string Time { get; set; }
    }
}
