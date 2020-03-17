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
    public class TeacherService : ITeacherService
    {
        readonly IRepository _repository;

        public TeacherService(IRepository repository)
        {
            _repository = repository;
        }
        public bool Save(Teacher teacher)
        {
            return true;
        }

        public IEnumerable<TeacherProjection> GetTeachers()
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             orderby t.FirstName
                             select new TeacherProjection
                             {
                                 UserId = t.UserId,
                                 FirstName = t.FirstName,
                                 MiddleName = t.MiddleName,
                                 LastName = t.LastName,
                                 ContactNo = t.ContactNo,
                                 Description = t.Description,
                                 Email = t.User.Email

                             }).ToArray());
        }
        public IEnumerable<TeacherProjection> GetAllTeachersByPrefix(string prefix)
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             where t.Qualification.StartsWith(prefix)
                             select new TeacherProjection
                             {
                                 UserId = t.UserId,
                                 FirstName = t.FirstName,
                                 MiddleName = t.MiddleName,
                                 LastName = t.LastName,
                                 Email = t.User.Email,
                                 Qualification = t.Qualification
                             }).Distinct().ToArray());
        }

        public CMSResult Update(Teacher user)
        {
            var result = new CMSResult();
            var teacher = _repository.Project<Teacher, bool>(users => (from u in users where u.UserId == user.UserId select u).Any());

            if (!teacher)
            {
                result.Results.Add(
                        new Result
                        {
                            IsSuccessful = false,
                            Message = string.Format("Teacher not exists!")
                        });
            }
            else
            {
                var teacherContact = _repository.Project<Teacher, bool>(users => (from u in users where u.ContactNo == user.ContactNo && u.UserId != user.UserId select u).Any());
                if (teacherContact)
                {
                    result.Results.Add(
                        new Result
                        {
                            IsSuccessful = false,
                            Message = string.Format("Contact Number already exists!")
                        });
                }
                else
                {
                    var teachertUser = _repository.Load<ApplicationUser>(x => x.Id == user.UserId, x => x.Teacher);
                    teachertUser.Teacher.FirstName = user.FirstName;
                    teachertUser.Teacher.MiddleName = user.MiddleName;
                    teachertUser.Teacher.LastName = user.LastName;
                    teachertUser.Teacher.ContactNo = user.ContactNo;
                    teachertUser.Teacher.Description = user.Description;
                    teachertUser.Teacher.BranchId = user.BranchId;
                    teachertUser.Teacher.IsActive = user.IsActive;
                    teachertUser.Teacher.Qualification = user.Qualification;
                    _repository.Update(teachertUser);
                    result.Results.Add(
                        new Result
                        {
                            IsSuccessful = true,
                            Message = string.Format("Teacher '{0} {1}' successfully updated!", user.FirstName, user.LastName)
                        });
                }

            }
            return result;
        }

        public TeacherProjection GetTeacherById(string teacherId)
        {
            return _repository.Project<Teacher, TeacherProjection>(
                teachers => (from s in teachers
                             where s.UserId == teacherId
                             select new TeacherProjection
                             {
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 ContactNo = s.ContactNo,
                                 Description = s.Description,
                                 Email = s.User.Email,
                                 UserId = s.UserId,
                                 BranchId = s.BranchId,
                                 BranchName = s.Branch.Name,
                                 IsActive = s.IsActive,
                                 Qualification=s.Qualification
                             }).FirstOrDefault());
        }

        public IEnumerable<TeacherProjection> GetTeachersBind()
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             select new TeacherProjection
                             {
                                 UserId = t.UserId,
                                 FirstName = t.FirstName,
                                 MiddleName = t.MiddleName,
                                 LastName = t.LastName
                             }).ToArray());
        }

        public IEnumerable<TeacherProjection> GetTeachers(int branchId)
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             orderby t.FirstName
                             where t.ClientId==branchId
                             select new TeacherProjection
                             {
                                 UserId = t.UserId,
                                 FirstName = t.FirstName,
                                 MiddleName = t.MiddleName,
                                 LastName = t.LastName,
                                 ContactNo = t.ContactNo,
                                 Description = t.Description,
                                 Email = t.User.Email,
                                 TId=t.TId,
                                 Qualification=t.Qualification
                             }).ToArray());
        }

        public IEnumerable<TeacherProjection> GetTeachersByClientId(int clientId)
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             orderby t.FirstName
                             where t.ClientId==clientId
                             select new TeacherProjection
                             {
                                
                                 UserId = t.UserId,
                                 FirstName = t.FirstName,
                                 MiddleName = t.MiddleName,
                                 LastName = t.LastName,
                                 ContactNo = t.ContactNo,
                                 Description = t.Description,
                                 Email = t.User.Email,
                                 TId = t.TId,
                                 Qualification = t.Qualification,
                                 ClientId = t.ClientId
                             }).ToArray());
        }


        public IEnumerable<TeacherProjection> GetTeacherContactList()
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             where t.IsActive == true
                             select new TeacherProjection
                             {
                                 Email = t.User.Email,
                                 ContactNo = t.ContactNo,
                                 BranchId = t.BranchId,
                                 IsActive = t.IsActive,
                                 Name = t.FirstName + " " + t.MiddleName + " " + t.LastName,

                             }).ToArray());
        }

        public int GetTeachersCount()
        {
            return _repository.Project<Teacher, int>(
               teachers => (from t in teachers select t).Count());
        }

        public int GetTeachersCountByClientId(int ClientId)
        {
            return _repository.Project<Teacher, int>(
               teachers => (from t in teachers  where t.ClientId==ClientId select t).Count());
        }


        public IEnumerable<TeacherGridModel> GetTeacherData(out int totalRecords, string filterFirstName,
           string filterLastName, int userId, int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = 0;
                   
            var query = _repository.Project<Teacher, IQueryable<TeacherGridModel>>(teachers => (
           from t in teachers 
           select new TeacherGridModel
           {
               TId = t.TId,
               FirstName = t.FirstName,
               LastName = t.LastName,
               ContactNo = t.ContactNo,
               Email = t.User.Email,
               BranchName = t.Branch.Name,
               Description = t.Description,
               IsActive = t.IsActive,
               UserId = t.UserId,
               BranchId = t.BranchId,
               ClientId = t.ClientId,
               CreatedOn = t.CreatedOn,
               Qualification=t.Qualification

           })).AsQueryable();
                
            if (userId != 0)
            {
                query = query.Where(p => p.ClientId == userId);
            }
            if (BranchId != 0)
            {
                query = query.Where(p => p.BranchId == BranchId);
            }
            if (!string.IsNullOrWhiteSpace(filterFirstName))
            {
                query = query.Where(p => p.FirstName.Contains(filterFirstName));
            }
            if (!string.IsNullOrWhiteSpace(filterLastName))
            {
                query = query.Where(p => p.LastName.Contains(filterLastName));
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
                    case nameof(TeacherGridModel.Qualification):
                        if (!desc)
                            query = query.OrderBy(p => p.Qualification);
                        else
                            query = query.OrderByDescending(p => p.Qualification);
                        break;
                    case nameof(TeacherGridModel.FirstName):
                        if (!desc)
                            query = query.OrderBy(p => p.FirstName);
                        else
                            query = query.OrderByDescending(p => p.FirstName);
                        break;
                    case nameof(TeacherGridModel.LastName):
                        if (!desc)
                            query = query.OrderBy(p => p.LastName);
                        else
                            query = query.OrderByDescending(p => p.LastName);
                        break;
                    case nameof(TeacherGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(TeacherGridModel.IsActive):
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

        public IEnumerable<TeacherProjection> GetTeacherContactListBrbranchId(int branchId)
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             where t.IsActive == true && t.BranchId == branchId
                             select new TeacherProjection
                             {
                                 Email = t.User.Email,
                                 ContactNo = t.ContactNo,
                                 BranchId = t.BranchId,
                                 IsActive = t.IsActive,
                                 Name = t.FirstName + " " + t.MiddleName + " " + t.LastName,

                             }).ToArray());
        }

        public IEnumerable<TeacherProjection> GetTeacherContactListBrclientId(int clientId)
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             where t.IsActive == true && t.ClientId == clientId
                             select new TeacherProjection
                             {
                                 Email = t.User.Email,
                                 ContactNo = t.ContactNo,
                                 ClientId = t.ClientId,
                                 IsActive = t.IsActive,
                                 Name = t.FirstName + " " + t.MiddleName + " " + t.LastName,

                             }).ToArray());
        }

        public IEnumerable<TeacherProjection> GetTeachersForWebSite()
        {
            return _repository.Project<Teacher, TeacherProjection[]>(
                teachers => (from t in teachers
                             where t.IsActive
                             orderby t.BranchId, t.FirstName
                             select new TeacherProjection
                             {
                                 FirstName = t.FirstName,
                                 MiddleName = t.MiddleName,
                                 LastName = t.LastName,
                                 ContactNo = t.ContactNo,
                                 Description = t.Description,
                                 Email = t.User.Email,
                                 BranchId = t.BranchId,
                                 BranchName = t.Branch.Name,
                                 IsActive = t.IsActive,
                                 Qualification=t.Qualification
                             }).ToArray());
        }
    }
}
