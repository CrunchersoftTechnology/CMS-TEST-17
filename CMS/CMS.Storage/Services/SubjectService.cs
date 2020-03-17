using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Domain.Storage.Services
{
    public class SubjectService : ISubjectService
    {
        readonly IRepository _repository;

        public SubjectService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Delete(int subjectId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Subject>(x => x.SubjectId == subjectId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Subject '{0}' already exists!", model.Name) });
            }
            else
            {
                var isExistsChapter = _repository.Project<Chapter, bool>(chapters => (
                                            from c in chapters
                                            where c.SubjectId == subjectId
                                            select c)
                                            .Any());

                var isExistsMasterFee = _repository.Project<MasterFee, bool>(masterFees => (
                                            from m in masterFees
                                            where m.SubjectId == subjectId
                                            select m)
                                            .Any());


                if (isExistsChapter || isExistsMasterFee)
                {
                    var selectModel = "";
                    selectModel += (isExistsChapter) ? "Chapter, " : "";
                    selectModel += (isExistsMasterFee) ? "MasterFee, " : "";
                    selectModel = selectModel.Trim().TrimEnd(',');
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("You can not delete Subject '{0}'. Because it belongs to {1}!", model.Name, selectModel) });
                }
                else
                {
                    _repository.Delete(model);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Subject '{0}' deleted successfully!", model.Name) });
                }
            }
            return result;
        }

        public IEnumerable<SubjectProjection> GetAllSubjects()
        {
            return _repository.Project<Subject, SubjectProjection[]>(
                subjects => (from subject in subjects
                             select new SubjectProjection
                             {
                                 SubjectId = subject.SubjectId,
                                 ClassId = subject.ClassId,
                                 Name = subject.Name,
                                 ClassName = subject.Class.Name,
                                 ClientId=subject.ClientId
                                 
                             }).ToArray());
        }

        public IEnumerable<SubjectProjection> GetSubjects(int ClassId)
        {
            return _repository.Project<Subject, SubjectProjection[]>(
                subjects => (from subject in subjects
                             where subject.ClassId == ClassId
                             select new SubjectProjection
                             {
                                 SubjectId = subject.SubjectId,
                                 ClassId = subject.ClassId,
                                 Name = subject.Name,
                                 ClassName = subject.Class.Name,
                                 ClientId=subject.ClientId
                                 
                             }).ToArray());
        }

        public CMSResult Save(Subject subject, List<ClassProjection> classList, int ClientId)
        {
            CMSResult result = new CMSResult();
            foreach (var cls in classList)
            {
                var isExists = _repository.Project<Subject, bool>(subjects => (from subj in subjects where subj.Name == subject.Name && subj.ClassId == cls.ClassId && subj.ClientId == ClientId select subj).Any());
                if (isExists)
                {
                    result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Subject '{0}' already exists!", subject.Name + " - " + cls.Name) });
                }
                else
                {
                    subject.ClassId = cls.ClassId;
                    subject.ClientId = ClientId;
                    _repository.Add(subject);
                    _repository.CommitChanges();
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Subject '{0}' successfully added!", subject.Name + " - " + cls.Name) });
                }
            }
            return result;
        }

        public CMSResult Update(Subject subject)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Subject, bool>(subjects =>
            (from subj in subjects
             where subj.SubjectId != subject.SubjectId
             && subj.Name == subject.Name
             && subj.ClassId == subject.ClassId
             select subj).Any());

            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Subject '{0}' already exists!", subject.Name) });
            }
            else
            {

                var sub = _repository.Load<Subject>(x => x.SubjectId == subject.SubjectId);
                sub.ClassId = subject.ClassId;
                sub.Name = subject.Name;
                _repository.Update(sub);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Subject '{0}' successfully updated!", subject.Name) });
            }
            return result;
        }

        public SubjectProjection GetSubjectById(int subjectId)
        {
            return _repository.Project<Subject, SubjectProjection>(
                subjects => (from subject in subjects
                             where subject.SubjectId == subjectId
                             select new SubjectProjection
                             {
                                 SubjectId = subject.SubjectId,
                                 ClassId = subject.ClassId,
                                 Name = subject.Name,
                                 ClassName = subject.Class.Name,
                                 SelectedClasses = subject.ClassId.ToString()
                             }).FirstOrDefault());
        }

        public IEnumerable<SubjectProjection> GetAllSubjectsByPrefix(string prefix)
        {
            return _repository.Project<Subject, SubjectProjection[]>(
                subjects => (from subject in subjects
                             where subject.Name.StartsWith(prefix)
                             select new SubjectProjection
                             {
                               
                                 Name = subject.Name,
                                
                             }).Distinct().ToArray());
        }
        public IEnumerable<SubjectGridModel> GetSubjectData(out int totalRecords, string Name, int filterClassName,
         int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Subject, IQueryable<SubjectGridModel>>(Subjects => (
                 from s in Subjects
                 select new SubjectGridModel
                 {
                     SubjectId = s.SubjectId,
                     SubjectName = s.Name,
                     ClassName = s.Class.Name,
                     ClassId = s.ClassId,
                     CreatedOn = s.CreatedOn,
                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.SubjectName.Contains(Name));
            }
            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(SubjectGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(SubjectGridModel.ClassName):
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



        public IEnumerable<SubjectGridModel> GetSubjectDataByClientId(out int totalRecords, string Name, int filterClassName,int ClientId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            var query = _repository.Project<Subject, IQueryable<SubjectGridModel>>(Subjects => (
                 from s in Subjects
                 where s.ClientId == ClientId
                 select new SubjectGridModel
                 {
                     SubjectId = s.SubjectId,
                     SubjectName = s.Name,
                     ClassName = s.Class.Name,
                     ClassId = s.ClassId,
                     CreatedOn = s.CreatedOn,
                 })).AsQueryable();

            if (!string.IsNullOrWhiteSpace(Name))
            {
                query = query.Where(p => p.SubjectName.Contains(Name) );
            }
            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName );
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(SubjectGridModel.SubjectName):
                        if (!desc)
                            query = query.OrderBy(p => p.SubjectName);
                        else
                            query = query.OrderByDescending(p => p.SubjectName);
                        break;
                    case nameof(SubjectGridModel.ClassName):
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





        public IEnumerable<SubjectProjection> GetSubjectByClassId(int classId)
        {
            return _repository.Project<Subject, SubjectProjection[]>(
                 subjects => (from sub in subjects
                              where sub.ClassId == classId
                              select new SubjectProjection
                              {
                                  SubjectId = sub.SubjectId,
                                  Name = sub.Name
                              }).ToArray());

        }

        public IEnumerable<SubjectProjection> GetSubjectSubjectIds(string selectedSubject)
        {
            var subjectIds = selectedSubject.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Subject, SubjectProjection[]>(
                subjects => (from sub in subjects
                             where subjectIds.Contains(sub.SubjectId)
                             select new SubjectProjection
                             {
                                 Name = sub.Name,
                             }).ToArray());
        }



    }
}
