using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IArrangeTestResultService
    {
        CMSResult Save(ArrangeTestResult newArrangeTestResult);
        IEnumerable<ArrangeTestResultProjection> GetAllArrangeTestResult();
        IEnumerable<ArrangeTestResultGridModel> GetArrangeTestPaperResultData(out int totalRecords, int userId, int filterClassId, int filterTestPaperId,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        ArrangeTestResultProjection GetArrangeTestResultById(int arrangeTestResultId);
    }
}
