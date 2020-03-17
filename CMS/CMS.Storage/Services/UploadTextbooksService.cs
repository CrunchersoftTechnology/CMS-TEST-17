using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadTextbooksService : IUploadTextbooksService
    {
        readonly IRepository _repository;

        public UploadTextbooksService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(UploadTextbooks newUploadTextbooks)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadTextbooks, bool>(uploadTextbooks => (
                                from p in uploadTextbooks
                                where p.FileName == newUploadTextbooks.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Textbooks file '{0}' already exists!", newUploadTextbooks.FileName) });
            }
            else
            {
                _repository.Add(newUploadTextbooks);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Textbooks file '{0}' added successfully!", newUploadTextbooks.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadTextbooksGridModel> GetUploadTextbooksData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadTextbooks, IQueryable<UploadTextbooksGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadTextbooksGridModel
                 {
                     UploadTextbooksId = p.UploadTextbooksId,
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
                    case nameof(UploadTextbooksGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadTextbooksGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadTextbooksGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadTextbooksGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadTextbooksGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadTextbooksGridModel.IsVisible):
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

        public UploadTextbooksProjection GetTextbooksById(int uploadTextbooksId)
        {
            return _repository.Project<UploadTextbooks, UploadTextbooksProjection>(
                    uploadTextbooks => (from Textbooks in uploadTextbooks
                                        where Textbooks.UploadTextbooksId == uploadTextbooksId
                                        select new UploadTextbooksProjection
                                        {
                                            UploadTextbooksId = Textbooks.UploadTextbooksId,
                                        BoardName = Textbooks.BoardName,
                                        ClassName = Textbooks.ClassName,
                                        SubjectName = Textbooks.SubjectName,
                                        FileName = Textbooks.FileName,
                                        LogoName = Textbooks.LogoName,
                                        UploadDate = Textbooks.UploadDate,
                                        Title = Textbooks.Title,
                                        IsVisible = Textbooks.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadTextbooks uploadNewTextbooks)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadTextbooks, bool>(uploadTextbooks => (
                                from p in uploadTextbooks
                                where p.FileName == uploadNewTextbooks.FileName && p.UploadTextbooksId != uploadNewTextbooks.UploadTextbooksId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Textbooks file '{0}' already exists!", uploadNewTextbooks.FileName) });
            }
            else
            {
                var Textbooks = _repository.Load<UploadTextbooks>(x => x.UploadTextbooksId == uploadNewTextbooks.UploadTextbooksId);
                Textbooks.ClassName = uploadNewTextbooks.ClassName;
                Textbooks.Title = uploadNewTextbooks.Title;
                Textbooks.FileName = uploadNewTextbooks.FileName;
                Textbooks.LogoName = uploadNewTextbooks.LogoName;
                Textbooks.BoardName = uploadNewTextbooks.BoardName;
                Textbooks.SubjectName = uploadNewTextbooks.SubjectName;
                Textbooks.UploadDate = uploadNewTextbooks.UploadDate;
                Textbooks.IsVisible = uploadNewTextbooks.IsVisible;
                _repository.Update(Textbooks);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Textbooks updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadTextbooksId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadTextbooks>(b => b.UploadTextbooksId == uploadTextbooksId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Textbooks e exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Textbooks deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadTextbooksProjection> GetUploadTextbooksList()
        {
            return _repository.Project<UploadTextbooks, UploadTextbooksProjection[]>(
                UploadTextbooks => (from Textbooks in UploadTextbooks
                                    where Textbooks.IsVisible == true
                                orderby Textbooks.CreatedOn descending
                                select new UploadTextbooksProjection
                                {
                                    BoardName = Textbooks.BoardName,
                                    ClassName = Textbooks.ClassName,
                                    SubjectName = Textbooks.SubjectName,
                                    Title = Textbooks.Title,
                                    UploadDate = Textbooks.UploadDate,
                                    FileName = Textbooks.FileName,
                                    LogoName = Textbooks.LogoName,
                                    IsVisible = Textbooks.IsVisible,
                                    UploadTextbooksId = Textbooks.UploadTextbooksId,
                                    ClassId = Textbooks.ClassId,
                                    BoardId = Textbooks.BoardId,
                                    SubjectId = Textbooks.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadTextbooksProjection> GetUploadTextbooksListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadTextbooks, UploadTextbooksProjection[]>(
                UploadTextbooks => (from Textbooks in UploadTextbooks
                                where Textbooks.IsVisible == true && Textbooks.SubjectId == subjectId
                                orderby Textbooks.CreatedOn descending
                                select new UploadTextbooksProjection
                                {
                                    Title = Textbooks.Title,
                                    UploadTextbooksId = Textbooks.UploadTextbooksId
                                }).ToArray());
        }

    }
}
