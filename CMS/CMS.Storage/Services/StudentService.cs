using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Helpers;
using CMS.Domain.Storage.Projections;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class StudentService : IStudentService
    {
        readonly IRepository _repository;

        public StudentService(IRepository repository)
        {
            _repository = repository;
        }

        public IEnumerable<StudentProjection> GetAllStudents()
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             orderby s.FirstName
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                 Email = s.User.Email,
                                 MotherName = s.MotherName,
                               //  VANArea = s.VANArea,
                              //   VANFee = s.VANFee,
                                 RollNo=s.RollNo,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetAllStudentByPrefix(string prefix)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from b in students
                             where b.FirstName.StartsWith(prefix)
                             select new StudentProjection
                             {
                                 Name = b.FirstName,

                             }).Distinct().ToArray());
        }
        public IEnumerable<StudentProjection> GetAllPinByPrefix(string prefix)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from b in students
                             where b.Pin.StartsWith(prefix)
                             select new StudentProjection
                             {
                               
                                 Pin = b.Pin
                             }).Distinct().ToArray());
        }
        public IEnumerable<StudentProjection> GetStudentsByClassId(int classId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId
                             orderby s.FirstName
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                 Email = s.User.Email,
                                 MotherName = s.MotherName,
                               //  VANArea = s.VANArea,
                               //  VANFee = s.VANFee,
                                 RollNo=s.RollNo,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name,
                                 DOB = s.DOB
                             }).ToArray());
        }

        [Obsolete] //?????
        public bool Save(Student student)
        {
            return true;
        }

        public CMSResult Update(Student user)
        {
            bool studentContact = false;
            var result = new CMSResult();
            var batch = _repository.Project<Student, bool>(users => (from u in users where u.UserId == user.UserId select u).Any());

            if (!batch)
            {
                result.Results.Add(new Result { Message = "Student not exists." });
            }
            else
            {
                var isPunchExists = _repository.Project<Student, bool>(students => (from s in students where s.PunchId == user.PunchId && s.UserId != user.UserId && s.BranchId == user.BranchId select s).Any());
                if (user.StudentContact != "")
                {
                    studentContact = _repository.Project<Student, bool>(students => (from s in students where s.StudentContact == user.StudentContact && s.UserId != user.UserId select s).Any());
                }
                if (studentContact)
                {
                    result.Results.Add(
                        new Result
                        {
                            IsSuccessful = false,
                            Message = string.Format("student Contact Number already exists!")
                        });
                }
                if (isPunchExists)
                {
                    result.Results.Add(new Result
                    {
                        IsSuccessful = false,
                        Message = "The Punch Id already exists!"
                    });
                }
                if (!studentContact && !isPunchExists)
                {
                    var commaseperatedList = user.SelectedSubject ?? string.Empty;
                    var subjectIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

                    var studentUser = _repository.Load<ApplicationUser>(x => x.Id == user.UserId, x => x.Student);
                    var student = _repository.Load<Student>(x => x.UserId == user.UserId, x => x.Subjects);
                    var subjects = _repository.LoadList<Subject>(x => subjectIds.Contains(x.SubjectId)).ToList();
                    student.Subjects.Clear();
                    student.Subjects.AddRange(subjects);

                    studentUser.Student.BoardId = user.BoardId;
                    studentUser.Student.ClassId = user.ClassId;
                    studentUser.Student.FirstName = user.FirstName;
                    studentUser.Student.MiddleName = user.MiddleName;
                    studentUser.Student.LastName = user.LastName;
                    studentUser.Student.Gender = user.Gender;
                    studentUser.Student.Address = user.Address;
                    studentUser.Student.Pin = user.Pin;
                    studentUser.Student.DOB = user.DOB;
                    studentUser.Student.BloodGroup = user.BloodGroup;
                    studentUser.Student.StudentContact = user.StudentContact;
                    studentUser.Student.ParentContact = user.ParentContact;
                 //   studentUser.Student.PickAndDrop = user.PickAndDrop;
                    studentUser.Student.DOJ = user.DOJ;
                    studentUser.Student.SchoolId = user.SchoolId;
                    studentUser.Student.FinalFees = user.FinalFees;
                    studentUser.Student.IsWhatsApp = user.IsWhatsApp;
                    studentUser.Student.IsActive = user.IsActive;
                    studentUser.Student.SelectedSubject = user.SelectedSubject;
                    studentUser.Student.PunchId = user.PunchId;
                    studentUser.Student.MotherName = user.MotherName;
                 // studentUser.Student.VANArea = user.VANArea;
                //  studentUser.Student.VANFee = user.VANFee;
                    studentUser.Student.SeatNumber = user.SeatNumber;
                    studentUser.Student.BranchId = user.BranchId;
                 // studentUser.Student.PhotoPath = user.PhotoPath;
                    studentUser.Student.TotalFees = user.TotalFees;
                    studentUser.Student.ParentEmailId = user.ParentEmailId;
                  //studentUser.Student.EmergencyContact = user.EmergencyContact;
                    studentUser.Student.RollNo = user.RollNo;
                    studentUser.Student.BatchId = user.BatchId;
                    studentUser.Student.PaymentLists = user.PaymentLists;
                    studentUser.Student.studentAppPlayerId = user.studentAppPlayerId;
                    studentUser.Student.parentAppPlayerId= user.parentAppPlayerId;
                    _repository.Update(studentUser);
                    _repository.Update(student);
                    result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Student '{0} {1}' successfully updated!", user.FirstName, user.LastName) });
                }
            }

            return result;
        }

        public StudentProjection GetStudentById(string studentId)
        {
            return _repository.Project<Student, StudentProjection>(
                students => (from s in students
                             where s.UserId == studentId
                             select new StudentProjection
                             {
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 Address = s.Address,
                                 Pin = s.Pin,
                                 DOB = s.DOB,
                                 BloodGroup = s.BloodGroup,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                              //   PickAndDrop = s.PickAndDrop,
                                 DOJ = s.DOJ,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 Email = s.User.Email,
                                 UserId = s.UserId,
                                 ConfirmEmail = s.User.Email,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                // PhotoPath = s.PhotoPath,
                                 SelectedSubjects = s.SelectedSubject,
                                 BatchId = s.BatchId,
                                 IsActive = s.IsActive,
                                 IsWhatsApp = s.IsWhatsApp,
                                 PunchId = s.PunchId,
                                 MotherName = s.MotherName,
                             //    VANArea = s.VANArea,
                              //   VANFee = s.VANFee,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name,
                                 BranchId = s.BranchId,
                                 BranchName = s.Branch.Name,
                                 ParentEmailId = s.ParentEmailId,
                               //  EmergencyContact = s.EmergencyContact,
                                 RollNo=s.RollNo,
                                 BatchName = s.Batches.Name,
                                 PaymentLists = s.PaymentLists
                             }).FirstOrDefault());
        }

        public decimal GetStudentFeeByUserId(string userId)
        {
            return _repository.Project<Student, decimal>(
                students => (from s in students
                             where s.UserId == userId
                             select s.FinalFees
                             ).FirstOrDefault());

        }

        public decimal GetTotalFees(string selectedSubject, string selectedYear)
        {
            var commaseperatedList = selectedSubject ?? string.Empty;
            // var subjectsIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

            //var subjects = _repository.LoadList<Subject>(x => subjectsIds.Contains(x.SubjectId)).ToList();
            //var subjectId = "";

            //foreach (var s in subjects)
            //{
            //    subjectId += string.Format("{0},", s.SubjectId);
            //}

            // subjectId = subjectId.TrimEnd(',');

            //  var subjectIds = subjectId.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).Distinct();

            var subjectIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).Distinct();
            var subjectFees = _repository.LoadList<MasterFee>(x => subjectIds.Contains(x.SubjectId) && x.Year == selectedYear).ToList();

            if (subjectIds.Count() == subjectFees.Count())
                return subjectFees.Select(x => x.Fee).Sum();
            else
                return 0;
        }

        public IEnumerable<StudentProjection> GetStudentsByBranchId(int branchId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.BranchId == branchId
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                 Email = s.User.Email,
                                 MotherName = s.MotherName,
                              //   VANArea = s.VANArea,
                                // VANFee = s.VANFee,
                                 RollNo=s.RollNo,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsByClientId(int clientId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClientId == clientId
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                 Email = s.User.Email,
                                 MotherName = s.MotherName,
                               //  VANArea = s.VANArea,
                                // VANFee = s.VANFee,
                                 RollNo=s.RollNo,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsPaymentByClientId(int clientId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClientId == clientId
                             select new StudentProjection
                             {
                                 //UserId = s.UserId,
                                // BoardId = s.BoardId,
                                 //ClassId = s.ClassId,
                                // FirstName = s.FirstName,
                                 //MiddleName = s.MiddleName,
                                 //LastName = s.LastName,
                                 PaymentLists=s.PaymentLists,
                                 //StudentContact = s.StudentContact,
                                // ParentContact = s.ParentContact,
                               
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsByBranchAndClassId(int classId, int branchId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId && s.BranchId == branchId
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                 Email = s.User.Email,
                                 MotherName = s.MotherName,
                               //  VANArea = s.VANArea,
                               //  VANFee = s.VANFee,
                                 RollNo=s.RollNo,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name,
                                 DOB = s.DOB
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsByClientAndClassId(int classId, int clientId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId && s.ClientId == clientId
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 BoardId = s.BoardId,
                                 ClassId = s.ClassId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 Gender = s.Gender,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 SchoolId = s.SchoolId,
                                 TotalFees = s.TotalFees,
                                 Discount = s.Discount,
                                 FinalFees = s.FinalFees,
                                 ClassName = s.Class.Name,
                                 BoardName = s.Board.Name,
                                 Email = s.User.Email,
                                 MotherName = s.MotherName,
                               //  VANArea = s.VANArea,
                                // VANFee = s.VANFee,
                                 RollNo=s.RollNo,
                                 SeatNumber = s.SeatNumber,
                                 SchoolName = s.School.Name,
                                 DOB = s.DOB
                             }).ToArray());
        }

        public StudentProjection GetStudentDetailForAttendance(int punchId, int branchId)
        {
            return _repository.Project<Student, StudentProjection>(
                students => (from s in students
                             where s.PunchId == punchId && s.BranchId == branchId && s.IsActive
                             select new StudentProjection
                             {
                                 ClassId = s.ClassId,
                                 SId = s.SId,
                                 SelectedSubjects = s.SelectedSubject,
                                 PunchId = s.PunchId,
                                 BranchId = s.BranchId,
                                 BatchId = s.BatchId
                             }).FirstOrDefault());
        }

        public IEnumerable<StudentProjection> GetClasses()
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where student.IsActive == true
                             select new StudentProjection
                             {
                                 ClassId = student.ClassId,
                                 ClassName = student.Class.Name
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetAllStudentParentList()
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where student.IsActive == true
                             select new StudentProjection
                             {
                                 Email = student.User.Email,
                                 StudentContact = student.StudentContact,
                                 ParentContact = student.ParentContact,
                                 ClassId = student.ClassId,
                                 BranchId = student.BranchId,
                                 Subjects = student.Subjects,
                                 parentAppPlayerId = student.parentAppPlayerId,
                                 studentAppPlayerId = student.studentAppPlayerId,
                                 Name = student.FirstName + " " + student.MiddleName + " " + student.LastName,
                                 SId = student.SId,
                                 BatchId = student.BatchId,
                                 ParentEmailId = student.ParentEmailId,
                                 MiddleName = student.MiddleName,
                                 LastName = student.LastName
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentParentByBranchClassBatch(int branchId, string selectedClasses, string selectedBatches)
        {

            var classIds = selectedClasses.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var batchIds = selectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            var b = _repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId));

            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where student.BranchId == branchId &&
                             classIds.Contains(student.ClassId)
                             select new StudentProjection
                             {
                                 Email = student.User.Email,
                                 StudentContact = student.StudentContact,
                                 ParentContact = student.ParentContact,
                                 //Batches = student.Batches,
                                 ClassId = student.ClassId
                             }).ToArray());
        }

        public int GetStudentsCount()
        {
            return _repository.Project<Student, int>(
              students => (from student in students select student).Count());
        }

        public int GetStudentsCountByClientId(int ClientId)
        {
            return _repository.Project<Student, int>(
              students => (from student in students where student.ClientId== ClientId  select student).Count());
        }


        public IEnumerable<StudentGridModel> GetData(out int totalRecords, int filterClassName, string filterFirstName, int userId,
            string filterLastName, int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            var query = _repository.Project<Student, IQueryable<StudentGridModel>>(students => (
              from s in students
              select new StudentGridModel
              {
                  UserId = s.UserId,
                  FirstName = s.FirstName,
                  LastName = s.LastName,
                  StudentContact = s.StudentContact,
                  ParentContact = s.ParentContact,
                  TotalFees = s.TotalFees,
                  ClassName = s.Class.Name,
                  BoardName = s.Board.Name,
                  Email = s.User.Email,
                  SchoolName = s.School.Name,
              //    PhotoPath = s.PhotoPath,
                  BranchName = s.Branch.Name,
                  DOJ = s.DOJ,
                  Gender = s.Gender,
                  IsActive = s.IsActive,
                  ClassId = s.ClassId,
                  Address = s.Address,
                  pin = s.Pin,
                  DOB = s.DOB,
                //  PickAndDrop = s.PickAndDrop,
                  BloodGroup = s.BloodGroup,
                  SeatNumber = s.SeatNumber,
                //  VANFee = s.VANFee,
               //   VANArea = s.VANArea,
                  Discount = s.Discount,
                  FinalFees = s.FinalFees,
                  PunchId = s.PunchId,
                  RollNo=s.RollNo,
                  IsWhatsApp = s.IsWhatsApp,
                  BranchId = s.BranchId,
                  Createdon = s.CreatedOn,
              })).AsQueryable();

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

            if (filterClassName != 0)
            {
                query = query.Where(p => p.ClassId == filterClassName);
            }

            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(StudentGridModel.RollNo):
                        if (!desc)
                            query = query.OrderBy(p => p.RollNo);
                        else
                            query = query.OrderByDescending(p => p.RollNo);
                        break;
                    case nameof(StudentGridModel.FirstName):
                        if (!desc)
                            query = query.OrderBy(p => p.FirstName);
                        else
                            query = query.OrderByDescending(p => p.FirstName);
                        break;
                    case nameof(StudentGridModel.LastName):
                        if (!desc)
                            query = query.OrderBy(p => p.LastName);
                        else
                            query = query.OrderByDescending(p => p.LastName);
                        break;
                    case nameof(StudentGridModel.DOJ):
                        if (!desc)
                            query = query.OrderBy(p => p.DOJ);
                        else
                            query = query.OrderByDescending(p => p.DOJ);
                        break;
                    case nameof(StudentGridModel.ClassName):
                        if (!desc)
                            query = query.OrderBy(p => p.ClassName);
                        else
                            query = query.OrderByDescending(p => p.ClassName);
                        break;
                    case nameof(StudentGridModel.IsActive):
                        if (!desc)
                            query = query.OrderBy(p => p.IsActive);
                        else
                            query = query.OrderByDescending(p => p.IsActive);
                        break;
                    case nameof(StudentGridModel.TotalFees):
                        if (!desc)
                            query = query.OrderBy(p => p.TotalFees);
                        else
                            query = query.OrderByDescending(p => p.TotalFees);
                        break;
                    case nameof(StudentGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(StudentGridModel.BoardName):
                        if (!desc)
                            query = query.OrderBy(p => p.BoardName);
                        else
                            query = query.OrderByDescending(p => p.BoardName);
                        break;
                    case nameof(StudentGridModel.FinalFees):
                        if (!desc)
                            query = query.OrderBy(p => p.FinalFees);
                        else
                            query = query.OrderByDescending(p => p.FinalFees);
                        break;
                    case nameof(StudentGridModel.SchoolName):
                        if (!desc)
                            query = query.OrderBy(p => p.SchoolName);
                        else
                            query = query.OrderByDescending(p => p.SchoolName);
                        break;
                    case nameof(StudentGridModel.PunchId):
                        if (!desc)
                            query = query.OrderBy(p => p.PunchId);
                        else
                            query = query.OrderByDescending(p => p.PunchId);
                        break;
                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.Createdon);
                        else
                            query = query.OrderByDescending(p => p.Createdon);
                        break;
                }
            }


            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }

            return query.ToList();
        }

        public IEnumerable<StudentProjection> GetClassesByMultipleBranchId(string selectedBranch)
        {
            var branchIds = selectedBranch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where branchIds.Contains(s.BranchId) && s.IsActive == true
                             select new StudentProjection
                             {
                                 ClassId = s.ClassId,
                                 ClassName = s.Class.Name,
                                 //Batches = s.Batches
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsByClassBranch(string selectedClasses, string selectedBranch)
        {
            var classIds = selectedClasses.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var branchIds = selectedBranch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where classIds.Contains(student.ClassId) && student.IsActive == true &&
                             branchIds.Contains(student.BranchId)
                             select new StudentProjection
                             {
                                 ClassId = student.ClassId,
                                 BranchId = student.BranchId,
                                 BatchId = student.BatchId,
                                 ClassName = student.Class.Name

                                 //Batches = student.Subjects,
                                 // SelectedBatches = student.SelectedBatches
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsForSendAttendance(int classId, int ClientId,int branchId, int batchId, DateTime date)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId
                             && s.ClientId==ClientId
                             && s.BranchId == branchId
                             && s.IsActive
                             && s.BatchId == batchId
                             select new StudentProjection
                             {
                                 SId = s.SId,
                                 StudentContact = s.StudentContact,
                                 ParentContact = s.ParentContact,
                                 Email = s.User.Email,
                                 parentAppPlayerId = s.parentAppPlayerId,
                                 //Batches = s.Batches,
                                 DOJ = s.DOJ,
                                 Name = s.FirstName + " " + s.MiddleName + " " + s.LastName,
                                 BranchName = s.Branch.Name,
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetClassesByBranchId(int branchId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where student.IsActive == true && student.BranchId == branchId
                             select new StudentProjection
                             {
                                 ClassId = student.ClassId,
                                 ClassName = student.Class.Name,
                                 BranchId = student.BranchId
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetClassesByClientId(int clientId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where student.IsActive == true && student.ClientId == clientId
                             select new StudentProjection
                             {
                                 ClassId = student.ClassId,
                                 ClassName = student.Class.Name,
                                 ClientId = student.ClientId
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentByClsandBatch(int classId, DateTime attendanceDate, int branchId, int batchId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId && s.IsActive == true &&
                             s.BatchId == batchId && s.BranchId == branchId && s.DOJ < attendanceDate
                             orderby s.FirstName
                             select new StudentProjection
                             {
                                 SId = s.SId,
                                 FirstName = s.FirstName,
                                 MiddleName = s.MiddleName,
                                 LastName = s.LastName,
                                 BatchId = s.BatchId,
                                 BranchId = s.BranchId,
                                 DOJ = s.DOJ
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetBranchesTestByClassId(int classId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId
                             select new StudentProjection
                             {
                                 BranchId = s.BranchId,
                                 BranchName = s.Branch.Name
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentByBranchClassBatchForTestPaper(string selectedBranch, string selectedClasses, string selectedBatches)
        {
            var classIds = selectedClasses.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

            var branchIds = selectedBranch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where branchIds.Contains(student.BranchId) &&
                                   classIds.Contains(student.ClassId) &&
                                   student.IsActive == true
                             select new StudentProjection
                             {
                                 Email = student.User.Email,
                                 StudentContact = student.StudentContact,
                                 ParentContact = student.ParentContact,
                                 BatchId = student.BatchId,
                                 ClassId = student.ClassId,
                                 studentAppPlayerId = student.studentAppPlayerId,
                                 parentAppPlayerId = student.parentAppPlayerId,
                                 Name = student.FirstName + " " + student.MiddleName + " " + student.LastName,
                                 BatchName = student.Batches.Name,
                                 SId = student.SId
                             }).ToArray());
        }

        public StudentProjection GetStudentForShowAttendance(string userId)
        {
            return _repository.Project<Student, StudentProjection>(
                 student => (from s in student
                             where s.UserId == userId
                             select new StudentProjection
                             {
                                 SId = s.SId,
                                 ClassName = s.Class.Name,
                                 ClassId = s.ClassId,
                                 BranchId = s.BranchId,
                                 BatchId = s.BatchId
                             }).FirstOrDefault());
        }

        public IEnumerable<StudentAttendanceProjection> GetStudentDetailForAttendanceList(List<int> sIdList, int branchId)
        {
            return _repository.Project<Student, StudentAttendanceProjection[]>(
                students => (from s in students
                             where sIdList.Contains(s.PunchId) && s.BranchId == branchId && s.IsActive
                             select new StudentAttendanceProjection
                             {
                                 ClassId = s.ClassId,
                                 SId = s.SId,
                                 BatchId = s.BatchId,
                                 PunchId = s.PunchId,
                                 BranchId = s.BranchId,
                                 InTime = s.Batches.InTime,
                                 OutTime = s.Batches.OutTime,
                                 parentAppPlayerId = s.parentAppPlayerId,
                                 studentAppPlayerId = s.studentAppPlayerId,
                                 StudentName = s.FirstName + " " + s.LastName
                             }).ToArray());
        }

        public bool IsExistAdmission(string email)
        {
            return _repository.Project<Student, bool>(
                student => (from s in student
                            where s.User.Email == email
                            select s)
                            .Any());
        }

        public IEnumerable<StudentProjection> GetStudentsByBranchAndClassIdForAttendance(int classId, int branchId, DateTime attendanceDate)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId && s.IsActive && s.DOJ <= attendanceDate && s.BranchId == branchId
                             select new StudentProjection
                             {
                                 UserId = s.UserId,
                                 Email = s.User.Email,
                                 parentAppPlayerId = s.parentAppPlayerId,
                                 SId = s.SId,
                                 BatchId = s.BatchId,
                                 BatchName = s.Batches.Name,
                                 ClassId = s.ClassId,
                                 BranchId = s.BranchId
                             }).ToArray());
        }

        public StudentProjection GetStudentUserIdInstallment(int punchId, string email)
        {
            return _repository.Project<Student, StudentProjection>(
                students => (from s in students
                             where s.PunchId == punchId && s.User.Email == email
                             select new StudentProjection
                             {
                                 UserId = s.UserId
                             }).FirstOrDefault());
        }

        public CMSResult ClearPunchId()
        {
            CMSResult cmsresult = new CMSResult();
            var result = new Result();

            var studentList = _repository.LoadAll<Student>(x => x.User).ToList();

            studentList.ForEach(x => x.PunchId = 0);
            result.IsSuccessful = true;
            result.Message = string.Format("Punch Id Clear successfully!");
            cmsresult.Results.Add(result);
            return cmsresult;
        }

        public IEnumerable<StudentProjection> GetStudentBranch()
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.IsActive == true
                             select new StudentProjection
                             {
                                 BranchId = s.BranchId,
                                 Name = s.Branch.Name,
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentForUploadMarks(int classId, string selectedBatches, string selectedBranches)
        {
            var batchIds = selectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

            var branchIds = selectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            return _repository.Project<Student, StudentProjection[]>(
                students => (from student in students
                             where branchIds.Contains(student.BranchId) &&
                                   batchIds.Contains(student.BatchId) &&
                                   classId == student.ClassId &&
                                   student.IsActive == true
                             select new StudentProjection
                             {
                                 FirstName = student.FirstName,
                                 MiddleName = student.MiddleName,
                                 LastName = student.LastName,
                                 SId = student.SId,
                                 Subjects = student.Subjects,
                                 UserId = student.UserId,
                                 StudentContact = student.StudentContact,
                                 Email = student.User.Email,
                                 parentAppPlayerId = student.parentAppPlayerId
                             }).ToArray());
        }

        public IEnumerable<StudentProjection> GetStudentsAppPlayerIdByClass(int classId)
        {
            return _repository.Project<Student, StudentProjection[]>(
                students => (from s in students
                             where s.ClassId == classId
                             && s.studentAppPlayerId != null
                             select new StudentProjection
                             {
                                 studentAppPlayerId = s.studentAppPlayerId
                             }).ToArray());
        }
    }
}
