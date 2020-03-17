using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadReferencebooksService
    {
        CMSResult Save(UploadReferencebooks newUploadReferencebooks);
        IEnumerable<UploadReferencebooksGridModel> GetUploadReferencebooksData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadReferencebooksProjection GetReferencebooksById(int uploadReferencebooksId);
        CMSResult Update(UploadReferencebooks uploadNewReferencebooks);
        CMSResult Delete(int uploadReferencebooksId);
        IEnumerable<UploadReferencebooksProjection> GetUploadReferencebooksList();
        IEnumerable<UploadReferencebooksProjection> GetUploadReferencebooksListBySubjectId(int? subjectId);
    }
}
