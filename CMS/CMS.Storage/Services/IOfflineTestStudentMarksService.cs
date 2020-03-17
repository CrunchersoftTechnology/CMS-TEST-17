using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IOfflineTestStudentMarksService
    {
        IEnumerable<StudentProjection> GetStudentsAutoComplete(string query, int classId, string SelectedBranches);
        CMSResult Save(OfflineTestStudentMarks newOfflineTestStudentMarks);
        IEnumerable<OfflineTestStudentMarksGridModel> GetOfflineNotificationData(out int totalRecords, int userId,
       int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        OfflineTestStudentMarksProjection GetOfflineTestMarksById(int id);
        CMSResult Delete(int offlineTestStudentMarksId);
        CMSResult Update(OfflineTestStudentMarks OfflineTestPaper);
        IEnumerable<OfflineTestPaperProjection> GetOfflineTest();
        IEnumerable<UploadOfflineMarksProjection> GetOfflineTestByOfflineTestPaperId(int offlineTestPaperId);
    }
}
