using System;

namespace CMS.Common.GridModels
{
    public class BranchAdminGridModel
    {
        public int AId { get; set; }

        public int BranchId { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string ContactNo { get; set; }

        public string BranchName { get; set; }

        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
