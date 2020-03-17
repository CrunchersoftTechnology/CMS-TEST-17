using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class StudentFeedbackService : IStudentFeedbackService
    {
        readonly IRepository _repository;

        public StudentFeedbackService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(StudentFeedback newStudentFeedback)
        {
            var result = new CMSResult();
            _repository.Add(newStudentFeedback);
            result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Student Feedback '{0}' added successfully!", newStudentFeedback.Name) });
            return result;
        }

        public IEnumerable<StudentFeedbackProjection> GetStudentFeedback()
        {
            return _repository.Project<StudentFeedback, StudentFeedbackProjection[]>(
                studentFeedback => (from feedback in studentFeedback
                                    select new StudentFeedbackProjection
                                    {
                                        Contact = feedback.Contact,
                                        Name = feedback.Name,
                                        Email = feedback.Email,
                                        Message = feedback.Message,
                                        UserId = feedback.UserId.ToString(),
                                        Status = feedback.Status,
                                        StudentFeedbackId = feedback.StudentFeedbackId,
                                        TeacherName = feedback.Teacher.FirstName + " " + feedback.Teacher.MiddleName + " " + feedback.Teacher.LastName,
                                        Rating = feedback.Rating,
                                        CreatedOn = feedback.CreatedOn
                                    }).ToArray());
        }

        public CMSResult UpdateMultipleFeedback(string selectedFeedback, string status)
        {
            var commaseperatedList = selectedFeedback ?? string.Empty;
            var FeedbackIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            CMSResult cmsresult = new CMSResult();
            var result = new Result();

            var feedback = _repository.LoadList<StudentFeedback>(x => FeedbackIds.Contains(x.StudentFeedbackId)).ToList();
            if (feedback == null)
            {
                result.IsSuccessful = false;
                result.Message = "Feedback not exists!";
            }
            else
            {
                feedback.ForEach(x => x.Status = status);
                _repository.CommitChanges();
                result.IsSuccessful = true;
                result.Message = string.Format("Student  Feedback updated successfully!");
            }

            cmsresult.Results.Add(result);
            return cmsresult;
        }
    }
}
