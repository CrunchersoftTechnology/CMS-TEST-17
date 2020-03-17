using CMS.Common;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Authorize(Roles = Common.Constants.AdminRole + "," + Common.Constants.BranchAdminRole + "," + Common.Constants.StudentRole + "," + Common.Constants.ClientAdminRole)]
    public class AdminController : BaseController
    {
        readonly IClientService _clientService;
        readonly IBranchService _branchService;
        readonly IBatchService _batchService;
        readonly ITeacherService _teacherService;
        readonly IStudentService _studentService;
        readonly ILogger _logger;
        readonly IAspNetRoles _aspNetRolesService;
        readonly IApiService _apiService;
        readonly IClientAdminService _clientAdminService;
        readonly IBranchAdminService _branchAdminService;

        public AdminController(IBranchAdminService branchAdminService, IClientAdminService clientAdminService,IClientService clientService, IBranchService branchService,
            IBatchService batchService,
            ITeacherService teacherService,
            IStudentService studentService,
            ILogger logger, IAspNetRoles aspNetRolesService,IApiService apiService)
        {
            _clientService = clientService;
            _branchService = branchService;
            _batchService = batchService;
            _teacherService = teacherService;
            _studentService = studentService;
            _logger = logger;
            _aspNetRolesService = aspNetRolesService;
            _apiService = apiService;
            _clientAdminService = clientAdminService;
            _branchAdminService = branchAdminService;
        }
        // GET: Admin
        //[Route("adminLogin")]
        public ActionResult Index()
        {


            var roleUserId = User.Identity.GetUserId();
            var roles = _aspNetRolesService.GetCurrentUserRole(roleUserId);
            //var projection = roles == "Client" ? _clientAdminService.GetClientAdminById(roleUserId) : null;

            if (roles == "Admin")
            {

                var summaryModel = new AdminSummaryViewModel
                {
                    BatchesCount = _batchService.GetBatchesCount(),
                    BranchesCount = _branchService.GetBranchesCount(),
                    ClientsCount = _clientService.GetClientsCount(),
                    StudentsCount = _studentService.GetStudentsCount(),
                    TeachersCount = _teacherService.GetTeachersCount(),
                    //PendingAdmissionCount = _apiService.GetPendingAdmission(),
                    CurrentUserRole = roles

                };
                ViewBag.CurrentUserRole = roles;
                return View(summaryModel);

            }
            else if (roles == "Client")
            {
                var projection = _clientAdminService.GetClientAdminById(roleUserId);

                var summaryModel = new AdminSummaryViewModel
                {
                    BatchesCount = _batchService.GetBatchesCountByClientId(projection.ClientId),
                    BranchesCount = _branchService.GetBranchesCountByClientId(projection.ClientId),
                    //ClientsCount = _clientService.GetClientsCountByClientId(projection.ClientId),
                    StudentsCount = _studentService.GetStudentsCountByClientId(projection.ClientId),
                    TeachersCount = _teacherService.GetTeachersCountByClientId(projection.ClientId),
                    //PendingAdmissionCount = _apiService.GetPendingAdmissionByClientId(projection.ClientId),
                    CurrentUserRole = roles
                };
                ViewBag.CurrentUserRole = roles;
                return View(summaryModel);
            }
            else if (roles == "BranchAdmin")
            {
                var summaryModel = new AdminSummaryViewModel
                {
                    BatchesCount = _batchService.GetBatchesCount(),
                    BranchesCount = _branchService.GetBranchesCount(),
                    ClientsCount = _clientService.GetClientsCount(),
                    StudentsCount = _studentService.GetStudentsCount(),
                    TeachersCount = _teacherService.GetTeachersCount(),
                    //PendingAdmissionCount = _apiService.GetPendingAdmission(),
                    CurrentUserRole = roles

                };
                ViewBag.CurrentUserRole = roles;
                return View(summaryModel);

            }
            else if (roles == "Student")
            {
                return RedirectToAction("Index", "StudentDashBoard");
            }

            return View();
        }

        public ActionResult PendingAdmissionList()
        {
            var admissionList = _apiService.GetPendingAdmissionList();
            var admission = JsonConvert.DeserializeObject<List<PendingStudentAdmissionProjection>>(admissionList);
            var viewModelList = AutoMapper.Mapper.Map<List<PendingStudentAdmissionProjection>, PendingStudentAdmissionViewModel[]>(admission);
            return View(viewModelList);
        }
    }
}