using CMS.Domain.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class ArrangeTestResult : AuditableEntity
    {
        public int ArrangeTestResultId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Student { get; set; }

        public int TestPaperId { get; set; }

        [ForeignKey("TestPaperId")]
        public virtual TestPaper TestPapers { get; set; }

        public DateTime TestDate { get; set; }

        public int TimeDuration { get; set; }

        public DateTime StartTime { get; set; }

        public string Questions { get; set; }

        public int ObtainedMarks { get; set; }

        public int OutOfMarks { get; set; }
    }
}
