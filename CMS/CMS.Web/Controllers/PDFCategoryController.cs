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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class PDFCategoryController : BaseController
    {
        readonly IPDFCategoryService _pdfCategoryService;
        readonly ILogger _logger;
        readonly IRepository _repository;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IClientAdminService _clientAdminService;

        public PDFCategoryController(IClientAdminService clientAdminService, IPDFCategoryService pdfCategoryService, ILogger logger, IRepository repository, IEmailService emailService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService)
        {
            _pdfCategoryService = pdfCategoryService;
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
            if (roles == "Admin")
            {
                ViewBag.userId = 0;
                var pdfcotegories = _pdfCategoryService.GetPDFCategories().ToList();
                var viewModelList = AutoMapper.Mapper.Map<List<PDFCategoryProjection>, PDFCategoryViewModel[]>(pdfcotegories);
                return View(viewModelList);
            }
            else if (roles == "Client")
            {
                ViewBag.userId = projection.ClientId;
                var pdfcotegories = _pdfCategoryService.GetPDFCategoriesByClientId(projection.ClientId).ToList();
                var viewModelList = AutoMapper.Mapper.Map<List<PDFCategoryProjection>, PDFCategoryViewModel[]>(pdfcotegories);
                return View(viewModelList);
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

                return View(new PDFCategoryViewModel
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
                return View(new PDFCategoryViewModel
                {
                    CurrentUserRole = roles,
                    ClientId = projection.ClientId,
                    ClientName = projection.ClientName
                });
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PDFCategoryViewModel viewModel)
        {

            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;
            int ClientId = projection.ClientId;
            if (ModelState.IsValid)
            {
                var result = _pdfCategoryService.Save(ClientId, new PDFCategory { Name = viewModel.Name });
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - PDF Category Create";
                    var message = "PDF Category Created Successfully";
                    SendMailToAdmin(message, viewModel.Name, bodySubject);
                    Success(result.Results.FirstOrDefault().Message);
                    ModelState.Clear();
                    viewModel = new PDFCategoryViewModel();
                }
                else
                {
                    _logger.Warn(result.Results.FirstOrDefault().Message);
                    Warning(result.Results.FirstOrDefault().Message, true);
                }
            }
            viewModel = new PDFCategoryViewModel();
            return View(viewModel);
        }

        public ActionResult Edit(int id)
        {
            var projection = _pdfCategoryService.GetPDFCategoryById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("PDF Category does not Exists {0}.", id));
                Warning("PDF Category does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<PDFCategoryProjection, PDFCategoryViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PDFCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var pdfcs = _repository.Project<PDFCategory, bool>(pcs => (from b in pcs where b.PDFCategoryId == viewModel.PDFCategoryId select b).Any());
                if (!pdfcs)
                {
                    _logger.Warn(string.Format("PDF Category not exists '{0}'.", viewModel.Name));
                    Danger(string.Format("PDF Category not exists '{0}'.", viewModel.Name));
                }
                var result = _pdfCategoryService.Update(new PDFCategory { PDFCategoryId = viewModel.PDFCategoryId, Name = viewModel.Name });
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - PDF Category update";
                    var message = "PDF Category Updated Successfully";
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
            return View(viewModel);
        }

        public ActionResult Delete(int id)
        {
            var projection = _pdfCategoryService.GetPDFCategoryById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("PDF Category does not Exists {0}.", id));
                Warning("PDF Category does not Exists.");
                return RedirectToAction("Index");
            }
            var viewModel = AutoMapper.Mapper.Map<PDFCategoryProjection, PDFCategoryViewModel>(projection);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(PDFCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var result = _pdfCategoryService.Delete(viewModel.PDFCategoryId);
                if (result.Success)
                {
                    var bodySubject = "Web portal changes - PDF Category delete";
                    var message = "PDF Category deleted Successfully";
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