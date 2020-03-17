using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class TeacherApiController : ApiController
    {
        readonly ITeacherService _teacherService;

        public TeacherApiController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public HttpResponseMessage Get()
        {
            var teachers = _teacherService.GetTeachersForWebSite();
            return Request.CreateResponse(HttpStatusCode.OK, teachers);
        }
    }
}
