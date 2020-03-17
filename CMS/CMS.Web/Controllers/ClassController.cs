using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class ClassController : BaseController
    {
        readonly IClassService _classService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IClientAdminService _clientAdminService;

        public ClassController(IClientAdminService clientAdminService,IClassService classService, ILogger logger, IRepository repository, IEmailService emailService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService)
        {
            _classService = classService;
            _logger = logger;
            _repository = repository;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _clientAdminService = clientAdminService;

        }

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

                return View(new ClassViewModel
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
                return View(new ClassViewModel
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
                return View(new ClassViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                  
                });
            }

            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ClassViewModel viewModel)
        {

            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;
            if (ModelState.IsValid)
            {
                var classes = new Class
                {
                  
                    Name = viewModel.Name,
                    ClientId = viewModel.ClientId,
                };

                var result = _classService.Save(classes);
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
            viewModel = new ClassViewModel();
            ReturnViewModel(roles, viewModel, clientId, clientName);
            return View(viewModel);
        }
        public void ReturnViewModel(string roles, ClassViewModel viewModel, int clientId, string clientName)
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
            else if (roles == "Client" || roles=="BranchAdmin")
            {
                viewModel.ClientId = clientId;
                viewModel.ClientName = clientName;
            }

            viewModel.CurrentUserRole = roles;
        }

        public ActionResult Edit(int id)
        {
            var projection = _classService.GetClassById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Class does not Exists {0}.", id));
                Warning("Class does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<ClassProjection, ClassViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ClassViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var cls = _repository.Project<Class, bool>(classes => (from clss in classes where clss.ClassId == viewModel.ClassId select clss).Any());

                if (!cls)
                {
                    _logger.Warn(string.Format("Class not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Class not exists '{0}'.", viewModel.Name));
                }

                var result = _classService.Update(new Class { ClassId = viewModel.ClassId, Name = viewModel.Name });
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - Class updated";
                    SendMailToAdmin(result.Results.FirstOrDefault().Message, bodySubject);
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

        public ActionResult Delete(int id)
        {
            var projection = _classService.GetClassById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Class does not Exists {0}.", id));
                Warning("Class does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<ClassProjection, ClassViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(ClassViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var result = _classService.Delete(viewModel.ClassId);
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - Class Delete";
                    SendMailToAdmin(result.Results.FirstOrDefault().Message, bodySubject);
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

        //public ActionResult GetClasses()
        //{
        //    var subjects = _classService.GetClasses().Select(x => new { output = new { id = x.ClassId, name = x.Name }, selected = "" });
        //    return Json(subjects, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetClasses()
        {
            var subjects = _classService.GetClasses().Select(x => new { x.ClassId, x.Name });
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        public void SendMailToAdmin(string message, string bodySubject)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                var branchName = branchAdmin.BranchName;

                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", branchName);
                body = body.Replace("{ModuleName}", message);
                body = body.Replace("{BranchAdminEmail}", "( " + User.Identity.GetUserName() + " )");
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsBranchAdmin = true
                };
                _emailService.Send(emailMessage);
            }
        }

        [HttpPost]
        public JsonResult GetClasses1(string Prefix)
        {
            var Class = _classService.GetAllClassByPrefix(Prefix);
            return Json(Class, JsonRequestBehavior.AllowGet);
        }
    }
}