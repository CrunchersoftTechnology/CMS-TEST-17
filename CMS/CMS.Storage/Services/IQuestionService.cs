using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IQuestionService
    {
        IEnumerable<QuestionProjection> GetQuestion();
        CMSResult Save(Question newQuestion);
        CMSResult Update(Question oldQuestion);
        CMSResult Delete(int id);
        QuestionProjection GetQuestionById(int questionSequence, int chapterId);
        int GetQuestionCount(int ClassId, int SubjectId, int ChapterId);
        CMSResult SaveQuestionImage(int QuestionId, bool IsDelete, string FolderPath);
        CMSResult SaveOptionImage(int QuestionId, bool IsDelete, string FolderPath);
        CMSResult SaveHintImage(int QuestionId, bool IsDelete, string FolderPath);
        CMSResult DeleteImage(int questionId, int imageType);
        List<QuestionCountProjection> GetQuestionCountsByChapters(string subjectId);
        int GetQuesCountByChapterId(int ChapterId, int level, int type, int asked, int used, int hint);
        QuestionDetailWithCountProjection GetQuesCountByChapterIdFilter(int questionSequence, int ChapterId, int level, int type, int asked, int used, int hint, out int count);
        IEnumerable<QuestionProjection> GetQuestionByChapter(int ChapterId);
        IEnumerable<QuestionProjection> GetQuestionByQuestionId(string QuestionIds);
        IEnumerable<QuestionProjection> GetQuestionsDetailsForStudentAppOnlineTest(List<int> questionIds);
    }
}
