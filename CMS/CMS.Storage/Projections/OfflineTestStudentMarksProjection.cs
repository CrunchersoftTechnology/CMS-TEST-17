using System;

namespace CMS.Domain.Storage.Projections
{
   public class OfflineTestStudentMarksProjection
    {
        public int OfflineTestStudentMarksId { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public DateTime CreatedOn { get; set; }

        public int OfflineTestPaperId { get; set; }

        public string Title { get; set; }

        public int MarksObtained { get; set; }

        public string ClassName { get; set; }

        public string SubjectName { get; set; }

        public string SelectedBatches { get; set; }

        public string SelectedBranches { get; set; }

        public int TotalMarks { get; set; }

        public string StudentEmail { get; set; }

        public string StudentContact { get; set; }

    }
}
