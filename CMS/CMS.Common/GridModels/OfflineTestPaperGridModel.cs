using System;

namespace CMS.Common.GridModels
{
    public class OfflineTestPaperGridModel
    {
        public int OfflineTestPaperId { get; set; }

        public string Title { get; set; }

        public int ClassId { get; set; }

        public int SubjectId { get; set; }

        public int TotalMarks { get; set; }

        public DateTime TestDate { get; set; }

        public DateTime TestInTime { get; set; }

        public DateTime TestOutTime { get; set; }

        public string Media { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Class { get; set; }

        public string Subject { get; set; }

        public string SelectedBranches { get; set; }

    }
}
