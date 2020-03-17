using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
  public  class OfflineTestStudentMarks : AuditableEntity
    {
        public int OfflineTestStudentMarksId { get; set; }

        public int OfflineTestPaperId { get; set; }

        [ForeignKey("OfflineTestPaperId")]
        public virtual OfflineTestPaper OfflineTestPaper { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Student { get; set; }

        public int ObtainedMarks { get; set; }

        public decimal Percentage { get; set; }

        public bool IsPresent { get; set; }
    }
}
