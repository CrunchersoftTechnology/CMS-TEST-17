using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;
using CMS.Domain.Models;
using CMS.Domain.Infrastructure;
using CMS.Common;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class ChapterService : IChapterService
    {
        readonly IRepository _repository;

        public ChapterService(IRepository repository)
        {
            _repository = repository;
        }
        public IEnumerable<ChapterProjection> GetChapters(int subjectId, int classId)
        {
            return _repository.Project<Chapter, ChapterProjection[]>(
                chapters => (from chapter in chapters
                             where chapter.SubjectId == subjectId
                             && chapter.Subject.ClassId == classId
                            
                             select new ChapterProjection
                             {
                                 ChapterId = chapter.ChapterId,
                                 ChapterName = chapter.Name,
                                 SubjectName = chapter.Subject.Name,
                                 Weightage = chapter.Weightage,
                                 ClassName = chapter.Subject.Class.Name
                             }).ToArray());
        }

        public IEnumerable<ChapterProjection> GetAllChapters()
        {
            return _repository.Project<Chapter, ChapterProjection[]>(
                chapters => (from chapter in chapters
                             select new ChapterProjection
                             {
                                 ChapterId = chapter.ChapterId,
                                 SubjectId = chapter.SubjectId,
                                 ChapterName = chapter.Name,
                                 SubjectName = chapter.Subject.Name,
                                 Weightage = chapter.Weightage,
                                 ClassName = chapter.Subject.Class.Name
                             }).ToArray());
        }

        public CMSResult Save(int ClientId,Chapter newChapter)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Chapter, bool>(chapters => (from chap in chapters where chap.Name == newChapter.Name && chap.SubjectId == newChapter.SubjectId && chap.ClientId==newChapter.ClientId select chap).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Chapter '{0}' already exists!", newChapter.Name) });
            }
            else
            {
                newChapter.ClientId = ClientId;
                _repository.Add(newChapter);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Chapter '{0}' successfully added!", newChapter.Name) });
            }
            return result;
        }

        public CMSResult Update(Chapter oldChapter)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Chapter, bool>(chapters =>
            (from chap in chapters
             where chap.ChapterId != oldChapter.ChapterId && chap.SubjectId == oldChapter.SubjectId && chap.Name == oldChapter.Name
             select chap).Any());

            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Chapter '{0}' already exists!", oldChapter.Name) });
            }
            else
            {

                var chapt = _repository.Load<Chapter>(x => x.ChapterId == oldChapter.ChapterId);
                chapt.SubjectId = oldChapter.SubjectId;
                chapt.Name = oldChapter.Name;
                chapt.Weightage = oldChapter.Weightage;
                _repository.Update(chapt);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Chapter '{0}' successfully updated!", oldChapter.Name) });
            }
            return result;
        }

        public CMSResult Delete(int ChapterId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Chapter>(x => x.ChapterId == ChapterId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Chapter '{0}' already exists", model.Name) });
            }
            else
            {
                var isExistsQuestion = _repository.Project<Question, bool>(questions => (
                                            from q in questions
                                            where q.ChapterId == ChapterId
                                            select q)
                                            .Any());

                //var isExistsPaper = _repository.Project<TestPaper, bool>(paper => (
                //                            from t in paper
                //                            where (t.DelimitedChapterIds.Split(',').Select(int.Parse).Contains(ChapterId))
                //                            select t)
                //                            .Any());

                //var isExistsPaper = _repository.Project<TestPaper, bool>(paper => (
                //                            from t in paper
                //                            where (t.DelimitedChapterIds.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse)).Contains(ChapterId)
                //                            select t)
                //                            .Any());

                if (isExistsQuestion)
                {
                    var selectModel = "";
                    selectModel += (isExistsQuestion) ? "Question, " : "";
                   // selectModel += (isExistsPaper) ? "Paper, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Chapter '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Chapter '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }
        public ChapterProjection GetChapterById(int chaptertId)
        {
            return _repository.Project<Chapter, ChapterProjection>(
                chapters => (from c in chapters
                             where c.ChapterId == chaptertId
                             select new ChapterProjection
                             {
                                 ChapterId = c.ChapterId,
                                 SubjectId = c.SubjectId,
                                 SubjectName = c.Subject.Name,
                                 ChapterName = c.Name,
                                 Weightage = c.Weightage,
                                 ClassId = c.Subject.ClassId,
                                 ClassName = c.Subject.Class.Name
                             }).FirstOrDefault());
        }

        public IEnumerable<ChapterProjection> GetAllPaperChapters(int subjectId)
        {
            return _repository.Project<Chapter, ChapterProjection[]>(
                chapters => (from chapter in chapters
                             where chapter.SubjectId == subjectId
                             select new ChapterProjection
                             {
                                 ChapterId = chapter.ChapterId,
                                 ChapterName = chapter.Name
                             }).ToArray());
        }

        public int GetCountWeightage(int classId, int subjectId)
        {
            var d = _repository.Project<Chapter, int>(
               ChapWeightages => (from i in ChapWeightages
                                  where i.Subject.ClassId == classId && i.SubjectId == subjectId
                                  select i.Weightage
                                ).DefaultIfEmpty(0).Sum());
            return d;

        }

        public IEnumerable<ChapterGridModel> GetChapterData(out int totalRecords, int filterClassName, int filterSubjectName,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Chapter, IQueryable<ChapterGridModel>>(chapters => (
                 from c in chapters
                 select new ChapterGridModel
                 {
                     ChapterId = c.ChapterId,
                     ChapterName =c.Name,
                     ClassName = c.Subject.Class.Name,
                     SubjectName = c.Subject.Name,
                     SubjectId = c.SubjectId,
                     Weightage=c.Weightage,
                     ClassId=c.Subject.Class.ClassId,
                     CreatedOn =c.CreatedOn,

                 })).AsQueryable();

            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }
            if (filterSubjectName != 0)
            {
                query = query.Where(p => p.SubjectId == filterSubjectName);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ChapterGridModel.ChapterName):
                        if (!desc)
                            query = query.OrderBy(p => p.ChapterName);
                        else
                            query = query.OrderByDescending(p => p.ChapterName);
                        break;
                    case nameof(ChapterGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(ChapterGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(ChapterGridModel.Weightage):
                        if (!desc)
                            query = query.OrderBy(p => p.Weightage);
                        else
                            query = query.OrderByDescending(p => p.Weightage);
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

        public IEnumerable<ChapterGridModel> GetChapterDataByClientId(out int totalRecords, int filterClassName, int filterSubjectName, int ClientId,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Chapter, IQueryable<ChapterGridModel>>(chapters => (
                 from c in chapters
                 where c.ClientId == ClientId
                 select new ChapterGridModel
                 {
                     ChapterId = c.ChapterId,
                     ChapterName = c.Name,
                     ClassName = c.Subject.Class.Name,
                     SubjectName = c.Subject.Name,
                     SubjectId = c.SubjectId,
                     Weightage = c.Weightage,
                     ClassId = c.Subject.Class.ClassId,
                     CreatedOn = c.CreatedOn,

                 })).AsQueryable();

            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName && p.ClientId == ClientId);
            }
            if (filterSubjectName != 0)
            {
                query = query.Where(p => p.SubjectId == filterSubjectName && p.ClientId == ClientId);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ChapterGridModel.ChapterName):
                        if (!desc)
                            query = query.OrderBy(p => p.ChapterName);
                        else
                            query = query.OrderByDescending(p => p.ChapterName);
                        break;
                    case nameof(ChapterGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(ChapterGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(ChapterGridModel.Weightage):
                        if (!desc)
                            query = query.OrderBy(p => p.Weightage);
                        else
                            query = query.OrderByDescending(p => p.Weightage);
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
    }
}
