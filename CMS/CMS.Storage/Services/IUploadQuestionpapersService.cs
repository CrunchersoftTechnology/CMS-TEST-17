using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadQuestionpapersService
    {
        CMSResult Save(UploadQuestionpapers newUploadQuestionpapers);
        IEnumerable<UploadQuestionpapersGridModel> GetUploadQuestionpapersData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadQuestionpapersProjection GetQuestionpapersById(int uploadQuestionpapersId);
        CMSResult Update(UploadQuestionpapers uploadNewQuestionpapers);
        CMSResult Delete(int uploadQuestionpapersId);
        IEnumerable<UploadQuestionpapersProjection> GetUploadQuestionpapersList();
        IEnumerable<UploadQuestionpapersProjection> GetUploadQuestionpapersListBySubjectId(int? subjectId);
    }
}
