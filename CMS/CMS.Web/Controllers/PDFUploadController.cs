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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class PDFUploadController : BaseController
    {
        readonly IPDFUploadService _pdfUploadService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IClassService _classService;
        readonly IPDFCategoryService _pdfCategoryService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IStudentService _studentService;
        readonly ISendNotificationService _sendNotificationService;

        public PDFUploadController(IClassService classService, IPDFUploadService pdfUploadService, ILogger logger,
            IRepository repository, IPDFCategoryService pdfCategoryService, IEmailService emailService,
            IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService, IStudentService studentService,
            ISendNotificationService sendNotificationService)
        {
            _classService = classService;
            _pdfUploadService = pdfUploadService;
            _logger = logger;
            _repository = repository;
            _pdfCategoryService = pdfCategoryService;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _studentService = studentService;
            _sendNotificationService = sendNotificationService;
        }
        // GET: PDF
        public ActionResult Index()
        {
            var pdfUploadsFiles = _pdfUploadService.GetPDFUploadFiles().ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<PDFUploadProjection>, PDFUploadViewModel[]>(pdfUploadsFiles);
            return View(viewModelList);
        }

        public ActionResult Create()
        {
            var classes = _classService.GetClasses().ToList();
            var pdfcategories = _pdfCategoryService.GetPDFCategories().ToList();
            var viewModel = new PDFUploadViewModel();
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");
            viewModel.PDFCategories = new SelectList(pdfcategories, "PDFCategoryId", "Name");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PDFUploadViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                if (viewModel.FilePath.ContentLength == 0)
                {
                    cmsResult.Results.Add(new Result { Message = "Pdf is required", IsSuccessful = false });
                }
                if (!Common.Constants.PdfType.Contains(viewModel.FilePath.ContentType))
                {
                    cmsResult.Results.Add(new Result { Message = "Please choose pdf file.", IsSuccessful = false });
                    _logger.Warn(cmsResult.Results.FirstOrDefault().Message);
                    Warning(cmsResult.Results.FirstOrDefault().Message, true);
                }
                if (cmsResult.Success)
                {
                    var pdfupload = new PDFUpload
                    {
                        ClassId = viewModel.ClassId,
                        Title = viewModel.Title,
                        FileName = viewModel.FilePath.FileName,
                        IsVisible = viewModel.IsVisible,
                        PDFCategoryId = viewModel.PDFCategoryId,
                        IsSend = viewModel.IsSend
                    };

                    var result = _pdfUploadService.Save(pdfupload);
                    var pdfuploadId = pdfupload.PDFUploadId;
                    if (result.Success)
                    {
                        string folderPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.PdfFileFolder));
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var pathToSaveQI = Path.Combine(folderPath, string.Format("{0}", viewModel.FilePath.FileName));
                        if (viewModel.FilePath != null)
                            viewModel.FilePath.SaveAs(pathToSaveQI);

                        var bodySubject = "Web portal - PDF Upload Created";
                        var message = "PDFUpload Created Successfully";
                        SendMailToAdmin(message, viewModel.Title, viewModel.PDFCategoryName, viewModel.ClassName, bodySubject);
                        var successMessage = result.Results.FirstOrDefault().Message;
                        CMSResult sendNotificationResult = new CMSResult();
                        if (viewModel.IsSend)
                        {
                            sendNotificationResult = SendNotification(viewModel.ClassId, viewModel.Title, viewModel.FilePath.FileName, viewModel.PDFCategoryName, pdfuploadId);
                            successMessage += " <br/>" + sendNotificationResult.Results.FirstOrDefault().Message;
                        }
                        Success(successMessage);
                        ModelState.Clear();
                        viewModel = new PDFUploadViewModel();
                    }
                    else
                    {
                        _logger.Warn(result.Results.FirstOrDefault().Message);
                        Warning(result.Results.FirstOrDefault().Message, true);
                    }
                }
            }
            else
            {
                cmsResult.Results.Add(new Result { Message = "Please select PDF!", IsSuccessful = true });
            }
            var classes = _classService.GetClasses().ToList();
            var pdfcategories = _pdfCategoryService.GetPDFCategories().ToList();
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");
            viewModel.PDFCategories = new SelectList(pdfcategories, "PDFCategoryId", "Name");
            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.SelectedClass = from mt in _classService.GetClasses()
                                    select new SelectListItem
                                    {
                                        Value = mt.ClassId.ToString(),
                                        Text = mt.Name
                                    };
            ViewBag.SelectedPDFCategories = from mt in _pdfCategoryService.GetPDFCategories()
                                            select new SelectListItem
                                            {
                                                Value = mt.PDFCategoryId.ToString(),
                                                Text = mt.Name
                                            };

            var projection = _pdfUploadService.GetPdfFileById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("PDF does not Exists {0}.", id));
                Warning("PDF does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.ClassId = projection.ClassId;
            ViewBag.PDFCategoryId = projection.PDFCategoryId;
            var viewModel = AutoMapper.Mapper.Map<PDFUploadProjection, PDFUploadEditViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PDFUploadEditViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            ViewBag.SelectedClass = from mt in _classService.GetClasses()
                                    select new SelectListItem
                                    {
                                        Value = mt.ClassId.ToString(),
                                        Text = mt.Name
                                    };
            ViewBag.SelectedPDFCategories = from mt in _pdfCategoryService.GetPDFCategories()
                                            select new SelectListItem
                                            {
                                                Value = mt.PDFCategoryId.ToString(),
                                                Text = mt.Name
                                            };

            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.PDFCategoryId = viewModel.PDFCategoryId;
            string pdfUploadPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.PdfFileFolder));
            if (ModelState.IsValid)
            {
                var pdfFiles = _repository.Project<PDFUpload, bool>(pdfU => (from p in pdfU where p.PDFUploadId == viewModel.PDFUploadId select p).Any());
                if (!pdfFiles)
                {
                    _logger.Warn(string.Format("PDF not exists '{0}'.", viewModel.FileName));
                    Danger(string.Format("PDF not exists '{0}'.", viewModel.FileName));
                }
                if (viewModel.FilePath != null)
                {
                    if (viewModel.FilePath.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Pdf is required", IsSuccessful = false });
                    }
                    if (!Common.Constants.PdfType.Contains(viewModel.FilePath.ContentType))
                    {
                        cmsResult.Results.Add(new Result { Message = "Please choose pdf file.", IsSuccessful = false });
                    }
                }

                var fileNameSelect = (viewModel.FilePath != null) ? viewModel.FilePath.FileName : viewModel.FileName;

                var result = _pdfUploadService.Update(
                     new PDFUpload
                     {
                         PDFUploadId = viewModel.PDFUploadId,
                         Title = viewModel.Title,
                         ClassId = viewModel.ClassId,
                         FileName = fileNameSelect,
                         IsVisible = viewModel.IsVisible,
                         PDFCategoryId = viewModel.PDFCategoryId,
                         IsSend = viewModel.IsSend
                     });

                if (result.Success)
                {
                    if (viewModel.FilePath != null)
                    {
                        var pathToDeletepdf = Path.Combine(pdfUploadPath, string.Format("{0}", viewModel.FileName));
                        if ((System.IO.File.Exists(pathToDeletepdf)))
                        {
                            System.IO.File.Delete(pathToDeletepdf);
                        }

                        string folderPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.PdfFileFolder));
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var pathToSaveQI = Path.Combine(folderPath, string.Format("{0}", viewModel.FilePath.FileName));

                        viewModel.FilePath.SaveAs(pathToSaveQI);
                    }
                    var bodySubject = "Web portal changes - PDF Upload update";
                    var message = "PDFupload updated  Successfully";
                    SendMailToAdmin(message, viewModel.Title, viewModel.PDFCategoryName, viewModel.ClassName, bodySubject);
                    var successMessage = result.Results.FirstOrDefault().Message;
                    CMSResult sendNotificationResult = new CMSResult();
                    if (viewModel.IsSend)
                    {
                        sendNotificationResult = SendNotification(viewModel.ClassId, viewModel.Title, fileNameSelect, viewModel.PDFCategoryName, viewModel.PDFUploadId);
                        successMessage += " <br/>" + sendNotificationResult.Results.FirstOrDefault().Message;
                    }
                    Success(successMessage);
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

        public ActionResult Delete(int id)
        {
            var projection = _pdfUploadService.GetPdfFileById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("pdf does not Exists {0}.", id));
                Warning("pdf does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<PDFUploadProjection, PDFUploadDeleteViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PDFUploadDeleteViewModel viewModel)
        {
            string pdfUploadPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.PdfFileFolder));
            if (ModelState.IsValid)
            {
                var result = _pdfUploadService.Delete(viewModel.PDFUploadId);
                if (result.Success)
                {
                    var pathToDeletepdf = Path.Combine(pdfUploadPath, string.Format("{0}", viewModel.FileName));
                    if ((System.IO.File.Exists(pathToDeletepdf)))
                    {
                        System.IO.File.Delete(pathToDeletepdf);
                    }
                    var bodySubject = "Web portal changes - PDF Upload delete";
                    var message = "PDFUpload deleted Successfully";
                    SendMailToAdmin(message, viewModel.Title, viewModel.PDFCategoryName, viewModel.ClassName, bodySubject);
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

        public FileResult DownloadPDF(int id)
        {
            var filename = _pdfUploadService.GetPDFFileName(id);
            string path = "";
            var content_type = "";
            path = Path.Combine(Server.MapPath("~/PDF/PdfFile"), filename);

            if (filename.Contains(".pdf"))
            {
                content_type = "application/pdf";
            }
            return File(path, content_type, filename);
        }

        public void SendMailToAdmin(string message, string Name, string PDFCategoryName, string ClassName, string bodySubject)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                var branchName = branchAdmin.BranchName;
                var branchAdminEmail = branchAdmin.Email;
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", branchName);
                body = body.Replace("{ModuleName}", "Title: " + Name + "<br/>PDF Category: " + PDFCategoryName + "<br/>Class Name :" + ClassName + "<br/>" + message);
                body = body.Replace("{BranchAdminEmail}", "( " + branchAdminEmail + " )");
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsBranchAdmin = true
                };
                _emailService.Send(emailMessage);
            }
        }

        public CMSResult SendNotification(int classId, string title, string filename, string pdfCategory, int pdfuploadId)
        {
            var getStudentAppPlayerList = _studentService.GetStudentsAppPlayerIdByClass(classId);
            var sortlist = getStudentAppPlayerList.Where(x => !string.IsNullOrEmpty(x.studentAppPlayerId))
            .Select(y => y.studentAppPlayerId).ToList();
            if (sortlist.Count > 0)
            {
                var result = _sendNotificationService.SendNotificationByPlayersId(new SendNotification
                {
                    Message = "PDF-" + title + "$^$ File - " + filename + "$^$ Category - " + pdfCategory + "$^$" + pdfuploadId,
                    PlayerIds = sortlist,
                    AppIds = ConfigurationManager.AppSettings[Common.Constants.StudentAppId],
                    RestApiKey = ConfigurationManager.AppSettings[Common.Constants.StudentRestAppId]
                });

                return result;
            }
            else
            {
                var result = new CMSResult();
                result.Results.Add(new Result { IsSuccessful = false, Message = "There is no student register." });
                return result;
            }

        }
    }
}
