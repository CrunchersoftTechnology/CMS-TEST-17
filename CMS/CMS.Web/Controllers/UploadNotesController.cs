using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
    public class UploadNotesController : BaseController
    {
        readonly IApiService _apiService;
        readonly ILogger _logger;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IUploadNotesService _uploadNotesService;
        readonly IRepository _repository;
        readonly IClientAdminService _clientAdminService;
        readonly IUploadNotesService _uploadNoteService;

        public UploadNotesController(IUploadNotesService uploadNoteService, IEmailService emailService,IAspNetRoles aspNetRolesService,IClientAdminService clientAdminService,IApiService apiService, ILogger logger, IUploadNotesService uploadNotesService,
            IRepository repository)
        {
            _apiService = apiService;
            _logger = logger;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _uploadNotesService = uploadNotesService;
            _repository = repository;
            _clientAdminService = clientAdminService;
            _uploadNoteService = uploadNoteService;
        }
        // GET: UploadNotes
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            if (roles == "Admin")
            {
                ViewBag.userId = 0;
                var pdfcotegories = _uploadNoteService.GetUploadNotesList().ToList();
                var viewModelList = AutoMapper.Mapper.Map<List<UploadNotesProjection>, UploadNotesViewModel[]>(pdfcotegories);
                return View(viewModelList);
            }
            else if (roles == "Client")
            {
                ViewBag.userId = projection.ClientId;
                var pdfcotegories = _uploadNoteService.GetUploadNotesListByClientId(projection.ClientId).ToList();
                var viewModelList = AutoMapper.Mapper.Map<List<UploadNotesProjection>, UploadNotesViewModel[]>(pdfcotegories);
                return View(viewModelList);
            }
            return View();
        }

        public ActionResult Create()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

            var classResult = _apiService.GetClasses();
            var classes = JsonConvert.DeserializeObject<List<ClassProjection>>(classResult);
            var boardResult = _apiService.GetBoards();
            var boards = JsonConvert.DeserializeObject<List<BoardProjection>>(boardResult);
           

            var classList = (from b in classes
                             select new SelectListItem
                             {
                                 Value = b.ClassId.ToString(),
                                 Text = b.Name
                             }).ToList();

            var boardList = (from b in boards
                             select new SelectListItem
                             {
                                 Value = b.BoardId.ToString(),
                                 Text = b.Name
                             }).ToList();



            return View(new UploadNotesViewModel
            {
                Classes = classList,
                Boards = boardList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UploadNotesViewModel viewModel)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            int ClientId = projection.ClientId;
            var cmsResult = new CMSResult();
            var classResult = _apiService.GetClasses();
            var classes = JsonConvert.DeserializeObject<List<ClassProjection>>(classResult);
            var boardResult = _apiService.GetBoards();
            var boards = JsonConvert.DeserializeObject<List<BoardProjection>>(boardResult);

            var classList = (from b in classes
                             select new SelectListItem
                             {
                                 Value = b.ClassId.ToString(),
                                 Text = b.Name
                             }).ToList();

            var boardList = (from b in boards
                             select new SelectListItem
                             {
                                 Value = b.BoardId.ToString(),
                                 Text = b.Name
                             }).ToList();

            ViewBag.SubjectId = viewModel.SubjectId;
            if (ModelState.IsValid)
            {
                if (!Common.Constants.PdfType.Contains(viewModel.FilePath.ContentType))
                {
                    cmsResult.Results.Add(new Result { Message = "Please choose pdf file.", IsSuccessful = false });
                    _logger.Warn(cmsResult.Results.FirstOrDefault().Message);
                    Warning(cmsResult.Results.FirstOrDefault().Message, true);
                }
                if (!Common.Constants.ImageTypes.Contains(viewModel.LogoPath.ContentType))
                {
                    cmsResult.Results.Add(new Result { Message = "Please choose Image file.", IsSuccessful = false });
                    _logger.Warn(cmsResult.Results.FirstOrDefault().Message);
                    Warning(cmsResult.Results.FirstOrDefault().Message, true);
                }
                string photoPath = "";
                Guid guid = Guid.NewGuid();
                if (viewModel.LogoPath != null)
                    photoPath = string.Format(@"~/Images/{0}/{1}.jpg", Common.Constants.UploadNotesLogo, guid);
                ViewBag.SubjectId = viewModel.SubjectId;
                if (cmsResult.Success)
                {
                    var result = _uploadNotesService.Save(ClientId,new UploadNotes
                    {
                        BoardName = viewModel.BoardName,
                        ClassName = viewModel.ClassName,
                        SubjectName = viewModel.SubjectName,
                        Title = viewModel.Title,
                        UploadDate = viewModel.UploadDate,
                        FileName = viewModel.FilePath.FileName,
                        LogoName = photoPath,
                        IsVisible = viewModel.IsVisible,
                        ClassId = viewModel.ClassId,
                        BoardId = viewModel.BoardId,
                        SubjectId = viewModel.SubjectId,
                        ClientId= projection.ClientId,
                    });

                    if (result.Success)
                    {
                        //string folderPath = Server.MapPath(string.Concat("~/PDF/", Constants.UploadNotesPDF));
                        string folderPath = Server.MapPath(string.Concat("~/StudentAppPDF/", Common.Constants.UploadNotesPDF));
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var pathToSaveQI = Path.Combine(folderPath, string.Format("{0}", viewModel.FilePath.FileName));
                        if (viewModel.FilePath != null)
                            viewModel.FilePath.SaveAs(pathToSaveQI);

                        string LogoPath = Server.MapPath(string.Concat("~/Images/", Common.Constants.UploadNotesLogo));
                        if (!Directory.Exists(LogoPath))
                        {
                            Directory.CreateDirectory(LogoPath);
                        }
                        var pathToSaveLogo = Path.Combine(LogoPath, string.Format("{0}.jpg", guid));
                        if (viewModel.LogoPath != null)
                            viewModel.LogoPath.SaveAs(pathToSaveLogo);

                        Success(result.Results.FirstOrDefault().Message);
                        ModelState.Clear();
                        viewModel = new UploadNotesViewModel();
                    }
                    else
                    {
                        _logger.Warn(result.Results.FirstOrDefault().Message);
                        Warning(result.Results.FirstOrDefault().Message, true);
                    }
                }
            }

            viewModel.Classes = classList;
            viewModel.Boards = boardList;
            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            var projection = _uploadNotesService.GetNotesById(id);

            var classResult = _apiService.GetClasses();
            var classes = JsonConvert.DeserializeObject<List<ClassProjection>>(classResult);
            var boardResult = _apiService.GetBoards();
            var boards = JsonConvert.DeserializeObject<List<BoardProjection>>(boardResult);
            var classId = classes.Where(x => x.Name == projection.ClassName).Select(y => y.ClassId).FirstOrDefault();
            var subjectResult = _apiService.GetSubjects(classId);
            var subjects = JsonConvert.DeserializeObject<List<SubjectProjection>>(subjectResult);
            var boardId = boards.Where(x => x.Name == projection.BoardName).Select(y => y.BoardId).FirstOrDefault();
            var subjectId = subjects.Where(x => x.Name == projection.SubjectName).Select(y => y.SubjectId).FirstOrDefault();

            ViewBag.classList = (from b in classes
                                 select new SelectListItem
                                 {
                                     Value = b.ClassId.ToString(),
                                     Text = b.Name
                                 }).ToList();

            ViewBag.boardList = (from b in boards
                                 select new SelectListItem
                                 {
                                     Value = b.BoardId.ToString(),
                                     Text = b.Name
                                 }).ToList();

            ViewBag.subjectList = (from b in subjects
                                   select new SelectListItem
                                   {
                                       Value = b.SubjectId.ToString(),
                                       Text = b.Name
                                   }).ToList();

            if (projection == null)
            {
                _logger.Warn(string.Format("Notes does not Exists {0}."));
                Warning("Notes does not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<UploadNotesProjection, UploadNotesEditViewModel>(projection);
            ViewBag.ClassId = classId;
            ViewBag.BoardId = boardId;
            ViewBag.SubjectId = subjectId;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UploadNotesEditViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            var classResult = _apiService.GetClasses();
            var classes = JsonConvert.DeserializeObject<List<ClassProjection>>(classResult);
            var boardResult = _apiService.GetBoards();
            var boards = JsonConvert.DeserializeObject<List<BoardProjection>>(boardResult);
            var classId = classes.Where(x => x.ClassId == viewModel.ClassId).Select(y => y.ClassId).FirstOrDefault();
            var subjectResult = _apiService.GetSubjects(classId);
            var subjects = JsonConvert.DeserializeObject<List<SubjectProjection>>(subjectResult);
            var boardId = boards.Where(x => x.BoardId == viewModel.BoardId).Select(y => y.BoardId).FirstOrDefault();
            var subjectId = subjects.Where(x => x.SubjectId == viewModel.SubjectId).Select(y => y.SubjectId).FirstOrDefault();

            ViewBag.classList = (from b in classes
                                 select new SelectListItem
                                 {
                                     Value = b.ClassId.ToString(),
                                     Text = b.Name
                                 }).ToList();

            ViewBag.boardList = (from b in boards
                                 select new SelectListItem
                                 {
                                     Value = b.BoardId.ToString(),
                                     Text = b.Name
                                 }).ToList();

            ViewBag.subjectList = (from b in subjects
                                   select new SelectListItem
                                   {
                                       Value = b.SubjectId.ToString(),
                                       Text = b.Name
                                   }).ToList();

            ViewBag.ClassId = classId;
            ViewBag.BoardId = boardId;
            ViewBag.SubjectId = subjectId;
            if (ModelState.IsValid)
            {
                var uploadNotes = _repository.Project<UploadNotes, bool>(notes => (from p in notes where p.UploadNotesId == viewModel.UploadNotesId select p).Any());
                if (!uploadNotes)
                {
                    _logger.Warn(string.Format("Notes not exists."));
                    Danger(string.Format("Notes not exists."));
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

                string logoPath = "";
                if (viewModel.LogoPath != null)
                {
                    logoPath = string.Format(@"~/Images/{0}/{1}", Common.Constants.UploadNotesLogo, viewModel.LogoName.Split('/')[3]);
                    if (viewModel.LogoPath.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Logo is required", IsSuccessful = false });
                    }
                    if (!Common.Constants.ImageTypes.Contains(viewModel.LogoPath.ContentType))
                    {
                        _logger.Warn("Please choose either a JPEG, JPG or PNG image.");
                        Warning("Please choose either a JPEG, JPG or PNG image..", true);
                        return View(viewModel);
                    }
                }
                else
                {
                    logoPath = null;
                }

                var fileNameSelect = (viewModel.FilePath != null) ? viewModel.FilePath.FileName : viewModel.FileName;
                var result = _uploadNotesService.Update(new UploadNotes
                {
                    UploadNotesId = viewModel.UploadNotesId,
                    Title = viewModel.Title,
                    ClassName = viewModel.ClassName,
                    BoardName = viewModel.BoardName,
                    SubjectName = viewModel.SubjectName,
                    FileName = fileNameSelect,
                    UploadDate = viewModel.UploadDate,
                    LogoName = (viewModel.LogoPath != null) ? logoPath : viewModel.LogoName,
                    IsVisible = viewModel.IsVisible,
                    ClassId = viewModel.ClassId,
                    BoardId = viewModel.BoardId,
                    SubjectId = viewModel.SubjectId
                });

                if (result.Success)
                {
                    if (viewModel.LogoPath != null)
                    {
                        string NotesLogoPath = Server.MapPath(string.Concat("~/Images/", Common.Constants.UploadNotesLogo));
                        var pathToSave = Path.Combine(NotesLogoPath, viewModel.LogoName.Split('/')[3]);
                        viewModel.LogoPath.SaveAs(pathToSave);
                    }
                    if (viewModel.FilePath != null)
                    {
                        string pdfUploadPath = Server.MapPath(string.Concat("~/StudentAppPDF/", Common.Constants.UploadResultPDF));
                        var pathToDeletepdf = Path.Combine(pdfUploadPath, string.Format("{0}", viewModel.FileName));
                        if ((System.IO.File.Exists(pathToDeletepdf)))
                        {
                            System.IO.File.Delete(pathToDeletepdf);
                        }

                        if (!Directory.Exists(pdfUploadPath))
                        {
                            Directory.CreateDirectory(pdfUploadPath);
                        }
                        var pathToSaveQI = Path.Combine(pdfUploadPath, string.Format("{0}", viewModel.FilePath.FileName));

                        viewModel.FilePath.SaveAs(pathToSaveQI);
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
            var projection = _uploadNotesService.GetNotesById(id);
            string path = "";
            path = Path.Combine(Server.MapPath("~/StudentAppPDF/Notes"), projection.FileName);
            return File(path, "application/pdf");
        }

        public ActionResult Delete(int id)
        {
            var projection = _uploadNotesService.GetNotesById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Notes does not Exists."));
                Warning("Notes does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<UploadNotesProjection, UploadNotesEditViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UploadNotesEditViewModel viewModel)
        {
            string pdfUploadPath = Server.MapPath(string.Concat("~/StudentAppPDF/", Common.Constants.UploadNewsPDF));
            if (ModelState.IsValid)
            {
                var result = _uploadNotesService.Delete(viewModel.UploadNotesId);
                if (result.Success)
                {
                    var pathToDeletepdf = Path.Combine(pdfUploadPath, string.Format("{0}", viewModel.FileName));
                    if ((System.IO.File.Exists(pathToDeletepdf)))
                    {
                        System.IO.File.Delete(pathToDeletepdf);
                    }
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
    }
}