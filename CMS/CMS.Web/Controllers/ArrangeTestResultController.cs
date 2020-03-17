using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.ClientAdminRole)]
    public class ArrangeTestResultController : BaseController
    {
        readonly IArrangeTestResultService _arrangeTestResult;
        readonly ITestPaperService _testPaperService;
        readonly ILogger _logger;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IBranchAdminService _branchAdminService;

        public ArrangeTestResultController(IArrangeTestResultService arrangeTestResult, ILogger logger,
            ITestPaperService testPaperService, IAspNetRoles aspNetRolesService,
            IBranchAdminService branchAdminService)
        {
            _arrangeTestResult = arrangeTestResult;
            _logger = logger;
            _testPaperService = testPaperService;
            _aspNetRolesService = aspNetRolesService;
            _branchAdminService = branchAdminService;
        }

        // GET: ArrangeTestResult
        public ActionResult Index()
        {
            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            var projection = roles == "BranchAdmin" ? _branchAdminService.GetBranchAdminById(roleUserId) : null;
            if (roles == "Admin" || roles=="Client")
            {
                ViewBag.userId = 0;
            }
            else
            {
                ViewBag.userId = projection.BranchId;
            }
            var classesList = (from classes in _testPaperService.GetTestPapersClasses().ToList()
                               group classes by new
                               {
                                   classes.ClassId,
                                   classes.ClassName
                               } into grouping
                               select new TestPaperProjection
                               {
                                   ClassId = grouping.Key.ClassId,
                                   ClassName = grouping.Key.ClassName
                               }).ToList();

            ViewBag.ClassList = (from c in classesList
                                 select new SelectListItem
                                 {
                                     Value = c.ClassId.ToString(),
                                     Text = c.ClassName
                                 }).ToList();
            return View();
        }

        public ActionResult Details(int id)
        {
            var projection = _arrangeTestResult.GetArrangeTestResultById(id);
            if (projection == null)
            {
                _logger.Warn(string.Format("Arrange test paper result does not Exists {0}.", id));
                Warning("Batch does not Exists.");
                return RedirectToAction("Index");
            }

            ViewBag.QuestionsDetails = JsonConvert.DeserializeObject<List<QuestionDetails>>(projection.Questions);
            var viewModel = AutoMapper.Mapper.Map<ArrangeTestResultProjection, ArrangeTestResultViewModel>(projection);
            return View(viewModel);
        }
    }
}