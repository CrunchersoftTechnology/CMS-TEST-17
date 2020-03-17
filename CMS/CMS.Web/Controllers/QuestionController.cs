using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Constants.AdminRole + "," + Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class QuestionController : BaseController
    {
        readonly IClassService _classService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IQuestionService _questionService;
        readonly ISubjectService _subjectService;
        readonly IChapterService _chapterService;

        public QuestionController(IClassService classService, ILogger logger, IRepository repository, IQuestionService questionService, ISubjectService subjectService, IChapterService chapterService)
        {
            _classService = classService;
            _logger = logger;
            _repository = repository;
            _questionService = questionService;
            _subjectService = subjectService;
            _chapterService = chapterService;
        }

        public ActionResult Index()
        {
            //var questions = _questionService.GetQuestion().ToList();
            //var viewModelList = AutoMapper.Mapper.Map<List<QuestionProjection>, QuestionDataViewModel[]>(questions);
            //return View(viewModelList);
            return View();
        }

        public ActionResult Create()
        {
            var classList = (from c in _classService.GetClasses()
                             select new SelectListItem
                             {
                                 Value = c.ClassId.ToString(),
                                 Text = c.Name
                             }).ToList();

            return View(new QuestionDataViewModel { Classes = classList });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QuestionDataViewModel viewModel)
        {
            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.SubjectId = viewModel.SubjectId;
            ViewBag.ChapterId = viewModel.ChapterId;
            //viewModel.Classes = classList;

            Guid guid = Guid.NewGuid();
            string quesInfoPath = "", quesHiPath = "", quesOptPath = "";

            if (ModelState.IsValid)
            {
                if (viewModel.QuestionImageFile == null)
                    quesInfoPath = null;
                else
                    quesInfoPath = string.Format(@"~/{0}/Q{1}.jpg", Constants.QuestionImageFolder, guid);

                if (viewModel.HintImageFile == null)
                    quesHiPath = null;
                else
                    quesHiPath = string.Format(@"~/{0}/H{1}.jpg", Constants.QuestionImageFolder, guid);

                if (viewModel.OptionImageFile == null)
                    quesOptPath = null;
                else
                    quesOptPath = string.Format(@"~/{0}/O{1}.jpg", Constants.QuestionImageFolder, guid);

                var question = new Question
                {
                    Answer = viewModel.Answer.ToString(),
                    AttepmtCount = 0,
                    ChapterId = viewModel.ChapterId,
                    Hint = viewModel.Hint,
                    HintImagePath = quesHiPath,
                    IsHintAsImage = viewModel.IsHintAsImage,
                    IsOptionAsImage = viewModel.IsOptionAsImage,
                    IsQuestionAsImage = viewModel.IsQuestionAsImage,
                    OptionA = viewModel.OptionA,
                    OptionB = viewModel.OptionB,
                    OptionC = viewModel.OptionC,
                    OptionD = viewModel.OptionD,
                    OptionImagePath = quesOptPath,
                    QuestionImagePath = quesInfoPath,
                    QuestionInfo = viewModel.QuestionInfo,
                    QuestionLevel = viewModel.QuestionLevel,
                    QuestionType = viewModel.QuestionType,
                    Numerical_Answer = viewModel.Numerical_Answer,
                    Unit = viewModel.Unit,
                    QuestionYear = viewModel.QuestionYear,
                };

                var allowedExtensions = new[] { ".jpg", ".png", ".gif" };
                string queImgPath = Server.MapPath(string.Concat("~/", Constants.QuestionImageFolder));
                var questionImageCorrect = 1;
                var optionImageCorrect = 1;
                var hintImageCorrect = 1;

                if (!Directory.Exists(queImgPath))
                {
                    DirectoryInfo di = Directory.CreateDirectory(queImgPath);
                }

                if (viewModel.QuestionImageFile != null)
                {
                    var checkExtensionQuestion = Path.GetExtension(viewModel.QuestionImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(checkExtensionQuestion))
                    {
                        questionImageCorrect = 0;
                        ModelState.AddModelError("", "Please select valid format of question Image.");
                    }
                    else
                    {
                        questionImageCorrect = 1;
                        var pathToSaveQI = Path.Combine(queImgPath, string.Format("Q{0}.jpg", guid));
                        viewModel.QuestionImageFile.SaveAs(pathToSaveQI);
                    }
                }
                if (viewModel.HintImageFile != null)
                {
                    var checkExtensionQuestion = Path.GetExtension(viewModel.HintImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(checkExtensionQuestion))
                    {
                        hintImageCorrect = 0;
                        ModelState.AddModelError("", "Please select valid format of hint Image.");
                    }
                    else
                    {
                        hintImageCorrect = 1;
                        var pathToSaveHI = Path.Combine(queImgPath, string.Format("H{0}.jpg", guid));
                        viewModel.HintImageFile.SaveAs(pathToSaveHI);
                    }
                }
                if (viewModel.OptionImageFile != null)
                {
                    var checkExtensionQuestion = Path.GetExtension(viewModel.OptionImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(checkExtensionQuestion))
                    {
                        optionImageCorrect = 0;
                        ModelState.AddModelError("", "Please select valid format of option Image.");
                    }
                    else
                    {
                        optionImageCorrect = 1;
                        var pathToSaveOptImg = Path.Combine(queImgPath, string.Format("O{0}.jpg", guid));
                        viewModel.OptionImageFile.SaveAs(pathToSaveOptImg);
                    }
                }

                if (questionImageCorrect == 1 && hintImageCorrect == 1 && optionImageCorrect == 1)
                {
                    var success = _questionService.Save(question);
                    if (success.Success)
                    {
                        ViewBag.ClassId = 0;
                        ViewBag.SubjectId = 0;
                        ViewBag.ChapterId = 0;
                        Success("Question Added Successfully");
                        ModelState.Clear();
                        viewModel = new QuestionDataViewModel();
                    }
                    else
                    {
                        Danger("Error occured");
                        return View(viewModel);
                    }
                }
                else
                {
                    var pathToDeleteQ = Path.Combine(queImgPath, string.Format("Q{0}.jpg", guid));
                    if ((System.IO.File.Exists(pathToDeleteQ)))
                    {
                        System.IO.File.Delete(pathToDeleteQ);
                    }
                    var pathToDeleteO = Path.Combine(queImgPath, string.Format("O{0}.jpg", guid));
                    if ((System.IO.File.Exists(pathToDeleteO)))
                    {
                        System.IO.File.Delete(pathToDeleteO);
                    }
                    var pathToDeleteH = Path.Combine(queImgPath, string.Format("H{0}.jpg", guid));
                    if ((System.IO.File.Exists(pathToDeleteH)))
                    {
                        System.IO.File.Delete(pathToDeleteH);
                    }
                }
            }
            else
            {
                var allowedExtensions = new[] { ".jpg", ".png", ".gif" };

                if (viewModel.IsQuestionAsImage)
                {
                    if (viewModel.QuestionImageFile == null || viewModel.QuestionImageFile.ContentLength <= 0)
                    {
                        ModelState.AddModelError("", "Please Select Question Image File");
                    }
                    else
                    {
                        var checkExtensionQuestion = Path.GetExtension(viewModel.QuestionImageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(checkExtensionQuestion))
                        {
                            ModelState.AddModelError("", "Please select valid format of question Image.");
                        }
                    }
                }
                else
                {
                    if (viewModel.QuestionInfo == null)
                    {
                        ModelState.AddModelError("", "Please Enter QuestionInfo.");
                    }
                }

                if (viewModel.IsOptionAsImage)
                {
                    if (viewModel.OptionImageFile == null || viewModel.OptionImageFile.ContentLength <= 0)
                    {
                        ModelState.AddModelError("", "Please Select Option Image File");
                    }
                    else
                    {
                        var checkExtensionOption = Path.GetExtension(viewModel.OptionImageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(checkExtensionOption))
                        {
                            ModelState.AddModelError("", "Please select valid format of option Image.");
                        }
                    }
                }
                else
                {
                    if (viewModel.OptionA == null || viewModel.OptionB == null && viewModel.OptionC == null && viewModel.OptionC == null)
                    {
                        ModelState.AddModelError("", "Please Enter All Options.");
                    }
                }

                if (viewModel.IsHintAsImage)
                {
                    if (viewModel.HintImageFile == null || viewModel.HintImageFile.ContentLength <= 0)
                    {
                        ModelState.AddModelError("", "Please Select Hint Image File");
                    }
                    else
                    {
                        var checkExtensionHint = Path.GetExtension(viewModel.HintImageFile.FileName).ToLower();
                        if (!allowedExtensions.Contains(checkExtensionHint))
                        {
                            ModelState.AddModelError("", "Please select valid format of hint Image.");
                        }
                    }
                }
                else
                {
                    if (viewModel.Hint == null)
                    {
                        ModelState.AddModelError("", "Please Enter Hint Answer");
                    }
                }
            }

            var classList = (from c in _classService.GetClasses()
                             select new SelectListItem
                             {
                                 Value = c.ClassId.ToString(),
                                 Text = c.Name
                             }).ToList();

            viewModel.Classes = classList;
            return View(viewModel);
        }

        public ActionResult Edit()
        {
            return View();
        }

        public JsonResult GetQuestion(int id, int chapterId)
        {
            var projection = _questionService.GetQuestionById(id, chapterId);
            var viewModel = AutoMapper.Mapper.Map<QuestionProjection, QuestionGetViewModel>(projection);
            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQuestionCount(int classId, int? subjectId, int? chapterId)
        {
            subjectId = subjectId ?? 0;
            chapterId = chapterId ?? 0;
            var count = _questionService.GetQuestionCount(classId, (int)subjectId, (int)chapterId);
            return Json(count, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult Delete(int id)
        //{
        //    var projection = _questionService.GetQuestionById(id);

        //    if (projection == null)
        //    {
        //        _logger.Warn(string.Format("Question does not Exists {0}.", id));
        //        Warning("Question does not Exists.");
        //        return RedirectToAction("Index");
        //    }

        //    var viewModel = AutoMapper.Mapper.Map<QuestionProjection, QuestionViewModel>(projection);

        //    return View(viewModel);
        //}

        //[HttpPost]
        //public ActionResult Delete(QuestionViewModel viewModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var result = _questionService.Delete(viewModel.QuestionId);
        //        if (result.Success)
        //        {

        //            Success(result.Results.FirstOrDefault().Message);
        //            ModelState.Clear();
        //        }
        //        else
        //        {
        //            _logger.Warn(result.Results.FirstOrDefault().Message);
        //            Warning(result.Results.FirstOrDefault().Message, true);
        //        }
        //    }
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult SaveQuestion(QuestionCreateViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                Question question = new Question
                {
                    QuestionInfo = viewModel.QuestionInfo,
                    OptionA = viewModel.OptionA,
                    OptionB = viewModel.OptionB,
                    OptionC = viewModel.OptionC,
                    OptionD = viewModel.OptionD,
                    Answer = viewModel.Answer,
                    Hint = viewModel.Hint,
                    QuestionYear = viewModel.Year,
                    Numerical_Answer = viewModel.Numerical_Answer,
                    Unit = viewModel.Unit,
                    QuestionLevel = (QuestionLevel)viewModel.Level,
                    QuestionType = (QuestionType)viewModel.Type,
                    ChapterId = viewModel.ChapterId
                };
                cmsResult = _questionService.Save(question);
                var qid = question.QuestionId;
                cmsResult.Data = qid.ToString();
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

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public JsonResult UpdateQuestion(QuestionViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            if (ModelState.IsValid)
            {
                cmsResult = _questionService.Update(new Question
                {
                    QuestionId = viewModel.QuestionId,
                    QuestionInfo = viewModel.QuestionInfo,
                    OptionA = viewModel.OptionA,
                    OptionB = viewModel.OptionB,
                    OptionC = viewModel.OptionC,
                    OptionD = viewModel.OptionD,
                    Answer = viewModel.Answer,
                    Hint = viewModel.Hint,
                    QuestionYear = viewModel.Year,
                    Numerical_Answer = viewModel.Numerical_Answer,
                    Unit = viewModel.Unit,
                    QuestionLevel = (QuestionLevel)viewModel.Level,
                    QuestionType = (QuestionType)viewModel.Type,
                    ChapterId = viewModel.ChapterId
                });
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

        public JsonResult SaveQuestionImage(QuestionImageViewModel viewModel, bool isDelete)
        {
            var cmsResult = new CMSResult();

            try
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.QuestionImage.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Question image is required", IsSuccessful = false });
                    }
                    if (!Constants.ImageTypes.Contains(viewModel.QuestionImage.ContentType))
                    {
                        cmsResult.Results.Add(new Result { Message = "Please choose question image either a JPEG, JPG or PNG image.", IsSuccessful = false });
                    }
                    if (cmsResult.Success)
                    {
                        string folderPath = Server.MapPath(string.Concat("~/Images/", Constants.QuestionImageFolder));
                        cmsResult = _questionService.SaveQuestionImage(viewModel.QuestionId, isDelete, folderPath);
                        if (cmsResult.Success)
                        {
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            var pathToSaveQI = Path.Combine(folderPath, string.Format("Q{0}.jpg", viewModel.QuestionId));
                            if (viewModel.QuestionImage != null)
                                viewModel.QuestionImage.SaveAs(pathToSaveQI);
                        }
                        cmsResult.Results.Add(new Result { Message = "Question image uploaded successfully!", IsSuccessful = true });
                        _repository.CommitChanges();
                    }
                }
                else
                {
                    cmsResult.Results.Add(new Result { Message = "Please select image!", IsSuccessful = true });
                }
            }
            catch (Exception ex)
            {
                //Log here 
                _logger.Warn(string.Format("Something went wrong! Please try again. {0} {1}.", viewModel.QuestionId, ex.ToString()));
                //cmsResult.Results.Add(new Result("Something went wrong! Please try again.", false));
            }

            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveOptionImage(OptionImageViewModel viewModel, bool isDelete)
        {
            var cmsResult = new CMSResult();

            try
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.OptionImage.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Option image is required", IsSuccessful = false });
                    }
                    else if (!Constants.ImageTypes.Contains(viewModel.OptionImage.ContentType))
                    {
                        cmsResult.Results.Add(new Result { Message = "Please choose option image either a JPEG, JPG or PNG image.", IsSuccessful = false });
                    }
                    else
                    {
                        string folderPath = Server.MapPath(string.Concat("~/Images/", Constants.QuestionImageFolder));
                        var result = _questionService.SaveOptionImage(viewModel.QuestionId, isDelete, folderPath);
                        if (result.Success)
                        {
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            var pathToSaveQI = Path.Combine(folderPath, string.Format("O{0}.jpg", viewModel.QuestionId));
                            if (viewModel.OptionImage != null)
                                viewModel.OptionImage.SaveAs(pathToSaveQI);
                        }
                        cmsResult.Results.Add(new Result { Message = "Option image uploaded successfully!", IsSuccessful = true });
                        _repository.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                //Log here 
                _logger.Warn(string.Format("Something went wrong! Please try again. {0} {1}.", viewModel.QuestionId, ex.ToString()));
                //cmsResult.Results.Add(new Result("Something went wrong! Please try again.", false));
            }

            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveHintImage(HintImageViewModel viewModel, bool isDelete)
        {
            var cmsResult = new CMSResult();

            try
            {
                if (ModelState.IsValid)
                {
                    if (viewModel.HintImage.ContentLength == 0)
                    {
                        cmsResult.Results.Add(new Result { Message = "Hint image is required", IsSuccessful = false });
                    }
                    else if (!Constants.ImageTypes.Contains(viewModel.HintImage.ContentType))
                    {
                        cmsResult.Results.Add(new Result { Message = "Please choose hint image either a JPEG, JPG or PNG image.", IsSuccessful = false });
                    }
                    else
                    {
                        string folderPath = Server.MapPath(string.Concat("~/Images/", Constants.QuestionImageFolder));
                        var result = _questionService.SaveHintImage(viewModel.QuestionId, isDelete, folderPath);
                        if (result.Success)
                        {
                            if (!Directory.Exists(folderPath))
                            {
                                Directory.CreateDirectory(folderPath);
                            }

                            var pathToSaveQI = Path.Combine(folderPath, string.Format("H{0}.jpg", viewModel.QuestionId));
                            if (viewModel.HintImage != null)
                                viewModel.HintImage.SaveAs(pathToSaveQI);
                        }
                        cmsResult.Results.Add(new Result { Message = "Hint image uploaded successfully!", IsSuccessful = true });
                        _repository.CommitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                //Log here 
                _logger.Warn(string.Format("Something went wrong! Please try again. {0} {1}.", viewModel.QuestionId,ex.ToString()));
                //cmsResult.Results.Add(new Result("Something went wrong! Please try again.", false));
            }

            return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteImage(int questionId, int imageType)
        {
            var result = _questionService.DeleteImage(questionId, imageType);
            if (result.Success)
            {
                string queImgPath = Server.MapPath(string.Concat("~/Images/", Constants.QuestionImageFolder));
                if (imageType == 1)
                {
                    var pathToDeleteQ = Path.Combine(queImgPath, string.Format("Q{0}.jpg", questionId));
                    if ((System.IO.File.Exists(pathToDeleteQ)))
                    {
                        System.IO.File.Delete(pathToDeleteQ);
                    }
                }
                if (imageType == 2)
                {
                    var pathToDeleteO = Path.Combine(queImgPath, string.Format("O{0}.jpg", questionId));
                    if ((System.IO.File.Exists(pathToDeleteO)))
                    {
                        System.IO.File.Delete(pathToDeleteO);
                    }
                }
                if (imageType == 3)
                {
                    var pathToDeleteH = Path.Combine(queImgPath, string.Format("H{0}.jpg", questionId));
                    if ((System.IO.File.Exists(pathToDeleteH)))
                    {
                        System.IO.File.Delete(pathToDeleteH);
                    }
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
       
    }
}
