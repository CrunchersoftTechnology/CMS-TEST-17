using CMS.Domain.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class DailyPracticePaper : AuditableEntity
    {
        public int DailyPracticePaperId { get; set; }

        public string Description { get; set; }

        public string SelectedBranches { get; set; }

        public string SelectedClasses { get; set; }

        public string SelectedBatches { get; set; }

        public string FileName { get; set; }

        public DateTime  DailyPracticePaperDate { get; set; }

        public string AttachmentDescription { get; set; }
    }
}
