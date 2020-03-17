using System;

namespace CMS.Domain.Storage.Projections
{
    public class PendingStudentAdmissionProjection
    {
        public int AdmissionId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MiddleName { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
