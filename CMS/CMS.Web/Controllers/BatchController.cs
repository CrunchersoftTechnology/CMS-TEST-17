using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class BatchController : BaseController
    {
        readonly IClassService _classService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IBatchService _batchService;
        readonly ISubjectService _subjectService;
        readonly IStudentService _studentService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IClientAdminService _clientAdminService;
        readonly IBranchAdminService _branchAdminService;

        public BatchController(IClientAdminService clientAdminService,IClassService classService, ILogger logger, IRepository repository, IBatchService batchService, ISubjectService subjectService,
            IStudentService studentService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService)
        {
            _classService = classService;
            _logger = logger;
            _repository = repository;
            _batchService = batchService;
            _subjectService = subjectService;
            _studentService = studentService;
            _aspNetRolesService = aspNetRolesService;
            _clientAdminService = clientAdminService;
            _branchAdminService = branchAdminService;
        }

        // GET: Batch
        public ActionResult Index(int? classId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var projectionBranch = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;

            ViewBag.ClassList = (from c in _classService.GetClasses()
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.Name
                                 }).ToList();

            ViewBag.ClassId = classId;
            var batches = (classId == null) ? _batchService.GetAllBatches().ToList() : _batchService.GetBatches((int)classId).ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<BatchProjection>, BatchViewModel[]>(batches);
            ViewBag.userRole = roles;

            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else if (roles == "BranchAdmin")
            {
                ViewBag.userId = projectionBranch.ClientId;
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
            var projectionBranch = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;

            if (roles == "Admin")
            {
                var classes = _classService.GetClasses().ToList();
                var viewModel = new BatchViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(viewModel);
            }
            else if (roles == "Client")
            {
                var classes = _classService.GetClassesByClientId(Convert.ToInt32(projection.ClientId)).ToList();
                var viewModel = new BatchViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(new BatchViewModel
                {
                    Classes = viewModel.Classes,
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });
               
            }
            else if (roles == "BranchAdmin")
            {
                var classes = _classService.GetClassesByClientId(Convert.ToInt32(projectionBranch.ClientId)).ToList();
                var viewModel = new BatchViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(new BatchViewModel
                {
                    Classes = viewModel.Classes,
                    CurrentUserRole = roles,
                    ClientId = projectionBranch.ClientId,
                    
                });

            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(BatchViewModel viewModel)
        {
            ViewBag.ClassId = viewModel.ClassId;
            var classes = _classService.GetClasses().ToList();
            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;

            if (ModelState.IsValid)
            {
                if (viewModel.InTime != null && viewModel.OutTime != null)
                {
                    TimeSpan span = (Convert.ToDateTime(viewModel.OutTime) - Convert.ToDateTime(viewModel.InTime));
                    if (span < TimeSpan.FromHours(1) || span > TimeSpan.FromHours(6))
                    {
                        _logger.Warn(string.Format("The time limit should be min lengh of (1hr) & max length of  (6hrs)"));
                        Danger(string.Format("The time limit should be min lengh of (1hr) & max length of  (6hrs)"));
                        viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                        return View(viewModel);
                    }
                }
                var result = _batchService.Save(clientId, new Batch { Name = viewModel.Name, ClassId = viewModel.ClassId, InTime = (viewModel.InTime == null ? DateTime.Now.Date : Convert.ToDateTime(viewModel.InTime.Trim())), OutTime = (viewModel.OutTime == null ? DateTime.Now.Date : Convert.ToDateTime(viewModel.OutTime.Trim())) });

                // var dt = new DateTime(2017, 12, 23, 05, 36, 50).ToUniversalTime();
                if (result.Success)
                {
                    ViewBag.ClassId = 0;
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new BatchViewModel();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");
            return View(viewModel);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedClass = from mt in _classService.GetClasses()
                                    select new SelectListItem
                                    {
                                        Value = mt.ClassId.ToString(),
                                        Text = mt.Name
                                    };

            var projection = _batchService.GetBatcheById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Batch does not Exists {0}.", id));
                Warning("Batch does not Exists.");
                return RedirectToAction("Index");
            }
            ViewBag.ClassId = projection.ClassId;

            var viewModel = AutoMapper.Mapper.Map<BatchProjection, BatchViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(BatchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var batch = _repository.Project<Batch, bool>(batches => (from btch in batches where btch.BatchId == viewModel.BatchId select btch).Any());

                if (!batch)
                {
                    _logger.Warn(string.Format("Batch not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Batch not exists '{0}'.", viewModel.Name));
                    return RedirectToAction("Edit");
                }
                if (viewModel.OutTime != null && viewModel.OutTime != null && Convert.ToDateTime(viewModel.InTime).ToShortTimeString() != "12:00 AM" && Convert.ToDateTime(viewModel.OutTime).ToShortTimeString() != "12:00 AM")
                {
                    TimeSpan span = (Convert.ToDateTime(viewModel.OutTime) - Convert.ToDateTime(viewModel.InTime));
                    if (span < TimeSpan.FromHours(1) || span > TimeSpan.FromHours(6))
                    {
                        _logger.Warn(string.Format("The time limit should be min lengh of (1hr) & max length of  (6hrs)", viewModel.Name));
                        Danger(string.Format("The time limit should be min lengh of (1hr) & max length of  (6hrs)", viewModel.Name));
                        return RedirectToAction("Edit");
                    }
                }
                var result = _batchService.Update(new Batch { BatchId = viewModel.BatchId, Name = viewModel.Name/*, ClassId = viewModel.ClassId*/, InTime = Convert.ToDateTime(viewModel.InTime), OutTime = Convert.ToDateTime(viewModel.OutTime) });
                if (result.Success)
                {
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

            ViewBag.ClassId = viewModel.ClassId;
            return View(viewModel);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Delete(int id)
        {
            var projection = _batchService.GetBatcheById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Batch does not Exists {0}.", id));
                Warning("Batch does not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<BatchProjection, BatchViewModel>(projection);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(BatchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _batchService.Delete(viewModel.BatchId);
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

        public ActionResult GetSubjectsAndBatches(int classId)
        {
            var subjects = _batchService.GetAllBatches().Where(x => x.ClassId == classId).Select(x => new { x.BatchId, x.BatchName });
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBatchesByClassId(string selectedClasses, string selectedBranch)
        {
            var studentCount = _studentService.GetStudentsByClassBranch(selectedClasses, selectedBranch).Select(x => new { x.ClassId, x.ClassName }).Count();
            var batches = _batchService.GetBatchesByClassIds(selectedClasses).Select(x => new { x.BatchId, x.BatchName, x.ClassName });
            var result = new
            {
                studentParentCount = studentCount,
                batches = batches
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCountByBatches(string selectedBatches, string selectedClasses, string selectedBranch)
        {
            int studentCount = 0;
            var studentParentList = _studentService.GetStudentsByClassBranch(selectedClasses, selectedBranch).Select(x => new { x.ClassId, x.ClassName, x.BatchId /*, x.SelectedBatches*/ });
            var batchIds = selectedBatches.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
            foreach (var student in studentParentList)
            {
                int studentCountm = 0;
                //foreach (var batch in student.b)
                //{
                    if (batchIds.Contains(student.BatchId))
                    {
                        studentCountm++;
                    }
                //}
                if (studentCountm > 0)
                    studentCount++;
            }
            return Json(studentCount, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetBatches(string Prefix)
        {
           CMSDbContext db = new CMSDbContext();
            var Batch = (from c in db.Batches
                         where c.Name.StartsWith(Prefix)
                         select new { c.Name });
            return Json(Batch, JsonRequestBehavior.AllowGet);

            /*  var batch = (from s in _batchService.GetAllBatches()
                            where s.BatchName.StartsWith(Prefix)
                            select new BatchProjection
                            {
                                BatchName = s.BatchName
                            });

             return Json(batch, JsonRequestBehavior.AllowGet); 

             var batch = _batchService.GetAllBatchByPrefix(Prefix);
              return Json(batch, JsonRequestBehavior.AllowGet); */
        }

    }
    }