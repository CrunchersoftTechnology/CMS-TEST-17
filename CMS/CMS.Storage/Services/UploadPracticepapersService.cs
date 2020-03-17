using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadPracticepapersService : IUploadPracticepapersService
    {
        readonly IRepository _repository;

        public UploadPracticepapersService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(UploadPracticepapers newUploadPracticepapers)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadPracticepapers, bool>(uploadPracticepapers => (
                                from p in uploadPracticepapers
                                where p.FileName == newUploadPracticepapers.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Practicepapers file '{0}' already exists!", newUploadPracticepapers.FileName) });
            }
            else
            {
                _repository.Add(newUploadPracticepapers);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Practicepapers file '{0}' added successfully!", newUploadPracticepapers.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadPracticepapersGridModel> GetUploadPracticepapersData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadPracticepapers, IQueryable<UploadPracticepapersGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadPracticepapersGridModel
                 {
                     UploadPracticepapersId = p.UploadPracticepapersId,
                     Title = p.Title,
                     BoardName = p.BoardName,
                     ClassName = p.ClassName,
                     SubjectName = p.SubjectName,
                     UploadDate = p.UploadDate,
                     FileName = p.FileName,
                     LogoName = p.LogoName,
                     IsVisible = p.IsVisible,
                     CreatedOn = p.CreatedOn
                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(UploadPracticepapersGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadPracticepapersGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadPracticepapersGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadPracticepapersGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadPracticepapersGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadPracticepapersGridModel.IsVisible):
                        if (!desc)
                            query = query.OrderBy(p => p.IsVisible);
                        else
                            query = query.OrderByDescending(p => p.IsVisible);
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

        public UploadPracticepapersProjection GetPracticepapersById(int uploadPracticepapersId)
        {
            return _repository.Project<UploadPracticepapers, UploadPracticepapersProjection>(
                    uploadPracticepapers => (from Practicepapers in uploadPracticepapers
                                             where Practicepapers.UploadPracticepapersId == uploadPracticepapersId
                                             select new UploadPracticepapersProjection
                                             {
                                        UploadPracticepapersId = Practicepapers.UploadPracticepapersId,
                                        BoardName = Practicepapers.BoardName,
                                        ClassName = Practicepapers.ClassName,
                                        SubjectName = Practicepapers.SubjectName,
                                        FileName = Practicepapers.FileName,
                                        LogoName = Practicepapers.LogoName,
                                        UploadDate = Practicepapers.UploadDate,
                                        Title = Practicepapers.Title,
                                        IsVisible = Practicepapers.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadPracticepapers uploadNewPracticepapers)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadPracticepapers, bool>(uploadPracticepapers => (
                                from p in uploadPracticepapers
                                where p.FileName == uploadNewPracticepapers.FileName && p.UploadPracticepapersId != uploadNewPracticepapers.UploadPracticepapersId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Practicepapers file '{0}' already exists!", uploadNewPracticepapers.FileName) });
            }
            else
            {
                var Practicepapers = _repository.Load<UploadPracticepapers>(x => x.UploadPracticepapersId == uploadNewPracticepapers.UploadPracticepapersId);
                Practicepapers.ClassName = uploadNewPracticepapers.ClassName;
                Practicepapers.Title = uploadNewPracticepapers.Title;
                Practicepapers.FileName = uploadNewPracticepapers.FileName;
                Practicepapers.LogoName = uploadNewPracticepapers.LogoName;
                Practicepapers.BoardName = uploadNewPracticepapers.BoardName;
                Practicepapers.SubjectName = uploadNewPracticepapers.SubjectName;
                Practicepapers.UploadDate = uploadNewPracticepapers.UploadDate;
                Practicepapers.IsVisible = uploadNewPracticepapers.IsVisible;
                _repository.Update(Practicepapers);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Practicepapers updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadPracticepapersId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadPracticepapers>(b => b.UploadPracticepapersId == uploadPracticepapersId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Practicepapers not exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Practicepapers deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadPracticepapersProjection> GetUploadPracticepapersList()
        {
            return _repository.Project<UploadPracticepapers, UploadPracticepapersProjection[]>(
                UploadPracticepapers => (from Practicepapers in UploadPracticepapers
                                         where Practicepapers.IsVisible == true
                                orderby Practicepapers.CreatedOn descending
                                select new UploadPracticepapersProjection
                                {
                                    BoardName = Practicepapers.BoardName,
                                    ClassName = Practicepapers.ClassName,
                                    SubjectName = Practicepapers.SubjectName,
                                    Title = Practicepapers.Title,
                                    UploadDate = Practicepapers.UploadDate,
                                    FileName = Practicepapers.FileName,
                                    LogoName = Practicepapers.LogoName,
                                    IsVisible = Practicepapers.IsVisible,
                                    UploadPracticepapersId = Practicepapers.UploadPracticepapersId,
                                    ClassId = Practicepapers.ClassId,
                                    BoardId = Practicepapers.BoardId,
                                    SubjectId = Practicepapers.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadPracticepapersProjection> GetUploadPracticepapersListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadPracticepapers, UploadPracticepapersProjection[]>(
                UploadPracticepapers => (from Practicepapers in UploadPracticepapers
                                         where Practicepapers.IsVisible == true && Practicepapers.SubjectId == subjectId
                                orderby Practicepapers.CreatedOn descending
                                select new UploadPracticepapersProjection
                                {
                                    Title = Practicepapers.Title,
                                    UploadPracticepapersId = Practicepapers.UploadPracticepapersId
                                }).ToArray());
        }

    }
}
