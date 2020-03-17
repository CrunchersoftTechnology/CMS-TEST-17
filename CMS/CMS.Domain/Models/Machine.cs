using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Machine: AuditableEntity
    {
        public int MachineId { get; set; }

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public int BranchId { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }

        public bool Status { get; set; }
    }
}
