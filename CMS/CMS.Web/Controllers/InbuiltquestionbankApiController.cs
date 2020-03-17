using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class InbuiltquestionbankApiController : ApiController
    {
        readonly IUploadInbuiltquestionbankService _uploadInbuiltquestionbankService;
        public InbuiltquestionbankApiController(IUploadInbuiltquestionbankService InbuiltquestionbankService)
        {
            _uploadInbuiltquestionbankService = InbuiltquestionbankService;
        }

        [Route("Api/InbuiltquestionbankApi/Get")]
        public HttpResponseMessage Get()
        {
            var Inbuiltquestionbank = _uploadInbuiltquestionbankService.GetUploadInbuiltquestionbankList();
            return Request.CreateResponse(HttpStatusCode.OK, Inbuiltquestionbank);
        }
        public HttpResponseMessage GetInbuiltquestionbankById(int id)
        {
            var Inbuiltquestionbank = _uploadInbuiltquestionbankService.GetInbuiltquestionbankById(id);
            return Request.CreateResponse(HttpStatusCode.OK, Inbuiltquestionbank);
        }
    }
}
