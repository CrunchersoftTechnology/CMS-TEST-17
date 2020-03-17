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
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class MasterFeeController : BaseController
    {

        readonly IClassService _classService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly ISubjectService _subjectService;
        readonly IMasterFeeService _masterFeeService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IClientAdminService _clientAdminService;
        readonly IBranchAdminService _branchAdminService;

        public MasterFeeController(IBranchAdminService branchAdminService, IClientAdminService clientAdminService,IClassService classService, ILogger logger, IRepository repository,
            ISubjectService subjectService, IMasterFeeService masterFeeService, IEmailService emailService, IAspNetRoles aspNetRolesService)
        {
            _classService = classService;
            _logger = logger;
            _repository = repository;
            _subjectService = subjectService;
            _masterFeeService = masterFeeService;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _clientAdminService = clientAdminService;
            _branchAdminService = branchAdminService;
        }

        // GET: MasterFee
        public ActionResult Index( int? subjectId, int? classId)
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

            var masterFee = (subjectId == null && classId == null) ? _masterFeeService.GetAllMasterFees().ToList()
                : _masterFeeService.GetMasterFees((int)subjectId, (int)classId).ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<MasterFeeProjection>, MasterFeeViewModel[]>(masterFee);
            ViewBag.ClassId = classId;
            ViewBag.SubjectId = subjectId;
            ViewBag.userRole = roles;



            ViewBag.userRole = roles;
            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else if (roles == "Client")
            {
                ViewBag.userId = projection.ClientId;
            }
            else
            {
                ViewBag.userId = projectionBranch.ClientId;
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
                var viewModel = new MasterFeeViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(viewModel);
            }

            else if (roles == "Client")
            {
                var classes = _classService.GetClassesByClientId(Convert.ToInt32(projection.ClientId)).ToList();
                var viewModel = new MasterFeeViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(new MasterFeeViewModel
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
                var viewModel = new MasterFeeViewModel();
                viewModel.Classes = new SelectList(classes, "ClassId", "Name");
                return View(new MasterFeeViewModel
                {
                    Classes = viewModel.Classes,
                    CurrentUserRole = roles,
                    ClientId = projectionBranch.ClientId,
                    ClientName = projectionBranch.ClientName
                });
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MasterFeeViewModel viewModel)
        {
            ViewBag.ClassId = viewModel.ClassId;
            ViewBag.SubjectId = viewModel.SubjectId;
            //var classes = _classService.GetClasses().ToList();
            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;

           
           
            if (ModelState.IsValid)
            {
                var result = _masterFeeService.Save(clientId,new MasterFee
                {
                    ClientId=clientId,
                    ClassId = viewModel.ClassId,
                    SubjectId = viewModel.SubjectId,
                    Year = viewModel.Year,
                    Fee = viewModel.Fee
                });

                if (result.Success)
                {
                    ViewBag.ClassId = 0;
                    ViewBag.SubjectId = 0;
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }

            viewModel = new MasterFeeViewModel();

            var classes = _classService.GetClasses().ToList();
            viewModel.Classes = new SelectList(classes, "ClassId", "Name");

            var subjects = _subjectService.GetSubjects(viewModel.ClassId).ToList();
            viewModel.Subjects = new SelectList(subjects, "SubjectId", "Name");


            return View(viewModel);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(int id)
        {
            ViewBag.SelectedClasses = from mt in _classService.GetClasses()
                                      select new SelectListItem
                                      {
                                          Value = mt.ClassId.ToString(),
                                          Text = mt.Name
                                      };

            ViewBag.SelectedSubjects = from mt in _subjectService.GetAllSubjects()
                                       select new SelectListItem
                                       {
                                           Value = mt.SubjectId.ToString(),
                                           Text = mt.Name
                                       };

            var projection = _masterFeeService.GetMasterFeeById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("MasterFee does not Exists {0}.", id));
                Warning("MasterFee does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.SubjectId = projection.SubjectId;
            ViewBag.ClassId = projection.ClassId;

            var viewModel = AutoMapper.Mapper.Map<MasterFeeProjection, MasterFeeViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(MasterFeeViewModel viewModel)
        {
            ViewBag.SelectedClasses = from mt in _classService.GetClasses()
                                      select new SelectListItem
                                      {
                                          Value = mt.ClassId.ToString(),
                                          Text = mt.Name
                                      };
            ViewBag.ClassId = viewModel.ClassId;

            ViewBag.SelectedSubjects = from mt in _subjectService.GetAllSubjects()
                                       select new SelectListItem
                                       {
                                           Value = mt.SubjectId.ToString(),
                                           Text = mt.Name
                                       };
            ViewBag.SubjectId = viewModel.SubjectId;

            if (ModelState.IsValid)
            {
                var masterfees = _repository.Project<MasterFee, bool>(
                                    masterf => (from mfee in masterf
                                                where mfee.MasterFeeId == viewModel.MasterFeeId
                                                select mfee).Any());
                if (!masterfees)
                {
                    _logger.Warn(string.Format("MasterFee not exists for this year '{0}'.", viewModel.Year));
                    Danger(string.Format("MasterFee not exists for this year '{0}'.", viewModel.Year));
                }
                var result = _masterFeeService.Update(new MasterFee { MasterFeeId = viewModel.MasterFeeId, ClassId = viewModel.ClassId, SubjectId = viewModel.SubjectId, Year = viewModel.Year, Fee = viewModel.Fee });
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
            return View(viewModel);
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Delete(int id)
        {
            var projection = _masterFeeService.GetMasterFeeById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("MasterFee does not Exists {0}.", id));
                Warning("MasterFee does not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<MasterFeeProjection, MasterFeeViewModel>(projection);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(MasterFeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _masterFeeService.Delete(viewModel.MasterFeeId);
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
    }
}