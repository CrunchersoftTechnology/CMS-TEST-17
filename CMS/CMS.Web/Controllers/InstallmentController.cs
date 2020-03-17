using System.Web.Mvc;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Domain.Infrastructure;
using System.Linq;
using CMS.Domain.Storage.Projections;
using CMS.Web.ViewModels;
using System.Collections.Generic;
using CMS.Domain.Models;
using CMS.Web.Models;
using CMS.Web.Helpers;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Data.SqlClient;

namespace CMS.Web.Controllers
{
    public class InstallmentController : BaseController
    {
        string constr = ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString;

        readonly IInstallmentService _installmentService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IClassService _classService;
        readonly IStudentService _studentService;
        readonly IEmailService _emailService;
        readonly IBranchService _branchService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly ISmsService _smsService;
        readonly ISendNotificationService _sendNotificationService;
        readonly ISubjectService _subjectService;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureServices;

        public InstallmentController(IClientAdminService clientAdminService, IInstallmentService installmentService, ILogger logger, IRepository repository,
            IClassService classService, IStudentService studentService, IEmailService emailService, IConfigureServices configureServices,
            IBranchService branchService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService,
            ISmsService smsService, ISendNotificationService sendNotificationService,
            ISubjectService subjectService)
        {
            _installmentService = installmentService;
            _logger = logger;
            _repository = repository;
            _classService = classService;
            _studentService = studentService;
            _emailService = emailService;
            _branchService = branchService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _smsService = smsService;
            _sendNotificationService = sendNotificationService;
            _subjectService = subjectService;
            _clientAdminService = clientAdminService;
            _configureServices = configureServices;
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Index(int? classId, string userId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);


            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;


            ViewBag.BranchId = roles == "BranchAdmin" ? projection.BranchId : 0;
            ViewBag.ClientId = roles == "Client" ? projectionClient.ClientId : 0;
            ViewBag.ClassList = (from c in _classService.GetClasses()
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();
            var installment = (roles == "Admin" && classId == null && userId == null) ? _installmentService.GetAllInstallments().ToList()
               : (roles == "BranchAdmin" && classId == null && userId == null) ? _installmentService.GetInstallmentsByBranchId(projection.BranchId).ToList()
                : (roles == "Client" && classId == null && userId == null) ? _installmentService.GetInstallmentsByClientId(projectionClient.ClientId).ToList() : _installmentService.GetInstallments((int)classId, userId).ToList();
            ViewBag.ClassId = classId;
            ViewBag.UserId = userId;
            var viewModelList = AutoMapper.Mapper.Map<List<InstallmentProjection>, InstallmentViewModel[]>(installment);
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
            return View(viewModelList);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var classes = _classService.GetClasses().ToList();
            if (roles == "Admin")
            {
                var branchList = _branchService.GetAllBranches().ToList();

                ViewBag.BranchId = null;

                return View(new InstallmentViewModel
                {
                    Branches = new SelectList(branchList, "BranchId", "Name"),
                    CurrentUserRole = roles,
                    Classes = new SelectList(classes, "ClassId", "Name")
                });
            }
            else if (roles == "Client")
            {
                var branchList = _branchService.GetAllBranches().ToList();
                var projection = _clientAdminService.GetClientAdminById(roleUserId);

                ViewBag.ClientId = projection.ClientId;
                return View(new InstallmentViewModel
                {
                    Branches = new SelectList(branchList, "BranchId", "Name"),
                    CurrentUserRole = roles,
                    BranchId = projection.ClientId,
                    BranchName = projection.ClientName,
                    Classes = new SelectList(classes, "ClassId", "Name")
                });

            }

            else if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(roleUserId);

                ViewBag.BranchId = projection.BranchId;
                return View(new InstallmentViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projection.BranchId,
                    BranchName = projection.BranchName,
                    Classes = new SelectList(classes, "ClassId", "Name")
                });
            }

            return View();
        }
        // Default Mail Id Through Send mail
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(InstallmentViewModel viewModel)
        {
            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.BranchId = viewModel.BranchId;
            ViewBag.UserId = viewModel.UserId;
            ViewBag.installmentCount = viewModel.InstallmentNo;

            var students = _studentService.GetStudentById(viewModel.UserId);


            //var selectedSubjects =students.BatchName +" - "+ string.Join(",", subject.TrimEnd(',')) ;
            var studentFinalFee = _studentService.GetStudentFeeByUserId(viewModel.UserId);
            var paidFee = _installmentService.GetCountInstallment(viewModel.UserId);
            var remainFee = studentFinalFee - paidFee;
            var roles = viewModel.CurrentUserRole;
            var branchId = viewModel.BranchId;
            var branchName = viewModel.BranchName;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;
            var remainingPayment = remainFee - viewModel.Payment;
            var ReceivedFee = paidFee + viewModel.Payment;
            viewModel.RemainingFee = remainingPayment;
            viewModel.TotalFee = (studentFinalFee).ToString();

            List<string> listOfPlayerId = new List<string>();

            if (ModelState.IsValid)
            {
                if (remainFee < viewModel.Payment && remainFee != 0)
                {
                    ViewBag.StudentName = viewModel.StudentName;
                    ViewBag.TotalFee = viewModel.TotalFee;
                    ViewBag.RemainingFee = viewModel.RemainingFee;
                    _logger.Warn("Payment amount is exceded!");
                    Warning("Payment amount is exceded!", true);
                }
                else if (remainFee == 0)
                {
                    ViewBag.StudentName = viewModel.StudentName;
                    ViewBag.TotalFee = viewModel.TotalFee;
                    ViewBag.RemainingFee = viewModel.RemainingFee;
                    _logger.Warn("Your Remaining Fee is Nill!");
                    Warning("Your Remaining Fee is Nill!", true);
                }
                else
                {
                    var result = _installmentService.Save(new Installment
                    {
                        ClassId = viewModel.ClassId,
                        UserId = viewModel.UserId,
                        Payment = viewModel.Payment,
                        RemainingFee = remainingPayment,
                        ReceiptBookNumber = viewModel.ReceiptBookNumber,
                        ReceiptNumber = viewModel.ReceiptNumber,
                        ReceivedFee = ReceivedFee,
                    });
                    if (result.Success)
                    {
                        ViewBag.ClassId = 0;
                        ViewBag.BranchId = 0;
                        ViewBag.clientId = 0;
                        string createdName = "", userName = "";
                        bool isBranchAdmin = false;
                        bool isClientAdmin = false;
                        if (viewModel.CurrentUserRole == "BranchAdmin")
                        {
                            isBranchAdmin = true;
                            createdName = viewModel.BranchName;
                            userName = User.Identity.GetUserName();
                        }
                        else if (viewModel.CurrentUserRole == "Client")
                        {
                            isClientAdmin = true;
                            createdName = viewModel.ClientName;
                            createdName = viewModel.BranchName;
                            userName = User.Identity.GetUserName();
                        }
                        else
                        {
                            createdName = viewModel.BranchName;
                            userName = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
                        }


                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/InstallmentMailDesign.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{BatchWithSubjectName}", viewModel.StudBatch);
                        body = body.Replace("{BranchName}", createdName);
                        body = body.Replace("{StudentName}", viewModel.StudentName);
                        body = body.Replace("{ClassName}", students.ClassName);
                        body = body.Replace("{TotalFees}", studentFinalFee.ToString());
                        body = body.Replace("{PaidFees}", viewModel.Payment.ToString());
                        body = body.Replace("{RemainingFees}", remainingPayment.ToString());
                        body = body.Replace("{UserName}", userName);



                        var emailMessage = new MailModel
                        {
                            Body = body,
                            Subject = "Fees Payment",
                            To = viewModel.Email,
                            IsBranchAdmin = isBranchAdmin
                        };
                        _emailService.Send(emailMessage);

                        if (viewModel.SMS == true)
                        {

                            var smsModel = new SmsModel
                            {
                                Message = "Hi " + viewModel.ClassName + "\r\n Student Name: " + viewModel.StudentName + "\r\nPayment: " + viewModel.Payment + "\r\nRemaining Fees: " + viewModel.RemainingFee + "\r\nFees paid successfully.",
                                SendTo = viewModel.ParentContact + "," + viewModel.StudentContact
                            };
                            var smsResult = _smsService.SendMessage(smsModel);
                        }
                        if (viewModel.AppNotification == true)
                        {
                            listOfPlayerId.Add(viewModel.ParentAppPlayerId);
                            listOfPlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                            var sendAppNotification = new SendNotification
                            {
                                Message = "Fee-Student Name: " + viewModel.StudentName + " Payment: " + viewModel.Payment + " Remaining Fees: " + viewModel.RemainingFee + " Fees paid successfully.",
                                PlayerIds = listOfPlayerId,
                                AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                                RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                            };

                            if (listOfPlayerId.Count > 0)
                            {
                                var sendNotificationResult = _sendNotificationService.SendNotificationByPlayersId(sendAppNotification);
                            }
                        }

                        Success(result.Results.FirstOrDefault().Message);
                        ModelState.Clear();
                        viewModel = new InstallmentViewModel();
                        ViewBag.StudentName = "";
                        ViewBag.TotalFee = "0";
                        ViewBag.RemainingFee = "0";
                        ViewBag.installmentCount = "";
                    }
                    else
                    {
                        _logger.Warn(result.Results.FirstOrDefault().Message);
                        Warning(result.Results.FirstOrDefault().Message, true);
                        ViewBag.StudentName = viewModel.StudentName;
                        ViewBag.TotalFee = viewModel.TotalFee;
                        ViewBag.RemainingFee = viewModel.RemainingFee;
                    }
                }
            }
            else
            {
                ViewBag.StudentName = viewModel.StudentName;
                ViewBag.TotalFee = viewModel.TotalFee;
                ViewBag.RemainingFee = viewModel.RemainingFee;
            }

            if (roles == "Admin")
            {
                var branchList = _branchService.GetAllBranches().ToList();
                viewModel.Branches = new SelectList(branchList, "BranchId", "Name");
            }
            else if (roles == "Client")
            {

                var branchList = _branchService.GetAllBranches().ToList();
                viewModel.Branches = new SelectList(branchList, "BranchId", "Name");
            }
            else if (roles == "BranchAdmin")
            {

            }
            viewModel.CurrentUserRole = roles;
            var classes = _classService.GetClasses().ToList();
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");

            return View(viewModel);
        }
        */

        // Dyanamic Mail Id Through Send Mail 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(InstallmentViewModel viewModel)
        {
            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.BranchId = viewModel.BranchId;
            ViewBag.UserId = viewModel.UserId;
            ViewBag.installmentCount = viewModel.InstallmentNo;

            var students = _studentService.GetStudentById(viewModel.UserId);


            //var selectedSubjects =students.BatchName +" - "+ string.Join(",", subject.TrimEnd(',')) ;
            var studentFinalFee = _studentService.GetStudentFeeByUserId(viewModel.UserId);
            var paidFee = _installmentService.GetCountInstallment(viewModel.UserId);
            var remainFee = studentFinalFee - paidFee;
            var roles = viewModel.CurrentUserRole;
            var branchId = viewModel.BranchId;
            var branchName = viewModel.BranchName;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;
            var remainingPayment = remainFee - viewModel.Payment;
            var ReceivedFee = paidFee + viewModel.Payment;
            viewModel.RemainingFee = remainingPayment;
            viewModel.TotalFee = (studentFinalFee).ToString();

            List<string> listOfPlayerId = new List<string>();

            if (ModelState.IsValid)
            {
                if (remainFee < viewModel.Payment && remainFee != 0)
                {
                    ViewBag.StudentName = viewModel.StudentName;
                    ViewBag.TotalFee = viewModel.TotalFee;
                    ViewBag.RemainingFee = viewModel.RemainingFee;
                    _logger.Warn("Payment amount is exceded!");
                    Warning("Payment amount is exceded!", true);
                }
                else if (remainFee == 0)
                {
                    ViewBag.StudentName = viewModel.StudentName;
                    ViewBag.TotalFee = viewModel.TotalFee;
                    ViewBag.RemainingFee = viewModel.RemainingFee;
                    _logger.Warn("Your Remaining Fee is Nill!");
                    Warning("Your Remaining Fee is Nill!", true);
                }
                else
                {
                    var result = _installmentService.Save(new Installment
                    {
                        ClassId = viewModel.ClassId,
                        UserId = viewModel.UserId,
                        Payment = viewModel.Payment,
                        RemainingFee = remainingPayment,
                        ReceiptBookNumber = viewModel.ReceiptBookNumber,
                        ReceiptNumber = viewModel.ReceiptNumber,
                        ReceivedFee = ReceivedFee,
                    });
                    if (result.Success)
                    {
                        ViewBag.ClassId = 0;
                        ViewBag.BranchId = 0;
                        ViewBag.clientId = 0;
                        string createdName = "", userName = "";
                        bool isBranchAdmin = false;
                        bool isClientAdmin = false;
                        if (viewModel.CurrentUserRole == "BranchAdmin")
                        {
                            isBranchAdmin = true;
                            createdName = viewModel.BranchName;
                            userName = User.Identity.GetUserName();
                        }
                        else if (viewModel.CurrentUserRole == "Client")
                        {
                            isClientAdmin = true;
                            createdName = viewModel.ClientName;
                            createdName = viewModel.BranchName;
                            userName = User.Identity.GetUserName();
                        }
                        else
                        {
                            createdName = viewModel.BranchName;
                            userName = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
                        }


                        string body = string.Empty;
                        using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/InstallmentMailDesign.html")))
                        {
                            body = reader.ReadToEnd();
                        }
                        body = body.Replace("{BatchWithSubjectName}", viewModel.StudBatch);
                        body = body.Replace("{BranchName}", createdName);
                        body = body.Replace("{StudentName}", viewModel.StudentName);
                        body = body.Replace("{ClassName}", students.ClassName);
                        body = body.Replace("{TotalFees}", studentFinalFee.ToString());
                        body = body.Replace("{PaidFees}", viewModel.Payment.ToString());
                        body = body.Replace("{RemainingFees}", remainingPayment.ToString());
                        body = body.Replace("{UserName}", userName);

                        var roleUserId = User.Identity.GetUserId();
                        var rolesc = _aspNetRolesService.GetCurrentUserRole(roleUserId);

                        var projectionClient = rolesc == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
                        var ClientId = projectionClient.ClientId;

                        var projection = _configureServices.GetConfigureById(ClientId);
                        var emailid = projection.email_id;
                        var epassword = projection.emailpassword;
                        var institute = projection.name;
                        var senderid = projection.sender_id;
                        var username = projection.username;
                        var password = projection.password;

                        var className = _classService.GetClassById(viewModel.ClassId);
                        var clsname = className.Name;
                        var emailMessage = new MailModel
                        {
                            Body = body,
                            Subject = "Fees Payment",
                            To = viewModel.Email,
                            Emailid = emailid,
                            Password = epassword,
                            IsBranchAdmin = isBranchAdmin
                        };
                        _emailService.MailSend(emailMessage);

                        if (viewModel.SMS == true)
                        {

                            var smsModel = new SmsModel
                            {
                                SenderId = senderid,
                                UserName = username,
                                Password = password,

                                Message = "Hi " + institute + "\r\n Student Name: " + viewModel.StudentName + "\r\nPayment: " + viewModel.Payment + "\r\nRemaining Fees: " + viewModel.RemainingFee + "\r\nFees Paid Successfully.",
                                SendTo = viewModel.ParentContact + "," + viewModel.StudentContact
                            };
                            var smsResult = _smsService.DynamicsSendMessage(smsModel);
                        }
                        if (viewModel.AppNotification == true)
                        {
                            listOfPlayerId.Add(viewModel.ParentAppPlayerId);
                            listOfPlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                            var sendAppNotification = new SendNotification
                            {
                                Message = "Fee-Student Name: " + viewModel.StudentName + " Payment: " + viewModel.Payment + " Remaining Fees: " + viewModel.RemainingFee + " Fees Paid Successfully.",
                                PlayerIds = listOfPlayerId,
                                AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                                RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                            };

                            if (listOfPlayerId.Count > 0)
                            {
                                var sendNotificationResult = _sendNotificationService.SendNotificationByPlayersId(sendAppNotification);
                            }
                        }

                        Success(result.Results.FirstOrDefault().Message);
                        ModelState.Clear();
                        viewModel = new InstallmentViewModel();
                        ViewBag.StudentName = "";
                        ViewBag.TotalFee = "0";
                        ViewBag.RemainingFee = "0";
                        ViewBag.installmentCount = "";
                    }
                    else
                    {
                        _logger.Warn(result.Results.FirstOrDefault().Message);
                        Warning(result.Results.FirstOrDefault().Message, true);
                        ViewBag.StudentName = viewModel.StudentName;
                        ViewBag.TotalFee = viewModel.TotalFee;
                        ViewBag.RemainingFee = viewModel.RemainingFee;
                    }
                }
            }
            else
            {
                ViewBag.StudentName = viewModel.StudentName;
                ViewBag.TotalFee = viewModel.TotalFee;
                ViewBag.RemainingFee = viewModel.RemainingFee;
            }

            if (roles == "Admin")
            {
                var branchList = _branchService.GetAllBranches().ToList();
                viewModel.Branches = new SelectList(branchList, "BranchId", "Name");
            }
            else if (roles == "Client")
            {

                var branchList = _branchService.GetAllBranches().ToList();
                viewModel.Branches = new SelectList(branchList, "BranchId", "Name");
            }
            else if (roles == "BranchAdmin")
            {

            }
            viewModel.CurrentUserRole = roles;
            var classes = _classService.GetClasses().ToList();
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");

            return View(viewModel);
        }
        public ActionResult GetStudent(int classId)
        {
            var subjects = _studentService.GetStudentsByClassId(classId).Where(x => x.ClassId == classId).Select(x => new { x.UserId, x.FirstName, x.MiddleName, x.LastName, x.Email, x.DOB });
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStudentList(string query, int classId, int branchId)
        {
            var result = _installmentService.GetStudentsAutoComplete(query, classId, branchId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Search()
        {
            return View();
        }

        public JsonResult GetStudentFinalFee(string userId)
        {
            var studentFinalFee = _studentService.GetStudentFeeByUserId(userId);
            var paidFee = _installmentService.GetCountInstallment(userId);
            var installmentCount = _installmentService.GetInstallmentCount(userId);
            var result = new
            {
                StudentFinalFee = studentFinalFee,
                PaidFee = paidFee,
                installmentCount = installmentCount
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetStudentByBranchId(int classId, int branchId)
        {
            var subjects = _studentService.GetStudentsByBranchAndClassId(classId, branchId).Select(x => new { x.UserId, x.FirstName, x.MiddleName, x.LastName, x.Email, x.DOB });
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(int id)
        {

            var projection = _installmentService.GetInstallmentByInstallmentId(id);
            var installmentCount = _installmentService.GetInstallmentCount(projection.UserId);
            var studentSubjects = string.Join(",", projection.StudentSubjects);
            if (projection == null)
            {
                _logger.Warn(string.Format("Payment does not Exists {0}.", id));
                Warning("Payment does not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<InstallmentProjection, InstallmentViewModel>(projection);
            viewModel.InstallmentNo = installmentCount;
            viewModel.RemainingFeeFinal = projection.RemainingFee + projection.Payment;
            viewModel.StudBatch = projection.StudBatch + " ( " + studentSubjects + " )";
            return View(viewModel);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        [HttpPost]
        public ActionResult Edit(InstallmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var installmentExist = _repository.Project<Installment, bool>(
                                    installments => (from installment in installments
                                                     where installment.InstallmentId == viewModel.InstallmentId
                                                     select installment).Any());
                if (!installmentExist)
                {
                    _logger.Warn(string.Format("Payment not exists."));
                    Danger(string.Format("Payment not exists."));
                }

                var result = _installmentService.Update(new Installment
                {
                    InstallmentId = viewModel.InstallmentId,
                    ReceiptBookNumber = viewModel.ReceiptBookNumber,
                    ReceiptNumber = viewModel.ReceiptNumber,
                    Payment = viewModel.Payment,
                    ReceivedFee = viewModel.FinalFee - viewModel.RemainingFee
                });

                if (result.Success)
                {
                    string userName = User.Identity.GetUserName() + "(" + "Master Admin" + ")"; ;
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/InstallmentMailDesign.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{BatchWithSubjectName}", viewModel.StudBatch);
                    body = body.Replace("{BranchName}", viewModel.BranchName);
                    body = body.Replace("{StudentName}", viewModel.StudentName);
                    body = body.Replace("{ClassName}", viewModel.ClassName);
                    body = body.Replace("{TotalFees}", viewModel.FinalFee.ToString());
                    body = body.Replace("{PaidFees}", viewModel.Payment.ToString());
                    body = body.Replace("{RemainingFees}", viewModel.RemainingFee.ToString());
                    body = body.Replace("{UserName}", userName);
                    var emailMessage = new MailModel
                    {
                        Body = body,
                        Subject = "Updated Payment",
                        To = viewModel.Email,
                    };
                    _emailService.Send(emailMessage);

                    List<string> listOfPlayerId = new List<string>();

                    if (viewModel.SMS == true)
                    {

                        var smsModel = new SmsModel
                        {


                            Message = "Hi " + viewModel.ClassName + "\r\n Student Name: " + viewModel.StudentName + "\r\nPayment: " + viewModel.Payment + "\r\nRemaining Fees: " + viewModel.RemainingFee + "\r\nFees Paid Successfully.",
                            SendTo = viewModel.ParentContact + "," + viewModel.StudentContact
                        };

                        var smsResult = _smsService.SendMessage(smsModel);
                    }
                    if (viewModel.AppNotification == true)
                    {
                        listOfPlayerId.Add(viewModel.ParentAppPlayerId);
                        listOfPlayerId = listOfPlayerId.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        var sendAppNotification = new SendNotification
                        {
                            Message = "Fee-Student Name: " + viewModel.StudentName + " Payment: " + viewModel.Payment + " Remaining Fees: " + viewModel.RemainingFee + " Fees Paid Successfully.",
                            PlayerIds = listOfPlayerId,
                            AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                            RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
                        };

                        if (listOfPlayerId.Count > 0)
                        {
                            var sendNotificationResult = _sendNotificationService.SendNotificationByPlayersId(sendAppNotification);
                        }
                    }

                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            return View(viewModel);
        }
    }
}