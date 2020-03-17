using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class ReferencebooksApiController : ApiController
    {
        readonly IUploadReferencebooksService _uploadReferencebooksService;
        public ReferencebooksApiController(IUploadReferencebooksService ReferencebooksService)
        {
            _uploadReferencebooksService = ReferencebooksService;
        }

        [Route("Api/ReferencebooksApi/Get")]
        public HttpResponseMessage Get()
        {
            var Referencebooks = _uploadReferencebooksService.GetUploadReferencebooksList();
            return Request.CreateResponse(HttpStatusCode.OK, Referencebooks);
        }
        public HttpResponseMessage GetReferencebooksById(int id)
        {
            var Referencebooks = _uploadReferencebooksService.GetReferencebooksById(id);
            return Request.CreateResponse(HttpStatusCode.OK, Referencebooks);
        }
    }
}
