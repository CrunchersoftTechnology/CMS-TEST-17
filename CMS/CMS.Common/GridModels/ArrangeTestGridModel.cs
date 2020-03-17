using System;

namespace CMS.Common.GridModels
{
    public class ArrangeTestGridModel
    {
        public int ArrengeTestId { get; set; }

        public int TestPaperId { get; set; }
       
        public string TestPapers { get; set; }

        public int StudentCount { get; set; }

        public string Media { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Title { get; set; }

        public string TestType { get; set; }

        public string SelectedClass { get; set; }

        public string SubjectName { get; set; }
    }
}
