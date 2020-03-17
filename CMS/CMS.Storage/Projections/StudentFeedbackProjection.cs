using System;

namespace CMS.Domain.Storage.Projections
{
    public class StudentFeedbackProjection
    {
        public int StudentFeedbackId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Contact { get; set; }

        public string Message { get; set; }

        public string UserId { get; set; }

        public string Status { get; set; }

        public string TeacherName { get; set; }

        public int Rating { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
