using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IClassService
    {
        IEnumerable<ClassProjection> GetClasses();
        IEnumerable<ClassProjection> GetClassesByClientId(int ClientId);
        IEnumerable<ClassProjection> GetAllClassByPrefix(string prefix);
        CMSResult Save(Class newClass);
        CMSResult Update(Class oldClass);
        CMSResult Delete(int id);
        ClassProjection GetClassById(int classId);
        IEnumerable<ClassGridModel> GetClassData(out int totalRecords, string Name,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc);

        IEnumerable<ClassGridModel> GetClassDataByClientId(out int totalRecords, string Name, int userId,
    int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }
}