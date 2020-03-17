using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
    public class BranchController : BaseController
    {
        readonly IBranchService _branchService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IClientAdminService _clientAdminService;

        public BranchController(IClientAdminService clientAdminService, IAspNetRoles aspNetRolesService, IBranchService branchService, ILogger logger, IRepository repository)
        {
            _branchService = branchService;
            _logger = logger;
            _repository = repository;
            _aspNetRolesService = aspNetRolesService;
            _clientAdminService = clientAdminService;
        }

        // GET: Branch
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;


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

            }
            return View();
        }

        // GET: Branch/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Branch/Create
        public ActionResult Create(int? ClientId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            if (roles == "Admin")
            {
                ViewBag.CurrentUserRole = roles;
                return View(new BranchViewModel
                { 
                     CurrentUserRole = roles
                });

            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(roleUserId);

                ViewBag.ClientId = projection.ClientId;
                ViewBag.CurrentUserRole = roles;
                return View(new BranchViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });
            }

            return View();
        }
        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BranchViewModel viewModel)
        {

            var roles = viewModel.CurrentUserRole;
            var clientId = viewModel.ClientId;
            var clientName = viewModel.ClientName;
            if (ModelState.IsValid)
            {
                var branch = new Branch
                {
                    Address = viewModel.Address,
                    Name = viewModel.Name,
                    ClientId=viewModel.ClientId,
                   // UserId = viewModel.UserId


                };
                if (roles == "Admin")
                {
                    var result = _branchService.Save(branch);
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
                else if (roles == "Client")
                {

                    var result = _branchService.SaveBranch(branch);
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
            }
            viewModel = new BranchViewModel();
            ReturnViewModel(roles, viewModel, clientId, clientName);
            return View(viewModel);

        }
        public void ReturnViewModel(string roles, BranchViewModel viewModel, int clientId, string clientName)
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
            else if (roles == "Client")
            {
                viewModel.ClientId = clientId;
                viewModel.ClientName = clientName;
            }
            
            viewModel.CurrentUserRole = roles;
        }

        // GET: Branch/Edit/5
        public ActionResult Edit(int id)
        {
            var projection = _branchService.GetBoardById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Branch does not Exists {0}.", id));
                Warning("Branch does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<BranchProjection, BranchViewModel>(projection);
            return View(viewModel);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BranchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var board = _repository.Project<Branch, bool>(branches => (from b in branches where b.BranchId == viewModel.BranchId select b).Any());
                if (!board)
                {
                    _logger.Warn(string.Format("Branch not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Branch not exists '{0}'.", viewModel.Name));
                }
                var result = _branchService.Update(new Branch
                {
                    Name = viewModel.Name,
                    Address = viewModel.Address,
                    BranchId = viewModel.BranchId
                });
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

        // GET: Branch/Delete/5
        public ActionResult Delete(int id)
        {
            var projection = _branchService.GetBoardById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Board does not Exists {0}.", id));
                Warning("Board does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<BranchProjection, BranchViewModel>(projection);
            return View(viewModel);
        }

        // POST: Branch/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(BranchViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _branchService.Delete(viewModel.BranchId);
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
