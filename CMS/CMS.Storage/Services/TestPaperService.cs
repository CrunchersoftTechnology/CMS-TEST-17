using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class TestPaperService : ITestPaperService
    {
        readonly IRepository _repository;

        public TestPaperService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(TestPaper newTest)
        {
            CMSResult cmsresult = new CMSResult();
            var isExists = _repository.Project<TestPaper, bool>(
                    tests => (from t in tests
                              where t.Title == newTest.Title && t.ClassId == newTest.ClassId
                              select t).Any());

            if (isExists)
            {
                cmsresult.Results.Add(new Result("Title already exists!", false));
            }
            else
            {
                _repository.Add(newTest);
                _repository.CommitChanges();
                cmsresult.Results.Add(new Result("Test paper added successfully!", true));
            }
            return cmsresult;
        }

        public CMSResult Update(TestPaper oldTest)
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();
            var testPaper = _repository.Load<TestPaper>(x => x.TestPaperId == oldTest.TestPaperId);
            if (testPaper == null)
            {
                result.IsSuccessful = false;
                result.Message = "Test paper not exist!";
            }
            else
            {
                var isExists = _repository.Project<TestPaper, bool>(
                    tests => (from t in tests
                              where t.Title == oldTest.Title &&
                              t.TestPaperId != oldTest.TestPaperId
                              select t).Any());

                if (isExists)
                {
                    result.IsSuccessful = false;
                    result.Message = string.Format("Test paper {0} already exists!", oldTest.Title);
                }
                else
                {
                    testPaper.ClassId = oldTest.ClassId;
                    testPaper.DelimitedQuestionIds = oldTest.DelimitedQuestionIds;
                    testPaper.TestType = oldTest.TestType;
                    testPaper.Title = oldTest.Title;
                    testPaper.DelimitedChapterIds = oldTest.DelimitedChapterIds;
                    testPaper.QuestionCount = oldTest.QuestionCount;
                    _repository.Update(testPaper);

                    result.IsSuccessful = true;
                    result.Message = string.Format("Test paper added successfully!");
                }
            }
            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public IEnumerable<TestPaperProjection> getTestPapers()
        {
            return _repository.Project<TestPaper, TestPaperProjection[]>(
                tests => (from t in tests
                          orderby t.CreatedOn descending
                          select new TestPaperProjection
                          {
                              TestPaperId = t.TestPaperId,
                              Title = t.Title,
                              TestType = t.TestType,
                              TestTaken = t.TestTaken,
                              ClassName = t.Class.Name,
                              CreatedOn = t.CreatedOn,
                              DelimitedQuestionIds = t.DelimitedQuestionIds
                          }).ToArray());
        }

        public IEnumerable<TestPaperProjection> GetTestPaperById(int id)
        {
            return _repository.Project<TestPaper, TestPaperProjection[]>(
               tests => (from t in tests
                         select new TestPaperProjection
                         {
                             TestPaperId = t.TestPaperId,
                             Title = t.Title,
                             TestType = t.TestType,
                             TestTaken = t.TestTaken,
                             ClassId = t.ClassId,
                             DelimitedQuestionIds = t.DelimitedQuestionIds
                         }).ToArray());
        }

        public TestPaperProjection GetPaperById(int paperId)
        {
            return _repository.Project<TestPaper, TestPaperProjection>(
                tests => (from t in tests
                          where t.TestPaperId == paperId
                          select new TestPaperProjection
                          {
                              TestPaperId = t.TestPaperId,
                              Title = t.Title,
                              TestType = t.TestType,
                              TestTaken = t.TestTaken,
                              ClassId = t.ClassId,
                              DelimitedQuestionIds = t.DelimitedQuestionIds,
                              DelimitedChapterIds = t.DelimitedChapterIds,
                              CreatedOn = t.CreatedOn,
                              ClassName = t.Class.Name,
                              SubjectName = t.SubjectName
                          }).FirstOrDefault());
        }

        public List<QuestionCountProjection> GetCountChapterWise(string DelimitedChapterIds, out int chapterId)
        {
            var commaseperatedList = DelimitedChapterIds ?? string.Empty;
            var chapterIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

            chapterId = chapterIds.FirstOrDefault();

            return _repository.Project<Question, List<QuestionCountProjection>>(
               tests => (from t in tests
                         where chapterIds.Contains(t.ChapterId)
                         group t by new { t.ChapterId, t.Chapter.Name, t.Chapter.Subject.SubjectId, SubjectName = t.Chapter.Subject.Name } into grouping
                         select new QuestionCountProjection
                         {
                             ChapterId = grouping.Key.ChapterId,
                             ChapterName = grouping.Key.Name,
                             SubjectId = grouping.Key.SubjectId,
                             SubjectName = grouping.Key.SubjectName,
                             TotalQuestion = grouping.Count(),
                             Easy = grouping.Where(x => x.QuestionLevel == QuestionLevel.Easy).Count(),
                             Medium = grouping.Where(x => x.QuestionLevel == QuestionLevel.Medium).Count(),
                             Hard = grouping.Where(x => x.QuestionLevel == QuestionLevel.Hard).Count(),
                         }).ToList());
        }

        public CMSResult Delete(int TestPaperId)
        {
            CMSResult result = new CMSResult();

            var model = _repository.Load<TestPaper>(b => b.TestPaperId == TestPaperId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Test Paper already exists!") });
            }
            else
            {
                var isExistsArrangeTest = _repository.Project<ArrengeTest, bool>(arrangeTest => (
                                         from c in arrangeTest
                                         where c.TestPaperId == TestPaperId
                                         select c)
                                         .Any());
                //var isExists = _repository.Project<TestPaper, bool>(tests => (
                //                from t in tests
                //                where t.TestPaperId == TestPaperId
                //                select t
                //            ).Any());
                if (isExistsArrangeTest)
                {
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Test Paper '{0}'. Because it belongs to {1}!", model.Title, "Arrange test") });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Test Paper '{0}' deleted successfully!", model.Title) });
                }
            }
            return result;
        }

        public IEnumerable<TestPaperGridModel> GetTestPaperData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<TestPaper, IQueryable<TestPaperGridModel>>(machines => (
                 from t in machines
                 select new TestPaperGridModel
                 {
                     TestPaperId = t.TestPaperId,
                     ClassName = t.Class.Name,
                     TestTaken = t.TestTaken,
                     TestType = t.TestType,
                     Title = t.Title,
                     DelimitedChapterIds = t.DelimitedQuestionIds,
                     CreatedOn = t.CreatedOn,
                     QuestionCount = t.QuestionCount,
                     SubjectName = t.SubjectName

                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(TestPaperGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(TestPaperGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(TestPaperGridModel.TestTaken):
                        if (!desc)
                            query = query.OrderBy(p => p.TestTaken);
                        else
                            query = query.OrderByDescending(p => p.TestTaken);
                        break;
                    case nameof(TestPaperGridModel.TestType):
                        if (!desc)
                            query = query.OrderBy(p => p.TestType);
                        else
                            query = query.OrderByDescending(p => p.TestType);
                        break;
                    case nameof(TestPaperGridModel.QuestionCount):
                        if (!desc)
                            query = query.OrderBy(p => p.QuestionCount);
                        else
                            query = query.OrderByDescending(p => p.QuestionCount);
                        break;
                    case nameof(TestPaperGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
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

        public CMSResult SaveArrengeTest(ArrengeTest newArrengeTest)
        {
            CMSResult cmsresult = new CMSResult();
            //var isExists = _repository.Project<ArrengeTest, bool>(
            //        arrengeTestests => (from t in arrengeTestests
            //                            where t.TestPaperId == newArrengeTest.TestPaperId && t.SelectedBatches == newArrengeTest.SelectedBatches
            //                            && t.SelectedBranches == newArrengeTest.SelectedBranches
            //                  select t).Any());

            //if (isExists)
            //{
            //    cmsresult.Results.Add(new Result("Test already Arrenge for this batch!", false));
            //}
            //else
            //{
            _repository.Add(newArrengeTest);
            _repository.CommitChanges();
            cmsresult.Results.Add(new Result("Test Arrenge added successfully!", true));
            // }
            return cmsresult;
        }

        public CMSResult UpdateTestStatus(TestPaper oldTest)
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();
            var testPaper = _repository.Load<TestPaper>(x => x.TestPaperId == oldTest.TestPaperId);
            if (testPaper == null)
            {
                result.IsSuccessful = false;
                result.Message = "Test paper not exist!";
            }
            else
            {
                var isExists = _repository.Project<TestPaper, bool>(
                    tests => (from t in tests
                              where t.Title == oldTest.Title &&
                              t.TestPaperId != oldTest.TestPaperId
                              select t).Any());

                if (isExists)
                {
                    result.IsSuccessful = false;
                    result.Message = string.Format("Test paper {0} already exists!", oldTest.Title);
                }
                else
                {
                    testPaper.TestTaken = true;

                    _repository.Update(testPaper);
                    result.IsSuccessful = true;
                    result.Message = string.Format("Test paper added successfully!");
                }
            }
            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public IEnumerable<ArrangeTestGridModel> GetArrangeTestData(out int totalRecords,
      int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<ArrengeTest, IQueryable<ArrangeTestGridModel>>(machines => (
                 from t in machines
                 select new ArrangeTestGridModel
                 {
                     ArrengeTestId = t.ArrengeTestId,
                     TestType = t.TestPapers.TestType.ToString(),
                     Title = t.TestPapers.Title,
                     CreatedOn = t.CreatedOn,
                     StudentCount = t.StudentCount,
                     SelectedClass = t.TestPapers.Class.Name,
                     SubjectName = t.TestPapers.SubjectName
                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ArrangeTestGridModel.SelectedClass):
                        if (!desc)
                            query = query.OrderBy(p => p.SelectedClass);
                        else
                            query = query.OrderByDescending(p => p.SelectedClass);
                        break;
                    case nameof(ArrangeTestGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(ArrangeTestGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(ArrangeTestGridModel.TestType):
                        if (!desc)
                            query = query.OrderBy(p => p.TestType);
                        else
                            query = query.OrderByDescending(p => p.TestType);
                        break;
                    case nameof(ArrangeTestGridModel.StudentCount):
                        if (!desc)
                            query = query.OrderBy(p => p.StudentCount);
                        else
                            query = query.OrderByDescending(p => p.StudentCount);
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

        public ArrangeTestProjection GetArrangeTestById(int id)
        {
            return _repository.Project<ArrengeTest, ArrangeTestProjection>(
                ArrengeTest => (from a in ArrengeTest
                                where a.ArrengeTestId == id
                                select new ArrangeTestProjection
                                {
                                    Title = a.TestPapers.Title,
                                    TestType = a.TestPapers.TestType.ToString(),
                                    CreatedOn = a.CreatedOn,
                                    StudentCount = a.StudentCount,
                                    Media = a.Media,
                                    SelectedBranches = a.SelectedBranches,
                                    SelectedBatches = a.SelectedBatches,
                                    SelectedClass = a.TestPapers.Class.Name,
                                    SubjectName = a.TestPapers.SubjectName,
                                    Date = a.Date,
                                    StartTime = a.StartTime,
                                    TimeDuration = a.TimeDuration
                                }).FirstOrDefault());
        }

        public bool TestIsExit(TestPaper newTest)
        {
            var isExists = _repository.Project<TestPaper, bool>(
                    tests => (from t in tests
                              where t.Title == newTest.Title && t.ClassId == newTest.ClassId
                              select t).Any());
            return isExists;
        }

        public IEnumerable<TestPaperProjection> getTestPapersDelimitedQuestion()
        {
            return _repository.Project<TestPaper, TestPaperProjection[]>(
                tests => (from t in tests
                          orderby t.CreatedOn descending
                          select new TestPaperProjection
                          {
                              DelimitedQuestionIds = t.DelimitedQuestionIds
                          }).ToArray());
        }

        public IEnumerable<TestPaperProjection> GetTestPapersClasses()
        {
            return _repository.Project<TestPaper, TestPaperProjection[]>(
                    testPapers => (from testPaper in testPapers
                                   select new TestPaperProjection
                                   {
                                       ClassId = testPaper.ClassId,
                                       ClassName = testPaper.Class.Name
                                   }).ToArray());
        }

        public IEnumerable<TestPaperProjection> GetTestPapersByClassId(int classId)
        {
            return _repository.Project<TestPaper, TestPaperProjection[]>(
                    testPapers => (from testPaper in testPapers
                                   select new TestPaperProjection
                                   {
                                       TestPaperId = testPaper.TestPaperId,
                                       Title = testPaper.Title
                                   }).ToArray());
        }
    }
}
