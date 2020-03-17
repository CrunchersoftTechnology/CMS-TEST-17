using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class AssignmentsApiController : ApiController
    {
        readonly IUploadAssignmentsService _uploadAssignmentsService;
        public AssignmentsApiController(IUploadAssignmentsService AssignmentsService)
        {
            _uploadAssignmentsService = AssignmentsService;
        }

        [Route("Api/AssignmentsApi/Get")]
        public HttpResponseMessage Get()
        {
            var Assignments = _uploadAssignmentsService.GetUploadAssignmentsList();
            return Request.CreateResponse(HttpStatusCode.OK, Assignments);
        }
        public HttpResponseMessage GetAssignmentsById(int id)
        {
            var Assignments = _uploadAssignmentsService.GetAssignmentsById(id);
            return Request.CreateResponse(HttpStatusCode.OK, Assignments);
        }
    }
}
