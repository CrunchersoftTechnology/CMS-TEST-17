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
using System.Threading;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class MachineController : BaseController
    {
        readonly IBranchService _branchService;
        readonly IMachineService _machineService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IBranchAdminService _branchAdminService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IEmailService _emailService;

        public MachineController(IMachineService machineService, ILogger logger, IRepository repository, IBranchService branchService, IBranchAdminService branchAdminService, IAspNetRoles aspNetRolesService, IEmailService emailService)
        {
            _machineService = machineService;
            _logger = logger;
            _repository = repository;
            _branchService = branchService;
            _branchAdminService = branchAdminService;
            _aspNetRolesService = aspNetRolesService;
            _emailService = emailService;

        }

        // GET: Machine
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            var machines = roles == "Admin" ? _machineService.GetMachines().ToList() : roles == "Client" ? _machineService.GetMachines().ToList() : roles == "BranchAdmin" ? _machineService.GetMachineByBranchId(projection.BranchId).ToList() : null;

            var viewModelList = AutoMapper.Mapper.Map<List<MachineProjection>, MachineViewModel[]>(machines);

            if (roles == "Admin" || roles=="Client")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.BranchId;
            }
            return View(viewModelList);
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

                ViewBag.BranchId = null;

                return View(new MachineViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles
                });
            }
            else if (roles == "BranchAdmin")
            {
                var projection = _branchAdminService.GetBranchAdminById(roleUserId);

                ViewBag.BranchId = projection.BranchId;
                return View(new MachineViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = projection.BranchId,
                    BranchName = projection.BranchName
                });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MachineViewModel viewModel)
        {
            var roles = viewModel.CurrentUserRole;
            var branchId = viewModel.BranchId;
            var branchName = viewModel.BranchName;
            if (ModelState.IsValid)
            {
                var result = _machineService.Save(new Machine { Name = viewModel.Name, SerialNumber = viewModel.SerialNumber, BranchId = viewModel.BranchId });
                if (result.Success)
                {
                    var message = "Machine Name :" + viewModel.Name + "<br/>Machine Serial No :" + viewModel.SerialNumber + "<br/> Machine created successsfully.";
                    SendMailToAdmin(message);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new MachineViewModel();
                }
                else
                {
                    var messages = "";
                    foreach (var message in result.Results)
                    {
                        messages += message.Message + "<br />";
                    }
                    _logger.Warn(messages);
                    Warning(messages, true);
                }
            }

            if (roles == "Admin" || roles=="Client")
            {
                var branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

                viewModel.Branches = branchList;
                ViewBag.BranchId = null;

                return View(new MachineViewModel
                {
                    Branches = branchList,
                    CurrentUserRole = roles
                });
            }
            else if (roles == "BranchAdmin")
            {
                return View(new MachineViewModel
                {
                    CurrentUserRole = roles,
                    BranchId = branchId,
                    BranchName = branchName
                });
            }

            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            var projection = _machineService.GetMachineById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Machine not Exists!"));
                Warning("Machine not Exists.");
                return RedirectToAction("Index");
            }

            var viewModel = AutoMapper.Mapper.Map<MachineProjection, MachineViewModel>(projection);

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "Admin" || roles=="Client")
            {
                viewModel.CurrentUserRole = roles;
                ViewBag.branchList = (from b in _branchService.GetAllBranches()
                                      select new SelectListItem
                                      {
                                          Value = b.BranchId.ToString(),
                                          Text = b.Name
                                      }).ToList();
                ViewBag.BranchId = projection.BranchId;
                return View(viewModel);
            }
            else if (roles == "BranchAdmin")
            {
                viewModel.CurrentUserRole = roles;
            }

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MachineViewModel viewModel)
        {
            ViewBag.BranchId = viewModel.BranchId;
            if (ModelState.IsValid)
            {
                var machine = _repository.Project<Machine, bool>(machines => (from m in machines where m.MachineId == viewModel.MachineId select m).Any());

                if (!machine)
                {
                    _logger.Warn(string.Format("Machine '{0}' not exists.", viewModel.Name));
                    Danger(string.Format("Machine '{0}' not exists.", viewModel.Name));
                }

                var result = _machineService.Update(new Machine { MachineId = viewModel.MachineId, SerialNumber = viewModel.SerialNumber, Name = viewModel.Name, BranchId = viewModel.BranchId });
                if (result.Success)
                {
                    var message = "Machine Name :" + viewModel.Name + "<br/>Machine Serial No :" + viewModel.SerialNumber + "<br/> Machine updated successsfully.";
                    SendMailToAdmin(message);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    return RedirectToAction("Index");
                }
                else
                {
                    var messages = "";
                    foreach (var message in result.Results)
                    {
                        messages += message.Message + "<br />";
                    }
                    _logger.Warn(messages);
                    Warning(messages, true);
                }
            }

            if (viewModel.CurrentUserRole == "Admin" || viewModel.CurrentUserRole == "Client")
            {
                ViewBag.branchList = (from b in _branchService.GetAllBranches()
                                      select new SelectListItem
                                      {
                                          Value = b.BranchId.ToString(),
                                          Text = b.Name
                                      }).ToList();
            }
            else if (viewModel.CurrentUserRole == "BranchAdmin")
            {
            }

            return View(viewModel);
        }


        public ActionResult Delete(int id)
        {
            var projection = _machineService.GetMachineById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Machine not Exists {0}.", id));
                Warning("Machine not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<MachineProjection, MachineViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(MachineViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var result = _machineService.Delete(viewModel.MachineId);
                if (result.Success)
                {
                    var message = "Machine Name :" + viewModel.Name + "<br/>Machine Serial No :" + viewModel.SerialNumber + "<br/> Machine deleted successsfully.";
                    SendMailToAdmin(message);
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

        public void SendMailToAdmin(string message)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            if (roles == "BranchAdmin")
            {
                var branchAdmin = _branchAdminService.GetBranchAdminById(roleUserId);
                var branchAdminName = branchAdmin.Name;
                var branchAdminEmail = branchAdmin.Email;
                var branchName = branchAdmin.BranchName;
                new Thread(() =>
                {
                    string body = string.Empty;
                    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                    {
                        body = reader.ReadToEnd();
                    }
                    body = body.Replace("{BranchName}", branchName);
                    body = body.Replace("{ModuleName}", message);
                    body = body.Replace("{BranchAdminName}", branchAdminName);
                    body = body.Replace("{BranchAdminEmail}", "( " + branchAdminEmail + " )");
                    var emailMessage = new MailModel
                    {
                        Body = body,
                        Subject = "Web portal changes Machine",
                        To = ConfigurationManager.AppSettings[Common.Constants.AdminEmail]
                    };
                    _emailService.Send(emailMessage);
                }).Start();
            }
        }
    }
}