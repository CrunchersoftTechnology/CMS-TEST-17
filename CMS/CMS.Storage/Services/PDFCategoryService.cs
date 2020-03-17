using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CMS.Domain.Storage.Services
{
    public class PDFCategoryService : IPDFCategoryService
    {
        readonly IRepository _repository;

        public PDFCategoryService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(int ClientId, PDFCategory newPDFCategory)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<PDFCategory, bool>(pdfcategories => (
                                from b in pdfcategories
                                where b.Name == newPDFCategory.Name &&
                                b.ClientId==ClientId
                                select b
                            ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("PDF Category '{0}' already exists!", newPDFCategory.Name) });
            }
            else
            {
                newPDFCategory.ClientId = ClientId;
                _repository.Add(newPDFCategory);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("PDF Category '{0}' added successfully!", newPDFCategory.Name) });
            }
            return result;
        }
        public IEnumerable<PDFCategoryProjection> GetPDFCategories()
        {
            return _repository.Project<PDFCategory, PDFCategoryProjection[]>(
                 pdfc => (from p in pdfc
                          select new PDFCategoryProjection
                          {
                              PDFCategoryId = p.PDFCategoryId,
                              Name = p.Name
                          }).ToArray());

        }

        public IEnumerable<PDFCategoryProjection> GetPDFCategoriesByClientId(int ClientId)
        {
            return _repository.Project<PDFCategory, PDFCategoryProjection[]>(
                 pdfc => (from p in pdfc
                          where p.ClientId==ClientId
                          select new PDFCategoryProjection
                          {

                              PDFCategoryId = p.PDFCategoryId,
                              Name = p.Name
                          }).ToArray());

        }

        public PDFCategoryProjection GetPDFCategoryById(int pdfCategoryId)
        {
            return _repository.Project<PDFCategory, PDFCategoryProjection>(
                pdfcs => (from p in pdfcs
                          where p.PDFCategoryId == pdfCategoryId
                          select new PDFCategoryProjection
                          {
                              PDFCategoryId = p.PDFCategoryId,
                              Name = p.Name
                          }).FirstOrDefault());
        }

        public CMSResult Update(PDFCategory oldPDFCategory)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<PDFCategory, bool>(pdfcrs => (from b in pdfcrs where b.PDFCategoryId != oldPDFCategory.PDFCategoryId && b.Name == oldPDFCategory.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Board '{0}' already exists!", oldPDFCategory.Name) });
            }
            else
            {
                var pdfcs = _repository.Load<PDFCategory>(b => b.PDFCategoryId == oldPDFCategory.PDFCategoryId);
                pdfcs.Name = oldPDFCategory.Name;
                _repository.Update(pdfcs);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("PDF Category '{0}' updated successfully!", oldPDFCategory.Name) });
            }
            return result;
        }

        public CMSResult Delete(int pdfCategoryId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<PDFCategory>(p => p.PDFCategoryId == pdfCategoryId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("PDF Category '{0}' already exists!", model.Name) });
            }
            else
            {
                var isExistsPDFUpload = _repository.Project<PDFUpload, bool>(pdfUploads => (
                                            from s in pdfUploads
                                            where s.PDFCategoryId == pdfCategoryId
                                            select s)
                                            .Any());
                if (isExistsPDFUpload)
                {
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete PDF Category '{0}'. Because it belongs to PDF Upload!", model.Name) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("PDF Category '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public IEnumerable<PDFCategoryGridModel> GetPDFCategoryData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc, int ClientId)
        {

            var query = _repository.Project<PDFCategory, IQueryable<PDFCategoryGridModel>>(pdfCategories => (
                 from p in pdfCategories
                 select new PDFCategoryGridModel
                 {
                     ClientId=p.ClientId,
                     PDFCategoryId = p.PDFCategoryId,
                     Name = p.Name,
                     CreatedOn = p.CreatedOn,
                 })).AsQueryable();

            totalRecords = query.Count();
            if (ClientId != 0)
            {
                query = query.Where(p => p.ClientId == ClientId);
            }
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(PDFCategoryGridModel.Name):
                        if (!desc)
                            query = query.OrderBy(p => p.Name);
                        else
                            query = query.OrderByDescending(p => p.Name);
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

       /* CMSResult IPDFCategoryService.Save(PDFCategory newPDFCategory)
        {
            throw new NotImplementedException();
        }

        CMSResult IPDFCategoryService.Update(PDFCategory oldPDFCategory)
        {
            throw new NotImplementedException();
        }

        CMSResult IPDFCategoryService.Delete(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<PDFCategoryProjection> IPDFCategoryService.GetPDFCategories()
        {
            throw new NotImplementedException();
        }

        PDFCategoryProjection IPDFCategoryService.GetPDFCategoryById(int pdfCategoryId)
        {
            throw new NotImplementedException();
        }

        IEnumerable<PDFCategoryGridModel> IPDFCategoryService.GetPDFCategoryData(out int totalRecords, int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            throw new NotImplementedException();
        }*/

        object IPDFCategoryService.GetPDFCategoryById(List<int> pdfIds)
        {
            throw new NotImplementedException();
        }
    }
}
