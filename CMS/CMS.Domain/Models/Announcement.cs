using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace CMS.Domain.Models
{
    public class Announcement : AuditableEntity
    {
        public int AnnouncementId { get; set; }

        [Required]
        [MaxLength(500)]
        [DataType(DataType.MultilineText)]
        public string AnnouncementDetails { get; set; }

        [MaxLength(128)]
        public string Url { get; set; }

        [Required]
        [Display(Name = "Visible")]
        public bool IsVisible { get; set; }
    }
}
