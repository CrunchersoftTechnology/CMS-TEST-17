using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using System.Web.Mvc;
using System.Linq;
using CMS.Domain.Models;
using System;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;
using CMS.Web.Models;
using System.Configuration;
using System.IO;
using Microsoft.AspNet.Identity;
using System.Threading;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole)]
    public class ClientAdminController : BaseController
    {
        readonly IClientService _clientService;
        readonly IClientAdminService _clientAdminService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IApplicationUserService _applicationUserService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly ILocalDateTimeService _localDateTimeService;
        readonly ISmsService _smsService;

        public ClientAdminController(ILogger logger, IRepository repository,
            IApplicationUserService applicationUserService, IClientAdminService clientAdminService,
            IEmailService emailService, IClientService clientService, IAspNetRoles aspNetRolesService,
            ILocalDateTimeService localDateTimeService, ISmsService smsService)
        {
            _logger = logger;
            _repository = repository;
            _applicationUserService = applicationUserService;
            _clientAdminService = clientAdminService;
            _emailService = emailService;
            _clientService = clientService;
            _aspNetRolesService = aspNetRolesService;
            _localDateTimeService = localDateTimeService;
            _smsService = smsService;
        }

        public ActionResult Index()
        {
            var clientAdmins = _clientAdminService.GetClients().ToList();
            var viewModelList = AutoMapper.Mapper.Map<List<ClientAdminProjection>, ClientAdminEditViewModel[]>(clientAdmins);
            return View();
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create()
        {
            var ClientList = (from b in _clientService.GetAllClients()
                              select new SelectListItem
                              {
                                  Value = b.ClientId.ToString(),
                                  Text = b.ClientName
                              }).ToList();
            return View(new ClientAdminViewModel
            {
                Clients = ClientList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(ClientAdminViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var localTime = (_localDateTimeService.GetDateTime());
                var user = new ApplicationUser();
                user.UserName = viewModel.Email;
                user.Email = viewModel.Email.Trim();
                user.CreatedBy = User.Identity.Name;
                user.CreatedOn = localTime;
                user.PhoneNumber = viewModel.ContactNo.Trim();
                user.ClientAdmin = new ClientAdmin
                {
                    CreatedBy = User.Identity.Name,
                    CreatedOn = localTime,
                    ClientId = viewModel.ClientId,
                    Name = viewModel.Name.Trim(),
                    Active = viewModel.Active,
                    ContactNo = viewModel.ContactNo
                };

                string userPassword = PasswordHelper.GeneratePassword();

                var result = _applicationUserService.SaveClientAdmin(user, userPassword);

                if (result.Success)
                {
                    string message = "User Name :" + viewModel.Email + "<br/>Password : " + userPassword + "<br/>Client Admin Created Successfully In CruncherSoft Technology.";
                    string txtmessage = "User Name :" + viewModel.Email + ", Password : " + userPassword + "\n Client Admin Created Successfully In CruncherSoft Technology.";
                    string contact = viewModel.ContactNo;
                    SendSMS(txtmessage, contact);
                    string email = viewModel.Email;
                    SendMailToAdmin(message, email);  
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new ClientAdminViewModel();
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
            var clientList = (from b in _clientService.GetAllClients()
                              select new SelectListItem
                              {
                                  Value = b.ClientId.ToString(),
                                  Text = b.ClientName
                              }).ToList();
            viewModel.Clients = clientList;
            return View(viewModel);
        }

        public ActionResult Edit(string id)
        {
            ViewBag.clientList = (from b in _clientService.GetAllClients()
                                  select new SelectListItem
                                  {
                                      Value = b.ClientId.ToString(),
                                      Text = b.ClientName
                                  }).ToList();

            var projection = _clientAdminService.GetClientAdminById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Clients Admin not Exists {0}.", projection.Name));
                Warning("Clients Admin does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.ClientId = projection.ClientId;

            var viewModel = AutoMapper.Mapper.Map<ClientAdminProjection, ClientAdminEditViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(ClientAdminEditViewModel viewModel)
        {
            ViewBag.clientList = (from b in _clientService.GetAllClients()
                                  select new SelectListItem
                                  {
                                      Value = b.ClientId.ToString(),
                                      Text = b.ClientName
                                  }).ToList();

            ViewBag.ClientId = viewModel.ClientId;

            if (ModelState.IsValid)
            {
                var student = _repository.Project<ClientAdmin, bool>(users => (from u in users where u.UserId == viewModel.UserId select u).Any());

                if (!student)
                {
                    _logger.Warn(string.Format("Client Admin does not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Client Admin does not exists '{0}'.", viewModel.Name));
                    return RedirectToAction("Index");
                }

                var result = _clientAdminService.Update(new ClientAdmin
                {
                    ClientId = viewModel.ClientId,
                    ContactNo = viewModel.ContactNo,
                    Name = viewModel.Name,
                    Active = viewModel.Active,
                    UserId = viewModel.UserId
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

        public ActionResult Delete(string id)
        {
            var projection = _clientAdminService.GetClientAdminById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Client Admin does not Exists {0}.", id));
                Warning("Client Admin does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<ClientAdminProjection, ClientAdminDeleteViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Delete(ClientAdminDeleteViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var result = _clientAdminService.Delete(viewModel.UserId);
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

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Details(string id)
        {
            var clientAdmins = _clientAdminService.GetClientAdminById(id);
            var viewModel = AutoMapper.Mapper.Map<ClientAdminProjection, ClientAdminEditViewModel>(clientAdmins);
            return View(viewModel);

        }
        public void SendMailToAdmin(string message, string email)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/ClientAdminRegMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{ClientName}", User.Identity.GetUserName() + "(" + "Master Admin" + ")");
            body = body.Replace("{UserName}", message);
            var emailMessage = new MailModel
            {
                Body = body,
                Subject = "Client Admin Registration",
                To = email,
                IsClientAdmin = true
            };
            _emailService.Send(emailMessage);
        }

        public void SendSMS(string message, string contact)
        {

            var smsModel = new SmsModel
            {
                Message = message,
                SendTo = contact
            };
            var smsReg = _smsService.SendMessage(smsModel);
            //return cmsresult;

        }
    }
}