using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;
using System;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class BranchService : IBranchService
    {
        readonly IRepository _repository;

        public BranchService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Delete(int branchId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Branch>(x => x.BranchId == branchId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Branch '{0}' does not already exists!", model.Name) });
            }
            else
            {
                var isExistsBranchAdmin = _repository.Project<BranchAdmin, bool>(branchAdmins => (
                                            from b in branchAdmins
                                            where b.BranchId == branchId
                                            select b)
                                            .Any());

                var isExistsMachine = _repository.Project<Machine, bool>(machines => (
                                            from m in machines
                                            where m.BranchId == branchId
                                            select m)
                                            .Any());

                if (isExistsBranchAdmin || isExistsMachine)
                {
                    var selectModel = "";
                    selectModel += (isExistsBranchAdmin) ? "Branch Admin, " : "";
                    selectModel += (isExistsMachine) ? "Machine, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Branch '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public IEnumerable<BranchProjection> GetAllBranches()
        {
            return _repository.Project<Branch, BranchProjection[]>(
                branches => (from branch in branches

                             select new BranchProjection
                             {
                                 BranchId = branch.BranchId,
                                 Name = branch.Name,
                                 Address = branch.Address
                             }).ToArray());
        }


        public IEnumerable<BranchProjection> GetAllBranchesByClientId(int ClientId)
        {
            return _repository.Project<Branch, BranchProjection[]>(
                branches => (from branch in branches 
                             where branch.ClientId==ClientId
                             select new BranchProjection
                             {
                                 BranchId = branch.BranchId,
                                 Name = branch.Name,
                                 Address = branch.Address
                             }).ToArray());
        }

        public BranchProjection GetBoardById(int id)
        {
            return _repository.Project<Branch, BranchProjection>(
               branches => (from branch in branches
                            where branch.BranchId == id
                            select new BranchProjection
                            {
                                Address = branch.Address,
                                BranchId = branch.BranchId,
                                Name = branch.Name
                            }).FirstOrDefault());
        }

        public int GetBranchesCount()
        {
            return _repository.Project<Branch, int>(
              branches => (from branch in branches select branch).Count());
        }

        public int GetBranchesCountByClientId(int ClientId)
        {
            return _repository.Project<Branch, int>(
              branches => (from branch in branches where branch.ClientId==ClientId select branch).Count());
        }

        public CMSResult Save(Branch branch)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Branch, bool>(branches => (from b in branches where b.Name == branch.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Branch '{0}' already exists!", branch.Name) });
            }
            else
            {
                _repository.Add(branch);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch '{0}' successfully added!", branch.Name) });
            }
            return result;
        }

        public CMSResult SaveBranch(Branch branch)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Branch, bool>(branches => (from b in branches where b.Name == branch.Name && b.ClientId == branch.ClientId select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Branch '{0}' already exists!", branch.Name) });
            }
            else
            {
                _repository.Add(branch);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch '{0}' successfully added!", branch.Name) });
            }
            return result;
        }


        public CMSResult Update(Branch branch)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Branch, bool>(branches => (from b in branches where b.BranchId != branch.BranchId && b.Name == branch.Name select b).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Branch '{0}' already exists!", branch.Name) });
            }
            else
            {
                var brch = _repository.Load<Branch>(x => x.BranchId == branch.BranchId);
                brch.Name = branch.Name;
                brch.Address = branch.Address;

                if (brch.IsChangeDetected)
                {
                    _repository.Update(brch);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Branch '{0}' successfully updated!", branch.Name) });
                }
                else
                {
                    _repository.Detach(brch);
                    result.Results.Add(new Result { IsSuccessful = true, Message = "No Change Detected!" });
                }
            }
            return result;
        }

        public IEnumerable<BranchGridModel> GetBranchData(out int totalRecords, string Name,
      int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Branch, IQueryable<BranchGridModel>>(Branches => (
                 from b in Branches
                
                 select new BranchGridModel
                 {
                     
                     BranchId = b.BranchId,
                     BranchName = b.Name,
                     Address = b.Address,
                     CreatedOn = b.CreatedOn,

                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.BranchName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BranchGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(BranchGridModel.Address):
                        if (!desc)
                            query = query.OrderBy(p => p.Address);
                        else
                            query = query.OrderByDescending(p => p.Address);
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

        public IEnumerable<BranchGridModel> GetBranchDataByClientId(out int totalRecords, string Name, int userId,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int ClientId = userId;
            var query = _repository.Project<Branch, IQueryable<BranchGridModel>>(Branches => (
                 from b in Branches                
                 select new BranchGridModel
                 {
                     UserId = b.UserId,
                     ClientId=b.ClientId,
                     BranchId = b.BranchId,
                     BranchName = b.Name,
                     Address = b.Address,
                     CreatedOn = b.CreatedOn,

                 })).AsQueryable();
            if (ClientId != 0)
            {
                query = query.Where(p => p.ClientId == ClientId);
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.BranchName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BranchGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(BranchGridModel.Address):
                        if (!desc)
                            query = query.OrderBy(p => p.Address);
                        else
                            query = query.OrderByDescending(p => p.Address);
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

        public IEnumerable<BranchProjection> GetBranchByMultipleBranchId(string selectedBranch)
        {
            var branchIds = selectedBranch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Branch, BranchProjection[]>(
                branch => (from s in branch
                           where branchIds.Contains(s.BranchId)
                           select new BranchProjection
                           {
                               BranchId = s.BranchId,
                               Name = s.Name,
                           }).ToArray());
        }
    }
}
