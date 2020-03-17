using CMS.Domain.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class ArrengeTest : AuditableEntity
    {
        public int ArrengeTestId { get; set; }

        public int TestPaperId { get; set; }

        [ForeignKey("TestPaperId")]
        public virtual TestPaper TestPapers { get; set; }

        public string SelectedBatches { get; set; }

        public int StudentCount { get; set; }

        public string Media { get; set; }

        public string SelectedBranches { get; set; }

        public DateTime Date { get; set; }

        public DateTime StartTime { get; set; }

        public int TimeDuration { get; set; }
    }
}
