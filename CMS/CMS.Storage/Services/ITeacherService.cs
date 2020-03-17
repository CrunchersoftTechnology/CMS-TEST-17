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
    public interface ITeacherService
    {
        bool Save(Teacher teacher);
        CMSResult Update(Teacher user);
        IEnumerable<TeacherProjection> GetTeachers();
        TeacherProjection GetTeacherById(string teacherId);
        IEnumerable<TeacherProjection> GetTeachersBind();
        IEnumerable<TeacherProjection> GetTeachers(int branchId);
        IEnumerable<TeacherProjection> GetAllTeachersByPrefix(string prefix);
        IEnumerable<TeacherProjection> GetTeachersByClientId(int clientId);
        IEnumerable<TeacherProjection> GetTeacherContactList();
        int GetTeachersCount();

        int GetTeachersCountByClientId(int ClientId);
        IEnumerable<TeacherGridModel> GetTeacherData(out int totalRecords, string filterFirstName, 
            string filterLastName, int userId, int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<TeacherProjection> GetTeacherContactListBrbranchId(int branchId);

        IEnumerable<TeacherProjection> GetTeacherContactListBrclientId(int clientId);
        IEnumerable<TeacherProjection> GetTeachersForWebSite();
    }
}
