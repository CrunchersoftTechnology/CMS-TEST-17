using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Services;
using CMS.Web.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class StudentApiController : ApiController
    {
        readonly ILogger _logger;
        readonly IStudentService _studentService;
        readonly IRepository _repository;
        public StudentApiController(ILogger logger, IStudentService studentService, IRepository repository)
        {
            _logger = logger;
            _studentService = studentService;
            _repository = repository;
        }

        [HttpGet]
        public HttpResponseMessage SetPlayerIdCMSStudentAppData(string userId, string playerId)
        {
            var student = _repository.Load<Student>(x => x.UserId == userId);
            var result = "not found";
            if (student!=null)
            {
                if(student.studentAppPlayerId==string.Empty)
                {
                    result = "already set";
                }
                else
                {
                    result = "set";
                }
                student.studentAppPlayerId = playerId;

            }
            var list= _repository.LoadList<Student>(x => x.UserId != userId && x.studentAppPlayerId == playerId);

            foreach (var std in list)
            {
                std.studentAppPlayerId = string.Empty;
            }
            //_repository.SqlQuery<int>("UPDATE Students SET [StudentAppPlayerId]='' WHERE [StudentAppPlayerId]='"+ playerId + "' AND [UserId] <> '"+ userId + "'; ");
            
            return Request.CreateResponse(HttpStatusCode.OK, new { result });
        }



        [HttpGet]
        public HttpResponseMessage SetPlayerIdCMSParentAppData(string userId, string playerId)
        {
            var student = _repository.Load<Student>(x => x.UserId == userId);
            var result = "not found";
            if (student != null)
            {
                if (student.parentAppPlayerId == string.Empty)
                {
                    result = "already set";
                }
                else
                {
                    result = "set";
                }
                student.parentAppPlayerId = playerId;

            }
            var list = _repository.LoadList<Student>(x => x.UserId != userId && x.parentAppPlayerId == playerId);

            foreach (var std in list)
            {
                std.parentAppPlayerId = string.Empty;
            }
            //_repository.SqlQuery<int>("UPDATE Students SET [StudentAppPlayerId]='' WHERE [StudentAppPlayerId]='"+ playerId + "' AND [UserId] <> '"+ userId + "'; ");

            return Request.CreateResponse(HttpStatusCode.OK, new { result });
        }






    }
}
