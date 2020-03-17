using CMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Projections
{
  public class ArrangeTestProjection
    {
        public int ArrengeTestId { get; set; }

        public int TestPaperId { get; set; }

        public string SelectedClass { get; set; }

        public string SelectedBranches { get; set; }

        public string SelectedBatches { get; set; }

        public virtual TestPaper TestPapers { get; set; }

        public int StudentCount { get; set; }

        public string Media { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Title { get; set; }

        public string TestType { get; set; }

        public string SubjectName { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartTime { get; set; }

        public int TimeDuration { get; set; }
    }
}
