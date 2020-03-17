using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadAssignmentsService
    {
        CMSResult Save(UploadAssignments newUploadAssignments);
        IEnumerable<UploadAssignmentsGridModel> GetUploadAssignmentsData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadAssignmentsProjection GetAssignmentsById(int uploadAssignmentsId);
        CMSResult Update(UploadAssignments uploadNewAssignments);
        CMSResult Delete(int uploadAssignmentsId);
        IEnumerable<UploadAssignmentsProjection> GetUploadAssignmentsList();
        IEnumerable<UploadAssignmentsProjection> GetUploadAssignmentsListBySubjectId(int? subjectId);
    }
}
