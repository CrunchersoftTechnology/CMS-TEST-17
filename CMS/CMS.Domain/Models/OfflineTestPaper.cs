using CMS.Domain.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
   public class OfflineTestPaper : AuditableEntity
    {
        public int OfflineTestPaperId { get; set; }

        public string Title { get; set; }

        public string SelectedBranches { get; set; }

        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public int SubjectId { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }

        public string SelectedBatches { get; set; }

        public int TotalMarks { get; set; }

        public DateTime TestDate { get; set; }

        public DateTime TestInTime { get; set; }

        public DateTime TestOutTime { get; set; }

        public string Media { get; set; }
    }
}
