using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadTextbooksService
    {
        CMSResult Save(UploadTextbooks newUploadTextbooks);
        IEnumerable<UploadTextbooksGridModel> GetUploadTextbooksData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadTextbooksProjection GetTextbooksById(int uploadTextbooksId);
        CMSResult Update(UploadTextbooks uploadNewTextbooks);
        CMSResult Delete(int uploadTextbooksId);
        IEnumerable<UploadTextbooksProjection> GetUploadTextbooksList();
        IEnumerable<UploadTextbooksProjection> GetUploadTextbooksListBySubjectId(int? subjectId);
    }
}
