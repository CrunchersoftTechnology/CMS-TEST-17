using CMS.Domain.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Teacher : AuditableEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TId { get; set; }

        [Key, ForeignKey("User")]
        public string UserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string ContactNo { get; set; }

        public string Description { get; set; }

        public int BranchId { get; set; }

       // [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        public virtual ICollection<Branch> Branches { get; set; }


        public int ClientId { get; set; }

        //[ForeignKey("ClientId")]
        public virtual Client Client { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

        public bool IsActive { get; set; }

        public string Qualification { get; set; }
    }
}
