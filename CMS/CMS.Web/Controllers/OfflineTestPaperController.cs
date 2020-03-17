using Microsoft.AspNet.Identity;
using System.Web.Mvc;
using CMS.Web.ViewModels;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using System.Linq;
using CMS.Common;
using System.Collections.Generic;
using CMS.Domain.Infrastructure;
using CMS.Web.Logger;
using CMS.Domain.Models;
using CMS.Web.Models;
using System.IO;
using System;
using CMS.Domain.Storage.Projections;
using System.Web.Hosting;
using System.Configuration;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class OfflineTestPaperController : BaseController
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
        readonly ISendNotificationService _sendNotificationService;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureServices;

        public OfflineTestPaperController(IClientAdminService clientAdminService, IBranchService branchService, IClassService classService, IBatchService batchService,
                IEmailService emailService, IAspNetRoles aspNetRolesService, IConfigureServices configureServices,
                IBranchAdminService branchAdminService, IStudentService studentService,
                ISubjectService subjectService, IRepository repository, ILogger logger,
                ISmsService smsService, IOfflineTestPaper offlineTestPaper,
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
            _sendNotificationService = sendNotificationService;
            _clientAdminService = clientAdminService;
            _configureServices = configureServices;
        }
        // GET: OfflineTestPaper
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

            if (roles == "Admin")
            {
                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                ViewBag.BranchId = 0;
                ViewBag.CurrentUserRole = roles;

                return View(new OfflineTestPaperViewModel
                {
                    Branches = branchList.Distinct(),
                    CurrentUserRole = roles
                });
            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(User.Identity.GetUserId());
                ViewBag.ClientId = projection.ClientId;

                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                var classList = (from c in _studentService.GetStudentsByClientId(projection.ClientId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();

                ViewBag.CurrentUserRole = roles;
                return View(new OfflineTestPaperViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName,
                    Classes = classList,
                    Branches = branchList.Distinct(),
                });

            }
            else if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                ViewBag.BranchId = projection.BranchId;
                var classList = (from c in _studentService.GetStudentsByBranchId(projection.BranchId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();
                ViewBag.CurrentUserRole = roles;
                return View(new OfflineTestPaperViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projection.BranchId,
                    BranchName = projection.BranchName,
                    Classes = classList,
                    SelectedBranches = projection.BranchId.ToString()
                });
            }
            return View();

        }

        public ActionResult GetSubjetctByClassId(int classId)
        {
            var subjects = _subjectService.GetSubjects(classId).Select(x => new { x.SubjectId, x.Name });
            subjects = subjects.Distinct().ToList();
            var result = new
            {
                subjects = subjects,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBatchBySubjectId(int classId)
        {
            var batches = _batchService.GetBatchesBySubjectId(classId).Select(x => new { x.BatchId, x.BatchName });
            batches = batches.Distinct().ToList();
            var result = new
            {
                batches = batches,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public CMSResult SendNotificationToStudent(OfflineTestPaperViewModel viewModel, string subject, int offlineTestPaperId, List<string> listOfEmail,
            List<string> listOfName, List<string> listOfNumber, List<ListOfPlayerId> finalParentPlayerId, List<ListOfPlayerId> finalStudentPlayerId)
        {
            var cmsResult = new CMSResult();
            if (listOfEmail.Count > 0)
            {
                var result = SendEmailDynamic(listOfEmail.Distinct().ToList(), listOfName.Distinct().ToList(), viewModel.Title, viewModel.ClassName, viewModel.SubjectName, viewModel.SelectedBatchesName, viewModel.TestDate, viewModel.TotalMarks, subject);
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
                if (listOfNumber.Count > 0)
                {
                    var result = SendSMSDynamic(viewModel.Title, listOfNumber.Distinct().ToList());
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
                var response = SendAppNotification(viewModel, finalParentPlayerId, finalStudentPlayerId, offlineTestPaperId);

                if (response.Success)
                {
                    cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = true });
                }
                else
                {
                    cmsResult.Results.Add(new Result { Message = response.Results[0].Message, IsSuccessful = false });
                }
            }

            return cmsResult;
        }

        public JsonResult SendOffineTestPaperNotification(OfflineTestPaperViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            List<string> listOfEmail = new List<string>();
            List<string> listOfName = new List<string>();
            List<string> listOfNumber = new List<string>();
            var finalParentPlayerId = new List<ListOfPlayerId>();
            var finalStudentPlayerId = new List<ListOfPlayerId>();
            if (ModelState.IsValid)
            {
                TimeSpan span = (Convert.ToDateTime(viewModel.TestOutTime) - Convert.ToDateTime(viewModel.TestInTime));
                if (viewModel.TestInTime != null && viewModel.TestOutTime != null && Convert.ToDateTime(viewModel.TestInTime).ToShortTimeString() != "12:00 AM" && Convert.ToDateTime(viewModel.TestOutTime).ToShortTimeString() != "12:00 AM" && viewModel.TestOutTime != null && !(span >= TimeSpan.FromMinutes(30) && span <= TimeSpan.FromHours(3)))
                {
                    cmsResult.Results.Add(new Result { Message = "The time limit should be min lengh of (1/2 hr) & max length of  (3 hrs).", IsSuccessful = false });
                }
                else
                {
                    string subject = "Exam Schedule Create";
                    try
                    {
                        var studentList = _studentService.GetStudentByBranchClassBatchForTestPaper(viewModel.SelectedBranches, viewModel.ClassId.ToString(), viewModel.SelectedBatches);
                        var batchIds = viewModel.SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                        var batch = _repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId));

                        foreach (var student in studentList)
                        {
                            if (batchIds.Contains(student.BatchId))
                            {
                                if (viewModel.AppNotification)
                                {
                                    if (student.parentAppPlayerId != "" && student.parentAppPlayerId != null)
                                        finalParentPlayerId.Add(new ListOfPlayerId
                                        {
                                            SId = student.SId,
                                            ParentPlayerId = student.parentAppPlayerId
                                        });
                                    if (student.studentAppPlayerId != "" && student.studentAppPlayerId != null)
                                        finalStudentPlayerId.Add(new ListOfPlayerId
                                        {
                                            SId = student.SId,
                                            ParentPlayerId = student.studentAppPlayerId
                                        });
                                }

                                listOfEmail.Add(student.Email);
                                listOfName.Add(student.Name);
                                listOfNumber.Add(student.StudentContact);
                            }
                        }

                        if (listOfNumber.Count > 0)
                        {
                            var offlineTestPaper = new OfflineTestPaper
                            {
                                OfflineTestPaperId = viewModel.OfflineTestPaperId,
                                Title = viewModel.Title,
                                TestDate = viewModel.TestDate,
                                TestInTime = viewModel.TestInTime == null ? DateTime.Now.Date : Convert.ToDateTime(viewModel.TestInTime.Trim()),
                                TestOutTime = viewModel.TestOutTime == null ? DateTime.Now.Date : Convert.ToDateTime(viewModel.TestOutTime.Trim()),
                                TotalMarks = viewModel.TotalMarks,
                                SelectedBranches = viewModel.SelectedBranches,
                                SelectedBatches = viewModel.SelectedBatches,
                                ClassId = viewModel.ClassId,
                                SubjectId = viewModel.SubjectId,
                                Media = viewModel.Media
                            };
                            var result = _offlineTestPaper.Save(offlineTestPaper);

                            if (result.Success)
                            {
                                var offlineTestPaperId = offlineTestPaper.OfflineTestPaperId;
                                var roleUserId = User.Identity.GetUserId();
                                var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                                if (roles == "BranchAdmin")
                                {
                                    var resultEmail = sendMailfromBranchAdmin(viewModel.Title, viewModel.Media);
                                }

                                cmsResult = SendNotificationToStudent(viewModel, subject, offlineTestPaperId, listOfEmail, listOfName, listOfNumber,
                                    finalParentPlayerId, finalStudentPlayerId);

                                cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = true });
                            }
                            else
                            {
                                cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = false });
                            }
                        }
                        else
                        {
                            cmsResult.Results.Add(new Result { Message = "No student available please select another batch.", IsSuccessful = false });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex.Message + "catch SendOffineTestPaperNotification");
                        throw;
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

        public bool sendMailfromBranchAdmin(string Title, string Media)
        {
            var branchAdmin = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }

            body = body.Replace("{UserName}", User.Identity.GetUserName());
            body = body.Replace("{BranchName}", branchAdmin.BranchName);
            body = body.Replace("{NotificationMessage}", "Notification Message : " + Title + "<br/>" + Media);



            var emailMessage = new MailModel
            {
                Body = body,
                Subject = "Web portal Exam Schedule Notification.",
                IsBranchAdmin = true
            };
            _emailService.Send(emailMessage);
            return true;
        }

        public bool SendEmail(List<string> listOfEmail, List<string> listOfName, string Title, string ClassName, string SubjectName, string BatchName, DateTime Date, int TotalMarks, string subject)
        {
            string userRole = "";
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                userRole = branchAdmin.BranchName;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
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
                body = body.Replace("{NotificationMessage}", "<br/> Title : " + Title + "<br/>Class Name : " + ClassName + "<br/>Subject Name : "
                    + SubjectName + "<br/>Batch Name : " + BatchName +
                    "<br/> Test Date: " + Date.ToString("dd/MM/yyyy") + "<br/> Total Marks : " + TotalMarks);
                body = body.Replace("{UserName}", Name);

                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = subject,
                    To = listOfEmail[i]
                };
                emailAddress[i] = emailMessage;
                i++;
            }

            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailAddress, cancellationToken));

            return true;
        }


        public bool SendEmailDynamic(List<string> listOfEmail, List<string> listOfName, string Title, string ClassName, string SubjectName, string BatchName, DateTime Date, int TotalMarks, string subject)
        {
            string userRole = "";
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                userRole = branchAdmin.BranchName;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
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
                body = body.Replace("{NotificationMessage}", "<br/> Title : " + Title + "<br/>Class Name : " + ClassName + "<br/>Subject Name : "
                    + SubjectName + "<br/>Batch Name : " + BatchName +
                    "<br/> Test Date: " + Date.ToString("dd/MM/yyyy") + "<br/> Total Marks : " + TotalMarks);
                body = body.Replace("{UserName}", Name);

                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;

                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var emailid = projection.email_id;
                var password = projection.emailpassword;

                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = subject,
                    Emailid = emailid,
                    Password = password,
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
                Message = "Offline Test Title : " + title + " " + "....Check Your Mail for more Details",
                SendTo = listOfContact
            };

            return _smsService.SendMessage(smsModel);
        }

        public CMSResult SendSMSDynamic(string title, List<string> listOfNumber)
        {
            var listOfContact = string.Join(",", listOfNumber);

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var ClientId = projectionClient.ClientId;

            var projection = _configureServices.GetConfigureById(ClientId);
            var clientId = projection.ClientId;
          
            var senderid = projection.sender_id;
            var username = projection.username;
            var password = projection.password;

            var smsModel = new SmsModel
            {
                SenderId = senderid,
                UserName = username,
                Password = password,

                Message = "Offline Test Title : " + title + " " + "....Check Your Mail for more Details",
                SendTo = listOfContact
            };

            return _smsService.DynamicsSendMessage(smsModel);
        }
        public CMSResult SendAppNotification(OfflineTestPaperViewModel viewModel, List<ListOfPlayerId> listOfPlayerId, List<ListOfPlayerId> listOfStudentId,
            int offlineTestPaperId)
        {
            var cmsResult = new CMSResult();
            var notificationList = new List<SendNotificationByPlayerId>();
            foreach (var playerId in listOfPlayerId)
            {
                var studentSId = playerId.SId;
                var sendAppNotification = new SendNotificationByPlayerId
                {
                    Message = "OfflineTest-<br />" + viewModel.Title + " offline test paper on " + viewModel.TestDate.ToString("dd-MM-yyyy") + ", Total Marks - " + viewModel.TotalMarks +
                    "$^$" + playerId.SId + "@" + offlineTestPaperId,
                    PlayerIds = playerId.ParentPlayerId,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                };
                notificationList.Add(sendAppNotification);
            }
            foreach (var playerId in listOfStudentId)
            {
                var studentSId = playerId.SId;
                var sendAppNotification = new SendNotificationByPlayerId
                {
                    Message = "OfflineTest-<br />" + viewModel.Title + " offline test paper on " + viewModel.TestDate.ToString("dd-MM-yyyy") + ", Total Marks - " + viewModel.TotalMarks +
                    "$^$" + playerId.SId + "@" + offlineTestPaperId,
                    PlayerIds = playerId.ParentPlayerId,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.StudentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.StudentRestAppId]
                };
                notificationList.Add(sendAppNotification);
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

        public ActionResult Delete(int id)
        {
            var projection = _offlineTestPaper.GetOfflineTestById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Exam Schedule does not Exists {0}.", id));
                Warning("Exam Schedule does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<OfflineTestPaperProjection, OfflineTestPaperViewModel>(projection);
            var BranchList = _branchService.GetBranchByMultipleBranchId(projection.SelectedBranches).Select(x => x.Name).ToList();
            viewModel.SelectedBranchesName = string.Join(",", BranchList);

            var BatchList = _batchService.GetBatchesByBatchIds(projection.SelectedBatches).Select(x => x.BatchName).ToList();
            viewModel.SelectedBatchesName = string.Join(",", BatchList);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(OfflineTestPaperViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _offlineTestPaper.Delete(viewModel.OfflineTestPaperId);
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
            var offlineTest = _offlineTestPaper.GetOfflineTestById(id);
            if (offlineTest == null)
            {
                _logger.Warn(string.Format("Exam Schedule does not Exists {0}.", id));
                Warning("Exam Schedule does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<OfflineTestPaperProjection, OfflineTestPaperViewModel>(offlineTest);

            var BranchList = _branchService.GetBranchByMultipleBranchId(offlineTest.SelectedBranches).Select(x => x.Name).ToList();
            viewModel.SelectedBranchesName = string.Join(",", BranchList);

            var BatchList = _batchService.GetBatchesByBatchIds(offlineTest.SelectedBatches).Select(x => x.BatchName).ToList();
            viewModel.SelectedBatchesName = string.Join(",", BatchList);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(OfflineTestPaperViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            List<string> listOfEmail = new List<string>();
            List<string> listOfName = new List<string>();
            List<string> listOfNumber = new List<string>();
            var finalParentPlayerId = new List<ListOfPlayerId>();
            var finalStudentPlayerId = new List<ListOfPlayerId>();

            if (viewModel.SMS)
                viewModel.Media += ", SMS";
            if (viewModel.AppNotification)
                viewModel.Media += ", AppNotification";
            if (ModelState.IsValid)
            {
                var offlineTests = _repository.Project<OfflineTestPaper, bool>(offlineTest => (from offline in offlineTest where offline.OfflineTestPaperId == viewModel.OfflineTestPaperId select offline).Any());

                if (!offlineTests)
                {
                    _logger.Warn(string.Format("Exam Schedule not exists '{0}'.", viewModel.Title));
                    Danger(string.Format("Exam Schedule not exists '{0}'.", viewModel.Title));
                }
                TimeSpan span = (Convert.ToDateTime(viewModel.TestOutTime) - Convert.ToDateTime(viewModel.TestInTime));
                if (viewModel.TestInTime != null && viewModel.TestOutTime != null && Convert.ToDateTime(viewModel.TestInTime).ToShortTimeString() != "12:00 AM" && Convert.ToDateTime(viewModel.TestOutTime).ToShortTimeString() != "12:00 AM" && viewModel.TestOutTime != null && !(span >= TimeSpan.FromMinutes(30) && span <= TimeSpan.FromHours(3)))
                {
                    cmsResult.Results.Add(new Result { Message = "The time limit should be min lengh of (1/2 hr) & max length of  (3 hrs).", IsSuccessful = false });
                }
                else
                {
                    string subject = "Exam Schedule Update";
                    var studentList = _studentService.GetStudentByBranchClassBatchForTestPaper(viewModel.SelectedBranches, viewModel.ClassId.ToString(), viewModel.SelectedBatches);
                    var batchIds = viewModel.SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
                    var batch = _repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId));

                    foreach (var student in studentList)
                    {
                        if (batchIds.Contains(student.BatchId))
                        {
                            if (viewModel.AppNotification)
                            {
                                if (student.parentAppPlayerId != "" && student.parentAppPlayerId != null)
                                    finalParentPlayerId.Add(new ListOfPlayerId
                                    {
                                        SId = student.SId,
                                        ParentPlayerId = student.parentAppPlayerId
                                    });
                                if (student.studentAppPlayerId != "" && student.studentAppPlayerId != null)
                                    finalStudentPlayerId.Add(new ListOfPlayerId
                                    {
                                        SId = student.SId,
                                        ParentPlayerId = student.studentAppPlayerId
                                    });
                            }

                            listOfEmail.Add(student.Email);
                            listOfName.Add(student.Name);
                            listOfNumber.Add(student.StudentContact);
                        }
                    }

                    int resultCount = cmsResult.Results.Where(x => x.IsSuccessful == true).Count();
                    if (resultCount > 0)
                    {
                        var result = _offlineTestPaper.Update(new OfflineTestPaper
                        {
                            OfflineTestPaperId = viewModel.OfflineTestPaperId,
                            TotalMarks = viewModel.TotalMarks,
                            TestDate = viewModel.TestDate,
                            TestInTime = viewModel.TestInTime == null ? DateTime.Now.Date : Convert.ToDateTime(viewModel.TestInTime.Trim()),
                            TestOutTime = viewModel.TestOutTime == null ? DateTime.Now.Date : Convert.ToDateTime(viewModel.TestOutTime.Trim()),
                            Title = viewModel.Title,
                            Media = viewModel.Media
                        });

                        if (result.Success)
                        {
                            var roleUserId = User.Identity.GetUserId();
                            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                            if (roles == "BranchAdmin")
                            {
                                var resultEmail = sendMailfromBranchAdmin(viewModel.Title, viewModel.Media);
                            }
                            cmsResult = SendNotificationToStudent(viewModel, subject, viewModel.OfflineTestPaperId, listOfEmail, listOfName, listOfNumber,
                                finalParentPlayerId, finalStudentPlayerId);
                            var errorMessage = "";
                            foreach (var messageFailure in cmsResult.Results)
                            {
                                errorMessage += messageFailure.Message + "<br/>";
                            }
                            Success(result.Results.FirstOrDefault().Message + "<br />" + errorMessage);
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
            }
            return View(viewModel);
        }
    }
}