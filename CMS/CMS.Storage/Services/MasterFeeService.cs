using System.Collections.Generic;
using System.Linq;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Infrastructure;
using CMS.Common;
using CMS.Common.GridModels;
using System;

namespace CMS.Domain.Storage.Services
{
    public class MasterFeeService : IMasterFeeService
    {
        readonly IRepository _repository;

        public MasterFeeService(IRepository repository)
        {
            _repository = repository;
        }
        public CMSResult Delete(int id)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<MasterFee>(x => x.MasterFeeId == id);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("MasterFee '{0}' already exists!", model.MasterFeeId) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("MasterFee '{0}' deleted successfully!", model.MasterFeeId) });
            }
            return result;
        }

        public IEnumerable<MasterFeeProjection> GetMasterFees(int subjectId, int classId)
        {
            return _repository.Project<MasterFee, MasterFeeProjection[]>(
                masterfees => (from masterfee in masterfees
                               where masterfee.SubjectId == subjectId && masterfee.ClassId == classId
                               select new MasterFeeProjection
                               {
                                   MasterFeeId = masterfee.MasterFeeId,
                                   Year = masterfee.Year,
                                   Fee = masterfee.Fee,
                                   ClassName = masterfee.Class.Name,
                                   SubjectName = masterfee.Subject.Name,
                                   SubjectId = masterfee.SubjectId
                               }).ToArray());
        }

        public IEnumerable<MasterFeeProjection> GetAllMasterFees()
        {
            return _repository.Project<MasterFee, MasterFeeProjection[]>(
                masterfees => (from masterfee in masterfees
                               select new MasterFeeProjection
                               {
                                   MasterFeeId = masterfee.MasterFeeId,
                                   Year = masterfee.Year,
                                   Fee = masterfee.Fee,
                                   ClassName = masterfee.Class.Name,
                                   SubjectName = masterfee.Subject.Name,
                                   SubjectId = masterfee.SubjectId,
                                   ClassId = masterfee.ClassId


                               }).ToArray());
        }
        public MasterFeeProjection GetMasterFeeById(int masterfeeId)
        {
            return _repository.Project<MasterFee, MasterFeeProjection>(
                masterfees => (from c in masterfees
                               where c.MasterFeeId == masterfeeId
                               select new MasterFeeProjection
                               {
                                   MasterFeeId = c.MasterFeeId,
                                   SubjectName = c.Subject.Name,
                                   Year = c.Year,
                                   Fee = c.Fee,
                                   ClassName = c.Class.Name,
                                   ClassId = c.ClassId,
                                   SubjectId = c.SubjectId
                               }).FirstOrDefault());
        }

        public CMSResult Save(MasterFee masterfee)
        {
            var result = new CMSResult();
            var isExists = _repository.Project<MasterFee, bool>
                (masterfees => (from mfee in masterfees
                                where mfee.Year == masterfee.Year
                                && mfee.SubjectId == masterfee.SubjectId
                                && mfee.ClassId == masterfee.ClassId
                                select mfee).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("MasterFee already exists!") });
            }
            else
            {
                _repository.Add(masterfee);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("MasterFee added successfully!") });
            }
            return result;
        }

        public CMSResult Save(int ClientId,MasterFee masterfee)
        {
            var result = new CMSResult();
            var isExists = _repository.Project<MasterFee, bool>
                (masterfees => (from mfee in masterfees
                                where mfee.Year == masterfee.Year
                                && mfee.SubjectId == masterfee.SubjectId
                                && mfee.ClassId == masterfee.ClassId
                                && mfee.ClientId == masterfee.ClientId
                                select mfee).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("MasterFee already exists!") });
            }
            else
            {
                masterfee.ClientId = ClientId;
                _repository.Add(masterfee);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("MasterFee added successfully!") });
            }
            return result;
        }

        public CMSResult Update(MasterFee masterfee)
        {
            CMSResult result = new CMSResult();
         
            var isExists = _repository.Project<MasterFee, bool>(
                         masterfees => (from mfee in masterfees
                                        where mfee.MasterFeeId != masterfee.MasterFeeId
                                        && mfee.Year == masterfee.Year
                                        && mfee.SubjectId == masterfee.SubjectId
                                        && mfee.ClassId == masterfee.ClassId
                                        select mfee).Any());


            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("MasterFee already exists!") });
            }
            else
            {
                var masterf = _repository.Load<MasterFee>(x => x.MasterFeeId == masterfee.MasterFeeId);
                masterf.Year = masterfee.Year;
                masterf.Fee = masterfee.Fee;
                _repository.Update(masterf);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("MasterFee updated successfully!") });
            }
            return result;
        }

        public IEnumerable<MasterFeeGridModel> GetMasterFeeData(out int totalRecords, int filterClassName, int filterSubjectName,
          int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<MasterFee, IQueryable<MasterFeeGridModel>>(masterFees => (
                 from m in masterFees
                 select new MasterFeeGridModel
                 {
                     MasterFeeId = m.MasterFeeId,
                     Year = m.Year,
                     SubjectId = m.SubjectId,
                     SubjectName = m.Subject.Name,
                     ClassId = m.ClassId,
                     ClassName = m.Class.Name,
                     CreatedOn = m.CreatedOn,
                     Fee = m.Fee
                 })).AsQueryable();


            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }
            if (filterSubjectName != 0)
            {
                query = query.Where(p => p.SubjectId == filterSubjectName);
            }

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(MasterFeeGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(MasterFeeGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(MasterFeeGridModel.Fee):
                        if (!desc)
                            query = query.OrderBy(p => p.Fee);
                        else
                            query = query.OrderByDescending(p => p.Fee);
                        break;
                    case nameof(MasterFeeGridModel.Year):
                        if (!desc)
                            query = query.OrderBy(p => p.Year);
                        else
                            query = query.OrderByDescending(p => p.Year);
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

        public IEnumerable<MasterFeeGridModel> GetMasterFeeDataByClientId(out int totalRecords, int filterClassName, int filterSubjectName,int ClientId,
         int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {

            var query = _repository.Project<MasterFee, IQueryable<MasterFeeGridModel>>(masterFees => (
                 from m in masterFees
                 where m.ClientId == ClientId
                 select new MasterFeeGridModel
                 {
                     MasterFeeId = m.MasterFeeId,
                     Year = m.Year,
                     SubjectId = m.SubjectId,
                     SubjectName = m.Subject.Name,
                     ClassId = m.ClassId,
                     ClassName = m.Class.Name,
                     CreatedOn = m.CreatedOn,
                     Fee = m.Fee
                 })).AsQueryable();


            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName && p.ClientId == ClientId);
            }
            if (filterSubjectName != 0)
            {
                query = query.Where(p => p.SubjectId == filterSubjectName && p.ClientId == ClientId);
            }

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(MasterFeeGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(MasterFeeGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(MasterFeeGridModel.Fee):
                        if (!desc)
                            query = query.OrderBy(p => p.Fee);
                        else
                            query = query.OrderByDescending(p => p.Fee);
                        break;
                    case nameof(MasterFeeGridModel.Year):
                        if (!desc)
                            query = query.OrderBy(p => p.Year);
                        else
                            query = query.OrderByDescending(p => p.Year);
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
    }
}
