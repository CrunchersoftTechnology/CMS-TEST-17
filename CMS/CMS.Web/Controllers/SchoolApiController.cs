using System.Web.Http;
using System.Net.Http;
using CMS.Domain.Storage.Services;
using System.Net;

namespace CMS.Web.Controllers
{
    public class SchoolApiController : ApiController
    {
        readonly ISchoolService _schoolService;
            
           public SchoolApiController(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public HttpResponseMessage GetSchool()
        {
            var school = _schoolService.GetAllSchools();
            return Request.CreateResponse(HttpStatusCode.OK, school);
        }
    }
}
