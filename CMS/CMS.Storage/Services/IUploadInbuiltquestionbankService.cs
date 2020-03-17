using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadInbuiltquestionbankService
    {
        CMSResult Save(UploadInbuiltquestionbank newUploadInbuiltquestionbank);
        IEnumerable<UploadInbuiltquestionbankGridModel> GetUploadInbuiltquestionbankData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadInbuiltquestionbankProjection GetInbuiltquestionbankById(int uploadInbuiltquestionbankId);
        CMSResult Update(UploadInbuiltquestionbank uploadNewInbuiltquestionbank);
        CMSResult Delete(int uploadInbuiltquestionbankId);
        IEnumerable<UploadInbuiltquestionbankProjection> GetUploadInbuiltquestionbankList();
        IEnumerable<UploadInbuiltquestionbankProjection> GetUploadInbuiltquestionbankListBySubjectId(int? subjectId);
    }
}
