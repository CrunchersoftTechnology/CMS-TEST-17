using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class UploadInbuiltquestionbankService : IUploadInbuiltquestionbankService
    {
        readonly IRepository _repository;

        public UploadInbuiltquestionbankService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(UploadInbuiltquestionbank newUploadInbuiltquestionbank)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadInbuiltquestionbank, bool>(uploadInbuiltquestionbank => (
                                from p in uploadInbuiltquestionbank
                                where p.FileName == newUploadInbuiltquestionbank.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Inbuiltquestionbank file '{0}' already exists!", newUploadInbuiltquestionbank.FileName) });
            }
            else
            {
                _repository.Add(newUploadInbuiltquestionbank);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Inbuiltquestionbank file '{0}' added successfully!", newUploadInbuiltquestionbank.FileName) });
            }
            return result;
        }

        public IEnumerable<UploadInbuiltquestionbankGridModel> GetUploadInbuiltquestionbankData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<UploadInbuiltquestionbank, IQueryable<UploadInbuiltquestionbankGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new UploadInbuiltquestionbankGridModel
                 {
                     UploadInbuiltquestionbankId = p.UploadInbuiltquestionbankId,
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
                    case nameof(UploadInbuiltquestionbankGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(UploadInbuiltquestionbankGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(UploadInbuiltquestionbankGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(UploadInbuiltquestionbankGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(UploadInbuiltquestionbankGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(UploadInbuiltquestionbankGridModel.IsVisible):
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

        public UploadInbuiltquestionbankProjection GetInbuiltquestionbankById(int uploadInbuiltquestionbankId)
        {
            return _repository.Project<UploadInbuiltquestionbank, UploadInbuiltquestionbankProjection>(
                    uploadInbuiltquestionbank => (from Inbuiltquestionbank in uploadInbuiltquestionbank
                                    where Inbuiltquestionbank.UploadInbuiltquestionbankId == uploadInbuiltquestionbankId
                                    select new UploadInbuiltquestionbankProjection
                                    {
                                        UploadInbuiltquestionbankId = Inbuiltquestionbank.UploadInbuiltquestionbankId,
                                        BoardName = Inbuiltquestionbank.BoardName,
                                        ClassName = Inbuiltquestionbank.ClassName,
                                        SubjectName = Inbuiltquestionbank.SubjectName,
                                        FileName = Inbuiltquestionbank.FileName,
                                        LogoName = Inbuiltquestionbank.LogoName,
                                        UploadDate = Inbuiltquestionbank.UploadDate,
                                        Title = Inbuiltquestionbank.Title,
                                        IsVisible = Inbuiltquestionbank.IsVisible
                                    }).FirstOrDefault());
        }

        public CMSResult Update(UploadInbuiltquestionbank uploadNewInbuiltquestionbank)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<UploadInbuiltquestionbank, bool>(uploadInbuiltquestionbank => (
                                from p in uploadInbuiltquestionbank
                                where p.FileName == uploadNewInbuiltquestionbank.FileName && p.UploadInbuiltquestionbankId != uploadNewInbuiltquestionbank.UploadInbuiltquestionbankId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Inbuiltquestionbank file '{0}' already exists!", uploadNewInbuiltquestionbank.FileName) });
            }
            else
            {
                var Inbuiltquestionbank = _repository.Load<UploadInbuiltquestionbank>(x => x.UploadInbuiltquestionbankId == uploadNewInbuiltquestionbank.UploadInbuiltquestionbankId);
                Inbuiltquestionbank.ClassName = uploadNewInbuiltquestionbank.ClassName;
                Inbuiltquestionbank.Title = uploadNewInbuiltquestionbank.Title;
                Inbuiltquestionbank.FileName = uploadNewInbuiltquestionbank.FileName;
                Inbuiltquestionbank.LogoName = uploadNewInbuiltquestionbank.LogoName;
                Inbuiltquestionbank.BoardName = uploadNewInbuiltquestionbank.BoardName;
                Inbuiltquestionbank.SubjectName = uploadNewInbuiltquestionbank.SubjectName;
                Inbuiltquestionbank.UploadDate = uploadNewInbuiltquestionbank.UploadDate;
                Inbuiltquestionbank.IsVisible = uploadNewInbuiltquestionbank.IsVisible;
                _repository.Update(Inbuiltquestionbank);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Inbuiltquestionbank updated successfully!") });
            }
            return result;
        }

        public CMSResult Delete(int uploadInbuiltquestionbankId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<UploadInbuiltquestionbank>(b => b.UploadInbuiltquestionbankId == uploadInbuiltquestionbankId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Inbuiltquestionbank not exists!") });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Inbuiltquestionbank deleted successfully!") });
            }
            return result;
        }

        public IEnumerable<UploadInbuiltquestionbankProjection> GetUploadInbuiltquestionbankList()
        {
            return _repository.Project<UploadInbuiltquestionbank, UploadInbuiltquestionbankProjection[]>(
                UploadInbuiltquestionbank => (from Inbuiltquestionbank in UploadInbuiltquestionbank
                                where Inbuiltquestionbank.IsVisible == true
                                orderby Inbuiltquestionbank.CreatedOn descending
                                select new UploadInbuiltquestionbankProjection
                                {
                                    BoardName = Inbuiltquestionbank.BoardName,
                                    ClassName = Inbuiltquestionbank.ClassName,
                                    SubjectName = Inbuiltquestionbank.SubjectName,
                                    Title = Inbuiltquestionbank.Title,
                                    UploadDate = Inbuiltquestionbank.UploadDate,
                                    FileName = Inbuiltquestionbank.FileName,
                                    LogoName = Inbuiltquestionbank.LogoName,
                                    IsVisible = Inbuiltquestionbank.IsVisible,
                                    UploadInbuiltquestionbankId = Inbuiltquestionbank.UploadInbuiltquestionbankId,
                                    ClassId = Inbuiltquestionbank.ClassId,
                                    BoardId = Inbuiltquestionbank.BoardId,
                                    SubjectId = Inbuiltquestionbank.SubjectId
                                }).ToArray());
        }

        public IEnumerable<UploadInbuiltquestionbankProjection> GetUploadInbuiltquestionbankListBySubjectId(int? subjectId)
        {
            return _repository.Project<UploadInbuiltquestionbank, UploadInbuiltquestionbankProjection[]>(
                UploadInbuiltquestionbank => (from Inbuiltquestionbank in UploadInbuiltquestionbank
                                              where Inbuiltquestionbank.IsVisible == true && Inbuiltquestionbank.SubjectId == subjectId
                                orderby Inbuiltquestionbank.CreatedOn descending
                                select new UploadInbuiltquestionbankProjection
                                {
                                    Title = Inbuiltquestionbank.Title,
                                    UploadInbuiltquestionbankId = Inbuiltquestionbank.UploadInbuiltquestionbankId
                                }).ToArray());
        }

    }
}
