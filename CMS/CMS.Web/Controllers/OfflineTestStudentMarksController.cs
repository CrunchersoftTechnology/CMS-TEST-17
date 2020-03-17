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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole)]
    public class OfflineTestStudentMarksController : BaseController
    {
        readonly IOfflineTestPaper _offlineTestPaper;
        readonly IOfflineTestStudentMarksService _offlineTestStudentMarksService;
        readonly IBranchService _branchService;
        readonly IBatchService _batchService;
        readonly IRepository _repository;
        readonly IAspNetRoles _aspNetRolesService;
        readonly ILogger _logger;
        readonly IBranchAdminService _branchAdminService;
        readonly ISmsService _smsService;
        readonly IEmailService _emailService;



        public OfflineTestStudentMarksController(IOfflineTestPaper offlineTestPaper, IOfflineTestStudentMarksService offlineTestStudentMarksService,
            IBranchService branchService, IBatchService batchService, IRepository repository,
            IAspNetRoles aspNetRolesService, ILogger logger, IBranchAdminService branchAdminService,
            ISmsService smsService, IEmailService emailService)
        {
            _offlineTestPaper = offlineTestPaper;
            _offlineTestStudentMarksService = offlineTestStudentMarksService;
            _batchService = batchService;
            _branchService = branchService;
            _repository = repository;
            _aspNetRolesService = aspNetRolesService;
            _logger = logger;
            _branchAdminService = branchAdminService;
            _smsService = smsService;
            _emailService = emailService;

        }
        // GET: OfflineTestStudentMarks
        public ActionResult Index()
        {
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(User.Identity.GetUserId()) : null;
            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.BranchId;
            }
            return View();
        }

        public ActionResult Create()
        {
            var papers = _offlineTestPaper.GetOfflineTestPaper().ToList();
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                papers = _offlineTestPaper.GetOfflineTestPaper().ToList().Where(y => (y.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse)).Contains(projection.BranchId)).ToList();
            }
            var viewModel = new OfflineTestStudentMarksViewModel();
            viewModel.Papers = new SelectList(papers, "OfflineTestPaperId", "Title");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(OfflineTestStudentMarksViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (viewModel.TotalMarks < viewModel.MarksObtained)
                {
                    _logger.Warn("Marks obtained is grater than Total Marks!");

                    Warning("Marks obtained is grater than Total Marks!", true);
                }
                else
                {
                    var offlineTestStudentMarks = new OfflineTestStudentMarks
                    {
                        UserId = viewModel.UserId,
                        OfflineTestPaperId = viewModel.OfflineTestPaperId,
                        ObtainedMarks = viewModel.MarksObtained,
                    };
                    var result = _offlineTestStudentMarksService.Save(offlineTestStudentMarks);
                    if (result.Success)
                    {
                        string subject = "Student Test Marks";
                        result = SendNotificationToStudent(viewModel, subject);

                        var messages = "";
                        foreach (var message in result.Results)
                        {
                            messages += message.Message + "<br />";
                        }

                        Success(messages);
                        ModelState.Clear();
                        viewModel = new OfflineTestStudentMarksViewModel();
                    }
                    else
                    {
                        _logger.Warn(result.Results.FirstOrDefault().Message);
                        Warning(result.Results.FirstOrDefault().Message, true);
                    }
                }
            }
            viewModel = new OfflineTestStudentMarksViewModel();
            var papers = _offlineTestPaper.GetOfflineTestPaper().ToList();
            viewModel.Papers = new SelectList(papers, "OfflineTestPaperId", "Title");
            return View(viewModel);
        }

        public CMSResult SendNotificationToStudent(OfflineTestStudentMarksViewModel viewModel, string subject)
        {
            var cmsResult = new CMSResult();

            if (viewModel.StudentEmail != null)
            {
                var result = SendEmail(viewModel, subject);

                if (result == true)
                {
                    cmsResult.Results.Add(new Result { Message = "Email sent successfully.", IsSuccessful = true });
                }
                else
                {
                    cmsResult.Results.Add(new Result { Message = "Something went wrong to send email.", IsSuccessful = false });
                }
            }

            if (viewModel.SMS)
            {
                if (viewModel.StudentContact != null)
                {
                    var smsModel = new SmsModel
                    {
                        Message = "Offline Test Title : " + viewModel.Title + " " + "....check your Mail for more details",
                        SendTo = viewModel.StudentContact
                    };

                    var result = _smsService.SendMessage(smsModel);
                    if (result.Success)
                    {
                        cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = true });
                    }
                    else
                    {
                        cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = false });
                    }
                }
            }
            #region appNotification
            //if (viewModel.AppNotification)
            //{
            //    var response = SendAppNotification(viewModel.Title, listOfPlayerId);

            //    if (response.Success)
            //    {
            //        cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = true });
            //    }
            //    else
            //    {
            //        cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = false });
            //    }
            //}
            #endregion

            return cmsResult;
        }

        public bool SendEmail(OfflineTestStudentMarksViewModel viewModel, string subject)
        {
            string userRole = "";
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
            body = body.Replace("{BranchName}", userRole);
            body = body.Replace("{NotificationMessage}", "<br/> Title : " + viewModel.Title + "<br/>Class Name : " + viewModel.ClassName + "<br/>Subject Name : " +
                 viewModel.SubjectName + "<br/>Batch Name : " + viewModel.BatchName +
                "<br/> Total Marks : " + viewModel.TotalMarks.ToString() + "<br/> Student Marks : " + viewModel.MarksObtained.ToString());
            body = body.Replace("{UserName}", viewModel.StudentName);
            var emailMessage = new MailModel
            {
                Body = body,
                Subject = subject,
                To = viewModel.StudentEmail,
                IsBranchAdmin = isBranchAdmin
            };
            _emailService.Send(emailMessage);
            return true;
        }

        public ActionResult GetOfflineTestById(int OfflineTestPaperId)
        {
            var offlineTest = _offlineTestPaper.GetOfflineTestById(OfflineTestPaperId);

            var BranchList = _branchService.GetBranchByMultipleBranchId(offlineTest.SelectedBranches).Select(x => x.Name).ToList();
            var SelectedBranchesName = string.Join(",", BranchList);

            var BatchList = _batchService.GetBatchesByBatchIds(offlineTest.SelectedBatches).Select(x => x.BatchName).ToList();
            var SelectedBatchesName = string.Join(",", BatchList);
            var tests = new
            {
                className = offlineTest.ClassName,
                subjectName = offlineTest.SubjectName,
                batchName = SelectedBatchesName,
                branchName = SelectedBranchesName,
                branchId = offlineTest.SelectedBranches,
                batchId = offlineTest.SelectedBatches,
                classId = offlineTest.ClassId,
                totalMarks = offlineTest.TotalMarks
            };
            return Json(tests, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentList(string query, int classId, string SelectedBranches, string SelectedBatches)
        {
            List<StudentProjection> studentBatchWiseList = new List<StudentProjection>();
            var studentList = _offlineTestStudentMarksService.GetStudentsAutoComplete(query, classId, SelectedBranches).ToList();
            var batchIds = SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            var batch = _repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId));

            foreach (var student in studentList)
            {
                //foreach (var studentBatch in student.Batches)
                //{
                //    if (batchIds.Contains(studentBatch.BatchId))
                //    {
                //        studentBatchWiseList.Add(new StudentProjection
                //        {
                //            Name = student.Name,
                //            StudentContact = student.StudentContact,
                //            Email = student.Email,
                //            DOJ = student.DOJ,
                //            DOB = student.DOB,
                //            UserId = student.UserId,
                //            parentAppPlayerId = student.parentAppPlayerId,
                //           // StudentBatches = student.StudentBatches
                //        });
                //    }
                //}
            }
            return Json(studentBatchWiseList, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var projection = _offlineTestStudentMarksService.GetOfflineTestMarksById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Offline Test Paper Marks does not Exists {0}.", id));
                Warning("Offline Test Paper Marks does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<OfflineTestStudentMarksProjection, OfflineTestStudentMarksViewModel>(projection);
            var BranchList = _branchService.GetBranchByMultipleBranchId(projection.SelectedBranches).Select(x => x.Name).ToList();
            viewModel.BranchName = string.Join(",", BranchList);

            var BatchList = _batchService.GetBatchesByBatchIds(projection.SelectedBatches).Select(x => x.BatchName).ToList();
            viewModel.BatchName = string.Join(",", BatchList);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(OfflineTestStudentMarksViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _offlineTestStudentMarksService.Delete(viewModel.OfflineTestStudentMarksId);
                if (result.Success)
                {
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            var offlineTestMarks = _offlineTestStudentMarksService.GetOfflineTestMarksById(id);
            if (offlineTestMarks == null)
            {
                _logger.Warn(string.Format("Offline Test Marks does not Exists {0}.", id));
                Warning("Offline Test Marks does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<OfflineTestStudentMarksProjection, OfflineTestStudentMarksViewModel>(offlineTestMarks);

            var BranchList = _branchService.GetBranchByMultipleBranchId(offlineTestMarks.SelectedBranches).Select(x => x.Name).ToList();
            viewModel.BranchName = string.Join(",", BranchList);

            var BatchList = _batchService.GetBatchesByBatchIds(offlineTestMarks.SelectedBatches).Select(x => x.BatchName).ToList();
            viewModel.BatchName = string.Join(",", BatchList);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OfflineTestStudentMarksViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var offlineMarks = _repository.Project<OfflineTestStudentMarks, bool>(offlineStudentMarks => (from offline in offlineStudentMarks where offline.OfflineTestStudentMarksId == viewModel.OfflineTestStudentMarksId select offline).Any());
                if (!offlineMarks)
                {
                    _logger.Warn(string.Format("Offline Test Marks does not exists '{0}'.", viewModel.Title));
                    Danger(string.Format("Offline Test Marks does not exists '{0}'.", viewModel.Title));
                }
                if (viewModel.TotalMarks < viewModel.MarksObtained)
                {
                    _logger.Warn("Marks obtained is grater than Total Marks!");
                    Warning("Marks obtained is grater than Total Marks!", true);
                }
                else
                {
                    var result = _offlineTestStudentMarksService.Update(new OfflineTestStudentMarks { OfflineTestStudentMarksId = viewModel.OfflineTestStudentMarksId, ObtainedMarks = viewModel.MarksObtained });
                    if (result.Success)
                    {
                        string subject = "Student Test Marks Updated";
                        result = SendNotificationToStudent(viewModel, subject);
                        var messages = "";
                        foreach (var message in result.Results)
                        {
                            messages += message.Message + "<br />";
                        }

                        Success(messages);
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        _logger.Warn(result.Results.FirstOrDefault().Message);
                        Warning(result.Results.FirstOrDefault().Message, true);
                    }
                }
            }
            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            var offlineTest = _offlineTestStudentMarksService.GetOfflineTestMarksById(id);
            var viewModel = AutoMapper.Mapper.Map<OfflineTestStudentMarksProjection, OfflineTestStudentMarksViewModel>(offlineTest);

            var BranchList = _branchService.GetBranchByMultipleBranchId(offlineTest.SelectedBranches).Select(x => x.Name).ToList();
            viewModel.BranchName = string.Join(",", BranchList);

            var BatchList = _batchService.GetBatchesByBatchIds(offlineTest.SelectedBatches).Select(x => x.BatchName).ToList();
            viewModel.BatchName = string.Join(",", BatchList);

            return View(viewModel);
        }
    }
}