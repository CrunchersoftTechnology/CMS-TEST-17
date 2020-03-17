using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Services;
using CMS.Web.CustomAttributes;
using CMS.Web.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    [Roles(Common.Constants.AdminRole + "," + Common.Constants.ClientAdminRole)]
    public class ClearPunchIdController : BaseController
    {
        readonly ILogger _logger;
        readonly IStudentService _studentService;

        public ClearPunchIdController(ILogger logger, IStudentService studentService)
        {
            _logger = logger;
            _studentService = studentService;
        }
        public ActionResult Create()
        {
            return View();
        }
       public JsonResult ClearPunchId()
        {
            var cmsResult = new CMSResult();
            var result = _studentService.ClearPunchId();
            if (result.Success)
            {
                cmsResult.Results.Add(new Result { Message = result.Results.FirstOrDefault().Message, IsSuccessful = true });
            }
            else
            {
                cmsResult.Results.Add(new Result { Message ="There is problem for clear punch Id", IsSuccessful = false });
            }
           return Json(cmsResult, JsonRequestBehavior.AllowGet);
        }
    }
}