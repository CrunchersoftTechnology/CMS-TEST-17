using CMS.Domain.Infrastructure;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Client : AuditableEntity
    {
        public Client()
        {
            IsChangeDetected = false;
        }
      
        public int ClientId { get; set; }

        private string name;

        [Required]
        [MaxLength(100)]
        public string Name
        {
            get { return name; }
            set
            {
                if (name != null && value != name)
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
            set
            {
                if (address != null && value != address)
                    IsChangeDetected = true;
                address = value;
            }
        }

        [NotMapped]
        public bool IsChangeDetected { get; set; }
    }
}
