using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class BoardApiController : ApiController
    {
        readonly ILogger _logger;
        readonly IBoardService _boardService;

        public BoardApiController(ILogger logger, IBoardService boardService)
        {
            _logger = logger;
            _boardService = boardService;
        }

        public HttpResponseMessage Get()
        {
            var boards = _boardService.GetBoards();
            return Request.CreateResponse(HttpStatusCode.OK, boards);
        }
    }
}
