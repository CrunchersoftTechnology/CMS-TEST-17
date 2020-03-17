using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Class : AuditableEntity
    {
        public string UserId { get; set; }

        public int ClassId { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Subject> Subjects { get; set; }

        public virtual ICollection<PDFUpload> PDFUploads { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual Client ClientName { get; set; }
    }
}
