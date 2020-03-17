using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class TextbooksApiController : ApiController
    {
        readonly IUploadTextbooksService _uploadTextbooksService;
        public TextbooksApiController(IUploadTextbooksService TextbooksService)
        {
            _uploadTextbooksService = TextbooksService;
        }

        [Route("Api/TextbooksApi/Get")]
        public HttpResponseMessage Get()
        {
            var Textbooks = _uploadTextbooksService.GetUploadTextbooksList();
            return Request.CreateResponse(HttpStatusCode.OK, Textbooks);
        }
        public HttpResponseMessage GetTextbooksById(int id)
        {
            var Textbooks = _uploadTextbooksService.GetTextbooksById(id);
            return Request.CreateResponse(HttpStatusCode.OK, Textbooks);
        }
    }
}
