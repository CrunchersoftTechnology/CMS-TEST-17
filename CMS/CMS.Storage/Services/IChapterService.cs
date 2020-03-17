using CMS.Domain.Models;
using System.Collections.Generic;
using CMS.Domain.Storage.Projections;
using CMS.Common;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public interface IChapterService
    {
        IEnumerable<ChapterProjection> GetChapters(int subjectId, int classId);
        IEnumerable<ChapterProjection> GetAllChapters();
        CMSResult Save(int ClientId,Chapter newChapter);
        CMSResult Update(Chapter oldChapter);
        CMSResult Delete(int id);
        ChapterProjection GetChapterById(int chapterId);
        IEnumerable<ChapterProjection> GetAllPaperChapters(int subjectId);
        int GetCountWeightage(int classId, int subjectId);
        IEnumerable<ChapterGridModel> GetChapterData(out int totalRecords, int filterClassName, int filterSubjectName,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc);

        IEnumerable<ChapterGridModel> GetChapterDataByClientId(out int totalRecords, int filterClassName, int filterSubjectName,int ClientId,
       int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }
}
