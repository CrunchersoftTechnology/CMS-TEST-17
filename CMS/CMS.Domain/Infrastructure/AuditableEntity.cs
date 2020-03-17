using System;

namespace CMS.Domain.Infrastructure
{
    public abstract class AuditableEntity
    {
        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string UpdatedBy { get; set; }
    }
}
