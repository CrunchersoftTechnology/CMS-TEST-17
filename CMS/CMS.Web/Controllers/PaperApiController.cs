using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CMS.Domain.Storage.Services;
using System;
using CMS.Common;
using CMS.Domain.Models;
using CMS.Web.Logger;

namespace CMS.Web.Controllers
{
    public class PaperApiController : ApiController
    {
        readonly ILocalDateTimeService _localDateTimeService;
        readonly ITestPaperService _testPaperService;
        readonly IQuestionService _questionService;
        readonly IArrangeTestResultService _arrangeTestService;
        readonly ILogger _logger;

        public PaperApiController(ILocalDateTimeService localDateTimeService, ITestPaperService testPaperService,
            IQuestionService questionService, IArrangeTestResultService arrangeTestService, ILogger logger)
        {
            _localDateTimeService = localDateTimeService;
            _testPaperService = testPaperService;
            _questionService = questionService;
            _arrangeTestService = arrangeTestService;
            _logger = logger;
        }

        [Route("Api/PaperApi/{Id}")]
        public HttpResponseMessage Get(string Id)
        {
            int testPaperId = Convert.ToInt32(Id);
            var currentDateTime = _localDateTimeService.GetDateTime();
            var projection = _testPaperService.GetPaperById(testPaperId);
            var listOfQuestionIds = JsonConvert.DeserializeObject<List<TestPaperQuestionsDetails>>(projection.DelimitedQuestionIds);
            var questionIds = listOfQuestionIds.Select(x => x.questionId).ToList();
            var questionDetails = _questionService.GetQuestionsDetailsForStudentAppOnlineTest(questionIds);

            foreach (var question in questionDetails)
            {
                if (question.Answer == "1")
                    question.Answer = "A";
                else if (question.Answer == "2")
                    question.Answer = "B";
                else if (question.Answer == "3")
                    question.Answer = "C";
                else if (question.Answer == "4")
                    question.Answer = "D";
            }

            var result = new
            {
                CurrentDateTime = currentDateTime,
                QuestionDetails = questionDetails
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("Api/GetAPi")]
        public HttpResponseMessage SubmitTest(TestResult testDetails)
        {
            try
            {
                CMSResult cmsResult = new CMSResult();
                var questions = JsonConvert.SerializeObject(testDetails.Questions);
                var result = _arrangeTestService.Save(new ArrangeTestResult
                {
                    UserId = testDetails.UserId,
                    TestPaperId = testDetails.TestPaperId,
                    TestDate = testDetails.TestDate,
                    TimeDuration = testDetails.TimeDuration,
                    StartTime = testDetails.StartTime,
                    Questions = questions,
                    ObtainedMarks = testDetails.ObtainedMarks,
                    OutOfMarks = testDetails.OutOfMarks
                });
                return Request.CreateResponse(HttpStatusCode.OK, "OK");
            }
            catch (Exception ex)
            {
                _logger.Warn(ex.Message);
                return Request.CreateResponse(HttpStatusCode.OK, "Error");
            }
        }
    }

    public class TestResult
    {
        public string UserId { get; set; }
        public int TestPaperId { get; set; }
        public DateTime TestDate { get; set; }
        public int TimeDuration { get; set; }
        public DateTime StartTime { get; set; }
        public List<QuestionDetails> Questions { get; set; }
        public int ObtainedMarks { get; set; }
        public int OutOfMarks { get; set; }
    }

    public class QuestionDetails
    {
        public string QuestionId { get; set; }
        public string CorrectAnswer { get; set; }
        public string StudentAnswer { get; set; }
        public string Status { get; set; }
    }
}
