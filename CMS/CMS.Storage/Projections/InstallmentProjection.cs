using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Projections
{
    public class InstallmentProjection
    {
        public int InstallmentId { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public decimal Payment { get; set; }

        public DateTime CreatedOn { get; set; }

        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public decimal RemainingFee { get; set; }

        public decimal TotalFee { get; set; }

        public decimal FinalFee { get; set; }

        public string ReceiptNumber { get; set; }
        
        public string ReceiptBookNumber { get; set; }

        public int BranchId { get; set; }

        public int ClientId { get; set; }

        public string BranchName { get; set; }

        public string ClientName { get; set; }

        public string StudentContact { get; set; }

        public string ParentContact { get; set; }

        public string ParentAppPlayerId { get; set; }

        public string Email { get; set; }

        public string StudBatch { get; set; }

        public IEnumerable<string> StudentSubjects { get; set; }
    }
}
