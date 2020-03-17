using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Services
{
    public interface ISchoolService
    {
        IEnumerable<SchoolProjection> GetAllSchools();

        IEnumerable<SchoolProjection> GetAllSchoolsByClientId(int ClientId);
        SchoolProjection GetSchoolById(int schoolId);
        CMSResult Save(School newSchool);
        CMSResult Update(School oldSchool);
        CMSResult Delete(int id);
        IEnumerable<SchoolGridModel> GetSchoolData(out int totalRecords, string Name,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc);

        IEnumerable<SchoolGridModel> GetSchoolDataByClientId(out int totalRecords, string Name,int ClientId,
       int? limitOffset, int? limitRowCount, string orderBy, bool desc);
    }
}
