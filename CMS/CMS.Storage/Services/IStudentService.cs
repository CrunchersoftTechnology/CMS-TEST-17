using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IStudentService
    {
        IEnumerable<StudentProjection> GetStudentsByClassId(int classId);
        IEnumerable<StudentProjection> GetAllStudents();
        bool Save(Student student);
        CMSResult Update(Student student);
        StudentProjection GetStudentById(string studentId);
        decimal GetStudentFeeByUserId(string userId);
        decimal GetTotalFees(string selectedSubject, string selectedYear);
        IEnumerable<StudentProjection> GetStudentsByBranchId(int branchId);
        IEnumerable<StudentProjection> GetAllStudentByPrefix(string prefix);
        IEnumerable<StudentProjection> GetAllPinByPrefix(string prefix);
        IEnumerable<StudentProjection> GetStudentsByClientId(int clientId);
        IEnumerable<StudentProjection> GetStudentsPaymentByClientId(int clientId); 
         IEnumerable<StudentProjection> GetStudentsByBranchAndClassId(int classId, int branchId);

        IEnumerable<StudentProjection> GetStudentsByClientAndClassId(int classId, int clientId);
        StudentProjection GetStudentDetailForAttendance(int punchId, int branchId);
        IEnumerable<StudentProjection> GetClasses();
        IEnumerable<StudentProjection> GetAllStudentParentList();
        IEnumerable<StudentProjection> GetStudentParentByBranchClassBatch(int branchId, string selectedClasses, string selectedBatches);
        int GetStudentsCount();

        int GetStudentsCountByClientId(int ClientId);
        IEnumerable<StudentProjection> GetClassesByMultipleBranchId(string selectedBranch);
        IEnumerable<StudentProjection> GetStudentsByClassBranch(string selectedClasses, string selectedBranch);
        IEnumerable<StudentGridModel> GetData(out int totalRecords, int filterClassName, string filterFirstName, int userRole,
            string filterLastName, int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<StudentProjection> GetStudentsForSendAttendance(int classId,int ClientId, int branchId, int batchId, DateTime date);
        IEnumerable<StudentProjection> GetClassesByBranchId(int branchId);
        IEnumerable<StudentProjection> GetClassesByClientId(int clientId);
        IEnumerable<StudentProjection> GetBranchesTestByClassId(int classId);
        IEnumerable<StudentProjection> GetStudentByBranchClassBatchForTestPaper(string branchId, string selectedClasses, string selectedBatches);
        StudentProjection GetStudentForShowAttendance(string userId);
        IEnumerable<StudentAttendanceProjection> GetStudentDetailForAttendanceList(List<int> sIdList, int branchId);
        bool IsExistAdmission(string email);
        IEnumerable<StudentProjection> GetStudentByClsandBatch(int classId, DateTime attendanceDate, int branchId, int batchId);
        IEnumerable<StudentProjection> GetStudentsByBranchAndClassIdForAttendance(int classId, int branchId, DateTime attendanceDate);
        StudentProjection GetStudentUserIdInstallment(int punchId, string email);
        CMSResult ClearPunchId();
        IEnumerable<StudentProjection> GetStudentBranch();
        IEnumerable<StudentProjection> GetStudentForUploadMarks(int classId, string selectedBatches, string selectedBranches);
        IEnumerable<StudentProjection> GetStudentsAppPlayerIdByClass(int classId);
    }
}
