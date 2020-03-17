using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.GridModels
{
   public class InstallmentGridModel
    {
        public int InstallmentId { get; set; }
        public string UserId { get; set; }
        public string StudentFirstName { get; set; }
        public string StudentMiddleName { get; set; }
        public string StudentLastName { get; set; }
        public string TotalFee { get; set; }
        public string ReceivedFee { get; set; }
        public decimal RemainingFee { get; set; }
        public decimal Payment { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ClassName { get; set; }
        public decimal FinalFee { get; set; }
        public string Email { get; set; }
        public string ReceiptNumber { get; set; }
        public string ReceiptBookNumber { get; set; }
        public string BranchName { get; set; }
        public int InstallmentNo { get; set; }
        public string StudBatch { get; set; }
        public int ClassId { get; set; }
        public int BranchId { get; set; }
        [Exclude]
        public string Action { get; set; }
    }
}
