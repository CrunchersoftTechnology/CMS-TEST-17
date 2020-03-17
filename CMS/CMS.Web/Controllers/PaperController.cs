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
using System.Web.Hosting;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class PaperController : BaseController
    {
        readonly IClassService _classService;
        readonly IQuestionService _questionService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly ITestPaperService _testPaperService;
        readonly ISubjectService _subjectService;
        readonly IBranchService _branchService;
        readonly IStudentService _studentService;
        readonly IBatchService _batchService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IEmailService _emailService;
        readonly ISmsService _smsService;
        readonly ISendNotificationService _sendNotificationService;
        readonly ILocalDateTimeService _localDateTimeService;

        public PaperController(IClassService classService, ILogger logger, IRepository repository, IQuestionService questionService,
            ITestPaperService testPaperService, ISubjectService subjectService, IBranchService branchService, IStudentService studentService, IBatchService batchService,
            IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService, IEmailService emailService,
            ISmsService smsService, ISendNotificationService sendNotificationService, ILocalDateTimeService localDateTimeService)
        {
            _classService = classService;
            _questionService = questionService;
            _logger = logger;
            _repository = repository;
            _testPaperService = testPaperService;
            _subjectService = subjectService;
            _branchService = branchService;
            _studentService = studentService;
            _batchService = batchService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _emailService = emailService;
            _smsService = smsService;
            _sendNotificationService = sendNotificationService;
            _localDateTimeService = localDateTimeService;
        }

        // GET: Paper
        public ActionResult Index()
        {
            var testPapers = _testPaperService.getTestPapers().ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<TestPaperProjection>, TestPaperViewModel[]>(testPapers);
            return View(viewModelList);
        }

        public ActionResult Chapters()
        {
            return View();
        }

        public JsonResult getChapterWithCount(string subjectIds)
        {
            var result = _questionService.GetQuestionCountsByChapters(subjectIds);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuestionCountByChapterId(int chapterId, int? level, int? type, int? asked,
            int? used, int? hint)
        {
            level = level ?? 0;
            type = type ?? 0;
            used = used ?? 0;
            asked = asked ?? 0;
            hint = hint ?? 0;

            var count = _questionService.GetQuesCountByChapterId(chapterId, (int)level, (int)type, (int)asked,
            (int)used, (int)hint);

            return Json(count, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuestion(int id, int chapterId, int? level, int? type, int? asked,
            int? used, int? hint)
        {
            used = UseNotuseQuestionCount(chapterId);
            level = level ?? 0;
            type = type ?? 0;
            used = used ?? 0;
            asked = asked ?? 0;
            hint = hint ?? 0;

            var count = 0;

            var question = _questionService.GetQuesCountByChapterIdFilter(id, chapterId, (int)level, (int)type, (int)asked,
            (int)used, (int)hint, out count);

            var result = new
            {
                count = count,
                question = question
            };

            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public int UseNotuseQuestionCount(int chapterId)
        {
            var questions = _testPaperService.getTestPapersDelimitedQuestion();
            List<TestPaperProjection> DelimitedQuestion = new List<TestPaperProjection>();
            foreach (var question in questions)
            {
                DelimitedQuestion.AddRange(JsonConvert.DeserializeObject<List<TestPaperProjection>>(question.DelimitedQuestionIds));
            }
            var questionCount = DelimitedQuestion.Where(x => x.chapterId == chapterId).Select(x => x.questionId).Distinct().Count();
            int usedCount = questionCount;
            return usedCount;
        }

        [HttpPost]
        public JsonResult saveQuestion(TestPaperViewModel viewModel)
        {
            CMSResult cmsResult = new CMSResult();
            TestPaper testPaper = new TestPaper
            {
                ClassId = viewModel.ClassId,
                Title = viewModel.Title
            };
            var subjects = _subjectService.GetAllSubjects().Where(x => x.ClassId == viewModel.ClassId).Select(x => new { x.SubjectId, x.Name });
            var result = subjects.Count() != 0 ? _testPaperService.TestIsExit(testPaper) : false;
            var tId = result != false ? 0 : 1;
            var returnData = new
            {
                Data = tId.ToString(),
                Subjects = subjects
            };
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult updateQuestion(TestPaperViewModel viewModel)
        {
            CMSResult cmsResult = new CMSResult();
            cmsResult = _testPaperService.Update(new TestPaper
            {
                TestPaperId = viewModel.TestPaperId,
                ClassId = viewModel.ClassId,
                DelimitedQuestionIds = viewModel.DelimitedQuestionIds,
                TestType = viewModel.TestType,
                Title = viewModel.Title,
                DelimitedChapterIds = viewModel.DelimitedChapterIds,
                QuestionCount = viewModel.QuestionCount
            });
            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(int? id)
        {
            ViewBag.ClassList = (from c in _classService.GetClasses()
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();

            ViewBag.TestPaperId = id != null ? id : 0;

            return View();
        }

        public ActionResult Edit(int id)
        {
            var projection = _testPaperService.GetPaperById(id);
            var viewModel = AutoMapper.Mapper.Map<TestPaperProjection, TestPaperViewModel>(projection);
            return View(viewModel);
        }

        public JsonResult GetTestPaper(int TestPaperId, int? level, int? type, int? asked,
            int? used, int? hint)
        {
            var isExists = _repository.Project<TestPaper, bool>(
                tests => (from t in tests
                          where t.TestPaperId == TestPaperId
                          select t
                          ).Any());

            if (isExists)
            {

                var testPaper = _testPaperService.GetPaperById(TestPaperId);
                var viewModel = AutoMapper.Mapper.Map<TestPaperProjection, TestPaperViewModel>(testPaper);

                var chapterId = 0;
                var countChapterWise = _testPaperService.GetCountChapterWise(viewModel.DelimitedChapterIds, out chapterId);

                var result = new
                {
                    testPaper = testPaper,
                    countChapterWise = countChapterWise
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                _logger.Warn(string.Format("Test Paper not Exists."));
                Warning("Test Paper not Exists.");
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuestionIdByChapterId(int ChapterId, string questionId)
        {
            var selectQuestionInfo = _questionService.GetQuestionByQuestionId(questionId);
            var allQuestions = _questionService.GetQuestionByChapter(ChapterId);
            var result = new
            {
                selectQuestionInfo = selectQuestionInfo,
                allQuestions = allQuestions
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(int id)
        {
            var projection = _testPaperService.GetPaperById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Test Paper not Exists {0}.", id));
                Warning("Test Paper not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<TestPaperProjection, TestPaperDeleteViewModel>(projection);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(TestPaperDeleteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _testPaperService.Delete(viewModel.TestPaperId);
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

        public ActionResult ArrengeTest(int Id)
        {
            var testList = _testPaperService.GetPaperById(Id);

            var distinctBranchList = _studentService.GetBranchesTestByClassId(testList.ClassId).Select(x => new { x.BranchId, x.BranchName }).Distinct().ToList();
            var branchList = (from b in distinctBranchList
                              select new SelectListItem
                              {
                                  Value = b.BranchId.ToString(),
                                  Text = b.BranchName
                              }).ToList();
            ViewBag.classId = testList.ClassId;
            ViewBag.TestPaperId = testList.TestPaperId;
            var batchList = (from b in _batchService.GetAllBatchClsId(testList.ClassId)
                             select new SelectListItem
                             {
                                 Value = b.BatchId.ToString(),
                                 Text = b.BatchName /*+ " (" + b.SubjectName + " )"*/
                             }).ToList();

            ViewBag.BranchId = null;
            return View(new TestPaperDeleteViewModel
            {
                Branches = branchList,
                ClassName = testList.ClassName,
                Title = testList.Title,
                TestType = testList.TestType,
                Batches = batchList,
                SubjectName = testList.SubjectName
            });
        }

        public JsonResult SendTestPaper(TestPaperDeleteViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            int studentCount = 0;
            List<string> listOfEmail = new List<string>();
            List<string> listOfName = new List<string>();
            List<string> listOfNumber = new List<string>();
            List<ListOfPlayerId> listOfParentPlayerId = new List<ListOfPlayerId>();
            List<ListOfPlayerId> listOfStudentPlayerId = new List<ListOfPlayerId>();
            List<string> listOfStudentBatch = new List<string>();

            if (ModelState.IsValid)
            {
                var studentList = _studentService.GetStudentByBranchClassBatchForTestPaper(viewModel.SelectedBranches, viewModel.ClassId.ToString(), viewModel.SelectedBatches);
                var batchIds = viewModel.SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                var batch = _repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId));

                foreach (var student in studentList)
                {
                    if (batchIds.Contains(student.BatchId))
                    {
                        listOfEmail.Add(student.Email);
                        listOfName.Add(student.Name);
                        listOfNumber.Add(student.StudentContact);
                        listOfStudentPlayerId.Add(new ListOfPlayerId
                        {
                            SId = student.SId,
                            ParentPlayerId = student.studentAppPlayerId
                        });
                        //listOfParentPlayerId.Add(new ListOfPlayerId
                        //{
                        //    SId = student.SId,
                        //    ParentPlayerId = student.parentAppPlayerId
                        //});
                        listOfStudentBatch.Add(student.BatchName);
                    }
                }

                listOfParentPlayerId = listOfParentPlayerId.Where(s => !string.IsNullOrWhiteSpace(s.ParentPlayerId)).ToList();
                listOfStudentPlayerId = listOfStudentPlayerId.Where(s => !string.IsNullOrWhiteSpace(s.ParentPlayerId)).ToList();

                var numberOfStudent = (viewModel.Email ? listOfEmail.Distinct().ToList().Count : 0) + (viewModel.SMS ? listOfNumber.Distinct().ToList().Count : 0) +
                    listOfParentPlayerId.Count + listOfStudentPlayerId.Count;

                int arrangeTestId = 0;
                studentCount = listOfEmail.Distinct().ToList().Count;

                if (numberOfStudent > 0)
                {
                    try
                    {
                        var arrangeTest = new ArrengeTest
                        {
                            TestPaperId = viewModel.TestPaperId,
                            SelectedBranches = viewModel.SelectedBranches != null ? viewModel.SelectedBranches : "",
                            SelectedBatches = viewModel.SelectedBatches != null ? viewModel.SelectedBatches : "",
                            StudentCount = studentCount,
                            Media = viewModel.Media,
                            Date = viewModel.Date,
                            StartTime = Convert.ToDateTime(viewModel.StartTime),
                            TimeDuration = viewModel.TimeDuration
                        };

                        var result = _testPaperService.SaveArrengeTest(arrangeTest);

                        if (result.Success)
                        {
                            arrangeTestId = arrangeTest.ArrengeTestId;
                            var resultTest = _testPaperService.UpdateTestStatus(new TestPaper
                            {
                                TestPaperId = viewModel.TestPaperId
                            });
                        }
                        else
                        {
                            cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = false });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message + "catch SendTestPaper");
                    }
                }

                if (listOfName.Count > 0)
                {
                    if (viewModel.Email)
                    {
                        if (listOfEmail.Count > 0)
                        {
                            var result = SendEmail(listOfEmail.Distinct().ToList(), listOfName.Distinct().ToList(), viewModel, listOfStudentBatch);
                            studentCount = listOfEmail.Distinct().Count();
                            if (result == true)
                            {
                                cmsResult.Results.Add(new Result { Message = "Email sent successfully.", IsSuccessful = true });
                            }
                            else
                            {
                                cmsResult.Results.Add(new Result { Message = "Something went wrong to send email.", IsSuccessful = false });
                            }
                        }
                    }
                    if (viewModel.SMS)
                    {
                        if (listOfNumber.Count > 0)
                        {
                            var result = SendSMS(viewModel.Title, listOfNumber.Distinct().ToList());
                            studentCount = listOfNumber.Distinct().Count();
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
                    if (viewModel.AppNotification)
                    {
                        var response = SendAppNotification(viewModel, listOfParentPlayerId, listOfStudentPlayerId, arrangeTestId);
                        studentCount = listOfParentPlayerId.Distinct().Count();
                        studentCount = listOfStudentPlayerId.Distinct().Count();
                        if (response.Success)
                        {
                            cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = true });
                        }
                        else
                        {
                            cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = false });
                        }
                    }
                }
                else
                {
                    cmsResult.Results.Add(new Result { Message = "No student available please select another batch.", IsSuccessful = false });
                }
                int resultCount = cmsResult.Results.Where(x => x.IsSuccessful == true).Count();

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

        public bool SendEmail(List<string> listOfEmail, List<string> listOfName, TestPaperDeleteViewModel viewModel, List<string> listOfStudentBatch)
        {
            string userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                userRole = branchAdmin.BranchName;
            }

            var i = 0;
            MailModel[] emailAddress = new MailModel[listOfName.Count];
            foreach (var Name in listOfName)
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", userRole);
                body = body.Replace("{NotificationMessage}", "Test Type : " + viewModel.TestType + "<br/>Test title :" + viewModel.Title +
                        "<br/>Class Name :" + viewModel.ClassName + "<br/>Batch Name :" + listOfStudentBatch[i] + "<br/>Subject Name :" + viewModel.SubjectName);
                body = body.Replace("{UserName}", Name);

                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = "Online test arrange",
                    To = listOfEmail[i]
                };

                emailAddress[i] = emailMessage;
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailAddress, cancellationToken));
            return true;
        }

        public CMSResult SendSMS(string title, List<string> listOfNumber)
        {
            var listOfContact = string.Join(",", listOfNumber);
            var smsModel = new SmsModel
            {
                Message = "Arrange test : " + title + " check mail for more detail",
                SendTo = listOfContact
            };

            return _smsService.SendMessage(smsModel);
        }

        public CMSResult SendAppNotification(TestPaperDeleteViewModel viewmodel, List<ListOfPlayerId> listOfParentPlayerId, List<ListOfPlayerId> listOfStudentPlayerId,
            int arrangeTestId)
        {
            var notificationList = new List<SendNotificationByPlayerId>();
            var cmsResult = new CMSResult();
            listOfParentPlayerId = listOfParentPlayerId.Where(s => !string.IsNullOrWhiteSpace(s.ParentPlayerId)).ToList();
            listOfStudentPlayerId = listOfStudentPlayerId.Where(s => !string.IsNullOrWhiteSpace(s.ParentPlayerId)).ToList();
            foreach (var playerid in listOfParentPlayerId)
            {
                var sendAppNotification = new SendNotificationByPlayerId
                {
                    Message = "Test-" + viewmodel.Title + "$^$Date:" + viewmodel.Date.ToString("dd-MM-yyyy").Split(' ')[0] + "$^$Start Time:" + viewmodel.StartTime + "$^$Duration:" + viewmodel.TimeDuration + "$^$TestPaperId:" + viewmodel.TestPaperId + "$^$" + 1 + "," + arrangeTestId,
                    PlayerIds = playerid.ParentPlayerId,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                };
                notificationList.Add(sendAppNotification);
            }

            foreach (var playerid in listOfStudentPlayerId)
            {
                var sendAppNotification = new SendNotificationByPlayerId
                {
                    Message = "Test-" + viewmodel.Title + "$^$Date:" + viewmodel.Date.ToString("dd-MM-yyyy").Split(' ')[0] + "$^$Start Time:" + viewmodel.StartTime + "$^$Duration:" + viewmodel.TimeDuration + "$^$TestPaperId:" + viewmodel.TestPaperId + "$^$" + 1 + "," + arrangeTestId,
                    PlayerIds = playerid.ParentPlayerId,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.StudentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.StudentRestAppId]
                };
                notificationList.Add(sendAppNotification);
            }

            if (notificationList.Count > 0)
            {
                var notification = notificationList.ToArray();
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
                cmsResult.Results.Add(new Result { Message = "App Notification sent successfully.", IsSuccessful = true });
                return cmsResult;
            }
            else
            {
                cmsResult.Results.Add(new Result { Message = "No one is registered in parent app.", IsSuccessful = false });
                return cmsResult;
            }
        }

        public ActionResult ArrengeTestList()
        {
            return View();
        }

        public ActionResult DetailsArrengeTest(int id)
        {
            var arrangeTest = _testPaperService.GetArrangeTestById(id);
            var viewModel = AutoMapper.Mapper.Map<ArrangeTestProjection, ArrangeTestViewModel>(arrangeTest);

            //var commaseperatedBranchList = arrangeTest.SelectedBranches ?? string.Empty;
            //var branchIds = commaseperatedBranchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            //var branches = _repository.LoadList<Branch>(x => branchIds.Contains(x.BranchId)).ToList();
            //List<string> BranchList = new List<string>();
            //BranchList = branches.Select(x => x.Name).ToList();
            //var Branchlist = string.Join(",", BranchList);
            var BranchList = _branchService.GetBranchByMultipleBranchId(arrangeTest.SelectedBranches).Select(x => x.Name).ToList();
            viewModel.SelectedBranches = string.Join(",", BranchList);

            var commaseperatedBatchList = arrangeTest.SelectedBatches ?? string.Empty;
            var BatchIds = commaseperatedBatchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var Batch = _repository.LoadList<Batch>(x => BatchIds.Contains(x.BatchId)).ToList();
            List<string> BatchList = new List<string>();

            BatchList = Batch.Select(x => x.Name).ToList();
            var BatchesList = string.Join(",", BatchList);
            //string subject = "";
            //foreach (var s in Batch)
            //{
            //    //var subjects = _subjectService.GetSubjectById(s.SubjectId);
            //    subject += string.Format("{0} ({1}),", s.Name/*, subjects.Name*/);
            //}
            viewModel.SelectedBatches = BatchesList.TrimEnd(',');
            viewModel.SubjectName = arrangeTest.SubjectName;

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Save(TestPaperViewModel viewModel)
        {
            TestPaper testPaper = new TestPaper
            {
                ClassId = viewModel.ClassId,
                DelimitedQuestionIds = viewModel.DelimitedQuestionIds,
                TestType = viewModel.TestType,
                Title = viewModel.Title,
                DelimitedChapterIds = viewModel.DelimitedChapterIds,
                SubjectName = viewModel.SubjectName,
                QuestionCount = viewModel.QuestionCount
            };
            var result = _testPaperService.Save(testPaper);
            var tId = result != null ? testPaper.TestPaperId : 0;
            var returnData = new
            {
                Data = tId.ToString(),
            };
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTestPapers(int classId)
        {
            var testPaperLists = _testPaperService.GetTestPapersByClassId(classId).ToList();
            return Json(testPaperLists, JsonRequestBehavior.AllowGet);
        }
    }

    public class TestPaperQuestionsDetails
    {
        public int chapterId { get; set; }
        public int questionId { get; set; }
        public string chapterName { get; set; }
    }
}