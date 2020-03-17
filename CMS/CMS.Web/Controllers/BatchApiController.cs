using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class BatchApiController : ApiController
    {
        readonly IBatchService _batchService;

        public BatchApiController(IBatchService batchService)
        {
            _batchService = batchService;
        }

        public HttpResponseMessage Get()
        {
            var batches = _batchService.GetAllBatches();

            return Request.CreateResponse(HttpStatusCode.OK, batches);
        }
    }
}
