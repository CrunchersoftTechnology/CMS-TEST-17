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
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class SchoolController : BaseController
    {
        readonly ISchoolService _schoolService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IEmailService _emailService;
        readonly IBranchAdminService _branchAdminService;
        readonly IClientAdminService _clientAdminService;

        public SchoolController(IClientAdminService clientAdminService,ISchoolService schoolService, ILogger logger, IRepository repository, IAspNetRoles aspNetRolesService, IEmailService emailService, IBranchAdminService branchAdminService)
        {
            _schoolService = schoolService;
            _logger = logger;
            _repository = repository;
            _aspNetRolesService = aspNetRolesService;
            _emailService = emailService;
            _branchAdminService = branchAdminService;
            _clientAdminService = clientAdminService;

        }

        // GET: School
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var projectionBranch = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;


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
            return View();
        }

        // GET: School/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: School/Create
        public ActionResult Create(int? ClientId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "Admin")
            {
                var clientList = (from b in _clientAdminService.GetClients()
                                  select new SelectListItem
                                  {
                                      Value = b.ClientId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                ViewBag.ClientId = null;

                return View(new SchoolViewModel
                {
                    Clients = clientList,
                    CurrentUserRole = roles
                });
            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(roleUserId);

                ViewBag.ClientId = projection.ClientId;
                ViewBag.CurrentUserRole = roles;
                return View(new SchoolViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });
            }
            else if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(roleUserId);

                ViewBag.ClientId = projection.ClientId;
                ViewBag.CurrentUserRole = roles;
                return View(new SchoolViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                   
                });
            }

            return View();
        }

        // POST: School/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SchoolViewModel viewModel)
        {

            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;
            if (ModelState.IsValid)
            {
                var school = new School
                {
               
                    Name = viewModel.Name,
                    CenterNumber=viewModel.CenterNumber,
                    ClientId = viewModel.ClientId,
                };

                var result = _schoolService.Save(school);
                if (result.Success)
                {

                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                    ReturnViewModel(roles, viewModel, clientId, clientName);
                }
            }
            viewModel = new SchoolViewModel();
            ReturnViewModel(roles, viewModel, clientId, clientName);
            return View(viewModel);
        }
        public void ReturnViewModel(string roles, SchoolViewModel viewModel, int clientId, string clientName)
        {
            if (roles == "Admin")
            {
                var clientList = (from b in _clientAdminService.GetClients()
                                  select new SelectListItem
                                  {
                                      Value = b.ClientId.ToString(),
                                      Text = b.Name
                                  }).ToList();
                viewModel.Clients = clientList;
                ViewBag.ClientId = null;
            }
            else if (roles == "Client" || roles == "BranchAdmin")
            {
                viewModel.ClientId = clientId;
                viewModel.ClientName = clientName;

            }
           

            viewModel.CurrentUserRole = roles;
        }



    // GET: School/Edit/5
    public ActionResult Edit(int id)
        {
            var projection = _schoolService.GetSchoolById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("School does not Exists {0}.", id));
                Warning("School does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<SchoolProjection, SchoolViewModel>(projection);
            return View(viewModel);
        }

        // POST: School/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SchoolViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var board = _repository.Project<School, bool>(schools => (from s in schools where s.SchoolId == viewModel.SchoolId select s).Any());
                if (!board)
                {
                    _logger.Warn(string.Format("School not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("School not exists '{0}'.", viewModel.Name));
                }
                var result = _schoolService.Update(new School
                {
                    SchoolId = viewModel.SchoolId,
                    Name = viewModel.Name,
                    CenterNumber = viewModel.CenterNumber == null ? "" : viewModel.CenterNumber.Trim(),
                });
                if (result.Success)
                {
                    var roleUserId = User.Identity.GetUserId();
                    var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                    var bodySubject = "Web portal changes - School update";
                    var message = "School Updated Successfully";
                    SendMailToAdmin(roles, roleUserId, message, viewModel.Name, bodySubject);
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

        // GET: School/Delete/5
        public ActionResult Delete(int id)
        {
            var projection = _schoolService.GetSchoolById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("School does not Exists {0}.", id));
                Warning("School does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<SchoolProjection, SchoolViewModel>(projection);
            return View(viewModel);
        }

        // POST: School/Delete/5
        [HttpPost]
        public ActionResult Delete(SchoolViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _schoolService.Delete(viewModel.SchoolId);
                if (result.Success)
                {
                    var roleUserId = User.Identity.GetUserId();
                    var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
                    var bodySubject = "Web portal changes - School Delete";
                    var message = "School Deleted Successfully";
                    SendMailToAdmin(roles, roleUserId, message, viewModel.Name, bodySubject);
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

        public void SendMailToAdmin(string roles, string roleUserId, string message, string Name, string bodySubject)
        {
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
                body = body.Replace("{ModuleName}", Name + " " + message);
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
