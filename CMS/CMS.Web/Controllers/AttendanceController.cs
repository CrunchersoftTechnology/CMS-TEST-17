using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using CMS.Web.ViewModels;
using FileHelpers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class AttendanceController : BaseController
    {
        readonly IClassService _classService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IBatchService _batchService;
        readonly IAttendanceService _attendanceService;
        readonly ITeacherService _teacherService;
        readonly IStudentService _studentService;
        readonly IBranchService _branchService;
        readonly IBranchAdminService _branchAdminService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IEmailService _emailService;
        readonly ISmsService _smsService;
        readonly ISendNotificationService _sendNotificationService;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureServices;

        public AttendanceController(IClientAdminService clientAdminService, IClassService classService, ILogger logger, IRepository repository,
            IBatchService batchService, IAttendanceService attendanceService, ITeacherService teacherService,
            IStudentService studentService, IBranchService branchService, IBranchAdminService branchAdminService,
            IAspNetRoles aspNetRolesService, IEmailService emailService, ISmsService smsService, IConfigureServices configureServices,
            ISendNotificationService sendNotificationService)
        {
            _classService = classService;
            _logger = logger;
            _repository = repository;
            _batchService = batchService;
            _attendanceService = attendanceService;
            _teacherService = teacherService;
            _studentService = studentService;
            _branchService = branchService;
            _branchAdminService = branchAdminService;
            _aspNetRolesService = aspNetRolesService;
            _emailService = emailService;
            _smsService = smsService;
            _sendNotificationService = sendNotificationService;
            _clientAdminService = clientAdminService;
            _configureServices = configureServices;
        }

        // GET: Attendance
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var attendanceList = roles == "Admin" ? _attendanceService.GetAttendance().ToList() : roles == "Client" ? _attendanceService.GetAttendanceByClientId(projectionClient.ClientId).ToList() : roles == "BranchAdmin" ? _attendanceService.GetAttendanceByBranchId(projection.BranchId).ToList() : null;

            var viewModel = AutoMapper.Mapper.Map<List<AttendanceProjection>, AttendanceViewModel[]>(attendanceList);

            var result = new MachineAttendence
            {
                MachineSerial = "S1C100",
                PunchDataList = new List<PunchData>()
                {
                    new PunchData { PunchId = 1, PunchDateTime = DateTime.UtcNow },
                    new PunchData { PunchId = 2, PunchDateTime = DateTime.UtcNow },
                    new PunchData { PunchId = 3, PunchDateTime = DateTime.UtcNow }
                }
            };
            ViewBag.ClassList = (from c in _classService.GetClassesByClientId(projectionClient.ClientId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();
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

            return View(viewModel);
        }

        public ActionResult Create()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

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

                return View(new AttendanceViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles
                });
            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(roleUserId);

                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                ViewBag.ClientId = projection.ClientId;
                ViewBag.CurrentUserRole = roles;
                ViewBag.BranchId = 0;
                return View(new AttendanceViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });


            }
            else if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(roleUserId);

                ViewBag.BranchId = projection.BranchId;
                ViewBag.CurrentUserRole = roles;
                return View(new AttendanceViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projection.BranchId,
                    BranchName = projection.BranchName
                });
            }

            return View();
        }

        [HttpPost]
        public JsonResult SaveAttendance(AttendanceViewModel viewModel)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                cmsResult = _attendanceService.Save(new Attendance
                {
                    ClassId = viewModel.ClassId,
                    BatchId = viewModel.BatchId,
                    Date = viewModel.Date,
                    Activity = viewModel.Activity,
                    UserId = viewModel.UserId,
                    StudentAttendence = viewModel.StudentAttendence,
                    BranchId = viewModel.BranchId,
                    ClientId =projection.ClientId,
                    IsManual = true
                });
                if (cmsResult.Success)
                {
                    var bodySubject = "Web portal - Attendance create";
                    var batch = _batchService.GetBatcheById(viewModel.BatchId);
                    var batchTime = "( " + batch.InTime.ToShortTimeString() + " To " + batch.OutTime.ToShortTimeString() + " )";
                    var message = "Class Name : " + viewModel.ClassName + " <br/>Batch Name : " + viewModel.BatchName + " " + batchTime
                                 + "Attendance Date :" + viewModel.Date.ToString("dd/MM/yyyy") + "<br/>Attendance created successfully";
                    SendMailToAdminDyanamic(message, bodySubject);
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

        public ActionResult GetBatches(int classId)
        {
            var batches = _batchService.GetAllBatchClsId(classId).Select(x => new { x.BatchId, x.BatchName });
            return Json(batches, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudent(int classId, int batchId, int branchId, string Date)
        {
            DateTime attendanceDate = DateTime.Parse(Date);
            var projection = _studentService.GetStudentByClsandBatch(classId, attendanceDate, branchId, batchId).ToList();
            var viewModel = AutoMapper.Mapper.Map<List<StudentProjection>, StudentViewModel[]>(projection);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id)
        {

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection1 = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var projection = _attendanceService.GetAttendance(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Attendance not Exists."));
                Warning("Attendance not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<AttendanceProjection, AttendanceViewModel>(projection);
            var batchList = (from b in _batchService.GetAllBatchClsId(viewModel.ClassId)
                             select new SelectListItem
                             {
                                 Value = b.BatchId.ToString(),
                                 Text = string.Format("{0}", b.BatchName)
                             }).ToList();
            var classList = _classService.GetClassesByClientId(projection1.ClientId);
            var teacherList = (from c in _teacherService.GetTeachersBind()
                               select new SelectListItem
                               {
                                   Value = c.UserId,
                                   Text = string.Format("{0} {1} {2}", c.FirstName, c.MiddleName, c.LastName)
                               }).ToList();
            ViewBag.BatchList = batchList;
            ViewBag.TeacherList = teacherList;
            ViewBag.ClassList = classList;
            ViewBag.UserId = viewModel.UserId;
            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.BatchId = viewModel.BatchId;
            ViewBag.BranchId = viewModel.BranchId;


            // var roleUserId = User.Identity.GetUserId();
            // var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "Admin")
            {
                viewModel.CurrentUserRole = roles;
                var branchList = _branchService.GetAllBranches().ToList();
                ViewBag.BranchList = branchList;
                return View(viewModel);
            }
            else if (roles == "Client")
            {

                viewModel.CurrentUserRole = roles;
                var branchList = _branchService.GetAllBranchesByClientId(projection1.ClientId).ToList();
                ViewBag.BranchList = branchList;
                return View(viewModel);

            }
            else if (roles == "BranchAdmin")
            {
                viewModel.CurrentUserRole = roles;
            }

            return View(viewModel);
            // return RedirectToAction("Index");
        }

        public ActionResult GeneratePdf(int id)
        {
            var projection = _attendanceService.GetAttendance(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Attendance not Exists."));
                Warning("Attendance not Exists.");
                return RedirectToAction("Index");
            }

            List<AttendanceReportViewModel> attendanceReport = new List<AttendanceReportViewModel>();

            var StudentsList = _studentService
                                .GetStudentByClsandBatch(projection.ClassId, projection.Date, projection.BranchId, projection.BatchId)
                                .OrderBy(x => x.FirstName);
            var studentSIdList = projection.StudentAttendence.Split(',').Select(x => int.Parse(x));
            var studentInTime = projection.InTime != null ? JsonConvert.DeserializeObject<List<PunchDetails>>(projection.InTime) : null;
            var studentOutTime = projection.OutTime != null ? JsonConvert.DeserializeObject<List<PunchDetails>>(projection.OutTime) : null;

            foreach (var student in StudentsList)
            {
                if (studentSIdList.Contains(student.SId))
                {
                    var studentIn = studentInTime != null ? studentInTime.Where(x => x.SId == student.SId).FirstOrDefault() : null;
                    var studentOut = studentOutTime != null ? studentOutTime.Where(x => x.SId == student.SId).FirstOrDefault() : null;
                    attendanceReport.Add(new AttendanceReportViewModel
                    {
                        Name = string.Format("{0} {1}", student.FirstName, student.LastName),
                        Status = "Present",
                        InTime = studentIn != null ? studentIn.PunchTime : "-",
                        OutTime = studentOut != null ? studentOut.PunchTime : "-"
                    });
                }
                else
                {
                    attendanceReport.Add(new AttendanceReportViewModel
                    {
                        Name = string.Format("{0} {1}", student.FirstName, student.LastName),
                        Status = "Absent",
                        InTime = "-",
                        OutTime = "-"
                    });
                }
            }

            ViewBag.Branch = projection.BranchName;
            ViewBag.Class = projection.ClassName;
            ViewBag.Batch = projection.BatchName;
            ViewBag.Date = projection.Date.ToString("dd-MM-yyyy");
            ViewBag.StudentAttendanceList = attendanceReport;
            return View();
        }

        public JsonResult UpdateAttendance(AttendanceViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                cmsResult = _attendanceService.Update(new Attendance
                {
                    AttendanceId = viewModel.AttendanceId,
                    ClassId = viewModel.ClassId,
                    BatchId = viewModel.BatchId,
                    UserId = viewModel.UserId,
                    Activity = viewModel.Activity,
                    Date = viewModel.Date,
                    StudentAttendence = viewModel.StudentAttendence,
                    BranchId = viewModel.BranchId,
                    IsManual = true
                });
                if (cmsResult.Success)
                {
                    var bodySubject = "Web portal changes - Attendence update";
                    var batch = _batchService.GetBatcheById(viewModel.BatchId);
                    var batchTime = "( " + batch.InTime.ToShortTimeString() + " To " + batch.OutTime.ToShortTimeString() + " )";
                    var message = "Class Name : " + viewModel.ClassName + " <br/>Batch Name : " + viewModel.BatchName + " " + batchTime
                                         + "<br/>Attendance Date : " + viewModel.Date.ToString("dd/MM/yyyy") + "<br/>Attendance updated successfully";
                    SendMailToAdmin(message, bodySubject);
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

        public JsonResult ReadAttendanceFile()
        {
            var engine = new FileHelperEngine<AttendanceFile>();
            AttendanceFile[] result = engine.ReadFile(@"G:\PRITI\Websites crunchersoft\AGL_001 - Copy.TXT");
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [DelimitedRecord("\t")]
        [IgnoreFirst]
        public class AttendanceFile
        {
            [FieldTrim(TrimMode.Both)]
            public string No;

            [FieldTrim(TrimMode.Both)]
            public string TMNo;

            [FieldTrim(TrimMode.Both)]
            public string EnNo;

            [FieldTrim(TrimMode.Both)]
            public string Name;

            [FieldTrim(TrimMode.Both)]
            public string GMNo;

            [FieldTrim(TrimMode.Both)]
            public string Mode;

            [FieldTrim(TrimMode.Both)]
            public string INOUT;

            [FieldTrim(TrimMode.Both)]
            public string Antipass;

            [FieldTrim(TrimMode.Both)]
            public string DateTime;
        }

        public JsonResult SendAttendance(AttendanceViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            var attendanceList = _attendanceService.GetAttendanceByMultipleIds(viewModel.SelectedAttendance);
            List<SendAttendanceClass> list = new List<SendAttendanceClass>();

            foreach (var attendance in attendanceList)
            {
                var attendanceSIdList = attendance.StudentAttendence.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(int.Parse);
                var studentList = _studentService.GetStudentsForSendAttendance(
                                    attendance.ClassId,attendance.ClientId, attendance.BranchId, attendance.BatchId, attendance.Date)
                                    .Where(x => x.DOJ.Date <= attendance.Date.Date);

                foreach (var student in studentList)
                {
                    var message = attendanceSIdList.Contains(student.SId) ? attendance.Date.ToString("dd/MM/yyyy") + " - Present" : attendance.Date.ToString("dd/MM/yyyy") + " - Absent";
                    list.Add(new SendAttendanceClass
                    {
                        SId = student.SId,
                       
                        Message = message,
                        StudentContact = student.StudentContact,
                        ParentContact = student.ParentContact,
                        ParentAppPlayerId = student.parentAppPlayerId,
                        Email = student.Email,
                        BranchName = student.BranchName,
                        StudentName = student.Name,
                        BatchName = attendance.BatchName,
                        SubjectName = attendance.SubjectName

                    });
                }
            }
            var resultGroup = (from attendance in list
                               group attendance by new
                               {
                                   attendance.SId,
                                   attendance.StudentContact,
                                   attendance.ParentContact,
                                   attendance.ParentAppPlayerId,
                                   attendance.Email,
                                   attendance.BranchName,
                                   attendance.StudentName,
                                   attendance.BatchName,
                                   attendance.SubjectName
                               } into grouping
                               select new SendAttendanceClass
                               {
                                   SId = grouping.Key.SId,
                                   Email = grouping.Key.Email,
                                   StudentContact = grouping.Key.StudentContact,
                                   ParentContact = grouping.Key.ParentContact,
                                   ParentAppPlayerId = grouping.Key.ParentAppPlayerId,
                                   BranchName = grouping.Key.BranchName,
                                   StudentName = grouping.Key.StudentName,
                                   Message = grouping.Key.BatchName + "<br/>" + string.Join("<br/>", grouping.Select(x => x.Message)),
                               });

            var finalList = (from attendance in resultGroup
                             group attendance by new
                             {
                                 attendance.SId,
                                 attendance.StudentContact,
                                 attendance.ParentContact,
                                 attendance.ParentAppPlayerId,
                                 attendance.Email,
                                 attendance.BranchName,
                                 attendance.StudentName
                             } into grouping
                             select new SendAttendanceClass
                             {
                                 SId = grouping.Key.SId,
                                 Email = grouping.Key.Email,
                                 StudentContact = grouping.Key.StudentContact,
                                 ParentContact = grouping.Key.ParentContact,
                                 ParentAppPlayerId = grouping.Key.ParentAppPlayerId,
                                 BranchName = grouping.Key.BranchName,
                                 StudentName = grouping.Key.StudentName,
                                 Message = string.Join("<br/>", grouping.Select(x => x.Message)),
                             });

            if (viewModel.Email)
            {
                var result = SendEmailDynamic(finalList);
                cmsResult.Results.Add(new Result { Message = "Email sent successfully.", IsSuccessful = true });
            }
            #region Not confirm for sms sending
            //if (viewModel.SMS)
            //{
            //    var result = SendSMS(resultGroup);
            //    var sendCount = result.Results.Where(x => x.IsSuccessful).Count();
            //    var failCount = result.Results.Where(x => !x.IsSuccessful).Count();
            //    cmsResult.Results.Add(new Result { Message = sendCount + " SMS sent successfully and " + failCount + " send failed.", IsSuccessful = true });
            //} 
            #endregion
            if (viewModel.AppNotification)
            {
                var result = SendAppNotification(resultGroup);
                cmsResult.Results.Add(new Result { Message = "App notification sent successfully.", IsSuccessful = true });
            }
            int resultCount = cmsResult.Results.Where(x => x.IsSuccessful == true).Count();
            if (resultCount > 0)
            {
                var result = _attendanceService.UpdateMultipleAttendance(viewModel.SelectedAttendance);
            }
            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public class SendAttendanceClass
        {
            public int SId { get; set; }

            public string Message { get; set; }
            public string StudentContact { get; set; }
            public string ParentContact { get; set; }
            public string Email { get; set; }
            public string ParentAppPlayerId { get; set; }
            public string BranchName { get; set; }
            public string StudentName { get; set; }
            public string SubjectName { get; set; }
            public string BatchName { get; set; }
            public List<string> BatchList { get; set; }
        }

        public CMSResult SendEmail(IEnumerable<SendAttendanceClass> finalList)
        {
            var cmsResult = new CMSResult();
            var i = 0;
            MailModel[] emailAddress = new MailModel[finalList.Count()];
            foreach (var email in finalList)
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchAdminEmail}", "( " + User.Identity.GetUserName() + " )");
                body = body.Replace("{BranchName}", email.BranchName);
                body = body.Replace("{ModuleName}", "Hi , " + email.StudentName + "<br/>your Attendance below <br/><br/>"
                                 + "Batch:" + email.Message + "<br/>");

                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = "Attendance Details",
                    To = email.Email
                };
                emailAddress[i] = emailMessage;
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailAddress, cancellationToken));

            cmsResult.Results.Add(new Result { Message = "Sent successfully.", IsSuccessful = true });

            return cmsResult;
        }

        public CMSResult SendEmailDynamic(IEnumerable<SendAttendanceClass> finalList)
        {
            var cmsResult = new CMSResult();
            var i = 0;
            MailModel[] emailAddress = new MailModel[finalList.Count()];
            foreach (var email in finalList)
            {
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchAdminEmail}", "( " + User.Identity.GetUserName() + " )");
                body = body.Replace("{BranchName}", email.BranchName);
                body = body.Replace("{ModuleName}", "Hi , " + email.StudentName + "<br/>your Attendance below <br/><br/>"
                                 + "Batch:" + email.Message + "<br/>");

                var roleUserId = User.Identity.GetUserId();
                var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;

                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var emailid = projection.email_id;
                var password = projection.emailpassword;

                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = "Attendance Details",
                    To = email.Email,
                    Emailid = emailid,
                    Password = password

                };
                emailAddress[i] = emailMessage;
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailAddress, cancellationToken));

            cmsResult.Results.Add(new Result { Message = "Sent successfully.", IsSuccessful = true });

            return cmsResult;
        }
        public CMSResult SendSMS(IEnumerable<SendAttendanceClass> resultGroup)
        {
            var cmsResult = new CMSResult();

            var i = 0;
            SmsModel[] smsModelsMessage = new SmsModel[resultGroup.Count()];
            foreach (var sms in resultGroup)
            {
                List<string> contactList = new List<string>();
                contactList.Add(sms.StudentContact);
                //contactList.Add(sms.ParentContact);
                var listOfContact = string.Join(",", contactList);
                var smsModel = new SmsModel
                {
                    Message = sms.Message,
                    SendTo = listOfContact
                };
                smsModelsMessage[i] = smsModel;
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _smsService.StartProcessing(smsModelsMessage, cancellationToken));

            cmsResult.Results.Add(new Result { Message = "SMS Send Successfully", IsSuccessful = true });
            return cmsResult;
        }

        public CMSResult SendSMSDynamic(IEnumerable<SendAttendanceClass> resultGroup)
        {
            var cmsResult = new CMSResult();

            var i = 0;
            SmsModel[] smsModelsMessage = new SmsModel[resultGroup.Count()];
            foreach (var sms in resultGroup)
            {
                List<string> contactList = new List<string>();
                contactList.Add(sms.StudentContact);
                //contactList.Add(sms.ParentContact);
                var listOfContact = string.Join(",", contactList);

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

                var smsModel = new SmsModel
                {
                    SenderId = senderid,
                    UserName = username,
                    Password = password,

                    Message = sms.Message,
                    SendTo = listOfContact
                };
                smsModelsMessage[i] = smsModel;
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _smsService.StartProcessing(smsModelsMessage, cancellationToken));

            cmsResult.Results.Add(new Result { Message = "SMS Send Successfully", IsSuccessful = true });
            return cmsResult;
        }
        public CMSResult SendAppNotification(IEnumerable<SendAttendanceClass> resultGroup)
        {
            var cmsResult = new CMSResult();

            List<string> listOfPlayerId = new List<string>();
            var i = 0;
            SendNotificationByPlayerId[] notification = new SendNotificationByPlayerId[resultGroup.Count()];
            foreach (var sms in resultGroup)
            {
                listOfPlayerId.Add(sms.ParentAppPlayerId);
                var listOfContact = string.Join(",", listOfPlayerId);
                if (!(sms.ParentAppPlayerId == "null" || sms.ParentAppPlayerId == ""))
                {
                    var sendAppNotification = new SendNotificationByPlayerId
                    {
                        Message = "Attendance-" + sms.Message,
                        PlayerIds = sms.ParentAppPlayerId,
                        AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                        RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                    };
                    notification[i] = sendAppNotification;
                    i++;
                }
            }
            if (listOfPlayerId.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
                cmsResult.Results.Add(new Result { Message = "Send Addendance Notification Successfully.", IsSuccessful = true });
            }
            else
            {
                cmsResult.Results.Add(new Result { Message = "No one is registered in parent app.", IsSuccessful = false });
            }

            return cmsResult;
        }

        public void SendMailToAdminDyanamic(string message, string bodySubject)
        {
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                var branchName = branchAdmin.BranchName;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", branchName);
                body = body.Replace("{ModuleName}", "Branch Name :" + branchName + "<br/>" + message);
                body = body.Replace("{BranchAdminEmail}", "( " + User.Identity.GetUserName() + " )");

                var roleUserId = User.Identity.GetUserId();
              

                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;
                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var emailid = projection.email_id;
                var password = projection.emailpassword;
                var to = projection.email_id;
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    Emailid = emailid,
                    To=to,
                    Password = password,
                    IsBranchAdmin = true
                };
                _emailService.MailSend(emailMessage);
            }

            if (roles == "Client")
            {
                var clientAdmin = _clientAdminService.GetClientAdminById(User.Identity.GetUserId());
                var clientName = clientAdmin.ClientName;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{ClientName}", clientName);
                body = body.Replace("{ModuleName}", "Client Name :" + clientName + "<br/>" + message);
                body = body.Replace("{ClientAdminEmail}", "( " + User.Identity.GetUserName() + " )");
                var roleUserId = User.Identity.GetUserId();


                var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                var ClientId = projectionClient.ClientId;
                var projection = _configureServices.GetConfigureById(ClientId);
                var clientId = projection.ClientId;
                var emailid = projection.email_id;
                var password = projection.emailpassword;
                var to = projection.email_id;
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    To = to,
                    Emailid = emailid,
                    Password = password,
                    IsClientAdmin = true
                };
                _emailService.MailSend(emailMessage);
            }
        }


        public void SendMailToAdmin(string message, string bodySubject)
        {
            var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                var branchName = branchAdmin.BranchName;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", branchName);
                body = body.Replace("{ModuleName}", "Branch Name :" + branchName + "<br/>" + message);
                body = body.Replace("{BranchAdminEmail}", "( " + User.Identity.GetUserName() + " )");
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsBranchAdmin = true
                };
                _emailService.Send(emailMessage);
            }
            if (roles == "Client")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                var branchName = branchAdmin.BranchName;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", branchName);
                body = body.Replace("{ModuleName}", "Branch Name :" + branchName + "<br/>" + message);
                body = body.Replace("{ClientAdminEmail}", "( " + User.Identity.GetUserName() + " )");
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsClientAdmin = true
                };
                _emailService.Send(emailMessage);
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public FileResult Export(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "AttendanceReport.pdf");
            }
        }
    }

    public class PunchDetails
    {
        public int SId { get; set; }
        public string PunchTime { get; set; }
    }
}