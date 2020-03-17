using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class PDFUpload :AuditableEntity
    {
        public int ClassId { get; set; }

        public int PDFUploadId { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public bool IsVisible { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public int PDFCategoryId { get; set; }

        [ForeignKey("PDFCategoryId")]
        public virtual PDFCategory PDFCategory { get; set; }

        public bool IsSend { get; set; }
    }
}
