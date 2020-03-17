using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using System.Linq;
using System;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class BranchAdminService : IBranchAdminService
    {
        readonly IRepository _repository;

        public BranchAdminService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(BranchAdmin branchAdmin)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Branch, bool>(branches => (from b in branches where b.Name == branchAdmin.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Branch '{0}' already exists!", branchAdmin.Name) });
            }
            else
            {
                _repository.Add(branchAdmin);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch '{0}' successfully added!", branchAdmin.Name) });
            }
            return result;
        }

        public BranchAdminProjection GetBranchAdminById(string branchAdminId)
        {
            return _repository.Project<BranchAdmin, BranchAdminProjection>(
                branchAdmins => (from b in branchAdmins
                                 where b.UserId == branchAdminId 
                                 select new BranchAdminProjection
                                 {
                                     ClientId=b.ClientId,
                                     BranchId = b.BranchId,
                                     BranchName = b.Branch.Name,
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     Name = b.Name,
                                     UserId = b.UserId,
                                     Active = b.Active
                                 }).FirstOrDefault());
        }

      


        public CMSResult Update(BranchAdmin branchAdmin)
        {
            var result = new CMSResult();
            var batch = _repository.Project<BranchAdmin, bool>(users => (from u in users where u.UserId == branchAdmin.UserId select u).Any());

            if (!batch)
            {
                result.Results.Add(new Result { Message = "Branch Admin not exists." });
            }
            else
            {
                var branchAdminContact = _repository.Project<BranchAdmin, bool>(branchAdmins => (from a in branchAdmins where a.ContactNo == branchAdmin.ContactNo && a.UserId != branchAdmin.UserId select a).Any());
                if (branchAdminContact)
                {
                    result.Results.Add(
                        new Result
                        {
                            IsSuccessful = false,
                            Message = string.Format("Contact Number already exists!")
                        });
                }

                if (!branchAdminContact)
                {
                    var BranchAdminUser = _repository.Load<ApplicationUser>(x => x.Id == branchAdmin.UserId, x => x.BranchAdmin);

                    BranchAdminUser.BranchAdmin.BranchId = branchAdmin.BranchId;
                    BranchAdminUser.BranchAdmin.Name = branchAdmin.Name;
                    BranchAdminUser.BranchAdmin.ContactNo = branchAdmin.ContactNo;
                    BranchAdminUser.BranchAdmin.Active = branchAdmin.Active;

                    _repository.Update(BranchAdminUser);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch Admin '{0}' successfully updated!", branchAdmin.Name) });
                }
            }

            return result;
        }

        public IEnumerable<BranchAdminProjection> GetBranches()
        {
            return _repository.Project<BranchAdmin, BranchAdminProjection[]>(
                branchAdmins => (from b in branchAdmins
                                 orderby b.Name
                                 select new BranchAdminProjection
                                 {
                                     BranchName = b.Branch.Name,
                                     Name = b.Name,
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     Active = b.Active,
                                     UserId = b.UserId,
                                     AId = b.AId
                                 }).ToArray());
        }

        public CMSResult Delete(string userId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<BranchAdmin>(c => c.UserId == userId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Branch Admin '{0}' already exists!", model.Name) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch Admin '{0}' deleted successfully!", model.Name) });

            }
            return result;
        }

        public BranchAdminProjection GetBranchesById(string branchAdminId)
        {
            return _repository.Project<BranchAdmin, BranchAdminProjection>(
                branchAdmins => (from b in branchAdmins
                                 where b.UserId == branchAdminId
                                 select new BranchAdminProjection
                                 {
                                     BranchName = b.Branch.Name,
                                     BranchId = b.BranchId
                                 }).FirstOrDefault());
        }

        public IEnumerable<BranchAdminProjection> GetBranchAdminContactList()
        {
            return _repository.Project<BranchAdmin, BranchAdminProjection[]>(
                branchAdmins => (from b in branchAdmins
                                 where b.Active == true
                                 select new BranchAdminProjection
                                 {
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     BranchId = b.BranchId,
                                     Name = b.Name,

                                 }).ToArray());
        }

        public IEnumerable<BranchAdminGridModel> GetBranchAdminData(out int totalRecords, string Name,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<BranchAdmin, IQueryable<BranchAdminGridModel>>(branchAdmins => (
                 from b in branchAdmins
                 select new BranchAdminGridModel
                 {
                     AId = b.AId,
                     Name = b.Name,
                     ContactNo = b.ContactNo,
                     Email = b.User.Email,
                     BranchName = b.Branch.Name,
                     IsActive = b.Active,
                     UserId = b.UserId,
                     CreatedOn = b.CreatedOn,
                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.Name.Contains(Name));
            }
            //if (!string.IsNullOrWhiteSpace(globalSearch))
            //{
            //    query = query.Where(p => (p.FirstName + " " + p.LastName).Contains(globalSearch));
            //}

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BranchAdminGridModel.Name):
                        if (!desc)
                            query = query.OrderBy(p => p.Name);
                        else
                            query = query.OrderByDescending(p => p.Name);
                        break;
                    case nameof(BranchAdminGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(BranchAdminGridModel.IsActive):
                        if (!desc)
                            query = query.OrderBy(p => p.IsActive);
                        else
                            query = query.OrderByDescending(p => p.IsActive);
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

        public IEnumerable<BranchAdminProjection> GetBranchAdminContactListBrbranchId(int branchId)
        {
            return _repository.Project<BranchAdmin, BranchAdminProjection[]>(
                branchAdmins => (from b in branchAdmins
                                 where b.Active == true && b.BranchId == branchId
                                 select new BranchAdminProjection
                                 {
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     BranchId = b.BranchId,
                                     Name = b.Name,

                                 }).ToArray());
        }

        public IEnumerable<BranchAdminProjection> GetClientAdminContactListBrbranchId(int clientId)
        {
            return _repository.Project<BranchAdmin, BranchAdminProjection[]>(
                branchAdmins => (from b in branchAdmins
                                 where b.Active == true && b.ClientId == clientId
                                 select new BranchAdminProjection
                                 {
                                     Email = b.User.Email,
                                     ContactNo = b.ContactNo,
                                     ClientId = b.ClientId,
                                     Name = b.Name,

                                 }).ToArray());
        }
    }
}
