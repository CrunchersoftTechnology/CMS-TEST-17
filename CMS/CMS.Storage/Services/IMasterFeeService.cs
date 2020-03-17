using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IMasterFeeService
    {
        IEnumerable<MasterFeeProjection> GetAllMasterFees();
        CMSResult Save(MasterFee masterfee);

        CMSResult Save(int ClientId,MasterFee masterfee);
        CMSResult Update(MasterFee masterfee);
        CMSResult Delete(int id);
        IEnumerable<MasterFeeProjection> GetMasterFees(int subjectId, int classId);
        MasterFeeProjection GetMasterFeeById(int masterfeeId);
        IEnumerable<MasterFeeGridModel> GetMasterFeeData(out int totalRecords, int filterClassName, int filterSubjectName,
        int? limitOffset,
            int? limitRowCount, string orderBy, bool desc);

        IEnumerable<MasterFeeGridModel> GetMasterFeeDataByClientId(out int totalRecords, int filterClassName, int filterSubjectName,int ClientId,
      int? limitOffset,
          int? limitRowCount, string orderBy, bool desc);
    }
}
