namespace CMS.Domain.Storage.Projections
{
    public class TeacherProjection
    {
        public int TId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        public string ContactNo { get; set; }

        public string UserId { get; set; }

        public string Description { get; set; }

        public int BranchId { get; set; }

        public string BranchName { get; set; }


        public int ClientId { get; set; }

        public string ClientName { get; set; }


        public bool IsActive { get; set; }

        public string Name { get; set; }

        public string Qualification { get; set; }
    }
}
