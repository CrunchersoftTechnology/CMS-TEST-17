using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadQuestionpapersService : IUploadQuestionpapersService
    {
        readonly IRepository _repository;

        public UploadQuestionpapersService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(UploadQuestionpapers newUploadQuestionpapers)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadQuestionpapers, bool>(uploadQuestionpapers => (
                                from p in uploadQuestionpapers
                                where p.FileName == newUploadQuestionpapers.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Questionpapers file '{0}' already exists!", newUploadQuestionpapers.FileName) });
            }
            else
            {
                _repository.Add(newUploadQuestionpapers);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Questionpapers file '{0}' added successfully!", newUploadQuestionpapers.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadQuestionpapersGridModel> GetUploadQuestionpapersData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadQuestionpapers, IQueryable<UploadQuestionpapersGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadQuestionpapersGridModel
                 {
                     UploadQuestionpapersId = p.UploadQuestionpapersId,
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
                    case nameof(UploadQuestionpapersGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadQuestionpapersGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadQuestionpapersGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadQuestionpapersGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadQuestionpapersGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadQuestionpapersGridModel.IsVisible):
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

        public UploadQuestionpapersProjection GetQuestionpapersById(int uploadQuestionpapersId)
        {
            return _repository.Project<UploadQuestionpapers, UploadQuestionpapersProjection>(
                    uploadQuestionpapers => (from Questionpapers in uploadQuestionpapers
                                             where Questionpapers.UploadQuestionpapersId == uploadQuestionpapersId
                                             select new UploadQuestionpapersProjection
                                    {
                                                 UploadQuestionpapersId = Questionpapers.UploadQuestionpapersId,
                                        BoardName = Questionpapers.BoardName,
                                        ClassName = Questionpapers.ClassName,
                                        SubjectName = Questionpapers.SubjectName,
                                        FileName = Questionpapers.FileName,
                                        LogoName = Questionpapers.LogoName,
                                        UploadDate = Questionpapers.UploadDate,
                                        Title = Questionpapers.Title,
                                        IsVisible = Questionpapers.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadQuestionpapers uploadNewQuestionpapers)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadQuestionpapers, bool>(uploadQuestionpapers => (
                                from p in uploadQuestionpapers
                                where p.FileName == uploadNewQuestionpapers.FileName && p.UploadQuestionpapersId != uploadNewQuestionpapers.UploadQuestionpapersId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Questionpapers file '{0}' already exists!", uploadNewQuestionpapers.FileName) });
            }
            else
            {
                var Questionpapers = _repository.Load<UploadQuestionpapers>(x => x.UploadQuestionpapersId == uploadNewQuestionpapers.UploadQuestionpapersId);
                Questionpapers.ClassName = uploadNewQuestionpapers.ClassName;
                Questionpapers.Title = uploadNewQuestionpapers.Title;
                Questionpapers.FileName = uploadNewQuestionpapers.FileName;
                Questionpapers.LogoName = uploadNewQuestionpapers.LogoName;
                Questionpapers.BoardName = uploadNewQuestionpapers.BoardName;
                Questionpapers.SubjectName = uploadNewQuestionpapers.SubjectName;
                Questionpapers.UploadDate = uploadNewQuestionpapers.UploadDate;
                Questionpapers.IsVisible = uploadNewQuestionpapers.IsVisible;
                _repository.Update(Questionpapers);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Questionpapers updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadQuestionpapersId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadQuestionpapers>(b => b.UploadQuestionpapersId == uploadQuestionpapersId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Questionpapers not exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Questionpapers deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadQuestionpapersProjection> GetUploadQuestionpapersList()
        {
            return _repository.Project<UploadQuestionpapers, UploadQuestionpapersProjection[]>(
                UploadQuestionpapers => (from Questionpapers in UploadQuestionpapers
                                where Questionpapers.IsVisible == true
                                orderby Questionpapers.CreatedOn descending
                                select new UploadQuestionpapersProjection
                                {
                                    BoardName = Questionpapers.BoardName,
                                    ClassName = Questionpapers.ClassName,
                                    SubjectName = Questionpapers.SubjectName,
                                    Title = Questionpapers.Title,
                                    UploadDate = Questionpapers.UploadDate,
                                    FileName = Questionpapers.FileName,
                                    LogoName = Questionpapers.LogoName,
                                    IsVisible = Questionpapers.IsVisible,
                                    UploadQuestionpapersId = Questionpapers.UploadQuestionpapersId,
                                    ClassId = Questionpapers.ClassId,
                                    BoardId = Questionpapers.BoardId,
                                    SubjectId = Questionpapers.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadQuestionpapersProjection> GetUploadQuestionpapersListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadQuestionpapers, UploadQuestionpapersProjection[]>(
                UploadQuestionpapers => (from Questionpapers in UploadQuestionpapers
                                         where Questionpapers.IsVisible == true && Questionpapers.SubjectId == subjectId
                                orderby Questionpapers.CreatedOn descending
                                select new UploadQuestionpapersProjection
                                {
                                    Title = Questionpapers.Title,
                                    UploadQuestionpapersId = Questionpapers.UploadQuestionpapersId
                                }).ToArray());
        }

    }
}
