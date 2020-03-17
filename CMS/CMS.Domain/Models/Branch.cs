using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Branch : AuditableEntity
    {
        public Branch()
        {
            IsChangeDetected = false;
        }
        public int BranchId { get; set; }


        //[Key, ForeignKey("User")]
        public string UserId { get; set; }

       /* [Required]
        public ApplicationUser User { get; set; }*/

        private string name;

        [Required]
        [MaxLength(100)]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != null &&  value != name )
                    IsChangeDetected = true;
                name = value;
            }
        }
        private string address;

        [Required]
        [MaxLength(150)]
        public string Address
        {
            get { return address; }
            set {
                if (address != null && value != address)
                    IsChangeDetected = true;
                address = value;
            }
        }

        [NotMapped]
        public bool IsChangeDetected { get; set; }

        public int ClientId { get; set; }


        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public virtual Client ClientName { get; set; }
    }
}
