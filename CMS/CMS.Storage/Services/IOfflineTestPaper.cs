using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IOfflineTestPaper
    {
        CMSResult Save(OfflineTestPaper offlineTestPaper);
        IEnumerable<OfflineTestPaperGridModel> GetOfflineNotificationData(out int totalRecords, int userId,
          int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        CMSResult Delete(int OfflineTestPaperId);
        OfflineTestPaperProjection GetOfflineTestById(int id);
        CMSResult Update(OfflineTestPaper OfflineTestPaper);
        IEnumerable<OfflineTestPaperProjection> GetOfflineTestPaper();
    }
}
