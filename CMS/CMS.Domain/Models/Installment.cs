using CMS.Domain.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Models
{
    public class Installment : AuditableEntity
    {
        public int InstallmentId { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser Student { get; set; }

        public decimal Payment { get; set; }

        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class Class { get; set; }

        public decimal RemainingFee { get; set; }

        [MaxLength(50)]
        public string ReceiptNumber { get; set; }

        [MaxLength(50)]
        public string ReceiptBookNumber { get; set; }

        public decimal ReceivedFee { get; set; }
    }
}
