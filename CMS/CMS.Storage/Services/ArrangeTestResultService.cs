using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using System.Linq;
using System.Collections.Generic;
using CMS.Domain.Storage.Projections;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class ArrangeTestResultService : IArrangeTestResultService
    {
        readonly IRepository _repository;

        public ArrangeTestResultService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(ArrangeTestResult newArrangeTestResult)
        {
            CMSResult cmsresult = new CMSResult();
            var isExists = _repository.Project<ArrangeTestResult, bool>(
                    testResults => (from testresult in testResults
                                    where testresult.TestPaperId == newArrangeTestResult.TestPaperId
                                    && testresult.UserId == newArrangeTestResult.UserId
                                    && testresult.TestDate == newArrangeTestResult.TestDate
                                    select testresult).Any());

            if (isExists)
            {
                cmsresult.Results.Add(new Result("Title already exists!", false));
            }
            else
            {
                _repository.Add(newArrangeTestResult);
                cmsresult.Results.Add(new Result("Test paper result added successfully!", true));
            }
            return cmsresult;
        }

        public IEnumerable<ArrangeTestResultProjection> GetAllArrangeTestResult()
        {
            return _repository.Project<ArrangeTestResult, ArrangeTestResultProjection[]>(
                arrangeTestResults => (from arrangeTestResult in arrangeTestResults
                                       select new ArrangeTestResultProjection
                                       {
                                           ArrangeTestResultId = arrangeTestResult.ArrangeTestResultId,
                                           Questions = arrangeTestResult.Questions,
                                           StartTime = arrangeTestResult.StartTime,
                                           TestDate = arrangeTestResult.TestDate,
                                           TestPaperId = arrangeTestResult.TestPaperId,
                                           TimeDuration = arrangeTestResult.TimeDuration,
                                           StudentName = arrangeTestResult.Student.Student.FirstName + " " + arrangeTestResult.Student.Student.LastName
                                       }).ToArray());
        }

        public IEnumerable<ArrangeTestResultGridModel> GetArrangeTestPaperResultData(out int totalRecords, int userId, int filterClassId, int filterTestPaperId,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int branchId = userId;
            var query = _repository.Project<ArrangeTestResult, IQueryable<ArrangeTestResultGridModel>>(arrangeTestResults => (
                 from arrangeTestResult in arrangeTestResults
                 select new ArrangeTestResultGridModel
                 {
                     TestPaperId = arrangeTestResult.TestPaperId,
                     TestPaperTitle = arrangeTestResult.TestPapers.Title,
                     ArrangeTestResultId = arrangeTestResult.ArrangeTestResultId,
                     StudentName = arrangeTestResult.Student.Student.FirstName + " " + arrangeTestResult.Student.Student.LastName,
                     ObtainedMarks = arrangeTestResult.ObtainedMarks,
                     OutOfMarks = arrangeTestResult.OutOfMarks,
                     TestDate = arrangeTestResult.TestDate,
                     CreatedOn = arrangeTestResult.CreatedOn,
                     ClassId = arrangeTestResult.TestPapers.ClassId,
                     BranchId = arrangeTestResult.Student.Student.BranchId
                 })).AsQueryable();

            if (branchId != 0)
            {
                query = query.Where(x => x.BranchId == branchId);
            }

            totalRecords = query.Count();

            if (filterClassId != 0)
            {
                query = query.Where(p => p.ClassId == filterClassId);
            }

            if (filterTestPaperId != 0)
            {
                query = query.Where(p => p.TestPaperId == filterTestPaperId);
            }

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ArrangeTestResultGridModel.TestPaperTitle):
                        if (!desc)
                            query = query.OrderBy(p => p.TestPaperTitle);
                        else
                            query = query.OrderByDescending(p => p.TestPaperTitle);
                        break;
                    case nameof(ArrangeTestResultGridModel.StudentName):
                        if (!desc)
                            query = query.OrderBy(p => p.StudentName);
                        else
                            query = query.OrderByDescending(p => p.StudentName);
                        break;
                    case nameof(ArrangeTestResultGridModel.TestDate):
                        if (!desc)
                            query = query.OrderBy(p => p.TestDate);
                        else
                            query = query.OrderByDescending(p => p.TestDate);
                        break;
                    case nameof(ArrangeTestResultGridModel.ObtainedMarks):
                        if (!desc)
                            query = query.OrderBy(p => p.ObtainedMarks);
                        else
                            query = query.OrderByDescending(p => p.ObtainedMarks);
                        break;
                    case nameof(ArrangeTestResultGridModel.OutOfMarks):
                        if (!desc)
                            query = query.OrderBy(p => p.OutOfMarks);
                        else
                            query = query.OrderByDescending(p => p.OutOfMarks);
                        break;

                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }


            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }


            return query.ToList();
        }

        public ArrangeTestResultProjection GetArrangeTestResultById(int arrangeTestResultId)
        {
            return _repository.Project<ArrangeTestResult, ArrangeTestResultProjection>(
                    arrangeTestResults => (from arrangeTestResult in arrangeTestResults
                                           where arrangeTestResult.ArrangeTestResultId == arrangeTestResultId
                                           select new ArrangeTestResultProjection
                                           {
                                               ArrangeTestResultId = arrangeTestResult.ArrangeTestResultId,
                                               Questions = arrangeTestResult.Questions,
                                               StartTime = arrangeTestResult.StartTime,
                                               TestDate = arrangeTestResult.TestDate,
                                               TestPaperId = arrangeTestResult.TestPaperId,
                                               TimeDuration = arrangeTestResult.TimeDuration,
                                               StudentName = arrangeTestResult.Student.Student.FirstName + " " + arrangeTestResult.Student.Student.LastName,
                                               TestPaperTitle = arrangeTestResult.TestPapers.Title,
                                               ObtainedMarks = arrangeTestResult.ObtainedMarks,
                                               OutOfMarks = arrangeTestResult.OutOfMarks
                                           }).FirstOrDefault());
        }
    }
}
