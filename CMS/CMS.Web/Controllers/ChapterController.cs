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
using System.Threading;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class ChapterController : BaseController
    {
        readonly ISubjectService _subjectService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IChapterService _chapterService;
        readonly IClassService _classService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IClientAdminService _clientAdminService;

        public ChapterController(IClientAdminService clientAdminService,ISubjectService subjectService, ILogger logger, IRepository repository, IChapterService chapterService, IClassService classService, IEmailService emailService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService)
        {
            _subjectService = subjectService;
            _logger = logger;
            _repository = repository;
            _chapterService = chapterService;
            _classService = classService;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _clientAdminService = clientAdminService;

        }

        // GET: Chapter
        public ActionResult Index(int? subjectId, int? classId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

            ViewBag.ClassList = (from c in _classService.GetClasses()
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();

            ViewBag.ClassId = classId;
            ViewBag.SubjectId = subjectId;
            var chapters = (classId == null && subjectId == null) ? _chapterService.GetAllChapters().ToList() : _chapterService.GetChapters((int)subjectId, (int)classId).ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<ChapterProjection>, ChapterViewModel[]>(chapters);
            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.ClientId;
            }
          
            return View(viewModelList);
        }
        public ActionResult Create()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

           
            if (roles == "Admin")
            {
                var classes = _classService.GetClasses().ToList();
                var viewModel = new ChapterViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(viewModel);
            }
            else if (roles == "Client")
            {
                var classes = _classService.GetClassesByClientId(Convert.ToInt32(projection.ClientId)).ToList();
                var viewModel = new ChapterViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(new ChapterViewModel
                {
                    Classes = viewModel.Classes,
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChapterViewModel viewModel)
        {
            ViewBag.ClassId = viewModel.ClassId;
            var classes = _classService.GetClasses().ToList();
            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;

            if (ModelState.IsValid)
            {
                var WeightageTotal = _chapterService.GetCountWeightage(viewModel.ClassId, viewModel.SubjectId);
                ViewBag.Weightage = WeightageTotal.ToString();
                var wChk = WeightageTotal + viewModel.Weightage;
                var result = _chapterService.Save(clientId, new Chapter { Name = viewModel.Name, SubjectId = viewModel.SubjectId, Weightage = viewModel.Weightage });
                if (result.Success)
                {
                    if (wChk > 100)
                    {
                        Warning("Weightage more than 100.");
                    }
                    var bodySubject = "Web portal changes - Chapter Create";
                    var message = ", ClassName :" + viewModel.ClassName + ", SubjectName :" + viewModel.SubjectName + " Created Successfully";
                    SendMailToAdmin(message, viewModel.Name, bodySubject);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new ChapterViewModel();
                    ViewBag.Weightage = '0';
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }

            ViewBag.SubjectId = viewModel.SubjectId;
            ViewBag.ClassId = viewModel.ClassId;

            //var classes = _classService.GetClasses().ToList();
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");
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

            var projection = _chapterService.GetChapterById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Chapter does not Exists {0}.", id));
                Warning("Chapter does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.ClassId = projection.ClassId;
            ViewBag.SubjectId = projection.SubjectId;

            var viewModel = AutoMapper.Mapper.Map<ChapterProjection, ChapterViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChapterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var chapter = _repository.Project<Chapter, bool>(chapters => (from chap in chapters where chap.ChapterId == viewModel.ChapterId select chap).Any());
                if (!chapter)
                {
                    _logger.Warn(string.Format("Chapter name not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Chapter name not exists '{0}'.", viewModel.Name));
                }
                var result = _chapterService.Update(new Chapter { ChapterId = viewModel.ChapterId, SubjectId = viewModel.SubjectId, Name = viewModel.Name, Weightage = viewModel.Weightage });
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - Chapter update";
                    var message = "ClassName :" + viewModel.ClassName + "<br/>SubjectName :" + viewModel.SubjectName + "<br/>Updated Successfully";
                    SendMailToAdmin(message, viewModel.Name, bodySubject);
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

            ViewBag.SelectedClass = from mt in _classService.GetClasses()
                                    select new SelectListItem
                                    {
                                        Value = mt.ClassId.ToString(),
                                        Text = mt.Name
                                    };

            ViewBag.SubjectId = viewModel.SubjectId;
            ViewBag.ClassId = viewModel.ClassId;
            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var projection = _chapterService.GetChapterById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Chpter does not Exists {0}.", id));
                Warning("Chapter does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<ChapterProjection, ChapterViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ChapterViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _chapterService.Delete(viewModel.ChapterId);
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - Chapter delete";
                    var message = "ClassName :" + viewModel.ClassName + ", SubjectName :" + viewModel.SubjectName + " deleted Successfully";
                    SendMailToAdmin(message, viewModel.Name, bodySubject);
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

        public ActionResult GetChapters(int subjectId)
        {
            var subjects = _chapterService.GetAllChapters().Where(x => x.SubjectId == subjectId).Select(x => new { x.ChapterId, x.ChapterName });
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QGetChapters(string[] depdrop_parents)
        {
            var v = string.IsNullOrEmpty(depdrop_parents[0]) ? "0" : depdrop_parents[0];
            var subjects = _chapterService.GetAllChapters().Where(x => x.SubjectId == Convert.ToInt32(v)).Select(x => new { id = x.ChapterId, name = x.ChapterName });
            var obj = new { output = subjects, selected = "" };
            return Json(obj);
        }

        public ActionResult GetPaperChapters(int subjectId)
        {
            var subjects = _chapterService.GetAllPaperChapters(subjectId);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeightageCount(int classId, int subjectId)
        {
            var result = _chapterService.GetCountWeightage(classId, subjectId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public void SendMailToAdmin(string message, string Name, string bodySubject)
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
                body = body.Replace("{ModuleName}", "ChapterName :" + Name + "," + message);
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
    }
}