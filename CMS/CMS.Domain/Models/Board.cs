using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Board : AuditableEntity
    {
        public string UserId { get; set; }

        public int BoardId { get; set; }

        public string Name { get; set; }

        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual Client ClientName { get; set; }
    }
}
