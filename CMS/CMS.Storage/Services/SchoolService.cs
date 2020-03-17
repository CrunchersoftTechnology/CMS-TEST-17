using System;
using System.Collections.Generic;
using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Infrastructure;
using System.Linq;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class SchoolService : ISchoolService
    {
        readonly IRepository _repository;

        public SchoolService(IRepository repository)
        {
            _repository = repository;
        }
        public CMSResult Delete(int id)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<School>(x => x.SchoolId == id);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("School '{0}' does not already exists!", model.Name) });
            }
            else
            {
                var isExistsStudent = _repository.Project<Student, bool>(students => (
                                            from s in students
                                            where s.SchoolId == id
                                            select s)
                                            .Any());

                //var isExistsAttendance = _repository.Project<Attendance, bool>(attendances => (
                //                            from a in attendances
                //                            where a.BatchId == BatchId
                //                            select a)
                //                            .Any());

                if (isExistsStudent)
                {
                    var selectModel = "";
                    selectModel += (isExistsStudent) ? "Student, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete School '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("School '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public SchoolProjection GetSchoolById(int schoolId)
        {
            return _repository.Project<School, SchoolProjection>(
                schools => (from school in schools
                            where school.SchoolId == schoolId
                            select new SchoolProjection
                            {
                                SchoolId = school.SchoolId,
                                CenterNumber = school.CenterNumber,
                                Name = school.Name
                            }).FirstOrDefault());
        }

        public IEnumerable<SchoolProjection> GetAllSchools()
        {
            return _repository.Project<School, SchoolProjection[]>(
                schools => (from school in schools
                            select new SchoolProjection
                            {
                                SchoolId = school.SchoolId,
                                CenterNumber = school.CenterNumber,
                                Name = school.Name
                            }).ToArray()).ToArray();
        }

        public IEnumerable<SchoolProjection> GetAllSchoolsByClientId(int ClientId)
        {
            return _repository.Project<School, SchoolProjection[]>(
                schools => (from school in schools
                            where school.ClientId==ClientId
                            select new SchoolProjection
                            {
                                SchoolId = school.SchoolId,
                                CenterNumber = school.CenterNumber,
                                Name = school.Name
                            }).ToArray()).ToArray();
        }

        public CMSResult Save(School newSchool)
        {
            CMSResult result = new CMSResult();
            var isExistsCenterNumber = false;
            var isExistsName = _repository.Project<School, bool>(schools => (from s in schools where s.Name == newSchool.Name && s.ClientId==newSchool.ClientId select s).Any());
            if (newSchool.CenterNumber != null)
            {
               isExistsCenterNumber = _repository.Project<School, bool>(schools => (from s in schools where s.CenterNumber == newSchool.CenterNumber && s.ClientId == newSchool.ClientId select s).Any());
            }
            if (isExistsName)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("School '{0}' already exists!", newSchool.Name) });
            }
            else if (isExistsCenterNumber)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("CenterNumber '{0}' already exists!", newSchool.CenterNumber) });
            }
            else
            {
                _repository.Add(newSchool);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("School '{0}' successfully added!", newSchool.Name) });
            }
            return result;
        }

        public CMSResult Update(School oldSchool)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<School, bool>(schools => (from s in schools where s.SchoolId != oldSchool.SchoolId && s.Name == oldSchool.Name select s).Any());
            var isExistsCenterNumber = _repository.Project<School, bool>(schools => (from s in schools where s.SchoolId != oldSchool.SchoolId && s.CenterNumber == oldSchool.CenterNumber select s).Any());

            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("School '{0}' already exists!", oldSchool.Name) });
            }
            else if (isExistsCenterNumber)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("CenterNumber '{0}' already exists!", oldSchool.CenterNumber) });
            }
            else
            {
                var school = _repository.Load<School>(x => x.SchoolId == oldSchool.SchoolId);
                school.SchoolId = oldSchool.SchoolId;
                school.Name = oldSchool.Name;
                school.CenterNumber = oldSchool.CenterNumber;
                _repository.Update(school);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("School '{0}' successfully updated!", oldSchool.Name) });
            }
            return result;
        }

        public IEnumerable<SchoolGridModel> GetSchoolData(out int totalRecords, string Name,
          int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<School, IQueryable<SchoolGridModel>>(Schooles => (
                 from s in Schooles
                 select new SchoolGridModel
                 {
                     SchoolId =s.SchoolId,
                     SchoolName = s.Name,
                     CenterNumber = s.CenterNumber,
                     CreatedOn = s.CreatedOn,
                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.SchoolName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(SchoolGridModel.SchoolName):
                        if (!desc)
                            query = query.OrderBy(p => p.SchoolName);
                        else
                            query = query.OrderByDescending(p => p.SchoolName);
                        break;
                    case nameof(SchoolGridModel.CenterNumber):
                        if (!desc)
                            query = query.OrderBy(p => p.CenterNumber);
                        else
                            query = query.OrderByDescending(p => p.CenterNumber);
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


        public IEnumerable<SchoolGridModel> GetSchoolDataByClientId(out int totalRecords, string Name, int userId,
          int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int ClientId = userId;
            var query = _repository.Project<School, IQueryable<SchoolGridModel>>(Schooles => (
                 from s in Schooles
                 select new SchoolGridModel
                 {
                     SchoolId = s.SchoolId,
                     ClientId = s.ClientId,
                     SchoolName = s.Name,
                     CenterNumber = s.CenterNumber,
                     CreatedOn = s.CreatedOn,
                 })).AsQueryable();
            if (ClientId != 0)
            {
                query = query.Where(p => p.ClientId == ClientId);
            }
            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.SchoolName.Contains(Name));
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(SchoolGridModel.SchoolName):
                        if (!desc)
                            query = query.OrderBy(p => p.SchoolName);
                        else
                            query = query.OrderByDescending(p => p.SchoolName);
                        break;
                    case nameof(SchoolGridModel.CenterNumber):
                        if (!desc)
                            query = query.OrderBy(p => p.CenterNumber);
                        else
                            query = query.OrderByDescending(p => p.CenterNumber);
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
