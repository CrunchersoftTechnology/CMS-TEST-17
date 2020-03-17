using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace CMS.Web.Controllers
{
    public class NotesApiController : ApiController
    {
        readonly IUploadNotesService _uploadNotesService;
        public NotesApiController(IUploadNotesService notesService)
        {
            _uploadNotesService = notesService;
        }

        [Route("Api/NotesApi/Get")]
        public HttpResponseMessage Get()
        {
            var notes = _uploadNotesService.GetUploadNotesList();
            return Request.CreateResponse(HttpStatusCode.OK, notes);
        }
        public HttpResponseMessage GetNotesById(int id)
        {
            var notes = _uploadNotesService.GetNotesById(id);
            return Request.CreateResponse(HttpStatusCode.OK, notes);
        }
    }
}
