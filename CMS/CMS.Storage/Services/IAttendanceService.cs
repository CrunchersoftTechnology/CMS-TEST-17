using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IAttendanceService
    {
        CMSResult Save(Attendance attendance);
        CMSResult Update(Attendance oldAttendance);
        AttendanceProjection GetAttendance(int attendanceId);
        IEnumerable<AttendanceProjection> GetAttendance();
        IEnumerable<AttendanceProjection> GetAttendanceByBranchId(int branchId);
        IEnumerable<AttendanceProjection> GetAttendanceByClientId(int clientId);
        bool GetAttendanceExists(int classId, int batchId, DateTime date, int branchId);
        Attendance GetExistingAttendance(int classId, int batchId, DateTime date, int branchId);
        IEnumerable<AttendanceGridModel> GetAttendanceData(out int totalRecords, int filterClassName, int filterSubjectName, DateTime FilterDate, int userId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<AttendanceProjection> GetAttendanceByMultipleIds(string selectedAttendance);
        CMSResult UpdateMultipleAttendance(string selectedAttendance);
        IEnumerable<AttendanceProjection> GetAttendanceByUserId(int classId, int branchId, int studentBatches);
        IEnumerable<AttendanceProjection> GetAttendanceToSendNotification(int branchId, DateTime todayDate);
        IEnumerable<AttendanceGridModel> GetStudentAttendanceData(out int totalRecords, int classId, int branchId, int studentBatches,int sId,
           DateTime FilterDate, string TeacherName, int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        CMSResult UpdateAutoAttendance(Attendance oldAttendance);
    }
}
