using CMS.Domain.Models;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{

    public class AnnouncementController : BaseController
    {
        readonly IAnnouncementService _announcementService;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;
        readonly IClientAdminService _clientAdminService;

        public AnnouncementController(IAnnouncementService announcementService, IEmailService emailService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService, IClientAdminService clientAdminService)
        {
            _announcementService = announcementService;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
            _clientAdminService = clientAdminService;
        }

        // GET: Announcement
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Index()
        {
            var list = _announcementService.GetAllAnnouncements(0);
            return View(list);
        }

        // GET: Announcement/Create
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Announcement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Create(Announcement model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _announcementService.Save(model);
                    if (result)
                    {
                        string bodySubject = "Web portal changes -  Announcement Details create";
                        var message = "Announcement Details :" + model.AnnouncementDetails + "<br/>Announcement Url :" + model.Url + "<br/>Announcement created successfully";
                        SendMailToAdmin(message, bodySubject);
                        Success("Announcement added successfully!", true);
                        ModelState.Clear();
                        model = new Announcement();
                    }
                    else
                    {
                        Danger("Something went wrong! Please try again.", true);
                        return View(model);
                    }
                    // return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // GET: Announcement/Edit/5
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(int id)
        {
            var model = _announcementService.GetAnnouncement(id);
            return View(model);
        }

        // POST: Announcement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Edit(Announcement model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = _announcementService.Update(model);
                    if (result)
                    {
                        string bodySubject = "Web portal changes - Announcement Details update";
                        var message = "Announcement Details :" + model.AnnouncementDetails + "<br/>Announcement Url :" + model.Url + "<br/>Announcement updated successfully";
                        SendMailToAdmin(message, bodySubject);
                        Success("Announcement updated successfully!", true);
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Danger("Something went wrong! Please try again.", true);
                        return View(model);
                    }
                    //return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View();
            }
        }

        // GET: Announcement/Delete/5
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult Delete(int id)
        {
            var model = _announcementService.GetAnnouncement(id);
            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
        public ActionResult DeleteAnnouncement(int id, Announcement model)
        {
            try
            {
                var result = _announcementService.Delete(id);
                if (result)
                {
                    string bodySubject = "Web portal changes - Announcement Details delete";
                    var message = "Announcement Details :" + model.AnnouncementDetails + "<br/>Announcement Url :" + model.Url + "<br/>Announcement deleted successfully";
                    SendMailToAdmin(message, bodySubject);
                    Success("Announcement updated successfully!", true);
                }
                else
                {
                    Danger("Something went wrong! Please try again.", true);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        [AllowAnonymous]
        public ActionResult GetAllAnnouncement()
        {
            var result = _announcementService.GetAllAnnouncements(Convert.ToInt32(ConfigurationManager.AppSettings["AnnounmentDisplayCount"]));


            return PartialView("_AnnouncementPartial", result);
        }
        public void SendMailToAdmin(string message, string bodySubject)
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
                body = body.Replace("{ModuleName}", message);
                body = body.Replace("{BranchAdminEmail}", "( " + branchAdminEmail + " )");
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsBranchAdmin = true
                };
                _emailService.Send(emailMessage);
            }

            if (roles == "ClientAdmin")
            {
                var clientAdmin = _clientAdminService.GetClientAdminById(roleUserId);
                var clientName = clientAdmin.ClientName;
                var clientAdminEmail = clientAdmin.Email;
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/MailDesign/CommonMailDesign.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("{BranchName}", clientName);
                body = body.Replace("{ModuleName}", message);
                body = body.Replace("{ClientAdminEmail}", "( " + clientAdminEmail + " )");
                var emailMessage = new MailModel
                {
                    Body = body,
                    Subject = bodySubject,
                    IsClientAdmin = true
                };
                _emailService.Send(emailMessage);
            }
        }
    }
}