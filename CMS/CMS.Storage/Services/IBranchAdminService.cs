using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IBranchAdminService
    {
        CMSResult Save(BranchAdmin branchAdmin);
        BranchAdminProjection GetBranchAdminById(string branchAdminId);

      
        CMSResult Update(BranchAdmin branchAdmin);
        IEnumerable<BranchAdminProjection> GetBranches();
        CMSResult Delete(string userId);
        BranchAdminProjection GetBranchesById(string branchAdminId);
        IEnumerable<BranchAdminProjection> GetBranchAdminContactList();
        IEnumerable<BranchAdminGridModel> GetBranchAdminData(out int totalRecords, string Name,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<BranchAdminProjection> GetBranchAdminContactListBrbranchId(int branchId);

        IEnumerable<BranchAdminProjection> GetClientAdminContactListBrbranchId(int clientId);
    }
}
