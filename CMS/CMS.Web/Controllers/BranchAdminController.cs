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
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
    public class BranchAdminController : BaseController
    {
        readonly IBranchService _branchService;
        readonly IBranchAdminService _branchAdminService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IApplicationUserService _applicationUserService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly ILocalDateTimeService _localDateTimeService;
        readonly IClientAdminService _clientAdminService;
        readonly IConfigureServices _configureService;
        readonly ISmsService _smsService;
        

        public BranchAdminController(IClientAdminService clientAdminService, ILogger logger, IRepository repository, 
            IApplicationUserService applicationUserService, IBranchAdminService branchAdminService, IConfigureServices configureServices,
            IEmailService emailService, IBranchService branchService, IAspNetRoles aspNetRolesService, ISmsService smsService,
            ILocalDateTimeService localDateTimeService)
        {
            _logger = logger;
            _repository = repository;
            _applicationUserService = applicationUserService;
            _branchAdminService = branchAdminService;
            _emailService = emailService;
            _branchService = branchService;
            _aspNetRolesService = aspNetRolesService;
            _localDateTimeService = localDateTimeService;
            _clientAdminService = clientAdminService;
            _configureService = configureServices;
            _smsService = smsService;
        }

        public ActionResult Index()
        {
            //var branchAdmins = _branchAdminService.GetBranches().ToList();
            //var viewModelList = AutoMapper.Mapper.Map<List<BranchAdminProjection>, BranchAdminEditViewModel[]>(branchAdmins);
            return View();
        }

        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create()
        {
            var configureMail = (from c in _configureService.GetAllConfigure()
                              select new SelectListItem
                              {
                                Text=c.email_id

                              }).ToList();
            int flag = 0;
            if (configureMail.Count == 0)
            {
                flag = 1;
            }
             else
            {
                flag = 0;
            }
            ViewBag.Message = flag;
            

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;


            var branchList = (from b in _branchService.GetAllBranches()
                              select new SelectListItem
                              {
                                  Value = b.BranchId.ToString(),
                                  Text = b.Name
                              }).ToList();
            return View(new BranchAdminViewModel
            {

                Branches = branchList
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(BranchAdminViewModel viewModel)
        {


            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

            if (ModelState.IsValid)
            {
                var localTime = (_localDateTimeService.GetDateTime());
                var user = new ApplicationUser();
                user.UserName = viewModel.Email;
                user.Email = viewModel.Email.Trim();
                user.CreatedBy = User.Identity.Name;
                user.CreatedOn = localTime;
                user.PhoneNumber = viewModel.ContactNo.Trim();
                user.BranchAdmin = new BranchAdmin
                {
                    ClientId=projection.ClientId,
                    CreatedBy = User.Identity.Name,
                    CreatedOn = localTime,
                    BranchId = viewModel.BranchId,
                    Name = viewModel.Name.Trim(),
                    Active = viewModel.Active,
                    ContactNo = viewModel.ContactNo

                };

                string userPassword = PasswordHelper.GeneratePassword();

                var result = _applicationUserService.SaveBranchAdmin(user, userPassword);

                if (result.Success)
                {
                    string message ="User Name :"+ viewModel.Email + "<br/>Password : " + userPassword + "<br/>Branch Admin created successfully";
                    string txtmessage = "User Name :" + viewModel.Email + ", Password : " + userPassword + "\n Branch Admin created successfully";
                    string contact = viewModel.ContactNo;
                    string email = viewModel.Email;
                    SendMailDynamic(message, email);

                    DynamicSendSMS(txtmessage, contact); 
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new BranchAdminViewModel();
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
            var branchList = (from b in _branchService.GetAllBranches()
                              select new SelectListItem
                              {
                                  Value = b.BranchId.ToString(),
                                  Text = b.Name
                              }).ToList();
            viewModel.Branches = branchList;
            return View(viewModel);
        }

        public ActionResult Edit(string id)
        {
            ViewBag.branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

            var projection = _branchAdminService.GetBranchAdminById(id);

            if (projection == null)
            {
                _logger.Warn(string.Format("Branch Admin not Exists {0}.", projection.Name));
                Warning("Branch Admin does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.BranchId = projection.BranchId;

            var viewModel = AutoMapper.Mapper.Map<BranchAdminProjection, BranchAdminEditViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(BranchAdminEditViewModel viewModel)
        {
            ViewBag.branchList = (from b in _branchService.GetAllBranches()
                                  select new SelectListItem
                                  {
                                      Value = b.BranchId.ToString(),
                                      Text = b.Name
                                  }).ToList();

            ViewBag.BranchId = viewModel.BranchId;

            if (ModelState.IsValid)
            {
                var student = _repository.Project<BranchAdmin, bool>(users => (from u in users where u.UserId == viewModel.UserId select u).Any());

                if (!student)
                {
                    _logger.Warn(string.Format("Branch Admin does not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("Branch Admin does not exists '{0}'.", viewModel.Name));
                    return RedirectToAction("Index");
                }

                var result = _branchAdminService.Update(new BranchAdmin
                {
                    BranchId = viewModel.BranchId,
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
            var projection = _branchAdminService.GetBranchAdminById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Branch Admin does not Exists {0}.", id));
                Warning("Branch Admin does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<BranchAdminProjection, BranchAdminDeleteViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Delete(BranchAdminDeleteViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                var result = _branchAdminService.Delete(viewModel.UserId);
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
            var branchAdmins = _branchAdminService.GetBranchAdminById(id);
            var viewModel = AutoMapper.Mapper.Map<BranchAdminProjection, BranchAdminEditViewModel>(branchAdmins);
            return View(viewModel);
        }

        //public void SendMailToAdmin(string message, string email)
        //{
        //    string body = string.Empty;
        //    using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/TeacherAndBranchAdminMailDesign.html")))
        //    {
        //        body = reader.ReadToEnd();
        //    }
        //    body = body.Replace("{BranchName}", User.Identity.GetUserName() + "(" + "Master Admin" + ")");
        //    body = body.Replace("{UserName}", message);
        //    var emailMessage = new MailModel
        //    {
        //        Body = body,
        //        Subject = "Branch Admin Registration",
        //        To = email
        //    };
        //    _emailService.Send(emailMessage);
        //}

        public void SendMailDynamic(string message, string email)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/BranchAdminMailDesign.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{BranchName}", User.Identity.GetUserName() + "(" + "Master Admin" + ")");
            body = body.Replace("{UserName}", message);

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var ClientId = projection.ClientId;

            var configure = _configureService.GetConfigureById(ClientId);
            
            var emailid = configure.email_id;
            var password = configure.emailpassword;

            var emailMessage = new MailModel
            {
                Body = body,
                Subject = "Branch Admin Registration",
                To = email,
                Emailid = emailid,
                Password = password


            };
            _emailService.MailSend(emailMessage);
        }

        public CMSResult DynamicSendSMS(string txtmessage, string contact)
        {
            CMSResult cmsresult = new CMSResult();
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);

            var projectionClient = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            var ClientId = projectionClient.ClientId;

            var projection = _configureService.GetConfigureById(ClientId);
            var clientId = projection.ClientId;
            var senderid = projection.sender_id;
            var username = projection.username;
            var password = projection.password;
            var name = projection.name;

            if (roles == "BranchAdmin" || roles == "Client")
            {
                //userRole = branchName + " ( " + User.Identity.GetUserName() + " ) ";
                // isBranchAdmin = true;
              
                    var smsModel = new SmsModel
                    {
                        SenderId = senderid,
                        UserName = username,
                        Password = password,
                        Name=name,
                     

                        Message ="Welcome To "+name+","+txtmessage,
                        SendTo = contact
                    };
                    
                    var sendparentsms = _smsService.DynamicsSendMessage(smsModel);
                    cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });
                
            }
            else
            {
                    var smsModel = new SmsModel
                    {

                        Message =txtmessage,
                        SendTo = contact
                    };
                
                    var sendparentsms = _smsService.SendMessage(smsModel);
                    cmsresult.Results.Add(new Result { Message = sendparentsms.Results[0].Message, IsSuccessful = sendparentsms.Results[0].IsSuccessful });
                
            }
            return cmsresult;

        }
    }
}