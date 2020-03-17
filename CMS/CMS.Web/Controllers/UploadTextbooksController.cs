using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
    public class UploadTextbooksController : BaseController
    {
        readonly IApiService _apiService;
        readonly ILogger _logger;
        readonly IUploadTextbooksService _uploadTextbooksService;
        readonly IRepository _repository;

        public UploadTextbooksController(IApiService apiService, ILogger logger, IUploadTextbooksService uploadTextbooksService,
            IRepository repository)
        {
            _apiService = apiService;
            _logger = logger;
            _uploadTextbooksService = uploadTextbooksService;
            _repository = repository;
        }
        // GET: UploadNotes
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
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



            return View(new UploadTextbooksViewModel
            {
                Classes = classList,
                Boards = boardList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UploadTextbooksViewModel viewModel)
        {
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
                if (!Constants.PdfType.Contains(viewModel.FilePath.ContentType))
                {
                    cmsResult.Results.Add(new Result { Message = "Please choose pdf file.", IsSuccessful = false });
                    _logger.Warn(cmsResult.Results.FirstOrDefault().Message);
                    Warning(cmsResult.Results.FirstOrDefault().Message, true);
                }
                if (!Constants.ImageTypes.Contains(viewModel.LogoPath.ContentType))
                {
                    cmsResult.Results.Add(new Result { Message = "Please choose Image file.", IsSuccessful = false });
                    _logger.Warn(cmsResult.Results.FirstOrDefault().Message);
                    Warning(cmsResult.Results.FirstOrDefault().Message, true);
                }
                string photoPath = "";
                Guid guid = Guid.NewGuid();
                if (viewModel.LogoPath != null)
                    photoPath = string.Format(@"~/Images/{0}/{1}.jpg", Constants.UploadTextbooksLogo, guid);
                ViewBag.SubjectId = viewModel.SubjectId;
                if (cmsResult.Success)
                {
                    var result = _uploadTextbooksService.Save(new UploadTextbooks
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
                        SubjectId = viewModel.SubjectId
                    });

                    if (result.Success)
                    {
                        //string folderPath = Server.MapPath(string.Concat("~/PDF/", Constants.UploadNotesPDF));
                        string folderPath = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadTextbooksPDF));
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        var pathToSaveQI = Path.Combine(folderPath, string.Format("{0}", viewModel.FilePath.FileName));
                        if (viewModel.FilePath != null)
                            viewModel.FilePath.SaveAs(pathToSaveQI);

                        string LogoPath = Server.MapPath(string.Concat("~/Images/", Constants.UploadTextbooksLogo));
                        if (!Directory.Exists(LogoPath))
                        {
                            Directory.CreateDirectory(LogoPath);
                        }
                        var pathToSaveLogo = Path.Combine(LogoPath, string.Format("{0}.jpg", guid));
                        if (viewModel.LogoPath != null)
                            viewModel.LogoPath.SaveAs(pathToSaveLogo);

                        Success(result.Results.FirstOrDefault().Message);
                        ModelState.Clear();
                        viewModel = new UploadTextbooksViewModel();
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
            var projection = _uploadTextbooksService.GetTextbooksById(id);

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
                _logger.Warn(string.Format("Textbooks does not Exists {0}."));
                Warning("Textbooks does not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<UploadTextbooksProjection, UploadTextbooksEditViewModel>(projection);
            ViewBag.ClassId = classId;
            ViewBag.BoardId = boardId;
            ViewBag.SubjectId = subjectId;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UploadTextbooksEditViewModel viewModel)
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
                var uploadTextbooks = _repository.Project<UploadTextbooks, bool>(Textbooks => (from p in Textbooks where p.UploadTextbooksId == viewModel.UploadTextbooksId select p).Any());
                if (!uploadTextbooks)
                {
                    _logger.Warn(string.Format("Textbooks not exists."));
                    Danger(string.Format("Textbooks not exists."));
                }
                if (viewModel.FilePath != null)
                {
                    if (viewModel.FilePath.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Pdf is required", IsSuccessful = false });
                    }
                    if (!Constants.PdfType.Contains(viewModel.FilePath.ContentType))
                    {
                        cmsResult.Results.Add(new Result { Message = "Please choose pdf file.", IsSuccessful = false });
                    }
                }

                string logoPath = "";
                if (viewModel.LogoPath != null)
                {
                    logoPath = string.Format(@"~/Images/{0}/{1}", Common.Constants.UploadTextbooksLogo, viewModel.LogoName.Split('/')[3]);
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
                var result = _uploadTextbooksService.Update(new UploadTextbooks
                {
                    UploadTextbooksId = viewModel.UploadTextbooksId,
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
                        string TextbooksLogoPath = Server.MapPath(string.Concat("~/Images/", Common.Constants.UploadTextbooksLogo));
                        var pathToSave = Path.Combine(TextbooksLogoPath, viewModel.LogoName.Split('/')[3]);
                        viewModel.LogoPath.SaveAs(pathToSave);
                    }
                    if (viewModel.FilePath != null)
                    {
                        string pdfUploadPath = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadResultPDF));
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
            var projection = _uploadTextbooksService.GetTextbooksById(id);
            string path = "";
            path = Path.Combine(Server.MapPath("~/StudentAppPDF/UploadTextbooksPDF"), projection.FileName);
            return File(path, "application/pdf");
        }

        public ActionResult Delete(int id)
        {
            var projection = _uploadTextbooksService.GetTextbooksById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Textbooks does not Exists."));
                Warning("Textbooks does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<UploadTextbooksProjection, UploadTextbooksEditViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(UploadTextbooksEditViewModel viewModel)
        {
            string pdfUploadPath = Server.MapPath(string.Concat("~/StudentAppPDF/", Constants.UploadTextbooksPDF));
            if (ModelState.IsValid)
            {
                var result = _uploadTextbooksService.Delete(viewModel.UploadTextbooksId);
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