using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class PracticepapersApiController : ApiController
    {
        readonly IUploadPracticepapersService _uploadPracticepapersService;
        public PracticepapersApiController(IUploadPracticepapersService PracticepapersService)
        {
            _uploadPracticepapersService = PracticepapersService;
        }

        [Route("Api/PracticepapersApi/Get")]
        public HttpResponseMessage Get()
        {
            var Practicepapers = _uploadPracticepapersService.GetUploadPracticepapersList();
            return Request.CreateResponse(HttpStatusCode.OK, Practicepapers);
        }
        public HttpResponseMessage GetPracticepapersById(int id)
        {
            var Practicepapers = _uploadPracticepapersService.GetPracticepapersById(id);
            return Request.CreateResponse(HttpStatusCode.OK, Practicepapers);
        }
    }
}
