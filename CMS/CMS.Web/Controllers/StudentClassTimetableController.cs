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
    public class StudentClassTimetableController : BaseController
    {
        readonly IApiService _apiService;
        readonly IStudentTimetableService _studentTimetableService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IBranchService _branchService;
        readonly IStudentService _studentService;
        readonly IBatchService _batchService;
        readonly IClassService _classService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IEmailService _emailService;
        readonly ISendNotificationService _sendNotificationService;
        readonly IClientAdminService _clientAdminService;

        public StudentClassTimetableController(IClientAdminService clientAdminService, IApiService apiService, IStudentTimetableService studentTimetableService, ILogger logger,
            IRepository repository, IBranchService branchService,
            IStudentService studentService, IBatchService batchService,
            IClassService classService, IAspNetRoles aspNetRolesService,
            
            IBranchAdminService branchAdminService, IEmailService emailService, ISendNotificationService sendNotificationService)
        {
            _apiService = apiService;
            _studentTimetableService = studentTimetableService;
            _logger = logger;
            _repository = repository;
            _branchService = branchService;
            _studentService = studentService;
            _batchService = batchService;
            _classService = classService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _emailService = emailService;
            _sendNotificationService = sendNotificationService;
            _clientAdminService = clientAdminService;
        }
        // GET: StudentTimetable
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
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

                return View(new StudentTimetableViewModel
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

                var classList = (from c in _classService.GetClassesByClientId(projection.ClientId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();

                ViewBag.BranchId = 0;
                ViewBag.CurrentUserRole = roles;

                return View(new StudentTimetableViewModel
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
                                 }).Distinct().ToList();
                ViewBag.CurrentUserRole = roles;
                return View(new StudentTimetableViewModel
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StudentTimetableViewModel viewModel)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            int ClientId = projection.ClientId;
            var cmsResult = new CMSResult();

            if (ModelState.IsValid)
            {
                string fileName = "";
                Guid guid = Guid.NewGuid();
                if (viewModel.FilePath != null)
                {
                    if (viewModel.AttachmentDescription == null)
                    {
                        _logger.Warn("Please enter attachment description.");
                        Warning("Please enter attachment description.", true);
                        ReturnViewModel(viewModel, roles, roleUserId);
                        return View(viewModel);
                    }
                    else if (Common.Constants.ImageTypes.Contains(viewModel.FilePath.ContentType) ||
                        Common.Constants.PdfType.Contains(viewModel.FilePath.ContentType))
                    {
                        if (viewModel.FilePath != null)
                            if (Common.Constants.ImageTypes.Contains(viewModel.FilePath.ContentType))
                                fileName = string.Format("{0}.jpg", guid);
                            else
                                fileName = string.Format("{0}.pdf", guid);
                    }
                    else
                    {
                        _logger.Warn("Please choose either a JPEG, JPG, PNG image or pdf file.");
                        Warning("Please choose either a JPEG, JPG, PNG image or pdf file", true);
                        ReturnViewModel(viewModel, roles, roleUserId);
                        return View(viewModel);
                    }
                }
                var description = viewModel.Description.Replace("\r\n", "<br />");
                viewModel.Description = description;
                var studentClassTimetable = new StudentTimetable
                {
                    Description = viewModel.Description,
                    SelectedBranches = viewModel.SelectedBranches != null ? viewModel.SelectedBranches : "",
                    SelectedClasses = viewModel.SelectedClasses != null ? viewModel.SelectedClasses : "",
                    SelectedBatches = viewModel.SelectedBatches != null ? viewModel.SelectedBatches : "",
                    FileName = fileName,
                    Category = Common.Enums.TimetableCategory.ClassTimetable,
                    AttachmentDescription = viewModel.AttachmentDescription != null ? viewModel.AttachmentDescription : "",
                    StudentTimetableDate = viewModel.StudentTimetableDate
                };
                var result = _studentTimetableService.Save(ClientId,studentClassTimetable);
                
                if (result.Success)
                {
                    var studentClassTimetableId = studentClassTimetable.StudentTimetableId;

                    if (viewModel.FilePath != null)
                    {
                        var pathToSaveQI = "";
                        string folderPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.StudentTimeTableFile));
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        if (Common.Constants.ImageTypes.Contains(viewModel.FilePath.ContentType))
                            pathToSaveQI = Path.Combine(folderPath, string.Format("{0}.jpg", guid));
                        else
                            pathToSaveQI = Path.Combine(folderPath, string.Format("{0}.pdf", guid));

                        if (viewModel.FilePath != null)
                            viewModel.FilePath.SaveAs(pathToSaveQI);
                    }

                    var sendAppNotification = SendAppNotification(viewModel, studentClassTimetableId);

                    if (roles == "BranchAdmin")
                        SendTimetable(viewModel);

                    Success(result.Results.FirstOrDefault().Message + "<br />" + sendAppNotification.Results[0].Message);
                    ModelState.Clear();
                    viewModel = new StudentTimetableViewModel();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            ReturnViewModel(viewModel, roles, roleUserId);
            return View(viewModel);
        }

        public void ReturnViewModel(StudentTimetableViewModel viewModel, string roles, string roleUserId)
        {
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            if (roles == "Admin" )
            {
                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();
                viewModel.Branches = branchList;
                ViewBag.BranchId = 0;
            }
            else if (roles == "Client")
            {

                var branchList = (from b in _branchService.GetAllBranchesByClientId(projection.ClientId)
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                var classList = (from c in _studentService.GetStudentsByBranchId(projection.ClientId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();
                viewModel.Branches = branchList;
                viewModel.Classes = classList;

                viewModel.ClientId = projection.ClientId;
                viewModel.ClientName = projection.ClientName;
                ViewBag.BranchId = 0;
            }
            else if (roles == "BranchAdmin")
            {
                var projection1 = _branchAdminService.GetBranchAdminById(roleUserId);
                ViewBag.BranchId = projection1.BranchId;
                var classList = (from c in _studentService.GetStudentsByBranchId(projection1.BranchId)
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();

                viewModel.BranchId = projection1.BranchId;
                viewModel.BranchName = projection1.BranchName;
                viewModel.Classes = classList;
            }

            viewModel.CurrentUserRole = roles;
        }

        public CMSResult SendTimetable(StudentTimetableViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            try
            {
                var projection = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                var bodySubject = "Student Time Table Created";
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", projection.BranchName);
                body = body.Replace("{ModuleName}", User.Identity.GetUserName() + "<br/>" + "Student Time Table:" + viewModel.Description + "<br/>");
                body = body.Replace("{BranchAdminEmail}", User.Identity.GetUserName());

                var emailMessage = new MailModel
                {
                    IsBranchAdmin = true,
                    Body = body,
                    Subject = bodySubject,
                    To = ConfigurationManager.AppSettings[Common.Constants.AdminEmail]
                };
                var result = _emailService.Send(emailMessage);
                if (result)
                    cmsResult.Results.Add(new Result { Message = "Sent Successfully.", IsSuccessful = true });
                else
                    cmsResult.Results.Add(new Result { Message = "Something went wrong.", IsSuccessful = false });

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message + "catch SendTimetable");
                throw;
            }

            return cmsResult;
        }

        public ActionResult Delete(int id)
        {
            BranchClassBatchShow(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(StudentTimetableEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _studentTimetableService.Delete(viewModel.StudentTimetableId);
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

        public ActionResult Details(int id)
        {
            BranchClassBatchShow(id);
            return View();
        }

        public ActionResult BranchClassBatchShow(int id)
        {
            var studentTimetable = _studentTimetableService.GetStudentTimetableById(id);
            ViewBag.fileName = studentTimetable.FileName;
            if (studentTimetable == null)
            {
                _logger.Warn(string.Format("Student Time Table does not Exists {0}.", id));
                Warning("Student Time Table does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<StudentTimetableProjection, StudentTimetableEditViewModel>(studentTimetable);

            var commaseperatedBranchList = studentTimetable.SelectedBranches ?? string.Empty;
            var branchIds = commaseperatedBranchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var branchResult = _branchService.GetAllBranches();
            var branches = (branchResult).Where(x => branchIds.Contains(x.BranchId)).ToList();
            List<string> BranchList = new List<string>();
            BranchList = branches.Select(x => x.Name).ToList();
            var Branchlist = string.Join(",", BranchList);
            viewModel.SelectedBranches = Branchlist;

            var commaseperatedClassList = studentTimetable.SelectedClasses ?? string.Empty;
            var ClassIds = commaseperatedClassList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var classResult = _classService.GetClasses();
            var Classees = (classResult).Where(x => ClassIds.Contains(x.ClassId)).ToList();
            List<string> ClassList = new List<string>();
            ClassList = Classees.Select(x => x.Name).ToList();
            var Classlist = string.Join(",", ClassList);
            viewModel.SelectedClasses = Classlist;

            var commaseperatedBatchList = studentTimetable.SelectedBatches ?? string.Empty;
            var BatchIds = commaseperatedBatchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var batchResult = _batchService.GetAllBatches();
            var Batch = (batchResult).Where(x => BatchIds.Contains(x.BatchId)).ToList();
            List<string> BatchList = new List<string>();
            BatchList = Batch.Select(x => x.BatchName).ToList();
            var BatchesList = string.Join(",", BatchList);
            viewModel.SelectedBatches = BatchesList.TrimEnd(',');
            var description = viewModel.Description.Replace("<br />", "\r\n");
            viewModel.Description = description;
            return View(viewModel);
        }

        public FileResult DownloadPDF(int id)
        {
            var projection = _studentTimetableService.GetStudentTimetableById(id);
            string path = Path.Combine(Server.MapPath("~/PDF/StudentTimetableFile"), projection.FileName);
            var fileExtension = Path.GetExtension(projection.FileName);
            if (fileExtension == ".jpg")
            {
                return File(path, "image/jpg", projection.FileName);
            }
            return File(path, "application/pdf");
        }

        public ActionResult Edit(int id)
        {
            BranchClassBatchShow(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StudentTimetableEditViewModel viewModel)
        {
            ViewBag.fileName = viewModel.FileName;
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                var examTimetable = _repository.Project<StudentTimetable, bool>(timetables => (from timetable in timetables where timetable.StudentTimetableId == viewModel.StudentTimetableId select timetable).Any());
                if (!examTimetable)
                {
                    _logger.Warn(string.Format("Exam timetable not exists '{0}'.", ""));
                    Danger(string.Format("Exam timetable not exists '{0}'.", ""));
                }
                string filePath = "";
                if (viewModel.FilePath != null)
                {
                    Guid guid = Guid.NewGuid();
                    if (viewModel.AttachmentDescription == null)
                    {
                        _logger.Warn("Please enter attachment description.");
                        Warning("Please enter attachment description.", true);
                        return View(viewModel);
                    }
                    else if (viewModel.FileName == null || viewModel.FileName == "")
                    {
                        if (Common.Constants.ImageTypes.Contains(viewModel.FilePath.ContentType))
                            viewModel.FileName = string.Format("{0}.jpg", guid);
                        else
                            viewModel.FileName = string.Format("{0}.pdf", guid);
                    }
                    else
                    {
                        if (Common.Constants.ImageTypes.Contains(viewModel.FilePath.ContentType))
                            viewModel.FileName = string.Format("{0}.jpg", viewModel.FileName.Split('.')[0]);
                        else
                            viewModel.FileName = string.Format("{0}.pdf", viewModel.FileName.Split('.')[0]);
                    }
                    filePath = viewModel.FileName;
                    if (viewModel.FilePath.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Please choose file", IsSuccessful = false });
                    }
                }
                else
                {
                    filePath = null;
                }
                var fileNameSelect = (viewModel.FilePath != null) ? filePath : viewModel.FileName;
                var description = viewModel.Description.Replace("\r\n", "<br />");
                var result = _studentTimetableService.Update(new StudentTimetable
                {
                    StudentTimetableId = viewModel.StudentTimetableId,
                    Description = description,
                    AttachmentDescription = viewModel.AttachmentDescription != null ? viewModel.AttachmentDescription : "",
                    StudentTimetableDate = viewModel.StudentTimetableDate,
                    FileName = fileNameSelect
                });
                if (result.Success)
                {
                    if (viewModel.FilePath != null)
                    {
                        string TestLogoPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.StudentTimeTableFile));
                        var pathToSave = Path.Combine(TestLogoPath, viewModel.FileName);
                        viewModel.FilePath.SaveAs(pathToSave);
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

        public CMSResult SendAppNotification(StudentTimetableViewModel viewModel, int studentClassTimetableId)
        {
            CMSResult cmsResult = new CMSResult();
            var notificationList = new List<SendNotificationByPlayerId>();
            try
            {
                var studentsList = _studentService.GetAllStudentParentList();
                if (viewModel.SelectedBranches != null)
                {
                    var branches = viewModel.SelectedBranches.Split(',').Select(x => int.Parse(x)).ToList();
                    studentsList = studentsList.Where(x => branches.Contains(x.BranchId));
                }
                if (viewModel.SelectedClasses != null)
                {
                    var classes = viewModel.SelectedClasses.Split(',').Select(x => int.Parse(x)).ToList();
                    studentsList = studentsList.Where(x => classes.Contains(x.ClassId));
                }
                if (viewModel.SelectedBatches != null)
                {
                    var batches = viewModel.SelectedBatches.Split(',').Select(x => int.Parse(x)).ToList();
                    studentsList = studentsList.Where(x => batches.Contains(x.BatchId));
                }

                var parentPlayerIds = studentsList.Where(x => !string.IsNullOrEmpty(x.parentAppPlayerId)).ToList();
                foreach (var playerId in parentPlayerIds)
                {
                    var studentSId = playerId.SId;
                    var sendAppNotification = new SendNotificationByPlayerId
                    {
                        Message = "CTT-" + viewModel.Description + "$^$" + playerId.SId + "@" + studentClassTimetableId,
                        PlayerIds = playerId.parentAppPlayerId,
                        AppIds = ConfigurationManager.AppSettings[Common.Constants.ParentAppId],
                        RestApiKey = ConfigurationManager.AppSettings[Common.Constants.ParentRestAppId]
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
            }
            catch (Exception ex)
            {
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = true });
            }
            return cmsResult;
        }
    }
}