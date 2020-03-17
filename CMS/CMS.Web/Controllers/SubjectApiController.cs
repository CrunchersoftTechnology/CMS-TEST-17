using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class SubjectApiController : ApiController
    {
        readonly ILogger _logger;
        readonly ISubjectService _subjectService;

        public SubjectApiController(ILogger logger, ISubjectService subjectService)
        {
            _logger = logger;
            _subjectService = subjectService;
        }

        [Route("Api/SubjectApi/{ClassId}")]
       public HttpResponseMessage Get(string ClassId)
        {
            var subjects = _subjectService.GetSubjects(Convert.ToInt16(ClassId));
            return Request.CreateResponse(HttpStatusCode.OK, subjects);
        }
    }
}
