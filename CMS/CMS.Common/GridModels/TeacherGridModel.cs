using System;

namespace CMS.Common.GridModels
{
    public class TeacherGridModel
    {
        public int TId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string ContactNo { get; set; }

        public string UserId { get; set; }

        public string Description { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public bool IsActive { get; set; }
        [Exclude]
        public string Action { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Qualification { get; set; }
    }
}
