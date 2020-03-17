using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
   public interface IStudentFeedbackService
    {
        CMSResult Save(StudentFeedback newStudentFeedback);
        IEnumerable<StudentFeedbackProjection> GetStudentFeedback();
        CMSResult UpdateMultipleFeedback(string selectedFeedback, string status);
    }
}
