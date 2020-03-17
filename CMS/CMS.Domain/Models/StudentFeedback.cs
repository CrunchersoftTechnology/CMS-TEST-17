using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class StudentFeedback : AuditableEntity
    {
        public int StudentFeedbackId { get; set; }

        public string Contact { get; set; }

        public string Email { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Teacher Teacher { get; set; }

        public string Status { get; set; }

        public string Name { get; set; }

        public int Rating { get; set; }
    }
}
