using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System;
using CMS.Web.Logger;
using CMS.Web.Models;

namespace CMS.Web.Controllers
{
    public class StudentFeedbackApiController : ApiController
    {
        readonly IStudentFeedbackService _studentFeedbackService;
        readonly ILogger _logger;

        public StudentFeedbackApiController(IStudentFeedbackService studentFeedbackService, ILogger logger)
        {
            _studentFeedbackService = studentFeedbackService;
            _logger = logger;
        }
        public HttpResponseMessage Get()
        {
            var studentFeedback = _studentFeedbackService.GetStudentFeedback();

            return Request.CreateResponse(HttpStatusCode.OK, studentFeedback);
        }

        [Route("Api/StudentFeedbackApi/Post")]
        public HttpResponseMessage Post(StudentFeedbackData Data)
        {
            try
            {
                var result = _studentFeedbackService.UpdateMultipleFeedback(Data.SelectedFeedback, Data.Status);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString() +"student feedback post");
            }
            _logger.Info("Feedback Successfully Updated.");
            return Request.CreateResponse(HttpStatusCode.OK, "post successfully");
        }
    }
}
