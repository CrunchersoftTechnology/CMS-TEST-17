using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CMS.Web.CustomAttributes;

namespace CMS.Web.Controllers
{
    [Roles(Common.Constants.AdminRole, Common.Constants.StudentRole, Common.Constants.BranchAdminRole, Common.Constants.ClientAdminRole)]
    public class StudentDashboardController : BaseController
    {
        readonly IInstallmentService _installmentService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IStudentService _studentService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IAttendanceService _attendanceService;
        readonly IApiService _apiService;
        readonly ITeacherService _teacherService;
        readonly IStudentFeedbackService _studentFeedbackService;
        readonly IEmailService _emailService;
        readonly IBatchService _batchService;
        readonly ISubjectService _subjectService;


        public StudentDashboardController(IInstallmentService installmentService, ILogger logger
            , IRepository repository, IStudentService studentService, IAspNetRoles aspNetRolesService,
          IAttendanceService attendanceService, IApiService apiService, ITeacherService teacherService
            , IStudentFeedbackService studentFeedbackService, IEmailService emailService, IBatchService batchService , ISubjectService subjectService)
        {
            _installmentService = installmentService;
            _logger = logger;
            _repository = repository;
            _studentService = studentService;
            _aspNetRolesService = aspNetRolesService;
            _attendanceService = attendanceService;
            _apiService = apiService;
            _teacherService = teacherService;
            _studentFeedbackService = studentFeedbackService;
            _emailService = emailService;
            _batchService = batchService;
            _subjectService = subjectService;
        }

        //[Route("studentLogin")]
        public ActionResult Index()
        {
            var students = _studentService.GetStudentById(User.Identity.GetUserId());
            var viewModel = AutoMapper.Mapper.Map<StudentProjection, StudentViewModel>(students);

            var commaseperatedList = students.SelectedSubjects ?? string.Empty;
            var subjectIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

            var subjects = _repository.LoadList<Subject>(x => subjectIds.Contains(x.SubjectId)).ToList();
            string subject = "";
            foreach (var s in subjects)
            {
                subject += string.Format("{0},", s.Name);
            }

            viewModel.SelectedSubject = subject.TrimEnd(',');

            return View(viewModel);
        }

        public ActionResult FeesDetails()
        {
            var installment = _installmentService.GetStudInstallments(User.Identity.GetUserId()).ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<InstallmentProjection>, InstallmentViewModel[]>(installment);
            return View(viewModelList);
        }

        public ActionResult Attendance()
        {
            var studentProjection = _studentService.GetStudentForShowAttendance(User.Identity.GetUserId());
            ViewBag.classId = studentProjection.ClassId;
            ViewBag.branchId = studentProjection.BranchId;
            ViewBag.studentBatches = studentProjection.BatchId;
            ViewBag.sId = studentProjection.SId.ToString();
            return View();
        }

        public ActionResult GetNews()
        {
            var newsResult = _apiService.GetNews();
            ViewBag.newsResult = JsonConvert.DeserializeObject<List<NewsProjection>>(newsResult).ToList();

            return View();
        }

        public ActionResult GetNotes()
        {
            var getDetails = GetBoardClassSubjectId();
            var subjectList = GetSubjectStudent(getDetails[2]);
            var notesResult = _apiService.GetNotes();
            ViewBag.notesResult = JsonConvert.DeserializeObject<List<NotesProjection>>(notesResult).ToList().Where(x => x.ClassId == Convert.ToInt32(getDetails[0]) && x.BoardId == Convert.ToInt32(getDetails[1]) && subjectList.Contains(x.SubjectId)).ToList();

            return View();
        }

        public ActionResult GetAssignement()
        {
            var getDetails = GetBoardClassSubjectId();
            var subjectList = GetSubjectStudent(getDetails[2]);
            var assignmentResult = _apiService.GetAssignments();
            ViewBag.assignmentResult = JsonConvert.DeserializeObject<List<AssignmentProjection>>(assignmentResult).ToList().
                Where(x => x.ClassId == Convert.ToInt32(getDetails[0]) && x.BoardId == Convert.ToInt32(getDetails[1]) && subjectList.Contains(x.SubjectId)).ToList();

            return View();
        }

        public ActionResult GetTests()
        {
            var getDetails = GetBoardClassSubjectId();
            var subjectList = GetSubjectStudent(getDetails[2]);
            var getTests = _apiService.GetTest();
            ViewBag.testsResult = JsonConvert.DeserializeObject<List<TestProjection>>(getTests).ToList().
                Where(x => x.ClassId == Convert.ToInt32(getDetails[0]) && x.BoardId == Convert.ToInt32(getDetails[1]) && subjectList.Contains(x.SubjectId)).ToList();
            return View();
        }

        public ActionResult StudentFeedback()
        {
            var student = _studentService.GetStudentById(User.Identity.GetUserId());
            ViewBag.BranchId = student.BranchId;
            var studetnName = _studentService.GetStudentById(User.Identity.GetUserId());
            var name = studetnName.FirstName + " " + studetnName.MiddleName + " " + studetnName.LastName;

            return View(new StudentFeedbackViewModel
            {
                Name = name
            });
        }

        [HttpPost]
        public ActionResult StudentFeedback(StudentFeedbackViewModel viewModel)
        {
            var student = _studentService.GetStudentById(User.Identity.GetUserId());
            ViewBag.BranchId = student.BranchId;

            var studetnName = _studentService.GetStudentById(User.Identity.GetUserId());
            var name = studetnName.FirstName + " " + studetnName.MiddleName + " " + studetnName.LastName;

            if (ModelState.IsValid)
            {
                var result = _studentFeedbackService.Save(new StudentFeedback
                {
                    Name = name,
                    Email = viewModel.Email,
                    Contact = viewModel.Contact,
                    Message = viewModel.Message,
                    UserId = viewModel.UserId,
                    Status = "Waiting",
                    Rating = viewModel.Rating
                });
                if (result.Success)
                {
                    var bodySubject = "Student Feedback";
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{BranchName}", studetnName.BranchName);
                    body = body.Replace("{ModuleName}", "Student : " + name + "<br/>" + "Feedback:" + viewModel.Message + "<br/>");
                    body = body.Replace("{BranchAdminEmail}", viewModel.Email);

                    var emailMessage = new MailModel
                    {
                        From = viewModel.Email,
                        Body = body,
                        Subject = bodySubject,
                        To = ConfigurationManager.AppSettings[Common.Constants.AdminEmail]
                    };
                    _emailService.Send(emailMessage);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new StudentFeedbackViewModel();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            viewModel.Name = name;
            return View(viewModel);
        }

        public List<string> GetBoardClassSubjectId()
        {
            List<string> listOfId = new List<string>();
            var students = _studentService.GetStudentById(User.Identity.GetUserId());
            int classId = students.ClassId;
            int boardId = students.BoardId;
            listOfId.Add(classId.ToString());
            listOfId.Add(boardId.ToString());
            listOfId.Add(students.SelectedSubjects);

            return listOfId;
        }

        public List<int> GetSubjectStudent(string subjectList)
        {
            var commaseperatedBatchList = subjectList ?? string.Empty;
            var SubjectIds = commaseperatedBatchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var subjectResult = _subjectService.GetAllSubjects();
            var Subject = (subjectResult).Where(x => subjectList.Contains(x.SubjectId.ToString())).ToList();
           // var subjectList = Batch.Select(x => x.SubjectId).ToList();
            var subjects = Subject.Select(x => x.SubjectId).ToList();
            return subjects;
        }

        [Roles(Common.Constants.StudentRole)]
        public ActionResult DownloadPDFTest(int id)
        {
            var getTests = _apiService.GetTestByUploadId(id);
            var projection = JsonConvert.DeserializeObject<TestProjection>(getTests);
            string path = Path.Combine("http://arpitaweb.crunchersoft.com/PDF/UploadTestsPDF/", projection.FileName);
            return Redirect(path);
        }

        [Roles(Common.Constants.StudentRole)]
        public ActionResult DownloadPDFNotes(int id)
        {
            var getNotes = _apiService.GetNotesByUploadId(id);
            var projection = JsonConvert.DeserializeObject<TestProjection>(getNotes);
            string path = Path.Combine("http://arpitaweb.crunchersoft.com/PDF/UploadNotesPDF/", projection.FileName);
            return Redirect(path);
        }

          [Roles(Common.Constants.StudentRole)]
        public ActionResult DownloadPDFAssignment(int id)
        {
            var getAssignment = _apiService.GetAssignmentByUploadId(id);
            var projection = JsonConvert.DeserializeObject<TestProjection>(getAssignment);
            string path = Path.Combine("http://arpitaweb.crunchersoft.com/PDF/UploadAssignmentsPDF/", projection.FileName);
            return Redirect(path);
        }

    }
}