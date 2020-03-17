using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IUploadNotesService
    {
        CMSResult Save(int ClientId,UploadNotes newUploadNotes);
        IEnumerable<UploadNotesGridModel> GetUploadNotesData(out int totalRecords,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        UploadNotesProjection GetNotesById(int uploadNotesId);
        CMSResult Update(UploadNotes uploadNewNotes);
        CMSResult Delete(int uploadNotesId);
        IEnumerable<UploadNotesProjection> GetUploadNotesList();

        IEnumerable<UploadNotesProjection> GetUploadNotesListByClientId(int ClientId);
        IEnumerable<UploadNotesProjection> GetUploadNotesListBySubjectId(int? subjectId);
    }
}
