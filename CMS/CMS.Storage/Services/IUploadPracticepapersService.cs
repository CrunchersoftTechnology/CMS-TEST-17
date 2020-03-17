using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadPracticepapersService
    {
        CMSResult Save(UploadPracticepapers newUploadPracticepapers);
        IEnumerable<UploadPracticepapersGridModel> GetUploadPracticepapersData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadPracticepapersProjection GetPracticepapersById(int uploadPracticepapersId);
        CMSResult Update(UploadPracticepapers uploadNewPracticepapers);
        CMSResult Delete(int uploadPracticepapersId);
        IEnumerable<UploadPracticepapersProjection> GetUploadPracticepapersList();
        IEnumerable<UploadPracticepapersProjection> GetUploadPracticepapersListBySubjectId(int? subjectId);
    }
}
