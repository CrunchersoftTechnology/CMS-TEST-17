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
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    public class UploadOfflineMarksController : BaseController
    {
        readonly IBranchService _branchService;
        readonly IClassService _classService;
        readonly IBatchService _batchService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IStudentService _studentService;
        readonly ISubjectService _subjectService;
        readonly IRepository _repository;
        readonly ILogger _logger;
        readonly ISmsService _smsService;
        readonly IOfflineTestPaper _offlineTestPaper;
        readonly IOfflineTestStudentMarksService _offlineTestStudentMarksService;
        readonly ISendNotificationService _sendNotificationService;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureServices;

        public UploadOfflineMarksController(IClientAdminService clientAdminService, IBranchService branchService, IClassService classService, IBatchService batchService,
                IEmailService emailService, IAspNetRoles aspNetRolesService, IConfigureServices configureServices,
                IBranchAdminService branchAdminService, IStudentService studentService,
                ISubjectService subjectService, IRepository repository, ILogger logger,
                ISmsService smsService, IOfflineTestPaper offlineTestPaper,
                IOfflineTestStudentMarksService offlineTestStudentMarks,
                ISendNotificationService sendNotificationService)
        {
            _branchService = branchService;
            _classService = classService;
            _batchService = batchService;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _studentService = studentService;
            _subjectService = subjectService;
            _logger = logger;
            _repository = repository;
            _smsService = smsService;
            _offlineTestPaper = offlineTestPaper;
            _offlineTestStudentMarksService = offlineTestStudentMarks;
            _sendNotificationService = sendNotificationService;
            _clientAdminService = clientAdminService;
            _configureServices = configureServices;
        }
        // GET: UploadMarks
        public ActionResult Index()
        {
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(User.Identity.GetUserId()) : null;
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(User.Identity.GetUserId()) : null;
            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else if (roles == "Client")
            {
                ViewBag.userId = projectionClient.ClientId;

            }
            else
            {
                ViewBag.userId = projection.BranchId;
            }
            return View();
        }

        public ActionResult Create()
        {
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(User.Identity.GetUserId()) : null;
           // var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            var papers = _offlineTestPaper.GetOfflineTestPaper().ToList();
            if (roles == "Admin")
            {
                var selectedBranch = papers.Select(x => x.SelectedBranches).ToList();
                var branchCommaSepratedList = string.Join(",", selectedBranch).Split(',').Select(x => int.Parse(x)).ToList().Distinct();
                var branchLists = _branchService.GetAllBranches().Where(x => branchCommaSepratedList.Contains(x.BranchId));

                var branchList = (from b in branchLists
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                ViewBag.BranchId = 0;
                ViewBag.CurrentUserRole = roles;

                return View(new UploadOfflineMarksViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles,
                    Papers = new SelectList(papers, "OfflineTestPaperId", "Title")
                });
            }
            else if (roles == "Client")
            {
                var selectedBranch = papers.Select(x => x.SelectedBranches).ToList();
                var branchCommaSepratedList = string.Join(",", selectedBranch).Split(',').Select(x => int.Parse(x)).ToList().Distinct();
                var branchLists = _branchService.GetAllBranchesByClientId(projectionClient.ClientId).Where(x => branchCommaSepratedList.Contains(x.BranchId));

                var branchList = (from b in branchLists
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                var examScedule = _clientAdminService.GetClientAdminById(User.Identity.GetUserId());
                papers = _offlineTestPaper.GetOfflineTestPaper().ToList().Where(y => (y.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse)).Contains(examScedule.ClientId)).ToList();

                var projection = _clientAdminService.GetClientAdminById(User.Identity.GetUserId());
                ViewBag.ClientId = projection.ClientId;
                var classList = (from c in _studentService.GetStudentsByClientId(projection.ClientId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();

                return View(new UploadOfflineMarksViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName,
                    Classes = classList,
                    Branches = branchList,
                    Papers = new SelectList(papers, "OfflineTestPaperId", "Title")
                });
            }
            else if (roles == "BranchAdmin")
            {
                var examScedule = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                papers = _offlineTestPaper.GetOfflineTestPaper().ToList().Where(y => (y.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse)).Contains(examScedule.BranchId)).ToList();

                var projection = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                ViewBag.BranchId = projection.BranchId;
                var classList = (from c in _studentService.GetStudentsByBranchId(projection.BranchId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();
                ViewBag.CurrentUserRole = roles;
                return View(new UploadOfflineMarksViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projection.BranchId,
                    BranchName = projection.BranchName,
                    Classes = classList,
                    SelectedBranches = projection.BranchId.ToString(),
                    Papers = new SelectList(papers, "OfflineTestPaperId", "Title")
                });
            }
            return View();
        }

        public JsonResult GetStudent(int offlineTestPaperId, string selectedBatches, string selectedBranches)
        {
            List<StudentProjection> studentsLists = new List<StudentProjection>();
            List<StudentProjection> studentBatchWiseList = new List<StudentProjection>();
            var offlineTest = _offlineTestPaper.GetOfflineTestById(offlineTestPaperId);
            var studentsList = _studentService.GetStudentForUploadMarks(offlineTest.ClassId, selectedBatches, selectedBranches).ToList();
            List<StudentProjection> students = new List<StudentProjection>();
            if (offlineTest.SubjectId != 0)
            {
                foreach (var student in studentsList)
                {
                    foreach (var subjecttest in student.Subjects)
                    {
                        if (subjecttest.SubjectId == offlineTest.SubjectId)
                            students.Add(student);
                    }
                }
            }

            if (students != null && students.Count > 0)
                studentsLists.AddRange(students);
            else
                studentsLists.AddRange(studentsList.ToList());

            foreach (var student in studentsLists)
            {
                foreach (var subjects in student.Subjects)
                {
                    if (offlineTest.SubjectId == subjects.SubjectId)
                    {
                        studentBatchWiseList.Add(new StudentProjection
                        {
                            FirstName = student.FirstName,
                            MiddleName = student.MiddleName,
                            LastName = student.LastName,
                            UserId = student.UserId,
                            Email = student.Email,
                            StudentContact = student.StudentContact,
                            parentAppPlayerId = student.parentAppPlayerId
                        });
                    }
                }
            }
            var studentLists = AutoMapper.Mapper.Map<List<StudentProjection>, StudentViewModel[]>(studentBatchWiseList.ToList());
            var result = new
            {
                studentList = studentLists,
                marks = offlineTest.TotalMarks,
                title = offlineTest.Title
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaperByMultipleBranches(string selectedBranches)
        {
            var offlineList = GetPaperList(selectedBranches);
            var offlineLists = (from offlineTest in offlineList
                                group offlineTest by new
                                {
                                    offlineTest.OfflineTestPaperId,
                                    offlineTest.Title

                                } into grouping
                                select new OfflineTestPaperProjection
                                {
                                    Title = grouping.Key.Title,
                                    OfflineTestPaperId = grouping.Key.OfflineTestPaperId
                                });
            var result = new
            {
                paperList = offlineLists
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPaperByMultipleBranchesAndClass(string selectedBranches, int classId)
        {
            var offlineList = GetPaperList(selectedBranches).Where(x => x.ClassId == classId); ;
            var result = new
            {
                paperList = offlineList.Distinct()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPaperByBranchandClassAndSubjectId(string selectedBranches, int classId, int subjectId)
        {
            var offlineList = GetPaperList(selectedBranches).Where(x => x.ClassId == classId && x.SubjectId == subjectId);
            var result = new
            {
                paperList = offlineList.Distinct()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FillPaperByBranchandClassAndSubjectIdAndBatch(string selectedBranches, int classId, int subjectId, string selectedBatch)
        {
            var offlineList = GetPaperList(selectedBranches).Where(x => x.ClassId == classId && x.SubjectId == subjectId);

            List<OfflineTestPaperProjection> offlineLists = new List<OfflineTestPaperProjection>();
            var batchIds = selectedBatch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

            foreach (var batchchId in batchIds)
            {
                foreach (var paper in offlineList)
                {
                    var batch = paper.SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                    if (batch.Contains(batchchId))
                    {
                        offlineLists.Add(new OfflineTestPaperProjection
                        {
                            OfflineTestPaperId = paper.OfflineTestPaperId,
                            Title = paper.Title
                        });
                    }
                }
            }
            var offlinePaperList = (from offlineTest in offlineLists
                                    group offlineTest by new
                                    {
                                        offlineTest.OfflineTestPaperId,
                                        offlineTest.Title

                                    } into grouping
                                    select new OfflineTestPaperProjection
                                    {
                                        Title = grouping.Key.Title,
                                        OfflineTestPaperId = grouping.Key.OfflineTestPaperId
                                    });
            var result = new
            {
                paperList = offlinePaperList.Distinct()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public IEnumerable<OfflineTestPaperProjection> GetPaperList(string selectedBranches)
        {
            List<OfflineTestPaperProjection> offlineList = new List<OfflineTestPaperProjection>();
            var branchIds = selectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var paperList = _offlineTestStudentMarksService.GetOfflineTest();

            foreach (var branchId in branchIds)
            {
                foreach (var paper in paperList)
                {
                    var branch = paper.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                    if (branch.Contains(branchId))
                    {
                        offlineList.Add(new OfflineTestPaperProjection
                        {
                            OfflineTestPaperId = paper.OfflineTestPaperId,
                            Title = paper.Title,
                            ClassId = paper.ClassId,
                            SelectedBatches = paper.SelectedBatches,
                            SubjectId = paper.SubjectId
                        });
                    }
                }
            }
            return offlineList;
        }

        public ActionResult SaveUploadMarks(UploadOfflineMarksViewModel viewModel)
        {
            var uploadMarksdata = JsonConvert.DeserializeObject<List<UploadOfflineMarksProjection>>(viewModel.StudentOfflineMarks).ToList();
            var cmsResult = new CMSResult();
            if (uploadMarksdata.Count == 0)
            {
                cmsResult.Results.Add(new Result { Message = "Please fill student marks.", IsSuccessful = false });
            }
            else
            {
                var offlineTest = _offlineTestPaper.GetOfflineTestById(viewModel.OfflineTestPaperId);
                if (ModelState.IsValid)
                {
                    var offlineTestStudentMarksList = new List<OfflineTestStudentMarks>();
                    foreach (var uploadMark in uploadMarksdata)
                    {
                        var offlineTestStudentMarks = new OfflineTestStudentMarks
                        {
                            UserId = uploadMark.UserId,
                            OfflineTestPaperId = viewModel.OfflineTestPaperId,
                            ObtainedMarks = uploadMark.ObtainedMarks,
                            Percentage = uploadMark.Percentage,
                            IsPresent = uploadMark.IsPresent
                        };

                        offlineTestStudentMarksList.Add(offlineTestStudentMarks);
                        #region rough
                        //cmsResult = _offlineTestStudentMarksService.Save(offlineTestStudentMarks);
                        //if (cmsResult.Success)
                        //{
                        //    string subject = "Student Test Marks";
                        //    viewModel.StudentEmail = uploadMark.EmailId;
                        //    viewModel.StudentContact = uploadMark.StudentContact;
                        //    viewModel.ObtainedMarks = uploadMark.ObtainedMarks;
                        //    viewModel.Percentage = uploadMark.Percentage;
                        //    viewModel.ClassName = offlineTest.ClassName;
                        //    viewModel.SubjectName = offlineTest.SubjectName;
                        //    var BatchList = _batchService.GetBatchesByBatchIds(offlineTest.SelectedBatches).Select(x => x.BatchName).ToList();
                        //    viewModel.SelectedBatchesName = string.Join(",", BatchList);

                        //    cmsResult = SendNotification(viewModel, subject);
                        //} 
                        #endregion
                    }

                    var offlineTestStudentMarksArray = offlineTestStudentMarksList.ToArray();
                    if (offlineTestStudentMarksList.Count > 0)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => StartProcessingSaveOfflineTestMarks(offlineTestStudentMarksArray, cancellationToken));
                        cmsResult.Results.Add(new Result { Message = "Offline test paper marks added successfully.", IsSuccessful = true });
                        string subject = "Student Test Marks";
                        var result = SendNotification(viewModel, subject);
                        cmsResult.Results.AddRange(result.Results);
                    }

                    #region rough
                    //if (cmsResult.Success)
                    //{
                    //    var messages = "";
                    //    foreach (var message in cmsResult.Results)
                    //    {
                    //        messages += message.Message + "<br />";
                    //    }
                    //    Success(messages);
                    //    ModelState.Clear();
                    //} 
                    #endregion
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(x => x.Errors).ToArray();
                    foreach (var item in errors)
                    {
                        cmsResult.Results.Add(new Result { Message = item.ErrorMessage, IsSuccessful = false });
                    }
                }
            }
            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public CMSResult SendNotification(UploadOfflineMarksViewModel viewModel, string subject)
        {
            var cmsResult = new CMSResult();
            List<StudentProjection> studentsLists = new List<StudentProjection>();
            var studentsList = _studentService.GetAllStudentParentList();
            List<StudentProjection> students = new List<StudentProjection>();
            if (viewModel.SelectedBranches != null)
            {
                var branches = viewModel.SelectedBranches.Split(',').Select(x => int.Parse(x)).ToList();
                studentsList = studentsList.Where(x => branches.Contains(x.BranchId));
            }
            if (viewModel.ClassId != null && viewModel.SubjectId != 0)
            {
                studentsList = studentsList.Where(x => x.ClassId == viewModel.ClassId);
            }
            if (viewModel.SelectedBatches != null)
            {
                var batches = viewModel.SelectedBatches.Split(',').Select(x => int.Parse(x)).ToList();
                studentsList = studentsList.Where(x => batches.Contains(x.BatchId));
            }
            if (viewModel.SubjectId != null && viewModel.SubjectId != 0)
            {
                foreach (var student in studentsList)
                {
                    foreach (var subjecttest in student.Subjects)
                    {
                        if (subjecttest.SubjectId == viewModel.SubjectId)
                            students.Add(student);
                    }
                }
            }

            if (students != null && students.Count > 0)
                studentsLists.AddRange(students);
            else
                studentsLists.AddRange(studentsList.ToList());

            var uploadOfflineMarks = JsonConvert.DeserializeObject<List<UploadOfflineTestMarks>>(viewModel.StudentOfflineMarks);

            if (uploadOfflineMarks.Count > 0)
            {
                var result = SendEmailDynamic(viewModel, subject, uploadOfflineMarks, studentsLists);

                if (result == true)
                    cmsResult.Results.Add(new Result { Message = "Email sent successfully.", IsSuccessful = true });
                else
                    cmsResult.Results.Add(new Result { Message = "Something went wrong to send email.", IsSuccessful = false });
            }

            if (viewModel.SMS)
            {
                var result = SendSMSDynamic(viewModel, studentsLists, uploadOfflineMarks);
                if (result.Success)
                    cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = true });
                else
                    cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = false });

            }
            #region appNotification
            if (viewModel.AppNotification)
            {
                var response = SendAppNotification(viewModel, studentsList.ToList(), uploadOfflineMarks);

                if (response.Success)
                {
                    cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = true });
                }
                else
                {
                    cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = false });
                }
            }
            #endregion

            return cmsResult;
        }

        public bool SendEmail(UploadOfflineMarksViewModel viewModel, string subject, List<UploadOfflineTestMarks> uploadOfflineMarks,
            List<StudentProjection> studentList)
        {
            string userRole = "";
            var emailModels = new List<MailModel>();
            var roleUserId = User.Identity.GetUserId();
            bool isBranchAdmin = false;
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                userRole = branchAdmin.BranchName;
                isBranchAdmin = true;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            }
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }
            if (isBranchAdmin)
            {
                var emailFormat = body;
                emailFormat = emailFormat.Replace("{BranchName}", userRole);
                emailFormat = emailFormat.Replace("{NotificationMessage}", "<br/> Title : " + viewModel.Title + "<br/>Class Name : " + viewModel.ClassName + "<br/>Subject Name : " +
                     viewModel.SubjectName + "<br/>Batch Name : " + viewModel.SelectedBatchesName +
                    "<br/> Total Marks : " + viewModel.TotalMarks);
                emailFormat = emailFormat.Replace("{UserName}", "");


                var emailMessage = new MailModel
                {
                    Body = emailFormat,
                    Subject = subject,
                    To = ConfigurationManager.AppSettings[Common.Constants.FromEmail]
                };
                emailModels.Add(emailMessage);
            }

            foreach (var email in uploadOfflineMarks)
            {
                var studentName = studentList.Where(x => x.Email == email.EmailId).FirstOrDefault().Name;
                var message = "Absent for offline test paper.";
                if (email.IsPresent)
                    message = "Name - " + studentName + "<br />Title - " + viewModel.Title + "<br />Marks - " + email.ObtainedMarks + "/" + viewModel.TotalMarks +
                        "<br />Percentage - " + email.Percentage;

                var emailFormat = body;
                emailFormat = emailFormat.Replace("{BranchName}", userRole);
                emailFormat = emailFormat.Replace("{NotificationMessage}", message);
                emailFormat = emailFormat.Replace("{UserName}", "");

                var emailModel = new MailModel
                {
                    Body = emailFormat,
                    Subject = subject,
                    To = email.EmailId
                };

                emailModels.Add(emailModel);
            }
            var emailModelArray = emailModels.ToArray();
            if (emailModels.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailModelArray, cancellationToken));
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SendEmailDynamic(UploadOfflineMarksViewModel viewModel, string subject, List<UploadOfflineTestMarks> uploadOfflineMarks,
           List<StudentProjection> studentList)
        {
            string userRole = "";
            var emailModels = new List<MailModel>();
            var roleUserId = User.Identity.GetUserId();
            bool isBranchAdmin = false;
            bool isClientAdmin = false;
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin" || roles=="Client")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                userRole = branchAdmin.BranchName;
                isBranchAdmin = true;
                isClientAdmin = true;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            }
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }
            if (isBranchAdmin || isClientAdmin)
            {
                var emailFormat = body;
                emailFormat = emailFormat.Replace("{BranchName}", userRole);
                emailFormat = emailFormat.Replace("{NotificationMessage}", "<br/> Title : " + viewModel.Title + "<br/>Class Name : " + viewModel.ClassName + "<br/>Subject Name : " +
                     viewModel.SubjectName + "<br/>Batch Name : " + viewModel.SelectedBatchesName +
                    "<br/> Total Marks : " + viewModel.TotalMarks);
                emailFormat = emailFormat.Replace("{UserName}", "");


              //  var roleUserId = User.Identity.GetUserId();
               // var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;

                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var emailid = projection.email_id;
                var password = projection.emailpassword;

                var emailMessage = new MailModel
                {
                    Emailid = emailid,
                    Password = password,
                    Body = emailFormat,
                    Subject = subject,
                    To = emailid
                };
                emailModels.Add(emailMessage);
            }

            foreach (var email in uploadOfflineMarks)
            {
                var studentName = studentList.Where(x => x.Email == email.EmailId).FirstOrDefault().Name;
                var message = "Absent for offline test paper.";
                if (email.IsPresent)
                    message = "Name - " + studentName + "<br />Title - " + viewModel.Title + "<br />Marks - " + email.ObtainedMarks + "/" + viewModel.TotalMarks +
                        "<br />Percentage - " + email.Percentage;

                var emailFormat = body;
                emailFormat = emailFormat.Replace("{BranchName}", userRole);
                emailFormat = emailFormat.Replace("{NotificationMessage}", message);
                emailFormat = emailFormat.Replace("{UserName}", "");

                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;

                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var emailid = projection.email_id;
                var password = projection.emailpassword;

                var emailModel = new MailModel
                {
                    Emailid = emailid,
                    Password = password,
                    Body = emailFormat,
                    Subject = subject,
                    To = email.EmailId
                };

                emailModels.Add(emailModel);
            }
            var emailModelArray = emailModels.ToArray();
            if (emailModels.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailModelArray, cancellationToken));
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Edit(int id)
        {
            ViewBag.offlineTestPaperId = id;
            return View();
        }

        public JsonResult GetStudentMarksList(int id)
        {
            var uploadMarksList = _offlineTestStudentMarksService.GetOfflineTestByOfflineTestPaperId(id);
            return Json(uploadMarksList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateUploadMarks(UploadOfflineMarksViewModel viewModel)
        {
            var uploadMarksdata = JsonConvert.DeserializeObject<List<UploadOfflineMarksProjection>>(viewModel.StudentOfflineMarks).ToList();
            var cmsResult = new CMSResult();
            var offlineTest = _offlineTestPaper.GetOfflineTestById(viewModel.OfflineTestPaperId);
            if (ModelState.IsValid)
            {
                var offlineTestStudentMarksList = new List<OfflineTestStudentMarks>();
                foreach (var uploadMark in uploadMarksdata)
                {
                    var offlineTestStudentMarks = new OfflineTestStudentMarks
                    {
                        UserId = uploadMark.UserId,
                        OfflineTestPaperId = viewModel.OfflineTestPaperId,
                        ObtainedMarks = uploadMark.ObtainedMarks,
                        Percentage = uploadMark.Percentage,
                        OfflineTestStudentMarksId = uploadMark.OfflineTestStudentMarksId,
                        IsPresent = uploadMark.IsPresent
                    };

                    offlineTestStudentMarksList.Add(offlineTestStudentMarks);
                    #region rough
                    //cmsResult = _offlineTestStudentMarksService.Update(offlineTestStudentMarks);
                    //if (cmsResult.Success)
                    //{
                    //    string subject = "Student Update Offline Test Marks";
                    //    viewModel.StudentEmail = uploadMark.EmailId;
                    //    viewModel.StudentContact = uploadMark.StudentContact;
                    //    viewModel.ObtainedMarks = uploadMark.ObtainedMarks;
                    //    viewModel.Percentage = uploadMark.Percentage;
                    //    viewModel.ClassName = offlineTest.ClassName;
                    //    viewModel.SubjectName = offlineTest.SubjectName;
                    //    var BatchList = _batchService.GetBatchesByBatchIds(offlineTest.SelectedBatches).Select(x => x.BatchName).ToList();
                    //    viewModel.SelectedBatchesName = string.Join(",", BatchList);
                    //    cmsResult = SendNotification(viewModel, subject);
                    //}
                    //else
                    //{
                    //    cmsResult.Results.Add(new Result { Message = cmsResult.Results.FirstOrDefault().ToString(), IsSuccessful = false });
                    //} 
                    #endregion
                }

                var offlineTestStudentMarksArray = offlineTestStudentMarksList.ToArray();
                if (offlineTestStudentMarksList.Count > 0)
                {
                    HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => StartProcessingUpdateOfflineTestMarks(offlineTestStudentMarksArray, cancellationToken));
                    cmsResult.Results.Add(new Result { Message = "Offline test paper marks updated successfully.", IsSuccessful = true });
                    string subject = "Update Student Test Marks";
                    var result = SendNotification(viewModel, subject);
                }

                #region Rough
                //if (cmsResult.Success)
                //{
                //    var messages = "";
                //    foreach (var message in cmsResult.Results)
                //    {
                //        messages += message.Message + "<br />";
                //    }
                //    Success(messages);
                //    ModelState.Clear();
                //    //viewModel = new UploadOfflineMarksViewModel();
                //}
                //else
                //{
                //    cmsResult.Results.Add(new Result { Message = cmsResult.Results.FirstOrDefault().ToString(), IsSuccessful = false });
                //} 
                #endregion
            }
            else
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).ToArray();
                foreach (var item in errors)
                {
                    cmsResult.Results.Add(new Result { Message = item.ErrorMessage, IsSuccessful = false });
                }
            }

            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id)
        {
            ViewBag.offlineTestPaperId = id;
            return View();
        }

        public ActionResult Delete(int id)
        {
            ViewBag.offlineTestPaperId = id;
            return View();
        }

        public ActionResult DeleteUploadMarks(UploadOfflineMarksViewModel viewModel)
        {
            var uploadMarksdata = JsonConvert.DeserializeObject<List<UploadOfflineMarksProjection>>(viewModel.StudentOfflineMarks).ToList();
            var cmsResult = new CMSResult();

            if (ModelState.IsValid)
            {
                foreach (var uploadMark in uploadMarksdata)
                {
                    cmsResult = _offlineTestStudentMarksService.Delete(uploadMark.OfflineTestStudentMarksId);
                    if (cmsResult.Success)
                    {
                        cmsResult.Results.Add(new Result { Message = cmsResult.Results.FirstOrDefault().ToString(), IsSuccessful = true });
                        ModelState.Clear();
                    }
                    else
                    {
                        cmsResult.Results.Add(new Result { Message = cmsResult.Results.FirstOrDefault().ToString(), IsSuccessful = false });
                    }
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).ToArray();
                foreach (var item in errors)
                {
                    cmsResult.Results.Add(new Result { Message = item.ErrorMessage, IsSuccessful = false });
                }
            }

            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public CMSResult SendAppNotification(UploadOfflineMarksViewModel viewModel, List<StudentProjection> studentList, List<UploadOfflineTestMarks> uploadOfflineTestMarks)
        {
            var cmsResult = new CMSResult();
            var notificationList = new List<SendNotificationByPlayerId>();
            foreach (var playerId in uploadOfflineTestMarks)
            {
                var student = studentList.Where(x => x.Email == playerId.EmailId).FirstOrDefault();
                var message = "Absent for offline test paper."+ "$^$" + student.SId + "@0";
                if (playerId.IsPresent)
                    message = "<br />Name - " + student.Name + "<br />Title - " + viewModel.Title + "<br />Marks - " + playerId.ObtainedMarks + "/" + viewModel.TotalMarks +
                        "<br />Percentage - " + playerId.Percentage + "$^$" + student.SId + "@0";

                if (student.parentAppPlayerId != null && student.parentAppPlayerId != "")
                {
                    var sendParentAppNotification = new SendNotificationByPlayerId
                    {
                        Message = "Marks-" + message,
                        PlayerIds = student.parentAppPlayerId,
                        AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                        RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                    };
                    notificationList.Add(sendParentAppNotification);
                }

                if (student.studentAppPlayerId != null && student.studentAppPlayerId != "")
                {
                    var sendStudentAppNotification = new SendNotificationByPlayerId
                    {
                        Message = "Marks-" + message,
                        PlayerIds = student.studentAppPlayerId,
                        AppIds = ConfigurationManager.AppSettings[Common.Constants.StudentAppId],
                        RestApiKey = ConfigurationManager.AppSettings[Common.Constants.StudentRestAppId]
                    };
                    notificationList.Add(sendStudentAppNotification);
                }
            }
            var notification = notificationList.ToArray();
            if (notificationList.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
                cmsResult.Results.Add(new Result { Message = "App Notification sent successfully.", IsSuccessful = true });
            }
            else
            {
                cmsResult.Results.Add(new Result { Message = "No one is registered in parent app.", IsSuccessful = true });
            }
            return cmsResult;
        }

        public CMSResult SendSMS(UploadOfflineMarksViewModel viewModel, List<StudentProjection> studentList, List<UploadOfflineTestMarks> uploadOfflineTestMarks)
        {
            var cmsResult = new CMSResult();
            var smsList = new List<SmsModel>();
            foreach (var sms in uploadOfflineTestMarks)
            {
                var studentName = studentList.Where(x => x.Email == sms.EmailId).FirstOrDefault();
                var message = "Absent for Offline Test Paper.";
                if (sms.IsPresent)
                    message = "Name - " + studentName.Name + "<br />Title - " + viewModel.Title + "<br />Marks - " + sms.ObtainedMarks + "/" + viewModel.TotalMarks + "<br />Percentage - " + sms.Percentage;

                var sendSMSToParent = new SmsModel
                {
                    Message = message,
                    SendTo = studentName.ParentContact
                };

                var sendSMSToStudent = new SmsModel
                {
                    Message = message,
                    SendTo = studentName.StudentContact
                };

                smsList.Add(sendSMSToParent);
                smsList.Add(sendSMSToStudent);
            }

            var smsModelLists = smsList.ToArray();
            if (smsList.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _smsService.StartProcessing(smsModelLists, cancellationToken));
                cmsResult.Results.Add(new Result { Message = "SMS sent Successfully.", IsSuccessful = true });
            }

            return cmsResult;
        }

        public CMSResult SendSMSDynamic(UploadOfflineMarksViewModel viewModel, List<StudentProjection> studentList, List<UploadOfflineTestMarks> uploadOfflineTestMarks)
        {
            var cmsResult = new CMSResult();
            var smsList = new List<SmsModel>();
            foreach (var sms in uploadOfflineTestMarks)
            {
                var studentName = studentList.Where(x => x.Email == sms.EmailId).FirstOrDefault();
                var message = "Absent for Offline Test Paper.";
                if (sms.IsPresent)
                    message = "Name - " + studentName.Name + ",Title - " + viewModel.Title + ",Marks - " + sms.ObtainedMarks + "/" + viewModel.TotalMarks + ",Percentage - " + sms.Percentage;

                var roleUserId = User.Identity.GetUserId();
                var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;

                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var senderid = projection.sender_id;
                var username = projection.username;
                var password = projection.password;
                var name = projection.name;

                var sendSMSToParent = new SmsModel
                {
                    SenderId = senderid,
                    UserName = username,
                    Password = password,
                    Message = message,
                    SendTo = studentName.ParentContact
                };

                var sendSMSToStudent = new SmsModel
                {
                    SenderId = senderid,
                    UserName = username,
                    Password = password,
                    Message = message,
                    SendTo = studentName.StudentContact
                };

                smsList.Add(sendSMSToParent);
                smsList.Add(sendSMSToStudent);
            }

            var smsModelLists = smsList.ToArray();
            if (smsList.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _smsService.StartProcessing(smsModelLists, cancellationToken));
                cmsResult.Results.Add(new Result { Message = "SMS sent Successfully.", IsSuccessful = true });
            }

            return cmsResult;
        }
        public void StartProcessingSaveOfflineTestMarks(OfflineTestStudentMarks[] offlineTestStudentMarks, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                {
                    foreach (var notificationModel in offlineTestStudentMarks)
                    {
                        //execute when task has been cancel  
                        cancellationToken.ThrowIfCancellationRequested();
                        //send email here
                        //MailSend(mailModel); // Dyanamic Send Mail
                      // Send(mailModel);   // Default Mail Send
                        _offlineTestStudentMarksService.Save(notificationModel);
                        _logger.Info("Offline test student marks saved Successfully.");
                        Thread.Sleep(1500);   // wait to 1.5 sec every time  
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }

        public void StartProcessingUpdateOfflineTestMarks(OfflineTestStudentMarks[] offlineTestStudentMarks, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                {
                    foreach (var notificationModel in offlineTestStudentMarks)
                    {
                        //execute when task has been cancel  
                        cancellationToken.ThrowIfCancellationRequested();
                        //send email here
                        //MailSend(mailModel); // Dyanamic Send Mail
                        // Send(mailModel);   // Default Mail Send
                        _offlineTestStudentMarksService.Update(notificationModel);
                        _logger.Info("Offline Test Student Marks Saved Successfully.");
                        Thread.Sleep(1500);   // wait to 1.5 sec every time  
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error Occured : " + ex.GetType().ToString() + " : " + ex.Message);
            }
        }

        public class UploadOfflineTestMarks
        {
            public string UserId { get; set; }
            public int ObtainedMarks { get; set; }
            public string Name { get; set; }
            public decimal Percentage { get; set; }
            public string EmailId { get; set; }
            public string StudentContact { get; set; }
            public bool IsPresent { get; set; }
        }
    }
}