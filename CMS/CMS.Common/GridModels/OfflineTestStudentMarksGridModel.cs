using System;

namespace CMS.Common.GridModels
{
 public  class OfflineTestStudentMarksGridModel
    {
        public int OfflineTestStudentMarksId { get; set; }

        public string Title { get; set; }

        public int ClassId { get; set; }

        public int SubjectId { get; set; }

        public string SelectedBatches { get; set; }

        public int TotalMarks { get; set; }

        public DateTime Date { get; set; }

        public string Class { get; set; }

        public string Subject { get; set; }
        [Exclude]
        public string Action { get; set; }

        public int BranchId { get; set; }

        public int ClientId { get; set; }

        public int OfflineTestPaperId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string SelectedBranches { get; set; }
    }
}
