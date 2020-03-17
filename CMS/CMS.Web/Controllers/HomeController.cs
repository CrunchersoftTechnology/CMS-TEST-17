using CMS.Web.Helpers;
using CMS.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEmailService _emailService;
        public HomeController(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
       
        [HttpPost]
        public ActionResult Run(FormCollection form)
        {
            Response.Write("<h1>Application Stared</h1>");

            var model = new MailModel { Body = "Test1", From = "", Subject = "Bulk Testing", To = "trupti.2193@gmail.com" };
            HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => _emailService.StartProcessing(new Models.MailModel[] { model, model, model, model, model, model, model, model, model, model, model, model, model, model, model }, cancellationToken));
            return Content("<h3>Background Task Started...</h3> <h1>Application Ended </h1>(Now You can close this application...)");
        }
    }
}