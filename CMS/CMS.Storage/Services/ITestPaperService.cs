using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface ITestPaperService
    {
        CMSResult Save(TestPaper newTest);
        CMSResult Update(TestPaper oldTest);
        CMSResult Delete(int TestPaperId);
        IEnumerable<TestPaperProjection> getTestPapers();
        IEnumerable<TestPaperProjection> GetTestPaperById(int id);
        TestPaperProjection GetPaperById(int paperId);
        List<QuestionCountProjection> GetCountChapterWise(string DelimitedChapterIds, out int chapterId);
        IEnumerable<TestPaperGridModel> GetTestPaperData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        CMSResult SaveArrengeTest(ArrengeTest newArrengeTest);
        CMSResult UpdateTestStatus(TestPaper oldTest);
        IEnumerable<ArrangeTestGridModel> GetArrangeTestData(out int totalRecords,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        ArrangeTestProjection GetArrangeTestById(int id);
        bool TestIsExit(TestPaper newTest);
        IEnumerable<TestPaperProjection> getTestPapersDelimitedQuestion();
        IEnumerable<TestPaperProjection> GetTestPapersClasses();
        IEnumerable<TestPaperProjection> GetTestPapersByClassId(int classId);
    }
}
