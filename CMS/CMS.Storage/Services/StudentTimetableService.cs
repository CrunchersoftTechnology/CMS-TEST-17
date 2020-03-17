using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class StudentTimetableService : IStudentTimetableService
    {
        readonly IRepository _repository;
        public StudentTimetableService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(int ClientId,StudentTimetable studentTimetable)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<StudentTimetable, bool>(studentTimetables => (
                             from p in studentTimetables
                             where p.Description == studentTimetable.Description && p.Category == studentTimetable.Category && p.ClientId==ClientId
                             select p
                         ).Any());
            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Time table '{0}' already exists !", studentTimetable.Description) });
            }
            else
            {
                studentTimetable.ClientId = ClientId;
                _repository.Add(studentTimetable);
                _repository.CommitChanges();
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Time Table added successfully!") });
            }
            return result;
        }

        public IEnumerable<StudentTimetableGridModel> GetStudentExamTimetable(out int totalRecords, int userId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc, int ClientId)
        {
            int BranchId = 0;
            IQueryable<StudentTimetableGridModel> finalList = null;
            List<StudentTimetableGridModel> list = new List<StudentTimetableGridModel>();
            var query = _repository.Project<StudentTimetable, IQueryable<StudentTimetableGridModel>>(studentTimetable => (
                 from n in studentTimetable
                 where n.Category == Common.Enums.TimetableCategory.ExamTimetable && n.ClientId==ClientId
                 select new StudentTimetableGridModel
                 {
                     StudentTimetableId = n.StudentTimetableId,
                     Description = n.Description,
                     CreatedOn = n.CreatedOn,
                     FileName = n.FileName,
                     Category = n.Category,
                     SelectedBranches = n.SelectedBranches,
                     StudentTimetableDate = n.StudentTimetableDate
                 })).AsQueryable();

            if (BranchId != 0)
            {
                foreach (var examTimeTable in query)
                {
                    var selectedBranchList = examTimeTable.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    if (selectedBranchList.Contains(BranchId))
                    {
                        list.Add(examTimeTable);
                    }
                }
                finalList = list.AsQueryable();
            }
            else
            {
                finalList = query;
            }

            totalRecords = finalList.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(StudentTimetableGridModel.Description):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Description);
                        else
                            finalList = finalList.OrderByDescending(p => p.Description);
                        break;
                    case nameof(StudentTimetableGridModel.StudentTimetableDate):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.StudentTimetableDate);
                        else
                            finalList = finalList.OrderByDescending(p => p.StudentTimetableDate);
                        break;
                    case nameof(StudentTimetableGridModel.Category):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Category);
                        else
                            finalList = finalList.OrderByDescending(p => p.Category);
                        break;
                    default:
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.CreatedOn);
                        else
                            finalList = finalList.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }

            if (limitOffset.HasValue)
            {
                finalList = finalList.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }

            return finalList.ToList();
        }

        public StudentTimetableProjection GetStudentTimetableById(int id)
        {
            return _repository.Project<StudentTimetable, StudentTimetableProjection>(
                StudentTimetable => (from n in StudentTimetable
                                     where n.StudentTimetableId == id
                                     select new StudentTimetableProjection
                                     {
                                         Description = n.Description,
                                         CreatedOn = n.CreatedOn,
                                         SelectedBranches = n.SelectedBranches,
                                         SelectedBatches = n.SelectedBatches,
                                         SelectedClasses = n.SelectedClasses,
                                         FileName = n.FileName,
                                         StudentTimetableId = n.StudentTimetableId,
                                         Category = n.Category,
                                         AttachmentDescription = n.AttachmentDescription,
                                         StudentTimetableDate = n.StudentTimetableDate
                                     }).FirstOrDefault());
        }

        public CMSResult Delete(int StudentTimetableId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<StudentTimetable>(m => m.StudentTimetableId == StudentTimetableId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Time Table '{0}' already exists!", model.Description) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Time Table  '{0}' deleted successfully!", model.Description) });
            }
            return result;
        }

        public IEnumerable<StudentTimetableGridModel> GetStudentClassTimetable(out int totalRecords, int userId,
    int? limitOffset, int? limitRowCount, string orderBy, bool desc, int ClientId)
        {
            int BranchId = 0;
            IQueryable<StudentTimetableGridModel> finalList = null;
            List<StudentTimetableGridModel> list = new List<StudentTimetableGridModel>();
            var query = _repository.Project<StudentTimetable, IQueryable<StudentTimetableGridModel>>(studentTimetable => (
                 from n in studentTimetable
                 where n.Category == Common.Enums.TimetableCategory.ClassTimetable && n.ClientId==ClientId
                 select new StudentTimetableGridModel
                 {
                     StudentTimetableId = n.StudentTimetableId,
                     Description = n.Description,
                     CreatedOn = n.CreatedOn,
                     FileName = n.FileName,
                     Category = n.Category,
                     SelectedBranches = n.SelectedBranches,
                     StudentTimetableDate = n.StudentTimetableDate
                 })).AsQueryable();

            if (BranchId != 0)
            {
                foreach (var classTimeTable in query)
                {
                    var selectedBranchList = classTimeTable.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    if (selectedBranchList.Contains(BranchId))
                    {
                        list.Add(classTimeTable);
                    }
                }
                finalList = list.AsQueryable();
            }
            else
            {
                finalList = query;
            }

            totalRecords = finalList.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(StudentTimetableGridModel.Description):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Description);
                        else
                            finalList = finalList.OrderByDescending(p => p.Description);
                        break;
                    case nameof(StudentTimetableGridModel.StudentTimetableDate):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.StudentTimetableDate);
                        else
                            finalList = finalList.OrderByDescending(p => p.StudentTimetableDate);
                        break;
                    case nameof(StudentTimetableGridModel.Category):
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.Category);
                        else
                            finalList = finalList.OrderByDescending(p => p.Category);
                        break;
                    default:
                        if (!desc)
                            finalList = finalList.OrderBy(p => p.CreatedOn);
                        else
                            finalList = finalList.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }


            if (limitOffset.HasValue)
            {
                finalList = finalList.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }


            return finalList.ToList();
        }

        public CMSResult Update(StudentTimetable oldTimeTable)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<StudentTimetable, bool>(timetables =>
            (from timeTable in timetables
             where timeTable.StudentTimetableId != oldTimeTable.StudentTimetableId && timeTable.Description == oldTimeTable.Description && timeTable.Category == oldTimeTable.Category
             select timeTable).Any());

            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Time table already exists!", "") });
            }
            else
            {

                var timetable = _repository.Load<StudentTimetable>(x => x.StudentTimetableId == oldTimeTable.StudentTimetableId);
                timetable.Description = oldTimeTable.Description;
                timetable.AttachmentDescription = oldTimeTable.AttachmentDescription;
                timetable.StudentTimetableDate = oldTimeTable.StudentTimetableDate;
                timetable.FileName = oldTimeTable.FileName;
                _repository.Update(timetable);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Time table successfully updated!", "") });
            }
            return result;
        }

    }
}
