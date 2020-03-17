using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class ClassService : IClassService
    {
        readonly IRepository _repository;

        public ClassService(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<ClassProjection> GetClasses()
        {
            return _repository.Project<Class, ClassProjection[]>(
                classes => (from cls in classes
                            select new ClassProjection
                            {
                                ClassId = cls.ClassId,
                                Name = cls.Name
                            }).ToArray());
        }

       

        public IEnumerable<ClassProjection> GetClassesByClientId(int ClientId)
        {
            return _repository.Project<Class, ClassProjection[]>(
                classes => (from cls in classes
                            where cls.ClientId == ClientId
                            select new ClassProjection
                            {
                                ClassId = cls.ClassId,
                                Name = cls.Name
                            }).ToArray());
        }

        public ClassProjection GetClassById(int classId)
        {
            return _repository.Project<Class, ClassProjection>(
                classes => (from c in classes
                            where c.ClassId == classId
                            select new ClassProjection
                            {
                                ClassId = c.ClassId,
                                Name = c.Name
                            }).FirstOrDefault());
        }

        public IEnumerable<ClassProjection> GetAllClassByPrefix(string prefix)
        {
            return _repository.Project<Class, ClassProjection[]>(
                classes => (from b in classes
                            where b.Name.StartsWith(prefix)
                            select new ClassProjection
                            {
                                Name = b.Name
                            }).Distinct().ToArray());
        }
        public CMSResult Save(Class newClass)
        {
            var result = new CMSResult();
            var isExists = _repository.Project<Class, bool>(classes => (
                                            from clss in classes
                                            where clss.Name == newClass.Name && clss.ClientId == newClass.ClientId
                                            select clss)
                                            .Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Class '{0}' already exists!", newClass.Name) });
            }
            else
            {
                _repository.Add(newClass);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Class '{0}' added successfully!", newClass.Name) });
            }
            return result;
        }

        public CMSResult Update(Class oldClass)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Class, bool>(classes => (from clss in classes where clss.ClassId != oldClass.ClassId && clss.Name == oldClass.Name select clss).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Class '{0}' already exists!", oldClass.Name) });
            }
            else
            {
                var cls = _repository.Load<Class>(x => x.ClassId == oldClass.ClassId);
                cls.Name = oldClass.Name;
                _repository.Update(cls);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Class '{0}' successfully updated!", oldClass.Name) });
            }
            return result;
        }

        public CMSResult Delete(int classId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Class>(c => c.ClassId == classId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Class '{0}' already exists!", model.Name) });
            }
            else
            {
                var isExistsBatch = _repository.Project<Batch, bool>(batches => (
                                            from batch in batches
                                            where batch.ClassId == classId
                                            select batch)
                                            .Any());

                var isExistsSubject = _repository.Project<Subject, bool>(subjects => (
                                            from s in subjects
                                            where s.ClassId == classId
                                            select s)
                                            .Any());

                var isExistsStudent = _repository.Project<Student, bool>(students => (
                                            from s in students
                                            where s.ClassId == classId
                                            select s)
                                            .Any());

                var isExistsInstallment = _repository.Project<Installment, bool>(installments => (
                                            from i in installments
                                            where i.ClassId == classId
                                            select i)
                                            .Any());

                var isExistsMasterFee = _repository.Project<MasterFee, bool>(masterFees => (
                                            from m in masterFees
                                            where m.ClassId == classId
                                            select m)
                                            .Any());

                var isExistsAttendance = _repository.Project<Attendance, bool>(attendance => (
                                            from a in attendance
                                            where a.ClassId == classId
                                            select a)
                                            .Any());

                var isExistsPaper = _repository.Project<TestPaper, bool>(paper => (
                                            from t in paper
                                            where t.ClassId == classId
                                            select t)
                                            .Any());


                if (isExistsSubject || isExistsStudent || isExistsInstallment || isExistsMasterFee || isExistsAttendance || isExistsPaper || isExistsBatch)
                {
                    var selectModel = "";
                    selectModel += (isExistsBatch) ? "Batch, " : "";
                    selectModel += (isExistsSubject) ? "Subject, " : "";
                    selectModel += (isExistsStudent) ? "Student, " : "";
                    selectModel += (isExistsInstallment) ? "Installment, " : "";
                    selectModel += (isExistsMasterFee) ? "MasterFee, " : "";
                    selectModel += (isExistsAttendance) ? "Attendance, " : "";
                    selectModel += (isExistsPaper) ? "Paper, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Class '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Class '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public IEnumerable<ClassGridModel> GetClassData(out int totalRecords, string Name,
       int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Class, IQueryable<ClassGridModel>>(Classes => (
                 from c in Classes
                 select new ClassGridModel
                 {
                     ClassId = c.ClassId,
                     ClassName = c.Name,
                     CreatedOn = c.CreatedOn,

                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.ClassName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(ClassGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
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

        public IEnumerable<ClassGridModel> GetClassDataByClientId(out int totalRecords, string Name, int userId,
     int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int ClientId = userId;
            var query = _repository.Project<Class, IQueryable<ClassGridModel>>(Classes => (
                 from b in Classes

                 select new ClassGridModel
                 {
                     UserId = b.UserId,
                     ClientId = b.ClientId,
                     ClassId = b.ClassId,
                     ClassName = b.Name,
                     CreatedOn = b.CreatedOn,

                 })).AsQueryable();
            if (ClientId != 0)
            {
                query = query.Where(p => p.ClientId == ClientId);
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.ClassName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(BranchGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
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
