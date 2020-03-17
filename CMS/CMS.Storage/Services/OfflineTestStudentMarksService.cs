using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class OfflineTestStudentMarksService : IOfflineTestStudentMarksService
    {
        readonly IRepository _repository;

        public OfflineTestStudentMarksService(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<StudentProjection> GetStudentsAutoComplete(string query, int classId, string SelectedBranches)
        {
            var branchIds = SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            return _repository.Project<Student, StudentProjection[]>(
                students => (from stud in students
                             where (stud.FirstName.Contains(query) ||
                             stud.MiddleName.Contains(query) ||
                             stud.LastName.Contains(query)) && stud.ClassId == classId && branchIds.Contains(stud.BranchId)
                             select new StudentProjection
                             {
                                 UserId = stud.UserId,
                                 Name = stud.FirstName + " " + stud.MiddleName + " " + stud.LastName,
                                 DOJ = stud.DOJ,
                                 Email = stud.User.Email,
                                 DOB = stud.DOB,
                                 //Batches = stud.Batches,
                                 StudentContact = stud.StudentContact,
                                 parentAppPlayerId = stud.parentAppPlayerId,
                                 //StudentBatches = stud.Batches.Select(x => /*x.Subject.Name +*/  " ( " + x.Name + " )")
                             }).ToArray());
        }

        public CMSResult Save(OfflineTestStudentMarks newOfflineTestStudentMarks)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<OfflineTestStudentMarks, bool>(OfflineTestStudentMarkss => (from offline in OfflineTestStudentMarkss where offline.OfflineTestPaperId == newOfflineTestStudentMarks.OfflineTestPaperId && offline.UserId == newOfflineTestStudentMarks.UserId select offline).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Student Marks already exists!") });
            }
            else
            {
                _repository.Add(newOfflineTestStudentMarks);
                _repository.CommitChanges();
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Offline Test Student Marks successfully added!") });
            }
            return result;
        }

        public IEnumerable<OfflineTestStudentMarksGridModel> GetOfflineNotificationData(out int totalRecords, int userId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            IQueryable<OfflineTestStudentMarksGridModel> finalList = null;
            List<OfflineTestStudentMarksGridModel> list = new List<OfflineTestStudentMarksGridModel>();
            var query = _repository.Project<OfflineTestStudentMarks, IQueryable<OfflineTestStudentMarksGridModel>>(offlineTestPaperMarks => (
                 from offlineTest in offlineTestPaperMarks
                 orderby offlineTest.CreatedOn descending
                 group offlineTest by new
                 {
                     offlineTest.OfflineTestPaperId,
                     offlineTest.OfflineTestPaper.Title,
                     offlineTest.OfflineTestPaper.TotalMarks,
                     offlineTest.OfflineTestPaper.Class.Name,
                     offlineTest.OfflineTestPaper.TestDate,
                     subjectName = offlineTest.OfflineTestPaper.Subject.Name,
                     selectedBranches = offlineTest.OfflineTestPaper.SelectedBranches
                 } into offlineTestPaper
                 select new OfflineTestStudentMarksGridModel
                 {
                     OfflineTestPaperId = offlineTestPaper.Key.OfflineTestPaperId,
                     Title = offlineTestPaper.Key.Title,
                     TotalMarks = offlineTestPaper.Key.TotalMarks,
                     Class = offlineTestPaper.Key.Name,
                     Date = offlineTestPaper.Key.TestDate,
                     CreatedOn = offlineTestPaper.Select(x => x.CreatedOn).FirstOrDefault(),
                     Subject = offlineTestPaper.Key.subjectName,
                     SelectedBranches = offlineTestPaper.Key.selectedBranches
                 }));
            if (BranchId != 0)
            {
                foreach (var offlineTestPaperMarks in query)
                {
                    var selectedBranchList = offlineTestPaperMarks.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    if (selectedBranchList.Contains(BranchId))
                    {
                        list.Add(offlineTestPaperMarks);
                    }
                }
                finalList = list.AsQueryable();
            }
            else
            {
                finalList = query;
            }

            totalRecords = finalList.Count();
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(OfflineTestStudentMarksGridModel.Class):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Class);
                        else
                            finalList = finalList.OrderByDescending(p => p.Class);
                        break;
                    case nameof(OfflineTestStudentMarksGridModel.Subject):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Subject);
                        else
                            finalList = finalList.OrderByDescending(p => p.Subject);
                        break;
                    case nameof(OfflineTestStudentMarksGridModel.Title):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Title);
                        else
                            finalList = finalList.OrderByDescending(p => p.Title);
                        break;
                    case nameof(OfflineTestStudentMarksGridModel.TotalMarks):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.TotalMarks);
                        else
                            finalList = finalList.OrderByDescending(p => p.TotalMarks);
                        break;
                    case nameof(OfflineTestStudentMarksGridModel.Date):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Date);
                        else
                            finalList = finalList.OrderByDescending(p => p.Date);
                        break;
                    default:
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.CreatedOn);
                        else
                            finalList = finalList.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }
            if (limitOffset.HasValue)
            {
                finalList = finalList.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }
            return finalList.ToList();
        }

        public OfflineTestStudentMarksProjection GetOfflineTestMarksById(int id)
        {
            return _repository.Project<OfflineTestStudentMarks, OfflineTestStudentMarksProjection>(
                OfflineStudentMarks => (from n in OfflineStudentMarks
                                        where n.OfflineTestStudentMarksId == id
                                        select new OfflineTestStudentMarksProjection
                                        {
                                            Title = n.OfflineTestPaper.Title,
                                            CreatedOn = n.CreatedOn,
                                            SelectedBranches = n.OfflineTestPaper.SelectedBranches,
                                            SelectedBatches = n.OfflineTestPaper.SelectedBatches,
                                            ClassName = n.OfflineTestPaper.Class.Name,
                                            SubjectName = n.OfflineTestPaper.Subject.Name,
                                            OfflineTestPaperId = n.OfflineTestPaperId,
                                            TotalMarks = n.OfflineTestPaper.TotalMarks,
                                            MarksObtained = n.ObtainedMarks,
                                            FirstName = n.Student.Student.FirstName,
                                            MiddleName = n.Student.Student.MiddleName,
                                            LastName = n.Student.Student.LastName,
                                            OfflineTestStudentMarksId = n.OfflineTestStudentMarksId,
                                            UserId = n.UserId,
                                            StudentContact = n.Student.Student.StudentContact,
                                            StudentEmail = n.Student.Student.User.Email
                                        }).FirstOrDefault());
        }

        public CMSResult Delete(int offlineTestStudentMarksId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<OfflineTestStudentMarks>(m => m.OfflineTestStudentMarksId == offlineTestStudentMarksId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Offline Test Paper Marks already exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Offline Test Paper Marks deleted successfully!") });
            }
            return result;
        }

        public CMSResult Update(OfflineTestStudentMarks OfflineTestPaper)
        {
            CMSResult result = new CMSResult();
            var marks = _repository.Load<OfflineTestStudentMarks>(x => x.OfflineTestStudentMarksId == OfflineTestPaper.OfflineTestStudentMarksId);
            marks.ObtainedMarks = OfflineTestPaper.ObtainedMarks;
            marks.Percentage = OfflineTestPaper.Percentage;
            marks.IsPresent = OfflineTestPaper.IsPresent;
            _repository.Update(marks);
            _repository.CommitChanges();
            result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Offline Test Marks successfully updated!") });
            return result;
        }

        public IEnumerable<OfflineTestPaperProjection> GetOfflineTest()
        {
            return _repository.Project<OfflineTestPaper, OfflineTestPaperProjection[]>(
                papers => (from off in papers
                           select new OfflineTestPaperProjection
                           {
                               OfflineTestPaperId = off.OfflineTestPaperId,
                               Title = off.Title,
                               SelectedBranches = off.SelectedBranches,
                               ClassId = off.ClassId,
                               SubjectId = off.SubjectId,
                               SelectedBatches = off.SelectedBatches
                           }).ToArray());
        }

        public IEnumerable<UploadOfflineMarksProjection> GetOfflineTestByOfflineTestPaperId(int offlineTestPaperId)
        {
            return _repository.Project<OfflineTestStudentMarks, UploadOfflineMarksProjection[]>(
                offlinePaperMarks => (from offlinePaperMark in offlinePaperMarks
                                      where offlinePaperMark.OfflineTestPaperId == offlineTestPaperId
                                      select new UploadOfflineMarksProjection
                                      {
                                          OfflineTestStudentMarksId = offlinePaperMark.OfflineTestStudentMarksId,
                                          OfflineTestPaperId = offlinePaperMark.OfflineTestPaperId,
                                          UserId = offlinePaperMark.UserId,
                                          ObtainedMarks = offlinePaperMark.ObtainedMarks,
                                          Percentage = offlinePaperMark.Percentage,
                                          Title = offlinePaperMark.OfflineTestPaper.Title,
                                          TotalMarks = offlinePaperMark.OfflineTestPaper.TotalMarks,
                                          StudentName = offlinePaperMark.Student.Student.FirstName + " " + offlinePaperMark.Student.Student.MiddleName + " " + offlinePaperMark.Student.Student.LastName,
                                          StudentContact = offlinePaperMark.Student.Student.StudentContact,
                                          EmailId = offlinePaperMark.Student.Student.User.Email,
                                          parentAppPlayerId = offlinePaperMark.Student.Student.parentAppPlayerId,
                                          IsPresent = offlinePaperMark.IsPresent
                                      }).ToArray());
        }

    }
}
