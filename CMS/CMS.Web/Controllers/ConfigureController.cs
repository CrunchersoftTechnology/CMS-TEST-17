using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class ConfigureController : BaseController
    {
        readonly IConfigureServices _configureServices;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly ILocalDateTimeService _localDateTimeService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IClientAdminService _clientAdminService;
        public ConfigureController(IClientAdminService clientAdminService, IConfigureServices configureServices, IAspNetRoles aspNetRolesService, ILocalDateTimeService localDateTimeService, ILogger logger, IRepository repository)
        {
            _clientAdminService = clientAdminService;
            _configureServices = configureServices;
            _logger = logger;
            _repository = repository;
            _localDateTimeService = localDateTimeService;
            _aspNetRolesService = aspNetRolesService;
        }

        // GET: Configure
        public ActionResult Index()
        {

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;


            if (roles == "Admin")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.ClientId;
            }
            return View();
        }

        // GET: Configure/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Configure/Create
        [ValidateInput(false)]
        public ActionResult Create(int? ClientId)
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var ConfigureList = (from b in _configureServices.GetAllConfigure()
                                 select new SelectListItem
                                 {
                                     Value = b.ClientId.ToString(),
                                     // Text = b.ClientName
                                 }).ToList();
            return View(new ConfigureViewModel
            {
                Configure = ConfigureList
            });
            // return View();
        }

        // POST: Configure/Create
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(ConfigureViewModel viewModel)
        {
            var roleUserId = User.Identity.GetUserId();
            // var roleUserId = viewModel.CurrentUserRole;
            //var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            // var roles = viewModel.CurrentUserRole;
            var projection = _clientAdminService.GetClientAdminById(roleUserId);
            var clientId = projection.ClientId;

            if (ModelState.IsValid)
            {
                var configure = new Configure
                {
                    name = viewModel.name,
                    ClientId = viewModel.ClientId,
                    address = viewModel.address,
                    email_id = viewModel.email_id,
                    confirmemail = viewModel.confirmemail,
                    emailpassword = viewModel.emailpassword,
                    username = viewModel.username,
                    aboutus = viewModel.aboutus,
                    sender_id = viewModel.sender_id,
                    password = viewModel.password

                };

                var result = _configureServices.Save(clientId, configure);
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

            viewModel = new ConfigureViewModel();

            return View(viewModel);

        }
        //public void ReturnViewModel(string roles, ConfigureViewModel viewModel, int clientId, string clientName)
        //{
        //    if (roles == "Admin")
        //    {
        //        var clientList = (from b in _clientAdminService.GetClients()
        //                          select new SelectListItem
        //                          {
        //                              Value = b.ClientId.ToString(),
        //                              Text = b.Name
        //                          }).ToList();
        //        viewModel.Clients = clientList;
        //        ViewBag.ClientId = null;
        //    }
        //    else if (roles == "Client")
        //    {
        //        viewModel.ClientId = clientId;
        //        viewModel.ClientName = clientName;
        //    }

        //    viewModel.CurrentUserRole = roles;
        //}

        // GET: Configure/Edit/5
        public ActionResult Edit(int id)
        {
            ViewBag.configureList = (from b in _configureServices.GetAllConfigure()
                                     select new SelectListItem
                                     {
                                         Value = b.ConfigureId.ToString(),
                                         Text = b.name
                                     }).ToList();

            var projection = _configureServices.GetConfById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Configure not Exists {0}.", projection.name));
                Warning("Configure does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.Id = projection.ConfigureId;

            var viewModel = AutoMapper.Mapper.Map<ConfigureProjection, Configure>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(ConfigureEditViewModel viewModel)
        {
            ViewBag.configureList = (from b in _configureServices.GetAllConfigure()
                                     select new SelectListItem
                                     {
                                         Value = b.ConfigureId.ToString(),
                                         Text = b.name
                                     }).ToList();

            ViewBag.ConfigureId = viewModel.ConfigureId;

            if (ModelState.IsValid)
            {
                var student = _repository.Project<Configure, bool>(users => (from u in users where u.ConfigureId == viewModel.ConfigureId select u).Any());

                if (!student)
                {
                    _logger.Warn(string.Format("Configure does not exists '{0}'.", viewModel.name));
                    Danger(string.Format("Configure does not exists '{0}'.", viewModel.name));
                    return RedirectToAction("Index");
                }

                var result = _configureServices.Update(new Configure
                {
                    ConfigureId = viewModel.ConfigureId,
                    name = viewModel.name,
                    address = viewModel.address,
                    email_id = viewModel.email_id,
                    confirmemail=viewModel.confirmemail,
                    emailpassword = viewModel.emailpassword,
                    username = viewModel.username,
                    aboutus = viewModel.aboutus,
                    sender_id = viewModel.sender_id,
                    password = viewModel.password

                });
                if (result.Success)
                {
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
            return View(viewModel);
        }

    }
}

