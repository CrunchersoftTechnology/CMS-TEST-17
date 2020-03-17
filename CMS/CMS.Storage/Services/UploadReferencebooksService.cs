using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadReferencebooksService : IUploadReferencebooksService
    {
        readonly IRepository _repository;

        public UploadReferencebooksService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(UploadReferencebooks newUploadReferencebooks)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadReferencebooks, bool>(uploadReferencebooks => (
                                from p in uploadReferencebooks
                                where p.FileName == newUploadReferencebooks.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Referencebooks file '{0}' already exists!", newUploadReferencebooks.FileName) });
            }
            else
            {
                _repository.Add(newUploadReferencebooks);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Referencebooks file '{0}' added successfully!", newUploadReferencebooks.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadReferencebooksGridModel> GetUploadReferencebooksData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadReferencebooks, IQueryable<UploadReferencebooksGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadReferencebooksGridModel
                 {
                     UploadReferencebooksId = p.UploadReferencebooksId,
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
                    case nameof(UploadReferencebooksGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadReferencebooksGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadReferencebooksGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadReferencebooksGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadReferencebooksGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadReferencebooksGridModel.IsVisible):
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

        public UploadReferencebooksProjection GetReferencebooksById(int uploadReferencebooksId)
        {
            return _repository.Project<UploadReferencebooks, UploadReferencebooksProjection>(
                    uploadReferencebooks => (from Referencebooks in uploadReferencebooks
                                    where Referencebooks.UploadReferencebooksId == uploadReferencebooksId
                                             select new UploadReferencebooksProjection
                                             {
                                                 UploadReferencebooksId = Referencebooks.UploadReferencebooksId,
                                        BoardName = Referencebooks.BoardName,
                                        ClassName = Referencebooks.ClassName,
                                        SubjectName = Referencebooks.SubjectName,
                                        FileName = Referencebooks.FileName,
                                        LogoName = Referencebooks.LogoName,
                                        UploadDate = Referencebooks.UploadDate,
                                        Title = Referencebooks.Title,
                                        IsVisible = Referencebooks.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadReferencebooks uploadNewReferencebooks)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadReferencebooks, bool>(uploadReferencebooks => (
                                from p in uploadReferencebooks
                                where p.FileName == uploadNewReferencebooks.FileName && p.UploadReferencebooksId != uploadNewReferencebooks.UploadReferencebooksId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Referencebooks file '{0}' already exists!", uploadNewReferencebooks.FileName) });
            }
            else
            {
                var Referencebooks = _repository.Load<UploadReferencebooks>(x => x.UploadReferencebooksId == uploadNewReferencebooks.UploadReferencebooksId);
                Referencebooks.ClassName = uploadNewReferencebooks.ClassName;
                Referencebooks.Title = uploadNewReferencebooks.Title;
                Referencebooks.FileName = uploadNewReferencebooks.FileName;
                Referencebooks.LogoName = uploadNewReferencebooks.LogoName;
                Referencebooks.BoardName = uploadNewReferencebooks.BoardName;
                Referencebooks.SubjectName = uploadNewReferencebooks.SubjectName;
                Referencebooks.UploadDate = uploadNewReferencebooks.UploadDate;
                Referencebooks.IsVisible = uploadNewReferencebooks.IsVisible;
                _repository.Update(Referencebooks);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Referencebooks updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadReferencebooksId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadReferencebooks>(b => b.UploadReferencebooksId == uploadReferencebooksId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Referencebooks not exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Referencebooks deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadReferencebooksProjection> GetUploadReferencebooksList()
        {
            return _repository.Project<UploadReferencebooks, UploadReferencebooksProjection[]>(
                UploadReferencebooks => (from Referencebooks in UploadReferencebooks
                                         where Referencebooks.IsVisible == true
                                orderby Referencebooks.CreatedOn descending
                                select new UploadReferencebooksProjection
                                {
                                    BoardName = Referencebooks.BoardName,
                                    ClassName = Referencebooks.ClassName,
                                    SubjectName = Referencebooks.SubjectName,
                                    Title = Referencebooks.Title,
                                    UploadDate = Referencebooks.UploadDate,
                                    FileName = Referencebooks.FileName,
                                    LogoName = Referencebooks.LogoName,
                                    IsVisible = Referencebooks.IsVisible,
                                    UploadReferencebooksId = Referencebooks.UploadReferencebooksId,
                                    ClassId = Referencebooks.ClassId,
                                    BoardId = Referencebooks.BoardId,
                                    SubjectId = Referencebooks.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadReferencebooksProjection> GetUploadReferencebooksListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadReferencebooks, UploadReferencebooksProjection[]>(
                UploadReferencebooks => (from Referencebooks in UploadReferencebooks
                                where Referencebooks.IsVisible == true && Referencebooks.SubjectId == subjectId
                                orderby Referencebooks.CreatedOn descending
                                select new UploadReferencebooksProjection
                                {
                                    Title = Referencebooks.Title,
                                    UploadReferencebooksId = Referencebooks.UploadReferencebooksId
                                }).ToArray());
        }

    }
}
