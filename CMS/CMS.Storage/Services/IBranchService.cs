using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IBranchService
    {
        CMSResult Save(Branch branch);

        CMSResult SaveBranch(Branch branch);
        CMSResult Update(Branch branch);
        CMSResult Delete(int branchId);
        IEnumerable<BranchProjection> GetAllBranches();

        IEnumerable<BranchProjection> GetAllBranchesByClientId(int ClientId);

        BranchProjection GetBoardById(int id);
        int GetBranchesCount();

        int GetBranchesCountByClientId(int Client);
        IEnumerable<BranchGridModel> GetBranchDataByClientId(out int totalRecords, string BranchName,int userId,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc);

        IEnumerable<BranchGridModel> GetBranchData(out int totalRecords, string BranchName,
   int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<BranchProjection> GetBranchByMultipleBranchId(string selectedBranch);
    }
}
