using System.Web.Http;
using System.Net.Http;
using CMS.Domain.Storage.Services;
using System.Net;

namespace CMS.Web.Controllers
{
    public class ClientApiController : ApiController
    {
        readonly IClientService _clientService;

        public ClientApiController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public HttpResponseMessage Get()
        {
            var clients = _clientService.GetAllClients();
            return Request.CreateResponse(HttpStatusCode.OK, clients);
        }
    }
}
