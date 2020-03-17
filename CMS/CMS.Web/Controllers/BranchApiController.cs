using System.Web.Http;
using System.Net.Http;
using CMS.Domain.Storage.Services;
using System.Net;

namespace CMS.Web.Controllers
{
    public class BranchApiController : ApiController
    {
        readonly IBranchService _branchService;

        public BranchApiController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        public HttpResponseMessage Get()
        {
            var branchs = _branchService.GetAllBranches();
            return Request.CreateResponse(HttpStatusCode.OK, branchs);
        }
    }
}
