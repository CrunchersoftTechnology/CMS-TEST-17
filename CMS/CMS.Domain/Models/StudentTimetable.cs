using CMS.Common.Enums;
using CMS.Domain.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
   public class StudentTimetable : AuditableEntity
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }
        public int StudentTimetableId { get; set; }

        public string Description { get; set; }

        public string SelectedBranches { get; set; }

        public string SelectedClasses { get; set; }

        public string SelectedBatches { get; set; }

        public string FileName { get; set; }

        public TimetableCategory Category { get; set; }

        public DateTime StudentTimetableDate { get; set; }

        public string AttachmentDescription { get; set; }

    }
}
