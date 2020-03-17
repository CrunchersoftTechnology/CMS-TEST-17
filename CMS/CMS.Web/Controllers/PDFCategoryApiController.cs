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
    public class PDFCategoryApiController : ApiController
    {
        readonly ILocalDateTimeService _localDateTimeService;
        readonly IPDFCategoryService _pdfcategoryservice;
        readonly IPDFUploadService _pdfuploadservice;
        readonly ILogger _logger;

        public PDFCategoryApiController(ILocalDateTimeService localDateTimeService, IPDFCategoryService pdfcategoryservice,
         IPDFUploadService pdfuploadservice,   ILogger logger)
        {
            _localDateTimeService = localDateTimeService;
            _pdfcategoryservice = pdfcategoryservice;
            _pdfuploadservice = pdfuploadservice;
          
            _logger = logger;
        }

        [Route("Api/PDFCategoryApi/{Id}")]
        public HttpResponseMessage Get(int Id)
        {
            int pdfcategory = Convert.ToInt32(Id);
            var currentDateTime = _localDateTimeService.GetDateTime();
            //var projection = _testPaperService.GetPaperById(testPaperId);
            var projection = _pdfcategoryservice.GetPDFCategoryById(Id);
           // var listOfQuestionIds = JsonConvert.DeserializeObject<List<TestPaperQuestionsDetails>>(projection.DelimitedQuestionIds);
            var listOfpdfIds = JsonConvert.DeserializeObject<List<PdfcategoryDetails>>(projection.PDFCategoryId.ToString());
           // var questionIds = listOfQuestionIds.Select(x => x.questionId).ToList();
            var pdfIds = listOfpdfIds.Select(x => x.PDFCategoryId).ToList();
            //var questionDetails = _questionService.GetQuestionsDetailsForStudentAppOnlineTest(questionIds);
            var pdfDEtails = _pdfcategoryservice.GetPDFCategoryById(pdfIds);

            /*foreach (var question in pdfDEtails)
            {
                if (question.Answer == "1")
                    question.Answer = "A";
                else if (question.Answer == "2")
                    question.Answer = "B";
                else if (question.Answer == "3")
                    question.Answer = "C";
                else if (question.Answer == "4")
                    question.Answer = "D";
            }*/

            var result = new
            {
                CurrentDateTime = currentDateTime,
                PdfcategoryDetails = pdfDEtails
            };
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [Route("Api/GetAPi")]
        public HttpResponseMessage SubmitTest(pdfResult pdfresult)
        {
            try
            {
                CMSResult cmsResult = new CMSResult();
                var pdf = JsonConvert.SerializeObject(pdfresult.PDFCategoryId);
               
                var result = _pdfuploadservice.Save(new PDFUpload
                {
                    PDFCategoryId = pdfresult.PDFCategoryId,
                    FileName= pdfresult.Name,
                    Title=pdfresult.PDFCategoryName


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

    public class pdfResult
    {
        public int PDFCategoryId { get; set; }

        public string Name { get; set; }

        public int ClassId { get; set; }

        public int PDFUploadId { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

       // public bool IsVisible { get; set; }

        public string ClassName { get; set; }

        public string PDFCategoryName { get; set; }

       // public DateTime CreatedOn { get; set; }

      //  public bool IsSend { get; set; }
    }

    public class PdfcategoryDetails
    {
        public int PDFCategoryId { get; set; }

        public string Name { get; set; }

        public int ClassId { get; set; }

        public int PDFUploadId { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

       // public bool IsVisible { get; set; }

        public string ClassName { get; set; }

        public string PDFCategoryName { get; set; }

       // public DateTime CreatedOn { get; set; }

        //public bool IsSend { get; set; }
    }
}
