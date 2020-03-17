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
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class NotificationController : BaseController
    {
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchService _branchService;
        readonly IBranchAdminService _branchAdminService;
        readonly IStudentService _studentService;
        readonly IEmailService _emailService;
        readonly ISmsService _smsService;
        readonly ITeacherService _teacherService;
        readonly IRepository _repository;
        readonly ISendNotificationService _sendNotificationService;
        readonly INotificationService _notificationService;
        readonly ISubjectService _subjectService;
        readonly ILogger _logger;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureServices;


        public NotificationController(IClientAdminService clientAdminService, IAspNetRoles aspNetRolesService, IBranchService branchService,
            IBranchAdminService branchAdminService, IStudentService studentService, IEmailService emailService, IConfigureServices configureServices,
            ISmsService smsService, ITeacherService teacherService, IRepository repository, ISendNotificationService sendNotificationService,
            INotificationService notificationService, ISubjectService subjectService, ILogger logger)
        {
            _aspNetRolesService = aspNetRolesService;
            _branchService = branchService;
            _branchAdminService = branchAdminService;
            _studentService = studentService;
            _emailService = emailService;
            _smsService = smsService;
            _teacherService = teacherService;
            _repository = repository;
            _sendNotificationService = sendNotificationService;
            _notificationService = notificationService;
            _subjectService = subjectService;
            _logger = logger;
            _clientAdminService =clientAdminService;
            _configureServices = configureServices;
        }

        // GET: Notification
        public ActionResult Index()
        {

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;


            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.ClientId;
            }
            return View();
        }

        public ActionResult Create()
        {
         //   var fdf = _notificationService.GetAutoNotificationsToSend();
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

                return View(new NotificationViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles
                });
            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(roleUserId);
                ViewBag.ClientId = projection.ClientId;


                var branchList = (from b in _branchService.GetAllBranchesByClientId((int)projection.ClientId)
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

                ViewBag.BranchId = 0;
                ViewBag.CurrentUserRole = roles;

                return View(new NotificationViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName,
                    Classes = classList,
                    Branches = branchList,
                });
            }
            else if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(roleUserId);
                ViewBag.BranchId = projection.BranchId;
                var classList = (from c in _studentService.GetStudentsByBranchId(projection.BranchId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();
                ViewBag.CurrentUserRole = roles;
                return View(new NotificationViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projection.BranchId,
                    BranchName = projection.BranchName,
                    Classes = classList
                });
            }
            return View();
        }

        public JsonResult SendNotification(NotificationViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            List<string> listOfNumber = new List<string>();
            List<string> listOfEmail = new List<string>();
            List<string> listOfPlayerId = new List<string>();
            List<string> listOfStudentPlayerId = new List<string>();
            List<string> listOfName = new List<string>();
            var finalPlayerId = new List<ListOfPlayerId>();
            var finalStudentPlayerId = new List<ListOfPlayerId>();

            if (ModelState.IsValid)
            {
                bool isSend = false;
                if (viewModel.NotificationAutoDate == null || viewModel.NotificationAutoDate == DateTime.Now.Date)
                {
                    isSend = true;
                    var studentParentNoList = _studentService.GetAllStudentParentList();
                    var branchAdminContactList = _branchAdminService.GetBranchAdminContactList();
                    var clientAdminContactList = _clientAdminService.GetClientAdminContactList();
                    var teacherContactList = _teacherService.GetTeacherContactList();

                    if (viewModel.Student || viewModel.Teacher || viewModel.Parent || viewModel.BranchAdmin || viewModel.ClientAdmin)
                    {
                        var branchIds = viewModel.SelectedBranches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                        if (viewModel.Student || viewModel.Parent)
                        {
                            var classIds = viewModel.SelectedClasses.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);

                            var batchIds = viewModel.SelectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                            var b = _repository.LoadList<Batch>(x => batchIds.Contains(x.BatchId)).ToList();

                            studentParentNoList = studentParentNoList.Where(
                                                        x => branchIds.Contains(x.BranchId) &&
                                                        batchIds.Contains(x.BatchId) &&
                                                        classIds.Contains(x.ClassId));


                            foreach (var student in studentParentNoList)
                            {
                                bool isExists = true;
                                if (isExists)
                                {
                                    if (viewModel.Student)
                                    {
                                        if (student.studentAppPlayerId != null && student.studentAppPlayerId != "")
                                        {
                                            var details = new ListOfPlayerId
                                            {
                                                SId = student.SId,
                                                ParentPlayerId = student.studentAppPlayerId
                                            };
                                            finalStudentPlayerId.Add(details);
                                        }
                                        listOfEmail.Add(student.Email);
                                        listOfNumber.Add(student.StudentContact);
                                        listOfName.Add(student.Name);
                                        listOfPlayerId.Add(student.parentAppPlayerId);
                                    }
                                    if (viewModel.Parent)
                                    {
                                        if (student.parentAppPlayerId != null && student.parentAppPlayerId != "")
                                        {
                                            var details = new ListOfPlayerId
                                            {
                                                SId = student.SId,
                                                ParentPlayerId = student.parentAppPlayerId
                                            };
                                            finalPlayerId.Add(details);
                                        }
                                        listOfEmail.Add(student.ParentEmailId);
                                        listOfNumber.Add(student.ParentContact);
                                        listOfName.Add(student.MiddleName +" "+ student.LastName);
                                        listOfPlayerId.Add(student.parentAppPlayerId);
                                    }
                                }
                            }
                        }
                        if (viewModel.BranchAdmin)
                        {
                            branchAdminContactList = branchAdminContactList.Where(x => branchIds.Contains(x.BranchId));
                            listOfEmail.AddRange(branchAdminContactList.Select(x => x.Email));
                            listOfNumber.AddRange(branchAdminContactList.Select(x => x.ContactNo));
                            listOfName.AddRange(branchAdminContactList.Select(x => x.Name));
                        }

                        if (viewModel.ClientAdmin)
                        {
                            clientAdminContactList = clientAdminContactList.Where(x => branchIds.Contains(x.ClientId));
                            listOfEmail.AddRange(clientAdminContactList.Select(x => x.Email));
                            listOfNumber.AddRange(clientAdminContactList.Select(x => x.ContactNo));
                            listOfName.AddRange(clientAdminContactList.Select(x => x.Name));
                        }
                        if (viewModel.Teacher)
                        {
                            teacherContactList = teacherContactList.Where(x => branchIds.Contains(x.BranchId));
                            listOfEmail.AddRange(teacherContactList.Select(x => x.Email));
                            listOfNumber.AddRange(teacherContactList.Select(x => x.ContactNo));
                            listOfName.AddRange(teacherContactList.Select(x => x.Name));
                        }
                    }
                    else if (viewModel.AllUser)
                    {
                        listOfEmail.AddRange(clientAdminContactList.Select(x => x.Email));
                        listOfEmail.AddRange(branchAdminContactList.Select(x => x.Email));
                        listOfEmail.AddRange(studentParentNoList.Select(x => x.Email));
                        listOfEmail.AddRange(teacherContactList.Select(x => x.Email));
                        listOfEmail.AddRange(studentParentNoList.Select(x => x.ParentEmailId));
                        listOfNumber.AddRange(branchAdminContactList.Select(x => x.ContactNo));
                        listOfNumber.AddRange(clientAdminContactList.Select(x => x.ContactNo));
                        listOfNumber.AddRange(studentParentNoList.Select(x => x.StudentContact));
                        listOfNumber.AddRange(teacherContactList.Select(x => x.ContactNo));
                        listOfNumber.AddRange(studentParentNoList.Select(x => x.ParentContact));
                        listOfPlayerId.AddRange(studentParentNoList.Select(x => x.parentAppPlayerId));
                        listOfPlayerId.AddRange(studentParentNoList.Select(x => x.studentAppPlayerId));
                        foreach (var item in studentParentNoList)
                        {
                            if (item.parentAppPlayerId != null && item.parentAppPlayerId != "")
                            {
                                var details = new ListOfPlayerId
                                {
                                    SId = item.SId,
                                    ParentPlayerId = item.parentAppPlayerId
                                };
                                finalPlayerId.Add(details);
                            }
                            if (item.studentAppPlayerId != null && item.studentAppPlayerId != "")
                            {
                                var details = new ListOfPlayerId
                                {
                                    SId = item.SId,
                                    ParentPlayerId = item.studentAppPlayerId
                                };
                                finalStudentPlayerId.Add(details);
                            }
                        }

                        listOfName.AddRange(branchAdminContactList.Select(x => x.Name));
                        listOfName.AddRange(studentParentNoList.Select(x => x.Name));
                        listOfName.AddRange(teacherContactList.Select(x => x.Name));
                        listOfName.AddRange(studentParentNoList.Select(x => x.MiddleName +" "+ x.LastName));
                       
                        var totalCount = listOfEmail.Count + listOfNumber.Count + listOfPlayerId.Count;
                        if (totalCount == 0)
                            cmsResult.Results.Add(new Result { Message = "Atleast one user must be added.", IsSuccessful = false });
                        //listOfPlayerId.AddRange(studentParentNoList.Select(x => x.studentAppPlayerId));
                    }

                    if (viewModel.Email)
                    {
                        if (listOfEmail.Count > 0)
                        {
                            // Default Mail Id Through Mail Send
                            //  var result = SendEmail(viewModel.NotificationMessage, listOfEmail, listOfName);

                            // Dynamic Mail Id Through Mail Send
                            var result = SendEmailDyanamic(viewModel.NotificationMessage, listOfEmail, listOfName);

                            if (result == true)
                            {
                                cmsResult.Results.Add(new Result { Message = "Email Sent Successfully.", IsSuccessful = true });
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
                            // Default Sender Id Through SMS Send
                            // var result = SendSMS(viewModel.NotificationMessage, listOfNumber);

                            // Dynamic Sender Id Through SMS Send
                            var result = DynamicSendSMS(viewModel.NotificationMessage, listOfNumber);

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
                }
                int savenNotificationId = 0;
                #region Rough
                //int resultCount = cmsResult.Results.Where(x => x.IsSuccessful == true).Count();
                //if (viewModel.AppNotification && (viewModel.Student || viewModel.Parent || viewModel.AllUser))
                //{
                //    List<string> savePlayerId = new List<string>();
                //    savePlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                //    resultCount += savePlayerId.Count;
                //}
                //  if (resultCount > 0)
                //  { 
                #endregion
                try
                {
                    var roles = _aspNetRolesService.GetCurrentUserRole(User.Identity.GetUserId());
                    var notification = new Notification
                    {
                        NotificationMessage = viewModel.NotificationMessage,
                        AllUser = viewModel.AllUser,
                        SelectedBranches = viewModel.SelectedBranches != null ? viewModel.SelectedBranches : "",
                        SelectedClasses = viewModel.SelectedClasses != null ? viewModel.SelectedClasses : "",
                        SelectedBatches = viewModel.SelectedBatches != null ? viewModel.SelectedBatches : "",
                        StudentCount = (viewModel.Student ? viewModel.StudentCount : viewModel.AllUser ? viewModel.StudentCount : 0),
                        ParentCount = viewModel.Parent ? viewModel.ParentCount : viewModel.AllUser ? viewModel.ParentCount : 0,
                        TeacherCount = viewModel.Teacher ? viewModel.TeacherCount : viewModel.AllUser ? viewModel.TeacherCount : 0,
                        BranchAdminCount = viewModel.BranchAdmin ? viewModel.BranchAdminCount : viewModel.AllUser ? viewModel.BranchAdminCount : 0,
                        ClientAdminCount = viewModel.ClientAdmin ? viewModel.ClientAdminCount : viewModel.AllUser ? viewModel.ClientAdminCount : 0,
                        Media = viewModel.Media,
                        NotificationAutoDate = viewModel.NotificationAutoDate,
                        IsSend = isSend,
                        UserName = User.Identity.GetUserName()
                    };
                    var result = _notificationService.Save(notification);
                    savenNotificationId = notification.NotificationId;
                    if (result.Success)
                    {
                        if (roles == "BranchAdmin")
                        {
                            var isUser = "";
                            var branchAdmin = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                            var branchId = branchAdmin.BranchId;
                            var branchName = branchAdmin.BranchName;
                            if (viewModel.AllUser)
                            {
                                isUser = "All User";
                            }
                            else
                            {
                                isUser = viewModel.Student ? "Student" : viewModel.AllUser ? "All User" : "";
                                isUser += viewModel.Parent ? ", Parent" : "";
                                isUser += viewModel.Teacher ? ", Teacher" : "";
                                isUser += viewModel.BranchAdmin ? ", BranchAdmin" : "";
                            }

                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
                            {
                                body = reader.ReadToEnd();
                            }

                            body = body.Replace("{UserName}", User.Identity.GetUserName());
                            body = body.Replace("{BranchName}", branchName);
                            body = body.Replace("{NotificationMessage}", "Notification Message : " + viewModel.NotificationMessage + "<br/>" + viewModel.Media + " sent to " + isUser);

                            var emailMessage = new MailModel
                            {
                                Body = body,
                                Subject = "Web portal changes Notifications ",
                                IsBranchAdmin = true
                            };
                            _emailService.Send(emailMessage);
                        }
                        else if (roles == "Client")
                        {

                            var isUser = "";
                            var clientAdmin = _clientAdminService.GetClientAdminById(User.Identity.GetUserId());
                            var clientId = clientAdmin.ClientId;
                            var clientName = clientAdmin.ClientName;
                            if (viewModel.AllUser)
                            {
                                isUser = "All User";
                            }
                            else
                            {
                                isUser = viewModel.Student ? "Student" : viewModel.AllUser ? "All User" : "";
                                isUser += viewModel.Parent ? ", Parent" : "";
                                isUser += viewModel.Teacher ? ", Teacher" : "";
                                isUser += viewModel.BranchAdmin ? ", BranchAdmin" : "";
                                isUser += viewModel.ClientAdmin ? ", ClientAdmin" : "";
                            }

                            string body = string.Empty;
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
                            {
                                body = reader.ReadToEnd();
                            }

                            body = body.Replace("{UserName}", User.Identity.GetUserName());
                            body = body.Replace("{ClientName}", clientName);
                            body = body.Replace("{NotificationMessage}", "Notification Message : " + viewModel.NotificationMessage + "<br/>" + viewModel.Media + " sent to " + isUser);

                            var emailMessage = new MailModel
                            {
                                Body = body,
                                Subject = "Web portal changes Notifications ",
                                IsClientAdmin = true
                            };
                            _emailService.Send(emailMessage);
                        }
                        cmsResult.Results.Add(new Result { Message = result.Results[0].Message, IsSuccessful = true });
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message + "catch notification");
                    throw;
                }

                //   }
                if (viewModel.NotificationAutoDate == null || viewModel.NotificationAutoDate == DateTime.Now.Date)
                {
                    if (viewModel.AppNotification && (viewModel.Student || viewModel.Parent || viewModel.AllUser))
                    {
                        var response = SendAppNotification(viewModel, listOfPlayerId, finalPlayerId, savenNotificationId, listOfStudentPlayerId, finalStudentPlayerId);

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

        public bool SendEmail(string notificationTextMessage, List<string> listOfEmail, List<string> listOfName)
        {
            string userRole = "";
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                userRole = branchAdmin.BranchName;
            }
            else if (roles == "Client")
            {
                var clientAdmin = _clientAdminService.GetClientAdminById(roleUserId);
                userRole = clientAdmin.ClientName;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            }

            var i = 0;
            MailModel[] emailAddress = new MailModel[listOfEmail.Count];
            foreach (var Name in listOfName)
            {
                if (listOfEmail[i] != "")
                {
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{BranchName}", userRole);
                    body = body.Replace("{NotificationMessage}", notificationTextMessage);
                    body = body.Replace("{UserName}", Name);

                    var emailMessage = new MailModel
                    {
                        Body = body,
                        Subject = "Notice",
                        To = listOfEmail[i]
                    };
                    emailAddress[i] = emailMessage;
                }
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailAddress, cancellationToken));
            #region foreach
            //var i = 0;
            //foreach (var Name in listOfName)
            //{
            //    string body = string.Empty;
            //    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
            //    {
            //        body = reader.ReadToEnd();
            //    }
            //    body = body.Replace("{BranchName}", userRole);
            //    body = body.Replace("{NotificationMessage}", notificationTextMessage);
            //    body = body.Replace("{UserName}", Name);

            //    var emailMessage = new MailModel
            //    {
            //        Body = body,
            //        Subject = "Notice",
            //        To = listOfEmail[i]

            //    };

            //    _emailService.Send(emailMessage); i++;
            //}
            #endregion
            #region parallerForeach
            //var i = 0;
            //Parallel.ForEach(listOfEmail.AsEnumerable(), row =>
            //{
            //    string body = string.Empty;
            //    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
            //    {
            //        body = reader.ReadToEnd();
            //    }
            //    body = body.Replace("{BranchName}", userRole);
            //    body = body.Replace("{NotificationMessage}", notificationTextMessage);
            //    body = body.Replace("{UserName}", listOfName[i]);

            //    var emailMessage = new MailModel
            //    {
            //        Body = body,
            //        Subject = "Notice",
            //        To = listOfEmail[i]
            //    };

            //    _emailService.Send(emailMessage);
            //    i++;
            //}); 
            #endregion
            return true;
        }

        public bool SendEmailDyanamic(string notificationTextMessage, List<string> listOfEmail, List<string> listOfName)
        {
            string userRole = "";
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                userRole = branchAdmin.BranchName;
            }
            else if (roles == "Client")
            {
                var clientAdmin = _clientAdminService.GetClientAdminById(roleUserId);
                userRole = clientAdmin.ClientName;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            }

            var i = 0;
            MailModel[] emailAddress = new MailModel[listOfEmail.Count];
            foreach (var Name in listOfName)
            {
                if (listOfEmail[i] != "")
                {
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{BranchName}", userRole);
                    body = body.Replace("{NotificationMessage}", notificationTextMessage);
                    body = body.Replace("{UserName}", Name);

                    // var roleUserId = User.Identity.GetUserId();
                    // var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

                    var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                    var ClientId = projectionClient.ClientId;

                    var projection = _configureServices.GetConfigureById(ClientId);
                    var clientId = projection.ClientId;
                    var emailid = projection.email_id;
                    var password = projection.emailpassword;
                    var name = projection.name;
                    var aboutus = projection.aboutus;

                    var emailMessage = new MailModel
                    {
                        Body = body,
                        Subject = "Notice",
                        Emailid = emailid,
                        Password = password,

                        To = listOfEmail[i]
                    };
                    emailAddress[i] = emailMessage;
                }
                i++;
            }
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(emailAddress, cancellationToken));
            #region foreach
            //var i = 0;
            //foreach (var Name in listOfName)
            //{
            //    string body = string.Empty;
            //    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
            //    {
            //        body = reader.ReadToEnd();
            //    }
            //    body = body.Replace("{BranchName}", userRole);
            //    body = body.Replace("{NotificationMessage}", notificationTextMessage);
            //    body = body.Replace("{UserName}", Name);

            //    var emailMessage = new MailModel
            //    {
            //        Body = body,
            //        Subject = "Notice",
            //        To = listOfEmail[i]

            //    };

            //    _emailService.Send(emailMessage); i++;
            //}
            #endregion
            #region parallerForeach
            //var i = 0;
            //Parallel.ForEach(listOfEmail.AsEnumerable(), row =>
            //{
            //    string body = string.Empty;
            //    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/NotificationMailDesign.html")))
            //    {
            //        body = reader.ReadToEnd();
            //    }
            //    body = body.Replace("{BranchName}", userRole);
            //    body = body.Replace("{NotificationMessage}", notificationTextMessage);
            //    body = body.Replace("{UserName}", listOfName[i]);

            //    var emailMessage = new MailModel
            //    {
            //        Body = body,
            //        Subject = "Notice",
            //        To = listOfEmail[i]
            //    };

            //    _emailService.Send(emailMessage);
            //    i++;
            //}); 
            #endregion
            return true;
        }

        public CMSResult SendSMS(string notificationTextMessage, List<string> listOfNumber)
        {
            var listOfContact = string.Join(",", listOfNumber);
            var smsModel = new SmsModel
            {
                Message = notificationTextMessage,
                SendTo = listOfContact
            };

            return _smsService.SendMessage(smsModel);
        }

        public CMSResult DynamicSendSMS(string notificationTextMessage, List<string> listOfNumber)
        {
            CMSResult cmsresult = new CMSResult();
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


            var listOfContact = string.Join(",", listOfNumber);
            var smsModel = new SmsModel
            {
                SenderId = senderid,
                UserName = username,
                Password = password,
                Name = name,

                Message = notificationTextMessage,
                SendTo = listOfContact
            };

            return _smsService.SendMessage(smsModel);
        }

        public CMSResult SendAppNotification(NotificationViewModel viewModel, List<string> listOfPlayerId, List<ListOfPlayerId> finalPlayerId, int savedNotificationId, List<string> listOfStudentPlayerId, List<ListOfPlayerId> finalStudentPlayerId)
        {
            var notificationList = new List<SendNotificationByPlayerId>();

            var getfinalPlayerId = (from list in finalPlayerId
                                    group finalPlayerId by new
                                    {
                                        list.SId,
                                        list.ParentPlayerId
                                    } into grouping
                                    select new ListOfPlayerId
                                    {
                                        SId = grouping.Key.SId,
                                        ParentPlayerId = grouping.Key.ParentPlayerId
                                    }).ToList();
            var getstudentfinalPlayerId = (from list in finalStudentPlayerId
                                    group finalStudentPlayerId by new
                                    {
                                        list.SId,
                                        list.ParentPlayerId
                                    } into grouping
                                    select new ListOfPlayerId
                                    {
                                        SId = grouping.Key.SId,
                                        ParentPlayerId = grouping.Key.ParentPlayerId
                                    }).ToList();

            var cmsResult = new CMSResult();
            listOfPlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            listOfStudentPlayerId = listOfStudentPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            
            foreach (var playerId in getfinalPlayerId)
            {
                var studentSId = playerId.SId;
                var sendAppNotification = new SendNotificationByPlayerId
                {
                    Message = "Notice-" + viewModel.NotificationMessage + "$^$" + playerId.SId + "@" + savedNotificationId,
                    PlayerIds = playerId.ParentPlayerId,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                };
                notificationList.Add(sendAppNotification);                    
            }
              
            foreach (var playerId in getstudentfinalPlayerId)
            {
                var studentSId = playerId.SId;
                var sendAppNotification = new SendNotificationByPlayerId
                {
                    Message = "Notice-" + viewModel.NotificationMessage + "$^$" + playerId.SId + "@" + savedNotificationId,
                    PlayerIds = playerId.ParentPlayerId,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.StudentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.StudentRestAppId]
                };
                notificationList.Add(sendAppNotification);
            }            

            var notification = notificationList.ToArray();

            if (viewModel.AllUser)
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
            else if(notificationList.Count > 0)
            {
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _sendNotificationService.StartProcessingByPlayerId(notification, cancellationToken));
            }
            else if(notificationList.Count == 0){
                cmsResult.Results.Add(new Result { Message = "No one is registered in app.", IsSuccessful = false });
                return cmsResult;
            }

            cmsResult.Results.Add(new Result { Message = "App Notification Sent Successfully.", IsSuccessful = true });
            return cmsResult;
        }

        public ActionResult Details(int id)
        {
            //  string classes = _studentService.GetClassesByMultipleBranchId(SelectedBranches).Select(x => new { x.ClassId, x.ClassName }).ToString();

            var notification = _notificationService.GetNotificationById(id);
            var viewModel = AutoMapper.Mapper.Map<NotificationProjection, NotificationViewModel>(notification);

            var commaseperatedBranchList = notification.SelectedBranches ?? string.Empty;
            var branchIds = commaseperatedBranchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var branches = _repository.LoadList<Branch>(x => branchIds.Contains(x.BranchId)).ToList();
            List<string> BranchList = new List<string>();
            BranchList = branches.Select(x => x.Name).ToList();
            var Branchlist = string.Join(",", BranchList);
            viewModel.SelectedBranches = Branchlist;

            var commaseperatedClassList = notification.SelectedClasses ?? string.Empty;
            var ClassIds = commaseperatedClassList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var Classees = _repository.LoadList<Class>(x => ClassIds.Contains(x.ClassId)).ToList();
            List<string> ClassList = new List<string>();
            ClassList = Classees.Select(x => x.Name).ToList();
            var Classlist = string.Join(",", ClassList);
            viewModel.SelectedClasses = Classlist;

            var commaseperatedBatchList = notification.SelectedBatches ?? string.Empty;
            var BatchIds = commaseperatedBatchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var Batch = _repository.LoadList<Batch>(x => BatchIds.Contains(x.BatchId)).ToList();
            List<string> BatchList = new List<string>();
            BatchList = Batch.Select(x => x.Name).ToList();
            var BatchesList = string.Join(",", BatchList);
            string batch = "";
            foreach (var s in Batch)
            {
                batch += string.Format("{0},", s.Name);
            }
            viewModel.SelectedBatches = batch.TrimEnd(',');

            return View(viewModel);
        }
    }

    public class ListOfPlayerId
    {
        public int SId { get; set; }
        public string ParentPlayerId { get; set; }
    }
}