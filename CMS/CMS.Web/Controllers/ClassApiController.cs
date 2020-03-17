using CMS.Domain.Storage.Services;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class ClassApiController : ApiController
    {
        readonly IClassService _classService;
        readonly IStudentService _studentService;

        public ClassApiController(IClassService classService, IStudentService studentService)
        {
            _classService = classService;
            _studentService = studentService;
        }

        public HttpResponseMessage Get()
        {
            var classes = _classService.GetClasses();
           
            return Request.CreateResponse(HttpStatusCode.OK, classes);
        }

        public HttpResponseMessage GetClassesByMultipleBranch(string selectedBranch)
        {
            var classes = _studentService.GetClassesByMultipleBranchId(selectedBranch).Select(x => new { x.ClassId, x.ClassName });

            return Request.CreateResponse(HttpStatusCode.OK, classes);
        }
    }
}
