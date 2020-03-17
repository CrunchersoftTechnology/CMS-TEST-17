using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class AttendanceService : IAttendanceService
    {
        readonly IRepository _repository;

        public AttendanceService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(Attendance attendance)
        {
            var result = new CMSResult();
            var isExists = _repository.Project<Attendance, bool>(attendances =>
                                (from a in attendances
                                 where a.ClassId == attendance.ClassId
                                 && a.BatchId == attendance.BatchId
                                 && a.Date == attendance.Date
                                 && a.BranchId == attendance.BranchId
                                  && a.ClientId == attendance.ClientId
                                 select a).Any());

            if (isExists)
            {
                result.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = string.Format("Attendance already exists!")
                });
            }
            else
            {
                attendance.ClientId = attendance.ClientId;
                _repository.Add(attendance);
                result.Results.Add(new Result
                {
                    IsSuccessful = true,
                    Message = string.Format("Attendance successfully added!")
                });
            }
            return result;
        }

        public CMSResult Update(Attendance oldAttendance)
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();
            var isExists = _repository.Project<Attendance, bool>(
                            attedances => (from a in attedances
                                           where a.AttendanceId != oldAttendance.AttendanceId
                                           && a.ClassId == oldAttendance.ClassId
                                           && a.BatchId == oldAttendance.BatchId
                                           && a.Date == oldAttendance.Date
                                           && a.BranchId == oldAttendance.BranchId
                                           select a).Any());

            if (isExists)
            {
                result.IsSuccessful = false;
                result.Message = "Attendance not exists!";
            }
            else
            {
                var attendance = _repository.Load<Attendance>(x => x.AttendanceId == oldAttendance.AttendanceId);
                if (attendance == null)
                {
                    result.IsSuccessful = false;
                    result.Message = "Attendance not exists!";
                }
                else
                {
                    attendance.ClassId = oldAttendance.ClassId;
                    attendance.BatchId = oldAttendance.BatchId;
                    attendance.UserId = oldAttendance.UserId;
                    attendance.Activity = oldAttendance.Activity;
                    attendance.Date = oldAttendance.Date;
                    attendance.StudentAttendence = oldAttendance.StudentAttendence;
                    attendance.BranchId = oldAttendance.BranchId;
                    attendance.IsManual = oldAttendance.IsManual;
                    _repository.Update(attendance);
                    result.IsSuccessful = true;
                    result.Message = string.Format("Attendance updated successfully!");
                }

            }
            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public CMSResult UpdateAutoAttendance(Attendance oldAttendance)
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();
            var isExists = _repository.Project<Attendance, bool>(
                            attedances => (from a in attedances
                                           where a.AttendanceId == oldAttendance.AttendanceId
                                           && a.ClassId == oldAttendance.ClassId
                                           && a.BatchId == oldAttendance.BatchId
                                           && a.Date == oldAttendance.Date
                                           && a.BranchId == oldAttendance.BranchId
                                           select a).Any());

            if (isExists)
            {
                var attendance = _repository.Load<Attendance>(x => x.AttendanceId == oldAttendance.AttendanceId);
                if (attendance == null)
                {
                    result.IsSuccessful = false;
                    result.Message = "Attendance not exists!";
                }
                else
                {
                    attendance.ClassId = oldAttendance.ClassId;
                    attendance.BatchId = oldAttendance.BatchId;
                    attendance.UserId = oldAttendance.UserId;
                    attendance.Activity = oldAttendance.Activity;
                    attendance.Date = oldAttendance.Date;
                    attendance.StudentAttendence = oldAttendance.StudentAttendence;
                    attendance.BranchId = oldAttendance.BranchId;
                    attendance.IsManual = oldAttendance.IsManual;
                    attendance.InTime = oldAttendance.InTime;
                    attendance.OutTime = oldAttendance.OutTime;
                    _repository.Update(attendance);
                    result.IsSuccessful = true;
                    result.Message = string.Format("Attendance updated successfully!");
                }

            }
            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public AttendanceProjection GetAttendance(int attendanceId)
        {
            return _repository.Project<Attendance, AttendanceProjection>(
                attendances => (from a in attendances
                                where a.AttendanceId == attendanceId
                                select new AttendanceProjection
                                {
                                    AttendanceId = a.AttendanceId,
                                    ClassId = a.ClassId,
                                    BatchId = a.BatchId,
                                    Date = a.Date,
                                    Activity = a.Activity,
                                    UserId = a.UserId,
                                    StudentAttendence = a.StudentAttendence,
                                    BranchId = a.BranchId,
                                    BranchName = a.Branch.Name,
                                    InTime = a.InTime,
                                    OutTime = a.OutTime,
                                    ClassName = a.Class.Name,
                                    BatchName = a.Batch.Name
                                }).FirstOrDefault());
        }

        public IEnumerable<AttendanceProjection> GetAttendance()
        {
            return _repository.Project<Attendance, AttendanceProjection[]>(
                attendances => (from a in attendances
                                orderby a.Date descending
                                select new AttendanceProjection
                                {
                                    AttendanceId = a.AttendanceId,
                                    ClassName = a.Class.Name,
                                    BatchName = a.Batch.Name,
                                    Date = a.Date,
                                    TeacherName = a.Teacher.FirstName + " " + a.Teacher.MiddleName + " " + a.Teacher.LastName,
                                    Activity = a.Activity,
                                    //  SubjectName = a.Batch.Subject.Name,
                                    BranchName = a.Branch.Name,
                                }).ToArray());
        }

        public IEnumerable<AttendanceProjection> GetAttendanceByBranchId(int branchId)
        {
            return _repository.Project<Attendance, AttendanceProjection[]>(
                attendances => (from a in attendances
                                where a.BranchId == branchId
                                orderby a.Date descending
                                select new AttendanceProjection
                                {
                                    AttendanceId = a.AttendanceId,
                                    ClassName = a.Class.Name,
                                    BatchName = a.Batch.Name,
                                    Date = a.Date,
                                    TeacherName = a.Teacher.FirstName + " " + a.Teacher.MiddleName + " " + a.Teacher.LastName,
                                    Activity = a.Activity,
                                    // SubjectName = a.Batch.Subject.Name,
                                    BranchName = a.Branch.Name
                                }).ToArray());
        }

        public IEnumerable<AttendanceProjection> GetAttendanceByClientId(int clientId)
        {
            return _repository.Project<Attendance, AttendanceProjection[]>(
                attendances => (from a in attendances
                                where a.ClientId == clientId
                                orderby a.Date descending
                                select new AttendanceProjection
                                {
                                    AttendanceId = a.AttendanceId,
                                    ClassName = a.Class.Name,
                                    BatchName = a.Batch.Name,
                                    Date = a.Date,
                                    TeacherName = a.Teacher.FirstName + " " + a.Teacher.MiddleName + " " + a.Teacher.LastName,
                                    Activity = a.Activity,
                                    // SubjectName = a.Batch.Subject.Name,
                                    ClientName = a.Client.Name
                                }).ToArray());
        }

        public bool GetAttendanceExists(int classId, int batchId, DateTime date, int branchId)
        {
            return _repository.Project<Attendance, bool>(attendances =>
                                (from a in attendances
                                 where a.ClassId == classId
                                 && a.BatchId == batchId
                                 && a.Date == date
                                 && a.BranchId == branchId
                                 select a).Any());
        }

        public Attendance GetExistingAttendance(int classId, int batchId, DateTime date, int branchId)
        {
            return _repository.Load<Attendance>(x =>
                        x.BatchId == batchId &&
                        x.ClassId == classId &&
                         //x.Date.Date == date.Date &&
                         DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date) &&
                        x.BranchId == branchId
                    );
        }

        public IEnumerable<AttendanceGridModel> GetAttendanceData(out int totalRecords, int filterClassName, int filterSubjectName, DateTime FilterDate, int userId,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            var query = _repository.Project<Attendance, IQueryable<AttendanceGridModel>>(machines => (
                 from m in machines
                 select new AttendanceGridModel
                 {
                     AttendanceId = m.AttendanceId,
                     ClassId = m.ClassId,
                     ClassName = m.Class.Name,
                     BranchId = m.BranchId,
                     BranchName = m.Branch.Name,
                     BatchId = m.BatchId,
                     BatchName = m.Batch.Name,
                     Date = m.Date,
                     TeacherName = m.Teacher.FirstName + " " + m.Teacher.LastName,
                     Activity = m.Activity,
                     //  SubjectName = m.Batch.Subject.Name,
                     //  SubjectId = m.Batch.SubjectId,
                     Intime = m.Batch.InTime,
                     Outime = m.Batch.OutTime,
                     IsManual = m.IsManual,
                     CreatedOn = m.CreatedOn,
                     IsSend = m.IsSend

                 })).AsQueryable();

            if (BranchId != 0)
            {
                query = query.Where(p => p.BranchId == BranchId);
            }
            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }
            //if (filterSubjectName != 0)
            //{
            //    query = query.Where(p => p.SubjectId == filterSubjectName);
            //}

            if (FilterDate.Date != null && FilterDate.Date.ToString("dd/MM/yyyy") != "01-01-0001")
            {
                query = query.Where(p => EntityFunctions.TruncateTime(p.Date) == FilterDate.Date);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(AttendanceGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(AttendanceGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(AttendanceGridModel.BatchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BatchName);
                        else
                            query = query.OrderByDescending(p => p.BatchName);
                        break;
                    case nameof(AttendanceGridModel.TeacherName):
                        if (!desc)
                            query = query.OrderBy(p => p.TeacherName);
                        else
                            query = query.OrderByDescending(p => p.TeacherName);
                        break;
                    case nameof(AttendanceGridModel.Date):
                        if (!desc)
                            query = query.OrderBy(p => p.Date);
                        else
                            query = query.OrderByDescending(p => p.Date);
                        break;
                    case nameof(AttendanceGridModel.Activity):
                        if (!desc)
                            query = query.OrderBy(p => p.Activity);
                        else
                            query = query.OrderByDescending(p => p.Activity);
                        break;
                    //case nameof(AttendanceGridModel.SubjectName):
                    //    if (!desc)
                    //        query = query.OrderBy(p => p.SubjectName);
                    //    else
                    //        query = query.OrderByDescending(p => p.SubjectName);
                    //    break;
                    case nameof(AttendanceGridModel.IsSend):
                        if (!desc)
                            query = query.OrderBy(p => p.IsSend);
                        else
                            query = query.OrderByDescending(p => p.IsSend);
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

        public IEnumerable<AttendanceProjection> GetAttendanceByMultipleIds(string selectedAttendance)
        {
            var commaseperatedList = selectedAttendance ?? string.Empty;
            var attendanceIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Attendance, AttendanceProjection[]>(
               attendances => (from a in attendances
                               where attendanceIds.Contains(a.AttendanceId)
                               select new AttendanceProjection
                               {
                                   AttendanceId = a.AttendanceId,
                                   ClassId = a.ClassId,
                                   BatchId = a.BatchId,
                                   BranchId = a.BranchId,
                                   StudentAttendence = a.StudentAttendence,
                                   Date = a.Date,
                                   BatchName = a.Batch.Name,
                                   // SubjectName = a.Batch.Subject.Name
                               }).ToArray());
        }

        public CMSResult UpdateMultipleAttendance(string selectedAttendance)
        {
            var commaseperatedList = selectedAttendance ?? string.Empty;
            var attendanceIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            CMSResult cmsresult = new CMSResult();
            var result = new Result();

            var attendance = _repository.LoadList<Attendance>(x => attendanceIds.Contains(x.AttendanceId)).ToList();
            if (attendance == null)
            {
                result.IsSuccessful = false;
                result.Message = "Attendance not exists!";
            }
            else
            {
                attendance.ForEach(x => x.IsSend = true);
                result.IsSuccessful = true;
                result.Message = string.Format("Attendance updated successfully!");
            }

            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public IEnumerable<AttendanceProjection> GetAttendanceByUserId(int classId, int branchId, int studentBatches)
        {
            // var batchIds = selectedBatch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            return _repository.Project<Attendance, AttendanceProjection[]>(
                attendance => (from a in attendance
                               where a.ClassId == classId && a.BranchId == branchId && a.BatchId == studentBatches
                               select new AttendanceProjection
                               {
                                   ClassName = a.Class.Name,
                                   BranchName = a.Branch.Name,
                                   BatchName = a.Batch.Name,
                                   Date = a.Date,
                                   TeacherName = a.Teacher.FirstName + " " + a.Teacher.LastName,
                                   Activity = a.Activity,
                                   // SubjectName = a.Batch.Subject.Name,
                                   StudentAttendence = a.StudentAttendence
                               }).ToArray());
        }

        public IEnumerable<AttendanceProjection> GetAttendanceToSendNotification(int branchId, DateTime todayDate)
        {
            return _repository.Project<Attendance, AttendanceProjection[]>(
                attendances => (from a in attendances
                                where a.BranchId == branchId
                                && DbFunctions.TruncateTime(a.Date) == DbFunctions.TruncateTime(todayDate)
                                && a.IsManual == false
                                select new AttendanceProjection
                                {
                                    AttendanceId = a.AttendanceId,
                                    ClassId = a.ClassId,
                                    BatchId = a.BatchId,
                                    Date = a.Date,
                                    Activity = a.Activity,
                                    UserId = a.UserId,
                                    StudentAttendence = a.StudentAttendence,
                                    BranchId = a.BranchId,
                                    BranchName = a.Branch.Name
                                }).ToArray());
        }

        public IEnumerable<AttendanceGridModel> GetStudentAttendanceData(out int totalRecords, int classId, int branchId, int studentBatches, int sId,
           DateTime FilterDate, string TeacherName, int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            IQueryable<AttendanceGridModel> query;
            List<AttendanceGridModel> queryList = new List<AttendanceGridModel>();
            var query1 = _repository.Project<Attendance, IQueryable<AttendanceGridModel>>(machines => (
                 from m in machines
                 select new AttendanceGridModel
                 {
                     ClassId = m.ClassId,
                     ClassName = m.Class.Name,
                     BranchId = m.BranchId,
                     BranchName = m.Branch.Name,
                     BatchId = m.BatchId,
                     BatchName = m.Batch.Name,
                     Date = m.Date,
                     TeacherName = m.Teacher.FirstName + " " + m.Teacher.LastName,
                     Activity = m.Activity,
                     StudentAttendence = m.StudentAttendence
                 })).AsQueryable();

            foreach (var attendances in query1)
            {
                var list = attendances.StudentAttendence.Split(',').Select(int.Parse);
                if (list.Contains(sId))
                {
                    attendances.StudentAttendence = "Present";
                }
                else
                {
                    attendances.StudentAttendence = "Absent";
                }
                queryList.Add(new AttendanceGridModel
                {
                    ClassId = attendances.ClassId,
                    ClassName = attendances.ClassName,
                    BranchId = attendances.BranchId,
                    BranchName = attendances.BranchName,
                    BatchId = attendances.BatchId,
                    BatchName = attendances.BatchName,
                    Date = attendances.Date,
                    TeacherName = attendances.TeacherName,
                    Activity = attendances.Activity,
                    StudentAttendence = attendances.StudentAttendence
                });
            }
            query = queryList.AsQueryable();

            if (classId != 0)
            {
                query = query.Where(p => p.ClassId == classId);
            }
            if (branchId != 0)
            {
                query = query.Where(p => p.BranchId == branchId);
            }
            if (studentBatches != 0)
            {
                query = query.Where(p => p.BatchId == studentBatches);
            }
            if (TeacherName != null)
            {
                query = query.Where(p => p.TeacherName == TeacherName);
            }
            if (FilterDate.Date != null && FilterDate.Date.ToString("dd-MM-yyyy") != "01-01-0001")
            {
                query = query.Where(p => p.Date == FilterDate.Date);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(AttendanceGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(AttendanceGridModel.Activity):
                        if (!desc)
                            query = query.OrderBy(p => p.Activity);
                        else
                            query = query.OrderByDescending(p => p.Activity);
                        break;
                    case nameof(AttendanceGridModel.TeacherName):
                        if (!desc)
                            query = query.OrderBy(p => p.TeacherName);
                        else
                            query = query.OrderByDescending(p => p.TeacherName);
                        break;
                    case nameof(AttendanceGridModel.StudentAttendence):
                        if (!desc)
                            query = query.OrderBy(p => p.StudentAttendence);
                        else
                            query = query.OrderByDescending(p => p.StudentAttendence);
                        break;
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.Date);
                        else
                            query = query.OrderByDescending(p => p.Date);
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
