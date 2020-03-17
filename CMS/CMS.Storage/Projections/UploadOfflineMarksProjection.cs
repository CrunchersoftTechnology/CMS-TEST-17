using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Projections
{
  public class UploadOfflineMarksProjection
    {
        public int OfflineTestStudentMarksId { get; set; }

        public string UserId { get; set; }

        public DateTime CreatedOn { get; set; }

        public int OfflineTestPaperId { get; set; }

        public int ObtainedMarks { get; set; }

        public string SelectedBranches { get; set; }

        public int ClassId { get; set; }

        public int SubjectId { get; set; }

        public string SelectedBatches { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        public string EmailId { get; set; }

        public decimal Percentage { get; set; }

        public string StudentContact { get; set; }

        public string Title { get; set; }

        public int TotalMarks { get; set; }

        public string StudentName { get; set; }

        public string parentAppPlayerId { get; set; }

        public bool IsPresent { get; set; }

    }
}
