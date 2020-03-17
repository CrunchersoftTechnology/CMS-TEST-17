using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IInstallmentService
    {
        IEnumerable<InstallmentProjection> GetAllInstallments();
        InstallmentProjection GetInstallmentById(int installmentId);
        CMSResult Save(Installment installment);
        IEnumerable<InstallmentProjection> GetStudInstallments(string userId);
        IEnumerable<InstallmentProjection> GetInstallments(int classId, string userId);
        object[] GetStudentsAutoComplete(string query, int classId, int branchId);
        decimal GetCountInstallment(string userId);
        IEnumerable<InstallmentProjection> GetInstallmentsByBranchId(int branchId);

        IEnumerable<InstallmentProjection> GetInstallmentsByClientId(int clientId);
        int GetInstallmentCount(string userId);
        IEnumerable<InstallmentGridModel> GetInstallmentData(out int totalRecords,int filterClassName, string filterUseId, int userId,
         int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        InstallmentProjection GetInstallmentByInstallmentId(int installmentId);
        CMSResult Update(Installment oldInstallment);
    }
}
