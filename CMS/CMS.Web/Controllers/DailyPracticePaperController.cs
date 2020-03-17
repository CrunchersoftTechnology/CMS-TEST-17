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
    public class DailyPracticePaperController : BaseController
    {
        readonly IApiService _apiService;
        readonly IDailyPracticePaperService _dailyPracticePaperService;
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

        public DailyPracticePaperController(IApiService apiService, IDailyPracticePaperService dailyPracticePaperService, ILogger logger,
            IRepository repository, IBranchService branchService,
            IStudentService studentService, IBatchService batchService, IClassService classService,
            IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService, IEmailService emailService, ISendNotificationService sendNotificationService)
        {
            _apiService = apiService;
            _dailyPracticePaperService = dailyPracticePaperService;
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
        }

        // GET: DailyPracticePactice
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            if (roles == "Admin" || roles=="Client")
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
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "Admin" || roles=="Client")
            {
                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                ViewBag.BranchId = 0;
                ViewBag.CurrentUserRole = roles;

                return View(new DailyPracticePaperViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles
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
                return View(new DailyPracticePaperViewModel
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
        public ActionResult Create(DailyPracticePaperViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

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
                var dailyPracticePaper = new DailyPracticePaper
                {
                    Description = viewModel.Description,
                    SelectedBranches = viewModel.SelectedBranches != null ? viewModel.SelectedBranches : "",
                    SelectedClasses = viewModel.SelectedClasses != null ? viewModel.SelectedClasses : "",
                    SelectedBatches = viewModel.SelectedBatches != null ? viewModel.SelectedBatches : "",
                    FileName = fileName,
                    AttachmentDescription = viewModel.AttachmentDescription != null ? viewModel.AttachmentDescription : "",
                    DailyPracticePaperDate = viewModel.DailyPracticePaperDate
                };
                var result = _dailyPracticePaperService.Save(dailyPracticePaper);
                
                if (result.Success)
                {
                    var dailyPracticePaperId = dailyPracticePaper.DailyPracticePaperId;
                    if (viewModel.FilePath != null)
                    {
                        var pathToSaveQI = "";
                        string folderPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.DailyPracticePaperFile));
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

                    var sendAppNotification = SendAppNotification(viewModel, dailyPracticePaperId);

                    if (roles == "BranchAdmin")
                        SendDailyPracticePaper(viewModel);

                    Success(result.Results.FirstOrDefault().Message + "<br />" + sendAppNotification.Results[0].Message);
                    ModelState.Clear();
                    viewModel = new DailyPracticePaperViewModel();
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

        public void ReturnViewModel(DailyPracticePaperViewModel viewModel, string roles, string roleUserId)
        {
            if (roles == "Admin" || roles=="Client")
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
                viewModel.BranchId = projection.BranchId;
                viewModel.BranchName = projection.BranchName;
                viewModel.Classes = classList;
            }
            viewModel.CurrentUserRole = roles;
        }

        public CMSResult SendDailyPracticePaper(DailyPracticePaperViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            try
            {
                var projection = _branchAdminService.GetBranchAdminById(User.Identity.GetUserId());
                var bodySubject = "Daily Practice Paper Created";
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", projection.BranchName);
                body = body.Replace("{ModuleName}", User.Identity.GetUserName() + "<br/>" + "Dily Practice Paper:" + viewModel.Description + "<br/>");
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
                _logger.Error(ex.Message + "catch SendDailyPracticePaper");
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
        public ActionResult Delete(DailyPracticePaperEditViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _dailyPracticePaperService.Delete(viewModel.DailyPracticePaperId);
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

            var dailyPracticePaper = _dailyPracticePaperService.GetDailyPracticePaperById(id);
            ViewBag.fileName = dailyPracticePaper.FileName;
            if (dailyPracticePaper == null)
            {
                _logger.Warn(string.Format("Daily Practice Paper does not Exists {0}.", id));
                Warning("Daily Practice Paper does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<DailyPracticePaperProjection, DailyPracticePaperEditViewModel>(dailyPracticePaper);

            var commaseperatedBranchList = dailyPracticePaper.SelectedBranches ?? string.Empty;
            var branchIds = commaseperatedBranchList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var branchResult = _branchService.GetAllBranches();
            var branches = (branchResult).Where(x => branchIds.Contains(x.BranchId)).ToList();
            List<string> BranchList = new List<string>();
            BranchList = branches.Select(x => x.Name).ToList();
            var Branchlist = string.Join(",", BranchList);
            viewModel.SelectedBranches = Branchlist;

            var commaseperatedClassList = dailyPracticePaper.SelectedClasses ?? string.Empty;
            var ClassIds = commaseperatedClassList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            var classResult = _classService.GetClasses();
            var Classees = (classResult).Where(x => ClassIds.Contains(x.ClassId)).ToList();
            List<string> ClassList = new List<string>();
            ClassList = Classees.Select(x => x.Name).ToList();
            var Classlist = string.Join(",", ClassList);
            viewModel.SelectedClasses = Classlist;

            var commaseperatedBatchList = dailyPracticePaper.SelectedBatches ?? string.Empty;
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

        public ActionResult Edit(int id)
        {
            BranchClassBatchShow(id);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DailyPracticePaperEditViewModel viewModel)
        {
            ViewBag.fileName = viewModel.FileName;
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                var practicePaper = _repository.Project<DailyPracticePaper, bool>(papers => (from paper in papers where paper.DailyPracticePaperId == viewModel.DailyPracticePaperId select paper).Any());
                if (!practicePaper)
                {
                    _logger.Warn(string.Format("Daily practice paper not exists '{0}'.", ""));
                    Danger(string.Format("Daily practice paper not exists '{0}'.", ""));
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
                var result = _dailyPracticePaperService.Update(new DailyPracticePaper
                {
                    DailyPracticePaperId = viewModel.DailyPracticePaperId,
                    Description = description,
                    AttachmentDescription = viewModel.AttachmentDescription != null ? viewModel.AttachmentDescription : "",
                    DailyPracticePaperDate = viewModel.DailyPracticePaperDate,
                    FileName = fileNameSelect
                });
                if (result.Success)
                {
                    if (viewModel.FilePath != null)
                    {
                        string PaperPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.DailyPracticePaperFile));
                        var pathToSave = Path.Combine(PaperPath, viewModel.FileName);
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

        public FileResult DownloadPDF(int id)
        {
            var projection = _dailyPracticePaperService.GetDailyPracticePaperById(id);
            string path = Path.Combine(Server.MapPath("~/PDF/DailyPracticePaperFile"), projection.FileName);
            var fileExtension = Path.GetExtension(projection.FileName);
            if (fileExtension == ".jpg")
            {
                return File(path, "image/jpg", projection.FileName);
            }
            return File(path, "application/pdf");
        }

        public CMSResult SendAppNotification(DailyPracticePaperViewModel viewModel, int dailyPracticePaperId)
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
                        Message = "DPP-" + viewModel.Description + "$^$" + playerId.SId + "@" + dailyPracticePaperId,
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
                cmsResult.Results.Add(new Result { Message = ex.Message, IsSuccessful = false });
            }
            return cmsResult;
        }
    }
}