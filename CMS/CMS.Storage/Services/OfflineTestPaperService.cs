using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class OfflineTestPaperService : IOfflineTestPaper
    {
        readonly IRepository _repository;

        public OfflineTestPaperService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(OfflineTestPaper offlineTestPaper)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<OfflineTestPaper, bool>(OfflineTestPapers => (
                                            from offlineTest in OfflineTestPapers
                                            where offlineTest.Title == offlineTestPaper.Title
                                            select offlineTest)
                                            .Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Exam schedule '{0}' already exists!", offlineTestPaper.Title) });
            }
            else
            {
                _repository.Add(offlineTestPaper);
                _repository.CommitChanges();
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Exam schedule Notification added successfully!") });
            }
                return result;
        }

        public IEnumerable<OfflineTestPaperGridModel> GetOfflineNotificationData(out int totalRecords, int userId,
          int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            IQueryable<OfflineTestPaperGridModel> finalList = null;
            List<OfflineTestPaperGridModel> list = new List<OfflineTestPaperGridModel>();
            var query = _repository.Project<OfflineTestPaper, IQueryable<OfflineTestPaperGridModel>>(offlineTestPaper => (
                 from n in offlineTestPaper
                 select new OfflineTestPaperGridModel
                 {
                     OfflineTestPaperId = n.OfflineTestPaperId,
                     Title = n.Title,
                     TestInTime = n.TestInTime,
                     TestOutTime = n.TestOutTime,
                     TotalMarks = n.TotalMarks,
                     Media = n.Media,
                     CreatedOn = n.CreatedOn,
                     Class = n.Class.Name,
                     Subject = n.Subject.Name,
                     TestDate = n.TestDate,
                     SelectedBranches = n.SelectedBranches
                 })).AsQueryable();

            if (BranchId != 0)
            {
                foreach (var offlineTestPaper in query)
                {
                    var selectedBranchList = offlineTestPaper.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    if (selectedBranchList.Contains(BranchId))
                    {
                        list.Add(offlineTestPaper);
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
                    case nameof(OfflineTestPaperGridModel.Title):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Title);
                        else
                            finalList = finalList.OrderByDescending(p => p.Title);
                        break;
                    case nameof(OfflineTestPaperGridModel.Class):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Class);
                        else
                            finalList = finalList.OrderByDescending(p => p.Class);
                        break;
                    case nameof(OfflineTestPaperGridModel.Subject):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Subject);
                        else
                            finalList = finalList.OrderByDescending(p => p.Subject);
                        break;
                    case nameof(OfflineTestPaperGridModel.TotalMarks):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.TotalMarks);
                        else
                            finalList = finalList.OrderByDescending(p => p.TotalMarks);
                        break;
                    case nameof(OfflineTestPaperGridModel.TestOutTime):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.TestOutTime);
                        else
                            finalList = finalList.OrderByDescending(p => p.TestOutTime);
                        break;
                    case nameof(OfflineTestPaperGridModel.TestInTime):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.TestInTime);
                        else
                            finalList = finalList.OrderByDescending(p => p.TestInTime);
                        break;
                    case nameof(OfflineTestPaperGridModel.Media):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Media);
                        else
                            finalList = finalList.OrderByDescending(p => p.Media);
                        break;
                    case nameof(OfflineTestPaperGridModel.TestDate):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.TestDate);
                        else
                            finalList = finalList.OrderByDescending(p => p.TestDate);
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

        public CMSResult Delete(int OfflineTestPaperId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<OfflineTestPaper>(m => m.OfflineTestPaperId == OfflineTestPaperId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Offline Test Paper '{0}' already exists!", model.Title) });
            }
            else
            {
                var isExistsOfflineTestStudentMarks = _repository.Project<OfflineTestStudentMarks, bool>(offlineMarks => (
                                       from offlineMark in offlineMarks
                                       where offlineMark.OfflineTestPaperId == OfflineTestPaperId
                                       select offlineMark)
                                       .Any());
                if (isExistsOfflineTestStudentMarks)
                {
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Exam Schedule '{0}'. Because it belongs to {1}!", model.Title, "UploadMarks") });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Exam schedule '{0}' deleted successfully!", model.Title) });
                }
            }
            return result;
        }

        public OfflineTestPaperProjection GetOfflineTestById(int id)
        {
            return _repository.Project<OfflineTestPaper, OfflineTestPaperProjection>(
                OfflineTestPaper => (from n in OfflineTestPaper
                                     where n.OfflineTestPaperId == id
                                     select new OfflineTestPaperProjection
                                     {
                                         Title = n.Title,
                                         CreatedOn = n.CreatedOn,
                                         SelectedBranches = n.SelectedBranches,
                                         SelectedBatches = n.SelectedBatches,
                                         ClassName = n.Class.Name,
                                         SubjectName = n.Subject.Name,
                                         OfflineTestPaperId = n.OfflineTestPaperId,
                                         TestDate = n.TestDate,
                                         TestInTime = n.TestInTime,
                                         TestOutTime = n.TestOutTime,
                                         Media = n.Media,
                                         TotalMarks = n.TotalMarks,
                                         ClassId = n.ClassId,
                                         SubjectId = n.SubjectId,
                                     }).FirstOrDefault());
        }

        public CMSResult Update(OfflineTestPaper OfflineTestPaper)
        {
            CMSResult result = new CMSResult();

            var isExists = _repository.Project<OfflineTestPaper, bool>(ofline => (from b in ofline
                                                                                  where b.OfflineTestPaperId != OfflineTestPaper.OfflineTestPaperId && b.Title == OfflineTestPaper.Title
                                                                                  select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Offline Test '{0}' already exists!", OfflineTestPaper.Title) });
            }
            else
            {
                var offlineTests = _repository.Load<OfflineTestPaper>(x => x.OfflineTestPaperId == OfflineTestPaper.OfflineTestPaperId);

                offlineTests.Title = OfflineTestPaper.Title;
                offlineTests.TestDate = OfflineTestPaper.TestDate;
                offlineTests.TestInTime = OfflineTestPaper.TestInTime;
                offlineTests.TestOutTime = OfflineTestPaper.TestOutTime;
                offlineTests.TotalMarks = OfflineTestPaper.TotalMarks;
                _repository.Update(offlineTests);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Exam schedule '{0}' updated successfully!", offlineTests.Title) });
            }
            return result;
        }

        public IEnumerable<OfflineTestPaperProjection> GetOfflineTestPaper()
        {
            return _repository.Project<OfflineTestPaper, OfflineTestPaperProjection[]>(
                OfflineTestPapers => (from oflnTestPaper in OfflineTestPapers
                                      select new OfflineTestPaperProjection
                                      {
                                          OfflineTestPaperId = oflnTestPaper.OfflineTestPaperId,
                                          Title = oflnTestPaper.Title,
                                          SelectedBranches = oflnTestPaper.SelectedBranches
                                      }).ToArray());
        }
    }
}
