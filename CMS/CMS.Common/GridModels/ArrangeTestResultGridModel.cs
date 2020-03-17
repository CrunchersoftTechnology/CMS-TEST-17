using System;

namespace CMS.Common.GridModels
{
    public class ArrangeTestResultGridModel
    {
        public int ArrangeTestResultId { get; set; }

        public string UserId { get; set; }

        public int TestPaperId { get; set; }

        public DateTime TestDate { get; set; }

        public int TimeDuration { get; set; }

        public DateTime StartTime { get; set; }

        public string Questions { get; set; }

        public string StudentName { get; set; }

        public string TestPaperTitle { get; set; }

        public int ObtainedMarks { get; set; }

        public int OutOfMarks { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ClassId { get; set; }

        public int BranchId { get; set; }
    }
}
