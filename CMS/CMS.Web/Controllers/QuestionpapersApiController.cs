using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class QuestionpapersApiController : ApiController
    {
        readonly IUploadQuestionpapersService _uploadQuestionpapersService;
        public QuestionpapersApiController(IUploadQuestionpapersService QuestionpapersService)
        {
            _uploadQuestionpapersService = QuestionpapersService;
        }

        [Route("Api/QuestionpapersApi/Get")]
        public HttpResponseMessage Get()
        {
            var Questionpapers = _uploadQuestionpapersService.GetUploadQuestionpapersList();
            return Request.CreateResponse(HttpStatusCode.OK, Questionpapers);
        }
        public HttpResponseMessage GetQuestionpapersById(int id)
        {
            var Questionpapers = _uploadQuestionpapersService.GetQuestionpapersById(id);
            return Request.CreateResponse(HttpStatusCode.OK, Questionpapers);
        }
    }
}
