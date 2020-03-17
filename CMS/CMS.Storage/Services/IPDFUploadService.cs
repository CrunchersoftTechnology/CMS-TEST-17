using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IPDFUploadService
    {
        CMSResult Save(PDFUpload newPdfUpload);
        CMSResult Update(PDFUpload pdfUpload);
        CMSResult Delete(int id);
        IEnumerable<PDFUploadProjection> GetPDFUploadFiles();
        PDFUploadProjection GetPdfFileById(int pdfUploadId);
        string GetPDFFileName(int id);
        IEnumerable<PDFUploadGridModel> GetPDFUploadData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }
}
