using CMS.Domain.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Batch : AuditableEntity
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; }
        public int BatchId { get; set; }

        public string Name { get; set; }

        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Classes { get; set; }

        public DateTime InTime { get; set; }

        public DateTime OutTime { get; set; }

        public virtual ICollection<Student> Students { get; set; }

        public Batch()
        {
            Students = new HashSet<Student>();
        }
    }
}
