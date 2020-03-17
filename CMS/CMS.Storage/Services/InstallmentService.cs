using System.Collections.Generic;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Infrastructure;
using System.Linq;
using CMS.Common;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class InstallmentService : IInstallmentService
    {
        readonly IRepository _repository;

        public InstallmentService(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<InstallmentProjection> GetAllInstallments()
        {
            return _repository.Project<Installment, InstallmentProjection[]>(
                installments => (from i in installments
                                 orderby i.CreatedOn descending
                                 select new InstallmentProjection
                                 {
                                     FirstName = i.Student.Student.FirstName,
                                     LastName = i.Student.Student.LastName,
                                     MiddleName = i.Student.Student.MiddleName == null ? "" : i.Student.Student.MiddleName,
                                     ClassName = i.Class.Name,
                                     Payment = i.Payment,
                                     CreatedOn = i.CreatedOn,
                                     //FinalFee = i.Student.Student.FinalFees,
                                     RemainingFee = i.RemainingFee,
                                     ReceiptBookNumber = i.ReceiptBookNumber,
                                     ReceiptNumber = i.ReceiptNumber
                                 }).ToArray());
        }

        public CMSResult Save(Installment installment)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Installment, bool>(installments => (
                                            from i in installments
                                            where i.ReceiptBookNumber == installment.ReceiptBookNumber && i.ReceiptNumber == installment.ReceiptNumber
                                            select i)
                                            .Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Receipt Number already exists!") });
            }
            else
            {
                _repository.Add(installment);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Payment successfully added!") });
            }
            return result;
        }

        public InstallmentProjection GetInstallmentById(int installmentId)
        {
            return _repository.Project<Installment, InstallmentProjection>(
                installments => (from i in installments
                                 where i.InstallmentId == installmentId
                                 select new InstallmentProjection
                                 {
                                     InstallmentId = i.InstallmentId,
                                     Payment = i.Payment
                                 }).FirstOrDefault());
        }

        public IEnumerable<InstallmentProjection> GetStudInstallments(string userId)
        {
            return _repository.Project<Installment, InstallmentProjection[]>(
                installments => (from i in installments
                                 where i.UserId == userId
                                 select new InstallmentProjection
                                 {
                                     Payment = i.Payment,
                                     CreatedOn = i.CreatedOn,
                                     ReceiptBookNumber = i.ReceiptBookNumber,
                                     ReceiptNumber = i.ReceiptNumber,
                                     RemainingFee = i.RemainingFee
                                 }).ToArray());
        }

        public IEnumerable<InstallmentProjection> GetInstallments(int classId, string userId)
        {
            return _repository.Project<Installment, InstallmentProjection[]>(
                installments => (from i in installments
                                 orderby i.CreatedOn descending
                                 where i.ClassId == classId && i.UserId == userId
                                 select new InstallmentProjection
                                 {
                                     FirstName = i.Student.Student.FirstName,
                                     LastName = i.Student.Student.LastName,
                                     MiddleName = i.Student.Student.MiddleName,
                                     ClassName = i.Class.Name,
                                     Payment = i.Payment,
                                     CreatedOn = i.CreatedOn,
                                     FinalFee = i.Student.Student.FinalFees,
                                     RemainingFee = i.RemainingFee,
                                     ReceiptBookNumber = i.ReceiptBookNumber,
                                     ReceiptNumber = i.ReceiptNumber
                                 }).ToArray());
        }

        public object[] GetStudentsAutoComplete(string query, int classId, int branchId)
        {
            return _repository.Project<Student, object[]>(
                students => (from stud in students
                             where (stud.FirstName.Contains(query) ||
                             stud.MiddleName.Contains(query) ||
                             stud.LastName.Contains(query)) && stud.ClassId == classId && stud.BranchId == branchId
                             select new
                             {
                                 id = stud.UserId,
                                 name = stud.FirstName + " " + stud.MiddleName + " " + stud.LastName,
                                 batch = stud.Subjects.Select(x => x.Name),
                                 batchName = stud.Batches.Name,
                                 doj = stud.DOJ,
                                 email = stud.User.Email,
                                 studentContact = stud.StudentContact,
                                 parentContact = stud.ParentContact,
                                 parentAppPlayerId = stud.parentAppPlayerId,
                                 DOB = stud.DOB
                             }).ToArray());
        }

        public decimal GetCountInstallment(string userId)
        {
            var d = _repository.Project<Installment, decimal>(
               installments => (from i in installments
                                where i.UserId == userId
                                select (decimal?)i.Payment).Sum() ?? 0);
            return d;

        }

        public IEnumerable<InstallmentProjection> GetInstallmentsByBranchId(int branchId)
        {
            return _repository.Project<Installment, InstallmentProjection[]>(
                installments => (from i in installments
                                 where i.Student.Student.BranchId == branchId
                                 orderby i.CreatedOn descending
                                 select new InstallmentProjection
                                 {
                                     FirstName = i.Student.Student.FirstName,
                                     LastName = i.Student.Student.LastName,
                                     MiddleName = i.Student.Student.MiddleName,
                                     ClassName = i.Class.Name,
                                     Payment = i.Payment,
                                     CreatedOn = i.CreatedOn,
                                     FinalFee = i.Student.Student.FinalFees,
                                     RemainingFee = i.RemainingFee,
                                     ReceiptBookNumber = i.ReceiptBookNumber,
                                     ReceiptNumber = i.ReceiptNumber
                                 }).ToArray());
        }

        public IEnumerable<InstallmentProjection> GetInstallmentsByClientId(int clientId)
        {
            return _repository.Project<Installment, InstallmentProjection[]>(
                installments => (from i in installments
                                 where i.Student.Student.ClientId == clientId
                                 orderby i.CreatedOn descending
                                 select new InstallmentProjection
                                 {
                                     FirstName = i.Student.Student.FirstName,
                                     LastName = i.Student.Student.LastName,
                                     MiddleName = i.Student.Student.MiddleName,
                                     ClassName = i.Class.Name,
                                     Payment = i.Payment,
                                     CreatedOn = i.CreatedOn,
                                     FinalFee = i.Student.Student.FinalFees,
                                     RemainingFee = i.RemainingFee,
                                     ReceiptBookNumber = i.ReceiptBookNumber,
                                     ReceiptNumber = i.ReceiptNumber
                                 }).ToArray());
        }


        public int GetInstallmentCount(string userId)
        {
            var d = _repository.Project<Installment, int>(
               installmentsCount => (from i in installmentsCount
                                     where i.UserId == userId
                                     select i.InstallmentId
                                ).Count());
            return d;

        }

        public IEnumerable<InstallmentGridModel> GetInstallmentData(out int totalRecords, int filterClassName, string filterUseId, int userId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            var query = _repository.Project<Installment, IQueryable<InstallmentGridModel>>(Installment => (
                 from install in Installment
                 select new InstallmentGridModel
                 {
                     InstallmentId = install.InstallmentId,
                     UserId = install.UserId,
                     StudentFirstName = install.Student.Student.FirstName,
                     StudentLastName = install.Student.Student.LastName,
                     StudentMiddleName = install.Student.Student.MiddleName,
                    RemainingFee = install.Student.Student.FinalFees - install.ReceivedFee,
                     Payment = install.Payment,
                     CreatedOn = install.CreatedOn,
                     ClassName = install.Class.Name,
                     ReceiptNumber = install.ReceiptNumber,
                     ReceiptBookNumber = install.ReceiptBookNumber,
                     TotalFee = install.Student.Student.FinalFees.ToString(),
                     ReceivedFee = install.ReceivedFee.ToString(),
                     ClassId = install.ClassId,
                     InstallmentNo = install.InstallmentId,
                     BranchId = install.Student.Student.BranchId,
                     BranchName = install.Student.Student.Branch.Name,
                 })).AsQueryable();

            if (BranchId != 0)
            {
                query = query.Where(p => p.BranchId == BranchId);
            }
            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }
            if (!string.IsNullOrWhiteSpace(filterUseId) && filterUseId != "null")
            {
                query = query.Where(p => p.UserId.Contains(filterUseId.ToString()));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(InstallmentGridModel.InstallmentId):
                        if (!desc)
                            query = query.OrderBy(p => p.InstallmentId);
                        else
                            query = query.OrderByDescending(p => p.InstallmentId);
                        break;
                    case nameof(InstallmentGridModel.StudentFirstName):
                        if (!desc)
                            query = query.OrderBy(p => p.StudentFirstName);
                        else
                            query = query.OrderByDescending(p => p.StudentFirstName);
                        break;
                    case nameof(InstallmentGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(InstallmentGridModel.RemainingFee):
                        if (!desc)
                            query = query.OrderBy(p => p.RemainingFee);
                        else
                            query = query.OrderByDescending(p => p.RemainingFee);
                        break;
                    case nameof(InstallmentGridModel.Payment):
                        if (!desc)
                            query = query.OrderBy(p => p.Payment);
                        else
                            query = query.OrderByDescending(p => p.Payment);
                        break;
                    case nameof(InstallmentGridModel.ReceiptBookNumber):
                        if (!desc)
                            query = query.OrderBy(p => p.ReceiptBookNumber);
                        else
                            query = query.OrderByDescending(p => p.ReceiptBookNumber);
                        break;
                    case nameof(InstallmentGridModel.ReceiptNumber):
                        if (!desc)
                            query = query.OrderBy(p => p.ReceiptNumber);
                        else
                            query = query.OrderByDescending(p => p.ReceiptNumber);
                        break;
                    case nameof(InstallmentGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }


            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }

            return query.ToList();
        }

        public InstallmentProjection GetInstallmentByInstallmentId(int installmentId)
        {
            return _repository.Project<Installment, InstallmentProjection>(
                installments => (from i in installments
                                 where i.InstallmentId == installmentId
                                 select new InstallmentProjection
                                 {
                                     BranchId = i.Student.Student.BranchId,
                                     ClassId = i.ClassId,
                                     UserId = i.Student.Student.UserId,
                                     FirstName = i.Student.Student.FirstName,
                                     LastName = i.Student.Student.LastName,
                                     MiddleName = i.Student.Student.MiddleName,
                                     FinalFee = i.Student.Student.FinalFees,
                                     InstallmentId = i.InstallmentId,
                                     Payment = i.Payment,
                                     RemainingFee = i.Student.Student.FinalFees - i.ReceivedFee,
                                     ReceiptBookNumber = i.ReceiptBookNumber,
                                     ReceiptNumber = i.ReceiptNumber,
                                     BranchName = i.Student.Student.Branch.Name,
                                     ClassName = i.Student.Student.Class.Name,
                                     Email = i.Student.Student.User.Email,
                                     StudentContact = i.Student.Student.StudentContact,
                                     ParentContact = i.Student.Student.ParentContact,
                                     ParentAppPlayerId = i.Student.Student.parentAppPlayerId,
                                     StudentSubjects = i.Student.Student.Subjects.Select(x => x.Name),
                                     StudBatch = i.Student.Student.Batches.Name
                                 }).FirstOrDefault());
        }

        public CMSResult Update(Installment oldInstallment)
        {
            CMSResult result = new CMSResult();

            var isExists = _repository.Project<Installment, bool>(
                         installments => (from installment in installments
                                          where installment.InstallmentId != oldInstallment.InstallmentId
                                          && installment.ReceiptBookNumber == oldInstallment.ReceiptBookNumber
                                          && installment.ReceiptNumber == oldInstallment.ReceiptNumber
                                          select installment).Any());


            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Receipt Number already exists!") });
            }
            else
            {
                var installment = _repository.Load<Installment>(x => x.InstallmentId == oldInstallment.InstallmentId);
                installment.Payment = oldInstallment.Payment;
                installment.ReceiptNumber = oldInstallment.ReceiptNumber;
                installment.ReceiptBookNumber = oldInstallment.ReceiptBookNumber;
                installment.ReceivedFee = oldInstallment.ReceivedFee;
                _repository.Update(installment);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Payment updated successfully!") });
            }
            return result;
        }
    }
}
