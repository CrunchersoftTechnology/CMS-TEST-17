using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class PDFUploadService : IPDFUploadService
    {
        readonly IRepository _repository;
        public PDFUploadService(IRepository repository)
        {
            _repository = repository;
        }
        public CMSResult Save(PDFUpload newPdfUpload)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<PDFUpload, bool>(pdfs => (
                                from p in pdfs
                                where p.FileName == newPdfUpload.FileName
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("PDF file '{0}' already exists!", newPdfUpload.FileName) });
            }
            else
            {
                _repository.Add(newPdfUpload);
                _repository.CommitChanges();
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("PDF file '{0}' added successfully!", newPdfUpload.FileName) });
            }
            return result;
        }

        public CMSResult Update(PDFUpload pdfUpload)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<PDFUpload, bool>(pdfs => (
                                from p in pdfs
                                where p.FileName == pdfUpload.FileName && p.PDFUploadId != pdfUpload.PDFUploadId
                                select p
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("PDF file '{0}' already exists!", pdfUpload.FileName) });
            }
            else
            {
                var pdfs = _repository.Load<PDFUpload>(x => x.PDFUploadId == pdfUpload.PDFUploadId);
                pdfs.ClassId = pdfUpload.ClassId;
                pdfs.Title = pdfUpload.Title;
                pdfs.FileName = pdfUpload.FileName;
                pdfs.IsVisible = pdfUpload.IsVisible;
                pdfs.PDFCategoryId = pdfUpload.PDFCategoryId;
                if (!pdfs.IsSend)
                    pdfs.IsSend = pdfUpload.IsSend;
                _repository.Update(pdfs);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("PDF '{0}' updated successfully!", pdfUpload.FileName) });
            }
            return result;
        }

        public CMSResult Delete(int pdfuploadId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<PDFUpload>(b => b.PDFUploadId == pdfuploadId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("PDF '{0}' already exists!", model.FileName) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("PDF '{0}' deleted successfully!", model.FileName) });
            }
            return result;
        }

        public IEnumerable<PDFUploadProjection> GetPDFUploadFiles()
        {
            return _repository.Project<PDFUpload, PDFUploadProjection[]>(
                 pdfUploads => (from p in pdfUploads
                                orderby p.CreatedOn descending
                                select new PDFUploadProjection
                                {
                                    PDFUploadId = p.PDFUploadId,
                                    ClassId = p.ClassId,
                                    Title = p.Title,
                                    FileName = p.FileName,
                                    IsVisible = p.IsVisible,
                                    ClassName = p.Class.Name,
                                    PDFCategoryName = p.PDFCategory.Name,
                                    CreatedOn = p.CreatedOn,
                                    IsSend = p.IsSend
                                }).ToArray());

        }

        public PDFUploadProjection GetPdfFileById(int pdfUploadId)
        {
            return _repository.Project<PDFUpload, PDFUploadProjection>(
                pdffiles => (from c in pdffiles
                             where c.PDFUploadId == pdfUploadId
                             select new PDFUploadProjection
                             {
                                 PDFUploadId = c.PDFUploadId,
                                 Title = c.Title,
                                 FileName = c.FileName,
                                 ClassName = c.Class.Name,
                                 IsVisible = c.IsVisible,
                                 ClassId = c.ClassId,
                                 PDFCategoryId = c.PDFCategoryId,
                                 PDFCategoryName = c.PDFCategory.Name,
                                 IsSend = c.IsSend
                             }).FirstOrDefault());
        }

        public string GetPDFFileName(int id)
        {
            return _repository.Project<PDFUpload, string>(pdfs => (
                                from p in pdfs
                                where p.PDFUploadId == id
                                select p.FileName
                            ).FirstOrDefault());
        }

        public IEnumerable<PDFUploadGridModel> GetPDFUploadData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<PDFUpload, IQueryable<PDFUploadGridModel>>(pdfUploads => (
                 from p in pdfUploads
                 select new PDFUploadGridModel
                 {
                     PDFCategoryId = p.PDFCategoryId,
                     Title = p.Title,
                     ClassId = p.ClassId,
                     ClassName = p.Class.Name,
                     CreatedOn = p.CreatedOn,
                     FileName = p.FileName,
                     IsVisible = p.IsVisible,
                     PDFCategoryName = p.PDFCategory.Name,
                     PDFUploadId = p.PDFUploadId,
                     IsSend = p.IsSend
                 })).AsQueryable();

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(PDFUploadGridModel.Title):
                        if (!desc)
                            query = query.OrderBy(p => p.Title);
                        else
                            query = query.OrderByDescending(p => p.Title);
                        break;
                    case nameof(PDFUploadGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(PDFUploadGridModel.PDFCategoryName):
                        if (!desc)
                            query = query.OrderBy(p => p.PDFCategoryName);
                        else
                            query = query.OrderByDescending(p => p.PDFCategoryName);
                        break;
                    case nameof(PDFUploadGridModel.FileName):
                        if (!desc)
                            query = query.OrderBy(p => p.FileName);
                        else
                            query = query.OrderByDescending(p => p.FileName);
                        break;
                    case nameof(PDFUploadGridModel.IsVisible):
                        if (!desc)
                            query = query.OrderBy(p => p.IsVisible);
                        else
                            query = query.OrderByDescending(p => p.IsVisible);
                        break;
                    case nameof(PDFUploadGridModel.CreatedOn):
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                    case nameof(PDFUploadGridModel.IsSend):
                        if (!desc)
                            query = query.OrderBy(p => p.IsSend);
                        else
                            query = query.OrderByDescending(p => p.IsSend);
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

