using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class MasterFee : AuditableEntity
    {

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public int MasterFeeId { get; set; }

        public string Year { get; set; }

        public int SubjectId { get; set; }

        public int ClassId { get; set; }

        public decimal Fee { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }     
    }
}
