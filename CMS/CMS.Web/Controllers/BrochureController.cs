using CMS.Common;
using CMS.Web.Logger;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using CMS.Web.ViewModels;
using System.Configuration;
using CMS.Web.Helpers;
using CMS.Domain.Storage.Services;
using Microsoft.AspNet.Identity;
using CMS.Web.Models;
using System.Threading;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class BrochureController : BaseController
    {
        readonly ILogger _logger;
        readonly IEmailService _emailService;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;

        public BrochureController(ILogger logger, IEmailService emailService, IAspNetRoles aspNetRolesService, IBranchAdminService branchAdminService)
        {
            _logger = logger;
            _emailService = emailService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
        }

        // GET: Brochure
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.IsFileExists = "no";
            string filename = Server.MapPath(ConfigurationManager.AppSettings["brochureFile"].ToString());
            if (System.IO.File.Exists(filename))
            {
                ViewBag.IsFileExists = "yes";
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BrochureViewModel viewModel)
        {
            var cmsResult = new CMSResult();
            string filename = Server.MapPath(ConfigurationManager.AppSettings["brochureFile"].ToString());
            ViewBag.FileName = (viewModel.FilePath != null) ? viewModel.FilePath.FileName : viewModel.FilePath.FileName;
            string folderPath = Server.MapPath(string.Concat("~/PDF/", Common.Constants.BrochureFolder));
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            if (viewModel.FilePath.ContentLength == 0)
            {
                cmsResult.Results.Add(new Result { Message = "Brochure is required", IsSuccessful = false });
                return View();
            }
            if (!Common.Constants.PdfType.Contains(viewModel.FilePath.ContentType))
            {
                cmsResult.Results.Add(new Result { Message = "Please choose pdf file.", IsSuccessful = false });
                _logger.Warn(cmsResult.Results.FirstOrDefault().Message);
                Warning(cmsResult.Results.FirstOrDefault().Message, true);
                return View();
            }
            if (viewModel.FilePath != null)
            {
                viewModel.FilePath.SaveAs(filename);
                cmsResult.Results.Add(new Result { Message = "Brochure save successfully.", IsSuccessful = true });
                #region email
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
                    body = body.Replace("{ModuleName}", "Brochure Name: " + ViewBag.FileName + "<br/> Brochure created successfully. ");
                    body = body.Replace("{BranchAdminEmail}", "( " + branchAdminEmail + " )");
                    var emailMessage = new MailModel
                    {
                        Body = body,
                        Subject = "Web portal changes Brochure",
                        IsBranchAdmin = true
                    };
                    _emailService.Send(emailMessage);
                }
                #endregion
                Success(cmsResult.Results.FirstOrDefault().Message);
            }
            ViewBag.IsFileExists = "no";
            if (System.IO.File.Exists(filename))
            {
                ViewBag.IsFileExists = "yes";
            }
            return View();
        }

        public FileResult DownloadBrochure()
        {
            string filename = Server.MapPath(ConfigurationManager.AppSettings["brochureFile"].ToString());

            var content_type = "";

            if (filename.Contains(".pdf"))
            {
                content_type = "application/pdf";
            }
            return File(filename, content_type, "BrochureFile.pdf");
        }
    }
}