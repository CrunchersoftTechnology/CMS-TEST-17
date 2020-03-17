using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.CustomAttributes;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Configuration;

using System.Web;

namespace CMS.Web.Controllers
{
    [Roles(Common.Constants.AdminRole, Common.Constants.StudentRole, Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    //[RequireHttpsAttribute]
    public class StudentController : BaseController
    {

        string constr = ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString;


        readonly IClassService _classService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IBoardService _boardService;
        readonly IStudentService _studentService;
        readonly IApplicationUserService _applicationUserService;
        readonly ISubjectService _subjectService;
        readonly IInstallmentService _installmentService;
        readonly IBatchService _batchService;
        readonly IEmailService _emailService;
        readonly ISchoolService _schoolService;
        readonly IBranchService _branchService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly ITeacherService _teacherService;
        readonly IApiService _apiService;
        readonly ILocalDateTimeService _localDateTimeService;
        readonly ISmsService _smsService;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureServices;


        public StudentController(IClientAdminService clientAdminService, IClassService classService, ILogger logger, IRepository repository,
            IBoardService boardService, IStudentService studentService, IConfigureServices configureServices,
            IApplicationUserService applicationUserService, ISubjectService subjectService,
            IInstallmentService installmentService, IBatchService batchService, IEmailService emailService,
            ISchoolService schoolService, IBranchService branchService, IAspNetRoles aspNetRolesService,
            IBranchAdminService branchAdminService, ITeacherService teacherService,
            IApiService apiService, ILocalDateTimeService localDateTimeService, ISmsService smsService)
        {
            _classService = classService;
            _logger = logger;
            _repository = repository;
            _boardService = boardService;
            _studentService = studentService;
            _applicationUserService = applicationUserService;
            _subjectService = subjectService;
            _installmentService = installmentService;
            _batchService = batchService;
            _emailService = emailService;
            _schoolService = schoolService;
            _branchService = branchService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _teacherService = teacherService;
            _apiService = apiService;
            _localDateTimeService = localDateTimeService;
            _smsService = smsService;
            _clientAdminService = clientAdminService;
            _configureServices = configureServices;
        }


        [Roles(Common.Constants.AdminRole, Common.Constants.BranchAdminRole, Common.Constants.ClientAdminRole)]
        public ActionResult Index(int? id)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            //var projection2 = roles == "Client" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            //var projection = projection1 != null ? projection1 : projection2 != null ? projection2 : null;



            ViewBag.ClassList = (from c in _classService.GetClasses()
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();

            ViewBag.ClassId = id;
            var students = (roles == "Admin" && id == null) ? _studentService.GetAllStudents().ToList()
                                 : (roles == "Admin" && id != null) ? _studentService.GetStudentsByClassId((int)id).ToList()
                                 : (roles == "Client" && id == null) ? _studentService.GetStudentsByClientId(projectionClient.ClientId).ToList()
                                   : (roles == "Client" && id != null) ? _studentService.GetStudentsByClientAndClassId((int)id, projectionClient.ClientId).ToList()
                                     : (roles == "BranchAdmin" && id == null) ? _studentService.GetStudentsByBranchId(projection.BranchId).ToList()
                                   : (roles == "BranchAdmin" && id != null) ? _studentService.GetStudentsByBranchAndClassId((int)id, projection.BranchId).ToList() : null;

            var viewModelList = AutoMapper.Mapper.Map<List<StudentProjection>, StudentViewModel[]>(students);
            // return View(viewModelList);
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

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(int? id)
        {

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

            var branchList = (from c in _branchService.GetAllBranchesByClientId(Convert.ToInt32(projection.ClientId))
                              select new SelectListItem
                              {
                                  Value = c.BranchId.ToString(),
                                  Text = c.Name
                              }).ToList();

            var boardList = (from c in _boardService.GetBoardsByClientId(Convert.ToInt32(projection.ClientId))
                             select new SelectListItem
                             {
                                 Value = c.BoardId.ToString(),
                                 Text = c.Name
                             }).ToList();

            var classList = (from c in _classService.GetClassesByClientId(Convert.ToInt32(projection.ClientId))
                             select new SelectListItem
                             {
                                 Value = c.ClassId.ToString(),
                                 Text = c.Name
                             }).ToList();

            var schoolList = (from s in _schoolService.GetAllSchoolsByClientId(Convert.ToInt32(projection.ClientId))
                              select new SelectListItem
                              {
                                  Value = s.SchoolId.ToString(),
                                  Text = s.Name
                              }).ToList();


            if (roles == "Admin")
            {

                if (id != null)
                {
                    var admissionResult = _apiService.GetAdmission(id);
                    var admission = JsonConvert.DeserializeObject<AdmissionProjection>(admissionResult);

                    var result = _studentService.IsExistAdmission(admission.Email);
                    if (result)
                    {
                        _logger.Warn(admission.FirstName + " " + admission.LastName + "(" + admission.Email + ")" + " student is already added.");
                        Warning(admission.FirstName + " " + admission.LastName + "(" + admission.Email + ")" + " student is already added.", true);
                        ViewBag.BranchId = null;
                        return View(new StudentViewModel
                        {
                            Branches = branchList,
                            CurrentUserRole = roles,
                            Boards = boardList,
                            Classes = classList,
                            Schools = schoolList,
                        });
                    }
                    else
                    {
                        ViewBag.SelectedSubjects = admission.SelectedSubject;
                        ViewBag.BatchId = admission.BatchId;
                        return View(new StudentViewModel
                        {
                            ClientId = Convert.ToInt32(projection.ClientId),
                            Branches = branchList,
                            CurrentUserRole = roles,
                            Boards = boardList,
                            Classes = classList,
                            Schools = schoolList,
                            BoardId = admission.BoardId,
                            ClassId = admission.ClassId,
                            FirstName = admission.FirstName.Trim(),
                            MiddleName = admission.MiddleName == null ? "" : admission.MiddleName.Trim(),
                            LastName = admission.LastName.Trim(),
                            Gender = admission.Gender,
                            Address = admission.Address.Trim(),
                            Pin = admission.Pin,
                            DOB = admission.DOB.Date,
                            BloodGroup = admission.BloodGroup,
                            StudentContact = admission.StudentContact == null ? "" : admission.StudentContact.Trim(),
                            ParentContact = admission.ParentContact.Trim(),
                           // PickAndDrop = admission.PickAndDrop,
                            DOJ = admission.DOJ.Date,
                            SchoolId = admission.SchoolId,
                            SelectedSubject = admission.SelectedSubject,
                            IsWhatsApp = admission.IsWhatsApp,
                            MotherName = admission.MotherName == null ? "" : admission.MotherName,
                            SeatNumber = admission.SeatNumber == null ? "" : admission.SeatNumber,
                            BranchId = admission.BranchId,
                            Email = admission.Email,
                            ConfirmEmail = admission.Email,
                            IsIdExits = id,
                            BranchName = admission.BranchName,
                            BatchId = admission.BatchId,
                           // EmergencyContact = admission.EmergencyContact == null ? "" : admission.EmergencyContact.Trim(),
                            ParentEmailId = admission.ParentEmailId == null ? "" : admission.ParentEmailId.Trim(),
                            PaymentLists = admission.PaymentLists == null ? "" : admission.PaymentLists
                        });
                    }
                }
                else
                {
                    ViewBag.BranchId = null;
                    return View(new StudentViewModel
                    {
                        Branches = branchList,
                        CurrentUserRole = roles,
                        Boards = boardList,
                        Classes = classList,
                        Schools = schoolList,
                    });
                }
            }
            else if (roles == "Client")
            {
                var projectionClient = _clientAdminService.GetClientAdminById(roleUserId);
                ViewBag.ClientId = projectionClient.ClientId;

                if (id != null)
                {
                    var admissionResult = _apiService.GetAdmission(id);
                    var admission = JsonConvert.DeserializeObject<AdmissionProjection>(admissionResult);

                    var result = _studentService.IsExistAdmission(admission.Email);
                    if (result)
                    {
                        _logger.Warn(admission.FirstName + " " + admission.LastName + "(" + admission.Email + ")" + " student is already added.");
                        Warning(admission.FirstName + " " + admission.LastName + "(" + admission.Email + ")" + " student is already added.", true);
                        ViewBag.BranchId = null;
                        return View(new StudentViewModel
                        {
                            Branches = branchList,
                            CurrentUserRole = roles,
                            Boards = boardList,
                            Classes = classList,
                            Schools = schoolList,
                        });
                    }
                    else
                    {
                        ViewBag.SelectedSubjects = admission.SelectedSubject;
                        ViewBag.BatchId = admission.BatchId;
                        return View(new StudentViewModel
                        {

                            ClientId = Convert.ToInt32(projection.ClientId),
                            Branches = branchList,
                            CurrentUserRole = roles,
                            Boards = boardList,
                            Classes = classList,
                            Schools = schoolList,
                            BoardId = admission.BoardId,
                            ClassId = admission.ClassId,
                            FirstName = admission.FirstName.Trim(),
                            MiddleName = admission.MiddleName == null ? "" : admission.MiddleName.Trim(),
                            LastName = admission.LastName.Trim(),
                            Gender = admission.Gender,
                            Address = admission.Address.Trim(),
                            Pin = admission.Pin,
                            DOB = admission.DOB.Date,
                            BloodGroup = admission.BloodGroup,
                            StudentContact = admission.StudentContact == null ? "" : admission.StudentContact.Trim(),
                            ParentContact = admission.ParentContact.Trim(),
                          //  PickAndDrop = admission.PickAndDrop,
                            DOJ = admission.DOJ.Date,
                            SchoolId = admission.SchoolId,
                            SelectedSubject = admission.SelectedSubject,
                            IsWhatsApp = admission.IsWhatsApp,
                            MotherName = admission.MotherName == null ? "" : admission.MotherName,
                            SeatNumber = admission.SeatNumber == null ? "" : admission.SeatNumber,
                            BranchId = admission.BranchId,
                            Email = admission.Email,
                            ConfirmEmail = admission.Email,
                            IsIdExits = id,
                            BranchName = admission.BranchName,
                            BatchId = admission.BatchId,
                            RollNo = admission.RollNo,
                          //  EmergencyContact = admission.EmergencyContact == null ? "" : admission.EmergencyContact.Trim(),
                            ParentEmailId = admission.ParentEmailId == null ? "" : admission.ParentEmailId.Trim(),
                            PaymentLists = admission.PaymentLists == null ? "" : admission.PaymentLists
                        });
                    }
                }
                else
                {
                    ViewBag.BranchId = null;
                    return View(new StudentViewModel
                    {
                        Branches = branchList,
                        CurrentUserRole = roles,
                        Boards = boardList,
                        Classes = classList,
                        Schools = schoolList,
                    });
                }
            }
            else if (roles == "BranchAdmin")
            {
                var projectionBranch = _branchAdminService.GetBranchAdminById(roleUserId);
                ViewBag.BranchId = projectionBranch.BranchId;

                return View(new StudentViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projectionBranch.BranchId,
                    BranchName = projectionBranch.BranchName,
                    Boards = boardList,
                    Classes = classList,
                    Schools = schoolList,
                });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(StudentViewModel viewModel, HttpPostedFileBase file)
        {
            try
            {


                var roleUserId = User.Identity.GetUserId();
                var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

                //var roles = viewModel.CurrentUserRole;
                var branchId = viewModel.BranchId;
                var branchName = viewModel.BranchName;
                var clientId = projection.ClientId;
                var clientName = projection.ClientName;



                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();


                var boardList = (from b in _boardService.GetBoards()
                                 select new SelectListItem
                                 {
                                     Value = b.BoardId.ToString(),
                                     Text = b.Name
                                 }).ToList();

                var classList = (from c in _classService.GetClasses()
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();

                var schoolList = (from s in _schoolService.GetAllSchools()
                                  select new SelectListItem
                                  {
                                      Value = s.SchoolId.ToString(),
                                      Text = s.Name
                                  }).ToList();

                if (ModelState.IsValid)
                {
                    if (viewModel.PaidFee != 0)
                    {
                        var errorMessage = "";
                        if (viewModel.FinalFees < viewModel.PaidFee)
                        {
                            errorMessage = "Payment amount is exceded!";
                            _logger.Warn(errorMessage);
                            Warning(errorMessage, true);
                            if (errorMessage != "")
                            {
                                ViewBag.BatchId = viewModel.BatchId;
                                ViewBag.SelectedSubjects = viewModel.SelectedSubject;
                                ReturnViewModel(roles, viewModel, clientId, clientName, branchId, branchName, boardList, schoolList, classList);
                                return View(viewModel);
                            }
                        }
                        else if (viewModel.ReceiptBookNumber == null || viewModel.ReceiptNumber == null)
                        {
                            errorMessage += "  Receipt Book Number and Receipt Number is required!";
                            _logger.Warn(errorMessage);
                            Warning(errorMessage, true);
                            if (errorMessage != "")
                            {
                                ViewBag.BatchId = viewModel.BatchId;
                                ViewBag.SelectedSubjects = viewModel.SelectedSubject;
                                ReturnViewModel(roles, viewModel, clientId, clientName, branchId, branchName, boardList, schoolList, classList);
                                return View(viewModel);
                            }
                        }
                    }

                    if (viewModel.PaymentErrorMessage != "" && viewModel.PaymentErrorMessage != null)
                    {
                        _logger.Warn(viewModel.PaymentErrorMessage);
                        Warning(viewModel.PaymentErrorMessage, true);
                        ViewBag.SelectedSubjects = viewModel.SelectedSubject;
                        ViewBag.BatchId = viewModel.BatchId;
                        ViewBag.BranchId = viewModel.BranchId;
                        ViewBag.ClientId = viewModel.ClientId;
                        ReturnViewModel(roles, viewModel, clientId, clientName, branchId, branchName, boardList, schoolList, classList);
                    }

                    else
                    {
                        string base64 = Request.Form["imgCropped"];
                        var localTime = (_localDateTimeService.GetDateTime());
                        string filename = Server.MapPath(ConfigurationManager.AppSettings["brochureFile"].ToString());
                        if (System.IO.File.Exists(filename))
                        {
                            var user = new ApplicationUser();
                            string photoPath = "";
                            if (viewModel.PhotoFilePath != null)
                            {
                                photoPath = string.Format(@"~/Images/{0}/{1}.jpg", Common.Constants.StudentImageFolder, user.Id);
                                if (!Common.Constants.ImageTypes.Contains(viewModel.PhotoFilePath.ContentType))
                                {
                                    _logger.Warn("Please choose either a JPEG, JPG or PNG image.");
                                    Warning("Please choose either a JPEG, JPG or PNG image..", true);
                                    viewModel.Schools = schoolList;
                                    viewModel.Classes = classList;
                                    viewModel.Boards = boardList;
                                    viewModel.Branches = branchList;
                                    return View(viewModel);
                                }
                            }
                            else if (viewModel.ImageData != null)
                            {
                                photoPath = string.Format(@"~/Images/{0}/{1}.jpg", Common.Constants.StudentImageFolder, user.Id);
                            }
                            else { photoPath = null; }
                            user.UserName = viewModel.Email;
                            user.Email = viewModel.Email.Trim();
                            user.CreatedBy = User.Identity.Name;
                            user.CreatedOn = localTime;
                            user.PhoneNumber = viewModel.StudentContact == null ? "" : viewModel.StudentContact.Trim();
                            user.Student = new Student
                            {

                                ClientId = projection.ClientId,
                                CreatedBy = User.Identity.Name,
                                CreatedOn = localTime,
                                BoardId = viewModel.BoardId,
                                ClassId = viewModel.ClassId,
                                FirstName = viewModel.FirstName.Trim(),
                                MiddleName = viewModel.MiddleName == null ? "" : viewModel.MiddleName.Trim(),
                                LastName = viewModel.LastName.Trim(),
                                Gender = viewModel.Gender,
                                Address = viewModel.Address.Trim(),
                                Pin = viewModel.Pin,
                                DOB = viewModel.DOB,
                                BloodGroup = viewModel.BloodGroup,
                                StudentContact = viewModel.StudentContact == null ? "" : viewModel.StudentContact.Trim(),
                                ParentContact = viewModel.ParentContact.Trim(),
                             //   PickAndDrop = viewModel.PickAndDrop,
                                DOJ = viewModel.DOJ,
                                SchoolId = viewModel.SchoolId,
                                TotalFees = viewModel.TotalFees,
                                Discount = viewModel.Discount,
                                FinalFees = viewModel.FinalFees,
                                SelectedSubject = viewModel.SelectedSubject,
                              //  PhotoPath = photoPath,
                                IsActive = viewModel.IsActive,
                                IsWhatsApp = viewModel.IsWhatsApp,
                                PunchId = viewModel.PunchId,
                                MotherName = viewModel.MotherName,
                             //   VANArea = viewModel.VANArea,
                                SeatNumber = viewModel.SeatNumber,
                             //   VANFee = viewModel.VANFee,
                                BranchId = viewModel.BranchId,
                                RollNo = viewModel.RollNo,
                                //    EmergencyContact = viewModel.EmergencyContact == null ? "" : viewModel.EmergencyContact.Trim(),
                                ParentEmailId = viewModel.ParentEmailId == null ? "" : viewModel.ParentEmailId.Trim(),
                                BatchId = viewModel.BatchId,
                                PaymentLists = viewModel.PaymentLists
                            };
                            string userPassword = PasswordHelper.GeneratePassword();


                            var result = _applicationUserService.Save(user, userPassword);

                            if (result.Success)
                            {
                                viewModel.UserId = user.Student.UserId;
                                // ViewBag.userId = viewModel.UserId;
                                if (viewModel.IsIdExits != null)
                                {
                                    var admissionResult = _apiService.UpdateAdmission(viewModel.Email);
                                }
                                if (viewModel.PhotoFilePath != null)
                                {
                                    byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                                    MemoryStream myMemStream = new MemoryStream(bytes);
                                    Image fullsizeImage = Image.FromStream(myMemStream);
                                    Image newImage = fullsizeImage.GetThumbnailImage(240, 240, null, IntPtr.Zero);
                                    MemoryStream myResult = new MemoryStream();
                                    newImage.Save(myResult, ImageFormat.Png);

                                    string StudentImagePath = Server.MapPath(string.Concat("~/Images/", Common.Constants.StudentImageFolder));
                                    var pathToSave = Path.Combine(StudentImagePath, user.Student.UserId + ".jpg");
                                    // viewModel.PhotoFilePath.SaveAs(pathToSave);
                                    System.IO.File.WriteAllBytes(pathToSave, myResult.ToArray());
                                }
                                else if (viewModel.ImageData != null)
                                {
                                    string StudentImagePath = Server.MapPath(string.Concat("~/Images/", Common.Constants.StudentImageFolder));
                                    var pathToSave = Path.Combine(StudentImagePath, user.Student.UserId + ".jpg");
                                    System.IO.File.WriteAllBytes(pathToSave, Convert.FromBase64String(viewModel.ImageData));
                                }
                                if (viewModel.StudentContact != null)
                                {
                                   // DynamicSendSMS(viewModel);
                                    //SendSMS(viewModel);
                                }
                                SendEmailDyanamic(viewModel, userPassword, filename);
                                // SendEmail(viewModel, userPassword, filename);
                            }
                            if (viewModel.PaidFee != 0 || result.Results.FirstOrDefault().Message == "Email already exists!")
                            {
                                if (viewModel.UserId == null)
                                {
                                    var userId = _studentService.GetStudentUserIdInstallment(viewModel.PunchId, viewModel.Email);
                                    viewModel.UserId = userId.UserId.ToString();
                                }
                                var remainingPayment = viewModel.FinalFees - viewModel.PaidFee;
                                var resultInstallment = _installmentService.Save(new Installment
                                {

                                    ClassId = viewModel.ClassId,
                                    UserId = viewModel.UserId,
                                    Payment = viewModel.PaidFee,
                                    RemainingFee = remainingPayment,
                                    ReceiptBookNumber = viewModel.ReceiptBookNumber,
                                    ReceiptNumber = viewModel.ReceiptNumber,
                                    ReceivedFee = viewModel.PaidFee,
                                });
                                if (resultInstallment.Success)
                                {
                                    Success(result.Results.FirstOrDefault().Message + " " + resultInstallment.Results.FirstOrDefault().Message);
                                    ModelState.Clear();
                                    viewModel = new StudentViewModel();
                                }
                                else
                                {
                                    _logger.Warn(result.Results.FirstOrDefault().Message + " " + resultInstallment.Results.FirstOrDefault().Message);
                                    Warning(result.Results.FirstOrDefault().Message + " " + resultInstallment.Results.FirstOrDefault().Message, true);

                                }
                            }
                            else
                            {
                                if (result.Success)
                                {
                                    Success(result.Results.FirstOrDefault().Message);
                                    ModelState.Clear();
                                    viewModel = new StudentViewModel();
                                }
                                else
                                {
                                    var messages = "";
                                    foreach (var message in result.Results)
                                    {
                                        messages += message.Message + "<br />";
                                    }
                                    _logger.Warn(messages);
                                    Warning(messages, true);
                                }
                            }

                        }
                        else
                        {
                            _logger.Warn("Please add Brochure.");
                            Warning("Please add Brochure.", true);
                        }
                    }
                }
                ViewBag.SelectedSubjects = viewModel.SelectedSubject;
                ViewBag.BatchId = viewModel.BatchId;
                ViewBag.BranchId = viewModel.BranchId;
                ViewBag.ClientId = viewModel.ClientId;
                ViewBag.CurrentUserRole = roles;

                ReturnViewModel(roles, viewModel, clientId, clientName, branchId, branchName, boardList, schoolList, classList);
                // viewModel.BatchId = ViewBag.BatchId;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + " student create");
                throw ex;
            }
        }

        public void ReturnViewModel(string roles, StudentViewModel viewModel, int ClientId, string ClientName, int branchId, string branchName, List<SelectListItem> boardList,
                List<SelectListItem> schoolList, List<SelectListItem> classList)
        {
            if (roles == "Admin")
            {
                var branchList = (from b in _branchService.GetAllBranchesByClientId(ClientId)
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                viewModel.Branches = branchList;
                ViewBag.BranchId = null;
            }
            else if (roles == "Client")
            {
                var branchList = (from b in _branchService.GetAllBranchesByClientId(ClientId)
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                viewModel.Branches = branchList;
                ViewBag.BranchId = null;
                ViewBag.ClientId = ClientId;
                //viewModel.BranchId = branchId;
                viewModel.BranchName = branchName;
            }
            else if (roles == "BranchAdmin")
            {
                viewModel.BranchId = branchId;
                viewModel.BranchName = branchName;
            }
            viewModel.Schools = schoolList;
            viewModel.Classes = classList;
            viewModel.Boards = boardList;
            viewModel.CurrentUserRole = roles;
        }

        public CMSResult SendSMS(StudentViewModel viewModel)
        {
            CMSResult cmsresult = new CMSResult();


            if (viewModel.ParentContact != "")
            {

                var smsModel = new SmsModel
                {
                    Message = string.Format("Your Student {0} {1} has been Joined " + viewModel.ClassName + " in {2} \n Click Here for Downloading Parent App \n {3} \n to Check Student Class Performance.",
                        viewModel.FirstName, viewModel.LastName, viewModel.BranchName, ConfigurationManager.AppSettings[Common.Constants.parentAppLink]),
                    SendTo = viewModel.ParentContact
                };


                var sendparentsms = _smsService.SendMessage(smsModel);
                cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });
            }

            if (viewModel.StudentContact != "")
            {
                var smsModel = new SmsModel
                {
                    Message = string.Format("Thanks for Joining " + viewModel.ClassName + " Family in  {0} {1} has been Joined" + viewModel.ClassName + " Family in {2} \n Click Here for Downloading Student App \n {3}.\n Grow Your Knowlege",
                        viewModel.FirstName, viewModel.LastName, viewModel.BranchName, ConfigurationManager.AppSettings[Common.Constants.studentAppLink]),
                    SendTo = viewModel.StudentContact
                };
                var sendparentsms = _smsService.SendMessage(smsModel);
                cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });

            }
            return cmsresult;

        }

        public bool SendEmail(StudentViewModel viewModel, string userPassword, string filename)
        {
            string userRole = "";
            bool isBranchAdmin = false;
            if (viewModel.CurrentUserRole == "BranchAdmin")
            {
                userRole = viewModel.BranchName + " ( " + User.Identity.GetUserName() + ")";
                isBranchAdmin = true;
            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            }

            var batchWithSubject = _batchService.GetBatcheById(viewModel.BatchId);
            var Subject = _subjectService.GetSubjectSubjectIds(viewModel.SelectedSubject).Select(x => x.Name).ToList();

            var paidFee = _installmentService.GetCountInstallment(viewModel.UserId);
            paidFee = viewModel.PaidFee == 0 ? 0 : viewModel.PaidFee;
            var remainFee = viewModel.FinalFees - paidFee;
            var className = _classService.GetClassById(viewModel.ClassId);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/StudentMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }

            //   body = body.Replace("{name}",name );
            body = body.Replace("{BranchName}", userRole);
            body = body.Replace("{UserName}", viewModel.FirstName + " " + viewModel.LastName);
            body = body.Replace("{Password}", userPassword + "<br/>is successfully register with us");
            body = body.Replace("{UserId}", viewModel.Email);
            body = body.Replace("{ClassName}", className.Name);
            body = body.Replace("{BatchWithSubjectName}", batchWithSubject.BatchName + "( " + string.Join(",", Subject) + " )");
            body = body.Replace("{TotalFees}", viewModel.TotalFees.ToString());
            body = body.Replace("{Discount}", viewModel.Discount.ToString());
            body = body.Replace("{FinalFees}", viewModel.FinalFees.ToString());
            body = body.Replace("{PaidFees}", paidFee.ToString());
            body = body.Replace("{RemainingFees}", remainFee.ToString());

            var emailMessage = new MailModel
            {
                Body = body,
                Subject = "Web portal - Student Create",
                To = viewModel.Email,

                IsBranchAdmin = isBranchAdmin
            };

            emailMessage.AttachmentPaths.Add(filename);
            return _emailService.Send(emailMessage);
        }

        public bool SendEmailDyanamic(StudentViewModel viewModel, string userPassword, string filename)
        {
            string userRole = "";
            bool isBranchAdmin = false;
            if (viewModel.CurrentUserRole == "BranchAdmin")
            {
                userRole = viewModel.BranchName + " ( " + User.Identity.GetUserName() + ")";
                isBranchAdmin = true;

            }
            else
            {
                userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";
            }

            var batchWithSubject = _batchService.GetBatcheById(viewModel.BatchId);
            var Subject = _subjectService.GetSubjectSubjectIds(viewModel.SelectedSubject).Select(x => x.Name).ToList();

            var paidFee = _installmentService.GetCountInstallment(viewModel.UserId);
            paidFee = viewModel.PaidFee == 0 ? 0 : viewModel.PaidFee;
            var remainFee = viewModel.FinalFees - paidFee;
            var className = _classService.GetClassById(viewModel.ClassId);


            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/StudentMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }


            body = body.Replace("{BranchName}", userRole);
            body = body.Replace("{UserName}", viewModel.FirstName + " " + viewModel.LastName);
            body = body.Replace("{Password}", userPassword + "<br/>is successfully register with us");
            body = body.Replace("{UserId}", viewModel.Email);
            body = body.Replace("{ClassName}", className.Name);
            body = body.Replace("{BatchWithSubjectName}", batchWithSubject.BatchName + "( " + string.Join(",", Subject) + " )");
            body = body.Replace("{TotalFees}", viewModel.TotalFees.ToString());
            body = body.Replace("{Discount}", viewModel.Discount.ToString());
            body = body.Replace("{FinalFees}", viewModel.FinalFees.ToString());
            body = body.Replace("{PaidFees}", paidFee.ToString());
            body = body.Replace("{RemainingFees}", remainFee.ToString());


            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var ClientId = projectionClient.ClientId;

            var projection = _configureServices.GetConfigureById(ClientId);
            var clientId = projection.ClientId;
            var emailid = projection.email_id;
            var password = projection.emailpassword;
            var name = projection.name;

            body = body.Replace("{name}", name);
            var emailMessage = new MailModel
            {
                Body = body,
                Subject = "Web portal - Student Create",
                To = viewModel.Email,
                Emailid = emailid,
                Password = password,

                IsBranchAdmin = isBranchAdmin
            };

            emailMessage.AttachmentPaths.Add(filename);
            return _emailService.MailSend(emailMessage);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(string id)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var ClientId = projectionClient.ClientId;
            ViewBag.boardList = from b in _boardService.GetBoardsByClientId(ClientId)
                                select new SelectListItem
                                {
                                    Value = b.BoardId.ToString(),
                                    Text = b.Name
                                };

            ViewBag.classList = from c in _classService.GetClassesByClientId(ClientId)
                                select new SelectListItem
                                {
                                    Value = c.ClassId.ToString(),
                                    Text = c.Name
                                };

            ViewBag.schoolList = (from s in _schoolService.GetAllSchoolsByClientId(ClientId)
                                  select new SelectListItem
                                  {
                                      Value = s.SchoolId.ToString(),
                                      Text = s.Name
                                  }).ToList();

            var projection = _studentService.GetStudentById(id);

            ViewBag.subjectList = from s in _subjectService.GetSubjectByClassId(projection.ClassId)
                                  select new SelectListItem
                                  {
                                      Value = s.SubjectId.ToString(),
                                      Text = s.Name
                                  };
            ViewBag.BatchList = from s in _batchService.GetAllBatchClsId(projection.ClassId)
                                select new SelectListItem
                                {
                                    Value = s.BatchId.ToString(),
                                    Text = s.BatchName
                                };

            ViewBag.SelectedSubjects = projection.SelectedSubjects;

            if (projection == null)
            {
                _logger.Warn(string.Format("Student does not Exists {0}.", id));
                Warning("Student does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = projection.BranchId;
            ViewBag.ClientId = projection.ClientId;
            ViewBag.ClassId = projection.ClassId;
            ViewBag.BoardId = projection.BoardId;
            ViewBag.SubjectId = projection.SubjectId;
            ViewBag.SchoolId = projection.SchoolId;
            ViewBag.BatchId = projection.BatchId;
            ViewBag.UrlPhoto = projection.PhotoPath;

            var viewModel = AutoMapper.Mapper.Map<StudentProjection, StudentEditViewModel>(projection);



            if (roles == "Admin")
            {
                viewModel.CurrentUserRole = roles;
                ViewBag.branchList = (from b in _branchService.GetAllBranches()
                                      select new SelectListItem
                                      {
                                          Value = b.BranchId.ToString(),
                                          Text = b.Name
                                      }).ToList();
                ViewBag.BranchId = projection.BranchId;
                return View(viewModel);
            }
            else if (roles == "Client")
            {

                viewModel.CurrentUserRole = roles;
                ViewBag.branchList = (from b in _branchService.GetAllBranchesByClientId(ClientId)
                                      select new SelectListItem
                                      {
                                          Value = b.BranchId.ToString(),
                                          Text = b.Name
                                      }).ToList();
                ViewBag.BranchId = projection.BranchId;
                return View(viewModel);

            }
            else if (roles == "BranchAdmin")
            {
                viewModel.CurrentUserRole = roles;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(StudentEditViewModel viewModel)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles1 = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var projectionClient = roles1 == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var ClientId = projectionClient.ClientId;

            string base64 = Request.Form["imgCropped"];
            var roles = viewModel.CurrentUserRole;
            ViewBag.boardList = from b in _boardService.GetBoardsByClientId(ClientId)
                                select new SelectListItem
                                {
                                    Value = b.BoardId.ToString(),
                                    Text = b.Name
                                };

            ViewBag.classList = from c in _classService.GetClassesByClientId(ClientId)
                                select new SelectListItem
                                {
                                    Value = c.ClassId.ToString(),
                                    Text = c.Name
                                };

            ViewBag.subjectList = from s in _subjectService.GetSubjectByClassId(viewModel.ClassId)
                                  select new SelectListItem
                                  {
                                      Value = s.SubjectId.ToString(),
                                      Text = s.Name
                                  };
            ViewBag.BatchList = from s in _batchService.GetAllBatchClsId(viewModel.ClassId)
                                select new SelectListItem
                                {
                                    Value = s.BatchId.ToString(),
                                    Text = s.BatchName
                                };

            ViewBag.schoolList = (from s in _schoolService.GetAllSchoolsByClientId(ClientId)
                                  select new SelectListItem
                                  {
                                      Value = s.SchoolId.ToString(),
                                      Text = s.Name
                                  }).ToList();

            ViewBag.BranchId = viewModel.BranchId;
            ViewBag.ClientId = viewModel.ClientId;
            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.BoardId = viewModel.BoardId;
            ViewBag.SubjectId = viewModel.SubjectId;
            ViewBag.SchoolId = viewModel.SchoolId;
            ViewBag.BatchId = viewModel.BatchId;

            if (ModelState.IsValid)
            {
                if (viewModel.PaymentErrorMessage != "" && viewModel.PaymentErrorMessage != null)
                {
                    _logger.Warn(viewModel.PaymentErrorMessage);
                    Warning(viewModel.PaymentErrorMessage, true);
                }
                else
                {
                    string filename = Server.MapPath(ConfigurationManager.AppSettings["brochureFile"].ToString());

                    if (System.IO.File.Exists(filename))
                    {
                        var user = new ApplicationUser();

                        string photoPath = "";
                        if (viewModel.PhotoFilePath != null)
                        {
                            photoPath = string.Format(@"~/Images/{0}/{1}.jpg", Common.Constants.StudentImageFolder, viewModel.UserId);
                            if (!Common.Constants.ImageTypes.Contains(viewModel.PhotoFilePath.ContentType))
                            {
                                _logger.Warn("Please choose either a JPEG, JPG or PNG image.");
                                Warning("Please choose either a JPEG, JPG or PNG image..", true);
                                return View(viewModel);
                            }
                        }
                        else if (viewModel.ImageData != null)
                        {
                            photoPath = string.Format(@"~/Images/{0}/{1}.jpg", Common.Constants.StudentImageFolder, viewModel.UserId);
                        }
                        else
                        {
                            photoPath = null;
                        }

                        var student = _repository.Project<ApplicationUser, bool>(users => (from u in users where u.Id == viewModel.UserId select u).Any());

                        if (!student)
                        {
                            _logger.Warn(string.Format("Student does not exists '{0} {1} {2}'.", viewModel.FirstName, viewModel.MiddleName, viewModel.LastName));
                            Danger(string.Format("Student does not exists '{0} {1} {2}'.", viewModel.FirstName, viewModel.MiddleName, viewModel.LastName));
                            return RedirectToAction("Index");
                        }

                        var result = _studentService.Update(new Student
                        {
                            BoardId = viewModel.BoardId,
                            ClassId = viewModel.ClassId,
                            FirstName = viewModel.FirstName.Trim(),
                            MiddleName = viewModel.MiddleName == null ? "" : viewModel.MiddleName.Trim(),
                            LastName = viewModel.LastName.Trim(),
                            Gender = viewModel.Gender,
                            Address = viewModel.Address.Trim(),
                            Pin = viewModel.Pin,
                            DOB = viewModel.DOB,
                            BloodGroup = viewModel.BloodGroup,
                            StudentContact = viewModel.StudentContact == null ? "" : viewModel.StudentContact.Trim(),
                            ParentContact = viewModel.ParentContact,
                          //  PickAndDrop = viewModel.PickAndDrop,
                            DOJ = viewModel.DOJ,
                            SchoolId = viewModel.SchoolId,
                            TotalFees = viewModel.TotalFees,
                            Discount = viewModel.Discount,
                            FinalFees = viewModel.FinalFees,
                            UserId = viewModel.UserId,
                            SelectedSubject = viewModel.SelectedSubject,
                            BatchId = viewModel.BatchId,
                            IsWhatsApp = viewModel.IsWhatsApp,
                            IsActive = viewModel.IsActive,
                            PunchId = viewModel.PunchId,
                            MotherName = viewModel.MotherName,
                          //  VANArea = viewModel.VANArea,
                            SeatNumber = viewModel.SeatNumber,
                         //   VANFee = viewModel.VANFee,
                            BranchId = viewModel.BranchId,
                            ClientId = viewModel.ClientId,
                            RollNo=viewModel.RollNo,
                         //   PhotoPath = photoPath == null ? viewModel.PhotoPath : photoPath,
                         //   EmergencyContact = viewModel.EmergencyContact == null ? "" : viewModel.EmergencyContact.Trim(),
                            ParentEmailId = viewModel.ParentEmailId == null ? "" : viewModel.ParentEmailId.Trim(),
                            PaymentLists = viewModel.PaymentLists
                        });


                        if (result.Success)
                        {
                            if (viewModel.PhotoFilePath != null)
                            {
                                byte[] bytes = Convert.FromBase64String(base64.Split(',')[1]);
                                MemoryStream myMemStream = new MemoryStream(bytes);
                                Image fullsizeImage = Image.FromStream(myMemStream);
                                Image newImage = fullsizeImage.GetThumbnailImage(240, 240, null, IntPtr.Zero);
                                MemoryStream myResult = new MemoryStream();
                                newImage.Save(myResult, ImageFormat.Png);

                                string StudentImagePath = Server.MapPath(string.Concat("~/Images/", Common.Constants.StudentImageFolder));
                                var pathToSave = Path.Combine(StudentImagePath, viewModel.UserId + ".jpg");
                                System.IO.File.WriteAllBytes(pathToSave, myResult.ToArray());
                                //  viewModel.PhotoFilePath.SaveAs(pathToSave);
                            }
                            else if (viewModel.ImageData != null)
                            {
                                string StudentImagePath = Server.MapPath(string.Concat("~/Images/", Common.Constants.StudentImageFolder));
                                var pathToSave = Path.Combine(StudentImagePath, viewModel.UserId + ".jpg");
                                System.IO.File.WriteAllBytes(pathToSave, Convert.FromBase64String(viewModel.ImageData));
                            }
                            if (viewModel.CurrentUserRole == "BranchAdmin")
                            {
                                string createdBranchName = viewModel.BranchName + " ( " + User.Identity.GetUserName() + " )";
                                var batchWithSubject = _batchService.GetBatcheById(viewModel.BatchId);
                                var Subject = _subjectService.GetSubjectSubjectIds(viewModel.SelectedSubject).Select(x => x.Name).ToList();
                                var paidFee = _installmentService.GetCountInstallment(viewModel.UserId);
                                var remainFee = viewModel.FinalFees - paidFee;
                                var className = _classService.GetClassById(viewModel.ClassId);
                                string body = string.Empty;
                                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/StudentMailDesign.html")))
                                {
                                    body = reader.ReadToEnd();
                                }
                                body = body.Replace("{BranchName}", createdBranchName);
                                body = body.Replace("{UserName}", viewModel.FirstName + " " + viewModel.MiddleName + " " + viewModel.LastName);
                                body = body.Replace("{ClassName}", className.Name);
                                body = body.Replace("{BatchWithSubjectName}", batchWithSubject.BatchName + "( " + string.Join(",", Subject) + " )");
                                body = body.Replace("{TotalFees}", viewModel.TotalFees.ToString() + "</br>Discount : " + viewModel.Discount
                                    + "<br/>Fee after Discount : " + viewModel.FinalFees);
                                body = body.Replace("{PaidFees}", paidFee.ToString());
                                body = body.Replace("{RemainingFees}", remainFee.ToString());

                                var emailMessage = new MailModel
                                {
                                    Body = body,
                                    Subject = "Web portal changes student",
                                    IsBranchAdmin = true,
                                };
                                emailMessage.AttachmentPaths.Add(filename);
                                _emailService.Send(emailMessage);
                            }
                            Success(result.Results.FirstOrDefault().Message);
                            ModelState.Clear();
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            var messages = "";
                            foreach (var message in result.Results)
                            {
                                messages += message.Message + "<br />";
                            }
                            _logger.Warn(messages);
                            Warning(messages, true);
                        }
                    }
                }
            }
            // var roleUserId = User.Identity.GetUserId();
            //var roles1 = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles1 == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            if (viewModel.CurrentUserRole == "Admin")
            {
                ViewBag.branchList = (from b in _branchService.GetAllBranches()
                                      select new SelectListItem
                                      {
                                          Value = b.BranchId.ToString(),
                                          Text = b.Name
                                      }).ToList();
            }
            else if (viewModel.CurrentUserRole == "Client")
            {
                ViewBag.branchList = (from b in _branchService.GetAllBranchesByClientId(projection.ClientId)
                                      select new SelectListItem
                                      {
                                          Value = b.BranchId.ToString(),
                                          Text = b.Name
                                      }).ToList();
            }
            else if (viewModel.CurrentUserRole == "BranchAdmin")
            {
            }

            return View(viewModel);

        }

        [Roles(Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Details(string id)
        {
            var students = _studentService.GetStudentById(id);
            var viewModel = AutoMapper.Mapper.Map<StudentProjection, StudentEditViewModel>(students);

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

        [Roles(Common.Constants.StudentRole)]
        public ActionResult Dashboard(string userId)
        {
            var students = _studentService.GetStudentById(userId);
            var viewModel = AutoMapper.Mapper.Map<StudentProjection, StudentViewModel>(students);
            return View(viewModel);
        }

        [Roles(Common.Constants.StudentRole)]
        public ActionResult FeesDetails(string userId)
        {
            var installment = _installmentService.GetStudInstallments(userId).ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<InstallmentProjection>, InstallmentViewModel[]>(installment);
            return View(viewModelList);
        }

        public ActionResult GetBatch(int classId)
        {
            var batch = _batchService.GetAllBatchesByClassId(classId).ToList();
            return Json(batch, JsonRequestBehavior.AllowGet);
        }

       /* public ActionResult GetNotify()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var notify = _studentService.GetStudentsPaymentByClientId(projection.ClientId);
            return Json(notify, JsonRequestBehavior.AllowGet);
        }*/

        public ActionResult GetSubjectFees(string selectedSubject, string selectedYear)
        {
            var totalFees = _studentService.GetTotalFees(selectedSubject, selectedYear);
            return Json(totalFees, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassesByBranchId(int branchId)
        {
            var subjects = _studentService.GetStudentsByBranchId(branchId).Select(x => new { x.ClassId, x.ClassName }).Distinct();
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassesByClientId(int clientId)
        {
            var subjects = _studentService.GetStudentsByClientId(clientId).Select(x => new { x.ClassId, x.ClassName }).Distinct();
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetClassesFromStudent(int? branchId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "BranchAdmin")
            {
                var classes = _studentService.GetClassesByBranchId((int)branchId).Select(x => new { x.ClassId, x.ClassName });
                int studentCount = classes.Count();
                classes = classes.Distinct();
                int teacherCount = _teacherService.GetTeacherContactListBrbranchId((int)branchId).Count();
                int branchAdminCount = _branchAdminService.GetBranchAdminContactListBrbranchId((int)branchId).Count();

                var result = new
                {
                    classes = classes,
                    studentParentCount = studentCount,
                    teacherCount = teacherCount,
                    branchAdminCount = branchAdminCount
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            if (roles == "Client")
            {
                var classes = _studentService.GetClassesByClientId((int)branchId).Select(x => new { x.ClassId, x.ClassName });
                int studentCount = classes.Count();
                classes = classes.Distinct();
                int teacherCount = _teacherService.GetTeacherContactListBrclientId((int)branchId).Count();
                int clientAdminCount = _branchAdminService.GetBranchAdminContactListBrbranchId((int)branchId).Count();

                var result = new
                {
                    classes = classes,
                    studentParentCount = studentCount,
                    teacherCount = teacherCount,
                    clientAdminCount = clientAdminCount
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var classes = _studentService.GetClasses().Select(x => new { x.ClassId, x.ClassName });
                int studentCount = classes.Count();
                classes = classes.Distinct();
                int teacherCount = _teacherService.GetTeacherContactList().Count();
                int branchAdminCount = _branchAdminService.GetBranchAdminContactList().Count();
                var result = new
                {
                    classes = classes,
                    studentParentCount = studentCount,
                    teacherCount = teacherCount,
                    branchAdminCount = branchAdminCount
                };

                return Json(result, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult GetClassesByMultipleBranches(string selectedBranch)
        {
            var classes = _studentService.GetClassesByMultipleBranchId(selectedBranch).Select(x => new { x.ClassId, x.ClassName });
            var studentCount = classes.Count();
            classes = classes.Distinct();
            var branchIds = selectedBranch.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var teacherCount = _teacherService.GetTeacherContactList().Where(x => branchIds.Contains(x.BranchId)).Count();
            var branchAdminCount = _branchAdminService.GetBranchAdminContactList().Where(x => branchIds.Contains(x.BranchId)).Count();
            var result = new
            {
                classes = classes,
                studentParentCount = studentCount,
                teacherCount = teacherCount,
                branchAdminCount = branchAdminCount
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult GetSubjects(int classId)
        {
            var subjects = _subjectService.GetSubjectByClassId(classId).ToList();
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public CMSResult DynamicSendSMS(StudentViewModel viewModel)
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
            var institute = projection.name;
            var className = _classService.GetClassById(viewModel.ClassId);
            var clsname = className.Name;

            if (roles == "BranchAdmin" || roles == "Client")
            {
                //userRole = branchName + " ( " + User.Identity.GetUserName() + " ) ";
                // isBranchAdmin = true;
                if (viewModel.ParentContact != "")
                {

                    var smsModel = new SmsModel
                    {
                        SenderId = senderid,
                        UserName = username,
                        Password = password,

                        Message = string.Format("Your Student {0} {1} has been Joined " + institute + " in {2}" + clsname + " \n Click Here for Downloading Parent App \n {3} \n to Check Student Class Performance.",
                            viewModel.FirstName, viewModel.LastName, viewModel.BranchName, "www.google.com"),
                        SendTo = viewModel.ParentContact
                    };


                    var sendparentsms = _smsService.DynamicsSendMessage(smsModel);
                    cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });
                }

                if (viewModel.StudentContact != "")
                {
                    var smsModel = new SmsModel
                    {

                        SenderId = senderid,
                        UserName = username,
                        Password = password,
                        Message = string.Format("Thanks for Joining " + institute + " Family in  {0} {1} has been Joined" + clsname + " Family in {2} \n Click Here for Downloading Student App \n {3}.\n Grow Your Knowlege",
                            viewModel.FirstName, viewModel.LastName, viewModel.BranchName, "www.google.com"),
                        SendTo = viewModel.StudentContact
                    };
                    var sendparentsms = _smsService.DynamicsSendMessage(smsModel);
                    cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });

                }

            }
            else
            {
                // userRole = User.Identity.GetUserName() + "(" + "Master Admin" + ")";

                if (viewModel.ParentContact != "")
                {

                    var smsModel = new SmsModel
                    {

                        Message = string.Format("Your Student {0} {1} has been Joined " + viewModel.ClassName + " in {2} \n Click Here for Downloading Parent App \n {3} \n to Check Student Class Performance.",
                            viewModel.FirstName, viewModel.LastName, viewModel.BranchName, "www.google.com"),
                        SendTo = viewModel.ParentContact
                    };


                    var sendparentsms = _smsService.SendMessage(smsModel);
                    cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });
                }

                if (viewModel.StudentContact != "")
                {
                    var smsModel = new SmsModel
                    {
                        Message = string.Format("Thanks for Joining " + viewModel.ClassName + " Family in  {0} {1} has been Joined" + viewModel.ClassName + " Family in {2} \n Click Here for Downloading Student App \n {3}.\n Grow Your Knowlege",
                            viewModel.FirstName, viewModel.LastName, viewModel.BranchName, "www.google.com"),
                        SendTo = viewModel.StudentContact
                    };
                    var sendparentsms = _smsService.SendMessage(smsModel);
                    cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });

                }
            }
            return cmsresult;

        }



        [System.Web.Services.WebMethod]
        public static string loadFields(string fields, string table)
        {
            string cons= ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString;


            string msg = "";        // A MESSAGE TO BE RETURNED TO THE AJAX CALL.

            try
            {
                // EXTRACT VALUES FROM THE "fields" STRING FOR THE COLUMNS.

                int iCnt = 0;
                string sColumns = "";
                for (iCnt = 0; iCnt <= fields.Split(',').Length - 1; iCnt++)
                {
                    if (string.IsNullOrEmpty(sColumns))
                    {
                        sColumns = "[" + fields.Split(',')[iCnt].Replace(" ", "") + "] VARCHAR (100)";
                    }
                    else
                    {
                        sColumns = sColumns + ", [" + fields.Split(',')[iCnt].Replace(" ", "") + "] VARCHAR (100)";
                    }
                }

                using (SqlConnection con = new SqlConnection(cons))
                {
                    // CREATE TABLE STRUCTURE USING THE COLUMNS AND TABLE NAME.

                    string sQuery = null;
                    sQuery = "IF OBJECT_ID('dbo." + table.Replace(" ", "_") + "', 'U') IS NULL " +
                        "BEGIN " +
                        "CREATE TABLE [dbo].[" + table.Replace(" ", "_") + "](" +
                        "[" + table.Replace(" ", "_") + "_ID" + "] INT IDENTITY(1,1) NOT NULL CONSTRAINT pk" +
                            table.Replace(" ", "_") + "_ID" + " PRIMARY KEY, " +
                        "[CreateDate] DATETIME, " + sColumns + ")" +
                        " END";

                    using (SqlCommand cmd = new SqlCommand(sQuery))
                    {
                        cmd.Connection = con;
                        con.Open();

                        cmd.ExecuteNonQuery();
                        con.Close();

                        msg = "Table created successfuly.";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "There was an error.";
            }
            finally
            { }

            return msg;
        }

           [HttpPost]
            public JsonResult GetStudents(string Prefix)
            {
                var student = (from s in _studentService.GetAllStudents()
                               where s.FirstName.StartsWith(Prefix)
                               select new StudentProjection
                               {
                                   FirstName = s.FirstName
                               }).Distinct();

                return Json(student, JsonRequestBehavior.AllowGet);

            }
      
        [HttpPost]
        public JsonResult GetPinsno(string Prefix)
        {
            var Student = _studentService.GetAllPinByPrefix(Prefix);
           // Student = Student.Distinct();
            return Json(Student, JsonRequestBehavior.AllowGet);
        }

        /*    /// <summary>
            /// Post method for importing users 
            /// </summary>
            /// <param name="postedFile"></param>
            /// <returns></returns>
            [HttpPost]
            public ActionResult Create(HttpPostedFileBase postedFile)
            {

                if (postedFile != null)
                {
                    try
                    {
                        string fileExtension = Path.GetExtension(postedFile.FileName);

                        //Validate uploaded file and return error.
                        if (fileExtension != ".xls" && fileExtension != ".xlsx")
                        {
                            ViewBag.Message = "Please select the excel file with .xls or .xlsx extension";
                            return View();
                        }

                        string folderPath = Server.MapPath("~/UploadedFiles/");
                        //Check Directory exists else create one
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        //Save file to folder
                        var filePath = folderPath + Path.GetFileName(postedFile.FileName);
                        postedFile.SaveAs(filePath);

                        //Get file extension

                        string excelConString = "";

                        //Get connection string using extension 
                        switch (fileExtension)
                        {
                            //If uploaded file is Excel 1997-2007.
                            case ".xls":
                                excelConString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                                break;
                            //If uploaded file is Excel 2007 and above
                            case ".xlsx":
                                excelConString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                                break;
                        }

                        //Read data from first sheet of excel into datatable
                        DataTable dt = new DataTable();
                        excelConString = string.Format(excelConString, filePath);

                        using (OleDbConnection excelOledbConnection = new OleDbConnection(excelConString))
                        {
                            using (OleDbCommand excelDbCommand = new OleDbCommand())
                            {
                                using (OleDbDataAdapter excelDataAdapter = new OleDbDataAdapter())
                                {
                                    excelDbCommand.Connection = excelOledbConnection;

                                    excelOledbConnection.Open();
                                    //Get schema from excel sheet
                                    DataTable excelSchema = GetSchemaFromExcel(excelOledbConnection);
                                    //Get sheet name
                                    string sheetName = excelSchema.Rows[0]["TABLE_NAME"].ToString();
                                    excelOledbConnection.Close();

                                    //Read Data from First Sheet.
                                    excelOledbConnection.Open();
                                    excelDbCommand.CommandText = "SELECT * From [" + sheetName + "]";
                                    excelDataAdapter.SelectCommand = excelDbCommand;
                                    //Fill datatable from adapter
                                    excelDataAdapter.Fill(dt);
                                    excelOledbConnection.Close();
                                }
                            }
                        }

                        //Insert records to Student table.
                        using (var context = new CMSDbContext())
                        {
                            //Loop through datatable and add Student data to Student table. 
                            foreach (DataRow row in dt.Rows)
                            {
                                context.Students.Add(GetStudentFromExcelRow(row));
                            }
                            context.SaveChanges();
                        }
                        ViewBag.Message = "Data Imported Successfully.";
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = ex.Message;
                    }
                }
                else
                {
                    ViewBag.Message = "Please select the file first to upload.";
                }
                return View();
            }

            private static DataTable GetSchemaFromExcel(OleDbConnection excelOledbConnection)
            {
                return excelOledbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            }

            //Convert each datarow into Student object
            private Student GetStudentFromExcelRow(DataRow row)
            {
                return new Student
                {
                    UserId = row[0].ToString(),
                    FirstName = row[1].ToString(),
                    DOJ =DateTime.Parse(row[2].ToString()),
                      MiddleName = row[1].ToString(),
                        LastName = row[2].ToString(),
                        MotherName = row[3].ToString(),
                        //  Gender = int.Parse(row[4].ToString()),
                        Address = row[4].ToString(),
                        Pin = row[5].ToString(),
                        //  DOB= DateTime.Parse(row[6].ToString()),
                        //  BloodGroup = int.Parse(row[7].ToString()),
                        StudentContact = row[6].ToString(),
                        ParentContact = row[7].ToString(),
                          DOJ= DateTime.Parse(row[9].ToString()),
                        EmergencyContact = row[8].ToString(),
                        ParentEmailId = row[9].ToString(),
                        // School= row[8].ToString(),
                        SeatNumber = row[10].ToString(),
                        TotalFees = decimal.Parse(row[11].ToString()),
                        Discount = decimal.Parse(row[12].ToString()),
                        PunchId = int.Parse(row[13].ToString()),


                };
            }   
            */

    }
}