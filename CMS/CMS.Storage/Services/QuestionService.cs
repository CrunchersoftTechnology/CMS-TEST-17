using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Collections;

namespace CMS.Domain.Storage.Services
{
    public class QuestionService : IQuestionService
    {
        readonly IRepository _repository;

        public QuestionService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Delete(int questionId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Question>(x => x.QuestionId == questionId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Question '{0}' not exists!", model.QuestionId) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Question '{0}' deleted successfully!", model.QuestionId) });
            }
            return result;
        }

        public IEnumerable<QuestionProjection> GetQuestion()
        {
            return _repository.Project<Question, QuestionProjection[]>(
                questions => (from q in questions
                              select new QuestionProjection
                              {
                                  QuestionId = q.QuestionId,
                                  QuestionInfo = q.QuestionInfo,
                                  Option1 = q.OptionA,
                                  Option2 = q.OptionB,
                                  Option3 = q.OptionC,
                                  Option4 = q.OptionD,
                                  Hint = q.Hint,
                                  Numerical_Answer=q.Numerical_Answer,
                                  Unit=q.Unit,
                                  ChapterId = q.ChapterId,
                                  QuestionLevel = q.QuestionLevel,
                                  QuestionType = q.QuestionType,
                                  QuestionYear = q.QuestionYear,
                                  ClassId = q.Chapter.Subject.ClassId,
                                  SubjectId = q.Chapter.SubjectId,
                                  ClassName = q.Chapter.Subject.Class.Name,
                                  SubjectName = q.Chapter.Subject.Name,
                                  ChapterName = q.Chapter.Name,
                                  QuestionImagePath = q.QuestionImagePath,
                                  HintImagePath = q.HintImagePath,
                                  OptionImagePath = q.OptionImagePath,
                                  Answer = q.Answer.ToString()
                              }).ToArray());
        }

        public CMSResult Save(Question newQuestion)
        {
            CMSResult cmsresult = new CMSResult();
            _repository.Add(newQuestion);
            _repository.CommitChanges();
            cmsresult.Results.Add(new Result("Question added successfully!", true));
            return cmsresult;
        }

        public CMSResult Update(Question oldQuestion)
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();
            var ques = _repository.Load<Question>(x => x.QuestionId == oldQuestion.QuestionId && x.ChapterId == oldQuestion.ChapterId);
            if (ques == null)
            {
                result.IsSuccessful = false;
                result.Message = "Question not exist!";
            }
            else
            {
                ques.QuestionInfo = oldQuestion.QuestionInfo;
                ques.OptionA = oldQuestion.OptionA;
                ques.OptionB = oldQuestion.OptionB;
                ques.OptionC = oldQuestion.OptionC;
                ques.OptionD = oldQuestion.OptionD;
                ques.Hint = oldQuestion.Hint;
                ques.Numerical_Answer = oldQuestion.Numerical_Answer;
                ques.Unit = oldQuestion.Unit;
                ques.QuestionLevel = oldQuestion.QuestionLevel;
                ques.QuestionType = oldQuestion.QuestionType;
                ques.QuestionYear = oldQuestion.QuestionYear;
                ques.Answer = oldQuestion.Answer;
                ques.ChapterId = oldQuestion.ChapterId;

                _repository.Update(ques);

                result.IsSuccessful = true;
                result.Message = string.Format("Question updated successfully!");
            }
            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public QuestionProjection GetQuestionById(int questionSequence, int chapterId)
        {
            if (questionSequence == 0)
                return null;

            var skip = questionSequence - 1;
            return _repository.Project<Question, QuestionProjection>(
                questions => (from q in questions
                              orderby q.QuestionId
                              where q.ChapterId == chapterId
                              select new QuestionProjection
                              {
                                  QuestionId = q.QuestionId,
                                  QuestionInfo = q.QuestionInfo,
                                  Option1 = q.OptionA,
                                  Option2 = q.OptionB,
                                  Option3 = q.OptionC,
                                  Option4 = q.OptionD,
                                  Hint = q.Hint,
                                  Numerical_Answer=q.Numerical_Answer,
                                  Unit=q.Unit,
                                  ChapterId = q.ChapterId,
                                  QuestionLevel = q.QuestionLevel,
                                  QuestionType = q.QuestionType,
                                  QuestionYear = q.QuestionYear,
                                  ClassId = q.Chapter.Subject.ClassId,
                                  SubjectId = q.Chapter.SubjectId,
                                  ClassName = q.Chapter.Subject.Class.Name,
                                  SubjectName = q.Chapter.Subject.Name,
                                  ChapterName = q.Chapter.Name,
                                  QuestionImagePath = q.QuestionImagePath,
                                  HintImagePath = q.HintImagePath,
                                  OptionImagePath = q.OptionImagePath,
                                  Answer = q.Answer.ToString(),
                                  IsHintAsImage = q.IsHintAsImage,
                                  IsOptionAsImage = q.IsOptionAsImage,
                                  IsQuestionAsImage = q.IsQuestionAsImage
                              }).Skip(skip).Take(questionSequence).FirstOrDefault());
        }

        public int GetQuestionCount(int ClassId, int SubjectId, int ChapterId)
        {
            var questionList = _repository.Project<Question, IQueryable<Question>>(
                questions => (from q in questions
                              where q.Chapter.Subject.ClassId == ClassId
                              select q).Include(x => x.Chapter));

            if (SubjectId != 0)
            {
                questionList = questionList.Where(x => x.Chapter.SubjectId == SubjectId);
            }

            if (ChapterId != 0)
            {
                questionList = questionList.Where(x => x.ChapterId == ChapterId);
            }

            return questionList.Count();

        }

        public CMSResult SaveQuestionImage(int questionId, bool isDelete, string folderPath)
        {
            CMSResult CMSResult = new CMSResult();
            var result = new Result();
            var question = _repository.Load<Question>(x => x.QuestionId == questionId);
            if (question == null)
            {
                result.IsSuccessful = false;
                result.Message = "Question not exist!";
            }
            else
            {
                question.QuestionImagePath = isDelete ? null : string.Format(@"Q{0}.jpg", questionId);
                _repository.Update(question);

                result.IsSuccessful = true;
                result.Message = "Question image saved successfully!";
            }
            return CMSResult;
        }

        public CMSResult SaveOptionImage(int questionId, bool isDelete, string folderPath)
        {
            CMSResult CMSResult = new CMSResult();
            var result = new Result();
            var question = _repository.Load<Question>(x => x.QuestionId == questionId);
            if (question == null)
            {
                result.IsSuccessful = false;
                result.Message = "Question not exist!";
            }
            else
            {
                question.OptionImagePath = isDelete ? null : string.Format(@"O{0}.jpg", questionId);
                _repository.Update(question);

                result.IsSuccessful = true;
                result.Message = "Option image saved successfully!";
            }
            return CMSResult;
        }

        public CMSResult SaveHintImage(int questionId, bool isDelete, string folderPath)
        {
            CMSResult CMSResult = new CMSResult();
            var result = new Result();
            var question = _repository.Load<Question>(x => x.QuestionId == questionId);
            if (question == null)
            {
                result.IsSuccessful = false;
                result.Message = "Question not exist!";
            }
            else
            {
                question.HintImagePath = isDelete ? null : string.Format(@"H{0}.jpg", questionId);
                _repository.Update(question);

                result.IsSuccessful = true;
                result.Message = "Hint image saved successfully!";
            }
            return CMSResult;
        }

        public CMSResult DeleteImage(int questionId, int imageType)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Question>(x => x.QuestionId == questionId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = "Question image not exists!" });
            }
            else
            {
                if (imageType == 1)
                    model.QuestionImagePath = string.Empty;
                if (imageType == 2)
                    model.OptionImagePath = string.Empty;
                if (imageType == 3)
                    model.HintImagePath = string.Empty;
                _repository.Update(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = "Question image deleted successfully!" });
            }
            return result;
        }

        public List<QuestionCountProjection> GetQuestionCountsByChapters(string subjects)
        {
            var subjectIds = subjects.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Question, List<QuestionCountProjection>>(qs =>
            (from q in qs
             where subjectIds.Contains(q.Chapter.SubjectId)
             group q by new { q.Chapter.Name, q.ChapterId, q.Chapter.SubjectId, subjectName = q.Chapter.Subject.Name } into grouping

             select new QuestionCountProjection
             {
                 ChapterName = grouping.Key.Name,
                 ChapterId = grouping.Key.ChapterId,
                 Easy = grouping.Where(x => x.QuestionLevel == QuestionLevel.Easy).Count(),
                 Medium = grouping.Where(x => x.QuestionLevel == QuestionLevel.Medium).Count(),
                 Hard = grouping.Where(x => x.QuestionLevel == QuestionLevel.Hard).Count(),
                 Numerical = grouping.Where(x => x.QuestionType == QuestionType.Numerical).Count(),
                 Theoretical = grouping.Where(x => x.QuestionType == QuestionType.Theoretical).Count(),
                 NewPatternNumerical = grouping.Where(x => x.QuestionType == QuestionType.NewPatternNumerical).Count(),
                 WithHint = grouping.Where(x => !string.IsNullOrEmpty(x.Hint)).Count(),
                 WithOutHint = grouping.Where(x => string.IsNullOrEmpty(x.Hint)).Count(),
                 Asked = grouping.Where(x => !string.IsNullOrEmpty(x.QuestionYear)).Count(),
                 NonAsked = grouping.Where(x => string.IsNullOrEmpty(x.QuestionYear)).Count(),
                 TotalQuestion = grouping.Where(x => subjectIds.Contains(x.Chapter.SubjectId)).Count(),
                 SubjectId = grouping.Key.SubjectId,
                 SubjectName = grouping.Key.subjectName
             }).ToList());
        }

        public int GetQuesCountByChapterId(int ChapterId, int level, int type, int asked, int used, int hint)
        {
            var questionList = _repository.Project<Question, IQueryable<Question>>(
                questions => (from q in questions
                              where q.ChapterId == ChapterId
                              select q));
            if (level != 0)
            {
                questionList = questionList.Where(x => (int)x.QuestionLevel == level);
            }

            if (type != 0)
            {
                questionList = questionList.Where(x => (int)x.QuestionType == type);
            }

            if (asked != 0)
            {
                if (asked == 1)
                    questionList = questionList.Where(x => !string.IsNullOrEmpty(x.QuestionYear));
                else if (asked == 2)
                    questionList = questionList.Where(x => string.IsNullOrEmpty(x.QuestionYear));
            }

            if (hint != 0)
            {
                if (hint == 1)
                    questionList = questionList.Where(x => !string.IsNullOrEmpty(x.Hint));
                else if (hint == 2)
                    questionList = questionList.Where(x => string.IsNullOrEmpty(x.Hint));
            }

            return questionList.Count();

        }

        public QuestionDetailWithCountProjection GetQuesCountByChapterIdFilter(int questionSequence, int ChapterId, int level, int type, int asked, int used, int hint, out int count)
        {
            count = 0;
            if (questionSequence == 0)
                return null;

            var skip = questionSequence - 1;

            var questionList = _repository.Project<Question, IQueryable<Question>>(
                questions => (from q in questions
                              where q.ChapterId == ChapterId
                              select q));
            if (level != 0)
            {
                questionList = questionList.Where(x => (int)x.QuestionLevel == level);
            }

            if (type != 0)
            {
                questionList = questionList.Where(x => (int)x.QuestionType == type);
            }

            if (asked != 0)
            {
                if (asked == 1)
                    questionList = questionList.Where(x => !string.IsNullOrEmpty(x.QuestionYear));
                else if (asked == 2)
                    questionList = questionList.Where(x => string.IsNullOrEmpty(x.QuestionYear));
            }

            if (hint != 0)
            {
                if (hint == 1)
                    questionList = questionList.Where(x => !string.IsNullOrEmpty(x.Hint));
                else if (hint == 2)
                    questionList = questionList.Where(x => string.IsNullOrEmpty(x.Hint));
            }

            count = questionList.Count();

            var quest = questionList.OrderBy(x => x.QuestionId).Skip(skip).Take(questionSequence).FirstOrDefault();

            if (quest != null)
            {
                var dto = new QuestionDetailWithCountProjection
                {
                    EasyCount = questionList.Where(x => x.QuestionLevel == QuestionLevel.Easy).Count(),
                    MediumCount = questionList.Where(x => x.QuestionLevel == QuestionLevel.Medium).Count(),
                    HardCount = questionList.Where(x => x.QuestionLevel == QuestionLevel.Hard).Count(),
                    TheoreticalCount = questionList.Where(x => x.QuestionType == QuestionType.Theoretical).Count(),
                    NumericalCount = questionList.Where(x => x.QuestionType == QuestionType.Numerical).Count(),
                    NewPatternNumericalCount = questionList.Where(x => x.QuestionType == QuestionType.NewPatternNumerical).Count(),
                    AskedCount = questionList.Where(x => !string.IsNullOrEmpty(x.QuestionYear)).Count(),
                    NotAskedCount = questionList.Where(x => string.IsNullOrEmpty(x.QuestionYear)).Count(),
                    HintCount = questionList.Where(x => !string.IsNullOrEmpty(x.Hint)).Count(),
                    WithoutHintCount = questionList.Where(x => string.IsNullOrEmpty(x.Hint)).Count(),
                    QuestionId = quest.QuestionId,
                    QuestionInfo = quest.QuestionInfo,
                    OptionA = quest.OptionA,
                    OptionB = quest.OptionB,
                    OptionC = quest.OptionC,
                    OptionD = quest.OptionD,
                    Hint = quest.Hint,
                    Numerical_Answer=quest.Numerical_Answer,
                    Unit=quest.Unit,
                    ChapterId = quest.ChapterId,
                    QuestionLevel = quest.QuestionLevel,
                    QuestionType = quest.QuestionType,
                    QuestionYear = quest.QuestionYear,
                    QuestionImagePath = quest.QuestionImagePath,
                    HintImagePath = quest.HintImagePath,
                    OptionImagePath = quest.OptionImagePath,
                    Answer = quest.Answer.ToString(),
                    IsHintAsImage = quest.IsHintAsImage,
                    IsOptionAsImage = quest.IsOptionAsImage,
                    IsQuestionAsImage = quest.IsQuestionAsImage,
                    UsedCount = used
                };
                return dto;


            }

            return null;

        }

        public IEnumerable<QuestionProjection> GetQuestionByChapter(int ChapterId)
        {
            return _repository.Project<Question, QuestionProjection[]>(
                questions => (from q in questions
                              where q.ChapterId == ChapterId
                              select new QuestionProjection
                              {
                                  QuestionId = q.QuestionId,
                                  QuestionLevel = q.QuestionLevel
                              }).ToArray());
        }

        public IEnumerable<QuestionProjection> GetQuestionByQuestionId(string QuestionIds)
        {
            var questionIds = QuestionIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Question, QuestionProjection[]>(
                questions => (from q in questions
                              where questionIds.Contains(q.QuestionId)
                              select new QuestionProjection
                              {
                                  QuestionId = q.QuestionId,
                                  QuestionLevel = q.QuestionLevel
                              }).ToArray());
        }

        public IEnumerable<QuestionProjection> GetQuestionsDetailsForStudentAppOnlineTest(List<int> questionIds)
        {
            return _repository.Project<Question, QuestionProjection[]>(
                questions => (from q in questions
                              where questionIds.Contains(q.QuestionId)
                              select new QuestionProjection
                              {
                                  QuestionId = q.QuestionId,
                                  QuestionLevel = q.QuestionLevel,
                                  QuestionInfo = q.QuestionInfo,
                                  Option1 = q.OptionA,
                                  Option2 = q.OptionB,
                                  Option3 = q.OptionC,
                                  Option4 = q.OptionD,
                                  QuestionYear = q.QuestionYear,
                                  Answer = q.Answer,
                                  Hint = q.Hint,
                                  Numerical_Answer=q.Numerical_Answer,
                                  Unit=q.Unit,
                                  IsQuestionAsImage = q.IsQuestionAsImage,
                                  IsOptionAsImage = q.IsOptionAsImage,
                                  IsHintAsImage = q.IsHintAsImage,
                                  QuestionImagePath = q.QuestionImagePath,
                                  OptionImagePath = q.OptionImagePath,
                                  HintImagePath = q.HintImagePath,
                                  QuestionType = q.QuestionType,
                                  ClassId = q.Chapter.Subject.ClassId,
                                  SubjectId = q.Chapter.SubjectId,
                                  ChapterId = q.ChapterId,
                                  ClassName = q.Chapter.Subject.Class.Name,
                                  SubjectName = q.Chapter.Subject.Name,
                                  ChapterName = q.Chapter.Name
                              }).ToArray());
        }
    }
}
